using Microsoft.SqlServer.Server;
using System.ComponentModel.DataAnnotations;

namespace mvc2025TermProject.Models
{
    public class RecipeCreateViewModel
    {
        [Required]
        [Display(Name = "Category")]
        public int? CategoryId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Recipe name must be between 3 and 100 characters.")]
        [Display(Name = "Recipe Name")]
        public string? Name { get; set; }

        [Required]
        [StringLength(4000, MinimumLength = 10, ErrorMessage = "Instructions must be at least 10 characters.")]
        [Display(Name = "Step-by-Step Instructions")]
        public string? Instructions { get; set; }

        [StringLength(1000, ErrorMessage = "Tips cannot exceed 1000 characters.")]
        [Display(Name = "Tips & Notes")]
        public string? Tips { get; set; }

        [Required]
        [Display(Name = "Preparation Time (minutes)")]
        [Range(0, 1440, ErrorMessage = "Preparation time must be between 0 and 1440 minutes.")]
        public int? PreparationTime { get; set; }

        [Display(Name = "Cook Time (minutes)")]
        [Range(0, 1440, ErrorMessage = "Cook time must be between 0 and 1440 minutes.")]
        public int? CookTime { get; set; }

        [Display(Name = "Cooking Temperature (°F)")]
        [Range(-20, 550, ErrorMessage = "Temperature must be between -20°F (freezing) and 550°F.")]
        public int? TemperatureFahrenheit { get; set; }

        [Required]
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

        // Navigation Property
        public List<RecipeIngredientViewModel>? Ingredients { get; set; }
        public List<Category>? AvailableCategories { get; set; }
        public List<Ingredient>? AvailableIngredients { get; set; }

    }

    public class RecipeOverviewViewModel
    {
        public int? ID { get; set; }

        [Display(Name = "Recipe Name")]
        public string? Name { get; set; }

        public int? CategoryId { get; set; }

        [Display(Name = "Category")]
        public string? CategoryName { get; set; }

        [Display(Name = "Total Time")]
        public string? TotalTime { get; set; }

        [Display(Name = "Servings")]
        public string? Servings { get; set; }

        [Display(Name = "Status")]
        public string? Status { get; set; }

        [Display(Name = "Created")]
        [DataType(DataType.Date)]
        public DateTime? CreatedDate { get; set; }

        public string? OwnerId { get; set; }

        [Display(Name = "Owner")]
        public string? OwnerName { get; set; }

        public List<Image>? Images { get; set; }
    }

    public class RecipeDetailsViewModel
    {
        public int? ID { get; set; }

        [Display(Name = "Recipe Name")]
        public string? Name { get; set; }

        public int? CategoryId { get; set; }

        [Display(Name = "Category")]
        public string? CategoryName { get; set; }

        [Display(Name = "Instructions")]
        public string? Instructions { get; set; }

        [Display(Name = "Tips & Notes")]
        public string? Tips { get; set; }

        [Display(Name = "Prep Time")]
        public string? PreparationTime { get; set; }

        [Display(Name = "Cook Time")]
        public string? CookTime { get; set; }

        [Display(Name = "Cook Temp")]
        public string? TemperatureFahrenheit { get; set; }

        [Display(Name = "Total Time")]
        public string? TotalTime { get; set; }

        [Display(Name = "Servings")]
        public string? Servings { get; set; }

        // E2 ADDITIONAL FIELDS
        [Display(Name = "Nutrition Information")]
        public string? NutritionInfo { get; set; }

        [Display(Name = "Special Equipment Needed")]
        public string? SpecialEquipment { get; set; }

        [Display(Name = "YouTube Video Link")]
        public string? YouTubeVideoLink { get; set; }
        // END

        [Display(Name = "Status")]
        public string? Status { get; set; }

        public string? OwnerId { get; set; }

        [Display(Name = "Owner")]
        public string? OwnerName { get; set; }

        [Display(Name = "Created")]
        public string? CreatedDate { get; set; }

        [Display(Name = "Last Modified")]
        public string? LastModifiedDate { get; set; }

        public List<Image>? Images { get; set; }
        public List<RecipeIngredientDetailViewModel>? Ingredients { get; set; }

        public virtual ShareRecipeViewModel? ShareRecipe { get; set; }
        public virtual ReportRecipeViewModel? ReportRecipe { get; set; }
    }

    public class RecipeEditViewModel
    {
        public int? ID { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Recipe name must be between 3 and 100 characters.")]
        [Display(Name = "Recipe Name")]
        public string? Name { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int? CategoryId { get; set; }

        [Required]
        [StringLength(4000, MinimumLength = 10, ErrorMessage = "Instructions must be at least 10 characters.")]
        [Display(Name = "Step-by-Step Instructions")]
        public string? Instructions { get; set; }

        [StringLength(1000, ErrorMessage = "Tips cannot exceed 1000 characters.")]
        [Display(Name = "Tips & Notes")]
        public string? Tips { get; set; }

        [Required]
        [Display(Name = "Preparation Time (minutes)")]
        [Range(0, 1440, ErrorMessage = "Preparation time must be between 0 and 1440 minutes.")]
        public int? PreparationTime { get; set; }

        [Display(Name = "Cook Time (minutes)")]
        [Range(0, 1440, ErrorMessage = "Cook time must be between 0 and 1440 minutes.")]
        public int? CookTime { get; set; }

        [Display(Name = "Cooking Temperature (°F)")]
        [Range(80, 550, ErrorMessage = "Temperature must be between 80°F and 550°F.")]
        public int? TemperatureFahrenheit { get; set; }

        [Required]
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

        [Display(Name = "Status")]
        public string? Status { get; set; }

        [Display(Name = "Owner")]
        public string? OwnerId { get; set; }

        [Display(Name = "Created")]
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "Last Modified")]
        public DateTime? LastModifiedDate { get; set; }
    }

    public class RecipeIngredientViewModel
    {
        [Required]
        [Display(Name = "Recipe")]
        public int? RecipeId { get; set; }

        [Required]
        [Display(Name = "Ingredient")]
        public int? IngredientId { get; set; }

        [Required]
        [Range(0.01, 1000, ErrorMessage = "Quantity must be between 0.01 and 1000.")]
        [Display(Name = "Quantity")]
        [DisplayFormat(DataFormatString = "{0:0.##}")]
        public decimal? Quantity { get; set; } // e.g., "2 cups", "1 tablespoon"

        [Required]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Unit must be between 1 and 20 characters if provided.")]
        [Display(Name = "Unit")]
        public string? Unit { get; set; } // e.g., "cups", "tablespoons", "grams"

        [Display(Name = "Preparation Notes")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Preparation notes must be at least 5 characters if provided.")]
        public string? Notes { get; set; } // e.g., "chopped", "diced"
    }

    public class RecipeIngredientDetailViewModel
    {
        [Display(Name = "Ingredient")]
        public int? IngredientId { get; set; }

        [Display(Name = "Recipe")]
        public int? RecipeId { get; set; }

        [Display(Name = "Ingredient")]
        public string? IngredientName { get; set; }

        [Display(Name = "Type")]
        public string? IngredientType { get; set; }

        [Display(Name = "Quantity")]
        public string? Quantity { get; set; } // e.g., "2 cups", "1 tablespoon"

        [Display(Name = "Unit")]
        public string? Unit { get; set; } // e.g., "cups", "tablespoons", "grams"

        [Display(Name = "Preparation Notes")]
        public string? Notes { get; set; } // e.g., "chopped", "diced"
    }
}