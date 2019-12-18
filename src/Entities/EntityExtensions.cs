using System;

using DataVault.Common.Extensions;
using DataVault.Entities.Dv;
using DataVault.Entities.Dv.Hub;
using DataVault.Entities.Dv.Link;

namespace DataVault.Entities
{
    public static class EntityExtensions
    {
        public static string GetId(this IDvEntity entity)
        {
            if (entity == null)
            {
                return null;
            }

            var entityType = entity.GetType();

            if (EntityHelper.IsHubEntity(entityType))
            {
                return (entity as IDvHubEntity).GetHubData().GetId();
            }

            if (EntityHelper.IsLinkEntity(entityType))
            {
                return (entity as IDvLinkEntity).GetLinkData().GetId();
            }

            throw new Exception("Cannot get Id without data vault entity");
        }

        public static string GetId(this IHub hub)
        {
            if (hub == null)
            {
                return null;
            }

            return (string)hub.GetPropertyValue($"{hub.GetType().Name.Replace(EntityHelper.HubPropertyPrefix, string.Empty)}Id.Value");
        }

        public static string GetId(this ILink link)
        {
            if (link == null)
            {
                return null;
            }

            return (string)link.GetPropertyValue($"{link.GetType().Name.Replace(EntityHelper.LinkPropertyPrefix, string.Empty)}Id.Value");
        }

        /// <summary>
        /// CAUTION: Some time it does not work with ReadView because some Views does not return child object Primary Key.
        /// Example: ReadViewDocument does not return HUB_ACCOUNT.PK_ACCOUNT_ID. So, this check will fail.
        /// </summary>
        public static bool Exist(this IDvEntity entity)
        {
            // TODO: need refactor Stored Procedure ReadView to ensure that always return child entity primary key.
            return entity != null && !entity.GetId().IsNullOrWhiteSpace();
        }

        public static bool Exist(this IDvHubEntity entity)
        {
            return entity != null && entity.GetHubData() != null;
        }

        public static bool Exist(this IDvLinkEntity entity)
        {
            return entity != null && entity.GetLinkData() != null;
        }

        public static bool ExistHub<THub>(this IDvEntity entity)
        {
            var hubName = typeof(THub).Name;
            return !((string)entity.GetPropertyValue($"{hubName}.Value.{hubName.Replace(EntityHelper.HubPropertyPrefix, string.Empty)}Id.Value")).IsNullOrWhiteSpace();
        }

        public static bool Exist(this IHub hub)
        {
            return !hub.GetId().IsNullOrWhiteSpace();
        }

        public static string GetBk(this IDvHubEntity entity)
        {
            if (entity == null)
            {
                return null;
            }

            var bkPropertyName = EntityHelper.GetBKColumnName(entity.GetType()).Replace(EntityHelper.BKColumnNamePrefix, string.Empty).ToPascalCase();

            return (string)entity.GetHubData().GetPropertyValue($"{bkPropertyName}.Value");
        }

        public static IHub GetHubData(this IDvHubEntity entity)
        {
            if (entity == null)
            {
                return null;
            }

            return (IHub)entity.GetPropertyValue($"{EntityHelper.GetHubPropertyName(entity.GetType())}.Value");
        }

        public static ILink GetLinkData(this IDvLinkEntity entity)
        {
            if (entity == null)
            {
                return null;
            }

            return (ILink)entity.GetPropertyValue($"{EntityHelper.LinkPropertyPrefix}{EntityHelper.GetEntityName(entity.GetType())}.Value");
        }
    }
}