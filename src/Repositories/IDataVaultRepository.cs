using DataVault.Entities.Dv;

namespace DataVault.Repositories
{
    /// <summary>
    /// Generic repository for data vault entity
    /// </summary>
    /// <typeparam name="TDvEntity"></typeparam>
    public interface IDataVaultRepository<TDvEntity> : IRepository<TDvEntity> where TDvEntity : IDvEntity, new()
    {
        TDvEntity[] GetListFromView(object satQuery);

        /// <summary>
        /// Get Link Entities with given entity id which current entity linked to.
        /// </summary>
        /// <typeparam name="TDvLinkEntity"></typeparam>
        /// <param name="id"></param>
        /// <param name="includeDetails"></param>
        /// <returns>List of Link entities</returns>
        TDvLinkEntity[] GetLinks<TDvLinkEntity>(string id, bool includeDetails = true) where TDvLinkEntity : IDvLinkEntity, new();

        bool HasLink<TDvLinkEntity>(string id)
            where TDvLinkEntity : IDvLinkEntity, new();

        /// <summary>
        /// Get Link entity with combine of FKs, only used for Link have 2 FKs
        /// </summary>
        /// <typeparam name="TDvLinkEntity"></typeparam>
        /// <typeparam name="TLinkedEntity"></typeparam>
        /// <param name="id"></param>
        /// <param name="linkedEntityId"></param>
        /// <returns>Link entity</returns>
        TDvLinkEntity GetLink<TDvLinkEntity, TLinkedEntity>(string id, string linkedEntityId)
            where TDvLinkEntity : IDvLinkEntity, new()
            where TLinkedEntity : IDvEntity, new();

        /// <summary>
        /// Get list of link entities which current entity linked to with array of linked entity ids.
        /// </summary>
        /// <typeparam name="TDvLinkEntity"></typeparam>
        /// <typeparam name="TLinkedEntity"></typeparam>
        /// <param name="id">List of link entities</param>
        /// <param name="linkedEntityIds"></param>
        /// <returns></returns>
        TDvLinkEntity[] GetLinks<TDvLinkEntity, TLinkedEntity>(string id, string[] linkedEntityIds)
            where TDvLinkEntity : IDvLinkEntity, new()
            where TLinkedEntity : IDvEntity, new();

        bool ExistLink<TDvLinkEntity, TLinkedDvEntity>(string id, string linkedEntityId)
            where TDvLinkEntity : IDvLinkEntity, new()
            where TLinkedDvEntity : IDvEntity, new();

        TLinkedDvEntity[] GetLinkedEntities<TDvLinkEntity, TLinkedDvEntity>(string id, bool includeDetails = true)
            where TDvLinkEntity : IDvLinkEntity, new()
            where TLinkedDvEntity : IDvHubEntity, new();

        string[] GetLinkedEntityIds<TDvLinkEntity, TLinkedDvEntity>(string id)
            where TDvLinkEntity : IDvLinkEntity, new()
            where TLinkedDvEntity : IDvEntity, new();

        string[] GetLinkedEntityIds<TDvLinkEntity, TLinkedDvEntity>(string[] ids)
            where TDvLinkEntity : IDvLinkEntity, new()
            where TLinkedDvEntity : IDvEntity, new();

        /// <summary>
        /// Link current entity to other entity, if link is existed return link id, if not, call LinkRecord.
        /// </summary>
        /// <typeparam name="TDvLinkEntity"></typeparam>
        /// <typeparam name="TLinkToDvEntity"></typeparam>
        /// <param name="entityId"></param>
        /// <param name="linkToEntityId"></param>
        /// <returns></returns>
        string LinkTo<TDvLinkEntity, TLinkToDvEntity>(string entityId, string linkToEntityId)
            where TDvLinkEntity : IDvLinkEntity, new()
            where TLinkToDvEntity : IDvEntity, new();

        string[] LinkTo<TDvLinkEntity, TLinkToDvEntity>(string entityId, params TLinkToDvEntity[] linkToEntities)
            where TDvLinkEntity : IDvLinkEntity, new()
            where TLinkToDvEntity : IDvEntity, new();

        /// <summary>
        /// Link current entity to other entity with many ids.
        /// </summary>
        /// <typeparam name="TDvLinkEntity"></typeparam>
        /// <typeparam name="TLinkToDvEntity"></typeparam>
        /// <param name="entityId"></param>
        /// <param name="linkToEntityIds"></param>
        /// <returns></returns>
        string[] LinkTo<TDvLinkEntity, TLinkToDvEntity>(string entityId, string[] linkToEntityIds)
            where TDvLinkEntity : IDvLinkEntity, new()
            where TLinkToDvEntity : IDvEntity, new();

        void UnLinkTo<TDvLinkEntity, TUnLinkToDvEntity>(string entityId, params TUnLinkToDvEntity[] unlinkToEntities)
            where TDvLinkEntity : IDvLinkEntity, new()
            where TUnLinkToDvEntity : IDvEntity, new();

        void UnLinkTo<TDvLinkEntity, TUnLinkToDvEntity>(string entityId, params string[] unlinkToEntityIds)
            where TDvLinkEntity : IDvLinkEntity, new()
            where TUnLinkToDvEntity : IDvEntity, new();

        void UnLinkAll<TDvLinkEntity>(string entityId)
            where TDvLinkEntity : IDvLinkEntity, new();
    }
}