using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mvc2025TermProject.Models
{
    public class Contact
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2-50 characters")]
        public string? Name { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        [Display(Name = "Email Address")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Please select a topic")]
        [Display(Name = "What's this about?")]
        public ContactTopic? Topic { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Subject must be between 5-100 characters")]
        [Display(Name = "Subject")]
        public string? Subject { get; set; }

        [Required]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Please provide more details (10-1000 characters)")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Your Message")]
        public string? Message { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date Submitted")]
        public DateTime? SubmittedAt { get; set; } = DateTime.Now;

        [Display(Name = "Status")]
        public bool? IsRead { get; set; } = false;
    }

    public enum ContactTopic
    {
        [Display(Name = "Recipe Help & Questions")]
        RecipeHelp,

        [Display(Name = "Suggest New Ingredient")]
        IngredientSuggestion,

        [Display(Name = "Suggest Food Category")]
        CategorySuggestion,

        [Display(Name = "Recipe Request")]
        RecipeRequest,

        [Display(Name = "Partnership Opportunity")]
        Partnership,

        [Display(Name = "General Feedback")]
        GeneralFeedback,

        [Display(Name = "Account Help")]
        AccountHelp,

        Report,
    }
}
