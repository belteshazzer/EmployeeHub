using EmployeeHub.Common.ApiResponse;
using EmployeeHub.Models.Dtos;
using EmployeeHub.Services.LookUpServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace EmployeeHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;
        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService ?? throw new ArgumentNullException(nameof(departmentService));
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllDepartments()
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            var response = new ApiResponse<IEnumerable<DepartmentDto>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Departments retrieved successfully.",
                Data = departments
            };

            return Ok(response);
        }

        [HttpGet("departments/{id}")]
        public async Task<IActionResult> GetDepartmentById(Guid id)
        {
            var department = await _departmentService.GetDepartmentByIdAsync(id);
            var response = new ApiResponse<DepartmentDto>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Department retrieved successfully.",
                Data = department
            };

            return Ok(response);
        }

        [HttpPost("departments")]
        public async Task<IActionResult> CreateDepartment([FromBody] DepartmentDto departmentDto)
        {
            if (departmentDto == null)
            {
                return BadRequest(new ApiResponse<DepartmentDto>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Invalid department data."
                });
            }

            var createdDepartment = await _departmentService.CreateDepartmentAsync(departmentDto);
            var response = new ApiResponse<DepartmentDto>
            {
                StatusCode = StatusCodes.Status201Created,
                Message = "Department created successfully.",
                Data = createdDepartment
            };

            return CreatedAtAction(nameof(GetDepartmentById), new { id = createdDepartment.Id }, response);
        }

        [HttpPut("departments/{id}")]
        public async Task<IActionResult> UpdateDepartment(Guid id, [FromBody] DepartmentDto departmentDto)
        {
            if (departmentDto == null)
            {
                return BadRequest(new ApiResponse<DepartmentDto>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Invalid department data."
                });
            }

            var updatedDepartment = await _departmentService.UpdateDepartmentAsync(id, departmentDto);
            var response = new ApiResponse<DepartmentDto>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Department updated successfully.",
                Data = updatedDepartment
            };

            return Ok(response);
        }

        [HttpDelete("departments/{id}")]
        public async Task<IActionResult> DeleteDepartment(Guid id)
        {
            await _departmentService.DeleteDepartmentAsync(id);
            var response = new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status204NoContent,
                Message = "Department deleted successfully."
            };

            return NoContent();
        }
    }
}