using AutoMapper;
using PharmaBridge.Abstraction.IServices.Pharmacy;
using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
using PharmaBridge.Domain.Exceptions;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Domain.Models.User;
using PharmaBridge.Domain.Models.UserAccess;
using PharmaBridge.Services.Specifications.Ratings;
using PharmaBridge.Services.Specifications.Request;
using PharmaBridge.Shared.Common.Pagination;
using PharmaBridge.Shared.Common.Params.Pharmacy;
using PharmaBridge.Shared.DTOs.PharmacyRating;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PharmaBridge.Services.ServicesImplementation.Pharmacy
{
    public class PharmacyRatingService(IUnitOfWork unitOfWork, IMapper mapper) : IPharmacyRatingService
    {
        // Test update
        /// <summary>
        /// Validates and submits a new rating for a pharmacy from a patient.
        /// It ensures the patient has a completed order, prevents duplicate ratings,
        /// and recalculates the pharmacy's average rating in a single transaction.
        /// </summary>
        public async Task<bool> SubmitRatingAsync(CreatePharmacyRatingDto createRatingDto, string patientId)
        {
            var orderRepo = unitOfWork.GetRepository<Order, int>();
            var ratingRepo = unitOfWork.GetRepository<PharmacyRating, int>();
            var pharmacyRepo = unitOfWork.GetRepository<PharmaBridge.Domain.Models.Pharma_Requests.Pharmacy, int>();
            var patientProfileRepo = unitOfWork.GetRepository<PatientProfile, string>();

            // 1. Validate patient profile exists
            var profileSpec = new PatientProfileByAppUserIdSpec(patientId);
            var patientProfile = await patientProfileRepo.GetByIdWithSpecAsync(profileSpec);
            if (patientProfile == null)
            {
                throw new BadRequestCustomeException("Patient profile not found for this user.");
            }

            var actualPatientProfileId = patientProfile.Id;

            // 2. Validate the order is completed and belongs to this patient & pharmacy
            var orderSpec = new OrderForRatingValidationSpec(createRatingDto.OrderId, createRatingDto.PharmacyId, actualPatientProfileId);
            var validOrder = await orderRepo.GetByIdWithSpecAsync(orderSpec);

            if (validOrder == null)
            {
                throw new BadRequestCustomeException("You can only rate a pharmacy if you have a completed order with them.");
            }

            // 3. Check for duplicate rating — filter by both OrderId and PatientProfileId for security
            var existingRatingSpec = new RatingByOrderAndPatientSpec(createRatingDto.OrderId, actualPatientProfileId);
            var existingRatings = await ratingRepo.GetAllWithSpecAsync(existingRatingSpec);
            if (existingRatings.Any())
            {
                throw new BadRequestCustomeException("You have already rated this order.");
            }

            // 4. Create the new rating
            var newRating = mapper.Map<PharmacyRating>(createRatingDto);
            newRating.PatientProfileId = actualPatientProfileId;

            await ratingRepo.AddAsync(newRating);

            // 5. Recalculate the pharmacy's average rating before saving
            //    Fetch all existing ratings + account for the new one to compute in a single save
            var ratingsSpec = new PharmacyRatingsByPharmacyIdSpec(createRatingDto.PharmacyId);
            var existingPharmacyRatings = await ratingRepo.GetAllWithSpecAsync(ratingsSpec);

            var pharmacy = await pharmacyRepo.GetByIdAsync(createRatingDto.PharmacyId);
            if (pharmacy != null)
            {
                // Calculate new average: existing ratings + the new rating being added
                var totalRatings = existingPharmacyRatings.Count + 1;
                var sumRatings = existingPharmacyRatings.Sum(r => r.RatingValue) + createRatingDto.RatingValue;
                pharmacy.AverageRating = (decimal)sumRatings / totalRatings;

                pharmacyRepo.UpdateAsync(pharmacy);
            }

            // 6. Single SaveChanges — both the new rating and updated average are saved atomically
            var saveResult = await unitOfWork.SaveChangesAsync();

            // [Team Note] After persisting the rating, we must call the Performance Snapshot Service 
            // to recalculate the AverageRating and update the pharmacy's real-time dashboard data.
            // (Cross-service synchronization requirement discussed on May 8th)

            return saveResult > 0;
        }

        public async Task<PaginationResponse<PharmacyRatingDto>> GetPharmacyReviewsAsync(int pharmacyId, PharmacyRatingQueryParams queryParams)
        {
            var ratingRepo = unitOfWork.GetRepository<PharmacyRating, int>();
            var pharmacyRepo = unitOfWork.GetRepository<PharmaBridge.Domain.Models.Pharma_Requests.Pharmacy, int>();

            var pharmacy = await pharmacyRepo.GetByIdAsync(pharmacyId);
            if (pharmacy == null)
            {
                throw new NotFoundCutomeException("Pharmacy not found.");
            }

            // Fetch paginated ratings
            var spec = new PharmacyRatingsByPharmacyIdSpec(pharmacyId, queryParams);
            var ratings = await ratingRepo.GetAllWithSpecAsync(spec);

            // Count total using a spec WITHOUT pagination applied (PageSize = 0 disables paging)
            var countParams = new PharmacyRatingQueryParams
            {
                RatingValue = queryParams.RatingValue,
                Search = queryParams.Search,
                PageSize = 0  // Explicitly disable paging so GetCountAsync returns the true total
            };
            var countSpec = new PharmacyRatingsByPharmacyIdSpec(pharmacyId, countParams);
            var totalCount = await ratingRepo.GetCountAsync(countSpec);

            var mappedRatings = mapper.Map<List<PharmacyRatingDto>>(ratings.ToList());

            return new PaginationResponse<PharmacyRatingDto>(
                index: queryParams.PageIndex,
                size: queryParams.PageSize,
                total: totalCount,
                data: mappedRatings.AsReadOnly()
            );
        }
    }
}