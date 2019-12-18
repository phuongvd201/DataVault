using System.Linq;

using DataVault.Common.Extensions;
using DataVault.Internal;

namespace DataVault.Query
{
    public class InsertCommandBuilder : DataCommandBuilder
    {
        public InsertCommandBuilder(IDataContext context) : base(context)
        {
        }

        public override IDataCommand BuildCommand()
        {
            return Build(Context);
        }

        public IDataCommand Build(IDataContext context)
        {
            var command = context.DataSession.CreateCommand();

            var values = Parameters.Keys.ToArray().ConvertArray(x => x.EnsureStartsWith('@')).JoinNotEmpty(", ");

            var sql = $"INSERT INTO {Table} ( {Columns.JoinNotEmpty(", ")} ) VALUE ( {values}  ); SELECT LAST_INSERT_ID();";

            command.Sql(sql);

            foreach (var parameter in Parameters)
            {
                command.Parameter(parameter.Key, parameter.Value);
            }

            return command;
        }
    }
}