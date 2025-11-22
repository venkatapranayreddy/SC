using DepartmentManager.Server.Models;
using DepartmentManager.Server.Models.DTOs;

namespace DepartmentManager.Server.Services.Interface
{
    public interface IMemberAffiliationService
    {
        Task<IEnumerable<MemberAffiliationDto>> GetPendingApprovalsAsync(int approverId, RequestType? requestType = null);
        Task<MemberAffiliationDto?> GetMemberAffiliationDtoAsync(int memberAffiliationId);
        Task<(bool Success, string Message)> ProcessApprovalAsync(int memberAffiliationId, int approverId, bool isApproved, string? rejectionReason);
    }
}
