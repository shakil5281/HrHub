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
        public DbSet<Line> Lines { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<Degree> Degrees { get; set; }
        public DbSet<RosterSchedule> RosterSchedules { get; set; }
        public DbSet<BangladeshAddress> BangladeshAddresses { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<PermissionGroup> PermissionGroups { get; set; }
        public new DbSet<UserRole> UserRoles { get; set; }
        public DbSet<ZkDevice> ZkDevices { get; set; }
        public DbSet<AttendanceLog> AttendanceLogs { get; set; }

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
                      .WithMany(c => c.Users)
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
                
                // Configure relationship with Company
                entity.HasOne(d => d.Company)
                      .WithMany()
                      .HasForeignKey(d => d.CompanyId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                // Create indexes for better query performance
                entity.HasIndex(e => e.Name);
                entity.HasIndex(e => e.CompanyId);
                entity.HasIndex(e => e.IsActive);
                
                // Create composite index for company + department name uniqueness
                entity.HasIndex(e => new { e.CompanyId, e.Name })
                      .IsUnique()
                      .HasDatabaseName("IX_Department_CompanyId_Name");
            });

            // Configure Line entity
            builder.Entity<Line>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                
                // Configure relationship with Company
                entity.HasOne(l => l.Company)
                      .WithMany()
                      .HasForeignKey(l => l.CompanyId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                // Create indexes for better query performance
                entity.HasIndex(e => e.Name);
                entity.HasIndex(e => e.CompanyId);
                entity.HasIndex(e => e.IsActive);
                
                // Create composite index for company + line name uniqueness
                entity.HasIndex(e => new { e.CompanyId, e.Name })
                      .IsUnique()
                      .HasDatabaseName("IX_Line_CompanyId_Name");
            });

            // Configure Shift entity
            builder.Entity<Shift>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                
                // Configure relationship with Company
                entity.HasOne(s => s.Company)
                      .WithMany()
                      .HasForeignKey(s => s.CompanyId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                // Create indexes for better query performance
                entity.HasIndex(e => e.Name);
                entity.HasIndex(e => e.CompanyId);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.StartTime);
                entity.HasIndex(e => e.EndTime);
                
                // Create composite index for company + shift name uniqueness
                entity.HasIndex(e => new { e.CompanyId, e.Name })
                      .IsUnique()
                      .HasDatabaseName("IX_Shift_CompanyId_Name");
            });

            // Configure Degree entity
            builder.Entity<Degree>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Level).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                
                // Configure relationship with Company
                entity.HasOne(d => d.Company)
                      .WithMany()
                      .HasForeignKey(d => d.CompanyId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                // Create indexes for better query performance
                entity.HasIndex(e => e.Name);
                entity.HasIndex(e => e.Level);
                entity.HasIndex(e => e.CompanyId);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.InstitutionType);
                
                // Create composite index for company + degree name uniqueness
                entity.HasIndex(e => new { e.CompanyId, e.Name })
                      .IsUnique()
                      .HasDatabaseName("IX_Degree_CompanyId_Name");
            });

            // Configure RosterSchedule entity
            builder.Entity<RosterSchedule>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                
                // Configure relationships
                entity.HasOne(rs => rs.Employee)
                      .WithMany()
                      .HasForeignKey(rs => rs.EmployeeId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(rs => rs.Shift)
                      .WithMany()
                      .HasForeignKey(rs => rs.ShiftId)
                      .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete for shifts
                
                entity.HasOne(rs => rs.Company)
                      .WithMany()
                      .HasForeignKey(rs => rs.CompanyId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                // Create indexes for better query performance
                entity.HasIndex(e => e.EmployeeId);
                entity.HasIndex(e => e.ShiftId);
                entity.HasIndex(e => e.CompanyId);
                entity.HasIndex(e => e.ScheduleDate);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.CheckInTime);
                entity.HasIndex(e => e.CheckOutTime);
                
                // Create composite indexes for common queries
                entity.HasIndex(e => new { e.EmployeeId, e.ScheduleDate })
                      .IsUnique()
                      .HasDatabaseName("IX_RosterSchedule_EmployeeId_ScheduleDate");
                
                entity.HasIndex(e => new { e.CompanyId, e.ScheduleDate })
                      .HasDatabaseName("IX_RosterSchedule_CompanyId_ScheduleDate");
                
                entity.HasIndex(e => new { e.ShiftId, e.ScheduleDate })
                      .HasDatabaseName("IX_RosterSchedule_ShiftId_ScheduleDate");
            });

            // Configure BangladeshAddress entity
            builder.Entity<BangladeshAddress>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Division).IsRequired().HasMaxLength(100);
                entity.Property(e => e.District).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PostalCode).IsRequired().HasMaxLength(10);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                
                // Create indexes for better query performance
                entity.HasIndex(e => e.Division);
                entity.HasIndex(e => e.District);
                entity.HasIndex(e => e.PostalCode);
                entity.HasIndex(e => e.Upazila);
                entity.HasIndex(e => e.Union);
                entity.HasIndex(e => e.IsActive);
                
                // Create composite indexes for common queries
                entity.HasIndex(e => new { e.Division, e.District })
                      .HasDatabaseName("IX_BangladeshAddress_Division_District");
                
                entity.HasIndex(e => new { e.District, e.PostalCode })
                      .HasDatabaseName("IX_BangladeshAddress_District_PostalCode");
                
                entity.HasIndex(e => new { e.Division, e.District, e.Upazila })
                      .HasDatabaseName("IX_BangladeshAddress_Division_District_Upazila");
                
                // Unique constraint on postal code
                entity.HasIndex(e => e.PostalCode)
                      .IsUnique()
                      .HasDatabaseName("IX_BangladeshAddress_PostalCode_Unique");
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
                entity.Property(e => e.EmpId).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.NIDNo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.FatherName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.MotherName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.PermanentAddress).IsRequired().HasMaxLength(500);
                entity.Property(e => e.PermanentDivision).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PermanentDistrict).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PermanentUpazila).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PermanentPostalCode).IsRequired().HasMaxLength(20);
                entity.Property(e => e.PresentAddress).IsRequired().HasMaxLength(500);
                entity.Property(e => e.PresentDivision).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PresentDistrict).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PresentUpazila).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PresentPostalCode).IsRequired().HasMaxLength(20);
                entity.Property(e => e.BloodGroup).HasMaxLength(10);
                entity.Property(e => e.Gender).HasMaxLength(50);
                entity.Property(e => e.Religion).HasMaxLength(50);
                entity.Property(e => e.MaritalStatus).HasMaxLength(50);
                entity.Property(e => e.Education).HasMaxLength(200);
                entity.Property(e => e.Floor).HasMaxLength(50);
                entity.Property(e => e.EmpType).HasMaxLength(50);
                entity.Property(e => e.Group).HasMaxLength(50);
                entity.Property(e => e.SalaryType).HasMaxLength(50);
                entity.Property(e => e.Bank).HasMaxLength(100);
                entity.Property(e => e.GrossSalary).HasColumnType("decimal(18,2)");
                entity.Property(e => e.BasicSalary).HasColumnType("decimal(18,2)");
                entity.Property(e => e.House).HasColumnType("decimal(18,2)");
                entity.Property(e => e.RentMedical).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Food).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Conveyance).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Transport).HasColumnType("decimal(18,2)");
                entity.Property(e => e.NightBill).HasColumnType("decimal(18,2)");
                entity.Property(e => e.MobileBill).HasColumnType("decimal(18,2)");
                entity.Property(e => e.OtherAllowance).HasColumnType("decimal(18,2)");
                entity.Property(e => e.BankAccountNo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                
                // Configure relationships
                entity.HasOne(e => e.Company)
                      .WithMany(c => c.Employees)
                      .HasForeignKey(e => e.CompanyId)
                      .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete
                
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
                
                entity.HasOne(e => e.Line)
                      .WithMany()
                      .HasForeignKey(e => e.LineId)
                      .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete
                
                entity.HasOne(e => e.Shift)
                      .WithMany()
                      .HasForeignKey(e => e.ShiftId)
                      .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete
                
                entity.HasOne(e => e.Degree)
                      .WithMany()
                      .HasForeignKey(e => e.DegreeId)
                      .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete
                
                // Create indexes for better query performance
                entity.HasIndex(e => e.EmpId).IsUnique(); // EmpId should be unique
                entity.HasIndex(e => e.Name);
                entity.HasIndex(e => e.NIDNo).IsUnique(); // NID should be unique
                entity.HasIndex(e => e.CompanyId);
                entity.HasIndex(e => e.DepartmentId);
                entity.HasIndex(e => e.SectionId);
                entity.HasIndex(e => e.DesignationId);
                entity.HasIndex(e => e.LineId);
                entity.HasIndex(e => e.ShiftId);
                entity.HasIndex(e => e.DegreeId);
                entity.HasIndex(e => e.JoiningDate);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.BankAccountNo);
                entity.HasIndex(e => e.BloodGroup);
                entity.HasIndex(e => e.Gender);
                entity.HasIndex(e => e.Religion);
                entity.HasIndex(e => e.MaritalStatus);
                entity.HasIndex(e => e.EmpType);
                entity.HasIndex(e => e.Group);
                entity.HasIndex(e => e.SalaryType);
                entity.HasIndex(e => e.Bank);
                entity.HasIndex(e => e.PermanentDivision);
                entity.HasIndex(e => e.PermanentDistrict);
                entity.HasIndex(e => e.PermanentUpazila);
                entity.HasIndex(e => e.PresentDivision);
                entity.HasIndex(e => e.PresentDistrict);
                entity.HasIndex(e => e.PresentUpazila);
            });

            // Configure Permission entity
            builder.Entity<Permission>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(200);
                entity.Property(e => e.Module).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Action).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Resource).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                
                // Create indexes for better query performance
                entity.HasIndex(e => e.Code).IsUnique();
                entity.HasIndex(e => e.Name);
                entity.HasIndex(e => e.Module);
                entity.HasIndex(e => e.Action);
                entity.HasIndex(e => e.Resource);
                entity.HasIndex(e => e.IsActive);
                
                // Create composite indexes for common queries
                entity.HasIndex(e => new { e.Module, e.Action })
                      .HasDatabaseName("IX_Permission_Module_Action");
                
                entity.HasIndex(e => new { e.Module, e.Resource })
                      .HasDatabaseName("IX_Permission_Module_Resource");
            });

            // Configure RolePermission entity
            builder.Entity<RolePermission>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.AssignedAt).HasDefaultValueSql("GETUTCDATE()");
                
                // Configure relationships
                entity.HasOne(rp => rp.Role)
                      .WithMany()
                      .HasForeignKey(rp => rp.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(rp => rp.Permission)
                      .WithMany(p => p.RolePermissions)
                      .HasForeignKey(rp => rp.PermissionId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                // Create indexes for better query performance
                entity.HasIndex(e => e.RoleId);
                entity.HasIndex(e => e.PermissionId);
                entity.HasIndex(e => e.IsGranted);
                entity.HasIndex(e => e.ExpiresAt);
                
                // Create composite unique index to prevent duplicate assignments
                entity.HasIndex(e => new { e.RoleId, e.PermissionId })
                      .IsUnique()
                      .HasDatabaseName("IX_RolePermission_RoleId_PermissionId");
            });

            // Configure UserPermission entity
            builder.Entity<UserPermission>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.AssignedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.Reason).HasMaxLength(200);
                
                // Configure relationships
                entity.HasOne(up => up.User)
                      .WithMany()
                      .HasForeignKey(up => up.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(up => up.Permission)
                      .WithMany(p => p.UserPermissions)
                      .HasForeignKey(up => up.PermissionId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                // Create indexes for better query performance
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.PermissionId);
                entity.HasIndex(e => e.IsGranted);
                entity.HasIndex(e => e.ExpiresAt);
                
                // Create composite unique index to prevent duplicate assignments
                entity.HasIndex(e => new { e.UserId, e.PermissionId })
                      .IsUnique()
                      .HasDatabaseName("IX_UserPermission_UserId_PermissionId");
            });

            // Configure PermissionGroup entity
            builder.Entity<PermissionGroup>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(200);
                entity.Property(e => e.Module).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                
                // Create indexes for better query performance
                entity.HasIndex(e => e.Name);
                entity.HasIndex(e => e.Module);
                entity.HasIndex(e => e.IsActive);
                
                // Create composite index for module + name uniqueness
                entity.HasIndex(e => new { e.Module, e.Name })
                      .IsUnique()
                      .HasDatabaseName("IX_PermissionGroup_Module_Name");
            });

            // Configure UserRole entity
            builder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.AssignedAt).HasDefaultValueSql("GETUTCDATE()");
                
                // Configure relationships
                entity.HasOne(ur => ur.User)
                      .WithMany()
                      .HasForeignKey(ur => ur.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ur => ur.Role)
                      .WithMany()
                      .HasForeignKey(ur => ur.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                // Create indexes for better query performance
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.RoleId);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.ExpiresAt);
                
                // Create composite unique index to prevent duplicate assignments
                entity.HasIndex(e => new { e.UserId, e.RoleId })
                      .IsUnique()
                      .HasDatabaseName("IX_UserRole_UserId_RoleId");
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

            // Configure ZkDevice entity
            builder.Entity<ZkDevice>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DeviceName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.IpAddress).IsRequired().HasMaxLength(15);
                entity.Property(e => e.SerialNumber).HasMaxLength(50);
                entity.Property(e => e.ProductName).HasMaxLength(50);
                entity.Property(e => e.MachineNumber).HasMaxLength(50);
                entity.Property(e => e.Location).HasMaxLength(100);
                entity.Property(e => e.CreatedBy).HasMaxLength(50);
                entity.Property(e => e.UpdatedBy).HasMaxLength(50);
                entity.HasIndex(e => e.IpAddress).IsUnique();
                entity.HasIndex(e => e.SerialNumber).IsUnique();
            });

            // Configure AttendanceLog entity
            builder.Entity<AttendanceLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.EmployeeId).IsRequired().HasMaxLength(50);
                entity.Property(e => e.EmployeeName).HasMaxLength(200);
                entity.Property(e => e.LogType).IsRequired().HasMaxLength(20);
                entity.Property(e => e.VerificationMode).HasMaxLength(20);
                entity.Property(e => e.WorkCode).HasMaxLength(20);
                entity.Property(e => e.Remarks).HasMaxLength(500);
                entity.Property(e => e.CreatedBy).HasMaxLength(50);
                entity.Property(e => e.UpdatedBy).HasMaxLength(50);
                
                // Foreign key relationship
                entity.HasOne(e => e.ZkDevice)
                      .WithMany(d => d.AttendanceLogs)
                      .HasForeignKey(e => e.ZkDeviceId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Indexes for better performance
                entity.HasIndex(e => new { e.ZkDeviceId, e.EmployeeId, e.LogTime });
                entity.HasIndex(e => e.LogTime);
                entity.HasIndex(e => e.EmployeeId);
            });
        }
    }
}
