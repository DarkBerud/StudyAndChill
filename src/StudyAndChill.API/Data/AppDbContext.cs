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
                .HasForeignKey(cs => cs.TeacherId);
        }
    }
}