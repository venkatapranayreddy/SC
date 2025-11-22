using System.Collections.Generic;
using System.Threading.Tasks;
using DepartmentManager.Server.Models;
using DepartmentManager.Server.Reposistory;

namespace DepartmentManager.Server.Reposistory.Interface
{
    public interface IRoleRepository : IRepository<Role>
    {
        Task<IEnumerable<Role>> GetByAffiliationIdAsync(int affiliationId);
    }
}
