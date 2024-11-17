using Common.Models;

namespace EmployeeAPI.Repositories
{
    public interface IPostgresRepository<T> where T : PostgresDocument
    {
        Task<List<T>> GetAllRecordsAsync();
        Task<T> InsertRecord(T record);
        Task<T> GetRecordById(Guid id);
        Task UpsertRecord(T record);
        Task DeleteRecord(Guid id);
    }
}
