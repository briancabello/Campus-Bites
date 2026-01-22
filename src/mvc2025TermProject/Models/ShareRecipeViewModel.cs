using System.ComponentModel.DataAnnotations;

namespace mvc2025TermProject.Models
{
    public class ShareRecipeViewModel
    {
        [Required]
        public int? RecipeId { get; set; }

        [Required]
        public string? RecipeName { get; set; }

        [Required]
        [Display(Name = "Email")]
        //[DataType(DataType.EmailAddress)]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Please enter a valid email address")]
        public string? RecipientEmail { get; set; }

        public string? Subject { get; set; }
        public string? Message { get; set; }
        public string? RecipeUrl { get; set; }
    }
}
