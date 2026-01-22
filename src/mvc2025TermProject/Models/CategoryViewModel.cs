using System.ComponentModel.DataAnnotations;

namespace mvc2025TermProject.Models
{
    public class CategoryFormFieldsViewModel
    {
        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Category name must be between 3 and 100 characters.")]
        [Display(Name = "Category Name")]
        public string? Name { get; set; }

        [StringLength(250, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 250 characters if provided.")]
        public string? Description { get; set; }
    }

    public class CategoryDetailViewModel
    {
        public int? ID { get; set; }

        [Display(Name = "Category Name")]
        public string? Name { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Status")]
        public string? Status { get; set; }

        [Display(Name = "Pending Modification")]
        public bool? IsPendingModification { get; set; }

        [Display(Name = "Proposed Category Name")]
        public string? ProposedName { get; set; }

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

    public class CategoryListViewModel
    {
        public int? ID { get; set; }

        [Display(Name = "Category Name")]
        public string? Name { get; set; }

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
}
