using System;
using System.Linq;

using DataVault.Common.Extensions;
using DataVault.Common.Json;
using DataVault.Entities;
using DataVault.Entities.Dv;
using DataVault.Entities.Dv.Hub;
using DataVault.Entities.Dv.Sat;

namespace DataVault.Repositories
{
    public class HubRepository<TDvHubEntity> : DataVaultRepository<TDvHubEntity>, IHubRepository<TDvHubEntity>
        where TDvHubEntity : IDvHubEntity, new()
    {
        public HubRepository(IDataVaultContext dataVaultContext) : base(dataVaultContext)
        {
        }

        public override TDvHubEntity Get(string id, bool includeDetails = true)
        {
            var entity = Find(id, includeDetails);

            if (entity == null)
            {
                throw new EntityNotFoundException(typeof(TDvHubEntity), id);
            }

            return entity;
        }

        public override TDvHubEntity Find(string id, bool includeDetails = true)
        {
            return DataVaultContext.ReadRecord<TDvHubEntity>(DataVaultQuery<TDvHubEntity>.EqualsPK(id), includeDetails ? new[] { "*" } : Array.Empty<object>());
        }

        public override string Create(TDvHubEntity entity)
        {
            return DataVaultContext.CreateRecord<TDvHubEntity>(entity.GetHubData(), entity);
        }

        public override void Update(string id, TDvHubEntity entity)
        {
            ReflectionHelper.SetValueByPath(entity, typeof(TDvHubEntity), $"{EntityHelper.GetHubPropertyName(entity.GetType())}", null);

            DataVaultContext.UpdateRecord<TDvHubEntity>(id, entity);
        }

        public override void Delete(string id)
        {
            Get(id, false);

            DataVaultContext.DeleteRecord<TDvHubEntity>(id);
        }

        public TDvHubEntity GetByHub<THub>(THub hubData, bool includeDetails = true) where THub : IHub, new()
        {
            var entity = FindByHub(hubData);

            if (entity == null)
            {
                throw new EntityNotFoundException(typeof(TDvHubEntity), hubData.ToJsonString());
            }

            return entity;
        }

        public TDvHubEntity FindByHub<THub>(THub hubData, bool includeDetails = true) where THub : IHub, new()
        {
            return DataVaultContext.ReadRecord<TDvHubEntity>(hubData, includeDetails ? new[] { "*" } : Array.Empty<object>());
        }

        public TDvHubEntity GetByBk(string bk, bool includeDetails = true)
        {
            var entity = FindByBk(bk, includeDetails);

            if (entity == null || string.IsNullOrEmpty(entity.GetId()))
            {
                throw new EntityNotFoundException(typeof(TDvHubEntity), bk);
            }

            return entity;
        }

        public TDvHubEntity FindByBk(string bk, bool includeDetails = true)
        {
            return DataVaultContext.ReadRecord<TDvHubEntity>(DataVaultQuery<TDvHubEntity>.EqualsBK(bk), includeDetails ? new[] { "*" } : Array.Empty<object>());
        }

        public TDvHubEntity[] GetList<TSat>(TSat satQuery, bool includeDetails = true) where TSat : ISat, new()
        {
            return DataVaultContext.ReadRecords<TDvHubEntity>(DataVaultQuery<TDvHubEntity>.FilterSat(satQuery));
        }

        public TDvHubEntity[] GetListByIds(string[] ids, bool includeDetails = true)
        {
            return ids.IsNullOrEmpty()
                ? Array.Empty<TDvHubEntity>()
                : DataVaultContext.ReadRecords<TDvHubEntity>(
                    new
                    {
                        filter = new[] { DataVaultQuery<TDvHubEntity>.ContainPKs(ids) },
                        select = includeDetails ? new[] { "*" } : Array.Empty<object>(),
                    });
        }

        public TDvHubEntity[] GetListByBKs(string[] bks, bool includeDetails = true)
        {
            return bks.IsNullOrEmpty()
                ? Array.Empty<TDvHubEntity>()
                : DataVaultContext.ReadRecords<TDvHubEntity>(
                    new
                    {
                        filter = new[] { DataVaultQuery<TDvHubEntity>.ContainBKs(bks) },
                        select = includeDetails ? new[] { "*" } : Array.Empty<object>(),
                    });
        }

        public TDvHubEntity[] CheckExistBKsAndGetList(string[] bks, bool includeDetails = true)
        {
            if (bks.IsNullOrEmpty())
            {
                return Array.Empty<TDvHubEntity>();
            }

            bks = bks.Where(x => !x.IsNullOrWhiteSpace()).ToArray();

            var entities = GetListByBKs(bks, includeDetails);

            var notExist = bks.Except(entities.ConvertArray(x => x.GetBk())).ToArray();

            if (!notExist.IsNullOrEmpty())
            {
                throw new EntityNotFoundException(typeof(TDvHubEntity), notExist.JoinNotEmpty(", "));
            }

            return entities;
        }

        public string[] CheckExistBKsAndGetIds(string[] bks)
        {
            return CheckExistBKsAndGetList(bks, false).ConvertArray(x => x.GetId());
        }

        public TDvHubEntity[] CheckExistPKsAndGetList(string[] ids, bool includeDetails = true)
        {
            if (ids.IsNullOrEmpty())
            {
                return Array.Empty<TDvHubEntity>();
            }

            ids = ids.Where(x => !x.IsNullOrWhiteSpace()).ToArray();

            var entities = GetListByIds(ids, includeDetails);

            var notExist = ids.Except(entities.ConvertArray(x => x.GetBk())).ToArray();

            if (!notExist.IsNullOrEmpty())
            {
                throw new EntityNotFoundException(typeof(TDvHubEntity), notExist.JoinNotEmpty(", "));
            }

            return entities;
        }
    }
}