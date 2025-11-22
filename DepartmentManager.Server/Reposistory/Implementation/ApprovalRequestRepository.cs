using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DepartmentManager.Server.Models;
using DepartmentManager.Server.Reposistory.Interface;
using Microsoft.EntityFrameworkCore;

namespace DepartmentManager.Server.Reposistory.Implementation
{
    public class ApprovalRequestRepository : Repository<ApprovalRequest>, IApprovalRequestRepository
    {
        public ApprovalRequestRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ApprovalRequest>> GetPendingRequestsForMemberAsync(int memberId, RequestType? requestType = null)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(request =>
                    request.RequestedToMemberId == memberId &&
                    request.Status == RequestStatus.Pending &&
                    (requestType == null || request.RequestType == requestType))
                .OrderByDescending(request => request.CreatedAt)
                .ToListAsync();
        }

        public async Task<ApprovalRequest?> GetPendingRequestAsync(int memberAffiliationId, int requestedToMemberId, RequestType? requestType = null)
        {
            return await _dbSet.FirstOrDefaultAsync(request =>
                request.MemberAffiliationId == memberAffiliationId &&
                request.RequestedToMemberId == requestedToMemberId &&
                request.Status == RequestStatus.Pending &&
                (requestType == null || request.RequestType == requestType));
        }

        public async Task<IEnumerable<ApprovalRequest>> GetPendingRequestsByAffiliationAsync(int memberAffiliationId)
        {
            return await _dbSet
                .Where(request =>
                    request.MemberAffiliationId == memberAffiliationId &&
                    request.Status == RequestStatus.Pending)
                .ToListAsync();
        }
    }
}

