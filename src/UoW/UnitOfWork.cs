using DataVault.Entities.Dv;
using DataVault.Internal;
using DataVault.Repositories;
using System;
using System.Collections.Generic;

namespace DataVault.UoW
{
    public class UnitOfWork : DisposableBase, IUnitOfWork
    {
        public IDataVaultContext DataVaultContext { get; set; }

        private Dictionary<Type, object> _repositories;

        public bool IsCompleted { get; set; }

        public UnitOfWork(IDataVaultContext dataVaultContext)
        {
            DataVaultContext = dataVaultContext;
        }

        public IHubRepository<TEntity> GetHubRepository<TEntity>() where TEntity : IDvHubEntity, new()
        {
            if (_repositories == null)
            {
                _repositories = new Dictionary<Type, object>();
            }

            var type = typeof(TEntity);
            if (!_repositories.ContainsKey(type))
            {
                _repositories[type] = new HubRepository<TEntity>(DataVaultContext);
            }

            return (IHubRepository<TEntity>)_repositories[type];
        }

        public ILinkRepository<TEntity> GetLinkRepository<TEntity>() where TEntity : IDvLinkEntity, new()
        {
            if (_repositories == null)
            {
                _repositories = new Dictionary<Type, object>();
            }

            var type = typeof(TEntity);
            if (!_repositories.ContainsKey(type))
            {
                _repositories[type] = new LinkRepository<TEntity>(DataVaultContext);
            }

            return (ILinkRepository<TEntity>)_repositories[type];
        }

        public void Complete()
        {
            DataVaultContext.SaveChanges();
            IsCompleted = true;
        }

        protected override void DisposeManagedResources()
        {
            // clear repositories
            if (_repositories != null)
            {
                _repositories.Clear();
            }

            DataVaultContext.Dispose();
        }
    }
}