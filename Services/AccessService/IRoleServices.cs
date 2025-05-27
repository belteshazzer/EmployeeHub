using EmployeeHub.Models.Dtos;

namespace EmployeeHub.Services.LookUpServices
{
    public interface IRoleServices
    {
        Task<IEnumerable<RoleDto>> GetAllRolesAsync();
        Task<RoleDto> GetRoleByIdAsync(Guid id);
        Task<RoleDto> CreateRoleAsync(RoleDto roleDto);
        Task<RoleDto> UpdateRoleAsync(Guid id, RoleDto roleDto);
        Task DeleteRoleAsync(Guid id);
    }
}