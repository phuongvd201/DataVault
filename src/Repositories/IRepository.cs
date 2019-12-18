using DataVault.Entities;
using DataVault.Entities.Dv;

namespace DataVault.Repositories
{
    /// <summary>
    /// Basic repository with CRUD method.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IRepository<TEntity> where TEntity : IDvEntity, new()
    {
        /// <summary>
        /// Gets an entity with given primary key.
        /// Throws <see cref="EntityNotFoundException"/> if can not find an entity with given id.
        /// </summary>
        /// <param name="id">Primary key of the entity to get</param>
        /// <param name="includeDetails">Set true to include all sat table of this entity</param>
        /// <returns>Entity</returns>
        TEntity Get(string id, bool includeDetails = true);

        /// <summary>
        /// Gets an entity with given primary key or null if not found.
        /// </summary>
        /// <param name="id">Primary key of the entity to get</param>
        /// <param name="includeDetails">Set true to include all sat table of this entity</param>
        TEntity Find(string id, bool includeDetails = true);

        string Create(TEntity entity);

        void Update(string id, TEntity entity);

        /// <summary>
        /// Delete an entity with given entity id
        /// /// Throws <see cref="EntityNotFoundException"/> if can not find an entity with given id.
        /// </summary>
        /// <param name="id"></param>
        void Delete(string id);

        /// <summary>
        /// Delete an entity with given entity
        /// /// Throws <see cref="EntityNotFoundException"/> if can not find an entity with given id.
        /// </summary>
        /// <param name="entity"></param>
        void Delete(TEntity entity);
    }
}