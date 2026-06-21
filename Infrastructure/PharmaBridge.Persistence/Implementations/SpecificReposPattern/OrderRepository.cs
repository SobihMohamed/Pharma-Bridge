using Microsoft.EntityFrameworkCore;
using PharmaBridge.Domain.Contracts.SpecificReposPattern;
using PharmaBridge.Domain.Models.UserAccess;
using PharmaBridge.Persistence.Implementations.ReposPattern;
using PharmaBridge.Persistence.Pharma_BridgeDbContext;
using PharmaBridge.Shared.EnumHelper.UserAccessEnums;
using System.Linq;
using System.Threading.Tasks;

namespace PharmaBridge.Persistence.Implementations.SpecificReposPattern
{
    public class OrderRepository : GenericRepo<Order, int>, IOrderRepository
    {
        private readonly PharmaDbContext _dbContext;

        public OrderRepository(PharmaDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<decimal> GetTotalRevenueAsync(int pharmacyId)
        {
            return await _dbContext.Set<Order>()
                .Where(o => o.PharmacyId == pharmacyId && o.OrderStatus == OrderStatus.Completed)
                .SumAsync(o => o.Amount);
        }
    }
}
