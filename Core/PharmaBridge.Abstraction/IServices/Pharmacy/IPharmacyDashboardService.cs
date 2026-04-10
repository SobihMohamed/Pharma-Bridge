using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Abstraction.IServices.Pharmacy
{
    /// This interface defines the contract for the Pharmacy's private dashboard.
    // Provides metrics, performance snapshots, and active summaries for a specific pharmacy.
    public interface IPharmacyDashboardService
    {
        // Gets quick stats: Today's Orders, Total Revenue, Pending Requests nearby, Average Rating.
        //Task<PharmaPerformSnapshotDto> GetMyPerformanceSnapshotAsync(int pharmacyId);
    }
}
