using Common.Models;
using EmployeeAPI.Repositories;
using EmployeeAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController(IPostgresRepository<Employee> employee, ISyncService<Employee> syncService) : ControllerBase
    {
        private readonly IPostgresRepository<Employee> _employeeRepository = employee;
        private readonly ISyncService<Employee> _employeeSyncService = syncService;

        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var records = await _employeeRepository.GetAllRecordsAsync();
            return Ok(records);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Employee employee)
        {
            employee.LastChangedAt = DateTime.UtcNow;
            var result = await _employeeRepository.InsertRecord(employee);
            _employeeSyncService.Upsert(employee);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var employee = await _employeeRepository.GetRecordById(id);
            if (employee == null)
            {
                return BadRequest("Not exist");
            }
            await _employeeRepository.DeleteRecord(id);

            employee.LastChangedAt = DateTime.UtcNow;

            _employeeSyncService.Delete(employee);

            return Ok($"Deleted {id}");
        }

        [HttpGet("{id}")]
        public async Task<Employee> GetEmployeeById(Guid id)
        {
            return await _employeeRepository.GetRecordById(id);
        }

        [HttpPut]
        public async Task<IActionResult> Upsert([FromBody] Employee employee)
        {
            if (employee.Id == Guid.Empty)
            {
                return BadRequest("Empty id");
            }
            employee.LastChangedAt = DateTime.UtcNow;
            await _employeeRepository.UpsertRecord(employee);

            _employeeSyncService.Upsert(employee);

            return Ok();
        }


        [HttpPut("sync")]
        public async Task<IActionResult> UpsertSync(Employee employee)
        {
            try
            {
                var existingEmployee = await _employeeRepository.GetRecordById(employee.Id);
                if (existingEmployee == null)
                {
                    await _employeeRepository.UpsertRecord(employee);
                    Console.WriteLine($"Inserted employee: {employee.Id}");
                }
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpsertSync: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }


        [HttpDelete("sync")]
        public async Task<IActionResult> DeleteSync(Employee employee)
        {
            var existingEmployee = _employeeRepository.GetRecordById(employee.Id);
            if (existingEmployee != null || employee.LastChangedAt > existingEmployee.Result.LastChangedAt)
            {
                await _employeeRepository.DeleteRecord(employee.Id);
            }
            return Ok();
        }
    }
}
