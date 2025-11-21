using System.ComponentModel.DataAnnotations;

namespace DepartmentManager.Server.Models.DTOs
{
    public class RegisterMemberDto
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        // Google OAuth fields (optional)
        public string? GoogleId { get; set; }
        public string? GoogleEmail { get; set; }

        // Additional fields
        public string? InstagramId { get; set; }
        public string? Address { get; set; }

        // Affiliation details
        [Required]
        public int CityId { get; set; }

        [Required]
        public int AffiliationId { get; set; }

        [Required]
        public int RoleId { get; set; }

        [Required]
        public int ApproverId { get; set; }

        [Required]
        public string GovtId { get; set; } = string.Empty;

        [Required]
        public string ProfilePictureUrl { get; set; } = string.Empty;

        [Required]
        public string DigitalSignatureUrl { get; set; } = string.Empty;

        [Required]
        public bool AcceptTermsAndConditions { get; set; }
    }
}

