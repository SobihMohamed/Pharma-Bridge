using MediatR;
using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
using PharmaBridge.Domain.Events;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Services.Specifications.PharmacySpecs;
using PharmaBridge.Services.Specifications.Request;
using PharmaBridge.Shared.EnumHelper.PharmaEnums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Services.EventHandlers
{
    public class PrescriptionRequestCreatedEventHandler : INotificationHandler<PrescriptionRequestCreatedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        public PrescriptionRequestCreatedEventHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(PrescriptionRequestCreatedEvent notification, CancellationToken cancellationToken)
        {
            var requestRepo = _unitOfWork.GetRepository<PrescriptionRequestEntity, int>();
            var pharmacyRepo = _unitOfWork.GetRepository<Pharmacy, int>();

            // 1- generate spec to get the request with its delivery address
            var requestSpec = new RequestWithAddressSpec(notification.RequestId);
            var request = await requestRepo.GetByIdWithSpecAsync(requestSpec);

            if (request == null || request.DeliveryAddress == null)
                return;

            var patientLat = request.DeliveryAddress.Latitude;
            var patientLng = request.DeliveryAddress.Longitude;

            // 2- calculate a radius in degrees (5 km radius, approximately 0.045 degrees)
            decimal radiusInDegrees = (decimal)(5.0 / 111.0);

            // 3- filtiration in the database to get active pharmacies within the radius using the spec
            var pharmacySpec = new ActivePharmaciesNearLocationSpec(patientLat, patientLng, radiusInDegrees);
            var targetedPharmacies = await pharmacyRepo.GetAllWithSpecAsync(pharmacySpec);

            if (targetedPharmacies == null || !targetedPharmacies.Any())
                return; // No nearby pharmacies within the radius, stop processing

            // 4- send notification to the targeted pharmacies 
            // await _notificationContext.SendNotificationAsync(request, targetedPharmacies);
        }
    }
}
