using PharmaBridge.Domain.Contracts.SpecificationPattern;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
namespace PharmaBridge.Domain.Contracts.GenericReposPattern
{
    public interface IGenericRepo<TEntity , TKey> where TEntity : IEntity<TKey>
    {
        // GetAll
        Task<IReadOnlyList<TEntity>> GetAllAsync();

        // get all with spec
        Task<IReadOnlyList<TEntity>> GetAllWithSpecAsync(ISpecifications<TEntity,TKey> specifications);
        // GetById
        Task<TEntity?> GetByIdAsync(TKey id);
        //get by id with spec
        Task<TEntity?> GetByIdWithSpecAsync(ISpecifications<TEntity,TKey> specifications);

        // Add Async Await
        Task AddAsync(TEntity entity); // call Db 

        // Update
        void UpdateAsync(TEntity entity); // in memory 

        // delete
        void DeleteAsync(TEntity entity); // in memory

        // Count 
        Task<int> GetCountAsync(ISpecifications<TEntity, TKey> specifications); // incredibly easy to implement search filters and pagination
    }
}
