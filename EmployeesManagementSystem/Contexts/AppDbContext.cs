using EmployeesManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
namespace EmployeesManagementSystem.Contexts
{
    public class AppDbContext : DbContext
    {
        public DbSet<Departament> Departaments { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<OperationList> Operations { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserDeportmentRole> UserDeportmentRoles { get; set; }
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
            modelBuilder.Entity<Departament>()
                .Property(d => d.Id)
                .HasColumnType("char(36)");

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "SuperAdmin" },
                new Role { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Admin" },
                new Role { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "User" }
            );


            modelBuilder.Entity<UserDeportmentRole>()
                .HasOne(u => u.User)
                .WithMany(r => r.UserDeportmentRoles)
                .HasForeignKey(ur => ur.IdUser);
            modelBuilder.Entity<UserDeportmentRole>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd();


            modelBuilder.Entity<UserDeportmentRole>()
                .HasOne(r => r.Role)
                .WithMany(ur => ur.UserDeportmentRoles)
                .HasForeignKey(ur => ur.IdRole);
            modelBuilder.Entity<UserDeportmentRole>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd();


            modelBuilder.Entity<UserDeportmentRole>()
                .HasOne(d => d.Departament)
                .WithMany(ur => ur.UserDeportmentRoles)
                .HasForeignKey(ur => ur.IdDeportment);
            modelBuilder.Entity<UserDeportmentRole>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd();


            modelBuilder.Entity<OperationList>()
                .HasOne(u => u.File)
                .WithMany(o => o.Operations)
                .HasForeignKey(ur => ur.FileID);
            modelBuilder.Entity<OperationList>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd();


            modelBuilder.Entity<OperationList>()
                .HasOne(u => u.UserSend)
                .WithMany(o => o.SendOperations)
                .HasForeignKey(ur => ur.SenderId);
            modelBuilder.Entity<OperationList>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd();



            modelBuilder.Entity<OperationList>()
                .HasOne(u => u.UserReceive)
                .WithMany(o => o.ReceiveOperations)
                .HasForeignKey(ur => ur.ReceiverId);
            modelBuilder.Entity<OperationList>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
