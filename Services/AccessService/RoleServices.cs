using System.Data;
using AutoMapper;
using EmployeeHub.Models.Dtos;
using EmployeeHub.Models.Entities;
using EmployeeHub.Repository;

namespace EmployeeHub.Services.LookUpServices
{
    public class RoleServices : IRoleServices
    {
        private readonly IGenericRepository<Roles> _roleRepository;
        private readonly IMapper _mapper;
        public RoleServices(IMapper mapper, IGenericRepository<Roles> roleRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        }

        public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
        {
            var roles = await _roleRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<RoleDto>>(roles);
        }

        public async Task<RoleDto> GetRoleByIdAsync(Guid id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
            {
                throw new KeyNotFoundException($"Role with ID {id} not found.");
            }
            return _mapper.Map<RoleDto>(role);
        }

        public async Task<RoleDto> CreateRoleAsync(RoleDto roleDto)
        {
            if (roleDto == null)
            {
                throw new ArgumentNullException(nameof(roleDto));
            }

            var role = _mapper.Map<Roles>(roleDto);
            role.Id = Guid.NewGuid(); // Ensure a new ID is generated
            role.CreatedAt = DateTime.UtcNow;
            role.UpdatedAt = DateTime.UtcNow;

            await _roleRepository.AddAsync(role);
            return _mapper.Map<RoleDto>(role);
        }

        public async Task<RoleDto> UpdateRoleAsync(Guid id, RoleDto roleDto)
        {
            if (roleDto == null)
            {
                throw new ArgumentNullException(nameof(roleDto));
            }

            var existingRole = await _roleRepository.GetByIdAsync(id);
            if (existingRole == null)
            {
                throw new KeyNotFoundException($"Role with ID {id} not found.");
            }

            existingRole.Name = roleDto.Name;
            existingRole.Description = roleDto.Description;
            existingRole.UpdatedAt = DateTime.UtcNow;

            await _roleRepository.UpdateAsync(existingRole);
            return _mapper.Map<RoleDto>(existingRole);
        }

        public async Task DeleteRoleAsync(Guid id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
            {
                throw new KeyNotFoundException($"Role with ID {id} not found.");
            }

            await _roleRepository.DeleteAsync(id);
        }

        
    }
}