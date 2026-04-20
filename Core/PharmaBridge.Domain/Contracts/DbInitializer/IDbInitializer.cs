
namespace PharmaBridge.Domain.DbInitializer
{
    public interface IDbInitializer
    {
        Task DataSeedAsync();
    }
}
