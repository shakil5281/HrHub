using HrHubAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HrHubAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<UserCompany> UserCompanies { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Designation> Designations { get; set; }
        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure the primary key length to avoid EF Core warnings
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.Id).HasMaxLength(128);
            });

            builder.Entity<ApplicationRole>(entity =>
            {
                entity.Property(e => e.Id).HasMaxLength(128);
            });

            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.Property(e => e.UserId).HasMaxLength(128);
                entity.Property(e => e.RoleId).HasMaxLength(128);
            });

            builder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.Property(e => e.UserId).HasMaxLength(128);
            });

            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.Property(e => e.LoginProvider).HasMaxLength(128);
                entity.Property(e => e.ProviderKey).HasMaxLength(128);
                entity.Property(e => e.UserId).HasMaxLength(128);
            });

            builder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.Property(e => e.RoleId).HasMaxLength(128);
            });

            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.Property(e => e.UserId).HasMaxLength(128);
                entity.Property(e => e.LoginProvider).HasMaxLength(128);
                entity.Property(e => e.Name).HasMaxLength(128);
            });

            // Configure Company entity
            builder.Entity<Company>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                
                // Create index on Name for better query performance
                entity.HasIndex(e => e.Name);
                entity.HasIndex(e => e.IsActive);
            });

            // Configure relationship between Company and ApplicationUser (existing one-to-many)
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.HasOne(u => u.Company)
                      .WithMany(c => c.Employees)
                      .HasForeignKey(u => u.CompanyId)
                      .IsRequired(false);
            });

            // Configure UserCompany junction table for many-to-many relationship
            builder.Entity<UserCompany>(entity =>
            {
                entity.HasKey(uc => uc.Id);
                
                entity.HasOne(uc => uc.User)
                      .WithMany()
                      .HasForeignKey(uc => uc.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(uc => uc.Company)
                      .WithMany()
                      .HasForeignKey(uc => uc.CompanyId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Create composite unique index to prevent duplicate assignments
                entity.HasIndex(uc => new { uc.UserId, uc.CompanyId })
                      .IsUnique()
                      .HasDatabaseName("IX_UserCompany_UserId_CompanyId");

                // Create indexes for better query performance
                entity.HasIndex(uc => uc.UserId);
                entity.HasIndex(uc => uc.CompanyId);
                entity.HasIndex(uc => uc.IsActive);
                
                entity.Property(uc => uc.AssignedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure Department entity
            builder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                
                // Create index on Name for better query performance
                entity.HasIndex(e => e.Name);
                entity.HasIndex(e => e.IsActive);
            });

            // Configure Section entity
            builder.Entity<Section>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                
                // Configure relationship with Department
                entity.HasOne(s => s.Department)
                      .WithMany()
                      .HasForeignKey(s => s.DepartmentId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                // Create indexes for better query performance
                entity.HasIndex(e => e.Name);
                entity.HasIndex(e => e.DepartmentId);
                entity.HasIndex(e => e.IsActive);
                
                // Create composite index for department + section name uniqueness
                entity.HasIndex(e => new { e.DepartmentId, e.Name })
                      .IsUnique()
                      .HasDatabaseName("IX_Section_DepartmentId_Name");
            });

            // Configure Designation entity
            builder.Entity<Designation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Grade).IsRequired().HasMaxLength(50);
                entity.Property(e => e.AttendanceBonus).HasColumnType("decimal(18,2)");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                
                // Configure relationship with Section
                entity.HasOne(d => d.Section)
                      .WithMany()
                      .HasForeignKey(d => d.SectionId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                // Create indexes for better query performance
                entity.HasIndex(e => e.Name);
                entity.HasIndex(e => e.SectionId);
                entity.HasIndex(e => e.Grade);
                entity.HasIndex(e => e.IsActive);
                
                // Create composite index for section + designation name uniqueness
                entity.HasIndex(e => new { e.SectionId, e.Name })
                      .IsUnique()
                      .HasDatabaseName("IX_Designation_SectionId_Name");
            });

            // Configure Employee entity
            builder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.NIDNo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.FatherName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.MotherName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Address).IsRequired().HasMaxLength(500);
                entity.Property(e => e.GrossSalary).HasColumnType("decimal(18,2)");
                entity.Property(e => e.BasicSalary).HasColumnType("decimal(18,2)");
                entity.Property(e => e.BankAccountNo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                
                // Configure relationships
                entity.HasOne(e => e.Department)
                      .WithMany()
                      .HasForeignKey(e => e.DepartmentId)
                      .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete
                
                entity.HasOne(e => e.Section)
                      .WithMany()
                      .HasForeignKey(e => e.SectionId)
                      .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete
                
                entity.HasOne(e => e.Designation)
                      .WithMany()
                      .HasForeignKey(e => e.DesignationId)
                      .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete
                
                // Create indexes for better query performance
                entity.HasIndex(e => e.Name);
                entity.HasIndex(e => e.NIDNo).IsUnique(); // NID should be unique
                entity.HasIndex(e => e.DepartmentId);
                entity.HasIndex(e => e.SectionId);
                entity.HasIndex(e => e.DesignationId);
                entity.HasIndex(e => e.JoiningDate);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.BankAccountNo);
            });

            // Seed default roles
            SeedRoles(builder);
        }

        private void SeedRoles(ModelBuilder builder)
        {
            var adminRoleId = Guid.NewGuid().ToString();
            var managerRoleId = Guid.NewGuid().ToString();
            var employeeRoleId = Guid.NewGuid().ToString();
            var itRoleId = Guid.NewGuid().ToString();
            var hrRoleId = Guid.NewGuid().ToString();
            var hrManagerRoleId = Guid.NewGuid().ToString();

            builder.Entity<ApplicationRole>().HasData(
                new ApplicationRole
                {
                    Id = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    Description = "System Administrator with full access",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new ApplicationRole
                {
                    Id = managerRoleId,
                    Name = "Manager",
                    NormalizedName = "MANAGER",
                    Description = "Manager with limited administrative access",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new ApplicationRole
                {
                    Id = employeeRoleId,
                    Name = "Employee",
                    NormalizedName = "EMPLOYEE",
                    Description = "Regular employee with basic access",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new ApplicationRole
                {
                    Id = itRoleId,
                    Name = "IT",
                    NormalizedName = "IT",
                    Description = "IT personnel with technical system access and user management capabilities",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new ApplicationRole
                {
                    Id = hrRoleId,
                    Name = "HR",
                    NormalizedName = "HR",
                    Description = "HR personnel with employee management and company-wide HR operations access",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new ApplicationRole
                {
                    Id = hrManagerRoleId,
                    Name = "HR Manager",
                    NormalizedName = "HR MANAGER",
                    Description = "Senior HR personnel with advanced HR management capabilities and strategic oversight",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                }
            );
        }
    }
}
