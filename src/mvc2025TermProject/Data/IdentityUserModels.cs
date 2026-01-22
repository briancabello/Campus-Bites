using Microsoft.AspNetCore.Identity;
using mvc2025TermProject.Models;
using System.ComponentModel.DataAnnotations;

namespace mvc2025TermProject.Data
{
    public class CampusBitesUser : IdentityUser
    {
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters.")]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [StringLength(50, MinimumLength = 1, ErrorMessage = "Middle name must be at least 1 character if provided.")]
        [Display(Name = "Middle Name")]
        public string? MiddleName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters.")]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Street address must be between 5 and 100 characters.")]
        [Display(Name = "Street Address")]
        public string? StreetAddress { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Municipality must be between 2 and 50 characters.")]
        [Display(Name = "Municipality/City")]
        public string? Municipality { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Province must be between 2 and 50 characters.")]
        [Display(Name = "Province")]
        public string? Province { get; set; }

        [Required]
        [RegularExpression(@"^[A-Za-z]\d[A-Za-z][ -]?\d[A-Za-z]\d$", ErrorMessage = "Please enter a valid Canadian postal code.")]
        [Display(Name = "Postal Code")]
        public string? PostalCode { get; set; }

        [Required]
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [StringLength(15, MinimumLength = 10, ErrorMessage = "Phone number must be between 10 and 15 digits.")]
        [Display(Name = "Phone Number")]
        public override string? PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        // Navigation properties
        public virtual ICollection<Recipe>? Recipes { get; set; }
        public virtual ICollection<Image>? Images { get; set; }
    }
}
