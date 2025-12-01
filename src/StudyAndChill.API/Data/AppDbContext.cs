using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudyAndChill.API.Models;

namespace StudyAndChill.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
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
    }
}