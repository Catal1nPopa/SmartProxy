
using Common.Models;
using Dapper;
using EmployeeAPI.Settings;
using Npgsql;
using System.Data;

namespace EmployeeAPI.Repositories
{
    public class PostgresRepository<T> : IPostgresRepository<T> where T : PostgresDocument
    {
        private readonly string _connectionString;
        private readonly string _tableName;

        public PostgresRepository(IPostgresDbSettings dbSettings)
        {
            _connectionString = dbSettings.ConnectionString;

            _tableName = typeof(T).Name.ToLower();

        }

        public IDbConnection GetConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }
        public async Task DeleteRecord(Guid id)
        {
            using var connection = GetConnection();
            var query = $"DELETE FROM {_tableName} WHERE id = @Id";
            await connection.ExecuteAsync(query, new { Id = id });
        }

        public async Task<List<T>> GetAllRecordsAsync()
        {
            using var connection = GetConnection();
            var query = $"SELECT * FROM {_tableName}";
            var result = await connection.QueryAsync<T>(query);
            return result.ToList();
        }

        public async Task<T> GetRecordById(Guid id)
        {
            using var connection = GetConnection();
            var query = $"SELECT * FROM {_tableName} WHERE id = @Id";
            return await connection.QuerySingleOrDefaultAsync<T>(query, new { Id = id });
        }


        public async Task<T> InsertRecord(T record)
        {
            using var connection = GetConnection();
            var columns = string.Join(", ", typeof(T).GetProperties().Select(p => p.Name.ToLower()));
            var values = string.Join(", ", typeof(T).GetProperties().Select(p => "@" + p.Name.ToLower()));

            var query = $"INSERT INTO {_tableName} ({columns}) VALUES ({values}) RETURNING id";
            var id = await connection.ExecuteScalarAsync<Guid>(query, record);

            typeof(T).GetProperty("Id")?.SetValue(record, id);
            return record;
        }

        public async Task UpsertRecord(T record)
        {
            using var connection = GetConnection();

            var querySelect = $"SELECT COUNT(*) FROM {_tableName} WHERE id = @Id";
            var exists = await connection.ExecuteScalarAsync<int>(querySelect, new { Id = typeof(T).GetProperty("Id")?.GetValue(record) }) > 0;

            if (exists)
            {
                var columns = string.Join(", ", typeof(T).GetProperties()
                    .Where(p => p.Name.ToLower() != "id") // Nu actualiza ID-ul
                    .Select(p => $"{p.Name.ToLower()} = @{p.Name.ToLower()}"));

                var queryUpdate = $"UPDATE {_tableName} SET {columns} WHERE id = @Id";
                await connection.ExecuteAsync(queryUpdate, record);
            }
            else
            {
                var columns = string.Join(", ", typeof(T).GetProperties().Select(p => p.Name.ToLower()));
                var values = string.Join(", ", typeof(T).GetProperties().Select(p => "@" + p.Name.ToLower()));

                var queryInsert = $"INSERT INTO {_tableName} ({columns}) VALUES ({values}) RETURNING id";
                var id = await connection.ExecuteScalarAsync<Guid>(queryInsert, record);

                typeof(T).GetProperty("Id")?.SetValue(record, id);
            }
        }

    }
}
