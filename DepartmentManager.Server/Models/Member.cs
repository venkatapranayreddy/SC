using DepartmentManager.Server.Models;
using System.ComponentModel.DataAnnotations;

namespace DepartmentManager.Server.Models
{
    public enum MemberStatus
    {
        Pending = 0,
        Active = 1,
        Inactive = 2,
        Rejected = 3
    }

    public class Member
    {
        [Key]
        public int MemberId { get; set; }

        [Required, StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required, Phone, StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        // Google OAuth fields
        public string? GoogleId { get; set; }
        public string? GoogleEmail { get; set; }

        // Additional fields
        public string? InstagramId { get; set; }
        public string? Address { get; set; }

        // Member status
        public MemberStatus Status { get; set; } = MemberStatus.Pending;

        // Terms acceptance
        public bool AcceptTermsAndConditions { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Manager relationship (for approvers)
        public int? ManagerId { get; set; }
        [ForeignKey("ManagerId")]
        public virtual Member? Manager { get; set; }

        // Navigation properties
        public virtual ICollection<MemberAffiliation> MemberAffiliations { get; set; } = new List<MemberAffiliation>();
        public virtual ICollection<MemberAffiliation> ApprovalsAsApprover { get; set; } = new List<MemberAffiliation>();
        public virtual ICollection<MemberAffiliation> ApprovalsAsManager { get; set; } = new List<MemberAffiliation>();
    }
}
