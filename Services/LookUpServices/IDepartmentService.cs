using EmployeeHub.Models.Dtos;

namespace EmployeeHub.Services.LookUpServices
{
    public interface IDepartmentService
    {
        Task<IEnumerable<DepartmentDto>> GetAllDepartmentsAsync();
        Task<DepartmentDto> GetDepartmentByIdAsync(Guid id);
        Task<DepartmentDto> CreateDepartmentAsync(DepartmentDto departmentDto);
        Task<DepartmentDto> UpdateDepartmentAsync(Guid id, DepartmentDto departmentDto);
        Task DeleteDepartmentAsync(Guid id);
    }
}