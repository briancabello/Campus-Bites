using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using mvc2025TermProject.Models;

namespace mvc2025TermProject.Data
{
    public class ApplicationDbContext : IdentityDbContext<CampusBitesUser>
    {
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
        public DbSet<Image> Images { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            // Recipe to Category (One-to-Many)
            builder.Entity<Recipe>()
                .HasOne(r => r.Category)
                .WithMany(c => c.Recipes)
                .HasForeignKey(r => r.CategoryId)
                .HasConstraintName("FK_Recipe_CategoryID");

            builder.Entity<Recipe>()
                .HasMany(r => r.RecipeIngredients)
                .WithOne(ri => ri.Recipe)
                .HasForeignKey(ri => ri.RecipeId)
                .HasConstraintName("FK_RecipeIngredient_RecipeID");

            // Ingredient to RecipeIngredients (One-to-Many)
            builder.Entity<Ingredient>()
                .HasMany(i => i.RecipeIngredients)
                .WithOne(ri => ri.Ingredient)
                .HasForeignKey(ri => ri.IngredientId)
                .HasConstraintName("FK_RecipeIngredient_IngredientID");

            // Composite Key (RecipeId, IngredientId)
            builder.Entity<RecipeIngredient>()
                .ToTable("RecipeIngredients")
                .HasKey(ri => new { ri.RecipeId, ri.IngredientId });

            builder.Entity<RecipeIngredient>()
                .Property(ri => ri.Quantity)
                .HasColumnType("decimal(18,3)");

            // Recipe to User/Owner (One-to-Many)
            builder.Entity<Recipe>()
                .HasOne(r => r.Owner)                    // Recipe has one Owner
                .WithMany(u => u.Recipes)                // Owner has many Recipes
                .HasForeignKey(r => r.OwnerId)           // Foreign key
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Recipe_OwnerID");

            // Image to Recipe (One-to-Many)  
            builder.Entity<Image>()
                .HasOne(i => i.Recipe)                   // Image has one Recipe
                .WithMany(r => r.Images)                 // Recipe has many Images
                .HasForeignKey(i => i.RecipeId)          // Foreign key
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Image_RecipeID");

            // Image to User/UploadedBy (One-to-Many)
            builder.Entity<Image>()
                .HasOne(i => i.UploadedBy)               // Image has one Uploader
                .WithMany(u => u.Images)                 // User has many Images
                .HasForeignKey(i => i.UploadedById)      // Foreign key
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Image_UploadedByID");
        }
    }
}
