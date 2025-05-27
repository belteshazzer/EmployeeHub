using AutoMapper;
using EmployeeHub.Models.Dtos;
using EmployeeHub.Models.Entities;
using EmployeeHub.Repository;

namespace EmployeeHub.Services.LookUpServices
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IMapper _mapper;
        public IGenericRepository<Departments> _departmentRepository;

        public DepartmentService(IMapper mapper, IGenericRepository<Departments> departmentRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _departmentRepository = departmentRepository ?? throw new ArgumentNullException(nameof(departmentRepository));
        }

        public async Task<IEnumerable<DepartmentDto>> GetAllDepartmentsAsync()
        {
            var departments = await _departmentRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<DepartmentDto>>(departments);
        }

        public async Task<DepartmentDto> GetDepartmentByIdAsync(Guid id)
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            if (department == null)
            {
                throw new KeyNotFoundException($"Department with ID {id} not found.");
            }
            return _mapper.Map<DepartmentDto>(department);
        }

        public async Task<DepartmentDto> CreateDepartmentAsync(DepartmentDto departmentDto)
        {
            if (departmentDto == null)
            {
                throw new ArgumentNullException(nameof(departmentDto));
            }

            var department = _mapper.Map<Departments>(departmentDto);
            department.Id = Guid.NewGuid(); // Ensure a new ID is generated
            department.CreatedAt = DateTime.UtcNow;
            department.UpdatedAt = DateTime.UtcNow;

            await _departmentRepository.AddAsync(department);
            return _mapper.Map<DepartmentDto>(department);
        }

        public async Task<DepartmentDto> UpdateDepartmentAsync(Guid id, DepartmentDto departmentDto)
        {
            if (departmentDto == null)
            {
                throw new ArgumentNullException(nameof(departmentDto));
            }

            var existingDepartment = await _departmentRepository.GetByIdAsync(id);
            if (existingDepartment == null)
            {
                throw new KeyNotFoundException($"Department with ID {id} not found.");
            }

            existingDepartment.Name = departmentDto.Name;
            existingDepartment.Description = departmentDto.Description;
            existingDepartment.UpdatedAt = DateTime.UtcNow;

            await _departmentRepository.UpdateAsync(existingDepartment);
            return _mapper.Map<DepartmentDto>(existingDepartment);
        }

        public async Task DeleteDepartmentAsync(Guid id)
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            if (department == null)
            {
                throw new KeyNotFoundException($"Department with ID {id} not found.");
            }

            await _departmentRepository.DeleteAsync(id);
        }
    }
}