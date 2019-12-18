using System;

using DataVault.Common.Extensions;
using DataVault.Entities.Dv;

namespace DataVault.Entities
{
    /// <summary>
    /// Some helper methods for entities.
    /// </summary>
    public static class EntityHelper
    {
        public const string DvEntityPrefix = "Dv";
        public const string HubTableNamePrefix = "HUB_";
        public const string HubPropertyPrefix = "Hub";
        public const string LinkTableNamePrefix = "LNK_";
        public const string LinkPropertyPrefix = "Lnk";
        public const string PKColumnNamePrefix = "PK_";
        public const string PKColumnNamePostfix = "_ID";
        public const string BKColumnNamePrefix = "BK_";

        public static bool IsEntity(Type entityType)
        {
            return typeof(IDvEntity).IsAssignableFrom(entityType);
        }

        public static bool IsHubEntity(Type entityType)
        {
            return typeof(IDvHubEntity).IsAssignableFrom(entityType);
        }

        public static bool IsLinkEntity(Type entityType)
        {
            return typeof(IDvLinkEntity).IsAssignableFrom(entityType);
        }

        public static string GetEntityName<TEntity>() where TEntity : IDvEntity
        {
            return GetEntityName(typeof(TEntity));
        }

        public static string GetEntityName(Type entityType)
        {
            return !IsEntity(entityType) ? string.Empty : entityType.Name.Replace(DvEntityPrefix, string.Empty);
        }

        public static string GetDbEntityName<TEntity>()
        {
            return GetDbEntityName(typeof(TEntity));
        }

        public static string GetDbEntityName(Type entityType)
        {
            return GetEntityName(entityType).ToUnderscoreCase().ToUpper();
        }

        public static string GetEntityUniqueSequenceName<TEntity>() where TEntity : IDvHubEntity
        {
            return $"SEQ-{GetEntityName<TEntity>().ToUpper()}";
        }

        public static string GetDbTableName<TEntity>() where TEntity : IDvEntity
        {
            var entityType = typeof(TEntity);
            if (IsHubEntity(entityType))
            {
                return HubTableNamePrefix + GetDbEntityName<TEntity>();
            }

            if (IsLinkEntity(entityType))
            {
                return LinkTableNamePrefix + GetDbEntityName<TEntity>();
            }

            return string.Empty;
        }

        public static string GetDbTableName(Type entityType)
        {
            if (IsHubEntity(entityType))
            {
                return HubTableNamePrefix + GetDbEntityName(entityType);
            }

            if (IsLinkEntity(entityType))
            {
                return LinkTableNamePrefix + GetDbEntityName(entityType);
            }

            return string.Empty;
        }

        public static string GetPKColumnName<TEntity>()
        {
            return $"{PKColumnNamePrefix}{GetDbEntityName<TEntity>()}{PKColumnNamePostfix}";
        }

        public static string GetPKColumnName(Type entityType)
        {
            return $"{PKColumnNamePrefix}{GetDbEntityName(entityType)}{PKColumnNamePostfix}";
        }

        public static string GetBKColumnName(Type type)
        {
            return $"{BKColumnNamePrefix}{GetDbEntityName(type)}_NUMBER";
        }

        public static string GetBKColumnName<TEntity>() where TEntity : IDvEntity
        {
            return GetBKColumnName(typeof(TEntity));
        }

        public static string GetHubPropertyName(Type entityType)
        {
            return $"{HubPropertyPrefix}{GetEntityName(entityType)}";
        }

        public static string NormalizeTableName(string tableIdentifier)
        {
            return !string.IsNullOrWhiteSpace(tableIdentifier) ? tableIdentifier.ToUpper() : string.Empty;
        }

        public static string GetLookupTableName(Type entityType)
        {
            return entityType.Name.ToUnderscoreCase().ToUpper();
        }

        public static string GetLookupTableName<TLookup>()
        {
            return GetLookupTableName(typeof(TLookup));
        }
    }
}