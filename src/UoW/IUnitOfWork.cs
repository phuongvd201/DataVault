using System;

using DataVault.Entities.Dv;
using DataVault.Repositories;

namespace DataVault.UoW
{
    public interface IUnitOfWork : IDisposable
    {
        IDataVaultContext DataVaultContext { get; set; }

        IHubRepository<TEntity> GetHubRepository<TEntity>() where TEntity : IDvHubEntity, new();

        ILinkRepository<TEntity> GetLinkRepository<TEntity>() where TEntity : IDvLinkEntity, new();

        bool IsCompleted { get; set; }

        void Complete();
    }
}