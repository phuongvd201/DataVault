using System;
using System.Collections.Generic;
using System.Linq;

using DataVault.Common.Extensions;
using DataVault.Entities;
using DataVault.Entities.Dv;

namespace DataVault.Repositories
{
    public abstract class DataVaultRepository<TDvEntity> : IDataVaultRepository<TDvEntity>
        where TDvEntity : IDvEntity, new()
    {
        public IDataVaultContext DataVaultContext { get; }

        protected DataVaultRepository(IDataVaultContext dataVaultContext)
        {
            DataVaultContext = dataVaultContext;
        }

        public TDvEntity[] GetListFromView(object satQuery)
        {
            return DataVaultContext.ReadView<TDvEntity>(satQuery);
        }

        public TDvLinkEntity[] GetLinks<TDvLinkEntity>(string id, bool includeDetails = true)
            where TDvLinkEntity : IDvLinkEntity, new()
        {
            return GetLinks<TDvLinkEntity>(new[] { id }, includeDetails);
        }

        public TDvLinkEntity[] GetLinks<TDvLinkEntity>(string[] ids, bool includeDetails = true)
            where TDvLinkEntity : IDvLinkEntity, new()
        {
            var query = DataVaultQuery<TDvLinkEntity>.FilterLinksWithPKs(
                new Dictionary<Type, string[]>
                {
                    { typeof(TDvEntity), ids },
                },
                includeDetails);

            return DataVaultContext.ReadLinks<TDvLinkEntity>(query);
        }

        public bool HasLink<TDvLinkEntity>(string id)
            where TDvLinkEntity : IDvLinkEntity, new()
        {
            return GetLinks<TDvLinkEntity>(id).Any();
        }

        public TDvLinkEntity[] GetLinks<TDvLinkEntity, TLinkedDvEntity>(string id, string[] linkedEntityIds)
            where TDvLinkEntity : IDvLinkEntity, new()
            where TLinkedDvEntity : IDvEntity, new()
        {
            if (linkedEntityIds.IsNullOrEmpty())
            {
                return Array.Empty<TDvLinkEntity>();
            }

            var query = DataVaultQuery<TDvLinkEntity>.FilterLinksWithPKs(
                new Dictionary<Type, string[]>
                {
                    { typeof(TDvEntity), new[] { id } },
                    { typeof(TLinkedDvEntity), linkedEntityIds }
                });

            return DataVaultContext.ReadLinks<TDvLinkEntity>(query);
        }

        public TDvLinkEntity GetLink<TDvLinkEntity, TLinkedDvEntity>(string id, string linkedEntityId)
            where TDvLinkEntity : IDvLinkEntity, new()
            where TLinkedDvEntity : IDvEntity, new()
        {
            var query = DataVaultQuery<TDvLinkEntity>.FilterLinksWithPKs(
                new Dictionary<Type, string[]>
                {
                    { typeof(TDvEntity), new[] { id } },
                    { typeof(TLinkedDvEntity), new[] { linkedEntityId } }
                });

            return DataVaultContext.ReadLinks<TDvLinkEntity>(query).FirstOrDefault();
        }

        public bool ExistLink<TDvLinkEntity, TLinkedDvEntity>(string id, string linkedEntityId)
            where TDvLinkEntity : IDvLinkEntity, new()
            where TLinkedDvEntity : IDvEntity, new()
        {
            return !GetLink<TDvLinkEntity, TLinkedDvEntity>(id, linkedEntityId).GetId().IsNullOrWhiteSpace();
        }

        public TLinkedDvEntity[] GetLinkedEntities<TDvLinkEntity, TLinkedDvEntity>(string id, bool includeDetails = true)
            where TDvLinkEntity : IDvLinkEntity, new()
            where TLinkedDvEntity : IDvHubEntity, new()
        {
            var linkedEntitiesIds = GetLinkedEntityIds<TDvLinkEntity, TLinkedDvEntity>(id);

            return linkedEntitiesIds.IsNullOrEmpty()
                ? Array.Empty<TLinkedDvEntity>()
                : DataVaultContext.ReadRecords<TLinkedDvEntity>(
                    new
                    {
                        filter = new[] { DataVaultQuery<TLinkedDvEntity>.ContainPKs(linkedEntitiesIds) },
                        select = includeDetails ? new[] { "*" } : Array.Empty<object>(),
                    });
        }

        public string[] LinkTo<TDvLinkEntity, TLinkToDvEntity>(string entityId, string[] linkToEntityIds)
            where TDvLinkEntity : IDvLinkEntity, new()
            where TLinkToDvEntity : IDvEntity, new()
        {
            if (linkToEntityIds.IsNullOrEmpty())
            {
                return Array.Empty<string>();
            }

            var newLinkIds = Enumerable.Empty<string>();
            foreach (string linkToEntityId in linkToEntityIds)
            {
                var newLinkId = LinkTo<TDvLinkEntity, TLinkToDvEntity>(entityId, linkToEntityId);
                newLinkIds = newLinkIds.Append(newLinkId);
            }

            return newLinkIds.ToArray();
        }

        public string[] LinkTo<TDvLinkEntity, TLinkToDvEntity>(string entityId, params TLinkToDvEntity[] linkToEntities)
            where TDvLinkEntity : IDvLinkEntity, new()
            where TLinkToDvEntity : IDvEntity, new()
        {
            var linkToEntityIds = linkToEntities.ConvertArray(x => x.GetId());

            return LinkTo<TDvLinkEntity, TLinkToDvEntity>(entityId, linkToEntityIds);
        }

        public string LinkTo<TDvLinkEntity, TLinkToDvEntity>(string entityId, string linkToEntityId)
            where TDvLinkEntity : IDvLinkEntity, new()
            where TLinkToDvEntity : IDvEntity, new()
        {
            if (linkToEntityId.IsNullOrWhiteSpace())
            {
                return string.Empty;
            }

            var linkEntity = GetLink<TDvLinkEntity, TLinkToDvEntity>(entityId, linkToEntityId);
            if (linkEntity.Exist())
            {
                return linkEntity.GetId();
            }

            return DataVaultContext.LinkRecord<TDvLinkEntity>(
                DataVaultQuery<TDvEntity>.LinkObject<TLinkToDvEntity>(entityId, linkToEntityId),
                new { } //TODO: need to add SAT data here
            );
        }

        public void UnLinkTo<TDvLinkEntity, TUnLinkToDvEntity>(string entityId, params string[] unlinkToEntityIds)
            where TDvLinkEntity : IDvLinkEntity, new()
            where TUnLinkToDvEntity : IDvEntity, new()
        {
            if (unlinkToEntityIds.IsNullOrEmpty())
            {
                return;
            }

            var linkEntities = GetLinks<TDvLinkEntity, TUnLinkToDvEntity>(entityId, unlinkToEntityIds);

            foreach (var linkEntity in linkEntities)
            {
                DataVaultContext.UnLinkRecord<TDvLinkEntity>(linkEntity.GetId());
            }
        }

        public void UnLinkTo<TDvLinkEntity, TUnLinkToDvEntity>(string entityId, params TUnLinkToDvEntity[] unlinkToEntities)
            where TDvLinkEntity : IDvLinkEntity, new()
            where TUnLinkToDvEntity : IDvEntity, new()
        {
            var unlinkToEntityIds = unlinkToEntities.ConvertArray(x => x.GetId());
            UnLinkTo<TDvLinkEntity, TUnLinkToDvEntity>(entityId, unlinkToEntityIds);
        }

        public void UnLinkAll<TDvLinkEntity>(string entityId)
            where TDvLinkEntity : IDvLinkEntity, new()
        {
            var linkEntities = GetLinks<TDvLinkEntity>(entityId);

            foreach (var linkEntity in linkEntities)
            {
                DataVaultContext.UnLinkRecord<TDvLinkEntity>(linkEntity.GetId());
            }
        }

        public string[] GetLinkedEntityIds<TDvLinkEntity, TLinkedDvEntity>(string id)
            where TDvLinkEntity : IDvLinkEntity, new()
            where TLinkedDvEntity : IDvEntity, new()
        {
            return GetLinkedEntityIds<TDvLinkEntity, TLinkedDvEntity>(new[] { id });
        }

        public string[] GetLinkedEntityIds<TDvLinkEntity, TLinkedDvEntity>(string[] ids)
            where TDvLinkEntity : IDvLinkEntity, new()
            where TLinkedDvEntity : IDvEntity, new()
        {
            return GetLinks<TDvLinkEntity>(ids, false)
                .ConvertArray(x => (string)x.GetLinkData().GetPropertyValue($"{EntityHelper.GetEntityName<TLinkedDvEntity>()}Id.Value"))
                .Distinct()
                .ToArray();
        }

        public abstract TDvEntity Get(string id, bool includeDetails = true);

        public abstract TDvEntity Find(string id, bool includeDetails = true);

        public abstract string Create(TDvEntity entity);

        public abstract void Update(string id, TDvEntity entity);

        public abstract void Delete(string id);

        public void Delete(TDvEntity entity)
        {
            var id = entity.GetId();

            Delete(id);
        }
    }
}