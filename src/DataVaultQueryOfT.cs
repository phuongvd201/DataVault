using DataVault.Entities;
using DataVault.Entities.Dv;
using System;
using System.Collections.Generic;
using System.Linq;

using DataVault.Common.Extensions;

using Newtonsoft.Json.Linq;

namespace DataVault
{
    public static class DataVaultQuery<TEntity> where TEntity : IDvEntity, new()
    {
        public static string DbTableName { get; } = EntityHelper.GetDbTableName<TEntity>();

        public static string EntityName { get; } = EntityHelper.GetEntityName<TEntity>();

        public static string PrimaryKeyColumnName => EntityHelper.GetPKColumnName<TEntity>();

        public static string BusinessKeyColumnName => EntityHelper.GetBKColumnName<TEntity>();

        public static string ContainPKs(string[] values)
        {
            return values.Select(x => $"UNHEX('{x}')").Select(x => $"{DbTableName}.{PrimaryKeyColumnName}={x}").JoinNotEmpty(" OR ");
        }

        public static string ContainBKs(string[] values)
        {
            return values.Select(x => $"'{x}'").Select(x => $"{DbTableName}.{BusinessKeyColumnName}={x}").JoinNotEmpty(" OR ");
        }

        public static object EqualsPK(string value)
        {
            return new JObject
            {
                [PrimaryKeyColumnName] = value
            };
        }

        public static object LinkObject<TLinkToHubEntity>(string entityId, string linkToEntityId) where TLinkToHubEntity : IDvEntity
        {
            return new JObject
            {
                [PrimaryKeyColumnName] = entityId,
                [EntityHelper.GetPKColumnName<TLinkToHubEntity>()] = linkToEntityId,
            };
        }

        public static object EqualsBK(string value)
        {
            return new JObject
            {
                [BusinessKeyColumnName] = value
            };
        }

        public static object FilterSat(object sat, bool includeDetails = true)
        {
            return new
            {
                filter = FromObjectToFilterConditions(sat),
                select = includeDetails ? new[] { "*" } : Array.Empty<object>(),
            };
        }

        internal static object RelatedHub<TRelatedHub>(string[] relatedHubIds, bool includeDetails = true)
        {
            var hubTableName = EntityHelper.GetDbEntityName<TRelatedHub>();
            var hubPKName = EntityHelper.GetPKColumnName<TRelatedHub>();
            return new
            {
                filterPK = relatedHubIds.Select(x => $"{hubTableName}.{hubPKName}={x}").JoinNotEmpty(" OR "),
                select = includeDetails ? new[] { "*" } : Array.Empty<object>(),
            };
        }

        public static object FilterPK(object link, bool includeDetails = true)
        {
            return new
            {
                filterPK = FromObjectToFilterConditions(link),
                select = includeDetails ? new[] { "*" } : Array.Empty<object>(),
            };
        }

        public static object FilterPK(string id)
        {
            return new JArray(new object[] { $"{EntityHelper.GetDbTableName<TEntity>()}.{EntityHelper.GetPKColumnName<TEntity>()}='{id}'" });
        }

        public static object FilterLinksWithPKs(Dictionary<Type, string[]> entityTypeWithIds, bool includeDetails = true)
        {
            return new
            {
                filter = new JArray(
                    entityTypeWithIds
                        .Where(x => !x.Value.IsNullOrEmpty())
                        .Select(
                            o => o.Value
                                .Select(x => $"{EntityHelper.GetDbTableName<TEntity>()}.{EntityHelper.GetPKColumnName(o.Key)}=UNHEX('{x}')")
                                .JoinNotEmpty(" OR ")
                                .WrapCurlyBracket())),
                select = includeDetails ? new[] { "*" } : Array.Empty<object>(),
            };
        }

        public static object FilterLink(object link, object sat, bool includeDetails = true)
        {
            return new
            {
                filterPK = FromObjectToFilterConditions(link),
                filter = FromObjectToFilterConditions(sat),
                select = includeDetails ? new[] { "*" } : Array.Empty<object>(),
            };
        }

        private static JArray FromObjectToFilterConditions(object link)
        {
            return new JArray(
                JObject.FromObject(link)
                    .Properties()
                    .Select(x => $"{link.GetType().Name.ToUnderscoreCase().ToUpper()}.{x.Name}='{x.Value}'"));
        }
    }
}