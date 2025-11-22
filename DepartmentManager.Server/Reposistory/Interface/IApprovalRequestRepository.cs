using System.Collections.Generic;
using System.Threading.Tasks;
using DepartmentManager.Server.Models;
using DepartmentManager.Server.Reposistory;

namespace DepartmentManager.Server.Reposistory.Interface
{
    public interface IApprovalRequestRepository : IRepository<ApprovalRequest>
    {
        Task<IEnumerable<ApprovalRequest>> GetPendingRequestsForMemberAsync(int memberId, RequestType? requestType = null);
        Task<ApprovalRequest?> GetPendingRequestAsync(int memberAffiliationId, int requestedToMemberId, RequestType? requestType = null);
        Task<IEnumerable<ApprovalRequest>> GetPendingRequestsByAffiliationAsync(int memberAffiliationId);
    }
}
