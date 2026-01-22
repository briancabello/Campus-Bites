using Microsoft.AspNetCore.Identity;
using mvc2025TermProject.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mvc2025TermProject.Models
{
    public class Image
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "File name cannot exceed 255 characters.")]
        [Display(Name = "File Name")]
        public string? FileName { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "File path cannot exceed 500 characters.")]
        [Display(Name = "File Path")]
        public string? FilePath { get; set; }

        [StringLength(255, MinimumLength = 5, ErrorMessage = "Alt text must be between 5 and 255 characters.")]
        [Display(Name = "Alternative Text")]
        public string? AltText { get; set; }

        [StringLength(500, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 500 characters.")]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "File Size (bytes)")]
        [Range(1, 2097152, ErrorMessage = "File size must be 2MB or less.")]
        public long? FileSize { get; set; }

        [Required]
        [StringLength(10)]
        [Display(Name = "File Type")]
        public string? FileType { get; set; }

        [Display(Name = "Status")]
        public bool? IsApproved { get; set; }

        [Display(Name = "Is Main Image")]
        public bool? IsMainImage { get; set; }

        [Required]
        [Display(Name = "Recipe")]
        public int? RecipeId { get; set; } // Foreign key to Recipe

        [Required]
        [Display(Name = "Uploaded By")]
        public string? UploadedById { get; set; } // Foreign key to CampusBitesUser

        [Display(Name = "Uploaded Date")]
        public DateTime? UploadedDate { get; set; }

        [Display(Name = "Approved By")]
        public string? ApprovedById { get; set; }

        [Display(Name = "Approved Date")]
        [DataType(DataType.DateTime)]
        public DateTime? ApprovedDate { get; set; }

        // Navigation property
        public virtual Recipe? Recipe { get; set; }

        public virtual CampusBitesUser? UploadedBy { get; set; }

        public virtual CampusBitesUser? ApprovedBy { get; set; }
    }
}
