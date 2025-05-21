using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TMS.Models;
using TMS.Models.UserModels;

namespace TMS.Data
{
    public class TMSDbContext : IdentityDbContext<User>
    {
        public TMSDbContext(DbContextOptions<TMSDbContext> options) : base(options)
        {
        }

        public DbSet<Department> departments { get; set; }
        public DbSet<Project> projects { get; set; }
        public DbSet<Assignment> assignments { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Department → Users 
            builder.Entity<Department>()
                .HasMany(d => d.Users)
                .WithOne(u => u.Department)
                .HasForeignKey(u => u.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict); // Don't delete users

            // Project → Assignments 
            builder.Entity<Project>()
                .HasMany(p => p.Assignments)
                .WithOne(a => a.Project)
                .HasForeignKey(a => a.ProjectId)
                .OnDelete(DeleteBehavior.Cascade); // Delete assignments with project

            // User → Assignments
            builder.Entity<User>()
                .HasMany(u => u.Assignments)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Don't delete assignments if user is deleted
        }


    }
}
