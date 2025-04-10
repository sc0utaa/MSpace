using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MSpace2.Data.Entities;

namespace MSpace2.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Albums> Albums { get; set; } = default!;
        public DbSet<AlbumRating> AlbumRatings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Create unique constraint so a user can only rate an album once
            modelBuilder.Entity<AlbumRating>()
                .HasIndex(r => new { r.AlbumId, r.UserId })
                .IsUnique();
        }
    }
}