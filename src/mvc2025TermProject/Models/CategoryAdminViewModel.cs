using System.ComponentModel.DataAnnotations;

namespace mvc2025TermProject.Models
{
    public class CategoryAdminDetailsViewModel
    {
        public int? ID { get; set; }

        [Display(Name = "Current Category Name")]
        public string? CurrentName { get; set; }

        [Display(Name = "Proposed Category Name")]
        public string? ProposedName { get; set; }

        [Display(Name = "Current Description")]
        public string? CurrentDescription { get; set; }

        [Display(Name = "Proposed Description")]
        public string? ProposedDescription { get; set; }

        [Display(Name = "Current Status")]
        public string? CurrentStatus { get; set; }

        [Display(Name = "Has Pending Modifications")]
        public bool? HasPendingModifications { get; set; }

        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }

        [Display(Name = "Created Date")]
        [DataType(DataType.Date)]
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "Last Modified")]
        [DataType(DataType.Date)]
        public DateTime? LastModifiedDate { get; set; }

        [Display(Name = "Approved Date")]
        [DataType(DataType.Date)]
        public DateTime? ApprovedDate { get; set; }

        [Display(Name = "Decided By")]
        public string? DecidedBy { get; set; }

        [Display(Name = "Request Type")]
        public string? RequestType { get; set; }

        [Display(Name = "Review Status")]
        public string? ReviewStatus { get; set; }

        [Display(Name = "Recipe Count")]
        public int? RecipeCount { get; set; }
    }

    public class CategoryAdminListViewModel
    {
        public int? ID { get; set; }

        [Display(Name = "Category Name")]
        public string? Name { get; set; }

        [Display(Name = "Type")]
        public string? RequestType { get; set; }

        [Display(Name = "Submitted By")]
        public string? CreatedBy { get; set; }

        [Display(Name = "Submitted Date")]
        [DataType(DataType.Date)]
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "Status")]
        public string? ReviewStatus { get; set; }
    }
}