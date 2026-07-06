using AutoMapper;
using PharmaBridge.Abstraction.IServices.Patient;
using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
using PharmaBridge.Domain.Exceptions;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Domain.Models.User;
using PharmaBridge.Domain.Models.UserAccess;
using PharmaBridge.Services.Specifications.OrderSpec;
using PharmaBridge.Services.Specifications.Request;
using PharmaBridge.Shared.Common.Params.Patient;
using PharmaBridge.Shared.DTOs.Order;
using PharmaBridge.Shared.DTOs.Patient;
using PharmaBridge.Shared.DTOs.PharmaRequests;

namespace PharmaBridge.Services.ServicesImplementation.Patient
{
    public class PatientHomeService(IUnitOfWork unitOfWork, IMapper mapper) : IPatientHomeService
    {
        public async Task<PatientHomeDto> GetPatientHomeAsync(Guid applicationUserId, PatientHomeQueryParams queryParams)
        {
            var patientProfileId = await ResolvePatientProfileIdAsync(applicationUserId.ToString());

            var latestRequests = await FetchLatestRequestsAsync(patientProfileId, queryParams.RequestsCount);

            var recentOrders = await FetchRecentOrdersAsync(patientProfileId, queryParams.OrdersCount);

            return new PatientHomeDto
            {
                LatestRequests = mapper.Map<IReadOnlyList<PrescriptionRequestDto>>(latestRequests),
                recentOrders = mapper.Map<IReadOnlyList<OrderDto>>(recentOrders)
            };
        }

        // ── Private Helpers ───────────────────────────────────────────────────

        private async Task<string> ResolvePatientProfileIdAsync(string applicationUserId)
        {
            var patientRepo = unitOfWork.GetRepository<PatientProfile, string>();
            var spec = new PatientProfileByAppUserIdSpec(applicationUserId);
            var profile = await patientRepo.GetByIdWithSpecAsync(spec);

            if (profile is null)
                throw new BadRequestCustomeException("Patient profile not found for this user.");

            return profile.Id;
        }

        private async Task<IReadOnlyList<PrescriptionRequestEntity>> FetchLatestRequestsAsync(
            string patientProfileId, int take)
        {
            var repo = unitOfWork.GetRepository<PrescriptionRequestEntity, int>();
            var spec = new LatestPatientRequestsSpecification(patientProfileId, take);
            return await repo.GetAllWithSpecAsync(spec);
        }

        private async Task<IReadOnlyList<Order>> FetchRecentOrdersAsync(string patientProfileId, int take)
        {
            var repo = unitOfWork.GetRepository<Order, int>();
            var spec = new RecentPatientOrdersSpecification(patientProfileId, take);
            return await repo.GetAllWithSpecAsync(spec);
        }
    }
}