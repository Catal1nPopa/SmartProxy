using Common.Models;
using Common.Utilities;
using EmployeeAPI.Settings;
using System.Text.Json;

namespace EmployeeAPI.Services
{
    public class SyncService<T> : ISyncService<T> where T : PostgresDocument
    {
        private readonly ISyncServiceSettings _settings;
        private readonly string _origin;
        public SyncService(ISyncServiceSettings settings, IHttpContextAccessor httpContextAccessor)
        {
            _settings = settings;
            _origin = httpContextAccessor.HttpContext.Request.Host.ToString();
        }
        public async Task<HttpResponseMessage> Delete(T record)
        {
            var syncType = _settings.DeleteHttpMethod;
            var json = ToSyncEntityJson(record, syncType);

            var response =await HttpClientUtility.SendJsonAsync(json, _settings.Host, "POST");

            return response;
        }

        public async Task<HttpResponseMessage> Upsert(T record)
        {
            var syncType = _settings.UpsertHttpMethod;
            var json = ToSyncEntityJson(record, syncType);

            var response = await HttpClientUtility.SendJsonAsync(json, _settings.Host, "POST");

            return response;
        }

        private string ToSyncEntityJson(T record, string syncType)
        {
            var objectType = typeof(T);

            var syncEntity = new SyncEntity()
            {
                JsonData = JsonSerializer.Serialize(record),
                SyncType = syncType,
                ObjectType = objectType.Name, //api/employee
                Id = record.Id,
                LastChangeAt = record.LastChangedAt,
                Origin = _origin,
            };

            var json = JsonSerializer.Serialize(syncEntity);

            return json;
        } 
    }
}
