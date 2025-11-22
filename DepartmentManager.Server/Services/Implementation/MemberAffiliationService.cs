using DepartmentManager.Server.Models;
using DepartmentManager.Server.Models.DTOs;
using DepartmentManager.Server.Reposistory.Interface;
using DepartmentManager.Server.Services.Interface;

namespace DepartmentManager.Server.Services.Implementation
{
    public class MemberAffiliationService : IMemberAffiliationService
    {
        private readonly IMemberAffiliationRepository _memberAffiliationRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IApprovalRequestRepository _approvalRequestRepository;
        private readonly ICityRepository _cityRepository;
        private readonly IAffiliationRepository _affiliationRepository;
        private readonly IRoleRepository _roleRepository;

        public MemberAffiliationService(
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

        public async Task<IEnumerable<MemberAffiliationDto>> GetPendingApprovalsAsync(int approverId, RequestType? requestType = null)
        {
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

            return result;
        }

        public async Task<MemberAffiliationDto?> GetMemberAffiliationDtoAsync(int memberAffiliationId)
        {
            var ma = await _memberAffiliationRepository.GetByIdAsync(memberAffiliationId);
            if (ma == null)
            {
                return null;
            }

            var member = await _memberRepository.GetByIdAsync(ma.MemberId);
            var city = await _cityRepository.GetCityByIdAsync(ma.CityId);
            var affiliation = await _affiliationRepository.GetByIdAsync(ma.AffiliationId);
            var role = await _roleRepository.GetByIdAsync(ma.RoleId);
            var approver = ma.ApproverId.HasValue ? await _memberRepository.GetByIdAsync(ma.ApproverId.Value) : null;
            var manager = approver?.ManagerId.HasValue == true ? await _memberRepository.GetByIdAsync(approver.ManagerId.Value) : null;

            return new MemberAffiliationDto
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
        }

        public async Task<(bool Success, string Message)> ProcessApprovalAsync(int memberAffiliationId, int approverId, bool isApproved, string? rejectionReason)
        {
            var memberAffiliation = await _memberAffiliationRepository.GetByIdAsync(memberAffiliationId);
            if (memberAffiliation == null)
            {
                return (false, "Member affiliation not found.");
            }

            var approver = await _memberRepository.GetByIdAsync(approverId);
            if (approver == null)
            {
                return (false, "Approver not found.");
            }

            // Find the approval request
            var approvalRequest = await _approvalRequestRepository.GetPendingRequestAsync(memberAffiliationId, approverId);

            if (approvalRequest == null)
            {
                return (false, "No pending approval request found for this approver.");
            }

            if (isApproved)
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
                            memberAffiliationId,
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
                if (string.IsNullOrWhiteSpace(rejectionReason))
                {
                    return (false, "Rejection reason is required.");
                }

                memberAffiliation.ApprovalStatus = ApprovalStatus.Rejected;
                memberAffiliation.RejectionReason = rejectionReason;

                // Update approval request
                approvalRequest.Status = RequestStatus.Rejected;
                approvalRequest.RejectionReason = rejectionReason;
                approvalRequest.RespondedAt = DateTime.UtcNow;

                // Reject all pending approval requests for this member affiliation
                var pendingRequests = await _approvalRequestRepository.GetPendingRequestsByAffiliationAsync(memberAffiliationId);

                foreach (var request in pendingRequests)
                {
                    request.Status = RequestStatus.Rejected;
                    request.RejectionReason = rejectionReason;
                    request.RespondedAt = DateTime.UtcNow;
                    await _approvalRequestRepository.UpdateAsync(request);
                }
            }

            memberAffiliation.UpdatedAt = DateTime.UtcNow;
            await _memberAffiliationRepository.UpdateAsync(memberAffiliation);
            await _approvalRequestRepository.UpdateAsync(approvalRequest);

            return (true, isApproved ? "Approved successfully." : "Rejected successfully.");
        }
    }
}
