using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DepartmentManager.Server.Models;
using DepartmentManager.Server.Reposistory.Interface;
using Microsoft.EntityFrameworkCore;

namespace DepartmentManager.Server.Reposistory.Implementation
{
    public class MemberAffiliationRepository : Repository<MemberAffiliation>, IMemberAffiliationRepository
    {
        public MemberAffiliationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<MemberAffiliation>> GetByMemberIdAsync(int memberId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(memberAffiliation => memberAffiliation.MemberId == memberId)
                .OrderByDescending(memberAffiliation => memberAffiliation.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<MemberAffiliation>> GetByIdsAsync(IEnumerable<int> memberAffiliationIds)
        {
            var ids = memberAffiliationIds?.Distinct().ToList() ?? new List<int>();
            if (!ids.Any())
            {
                return new List<MemberAffiliation>();
            }

            return await _dbSet
                .AsNoTracking()
                .Where(memberAffiliation => ids.Contains(memberAffiliation.MemberAffiliationId))
                .ToListAsync();
        }
    }
}

