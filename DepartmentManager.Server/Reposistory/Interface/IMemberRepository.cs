using System.Collections.Generic;
using System.Threading.Tasks;
using DepartmentManager.Server.Models;
using DepartmentManager.Server.Reposistory;

namespace DepartmentManager.Server.Reposistory.Interface
{
    public interface IMemberRepository : IRepository<Member>
    {
        Task<Member?> GetMemberByEmailAsync(string email);
        Task<IEnumerable<Member>> GetActiveMembersAsync();
    }
}
