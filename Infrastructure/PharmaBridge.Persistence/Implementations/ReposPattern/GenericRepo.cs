using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using PharmaBridge.Domain.Contracts;
using PharmaBridge.Domain.Contracts.GenericReposPattern;
using PharmaBridge.Persistence.Pharma_BridgeDbContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Persistence.Implementations.ReposPattern
{
    public class GenericRepo<TEntity, TKey> 
        : IGenericRepo<TEntity, TKey> where TEntity : class ,IEntity<TKey>
    {
        private readonly DbSet<TEntity> _dbSet;
        public GenericRepo(PharmaDbContext dbContext)
        {
            _dbSet = dbContext.Set<TEntity>();
        }
        public async Task AddAsync(TEntity entity) => await _dbSet.AddAsync(entity);

        public async Task<IReadOnlyList<TEntity>> GetAllAsync() => await _dbSet.AsNoTracking().ToListAsync();

        public async Task<TEntity?> GetByIdAsync(TKey id) => await _dbSet.FindAsync(id);

        public void UpdateAsync(TEntity entity) => _dbSet.Update(entity);

        public void DeleteAsync(TEntity entity) => _dbSet.Remove(entity);
    }
}
