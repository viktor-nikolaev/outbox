namespace ZionApps.Outbox.PostgreSql
{
    public class PostgreSqlOptions
    {
        public string Schema { get; set; } = "outbox";
        public string ConnectionString { get; set; } = string.Empty;
        public int FetchRowsLimit { get; set; } = 50;
    }
}