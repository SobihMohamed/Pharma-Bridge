using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
using PharmaBridge.Domain.Exceptions;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Services.Specifications.Pharmacy;
using System.Threading.Tasks;

namespace PharmaBridge.Services.ServicesImplementation.Pharmacy
{
    public partial class PharmacyDashboardService
    {
        private async Task<Domain.Models.Pharma_Requests.Pharmacy> GetValidPharmacyAsync(int pharmacyId, string userId, string role)
        {
            var spec = new PharmacyWithProfileOwnerSpec(pharmacyId);
            var pharmacyRepo = _unitOfWork.GetRepository<Domain.Models.Pharma_Requests.Pharmacy, int>();
            var pharmacy = await pharmacyRepo.GetByIdWithSpecAsync(spec);

            if (pharmacy == null)
                throw new NotFoundCutomeException("Pharmacy not found.");

            if (role != "Admin" && pharmacy.PharmaOwner.ApplicationUserId != userId)
                throw new UnAuthorizedCustomeException("You are not authorized to view the performance of this pharmacy.");

            return pharmacy;
        }
    }
}
