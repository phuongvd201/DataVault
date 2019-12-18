using DataVault.Entities.Dv;
using DataVault.Entities.Dv.Link;
using DataVault.Entities.Dv.Sat;

namespace DataVault.Repositories
{
    /// <summary>
    /// Generic repository for DataVault Link Entity.
    /// </summary>
    /// <typeparam name="TDvLinkEntity"></typeparam>
    public interface ILinkRepository<TDvLinkEntity> : IDataVaultRepository<TDvLinkEntity> where TDvLinkEntity : IDvLinkEntity, new()
    {
        TDvLinkEntity[] GetList<TLink, TSat>(TLink link, TSat sat, bool includeDetails = true)
            where TLink : ILink, new()
            where TSat : ISat, new();

        //TDvLinkEntity[] GetList<TRelatedHub>(string[] relatedHubIds, bool includeDetails = true);

        TDvLinkEntity[] GetListByLink<TLink>(TLink link, bool includeDetails = true)
            where TLink : ILink, new();

        TDvLinkEntity[] GetListBySat<TSat>(TSat sat, bool includeDetails = true)
            where TSat : ISat, new();
    }
}