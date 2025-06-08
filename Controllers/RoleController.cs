using EmployeeHub.Common.ApiResponse;
using EmployeeHub.Models.Dtos;
using EmployeeHub.Services.LookUpServices;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleServices _roleService;
        public RoleController(IRoleServices roleService)
        {
            _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _roleService.GetAllRolesAsync();
            var response = new ApiResponse<IEnumerable<RoleDto>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Roles retrieved successfully.",
                Data = roles
            };

            return Ok(response);
        }

        [HttpGet("roles/{id}")]
        public async Task<IActionResult> GetRoleById(Guid id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            var response = new ApiResponse<RoleDto>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Role retrieved successfully.",
                Data = role
            };

            return Ok(response);
        }

        [HttpPost("roles")]
        public async Task<IActionResult> CreateRole([FromBody] RoleDto roleDto)
        {
            if (roleDto == null)
            {
                return BadRequest(new ApiResponse<RoleDto>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Invalid role data."
                });
            }

            var createdRole = await _roleService.CreateRoleAsync(roleDto);
            var response = new ApiResponse<RoleDto>
            {
                StatusCode = StatusCodes.Status201Created,
                Message = "Role created successfully.",
                Data = createdRole
            };

            return CreatedAtAction(nameof(GetRoleById), new { id = createdRole.Id }, response);
        }

        [HttpPut("roles/{id}")]
        public async Task<IActionResult> UpdateRole(Guid id, [FromBody] RoleDto roleDto)
        {
            if (roleDto == null)
            {
                return BadRequest(new ApiResponse<RoleDto>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Invalid role data."
                });
            }

            var updatedRole = await _roleService.UpdateRoleAsync(id, roleDto);
            var response = new ApiResponse<RoleDto>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Role updated successfully.",
                Data = updatedRole
            };

            return Ok(response);
        }


        [HttpDelete("roles/{id}")]
        public async Task<IActionResult> DeleteRole(Guid id)
        {
            await _roleService.DeleteRoleAsync(id);
            var response = new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status204NoContent,
                Message = "Role deleted successfully.",
                Data = null
            };

            return NoContent();
        }
    }
}