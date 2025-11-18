using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DepartmentManager.Server.Models
{
    public class Member
    {
          [Key]
        public int MemberId { get; set; }

        [Required, StringLength(100)]
        public string FullName { get; set; }

        [Required, EmailAddress, StringLength(150)]
        public string Email { get; set; }

        [Phone, StringLength(20)]
        public string PhoneNumber { get; set; }

        // Foreign Key → City
        [Required]
        public int CityId { get; set; }
        [ForeignKey("CityId")]
        public City City { get; set; }

        // Foreign Key → Affiliation
        [Required]
        public int AffiliationId { get; set; }
        [ForeignKey("AffiliationId")]
        public Affiliation Affiliation { get; set; }

        // Foreign Key → Role
        [Required]
        public int RoleId { get; set; }
        [ForeignKey("RoleId")]
        public Role Role { get; set; }

        // ✅ Self-referencing Approver
        public int? ApproverId { get; set; }
        [ForeignKey("ApproverId")]
        public Member Approver { get; set; }

        [StringLength(50)]
        public string GovtId { get; set; }

        public string ProfilePictureUrl { get; set; }

        [StringLength(300)]
        public string Address { get; set; }

        public string DigitalSignatureUrl { get; set; }

        [Required]
        public bool AcceptTermsAndConditions { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}


