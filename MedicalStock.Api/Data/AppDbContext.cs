using MedicalStock.Api.Models;
using Microsoft.EntityFrameworkCore;
using Namotion.Reflection;

namespace MedicalStock.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Category> Categories => Set<Category>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

                entity.HasIndex(c => c.Name)
                .IsUnique();
            });
        }
    }
}
