using System.Data;

using DataVault.Internal;

namespace DataVault
{
    public interface IDataContext
    {
        IDataSession DataSession { get; }

        void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified);

        void SaveChanges();
    }
}