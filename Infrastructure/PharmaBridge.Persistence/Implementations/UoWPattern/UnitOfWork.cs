using PharmaBridge.Domain.Contracts;
using PharmaBridge.Domain.Contracts.GenericReposPattern;
using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
using PharmaBridge.Persistence.Implementations.ReposPattern;
using PharmaBridge.Persistence.Pharma_BridgeDbContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Persistence.Implementations.UoWPattern
{
    public class UnitOfWork(PharmaDbContext _context) : IUnitOfWork
    {
        private readonly Dictionary<string, object> _repository = [];
        public IGenericRepo<TEntity, Tkey> GetRepository<TEntity, Tkey>() where TEntity : class, IEntity<Tkey>
        {
            // Get the type name of the entity
            var typeName = typeof(TEntity).Name;

            if(_repository.ContainsKey(typeName))
                return (IGenericRepo<TEntity, Tkey>)_repository[typeName];

            // new object of the generic repository
            var repository = new GenericRepo<TEntity, Tkey>(_context);
            _repository[typeName] = repository;

            return repository;

        }

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
        public async ValueTask DisposeAsync() => await _context.DisposeAsync();
    }
}
