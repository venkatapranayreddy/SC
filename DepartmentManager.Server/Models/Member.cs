using DepartmentManager.Server.Models;
using System.ComponentModel.DataAnnotations;

public class Member
{
    [Key]
    public int MemberId { get; set; }

    [Required, StringLength(100)]
    public string FullName { get; set; }

    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; }

    [Required, Phone, StringLength(20)]
    public string PhoneNumber { get; set; }

    [Required]
    public int CityId { get; set; }
    public virtual City City { get; set; }

    [Required]
    public int AffiliationId { get; set; }
    public virtual Affiliation Affiliation { get; set; }

    [Required]
    public int RoleId { get; set; }
    public virtual Role Role { get; set; }

    public int? ApproverId { get; set; }
    public virtual Member Approver { get; set; }

    [Required, StringLength(50)]
    public string GovtId { get; set; }

    [Required]
    public string ProfilePictureUrl { get; set; }

    [Required, StringLength(300)]
    public string Address { get; set; }

    [Required]
    public string DigitalSignatureUrl { get; set; }

    [Required]
    public bool AcceptTermsAndConditions { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
