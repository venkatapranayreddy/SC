using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DepartmentManager.Server.Models
{
    public enum ApprovalStatus
    {
        PendingApprover = 0,
        PendingManager = 1,
        Approved = 2,
        Rejected = 3
    }

    public class MemberAffiliation
    {
        [Key]
        public int MemberAffiliationId { get; set; }

        [Required]
        public int MemberId { get; set; }
        [ForeignKey("MemberId")]
        public virtual Member Member { get; set; } = null!;

        [Required]
        public int CityId { get; set; }
        [ForeignKey("CityId")]
        public virtual City City { get; set; } = null!;

        [Required]
        public int AffiliationId { get; set; }
        [ForeignKey("AffiliationId")]
        public virtual Affiliation Affiliation { get; set; } = null!;

        [Required]
        public int RoleId { get; set; }
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; } = null!;

        public int? ApproverId { get; set; }
        [ForeignKey("ApproverId")]
        public virtual Member? Approver { get; set; }

        [Required, StringLength(50)]
        public string GovtId { get; set; } = string.Empty;

        [Required]
        public string ProfilePictureUrl { get; set; } = string.Empty;

        [Required]
        public string DigitalSignatureUrl { get; set; } = string.Empty;

        // Approval workflow
        public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.PendingApprover;
        public string? RejectionReason { get; set; }
        public int? ApprovedByApproverId { get; set; }
        [ForeignKey("ApprovedByApproverId")]
        public virtual Member? ApprovedByApprover { get; set; }
        public DateTime? ApprovedByApproverAt { get; set; }
        public int? ApprovedByManagerId { get; set; }
        [ForeignKey("ApprovedByManagerId")]
        public virtual Member? ApprovedByManager { get; set; }
        public DateTime? ApprovedByManagerAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}

