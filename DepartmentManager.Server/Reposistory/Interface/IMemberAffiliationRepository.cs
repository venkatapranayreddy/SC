using System.Collections.Generic;
using System.Threading.Tasks;
using DepartmentManager.Server.Models;
using DepartmentManager.Server.Reposistory;

namespace DepartmentManager.Server.Reposistory.Interface
{
    public interface IMemberAffiliationRepository : IRepository<MemberAffiliation>
    {
        Task<IEnumerable<MemberAffiliation>> GetByMemberIdAsync(int memberId);
        Task<IEnumerable<MemberAffiliation>> GetByIdsAsync(IEnumerable<int> memberAffiliationIds);
    }
}
