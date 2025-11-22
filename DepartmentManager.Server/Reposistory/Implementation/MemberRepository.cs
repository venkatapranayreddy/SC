using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DepartmentManager.Server.Models;
using DepartmentManager.Server.Reposistory.Interface;
using Microsoft.EntityFrameworkCore;

namespace DepartmentManager.Server.Reposistory.Implementation
{
    public class MemberRepository : Repository<Member>, IMemberRepository
    {
        public MemberRepository(ApplicationDbContext context) : base(context)
        {
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
    }
}

