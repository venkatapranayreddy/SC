namespace DepartmentManager.Server.Models.DTOs
{
    public class MemberDetailDto
    {
        public int MemberId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? InstagramId { get; set; }
        public string? Address { get; set; }
        public string? GoogleId { get; set; }
        public string? GoogleEmail { get; set; }
        public MemberStatus Status { get; set; }
        public List<MemberAffiliationDto> Affiliations { get; set; } = new List<MemberAffiliationDto>();
    }
}

