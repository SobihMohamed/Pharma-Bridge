using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Domain.Contracts.GenericReposPattern
{
    public interface IGenericRepo<TEntity , TKey> where TEntity : IEntity<TKey>
    {
        // GetAll
        Task<IReadOnlyList<TEntity>> GetAllAsync();

        // GetById
        Task<TEntity?> GetByIdAsync(TKey id);

        // Add Async Await
        Task AddAsync(TEntity entity); // call Db 

        // Update
        void UpdateAsync(TEntity entity); // in memory 

        // delete
        void DeleteAsync(TEntity entity); // in memory
    }
}
