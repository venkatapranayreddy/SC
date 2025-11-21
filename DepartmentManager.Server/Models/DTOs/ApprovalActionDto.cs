using System.ComponentModel.DataAnnotations;

namespace DepartmentManager.Server.Models.DTOs
{
    public class ApprovalActionDto
    {
        [Required]
        public int MemberAffiliationId { get; set; }

        [Required]
        public bool IsApproved { get; set; }

        public string? RejectionReason { get; set; }
    }
}

