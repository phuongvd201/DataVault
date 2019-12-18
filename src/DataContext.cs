using System;
using System.Data;

using DataVault.Internal;

namespace DataVault
{
    public class DataContext : DisposableBase, IDataContext
    {
        private readonly Lazy<IDataSession> _dataSession;

        public IDataSession DataSession => _dataSession.Value;

        public DataContext(Lazy<IDataSession> dataSession)
        {
            _dataSession = dataSession;
        }

        public void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            _dataSession.Value.BeginTransaction(isolationLevel);
        }

        public void SaveChanges()
        {
            if (_dataSession.IsValueCreated && _dataSession.Value.IsInTransaction)
            {
                try
                {
                    _dataSession.Value.Transaction.Commit();
                }
                catch (Exception exception)
                {
                    _dataSession.Value.Transaction.Rollback();
                    throw new DataUpdateException("Error occurs when update data vault.", exception);
                }
                finally
                {
                    _dataSession.Value.Transaction.Dispose();
                    _dataSession.Value.Transaction = null;
                    _dataSession.Value.ReleaseConnection();
                }
            }
        }

        protected override void DisposeManagedResources()
        {
            if (_dataSession.IsValueCreated)
            {
                _dataSession.Value.Dispose();
            }
        }
    }
}