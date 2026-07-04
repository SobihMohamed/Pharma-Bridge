using AutoMapper;
using Microsoft.AspNetCore.Http;
using PharmaBridge.Abstraction.IServices.Notification;
using PharmaBridge.Abstraction.IServices.Order;
using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
using PharmaBridge.Domain.Exceptions;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Domain.Models.User;
using PharmaBridge.Domain.Models.UserAccess;
using PharmaBridge.Services.Specifications.BidSpec;
using PharmaBridge.Services.Specifications.OrderSpec;
using PharmaBridge.Services.Specifications.PatientAddressSpec;
using PharmaBridge.Services.Specifications.PharmacySpec;
using PharmaBridge.Shared.Common.Pagination;
using PharmaBridge.Shared.Common.Params.Order;
using PharmaBridge.Shared.DTOs.Notificaiton;
using PharmaBridge.Shared.DTOs.Order;
using PharmaBridge.Shared.EnumHelper.PaymentEnums;
using PharmaBridge.Shared.EnumHelper.PharmaEnums;
using PharmaBridge.Shared.EnumHelper.UserAccessEnums;
using System.Security.Claims;
using PharmacyEntity = PharmaBridge.Domain.Models.Pharma_Requests.Pharmacy;

namespace PharmaBridge.Services.ServicesImplementation.OrderService
{
    public class OrderService(IUnitOfWork unitOfWork, IMapper mapper, INotificationService notificationService, IHttpContextAccessor httpContextAccessor) : IOrderService
    {

        private async Task<string> GetValidPatientProfileIdAsync(string applicationUserId)
        {
            var profileSpec = new PatientProfileByAppUserIdSpec(applicationUserId);
            var patientRepo = unitOfWork.GetRepository<PatientProfile, string>();
            var patientProfile = await patientRepo.GetByIdWithSpecAsync(profileSpec);

            if (patientProfile == null)
                throw new BadRequestCustomeException("Patient profile not found for this user.");

            return patientProfile.Id;
        }
        //System / Internal Operations

        public async Task<OrderDetailsDto> CreateOrderFromBidAsync(int winningBidId)
        {
            // 1 — Load the bid and validate it is ready to become an order
            var bid = await FetchValidatedBidAsync(winningBidId);

            // 2 — Construct the Order entity 
            var order = BuildOrderFromBid(bid);

            // 3 — Persist and commit
            await unitOfWork.GetRepository<Order, int>().AddAsync(order);

            var result = await unitOfWork.SaveChangesAsync();
            if (result <= 0)
                throw new BadRequestCustomeException("Failed to create order from bid.");

            // 4 — Refetch with all includes for the response DTO
            var saved = await FetchOrderWithDetailsOrThrowAsync(order.Id);
            return mapper.Map<OrderDetailsDto>(saved);

        }

        //Pharmacy (Provider) Operations

        public async Task<PaginationResponse<OrderDto>> GetPharmacyOrdersAsync(int? requestedPharmacyId, OrderQueryParams queryParams)
        {
            var actualPharmacyId = await ResolvePharmacyIdAsync(requestedPharmacyId);

            var orderRepo = unitOfWork.GetRepository<Order, int>();

            var dataSpec = new PharmacyOrdersSpecification(actualPharmacyId, queryParams);
            var countSpec = new PharmacyOrdersCountSpecification(actualPharmacyId, queryParams);

            var orders = await orderRepo.GetAllWithSpecAsync(dataSpec);
            var totalCount = await orderRepo.GetCountAsync(countSpec);

            var dtos = mapper.Map<IReadOnlyList<OrderDto>>(orders);

            return new PaginationResponse<OrderDto>(queryParams.PageIndex, queryParams.PageSize, totalCount, dtos);
        }

        public async Task<OrderDetailsDto> GetPharmacyOrderDetailsAsync(int orderId, int? requestedPharmacyId)
        {
            var actualPharmacyId = await ResolvePharmacyIdAsync(requestedPharmacyId);

            var order = await FetchOrderWithDetailsOrThrowAsync(orderId);

            EnsureOrderBelongsToPharmacy(order, actualPharmacyId);

            return mapper.Map<OrderDetailsDto>(order);
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto updateStatusDto, int? requestedPharmacyId)
        {
            var actualPharmacyId = await ResolvePharmacyIdAsync(requestedPharmacyId);

            var orderRepo = unitOfWork.GetRepository<Order, int>();

            var order = await FetchOrderWithDetailsOrThrowAsync(orderId);

            EnsureOrderBelongsToPharmacy(order, actualPharmacyId);
            ValidateStatusTransition(order.OrderStatus, updateStatusDto.OrderStatus);

            ApplyStatusChange(order, updateStatusDto);

            orderRepo.UpdateAsync(order);

            await unitOfWork.SaveChangesAsync();

            var patientUserId = order.PatientProfile?.ApplicationUserId;

            if (!string.IsNullOrEmpty(patientUserId))
            {
                string statusMessageAr = updateStatusDto.OrderStatus.ToLower() switch
                {
                    "preparing" => "جارٍ تجهيز طلبك في الصيدلية ⏳",
                    "intransit" => "طلبك في الطريق إليك 🛵",
                    "completed" => "تم توصيل طلبك بنجاح! بالشفاء العاجل 🎉",
                    "cancelled" => $"تم إلغاء طلبك. السبب: {updateStatusDto.CancelReason} ❌",
                    _ => $"تم تحديث حالة طلبك إلى: {updateStatusDto.OrderStatus}"
                };

                var message = new NotificationContentDto
                {
                    UserId = patientUserId,
                    Subject = "تحديث حالة الطلب 📦",
                    Body = statusMessageAr,
                    ReferenceId = order.Id,
                    Payload = null
                };

                await notificationService.SendNotificationAsync(message, Shared.EnumHelper.NotificationEnums.NotificationType.Push);
            }

            return true;
        }

        // phase 2

        public async Task<PaginationResponse<OrderDto>> GetPatientOrdersAsync(Guid applicationUserId, OrderQueryParams queryParams)
        {
            var actualPatientProfileId = await GetValidPatientProfileIdAsync(applicationUserId.ToString());

            var orderRepo = unitOfWork.GetRepository<Order, int>();

            var dataSpec = new PatientOrdersSpecification(actualPatientProfileId, queryParams);
            var countSpec = new PatientOrdersCountSpecification(actualPatientProfileId, queryParams);

            var orders = await orderRepo.GetAllWithSpecAsync(dataSpec);
            var totalCount = await orderRepo.GetCountAsync(countSpec);
            var dtos = mapper.Map<IReadOnlyList<OrderDto>>(orders);

            return new PaginationResponse<OrderDto>(
                queryParams.PageIndex, queryParams.PageSize, totalCount, dtos);
        }

        public async Task<OrderDetailsDto> GetPatientOrderDetailsAsync(int orderId, Guid applicationUserId)
        {
            var actualPatientProfileId = await GetValidPatientProfileIdAsync(applicationUserId.ToString());
            var order = await FetchOrderWithDetailsOrThrowAsync(orderId);

            // Security: verify this order belongs to the requesting patient
            EnsureOrderBelongsToPatient(order, actualPatientProfileId);

            return mapper.Map<OrderDetailsDto>(order);
        }

        public async Task<PaginationResponse<OrderDto>> GetAllPlatformOrdersAsync(OrderQueryParams queryParams)
        {
            var orderRepo = unitOfWork.GetRepository<Order, int>();
            var dataSpec = new AllPlatformOrdersSpecification(queryParams);
            var countSpec = new AllPlatformOrdersCountSpecification(queryParams);
            var orders = await orderRepo.GetAllWithSpecAsync(dataSpec);
            var totalCount = await orderRepo.GetCountAsync(countSpec);
            var dtos = mapper.Map<IReadOnlyList<OrderDto>>(orders);

            return new PaginationResponse<OrderDto>(
                queryParams.PageIndex, queryParams.PageSize, totalCount, dtos);
        }

        public async Task<AdminOrderDetailsDto> GetAdminOrderDetailsAsync(int orderId)
        {
            var spec = new AdminOrderWithDetailsSpecification(orderId);
            var order = await unitOfWork.GetRepository<Order, int>().GetByIdWithSpecAsync(spec)
                        ?? throw new NotFoundCutomeException(
                               $"Order with Id '{orderId}' was not found.");

            return mapper.Map<AdminOrderDetailsDto>(order);
        }


        #region Helper Methods - CreateOrderFromBidAsync 

        private async Task<Bid> FetchValidatedBidAsync(int bidId)
        {
            var spec = new BidWithOrderSpec(bidId);
            var bid = await unitOfWork.GetRepository<Bid, int>().GetByIdWithSpecAsync(spec)
                       ?? throw new NotFoundCutomeException($"Bid {bidId} not found.");

            // only Accepted bids can produce an Order

            if (bid.Status != BidStatus.Accepted)
                throw new BadRequestCustomeException($"Cannot create an order from Bid {bid.Id} " + $"Expected status 'Accepted' but found '{bid.Status}'");

            //  prevent a duplicate Order for the same Bid

            if (bid.Order is not null)
                throw new BadRequestCustomeException( $"Bid {bid.Id} has already been converted into Order {bid.Order.Id} " + $"Each Bid can only produce one Order");

            return bid;
        }

        private static Order BuildOrderFromBid(Bid bid)
        {
            return new Order
            {
                Amount = bid.TotalPrice,
                PaymentMethod = PaymentMethodType.CashOnDelivery, 
                PaymentStatus = PaymentStatus.Pending,
                OrderStatus = OrderStatus.Pending,

                // Order must link to PatientProfile, Pharmacy, and PrescriptionRequest

                BidId = bid.Id,
                PharmacyId = bid.PharmacyId,
                PrescriptionRequestId = bid.PrescriptionRequestId,
                PatientProfileId = bid.PrescriptionRequest.PatientProfileId,
                PatientAddressId = bid.PrescriptionRequest.DeliveryAddressId,
            };
        }

        #endregion

        #region Helper Methods — Shared Across Operations
        private async Task<int> ResolvePharmacyIdAsync(int? requestedPharmacyId)
        {
            var user = httpContextAccessor.HttpContext?.User;
            if (user == null) throw new UnAuthorizedCustomeException();

            if (user.IsInRole("Admin"))
            {
                if (!requestedPharmacyId.HasValue || requestedPharmacyId.Value == 0)
                    throw new BadRequestCustomeException("Pharmacy ID is required for Admins.");

                return requestedPharmacyId.Value;
            }

            if (user.IsInRole("PharmacyOwner"))
            {
                var ownerId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                               ?? throw new UnAuthorizedCustomeException();

                var spec = new PharmacyByOwnerAppUserIdSpec(ownerId);
                var pharmacy = await unitOfWork.GetRepository<PharmacyEntity, int>().GetByIdWithSpecAsync(spec)
                               ?? throw new NotFoundCutomeException("No active pharmacy found for this account.");

                return pharmacy.Id; 
            }

            throw new UnAuthorizedCustomeException();
        }
     
        private async Task<Order> FetchOrderWithDetailsOrThrowAsync(int orderId)
        {
            var spec = new OrderWithDetailsSpecification(orderId);

            return await unitOfWork.GetRepository<Order, int>().GetByIdWithSpecAsync(spec)
                ?? throw new NotFoundCutomeException($"Order with Id '{orderId}' was not found.");
        }

        private static void EnsureOrderBelongsToPharmacy(Order order, int pharmacyId)
        {
            if (order.PharmacyId != pharmacyId)
                throw new NotFoundCutomeException($"Order {order.Id} was not found.");
            // Surface as 404, not 403 — do not leak that the order exists under a different pharmacy.
        }

        private void ValidateStatusTransition(OrderStatus currentStatus, string requestedStatusStr)
        {
            if (!Enum.TryParse<OrderStatus>(requestedStatusStr, true, out var requestedStatus))
            {
                var validValues = string.Join(", ", Enum.GetNames(typeof(OrderStatus)));
                throw new BadRequestCustomeException($"'{requestedStatusStr}' is not a valid order status. Available values: {validValues}");
            }

            bool isValidTransition = (currentStatus, requestedStatus) switch
            {
                (OrderStatus.Pending, OrderStatus.Accepted) => true,
                (OrderStatus.Pending, OrderStatus.Preparing) => true,
                (OrderStatus.Accepted, OrderStatus.Preparing) => true,

                (OrderStatus.Preparing, OrderStatus.InTransit) => true,
                (OrderStatus.InTransit, OrderStatus.Completed) => true,

                (OrderStatus.Pending, OrderStatus.Cancelled) => true,
                (OrderStatus.Accepted, OrderStatus.Cancelled) => true,
                (OrderStatus.Preparing, OrderStatus.Cancelled) => true,

                (OrderStatus.InTransit, OrderStatus.Returned) => true,
                (OrderStatus.Completed, OrderStatus.Returned) => true,

                _ => false
            };

            if (!isValidTransition)
            {
                throw new BadRequestCustomeException(
                    $"Invalid status transition: cannot move from '{currentStatus}' to '{requestedStatus}'.");
            }
        }
        private static void EnsureOrderBelongsToPatient(Order order, string patientId)
        {
            if (order.PatientProfileId != patientId)
                throw new NotFoundCutomeException($"Order {order.Id} was not found.");
        }
        private static void ApplyStatusChange(Order order, UpdateOrderStatusDto dto)
        {
            var newStatus = Enum.Parse<OrderStatus>(dto.OrderStatus, ignoreCase: true);

            order.OrderStatus = newStatus;

            switch (newStatus)
            {
                case OrderStatus.Completed:
                    order.DeliveredAt = DateTime.UtcNow;
                    break;

                case OrderStatus.Cancelled:
                    order.CancelledAt = DateTime.UtcNow;
                    order.CancelReason = string.IsNullOrWhiteSpace(dto.CancelReason) ? "No reason provided." : dto.CancelReason;
                    break;
            }
        }

        #endregion

      
    }
}