using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudyAndChill.API.Models;

namespace StudyAndChill.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DbSet<User> Users { get; set; }
        public DbSet<UserInvitation> UserInvitations { get; set; }
        public DbSet<StudentProfile> StudentProfiles { get; set; }
        public DbSet<TeacherAvailability> TeacherAvailabilities { get; set; }
        public DbSet<ClassSession> ClassSessions { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Holiday> Holidays { get; set; }
        public DbSet<TeacherHolidayWork> TeacherHolidayWorks { get; set; }
        public DbSet<TeacherProfile> TeacherProfiles { get; set; }
        public DbSet<TeacherFinancialRecord> TeacherFinancialRecords { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ClassSession>()
                .HasMany(cs => cs.Students)
                .WithMany(u => u.ClassSessions)
                .UsingEntity(j => j.ToTable("ClassSessionUser"));

            modelBuilder.Entity<User>()
                .HasMany(u => u.TaughtClasses)
                .WithOne(cs => cs.Teacher)
                .HasForeignKey(cs => cs.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Contract>()
                .HasMany(c => c.ClassSessions)
                .WithOne(cs => cs.Contract)
                .HasForeignKey(cs => cs.ContractId);

            modelBuilder.Entity<User>()
                .HasMany<Contract>()
                .WithOne(c => c.Student)
                .HasForeignKey(c => c.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany<Contract>()
                .WithOne(c => c.Teacher)
                .HasForeignKey(c => c.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne<StudentProfile>()
                .WithOne(sp => sp.User)
                .HasForeignKey<StudentProfile>(sp => sp.UserId);

            modelBuilder.Entity<Holiday>()
                .HasMany(h => h.AffectedUsers)
                .WithMany(u => u.SpecificHoliday)
                .UsingEntity(j => j.ToTable("UserHolidays"));

            modelBuilder.Entity<TeacherHolidayWork>()
                .HasIndex(t => new { t.TeacherId, t.Date })
                .IsUnique();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var auditiEntries = OnBeforeSaveChanges();

            var result = await base.SaveChangesAsync(cancellationToken);

            if (auditiEntries.Any())
            {
                await OnAfterSaveChanges(auditiEntries);
            }

            return result;
        }

        private List<AuditEntry> OnBeforeSaveChanges()
        {
            ChangeTracker.DetectChanges();
            var auditiEntries = new List<AuditEntry>();

            var userIdString = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int? userId = null;
            if (int.TryParse(userIdString, out int id)) userId = id;

            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is AuditLog || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;

                var auditEntry = new AuditEntry(entry);
                auditEntry.TableName = entry.Entity.GetType().Name;
                auditEntry.UserId = userId;
                auditiEntries.Add(auditEntry);

                foreach (var property in entry.Properties)
                {
                    if (property.IsTemporary)
                    {
                        auditEntry.TemporaryProperties.Add(property);
                        continue;
                    }

                    string propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue;
                    }

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.AuditType = "Create";
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                            break;

                        case EntityState.Deleted:
                            auditEntry.AuditType = "Delete";
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                            break;

                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                auditEntry.AuditType = "Update";
                                auditEntry.OldValues[propertyName] = property.OriginalValue;
                                auditEntry.NewValues[propertyName] = property.CurrentValue;
                            }
                            break;
                    }
                }
            }

            foreach (var auditEntry in auditiEntries.Where(e => !e.HasTemporaryProperties))
            {
                AuditLogs.Add(auditEntry.ToAudit());
            }

            return auditiEntries.Where(e => e.HasTemporaryProperties).ToList();
        }

        private Task OnAfterSaveChanges(List<AuditEntry> auditEntries)
        {
            if (auditEntries == null || auditEntries.Count == 0) return Task.CompletedTask;

            foreach (var auditEntry in auditEntries)
            {
                foreach (var prop in auditEntry.TemporaryProperties)
                {
                    if (prop.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                    else
                    {
                        auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                }

                AuditLogs.Add(auditEntry.ToAudit());
            }

            return base.SaveChangesAsync();
        }

        public class AuditEntry
        {
            public AuditEntry(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
            {
                Entry = entry;
            }

            public Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry Entry { get; }
            public int? UserId { get; set; }
            public string TableName { get; set; }
            public Dictionary<string, object> KeyValues { get; } = new();
            public Dictionary<string, object> OldValues { get; } = new();
            public Dictionary<string, object> NewValues { get; } = new();
            public List<Microsoft.EntityFrameworkCore.ChangeTracking.PropertyEntry> TemporaryProperties { get; } = new();
            public string AuditType { get; set; }
            public bool HasTemporaryProperties => TemporaryProperties.Any();

            public AuditLog ToAudit()
            {
                var audit = new AuditLog
                {
                    UserId = UserId,
                    Type = AuditType,
                    TableName = TableName,
                    DateTime = DateTime.UtcNow,
                    PrimaryKey = JsonSerializer.Serialize(KeyValues),
                    OldValues = OldValues.Count == 0 ? null : JsonSerializer.Serialize(OldValues),
                    NewValues = NewValues.Count == 0 ? null : JsonSerializer.Serialize(NewValues)
                };
                return audit;
            }

        }


    }
}