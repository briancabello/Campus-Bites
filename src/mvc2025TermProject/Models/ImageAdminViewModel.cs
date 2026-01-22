using System.ComponentModel.DataAnnotations;

namespace mvc2025TermProject.Models
{
    // ===================================
    // Index Page View Models
    // ===================================

    /// <summary>
    /// Main view model for ImagesAdmin/Index page
    /// </summary>
    public class ImageAdminIndexViewModel
    {
        public PaginatedList<RecipeImageReviewViewModel>? Recipes { get; set; }
        public string? StatusFilter { get; set; }
        public int PendingCount { get; set; }
        public int ApprovedCount { get; set; }
        public int TotalCount { get; set; }
    }

    /// <summary>
    /// Represents a recipe card on the Index page
    /// </summary>
    public class RecipeImageReviewViewModel
    {
        public int? RecipeId { get; set; }

        [Display(Name = "Recipe Name")]
        public string? RecipeName { get; set; }

        [Display(Name = "Owner")]
        public string? RecipeOwnerName { get; set; }

        [Display(Name = "Pending Images")]
        public int PendingImageCount { get; set; }

        [Display(Name = "Approved Images")]
        public int ApprovedImageCount { get; set; }

        [Display(Name = "Total Images")]
        public int TotalImageCount { get; set; }

        public string? MainImagePath { get; set; }

        public DateTime? LatestUploadDate { get; set; }
    }

    // ===================================
    // Review Page View Model
    // ===================================

    /// <summary>
    /// View model for ImagesAdmin/Review page
    /// Shows one image at a time with navigation
    /// </summary>
    public class ImageReviewViewModel
    {
        public int RecipeId { get; set; }

        [Display(Name = "Recipe Name")]
        public string? RecipeName { get; set; }

        [Display(Name = "Recipe Owner")]
        public string? RecipeOwnerName { get; set; }

        // Current image being reviewed
        public Image? CurrentImage { get; set; }

        // Navigation
        public int CurrentImageIndex { get; set; }
        public int TotalImages { get; set; }
        public bool HasPreviousImage { get; set; }
        public bool HasNextImage { get; set; }

        // All images for thumbnail strip
        public List<Image>? AllImageThumbnails { get; set; }
    }
}
