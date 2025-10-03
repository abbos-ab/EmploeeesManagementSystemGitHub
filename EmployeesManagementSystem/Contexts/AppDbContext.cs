using EmployeesManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
namespace EmployeesManagementSystem.Contexts
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Files> File { get; set; }
        public DbSet<OperationList> Operations { get; set; }
        public object Files { get; internal set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = "server=localhost;database=userms;user=root;password=;";
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId);
            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd();



            modelBuilder.Entity<Files>()
                .HasOne(u => u.User)
                .WithMany(r => r.Files)
                .HasForeignKey(f => f.CreatedBy);
            modelBuilder.Entity<Files>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd();



            modelBuilder.Entity<OperationList>()
                 .HasOne(o => o.User)
                 .WithMany(u => u.operationLists)
                 .HasForeignKey(o => o.UserId);
            modelBuilder.Entity<User>()
                 .Property(u => u.Id)
                 .ValueGeneratedOnAdd();


            modelBuilder.Entity<OperationList>()
                .HasOne(o => o.File)
                .WithMany(f => f.Operations)
                .HasForeignKey(o => o.FileID);
            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd();



            modelBuilder.Entity<Files>()
                .HasOne(f => f.Receiver)
                .WithMany()
                .HasForeignKey(f => f.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Files>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
