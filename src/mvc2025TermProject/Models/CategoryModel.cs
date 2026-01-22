using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mvc2025TermProject.Models
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? ID { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Category name must be between 3 and 100 characters.")]
        [Display(Name = "Category Name")]
        public string? Name { get; set; }

        [StringLength(250, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 250 characters if provided.")]
        public string? Description { get; set; }

        [Display(Name = "Status")]
        public bool? IsApproved { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Created")]
        public DateTime? CreatedDate { get; set; }

        // TESTING : Who created the category
        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }

        // TESTING : Admin user who approved or rejected the category
        [Display(Name = "Decided By")]
        public string? DecidedBy { get; set; }

        // Pending modification fields
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Pending category name must be between 3 and 100 characters.")]
        [Display(Name = "Proposed Category Name")]
        public string? PendingName { get; set; }

        [StringLength(250, ErrorMessage = "Pending description must be between 10 and 250 characters if provided.")]
        [Display(Name = "Proposed Description")]
        public string? PendingDescription { get; set; }

        [Display(Name = "Proposed Status")]
        public bool? IsPendingModification { get; set; } = false;

        // OPTIONAL
        [Display(Name = "Modified")]
        [DataType(DataType.DateTime)]
        public DateTime? LastModifiedDate { get; set; }

        // OPTIONAL
        [Display(Name = "Approved")]
        [DataType(DataType.DateTime)]
        public DateTime? ApprovedDate { get; set; }

        // RELATIONSHIPS
        public virtual ICollection<Recipe>? Recipes { get; set; }
    }
}
