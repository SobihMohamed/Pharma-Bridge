using PharmaBridge.Domain.Contracts.GenericReposPattern;
using PharmaBridge.Domain.Models.UserAccess;
using System.Threading.Tasks;

namespace PharmaBridge.Domain.Contracts.SpecificReposPattern
{
    public interface IOrderRepository : IGenericRepo<Order, int>
    {
        Task<decimal> GetTotalRevenueAsync(int pharmacyId);
    }
}
