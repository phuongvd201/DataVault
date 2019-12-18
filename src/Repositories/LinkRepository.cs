using System;
using System.Linq;

using DataVault.Entities;
using DataVault.Entities.Dv;
using DataVault.Entities.Dv.Link;
using DataVault.Entities.Dv.Sat;

namespace DataVault.Repositories
{
    public class LinkRepository<TDvLinkEntity> : DataVaultRepository<TDvLinkEntity>, ILinkRepository<TDvLinkEntity>
        where TDvLinkEntity : IDvLinkEntity, new()
    {
        public LinkRepository(IDataVaultContext dataVaultContext) : base(dataVaultContext)
        {
        }

        public override TDvLinkEntity Get(string id, bool includeDetails = true)
        {
            var entity = Find(id, includeDetails);

            if (entity == null)
            {
                throw new EntityNotFoundException(typeof(TDvLinkEntity), id);
            }

            return entity;
        }

        public override TDvLinkEntity Find(string id, bool includeDetails = true)
        {
            var query = new
            {
                filterPK = DataVaultQuery<TDvLinkEntity>.FilterPK(id),
                select = includeDetails ? new[] { "*" } : Array.Empty<object>()
            };

            return DataVaultContext.ReadLinks<TDvLinkEntity>(query).FirstOrDefault();
        }

        public override string Create(TDvLinkEntity entity)
        {
            return DataVaultContext.LinkRecord<TDvLinkEntity>(entity.GetLinkData(), entity);
        }

        public override void Update(string id, TDvLinkEntity entity)
        {
            DataVaultContext.UpdateLink<TDvLinkEntity>(id, entity);
        }

        public override void Delete(string id)
        {
            DataVaultContext.UnLinkRecord<TDvLinkEntity>(id);
        }

        //public TDvLinkEntity[] GetList<TRelatedHub>(string[] relatedHubIds, bool includeDetails = true)
        //{
        //    return DataVaultContext.ReadLinks<TDvLinkEntity>(DataVaultQuery<TDvLinkEntity>.RelatedHub<TRelatedHub>(relatedHubIds));
        //}

        public TDvLinkEntity[] GetListByLink<TLink>(TLink link, bool includeDetails = true) where TLink : ILink, new()
        {
            return DataVaultContext.ReadLinks<TDvLinkEntity>(DataVaultQuery<TDvLinkEntity>.FilterPK(link));
        }

        public TDvLinkEntity[] GetListBySat<TSat>(TSat satQuery, bool includeDetails = true) where TSat : ISat, new()
        {
            return DataVaultContext.ReadLinks<TDvLinkEntity>(DataVaultQuery<TDvLinkEntity>.FilterSat(satQuery));
        }

        public TDvLinkEntity[] GetList<TLink, TSat>(TLink link, TSat sat, bool includeDetails = true)
            where TLink : ILink, new()
            where TSat : ISat, new()
        {
            return DataVaultContext.ReadLinks<TDvLinkEntity>(DataVaultQuery<TDvLinkEntity>.FilterLink(link, sat));
        }
    }
}