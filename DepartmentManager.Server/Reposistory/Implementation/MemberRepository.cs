using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DepartmentManager.Server.Models;
using DepartmentManager.Server.Models.DTOs;
using DepartmentManager.Server.Reposistory.Interface;
using Microsoft.EntityFrameworkCore;

namespace DepartmentManager.Server.Reposistory.Implementation
{
    public class MemberRepository : Repository<Member>, IMemberRepository
    {
        private readonly IMemberAffiliationRepository _memberAffiliationRepository;
        private readonly IApprovalRequestRepository _approvalRequestRepository;
        private readonly ICityRepository _cityRepository;
        private readonly IAffiliationRepository _affiliationRepository;
        private readonly IRoleRepository _roleRepository;

        public MemberRepository(
            ApplicationDbContext context,
            IMemberAffiliationRepository memberAffiliationRepository,
            IApprovalRequestRepository approvalRequestRepository,
            ICityRepository cityRepository,
            IAffiliationRepository affiliationRepository,
            IRoleRepository roleRepository) : base(context)
        {
            _memberAffiliationRepository = memberAffiliationRepository;
            _approvalRequestRepository = approvalRequestRepository;
            _cityRepository = cityRepository;
            _affiliationRepository = affiliationRepository;
            _roleRepository = roleRepository;
        }

        public async Task<Member?> GetMemberByEmailAsync(string email)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(member => member.Email == email);
        }

        public async Task<IEnumerable<Member>> GetActiveMembersAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(member => member.Status == MemberStatus.Active)
                .OrderBy(member => member.FullName)
                .ToListAsync();
        }

        public async Task<MemberDetailDto?> GetMemberDetailAsync(int id)
        {
            var member = await GetByIdAsync(id);
            if (member == null) return null;

            var memberAffiliations = await _memberAffiliationRepository.GetByMemberIdAsync(id);
            var affiliationDtos = new List<MemberAffiliationDto>();

            foreach (var ma in memberAffiliations)
            {
                var city = await _cityRepository.GetCityByIdAsync(ma.CityId);
                var affiliation = await _affiliationRepository.GetByIdAsync(ma.AffiliationId);
                var role = await _roleRepository.GetByIdAsync(ma.RoleId);
                var approver = ma.ApproverId.HasValue ? await GetByIdAsync(ma.ApproverId.Value) : null;
                var manager = approver?.ManagerId.HasValue == true ? await GetByIdAsync(approver.ManagerId.Value) : null;

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

            return new MemberDetailDto
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
        }

        public async Task<MemberDetailDto> RegisterMemberAsync(RegisterMemberDto dto)
        {
            // Validate email for non-Google registration
            if (string.IsNullOrEmpty(dto.GoogleId) && dto.Email.ToLower().EndsWith("@gmail.com"))
            {
                throw new InvalidOperationException("Gmail addresses are not allowed for form-based registration. Please use Google Sign-Up instead.");
            }

            // Check if member already exists
            var existingMember = await GetMemberByEmailAsync(dto.Email);
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

                member = await AddAsync(member);
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
                await UpdateAsync(member);
            }

            // Verify approver exists
            var approver = await GetByIdAsync(dto.ApproverId);
            if (approver == null)
            {
                throw new InvalidOperationException("Selected approver not found.");
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

            // Return the complete member detail
            return await GetMemberDetailAsync(member.MemberId) ?? throw new InvalidOperationException("Failed to retrieve member details after registration.");
        }

        public async Task<IEnumerable<object>> GetApproversAsync(int affiliationId)
        {
            // Get members who can be approvers for this affiliation
            // This would typically filter by role or other criteria
            // For now, return all active members with their managers
            var approvers = await _dbSet
                .AsNoTracking()
                .Include(m => m.Manager)
                .Where(member => member.Status == MemberStatus.Active)
                .OrderBy(member => member.FullName)
                .ToListAsync();
            
            return approvers.Select(m => new
            {
                memberId = m.MemberId,
                fullName = m.FullName,
                email = m.Email,
                managerId = m.ManagerId,
                managerName = m.Manager?.FullName
            }).ToList();
        }

        public async Task<IEnumerable<MemberAffiliationDto>> GetMemberAffiliationsAsync(int memberId)
        {
            var member = await GetByIdAsync(memberId);
            if (member == null)
            {
                throw new InvalidOperationException("Member not found.");
            }

            var memberAffiliations = await _memberAffiliationRepository.GetByMemberIdAsync(memberId);
            var affiliationDtos = new List<MemberAffiliationDto>();

            foreach (var ma in memberAffiliations)
            {
                var city = await _cityRepository.GetCityByIdAsync(ma.CityId);
                var affiliation = await _affiliationRepository.GetByIdAsync(ma.AffiliationId);
                var role = await _roleRepository.GetByIdAsync(ma.RoleId);
                var approver = ma.ApproverId.HasValue ? await GetByIdAsync(ma.ApproverId.Value) : null;
                var manager = approver?.ManagerId.HasValue == true ? await GetByIdAsync(approver.ManagerId.Value) : null;

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

            return affiliationDtos;
        }

        public async Task<bool> UpdateMemberAsync(int id, Member member)
        {
            if (id != member.MemberId)
            {
                throw new ArgumentException("Member ID mismatch.");
            }

            var existingMember = await GetByIdAsync(id);
            if (existingMember == null)
            {
                throw new InvalidOperationException("Member not found.");
            }

            await UpdateAsync(member);
            return true;
        }
    }
}

