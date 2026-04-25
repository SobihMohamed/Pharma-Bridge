// Path: PharmaBridge.Services/ServicesImplementation/CurrentUser/CurrentUserService.cs

using Microsoft.AspNetCore.Http;
using PharmaBridge.Abstraction.IServices.CurrentUser;
using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
using PharmaBridge.Domain.Exceptions;
using PharmaBridge.Services.Specifications.PharmacySpec;
using System.Security.Claims;
using PharmacyEntity = PharmaBridge.Domain.Models.Pharma_Requests.Pharmacy;

namespace PharmaBridge.Services.ServicesImplementation.CurrentUser
{
    public class CurrentUserService(
        IHttpContextAccessor httpContextAccessor,
        IUnitOfWork unitOfWork) : ICurrentUserService
    {
        private ClaimsPrincipal User =>
            httpContextAccessor.HttpContext?.User
            ?? throw new UnAuthorizedCustomeException();

        public string UserId =>
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnAuthorizedCustomeException();

        public bool IsAdmin => User.IsInRole("Admin");
        public bool IsPharmacyOwner => User.IsInRole("PharmacyOwner");

        public async Task<int> GetPharmacyIdAsync()
        {
            var spec = new PharmacyByOwnerAppUserIdSpec(UserId);
            var pharmacy = await unitOfWork
                               .GetRepository<PharmacyEntity, int>()
                               .GetByIdWithSpecAsync(spec)
                           ?? throw new NotFoundCutomeException(
                                  "No active pharmacy found for this account.");
            return pharmacy.Id;
        }
    }
}