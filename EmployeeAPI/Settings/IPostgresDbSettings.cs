namespace EmployeeAPI.Settings
{
    public interface IPostgresDbSettings
    {
        string DatabaseName { get; set; }
        string ConnectionString { get; set; }
    }
}
