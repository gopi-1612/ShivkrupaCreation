using KurtiWholesale.Api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShivkrupaCreation.API.Models;

namespace ShivkrupaCreation.API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Brand> Brands => Set<Brand>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductImage> ProductImages => Set<ProductImage>();
        public DbSet<ProductVideo> ProductVideos => Set<ProductVideo>();
        public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ---------- Category ----------
            builder.Entity<Category>(entity =>
            {
                entity.HasIndex(c => c.Slug).IsUnique();
                entity.Property(c => c.Name).HasMaxLength(150).IsRequired();
                entity.Property(c => c.Slug).HasMaxLength(180).IsRequired();
            });

            // ---------- Brand ----------
            builder.Entity<Brand>(entity =>
            {
                entity.HasIndex(b => b.Slug).IsUnique();
                entity.Property(b => b.Name).HasMaxLength(150).IsRequired();
            });

            // ---------- Product ----------
            builder.Entity<Product>(entity =>
            {
                entity.HasIndex(p => p.Slug).IsUnique();
                entity.HasIndex(p => p.Sku).IsUnique();

                entity.Property(p => p.Name).HasMaxLength(200).IsRequired();
                entity.Property(p => p.Sku).HasMaxLength(50).IsRequired();

                entity.Property(p => p.Mrp).HasColumnType("decimal(10,2)");
                entity.Property(p => p.WholesalePrice).HasColumnType("decimal(10,2)");

                entity.HasOne(p => p.Category)
                      .WithMany(c => c.Products)
                      .HasForeignKey(p => p.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Brand)
                      .WithMany(b => b.Products)
                      .HasForeignKey(p => p.BrandId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ---------- ProductImage ----------
            builder.Entity<ProductImage>(entity =>
            {
                entity.HasOne(pi => pi.Product)
                      .WithMany(p => p.Images)
                      .HasForeignKey(pi => pi.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(pi => pi.ImageUrl).HasMaxLength(500).IsRequired();
            });

            // ---------- ProductVideo ----------
            builder.Entity<ProductVideo>(entity =>
            {
                entity.HasOne(pv => pv.Product)
                      .WithMany(p => p.Videos)
                      .HasForeignKey(pv => pv.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(pv => pv.VideoUrl).HasMaxLength(500).IsRequired();
            });

            // ---------- ProductVariant ----------
            builder.Entity<ProductVariant>(entity =>
            {
                entity.HasOne(v => v.Product)
                      .WithMany(p => p.Variants)
                      .HasForeignKey(v => v.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(v => v.Sku).HasMaxLength(60).IsRequired();
                entity.Property(v => v.PriceOverride).HasColumnType("decimal(10,2)");

                entity.HasIndex(v => v.Sku).IsUnique();
            });

            // ---------- RefreshToken ----------
            builder.Entity<RefreshToken>(entity =>
            {
                entity.HasOne(rt => rt.User)
                      .WithMany()
                      .HasForeignKey(rt => rt.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(rt => rt.Token).HasMaxLength(200).IsRequired();
                entity.HasIndex(rt => rt.Token).IsUnique();
            });

            // ---------- ApplicationUser ----------
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(u => u.FullName).HasMaxLength(150);
            });
        }
    }
}
