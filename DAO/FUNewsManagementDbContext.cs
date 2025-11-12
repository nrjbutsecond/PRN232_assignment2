using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class FUNewsManagementDbContext : DbContext
    {
        public FUNewsManagementDbContext()
        {
        }

        public FUNewsManagementDbContext(DbContextOptions<FUNewsManagementDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public virtual DbSet<SystemAccount> SystemAccounts { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<NewsArticle> NewsArticles { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<NewsTag> NewsTags { get; set; }

        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ========================= SystemAccount Configuration =========================
            modelBuilder.Entity<SystemAccount>(entity =>
            {
                entity.ToTable("SystemAccount");
                entity.HasKey(e => e.AccountId);

                entity.Property(e => e.AccountId)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.AccountName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.AccountEmail)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasIndex(e => e.AccountEmail)
                    .IsUnique()
                    .HasDatabaseName("IX_SystemAccount_AccountEmail_Unique");

                entity.Property(e => e.AccountPassword)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.AccountRole)
                    .IsRequired()
                    .HasComment("0 = Admin (from appsettings), 1 = Staff, 2 = Lecturer");

                // Navigation: SystemAccount -> NewsArticles
                entity.HasMany(e => e.NewsArticles)
                    .WithOne(n => n.CreatedBy)
                    .HasForeignKey(n => n.CreatedById)
                    .OnDelete(DeleteBehavior.Restrict); 
            });

            // ========================= Category Configuration =========================
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");
                entity.HasKey(e => e.CategoryId);

                entity.Property(e => e.CategoryId)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.CategoryName)
                    .IsRequired()
                    .HasMaxLength(100);

                // Unique constraint cho CategoryName
                entity.HasIndex(e => e.CategoryName)
                    .IsUnique()
                    .HasDatabaseName("IX_Category_CategoryName_Unique");

                entity.Property(e => e.CategoryDescrip)
                    .HasMaxLength(500);

                entity.Property(e => e.ParentCategoryId)
                    .IsRequired(false);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true)
                    .HasComment("1 = Active, 0 = Inactive");

                // Self-referencing relationship: Category -> ParentCategory
                entity.HasOne(e => e.ParentCategory)
                    .WithMany(e => e.SubCategories)
                    .HasForeignKey(e => e.ParentCategoryId)
                    .OnDelete(DeleteBehavior.Restrict); 

                // Navigation: Category -> NewsArticles
                entity.HasMany(e => e.NewsArticles)
                    .WithOne(n => n.Category)
                    .HasForeignKey(n => n.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict); 
            });

            // ========================= NewsArticle Configuration =========================
            modelBuilder.Entity<NewsArticle>(entity =>
            {
                entity.ToTable("NewsArticle");
                entity.HasKey(e => e.NewsArticleId);

                entity.Property(e => e.NewsTitle)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Headline)
                    .HasMaxLength(500);

                entity.Property(e => e.NewsContent)
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                entity.Property(e => e.NewsSource)
                    .HasMaxLength(200);

                entity.Property(e => e.NewsStatus)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.Property(e => e.CreatedDate)
                    .IsRequired()
                    .HasDefaultValueSql("GETDATE()");

                // Foreign keys
                entity.HasOne(e => e.Category)
                    .WithMany(c => c.NewsArticles)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.CreatedBy)
                    .WithMany(a => a.NewsArticles)
                    .HasForeignKey(e => e.CreatedById)
                    .OnDelete(DeleteBehavior.Restrict);

                // Indexes
                entity.HasIndex(e => e.CategoryId);
                entity.HasIndex(e => e.CreatedById);
                entity.HasIndex(e => e.NewsStatus);
                entity.HasIndex(e => e.CreatedDate);
            });


            // ========================= Tag Configuration =========================
            modelBuilder.Entity<Tag>(entity =>
            {
                entity.ToTable("Tag");
                entity.HasKey(e => e.TagId);

                entity.Property(e => e.TagName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Note)
                    .HasMaxLength(255);

                // Index
                entity.HasIndex(e => e.TagName);
            });

            modelBuilder.Entity<NewsTag>(entity =>
            {
                entity.ToTable("NewsTag");

                // Composite Primary Key
                entity.HasKey(nt => new { nt.NewsArticleId, nt.TagId });

                // Foreign Key: NewsTag -> NewsArticle
                entity.HasOne(nt => nt.NewsArticle)
                    .WithMany(n => n.NewsTags)
                    .HasForeignKey(nt => nt.NewsArticleId)
                    .OnDelete(DeleteBehavior.Cascade); 

                // Foreign Key: NewsTag -> Tag
                entity.HasOne(nt => nt.Tag)
                    .WithMany(t => t.NewsTags)
                    .HasForeignKey(nt => nt.TagId)
                    .OnDelete(DeleteBehavior.Restrict); 

                // Indexes
                entity.HasIndex(nt => nt.NewsArticleId);
                entity.HasIndex(nt => nt.TagId);
            });
        }
    }
}