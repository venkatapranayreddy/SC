using Microsoft.AspNetCore.Mvc;
using DepartmentManager.Server.Models;
using DepartmentManager.Server.Models.DTOs;
using DepartmentManager.Server.Reposistory.Interface;

namespace DepartmentManager.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MemberAffiliationController : ControllerBase
    {
        private readonly IMemberAffiliationRepository _memberAffiliationRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IApprovalRequestRepository _approvalRequestRepository;
        private readonly ICityRepository _cityRepository;
        private readonly IAffiliationRepository _affiliationRepository;
        private readonly IRoleRepository _roleRepository;

        public MemberAffiliationController(
            IMemberAffiliationRepository memberAffiliationRepository,
            IMemberRepository memberRepository,
            IApprovalRequestRepository approvalRequestRepository,
            ICityRepository cityRepository,
            IAffiliationRepository affiliationRepository,
            IRoleRepository roleRepository)
        {
            _memberAffiliationRepository = memberAffiliationRepository;
            _memberRepository = memberRepository;
            _approvalRequestRepository = approvalRequestRepository;
            _cityRepository = cityRepository;
            _affiliationRepository = affiliationRepository;
            _roleRepository = roleRepository;
        }

        // GET: api/MemberAffiliation/pending-approvals/{approverId}
        [HttpGet("pending-approvals/{approverId}")]
        public async Task<ActionResult<IEnumerable<MemberAffiliationDto>>> GetPendingApprovals(int approverId, [FromQuery] RequestType? requestType = null)
        {
            var approver = await _memberRepository.GetByIdAsync(approverId);
            if (approver == null)
            {
                return NotFound(new { message = "Approver not found." });
            }

            var approvalRequests = await _approvalRequestRepository.GetPendingRequestsForMemberAsync(approverId, requestType);

            var memberAffiliationIds = approvalRequests.Select(ar => ar.MemberAffiliationId).Distinct().ToList();
            var memberAffiliations = await _memberAffiliationRepository.GetByIdsAsync(memberAffiliationIds);

            var result = new List<MemberAffiliationDto>();

            foreach (var ma in memberAffiliations)
            {
                var member = await _memberRepository.GetByIdAsync(ma.MemberId);
                var city = await _cityRepository.GetCityByIdAsync(ma.CityId);
                var affiliation = await _affiliationRepository.GetByIdAsync(ma.AffiliationId);
                var role = await _roleRepository.GetByIdAsync(ma.RoleId);
                var approverMember = ma.ApproverId.HasValue ? await _memberRepository.GetByIdAsync(ma.ApproverId.Value) : null;
                var manager = approverMember?.ManagerId.HasValue == true ? await _memberRepository.GetByIdAsync(approverMember.ManagerId.Value) : null;

                var request = approvalRequests.FirstOrDefault(ar => ar.MemberAffiliationId == ma.MemberAffiliationId);

                result.Add(new MemberAffiliationDto
                {
                    MemberAffiliationId = ma.MemberAffiliationId,
                    MemberId = ma.MemberId,
                    MemberName = member?.FullName ?? "",
                    MemberEmail = member?.Email ?? "",
                    MemberPhoneNumber = member?.PhoneNumber ?? "",
                    CityId = ma.CityId,
                    CityName = city?.CityName ?? "",
                    AffiliationId = ma.AffiliationId,
                    AffiliationName = affiliation?.Name ?? "",
                    RoleId = ma.RoleId,
                    RoleName = role?.Name ?? "",
                    ApproverId = ma.ApproverId,
                    ApproverName = approverMember?.FullName,
                    ManagerName = manager?.FullName,
                    GovtId = ma.GovtId,
                    ProfilePictureUrl = ma.ProfilePictureUrl,
                    DigitalSignatureUrl = ma.DigitalSignatureUrl,
                    ApprovalStatus = ma.ApprovalStatus,
                    RejectionReason = ma.RejectionReason,
                    CreatedAt = ma.CreatedAt
                });
            }

            return Ok(result);
        }

        // GET: api/MemberAffiliation/member-approvals/{approverId}
        [HttpGet("member-approvals/{approverId}")]
        public async Task<ActionResult<IEnumerable<MemberAffiliationDto>>> GetMemberApprovals(int approverId)
        {
            return await GetPendingApprovals(approverId, RequestType.MemberApproval);
        }

        // GET: api/MemberAffiliation/affiliation-approvals/{managerId}
        [HttpGet("affiliation-approvals/{managerId}")]
        public async Task<ActionResult<IEnumerable<MemberAffiliationDto>>> GetAffiliationApprovals(int managerId)
        {
            return await GetPendingApprovals(managerId, RequestType.AffiliationApproval);
        }

        // GET: api/MemberAffiliation/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<MemberAffiliationDto>> GetMemberAffiliation(int id)
        {
            var ma = await _memberAffiliationRepository.GetByIdAsync(id);
            if (ma == null)
            {
                return NotFound();
            }

            var member = await _memberRepository.GetByIdAsync(ma.MemberId);
            var city = await _cityRepository.GetCityByIdAsync(ma.CityId);
            var affiliation = await _affiliationRepository.GetByIdAsync(ma.AffiliationId);
            var role = await _roleRepository.GetByIdAsync(ma.RoleId);
            var approver = ma.ApproverId.HasValue ? await _memberRepository.GetByIdAsync(ma.ApproverId.Value) : null;
            var manager = approver?.ManagerId.HasValue == true ? await _memberRepository.GetByIdAsync(approver.ManagerId.Value) : null;

            var dto = new MemberAffiliationDto
            {
                MemberAffiliationId = ma.MemberAffiliationId,
                MemberId = ma.MemberId,
                MemberName = member?.FullName ?? "",
                MemberEmail = member?.Email ?? "",
                MemberPhoneNumber = member?.PhoneNumber ?? "",
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
            };

            return Ok(dto);
        }

        // POST: api/MemberAffiliation/approve?approverId={approverId}
        [HttpPost("approve")]
        public async Task<IActionResult> ApproveOrReject([FromBody] ApprovalActionDto dto, [FromQuery] int approverId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var memberAffiliation = await _memberAffiliationRepository.GetByIdAsync(dto.MemberAffiliationId);
            if (memberAffiliation == null)
            {
                return NotFound(new { message = "Member affiliation not found." });
            }

            var approver = await _memberRepository.GetByIdAsync(approverId);
            if (approver == null)
            {
                return NotFound(new { message = "Approver not found." });
            }

            // Find the approval request
            var approvalRequest = await _approvalRequestRepository.GetPendingRequestAsync(dto.MemberAffiliationId, approverId);

            if (approvalRequest == null)
            {
                return BadRequest(new { message = "No pending approval request found for this approver." });
            }

            if (dto.IsApproved)
            {
                // Check if this is approver or manager approval
                if (approvalRequest.RequestType == RequestType.MemberApproval)
                {
                    // Approver approval
                    memberAffiliation.ApprovalStatus = ApprovalStatus.PendingManager;
                    memberAffiliation.ApprovedByApproverId = approverId;
                    memberAffiliation.ApprovedByApproverAt = DateTime.UtcNow;
                    memberAffiliation.RejectionReason = null;

                    // Update approval request
                    approvalRequest.Status = RequestStatus.Approved;
                    approvalRequest.RespondedAt = DateTime.UtcNow;

                    // Check if approver has a manager
                    if (approver.ManagerId.HasValue)
                    {
                        // Manager approval request should already exist, but verify
                        var managerRequest = await _approvalRequestRepository.GetPendingRequestAsync(
                            dto.MemberAffiliationId,
                            approver.ManagerId.Value,
                            RequestType.AffiliationApproval);

                        if (managerRequest == null)
                        {
                            // Create manager approval request if it doesn't exist
                            managerRequest = new ApprovalRequest
                            {
                                MemberAffiliationId = memberAffiliation.MemberAffiliationId,
                                RequestType = RequestType.AffiliationApproval,
                                RequestedToMemberId = approver.ManagerId.Value,
                                Status = RequestStatus.Pending,
                                CreatedAt = DateTime.UtcNow
                            };
                            await _approvalRequestRepository.AddAsync(managerRequest);
                        }
                    }
                    else
                    {
                        // No manager, so approve directly
                        memberAffiliation.ApprovalStatus = ApprovalStatus.Approved;
                        memberAffiliation.ApprovedByManagerAt = DateTime.UtcNow;
                    }
                }
                else if (approvalRequest.RequestType == RequestType.AffiliationApproval)
                {
                    // Manager approval
                    memberAffiliation.ApprovalStatus = ApprovalStatus.Approved;
                    memberAffiliation.ApprovedByManagerId = approverId;
                    memberAffiliation.ApprovedByManagerAt = DateTime.UtcNow;
                    memberAffiliation.RejectionReason = null;

                    // Update approval request
                    approvalRequest.Status = RequestStatus.Approved;
                    approvalRequest.RespondedAt = DateTime.UtcNow;

                    // Activate member if this is their first approved affiliation
                    var member = await _memberRepository.GetByIdAsync(memberAffiliation.MemberId);
                    if (member != null && member.Status == MemberStatus.Pending)
                    {
                        member.Status = MemberStatus.Active;
                        member.UpdatedAt = DateTime.UtcNow;
                        await _memberRepository.UpdateAsync(member);
                    }
                }
            }
            else
            {
                // Rejection
                if (string.IsNullOrWhiteSpace(dto.RejectionReason))
                {
                    return BadRequest(new { message = "Rejection reason is required." });
                }

                memberAffiliation.ApprovalStatus = ApprovalStatus.Rejected;
                memberAffiliation.RejectionReason = dto.RejectionReason;

                // Update approval request
                approvalRequest.Status = RequestStatus.Rejected;
                approvalRequest.RejectionReason = dto.RejectionReason;
                approvalRequest.RespondedAt = DateTime.UtcNow;

                // Reject all pending approval requests for this member affiliation
                var pendingRequests = await _approvalRequestRepository.GetPendingRequestsByAffiliationAsync(dto.MemberAffiliationId);

                foreach (var request in pendingRequests)
                {
                    request.Status = RequestStatus.Rejected;
                    request.RejectionReason = dto.RejectionReason;
                    request.RespondedAt = DateTime.UtcNow;
                    await _approvalRequestRepository.UpdateAsync(request);
                }
            }

            memberAffiliation.UpdatedAt = DateTime.UtcNow;
            await _memberAffiliationRepository.UpdateAsync(memberAffiliation);
            await _approvalRequestRepository.UpdateAsync(approvalRequest);

            return Ok(new { message = dto.IsApproved ? "Approved successfully." : "Rejected successfully." });
        }
    }
}

