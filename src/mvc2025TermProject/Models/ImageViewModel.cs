using System.ComponentModel.DataAnnotations;

namespace mvc2025TermProject.Models
{
    public class ImageUploadViewModel
    {
        public int? RecipeId { get; set; }

        [Display(Name = "Recipe Name")]
        public string? RecipeName { get; set; }

        public string? OwnerId { get; set; }

        [Display(Name = "Owner")]
        public string? OwnerName { get; set; }

        public string? CurrentUserId { get; set; }

        public string? RecipeStatus { get; set; }

        [Required]
        [Display(Name = "Image File")]
        public IFormFile? ImageFile { get; set; }

        [StringLength(255, MinimumLength = 5, ErrorMessage = "Alt text must be between 5 and 255 characters.")]
        [Display(Name = "Alternative Text")]
        public string? AltText { get; set; }

        [StringLength(500, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 500 characters.")]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Set as main image")]
        public bool IsMainImage { get; set; }

        public List<Image>? Images { get; set; }
    }
}
