using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DepartmentManager.Server.Models
{
    public enum RequestType
    {
        MemberApproval = 0,
        AffiliationApproval = 1
    }

    public enum RequestStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }

    public class ApprovalRequest
    {
        [Key]
        public int ApprovalRequestId { get; set; }

        [Required]
        public int MemberAffiliationId { get; set; }
        [ForeignKey("MemberAffiliationId")]
        public virtual MemberAffiliation MemberAffiliation { get; set; } = null!;

        [Required]
        public RequestType RequestType { get; set; }

        [Required]
        public int RequestedToMemberId { get; set; } // Approver or Manager ID
        [ForeignKey("RequestedToMemberId")]
        public virtual Member RequestedToMember { get; set; } = null!;

        [Required]
        public RequestStatus Status { get; set; } = RequestStatus.Pending;

        public string? RejectionReason { get; set; }

        public DateTime? RespondedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}

