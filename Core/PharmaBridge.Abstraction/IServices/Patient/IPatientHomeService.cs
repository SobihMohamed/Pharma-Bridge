using PharmaBridge.Shared.Common.Params.Patient;
using PharmaBridge.Shared.DTOs.Patient;

namespace PharmaBridge.Abstraction.IServices.Patient
{
    public interface IPatientHomeService
    {
        Task<PatientHomeDto> GetPatientHomeAsync(Guid applicationUserId, PatientHomeQueryParams queryParams);
    }
}