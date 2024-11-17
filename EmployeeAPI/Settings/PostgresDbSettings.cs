namespace EmployeeAPI.Settings
{
    public class PostgresDbSettings : IPostgresDbSettings
    {
        public string DatabaseName { get; set; }
        public string ConnectionString { get; set; }
    }
}
