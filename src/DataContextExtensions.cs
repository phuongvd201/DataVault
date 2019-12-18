using DataVault.Query;

namespace DataVault
{
    public static class DataContextExtensions
    {
        public static SelectCommandBuilder Select(this IDataContext context, string table)
        {
            var builder = new SelectCommandBuilder(context);

            builder.From(table);

            return builder;
        }

        public static InsertCommandBuilder Insert(this IDataContext context, string table)
        {
            var builder = new InsertCommandBuilder(context);

            builder.From(table);

            return builder;
        }

        public static UpdateCommandBuilder Update(this IDataContext context, string table)
        {
            var builder = new UpdateCommandBuilder(context);

            builder.From(table);

            return builder;
        }
    }
}