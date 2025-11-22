using System.Collections.Generic;
using System.Threading.Tasks;
using DepartmentManager.Server.Models;
using DepartmentManager.Server.Models.DTOs;
using DepartmentManager.Server.Reposistory;

namespace DepartmentManager.Server.Reposistory.Interface
{
    public interface IMemberRepository : IRepository<Member>
    {
        Task<Member?> GetMemberByEmailAsync(string email);
        Task<IEnumerable<Member>> GetActiveMembersAsync();
        Task<MemberDetailDto?> GetMemberDetailAsync(int id);
        Task<MemberDetailDto> RegisterMemberAsync(RegisterMemberDto dto);
        Task<IEnumerable<object>> GetApproversAsync(int affiliationId);
        Task<IEnumerable<MemberAffiliationDto>> GetMemberAffiliationsAsync(int memberId);
        Task<bool> UpdateMemberAsync(int id, Member member);
    }
}
