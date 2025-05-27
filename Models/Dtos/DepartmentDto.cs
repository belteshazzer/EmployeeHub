namespace EmployeeHub.Models.Dtos
{
    public class DepartmentDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }
}