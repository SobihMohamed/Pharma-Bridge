using AutoMapper;
using Microsoft.AspNetCore.Http;
using PharmaBridge.Abstraction.IServices.CurrentUser;
using PharmaBridge.Abstraction.IServices.Order;
using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
using PharmaBridge.Domain.Exceptions;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Domain.Models.UserAccess;
using PharmaBridge.Services.Specifications.BidSpec;
using PharmaBridge.Services.Specifications.OrderSpec;
using PharmaBridge.Shared.Common.Pagination;
using PharmaBridge.Shared.Common.Params.Order;
using PharmaBridge.Shared.DTOs.Order;
using PharmaBridge.Shared.EnumHelper.PaymentEnums;
using PharmaBridge.Shared.EnumHelper.PharmaEnums;

using PharmaBridge.Shared.EnumHelper.UserAccessEnums;

namespace PharmaBridge.Services.ServicesImplementation.OrderService
{
    public class OrderService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUser) : IOrderService
    {

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

        public async Task<PaginationResponse<OrderDto>> GetPharmacyOrdersAsync( int pharmacyId, OrderQueryParams queryParams)
        {
            ValidatePharmacyAccess(pharmacyId);
            var orderRepo = unitOfWork.GetRepository<Order, int>();

            var dataSpec = new PharmacyOrdersSpecification(pharmacyId, queryParams);
            var countSpec = new PharmacyOrdersCountSpecification(pharmacyId, queryParams);

            var orders = await orderRepo.GetAllWithSpecAsync(dataSpec);
            var totalCount = await orderRepo.GetCountAsync(countSpec);

            var dtos = mapper.Map<IReadOnlyList<OrderDto>>(orders);

            return new PaginationResponse<OrderDto>(queryParams.PageIndex, queryParams.PageSize, totalCount, dtos);
        }

        public async Task<OrderDetailsDto> GetPharmacyOrderDetailsAsync(int orderId, int pharmacyId)
        {
            var order = await FetchOrderWithDetailsOrThrowAsync(orderId);
            ValidatePharmacyAccess(pharmacyId);

            EnsureOrderBelongsToPharmacy(order, pharmacyId);

            return mapper.Map<OrderDetailsDto>(order);
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto updateStatusDto,int pharmacyId)
        {
            ValidatePharmacyAccess(pharmacyId);
            var orderRepo = unitOfWork.GetRepository<Order, int>();

            var order = await orderRepo.GetByIdAsync(orderId)
                        ?? throw new NotFoundCutomeException($"Order with Id '{orderId}' was not found.");

            EnsureOrderBelongsToPharmacy(order, pharmacyId);
            ValidateStatusTransition(order.OrderStatus, updateStatusDto.OrderStatus); 
            ApplyStatusChange(order, updateStatusDto);

            orderRepo.UpdateAsync(order);
            await unitOfWork.SaveChangesAsync();

            return true;
        }


        #region Helper Methods - CreateOrderFromBidAsync 

        private async Task<Bid> FetchValidatedBidAsync(int bidId)
        {
            var spec = new BidWithDetailsSpecification(bidId);
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
        private void ValidatePharmacyAccess(int pharmacyId)
        {
            if (currentUser.IsAdmin)
                return;

            if (currentUser.IsPharmacyOwner)
            {
                var ownerPharmacyId = currentUser.GetPharmacyIdAsync().GetAwaiter().GetResult();

                if (pharmacyId != ownerPharmacyId)
                    throw new UnAuthorizedCustomeException();

                return;
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

        private static void ValidateStatusTransition(OrderStatus current, string requestedString)
        {
            // Parse the string into enum safely
            if (!Enum.TryParse<OrderStatus>(requestedString, ignoreCase: true, out var requested))
                throw new BadRequestCustomeException(
                    $"'{requestedString}' is not a valid order status.");

            if (!AllowedTransitions.TryGetValue(current, out var allowed))
                throw new BadRequestCustomeException(
                    $"Unrecognized current order status '{current}'.");

            if (!allowed.Contains(requested))
                throw new InvalidOrderStatusTransitionException(current, requested);
        }

        private static void ApplyStatusChange(Order order, UpdateOrderStatusDto dto)
        {
            var newStatus = Enum.Parse<OrderStatus>(dto.OrderStatus, ignoreCase: true);

            order.OrderStatus = newStatus;

            switch (newStatus)
            {
                case OrderStatus.Delivered:
                    order.DeliveredAt = DateTime.UtcNow;
                    break;

                case OrderStatus.Cancelled:
                    order.CancelledAt = DateTime.UtcNow;
                    order.CancelReason = string.IsNullOrWhiteSpace(dto.CancelReason) ? "No reason provided." : dto.CancelReason;
                    break;
            }
        }
        
        #endregion

        #region Status Transition Logic
        // Status transition table
        // Every legal forward move is listed here
        // Anything not listed is rejected with InvalidOrderStatusTransitionException

        private static readonly IReadOnlyDictionary<OrderStatus, IReadOnlyList<OrderStatus>> AllowedTransitions =
            new Dictionary<OrderStatus, IReadOnlyList<OrderStatus>>
            {
                // Pharmacy accepts the incoming order
                [OrderStatus.Pending] = new[] { OrderStatus.Accepted, OrderStatus.Cancelled },

                // Pharmacy starts preparing
                [OrderStatus.Accepted] = new[] { OrderStatus.Preparing, OrderStatus.Cancelled },

                // Order is handed to delivery
                [OrderStatus.Preparing] = new[] { OrderStatus.InTransit, OrderStatus.Cancelled },

                // Rider is on the way => can be delivered or returned ( patient not home)
                [OrderStatus.InTransit] = new[] { OrderStatus.Delivered, OrderStatus.Returned },

                // Terminal states => no further transitions allowed
                [OrderStatus.Delivered] = Array.Empty<OrderStatus>(),
                [OrderStatus.Cancelled] = Array.Empty<OrderStatus>(),
                [OrderStatus.Returned] = Array.Empty<OrderStatus>(),
            };
        #endregion
    }
}