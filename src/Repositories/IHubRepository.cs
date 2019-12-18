using DataVault.Entities;
using DataVault.Entities.Dv;
using DataVault.Entities.Dv.Hub;
using DataVault.Entities.Dv.Sat;

namespace DataVault.Repositories
{
    /// <summary>
    /// Generic repository for DataVault Hub Entity.
    /// </summary>
    /// <typeparam name="TDvHubEntity"></typeparam>
    public interface IHubRepository<TDvHubEntity> : IDataVaultRepository<TDvHubEntity> where TDvHubEntity : IDvHubEntity, new()
    {
        /// <summary>
        /// Get list of entity by filter sat query
        /// </summary>
        /// <param name="satQuery">Sat object to filter</param>
        /// <param name="includeDetails"></param>
        /// <returns>List of entities filtered by SAT</returns>
        TDvHubEntity[] GetList<TSat>(TSat satQuery, bool includeDetails = true) where TSat : ISat, new();

        /// <summary>
        /// Gets an entity with given business key.
        /// Throws <see cref="EntityNotFoundException"/> if can not find an entity with given business key.
        /// </summary>
        /// <param name="bk">Business key of the entity to get</param>
        ///  /// <param name="includeDetails">Set true to include all sat table of this entity</param>
        /// <returns>Entity</returns>
        TDvHubEntity GetByBk(string bk, bool includeDetails = true);

        /// <summary>
        /// Gets an entity with given business key or null if not found.
        /// </summary>
        /// <param name="bk">Business key of the entity to get</param>
        ///  /// <param name="includeDetails">Set true to include all sat table of this entity</param>
        /// /// <returns>Entity</returns>
        TDvHubEntity FindByBk(string bk, bool includeDetails = true);

        /// <summary>
        /// Gets an entity with given hub data.
        /// Throws <see cref="EntityNotFoundException"/> if can not find an entity with given hub data.
        /// </summary>
        /// <param name="hubData">Hub data of entity to get</param>
        /// <param name="includeDetails"></param>
        /// <returns>Entity</returns>
        TDvHubEntity GetByHub<THubTable>(THubTable hubData, bool includeDetails = true) where THubTable : IHub, new();

        /// <summary>
        /// Gets an entity with given hub data or null if not found.
        /// </summary>
        /// <param name="hubData">Hub data of entity to get</param>
        /// <param name="includeDetails"></param>
        /// <returns>Entity</returns>
        TDvHubEntity FindByHub<THubTable>(THubTable hubData, bool includeDetails = true) where THubTable : IHub, new();

        /// <summary>
        /// Get list of entities by given list of Primary keys
        /// </summary>
        /// <param name="ids">List of PKs to get</param>
        /// <param name="includeDetails"></param>
        /// <returns></returns>
        TDvHubEntity[] GetListByIds(string[] ids, bool includeDetails = true);

        /// <summary>
        /// Get list of entities by given list of BKs
        /// </summary>
        /// <param name="bks">List of BK to get</param>
        /// <param name="includeDetails">Indicate that get sat or not</param>
        /// <returns></returns>
        TDvHubEntity[] GetListByBKs(string[] bks, bool includeDetails = true);

        /// <summary>
        /// Check BK exist in database with given list of BKs (ignore all empty values) and return list of entities.
        /// Throws <see cref="EntityNotFoundException"/> if can not find an entity with one of given BKs.
        /// </summary>
        /// <param name="bks">List of BK to check and get</param>
        /// <param name="includeDetails"></param>
        /// <returns></returns>
        TDvHubEntity[] CheckExistBKsAndGetList(string[] bks, bool includeDetails = true);

        string[] CheckExistBKsAndGetIds(string[] bks);

        /// <summary>
        /// Check PK exist in database with given list of PKs (ignore all empty values) and return list of entities.
        /// Throws <see cref="EntityNotFoundException"/> if can not find an entity with one of given BKs.
        /// </summary>
        /// <param name="ids">List of PKs to check and get</param>
        /// <param name="includeDetails"></param>
        /// <returns></returns>
        TDvHubEntity[] CheckExistPKsAndGetList(string[] ids, bool includeDetails = true);
    }
}