using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mvc2025TermProject.Models
{
    public class Ingredient
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }

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

        [Display(Name = "Status")]
        public bool? IsApproved { get; set; } = false;

        [Display(Name = "Created")]
        [DataType(DataType.DateTime)]
        public DateTime? CreatedDate { get; set; } = DateTime.Now;

        // TESTING : User who created or proposed the ingredient
        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }

        // TESTING : Admin user who approved or rejected the ingredient
        [Display(Name = "Decided By")]
        public string? DecidedBy { get; set; }

        // Pending Modification Fields
        [StringLength(100, ErrorMessage = "Pending ingredient name must be between 2 and 100 characters.")]
        [Display(Name = "Pending Ingredient Name")]
        public string? PendingName { get; set; }

        [StringLength(50, ErrorMessage = "Pending ingredient type cannot exceed 50 characters.")]
        [Display(Name = "Pending Ingredient Type")]
        public string? PendingType { get; set; }

        [StringLength(500, ErrorMessage = "Pending description must be between 10 and 500 characters if provided.")]
        [Display(Name = "Pending Description")]
        public string? PendingDescription { get; set; }

        [Display(Name = "Is Pending Modification")]
        public bool? IsPendingModification { get; set; }

        [Display(Name = "Modified")]
        [DataType(DataType.DateTime)]
        public DateTime? LastModifiedDate { get; set; }

        [Display(Name = "Approved")]
        [DataType(DataType.DateTime)]
        public DateTime? ApprovedDate { get; set; }

        // RELATIONSHIPS
        public virtual ICollection<RecipeIngredient>? RecipeIngredients { get; set; }
    }
}
