using Microsoft.AspNetCore.Identity;
using mvc2025TermProject.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mvc2025TermProject.Models
{
    public class Recipe
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? ID { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Recipe name must be between 3 and 100 characters.")]
        [Display(Name = "Recipe Name")]
        public string? Name { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int? CategoryId { get; set; } // Foreign key to Category

        [Required]
        [StringLength(4000, MinimumLength = 10, ErrorMessage = "Instructions must be at least 10 characters.")]
        [Display(Name = "Instructions")]
        public string? Instructions { get; set; }

        [StringLength(1000, ErrorMessage = "Tips cannot exceed 1000 characters.")]
        [Display(Name = "Tips & Notes")]
        public string? Tips { get; set; }

        [Display(Name = "Preparation Time (minutes)")]
        [Range(0, 1440, ErrorMessage = "Preparation time must be between 0 and 1440 minutes.")]
        public int? PreparationTime { get; set; }

        [Display(Name = "Cook Time (minutes)")]
        [Range(0, 1440, ErrorMessage = "Cook time must be between 0 and 1440 minutes.")]
        public int? CookTime { get; set; }

        [Display(Name = "Cooking Temperature (°F)")]
        [Range(-20, 550, ErrorMessage = "Temperature must be between -20°F (freezing) and 550°F.")]
        public int? TemperatureFahrenheit { get; set; }

        [Display(Name = "Number of Servings")]
        [Range(1, 100, ErrorMessage = "Servings must be between 1 and 100.")]
        public int? Servings { get; set; }

        // E2 ADDITIONAL FIELDS
        [Display(Name = "Nutrition Information")]
        [StringLength(500, ErrorMessage = "Nutrition information cannot exceed 500 characters.")]
        public string? NutritionInfo { get; set; }

        [Display(Name = "Special Equipment Needed")]
        [StringLength(500, ErrorMessage = "Special equipment cannot exceed 500 characters.")]
        public string? SpecialEquipment { get; set; }

        [Display(Name = "YouTube Video Link")]
        [DataType(DataType.Url)]
        [StringLength(500, ErrorMessage = "Video link cannot exceed 500 characters.")]
        public string? YouTubeVideoLink { get; set; }
        // END

        [Required]
        [Display(Name = "Status")]
        public string? Status { get; set; }

        [Required]
        [Display(Name = "Owner")]
        public string? OwnerId { get; set; } // Foreign key to CampusBitesUser

        [DataType(DataType.DateTime)]
        [Display(Name = "Created")]
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "Last Modified")]
        [DataType(DataType.DateTime)]
        public DateTime? LastModifiedDate { get; set; }

        // Navigation property
        public virtual CampusBitesUser? Owner { get; set; }
        public virtual Category? Category { get; set; }
        public virtual ICollection<RecipeIngredient>? RecipeIngredients { get; set; }
        public virtual ICollection<Image>? Images { get; set; }
    }

    public enum RecipeStatus
    {
        Draft,
        Private,
        Public,
        Unlisted
    }

    public class RecipeIngredient
    {
        [Required]
        [Display(Name = "Recipe")]
        public int? RecipeId { get; set; } // Foreign key to Recipe

        [Required]
        [Display(Name = "Ingredient")]
        public int? IngredientId { get; set; } // Foreign key to Ingredient

        [Required]
        [Range(0.01, 1000, ErrorMessage = "Quantity must be between 0.01 and 1000.")]
        public decimal? Quantity { get; set; } // e.g., "2 cups", "1 tablespoon"

        [Required]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Unit must be between 1 and 20 characters if provided.")]
        public string? Unit { get; set; } // e.g., "cups", "tablespoons", "grams"

        [Display(Name = "Preparation Notes")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Preparation notes must be at least 5 characters if provided.")]
        public string? Notes { get; set; } // e.g., "chopped", "diced"

        public int? SortOrder { get; set; }

        // Navigation property
        public virtual Recipe? Recipe { get; set; }
        public virtual Ingredient? Ingredient { get; set; }
    }
}
