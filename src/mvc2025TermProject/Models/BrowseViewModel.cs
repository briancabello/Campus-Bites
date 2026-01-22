using Microsoft.AspNetCore.Mvc.Rendering;

namespace mvc2025TermProject.Models
{
    public class BrowseViewModel
    {
        // Search Form Properties
        public string? Keyword { get; set; }
        public int? SelectedCategoryId { get; set; }

        public DateTime? StartDate { get; set; }      // Start of date range
        public DateTime? EndDate { get; set; }        // End of date range


        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalItemCount { get; set; }

        public List<RecipeCardViewModel>? TastyOnBudget { get; set; }
        public List<RecipeCardViewModel>? SweetSavorySavings { get; set; }
        public List<CategoryCardViewModel>? PopularCategories { get; set; }
        public List<RecipeCardViewModel>? ChefsPick { get; set; }
        public List<RecipeCardViewModel>? OurPicks { get; set; }

        public List<SelectListItem>? Category { get; set; }
        public List<RecipeCardViewModel>? LatestRecipes { get; set; }
        public List<CategoryCardViewModel>? FeaturedCategories { get; set; }
        public List<RecipeCardViewModel>? SearchResults { get; set; }
    }

    public class RecipeCardViewModel
    {
        public int? ID { get; set; }
        public string? Name { get; set; }
        public string? CategoryName { get; set; }
        public string? TotalTime { get; set; }
        public int? Servings { get; set; }
        public string? OwnerId { get; set; }
        public string? OwnerName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? Description { get; set; }
        public List<Image>? Images { get; set; }
    }

    public class CategoryCardViewModel
    {
        public int? ID { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public int? RecipeCount { get; set; }
    }
}
