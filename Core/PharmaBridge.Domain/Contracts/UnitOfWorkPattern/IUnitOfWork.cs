using Microsoft.AspNetCore.Identity;
using PharmaBridge.Domain.Contracts.GenericReposPattern;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Domain.Contracts.UnitOfWorkPattern
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IGenericRepo<TEntity, Tkey> GetRepository<TEntity, Tkey>() where TEntity : IEntity<Tkey>;

        // save changes method
        Task<int> SaveChangesAsync();
    }
}
