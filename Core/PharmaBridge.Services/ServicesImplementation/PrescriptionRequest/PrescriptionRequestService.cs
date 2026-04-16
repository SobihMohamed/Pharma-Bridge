using AutoMapper;
using PharmaBridge.Abstraction.IServices.PrescriptionRequest;
using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
using PharmaBridge.Domain.Exceptions;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Domain.Models.User;
using PharmaBridge.Services.Specifications;
using PharmaBridge.Shared.Common.Pagination;
using PharmaBridge.Shared.Common.Params.PrescriptionRequest;
using PharmaBridge.Shared.DTOs.PharmaRequests;
using PharmaBridge.Shared.DTOs.PharmaRequestsFlow;
using PharmaBridge.Shared.EnumHelper.PharmaEnums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Services.ServicesImplementation.PrescriptionRequest
{
    public class PrescriptionRequestService(IUnitOfWork unitOfWork, IMapper mapper) : IPrescriptionRequestService
    {
        public async Task<PrescriptionRequestDto> CreateRequestAsync(CreatePrescriptionRequestDto createDto, Guid patientIdGuid)
        {
            var PatientId = patientIdGuid.ToString();
            // 1 - validate the input form user 
            ValidateRequestInput(createDto);

            // 2 - validate the delivery address id exist and belong to the patient
            var patientAddress = await GetValidAddressAsync(createDto.DeliveryAddressId, PatientId);

            // 3 - create the Prescription Request Object to send to Db 
            var request = BuidPrescriptionRequestEntity(createDto, PatientId);
            request.DeliveryAddress = patientAddress;
            // 4 - Get the Repo of the request
            await unitOfWork.GetRepository<PrescriptionRequestEntity, int>().AddAsync(request);
            var result = await unitOfWork.SaveChangesAsync();

            if (result <= 0) throw new BadRequestCustomeException("Failed to create prescription request");

            // 5 - Map the result to PrescriptionRequestDto to send it to client 
            var requestDto = mapper.Map<PrescriptionRequestDto>(request);
            return requestDto;

        }
        #region Helper Methods In CreateRequest Service
        private void ValidateRequestInput(CreatePrescriptionRequestDto requestDto) 
        {
            // Business Rule : Not Valid if Null of Medicin Name and Image Url 
            if(string.IsNullOrEmpty(requestDto.MedicineName) && string.IsNullOrEmpty(requestDto.ImageUrl))
            {
                throw new BadRequestCustomeException("Please Provide at least a medicin name or image of request");
            }
        }
        private PrescriptionRequestEntity BuidPrescriptionRequestEntity(CreatePrescriptionRequestDto createRequestDto, string patientId) 
        {
            var request = mapper.Map<PrescriptionRequestEntity>(createRequestDto);

            request.PatientProfileId = patientId;
            request.Status = PrescriptionStatus.Pending;
            request.ExpiresAt = DateTime.UtcNow.AddHours(24);
            request.CreatedAt = DateTime.UtcNow;

            request.PrescriptionRequestHistorys.Add(new PrescriptionRequestHistory
            {
                OldStatus = null,
                NewStatus = PrescriptionStatus.Pending,
                ChangedAt = DateTime.UtcNow,
                Notes = "Prescription Request Created by Patient",
                ChangedById = patientId,
            });
            return request;
        }
        private async Task<PatientAddress> GetValidAddressAsync(int addressId, string patientId)
        {
            var addressSpec = new PatientAddressWithPatientprofileSpec(addressId, patientId);
            var addressRepo = unitOfWork.GetRepository<PatientAddress, int>();

            var deliveryAddress = await addressRepo.GetByIdWithSpecAsync(addressSpec);
            if (deliveryAddress == null) throw new UnAuthorizedCustomeException();

            return deliveryAddress; 
        }

        #endregion
        public Task<PaginationResponse<PrescriptionRequestDto>> GetPatientRequestsAsync(Guid patientId, PrescriptionRequestQueryParams queryParams)
        {
            throw new NotImplementedException();
        }
    }
}
