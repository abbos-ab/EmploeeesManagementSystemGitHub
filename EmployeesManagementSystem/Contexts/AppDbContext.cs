using EmployeesManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeesManagementSystem.Contexts;

public class AppDbContext : DbContext
{
    public DbSet<Department> Departments { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<OperationList> Operations { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserDepartmentRole> UserDepartmentRoles { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = "server=localhost;database=userms;user=root;password=;";
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>()
            .Property(r => r.Id)
            .HasColumnType("char(36)");
        modelBuilder.Entity<Department>()
            .Property(d => d.Id)
            .HasColumnType("char(36)");

        modelBuilder.Entity<Role>().HasData(
            new Role { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "SuperAdmin" },
            new Role { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Admin" },
            new Role { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "User" }
        );


        modelBuilder.Entity<UserDepartmentRole>()
            .HasOne(u => u.User)
            .WithMany(r => r.UserDepartmentRoles)
            .HasForeignKey(ur => ur.UserId);
        modelBuilder.Entity<UserDepartmentRole>()
            .Property(u => u.Id)
            .ValueGeneratedOnAdd();


        modelBuilder.Entity<UserDepartmentRole>()
            .HasOne(r => r.Role)
            .WithMany(ur => ur.UserDepartmentRoles)
            .HasForeignKey(ur => ur.RoleId);
        modelBuilder.Entity<UserDepartmentRole>()
            .Property(u => u.Id)
            .ValueGeneratedOnAdd();


        modelBuilder.Entity<UserDepartmentRole>()
            .HasOne(d => d.Department)
            .WithMany(ur => ur.UserDepartmentRoles)
            .HasForeignKey(ur => ur.DepartmentId);
        modelBuilder.Entity<UserDepartmentRole>()
            .Property(u => u.Id)
            .ValueGeneratedOnAdd();


        modelBuilder.Entity<OperationList>()
            .HasOne(u => u.File)
            .WithMany(o => o.Operations)
            .HasForeignKey(ur => ur.FileId);
        modelBuilder.Entity<OperationList>()
            .Property(u => u.Id)
            .ValueGeneratedOnAdd();


        modelBuilder.Entity<OperationList>()
            .HasOne(u => u.Sender)
            .WithMany(o => o.SendOperations)
            .HasForeignKey(ur => ur.SenderId);
        modelBuilder.Entity<OperationList>()
            .Property(u => u.Id)
            .ValueGeneratedOnAdd();


        modelBuilder.Entity<OperationList>()
            .HasOne(u => u.Receiver)
            .WithMany(o => o.ReceiveOperations)
            .HasForeignKey(ur => ur.ReceiverId);
        modelBuilder.Entity<OperationList>()
            .Property(u => u.Id)
            .ValueGeneratedOnAdd();
    }
}