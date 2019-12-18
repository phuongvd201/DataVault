namespace DataVault.Query
{
    public class QueryCondition
    {
        public QueryField Field { get; set; }

        public QueryOperator Operator { get; set; }

        public QueryValue Value { get; set; }
    }
}