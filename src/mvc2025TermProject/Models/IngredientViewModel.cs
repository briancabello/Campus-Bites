using System.ComponentModel.DataAnnotations;

namespace mvc2025TermProject.Models
{
    public class IngredientFormFieldsViewModel
    {
        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Ingredient name must be between 2 and 100 characters.")]
        [Display(Name = "Ingredient Name")]
        public string? Name { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Ingredient type cannot exceed 50 characters.")]
        [Display(Name = "Ingredient Type")]
        public string? Type { get; set; }

        [StringLength(500, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 500 characters if provided.")]
        public string? Description { get; set; }
    }

    public class IngredientListViewModel
    {
        public int? ID { get; set; }

        [Display(Name = "Ingredient Name")]
        public string? Name { get; set; }

        [Display(Name = "Type")]
        public string? Type { get; set; }

        public string? Description { get; set; }

        public string? Status { get; set; }

        [Display(Name = "Recipe Count")]
        public int? RecipeCount { get; set; }

        [Display(Name = "Created")]
        [DataType(DataType.Date)]
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "Under Review")]
        public bool? UnderReview { get; set; }
    }

    public class IngredientDetailViewModel
    {
        public int? ID { get; set; }

        [Required]
        [Display(Name = "Ingredient Name")]
        public string? Name { get; set; }

        [Required]
        [Display(Name = "Type")]
        public string? Type { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Status")]
        public string? Status { get; set; }

        [Display(Name = "Pending Modification")]
        public bool? IsPendingModification { get; set; }

        [Display(Name = "Proposed Ingredient Name")]
        public string? ProposedName { get; set; }

        [Display(Name = "Proposed Type")]
        public string? ProposedType { get; set; }

        [Display(Name = "Proposed Description")]
        public string? ProposedDescription { get; set; }

        [Display(Name = "Created")]
        [DataType(DataType.Date)]
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }

        [Display(Name = "Last Modified")]
        [DataType(DataType.Date)]
        public DateTime? LastModifiedDate { get; set; }

        [Display(Name = "Approved")]
        [DataType(DataType.Date)]
        public DateTime? ApprovedDate { get; set; }

        [Display(Name = "Decided By")]
        public string? DecidedBy { get; set; }
    }
}
