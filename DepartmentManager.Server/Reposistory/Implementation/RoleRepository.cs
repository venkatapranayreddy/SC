using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DepartmentManager.Server.Models;
using DepartmentManager.Server.Reposistory.Interface;
using Microsoft.EntityFrameworkCore;

namespace DepartmentManager.Server.Reposistory.Implementation
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        public RoleRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Role>> GetByAffiliationIdAsync(int affiliationId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(role => role.AffiliationId == affiliationId)
                .OrderBy(role => role.Name)
                .ToListAsync();
        }
    }
}

