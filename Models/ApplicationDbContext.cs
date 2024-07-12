using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JWTRefreshTokenInDotNet6.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Lecture>()
           .HasOne(l => l.QrCodeImages)
           .WithOne(q => q.Lecture)
           .HasForeignKey<QrCodeImages>(q => q.lecId)
           .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<IdentityUserLogin<string>>()
        .HasKey(l => new { l.LoginProvider, l.ProviderKey });
        }
       
     
        public DbSet<QrCodeImages> QrCodeImages { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Reports> Reports { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Lecture> Lectures { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }
    }
}