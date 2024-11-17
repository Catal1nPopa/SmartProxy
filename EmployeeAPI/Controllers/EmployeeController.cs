using Common.Models;
using EmployeeAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController(IPostgresRepository<Employee> employee) : ControllerBase
    {
        private readonly IPostgresRepository<Employee> _employeeRepository = employee;

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
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var employee = await _employeeRepository.GetRecordById(id); 
            if(employee == null)
            {
                return BadRequest("Not exist");
            }
            await _employeeRepository.DeleteRecord(id);
            return Ok($"Deleted { id}");
        }

        [HttpGet("{id}")]
        public async Task<Employee> GetEmployeeById(Guid id)
        {
            return await _employeeRepository.GetRecordById(id);
        }

        [HttpPut]
        public async Task<IActionResult> Upsert([FromBody]Employee employee)
        {
            if (employee.Id == Guid.Empty)
            {
                return BadRequest("Empty id");
            }
            employee.LastChangedAt = DateTime.UtcNow;
            await _employeeRepository.UpsertRecord(employee);
            return Ok();
        }
    }
}
