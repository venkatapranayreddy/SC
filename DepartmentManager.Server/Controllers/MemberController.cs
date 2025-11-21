using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DepartmentManager.Server.Models;
using DepartmentManager.Server.Models.DTOs;
using DepartmentManager.Server.Reposistory;

namespace DepartmentManager.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MemberController : ControllerBase
    {
        private readonly IRepository<Member> _memberRepository;
        private readonly IRepository<MemberAffiliation> _memberAffiliationRepository;
        private readonly IRepository<ApprovalRequest> _approvalRequestRepository;
        private readonly IRepository<City> _cityRepository;
        private readonly IRepository<Affiliation> _affiliationRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly ApplicationDbContext _context;

        public MemberController(
            IRepository<Member> memberRepository,
            IRepository<MemberAffiliation> memberAffiliationRepository,
            IRepository<ApprovalRequest> approvalRequestRepository,
            IRepository<City> cityRepository,
            IRepository<Affiliation> affiliationRepository,
            IRepository<Role> roleRepository,
            ApplicationDbContext context)
        {
            _memberRepository = memberRepository;
            _memberAffiliationRepository = memberAffiliationRepository;
            _approvalRequestRepository = approvalRequestRepository;
            _cityRepository = cityRepository;
            _affiliationRepository = affiliationRepository;
            _roleRepository = roleRepository;
            _context = context;
        }

        // GET: api/Member
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Member>>> GetMembers()
        {
            var members = await _memberRepository.GetAllAsync();
            return Ok(members);
        }

        // GET: api/Member/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MemberDetailDto>> GetMember(int id)
        {
            var member = await _memberRepository.GetByIdAsync(id);

            if (member == null)
            {
                return NotFound();
            }

            var memberAffiliations = await _memberAffiliationRepository.FindAsync(ma => ma.MemberId == id);
            var affiliationDtos = new List<MemberAffiliationDto>();

            foreach (var ma in memberAffiliations)
            {
                var city = await _cityRepository.GetByIdAsync(ma.CityId);
                var affiliation = await _affiliationRepository.GetByIdAsync(ma.AffiliationId);
                var role = await _roleRepository.GetByIdAsync(ma.RoleId);
                var approver = ma.ApproverId.HasValue ? await _memberRepository.GetByIdAsync(ma.ApproverId.Value) : null;
                var manager = approver?.ManagerId.HasValue == true ? await _memberRepository.GetByIdAsync(approver.ManagerId.Value) : null;

                affiliationDtos.Add(new MemberAffiliationDto
                {
                    MemberAffiliationId = ma.MemberAffiliationId,
                    MemberId = ma.MemberId,
                    MemberName = member.FullName,
                    MemberEmail = member.Email,
                    MemberPhoneNumber = member.PhoneNumber,
                    CityId = ma.CityId,
                    CityName = city?.CityName ?? "",
                    AffiliationId = ma.AffiliationId,
                    AffiliationName = affiliation?.Name ?? "",
                    RoleId = ma.RoleId,
                    RoleName = role?.Name ?? "",
                    ApproverId = ma.ApproverId,
                    ApproverName = approver?.FullName,
                    ManagerName = manager?.FullName,
                    GovtId = ma.GovtId,
                    ProfilePictureUrl = ma.ProfilePictureUrl,
                    DigitalSignatureUrl = ma.DigitalSignatureUrl,
                    ApprovalStatus = ma.ApprovalStatus,
                    RejectionReason = ma.RejectionReason,
                    CreatedAt = ma.CreatedAt
                });
            }

            var memberDto = new MemberDetailDto
            {
                MemberId = member.MemberId,
                FullName = member.FullName,
                Email = member.Email,
                PhoneNumber = member.PhoneNumber,
                InstagramId = member.InstagramId,
                Address = member.Address,
                GoogleId = member.GoogleId,
                GoogleEmail = member.GoogleEmail,
                Status = member.Status,
                Affiliations = affiliationDtos
            };

            return Ok(memberDto);
        }

        // GET: api/Member/email/{email}
        [HttpGet("email/{email}")]
        public async Task<ActionResult<MemberDetailDto>> GetMemberByEmail(string email)
        {
            var member = await _memberRepository.FirstOrDefaultAsync(m => m.Email == email);

            if (member == null)
            {
                return NotFound();
            }

            return await GetMember(member.MemberId);
        }

        // POST: api/Member/register
        [HttpPost("register")]
        public async Task<ActionResult<MemberDetailDto>> RegisterMember([FromBody] RegisterMemberDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate email for non-Google registration
            if (string.IsNullOrEmpty(dto.GoogleId) && dto.Email.ToLower().EndsWith("@gmail.com"))
            {
                return BadRequest(new { message = "Gmail addresses are not allowed for form-based registration. Please use Google Sign-Up instead." });
            }

            // Check if member already exists
            var existingMember = await _memberRepository.FirstOrDefaultAsync(m => m.Email == dto.Email);
            Member member;

            if (existingMember == null)
            {
                // Create new member
                member = new Member
                {
                    FullName = dto.FullName,
                    Email = dto.Email,
                    PhoneNumber = dto.PhoneNumber,
                    GoogleId = dto.GoogleId,
                    GoogleEmail = dto.GoogleEmail,
                    InstagramId = dto.InstagramId,
                    Address = dto.Address,
                    Status = MemberStatus.Pending,
                    AcceptTermsAndConditions = dto.AcceptTermsAndConditions,
                    CreatedAt = DateTime.UtcNow
                };

                member = await _memberRepository.AddAsync(member);
            }
            else
            {
                // Update existing member if needed
                member = existingMember;
                if (!string.IsNullOrEmpty(dto.GoogleId))
                {
                    member.GoogleId = dto.GoogleId;
                    member.GoogleEmail = dto.GoogleEmail;
                }
                if (!string.IsNullOrEmpty(dto.InstagramId))
                {
                    member.InstagramId = dto.InstagramId;
                }
                if (!string.IsNullOrEmpty(dto.Address))
                {
                    member.Address = dto.Address;
                }
                member.UpdatedAt = DateTime.UtcNow;
                await _memberRepository.UpdateAsync(member);
            }

            // Verify approver exists and get their manager
            var approver = await _memberRepository.GetByIdAsync(dto.ApproverId);
            if (approver == null)
            {
                return BadRequest(new { message = "Selected approver not found." });
            }

            // Create MemberAffiliation
            var memberAffiliation = new MemberAffiliation
            {
                MemberId = member.MemberId,
                CityId = dto.CityId,
                AffiliationId = dto.AffiliationId,
                RoleId = dto.RoleId,
                ApproverId = dto.ApproverId,
                GovtId = dto.GovtId,
                ProfilePictureUrl = dto.ProfilePictureUrl,
                DigitalSignatureUrl = dto.DigitalSignatureUrl,
                ApprovalStatus = ApprovalStatus.PendingApprover,
                CreatedAt = DateTime.UtcNow
            };

            memberAffiliation = await _memberAffiliationRepository.AddAsync(memberAffiliation);

            // Create approval request for approver
            var approverRequest = new ApprovalRequest
            {
                MemberAffiliationId = memberAffiliation.MemberAffiliationId,
                RequestType = RequestType.MemberApproval,
                RequestedToMemberId = dto.ApproverId,
                Status = RequestStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _approvalRequestRepository.AddAsync(approverRequest);

            // If approver has a manager, create approval request for manager
            if (approver.ManagerId.HasValue)
            {
                var managerRequest = new ApprovalRequest
                {
                    MemberAffiliationId = memberAffiliation.MemberAffiliationId,
                    RequestType = RequestType.AffiliationApproval,
                    RequestedToMemberId = approver.ManagerId.Value,
                    Status = RequestStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };

                await _approvalRequestRepository.AddAsync(managerRequest);
            }

            return CreatedAtAction(nameof(GetMember), new { id = member.MemberId }, await GetMember(member.MemberId));
        }

        // GET: api/Member/approvers/{affiliationId}
        [HttpGet("approvers/{affiliationId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetApprovers(int affiliationId)
        {
            // Get members who can be approvers for this affiliation
            // This would typically filter by role or other criteria
            // For now, return all active members
            var approvers = await _memberRepository.FindAsync(m => m.Status == MemberStatus.Active);
            
            var result = approvers.Select(m => new
            {
                memberId = m.MemberId,
                fullName = m.FullName,
                email = m.Email,
                managerId = m.ManagerId,
                managerName = m.Manager?.FullName
            }).ToList();

            return Ok(result);
        }

        // GET: api/Member/{memberId}/affiliations
        [HttpGet("{memberId}/affiliations")]
        public async Task<ActionResult<IEnumerable<MemberAffiliationDto>>> GetMemberAffiliations(int memberId)
        {
            var member = await _memberRepository.GetByIdAsync(memberId);
            if (member == null)
            {
                return NotFound();
            }

            var memberAffiliations = await _memberAffiliationRepository.FindAsync(ma => ma.MemberId == memberId);
            var affiliationDtos = new List<MemberAffiliationDto>();

            foreach (var ma in memberAffiliations)
            {
                var city = await _cityRepository.GetByIdAsync(ma.CityId);
                var affiliation = await _affiliationRepository.GetByIdAsync(ma.AffiliationId);
                var role = await _roleRepository.GetByIdAsync(ma.RoleId);
                var approver = ma.ApproverId.HasValue ? await _memberRepository.GetByIdAsync(ma.ApproverId.Value) : null;
                var manager = approver?.ManagerId.HasValue == true ? await _memberRepository.GetByIdAsync(approver.ManagerId.Value) : null;

                affiliationDtos.Add(new MemberAffiliationDto
                {
                    MemberAffiliationId = ma.MemberAffiliationId,
                    MemberId = ma.MemberId,
                    MemberName = member.FullName,
                    MemberEmail = member.Email,
                    MemberPhoneNumber = member.PhoneNumber,
                    CityId = ma.CityId,
                    CityName = city?.CityName ?? "",
                    AffiliationId = ma.AffiliationId,
                    AffiliationName = affiliation?.Name ?? "",
                    RoleId = ma.RoleId,
                    RoleName = role?.Name ?? "",
                    ApproverId = ma.ApproverId,
                    ApproverName = approver?.FullName,
                    ManagerName = manager?.FullName,
                    GovtId = ma.GovtId,
                    ProfilePictureUrl = ma.ProfilePictureUrl,
                    DigitalSignatureUrl = ma.DigitalSignatureUrl,
                    ApprovalStatus = ma.ApprovalStatus,
                    RejectionReason = ma.RejectionReason,
                    CreatedAt = ma.CreatedAt
                });
            }

            return Ok(affiliationDtos);
        }

        // PUT: api/Member/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMember(int id, Member member)
        {
            if (id != member.MemberId)
            {
                return BadRequest();
            }

            var existingMember = await _memberRepository.GetByIdAsync(id);
            if (existingMember == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _memberRepository.UpdateAsync(member);
            return NoContent();
        }

        // DELETE: api/Member/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMember(int id)
        {
            var member = await _memberRepository.GetByIdAsync(id);
            if (member == null)
            {
                return NotFound();
            }

            await _memberRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}

