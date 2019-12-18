using DataVault.Common.Extensions;
using DataVault.Internal;

namespace DataVault.Query
{
    public class SelectCommandBuilder : DataCommandBuilder
    {
        private int _limit = 100;
        private int _offset = 0;
        private string _order;

        public SelectCommandBuilder(IDataContext context) : base(context)
        {
        }

        public string OrderString => _order.IsNullOrWhiteSpace() ? string.Empty : $"ORDER BY {_order}";

        public SelectCommandBuilder Limit(int limit)
        {
            _limit = limit;

            return this;
        }

        public SelectCommandBuilder Offset(int offset)
        {
            _offset = offset;

            return this;
        }

        public SelectCommandBuilder Paging(int page, int pageSize)
        {
            _limit = pageSize;
            _offset = pageSize * (page - 1);

            return this;
        }

        public SelectCommandBuilder OrderBy(string order)
        {
            _order = order;

            return this;
        }

        public override IDataCommand BuildCommand()
        {
            return Build(Context);
        }

        public IDataCommand Build(IDataContext context)
        {
            var command = context.DataSession.CreateCommand();

            var sql = $"SELECT * FROM {Table} {WhereString} {OrderString}  LIMIT {_offset}, {_limit}";

            command.Sql(sql);

            return command;
        }
    }
}