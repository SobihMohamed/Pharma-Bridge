using AutoMapper;
using PharmaBridge.Abstraction.IServices.PatientProfiles;
using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
using PharmaBridge.Domain.Exceptions;
using PharmaBridge.Domain.Models.User;
using PharmaBridge.Services.Specifications.Patient;
using PharmaBridge.Shared.Common.Pagination;
using PharmaBridge.Shared.Common.Params.Patient;
using PharmaBridge.Shared.DTOs.PatientProfiles;
using System;
using System.Collections.Generic;
using System.Text;
namespace PharmaBridge.Services.ServicesImplementation.Patient;

public class PatientProfileService(IUnitOfWork unitOfWork, IMapper mapper) : IPatientProfileService
{
    public async Task<PatientProfileDetailsDto> GetMyProfileAsync(string patientId)
    {
        var spec = new PatientProfileWithDetailsSpec(patientId);

        var patientRepo = unitOfWork.GetRepository<PatientProfile, string>();

        var patient = await patientRepo.GetByIdWithSpecAsync(spec);

        if (patient == null)
            throw new NotFoundCutomeException("Patient not found");

        return mapper.Map<PatientProfileDetailsDto>(patient);
    }

    public async Task<PatientProfileDetailsDto> UpdateMyProfileAsync(string patientId, PatientProfileToUpdateDto updateDto)
    {
        var patientRepo = unitOfWork.GetRepository<PatientProfile, string>();

        var spec = new PatientProfileWithDetailsSpec(patientId);

        var patient = await patientRepo.GetByIdWithSpecAsync(spec);

        if (patient == null)
            throw new NotFoundCutomeException("Patient not found");

        patient.ApplicationUser.FullName = updateDto.FullName;
        patient.ApplicationUser.PhoneNumber = updateDto.PhoneNumber;

        patientRepo.UpdateAsync(patient);

        var result = await unitOfWork.SaveChangesAsync();

        if (result <= 0)
            throw new BadRequestCustomeException("Failed to update patient profile");

        return mapper.Map<PatientProfileDetailsDto>(patient);
        }

    public async Task<PaginationResponse<PatientProfileDto>> GetAllPatientsAsync(PatientQueryParams queryParams)
    {
        var patientRepo = unitOfWork.GetRepository<PatientProfile, string>();

        var dataSpec = new PatientWithFiltersSpec(queryParams);
        var countSpec = new PatientWithFiltersSpec(queryParams.Search);

        var patients = await patientRepo.GetAllWithSpecAsync(dataSpec);

        var totalItems = await patientRepo.GetCountAsync(countSpec);

        var data = mapper.Map<IReadOnlyList<PatientProfileDto>>(patients);

        return new PaginationResponse<PatientProfileDto>(
            queryParams.PageIndex,
            queryParams.PageSize,
            totalItems,
            data
        );
    }
}
