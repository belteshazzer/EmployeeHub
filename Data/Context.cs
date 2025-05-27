using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EmployeeHub.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using EmployeeHub.Models.Dtos;

namespace EmployeeHub.Data
{
    public class EmployeeHubContext : IdentityDbContext<User, Roles, Guid>
    {
        public EmployeeHubContext(DbContextOptions<EmployeeHubContext> options)
            : base(options)
        {
        }

        public DbSet<Chat> Chats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure entity relationships and constraints here if needed
            modelBuilder.Entity<Chat>()
                .HasOne(c => c.User1)
                .WithMany(u => u.ChatsAsUser1)
                .HasForeignKey(c => c.User1Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Chat>()
                .HasOne(c => c.User2)
                .WithMany(u => u.ChatsAsUser2)
                .HasForeignKey(c => c.User2Id)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Chat>()
                .Property(c => c.MessagesJson)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<List<ChatHistory>>(v));

            modelBuilder.Entity<User>()
                .HasOne(u => u.Department)
                .WithMany()
                .HasForeignKey(u => u.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
                
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}