using System;
using System.Linq;

using DataVault.Common.Caching;
using DataVault.Common.Extensions;
using DataVault.Common.Json;
using DataVault.Entities;
using DataVault.Entities.Audit;
using DataVault.Entities.Dv;
using DataVault.Internal;

using Microsoft.Extensions.Caching.Distributed;

using Newtonsoft.Json.Linq;

namespace DataVault
{
    internal class DataVaultContext : DataContext, IDataVaultContext
    {
        private readonly IAuditContext _auditContext;

        protected IDistributedCache<LookupItem[]> Cache { get; }

        public DataVaultContext(
            IDataVaultConfiguration dataConfiguration,
            IAuditContext auditContext,
            IDistributedCache<LookupItem[]> cache)
            : base(new Lazy<IDataSession>(dataConfiguration.CreateSession))
        {
            _auditContext = auditContext;
            Cache = cache;
        }

        public TEntity ReadRecord<TEntity>(object hubData, object satData) where TEntity : IDvHubEntity
        {
            string stpName = nameof(ReadRecord) + EntityHelper.GetEntityName<TEntity>();

            return DataVaultCommand.ReadOnlyStoredProcedure(DataSession, stpName)
                .WithHubData(hubData)
                .WithSatData(satData ?? new[] { "*" })
                .ExecuteWithErrorHandling<TEntity>();
        }

        public string CreateRecord<TEntity>(object hubData, object satData) where TEntity : IDvHubEntity
        {
            string stpName = nameof(CreateRecord) + EntityHelper.GetEntityName<TEntity>();

            return DataVaultCommand.UpdateStoredProcedure(DataSession, _auditContext, stpName)
                .WithHubData(hubData ?? new { })
                .WithSatData(satData)
                .ExecuteWithErrorHandling();
        }

        public string LinkRecord<TEntity>(object foreignKeys, object satData) where TEntity : IDvLinkEntity
        {
            string stpName = nameof(LinkRecord) + EntityHelper.GetEntityName<TEntity>();

            return DataVaultCommand.UpdateStoredProcedure(DataSession, _auditContext, stpName)
                .WithForeignKeys(foreignKeys ?? new { })
                .WithSatData(satData)
                .ExecuteWithErrorHandling();
        }

        public string UpdateRecord<TEntity>(string hubPk, object satData) where TEntity : IDvHubEntity
        {
            var validSatData = JObject.FromObject(satData)
                .PropertyValues()
                .Any(value => !value.IsNullOrEmpty());

            if (!validSatData)
            {
                return hubPk;
            }

            string stpName = nameof(UpdateRecord) + EntityHelper.GetEntityName<TEntity>();

            return DataVaultCommand.UpdateStoredProcedure(DataSession, _auditContext, stpName)
                .WithHubPk(hubPk)
                .WithSatData(satData)
                .ExecuteWithErrorHandling();
        }

        public void UpdateLink<TEntity>(string linkPk, object satData) where TEntity : IDvLinkEntity
        {
            string stpName = nameof(UpdateLink) + EntityHelper.GetEntityName<TEntity>();

            DataVaultCommand.UpdateStoredProcedure(DataSession, _auditContext, stpName)
                .WithLinkPk(linkPk)
                .WithSatData(satData)
                .Execute();
        }

        public void DeleteRecord<TEntity>(string hubPk) where TEntity : IDvHubEntity
        {
            string stpName = nameof(DeleteRecord) + EntityHelper.GetEntityName<TEntity>();

            DataVaultCommand.UpdateStoredProcedure(DataSession, _auditContext, stpName)
                .WithHubPk(hubPk)
                .Execute();
        }

        public void UnLinkRecord<TEntity>(string linkPk) where TEntity : IDvLinkEntity
        {
            string stpName = nameof(UnLinkRecord) + EntityHelper.GetEntityName<TEntity>();

            DataVaultCommand.UpdateStoredProcedure(DataSession, _auditContext, stpName)
                .WithLinkPk(linkPk)
                .Execute();
        }

        public TEntity[] ReadRecords<TEntity>(object satQuery) where TEntity : IDvHubEntity
        {
            string stpName = nameof(ReadRecords) + EntityHelper.GetEntityName<TEntity>();

            return DataVaultCommand.ReadOnlyStoredProcedure(DataSession, stpName)
                       .WithSatQuery(satQuery)
                       .ExecuteWithErrorHandling<TEntity[]>()
                   ?? Array.Empty<TEntity>();
        }

        public TEntity[] ReadLinks<TEntity>(object satQuery) where TEntity : IDvLinkEntity
        {
            string stpName = nameof(ReadLinks) + EntityHelper.GetEntityName<TEntity>();

            return DataVaultCommand.ReadOnlyStoredProcedure(DataSession, stpName)
                       .WithSatQuery(satQuery)
                       .ExecuteWithErrorHandling<TEntity[]>()
                   ?? Array.Empty<TEntity>();
        }

        public TEntity[] ReadView<TEntity>(object satQuery) where TEntity : IDvEntity
        {
            string viewName = EntityHelper.GetEntityName<TEntity>();

            return ReadView<TEntity>(viewName, satQuery);
        }

        public TEntity[] ReadView<TEntity>(string viewName, object satQuery) where TEntity : IDvEntity
        {
            string stpName = nameof(ReadView) + viewName;

            return DataVaultCommand.ReadOnlyStoredProcedure(DataSession, stpName)
                       .WithSatQuery(satQuery)
                       .ExecuteWithErrorHandling<TEntity[]>()
                   ?? Array.Empty<TEntity>();
        }

        public void CheckLookupValue<TLookup>(string value)
        {
            var lookupItems = ReadLookup<TLookup>();

            if (!ValidateLookupValue<TLookup>(value))
            {
                throw new InvalidLookupException(typeof(TLookup), value, lookupItems);
            }
        }

        public bool ValidateLookupValue<TLookup>(string value)
        {
            return ValidateLookupValue(typeof(TLookup), value);
        }

        public bool ValidateLookupValue(Type lookupType, string value)
        {
            var lookupTableName = EntityHelper.GetLookupTableName(lookupType);

            return !value.IsNullOrWhiteSpace() && ValidateLookupValue(lookupTableName, value);
        }

        public bool ValidateLookupValue(string lookupTableName, string value)
        {
            var lookupItems = ReadLookup(lookupTableName);

            return !value.IsNullOrWhiteSpace() && lookupItems.Any(x => x.Code == value);
        }

        public LookupItem[] ReadLookups()
        {
            return DataVaultCommand.ReadOnlyStoredProcedure(DataSession, nameof(ReadLookups))
                .ExecuteWithErrorHandling<LookupItem[]>();
        }

        public LookupItem[] ReadLookup<TLookup>()
        {
            return ReadLookup(typeof(TLookup));
        }

        public LookupItem[] ReadLookup(Type lookupType)
        {
            var lookupTableName = EntityHelper.GetLookupTableName(lookupType);

            return ReadLookup(lookupTableName);
        }

        public LookupItem[] ReadLookup(string lookupTableName)
        {
            var cacheKey = $"Lookup:{lookupTableName}";

            return Cache.GetOrAdd(
                cacheKey,
                () => ReadLookupFromDb(lookupTableName),
                () => new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60) //TODO: Should be configurable from Global.
                }
            );
        }

        private LookupItem[] ReadLookupFromDb(string lookupTableName)
        {
            return DataVaultCommand.ReadOnlyStoredProcedure(DataSession, nameof(ReadLookup))
                .WithParameter("LOOKUP_TABLE", EntityHelper.NormalizeTableName(lookupTableName))
                .WithParameter("CODE", string.Empty) // TODO
                .ExecuteWithErrorHandling<LookupItem[]>();
        }

        public string GenerateEntityUniqueSequence<TEntity>() where TEntity : IDvHubEntity
        {
            string result = string.Empty;

            var sequenceName = EntityHelper.GetEntityUniqueSequenceName<TEntity>();

            DataSession.StoredProcedure("GetUniqueSequence")
                .Parameter("SeqName", sequenceName)
                .Return<string>(p => result = p)
                .Execute();

            return result;
        }
    }
}