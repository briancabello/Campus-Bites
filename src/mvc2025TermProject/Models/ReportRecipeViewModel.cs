using System.ComponentModel.DataAnnotations;

namespace mvc2025TermProject.Models
{
    public class ReportRecipeViewModel
    {
        [Required]
        public int? RecipeId { get; set; }

        public string? RecipeName { get; set; }

        [Required]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Please provide more details (10-1000 characters)")]
        public string? Reason { get; set; }

        [Required]
        [Display(Name = "Name")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2-50 characters")]
        public string? ReporterName { get; set; }

        [Required]
        [Display(Name = "Email")]
        //[DataType(DataType.EmailAddress)]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Please enter a valid email address")]
        public string? ReporterEmail { get; set; }

        public string? RecipeUrl { get; set; }
    }
}
