using System;
using System.Data;
using System.Data.Common;

namespace DataVault.Internal
{
    public interface IDataSession : IDisposable
    {
        DbConnection Connection { get; }

        DbTransaction Transaction { get; set; }

        DbTransaction BeginTransaction(IsolationLevel isolationLevel);

        bool IsInTransaction { get; }

        IDataCommand CreateCommand();

        IDataCommand Sql(string sql);

        IDataCommand StoredProcedure(string storedProcedureName);

        void EnsureConnection();

        void ReleaseConnection();

        void WriteLog(string message);
    }
}