using MedicalStock.Api.Models;
using Microsoft.EntityFrameworkCore;
using Namotion.Reflection;

namespace MedicalStock.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Product> Products => Set<Product>();

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

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(product => product.Id);

                entity.Property(product => product.Name)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(product => product.Barcode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(product => product.Manufacturer)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(product => product.Price)
                    .HasPrecision(10, 2);

                entity.HasIndex(product => product.Barcode)
                    .IsUnique();

                entity.HasOne(product => product.Category)
                    .WithMany(category => category.Products)
                    .HasForeignKey(product => product.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
