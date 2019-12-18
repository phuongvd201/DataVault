using System.Linq;

using DataVault.Common.Extensions;
using DataVault.Internal;

namespace DataVault.Query
{
    public class UpdateCommandBuilder : DataCommandBuilder
    {
        public UpdateCommandBuilder(IDataContext context) : base(context)
        {
        }

        public override IDataCommand BuildCommand()
        {
            return Build(Context);
        }

        public IDataCommand Build(IDataContext context)
        {
            var command = context.DataSession.CreateCommand();

            var updateSet = Parameters.Select(x => $"{x.Key} = @{x.Key}").JoinNotEmpty(",");

            var sql = $"UPDATE {Table} SET {updateSet}  {WhereString}  ";

            AddWhereParams(command);

            command.Sql(sql);

            foreach (var parameter in Parameters)
            {
                command.Parameter(parameter.Key, parameter.Value);
            }

            return command;
        }
    }
}