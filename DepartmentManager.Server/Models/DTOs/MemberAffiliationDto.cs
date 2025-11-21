namespace DepartmentManager.Server.Models.DTOs
{
    public class MemberAffiliationDto
    {
        public int MemberAffiliationId { get; set; }
        public int MemberId { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public string MemberEmail { get; set; } = string.Empty;
        public string MemberPhoneNumber { get; set; } = string.Empty;
        public int CityId { get; set; }
        public string CityName { get; set; } = string.Empty;
        public int AffiliationId { get; set; }
        public string AffiliationName { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public int? ApproverId { get; set; }
        public string? ApproverName { get; set; }
        public string? ManagerName { get; set; }
        public string GovtId { get; set; } = string.Empty;
        public string ProfilePictureUrl { get; set; } = string.Empty;
        public string DigitalSignatureUrl { get; set; } = string.Empty;
        public ApprovalStatus ApprovalStatus { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

