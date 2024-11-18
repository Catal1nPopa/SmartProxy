using Common.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeAPI.Services
{
    public interface ISyncService<T> where T : PostgresDocument
    {
        Task<HttpResponseMessage> Upsert(T record);
        Task<HttpResponseMessage> Delete(T record);
    }
}
