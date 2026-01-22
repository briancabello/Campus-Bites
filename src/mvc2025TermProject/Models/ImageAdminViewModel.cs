using System.ComponentModel.DataAnnotations;

namespace mvc2025TermProject.Models
{
    public class ImageAdminViewModel
    {
        public List<Image>? Images { get; set; }

        [Display(Name = "Status Filter")]
        public string? StatusFilter { get; set; }

        [Display(Name = "Pending Images")]
        public int PendingCount { get; set; }

        [Display(Name = "Approved Images")]
        public int ApprovedCount { get; set; }

        [Display(Name = "Total Images")]
        public int TotalCount { get; set; }

        [Display(Name = "Rejection Reason")]
        [StringLength(500, ErrorMessage = "Rejection reason cannot exceed 500 characters.")]
        public string? RejectionReason { get; set; }

        // Pagination support
        public PaginatedList<Image>? PaginatedList { get; set; }
    }
}
