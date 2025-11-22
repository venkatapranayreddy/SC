using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DepartmentManager.Server.Models;
using DepartmentManager.Server.Reposistory.Interface;
using Microsoft.EntityFrameworkCore;

namespace DepartmentManager.Server.Reposistory.Implementation
{
    public class AffiliationRepository : Repository<Affiliation>, IAffiliationRepository
    {
        public AffiliationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Affiliation>> GetByCityIdAsync(int cityId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(affiliation => affiliation.CityId == cityId)
                .OrderBy(affiliation => affiliation.Name)
                .ToListAsync();
        }
    }
}

