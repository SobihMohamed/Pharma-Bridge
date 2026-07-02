using AutoMapper;
using MediatR;
using PharmaBridge.Abstraction.IServices.Notification;
using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
using PharmaBridge.Domain.Events;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Services.Specifications.PharmacySpecs;
using PharmaBridge.Services.Specifications.Request;
using PharmaBridge.Shared.DTOs.Notificaiton;

using PharmaBridge.Shared.DTOs.PharmaRequests;
using PharmaBridge.Shared.EnumHelper.NotificationEnums;

namespace PharmaBridge.Services.EventHandlers
{
    public class PrescriptionRequestCreatedEventHandler : INotificationHandler<PrescriptionRequestCreatedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper; 

        public PrescriptionRequestCreatedEventHandler(
            IUnitOfWork unitOfWork,
            INotificationService notificationService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _mapper = mapper;
        }

        public async Task Handle(PrescriptionRequestCreatedEvent notification, CancellationToken cancellationToken)
        {
            var requestRepo = _unitOfWork.GetRepository<PrescriptionRequestEntity, int>();
            var pharmacyRepo = _unitOfWork.GetRepository<Pharmacy, int>();

            var requestSpec = new RequestWithAddressSpec(notification.RequestId);
            var request = await requestRepo.GetByIdWithSpecAsync(requestSpec);

            if (request == null || request.DeliveryAddress == null) return;

            var patientLat = request.DeliveryAddress.Latitude;
            var patientLng = request.DeliveryAddress.Longitude;

            decimal radiusInDegrees = (decimal)(5.0 / 111.0);

            var pharmacySpec = new ActivePharmaciesNearLocationSpec(patientLat, patientLng, radiusInDegrees);
            var targetedPharmacies = await pharmacyRepo.GetAllWithSpecAsync(pharmacySpec);

            if (targetedPharmacies == null || !targetedPharmacies.Any()) return;

            var requestCardData = _mapper.Map<PharmacyNearbyRequestDto>(request);

            foreach (var pharmacy in targetedPharmacies)
            {
                var ownerUserId = pharmacy.PharmaOwner?.ApplicationUserId;

                if (string.IsNullOrEmpty(ownerUserId)) continue;

                var message = new NotificationContentDto
                {
                    UserId = ownerUserId, 
                    Subject = "طلب جديد بالقرب منك",
                    Body = $"يوجد طلب دواء جديد على بعد مسافة قريبة من صيدليتك.",
                    ReferenceId = request.Id,
                    Payload = requestCardData 
                };

                await _notificationService.SendNotificationAsync(message, NotificationType.Push);
            }
        }
    }
}