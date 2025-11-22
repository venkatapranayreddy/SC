using Microsoft.AspNetCore.Mvc;
using DepartmentManager.Server.Models;
using DepartmentManager.Server.Models.DTOs;
using DepartmentManager.Server.Reposistory.Interface;

namespace DepartmentManager.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MemberController : ControllerBase
    {
        private readonly IMemberRepository _memberRepository;

        public MemberController(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }

        // GET: api/Member
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Member>>> GetMembers()
        {
            var members = await _memberRepository.GetAllAsync();
            return Ok(members);
        }

        // GET: api/Member/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MemberDetailDto>> GetMember(int id)
        {
            var memberDto = await _memberRepository.GetMemberDetailAsync(id);
            if (memberDto == null) return NotFound();
            return Ok(memberDto);
        }

        // GET: api/Member/email/{email}
        [HttpGet("email/{email}")]
        public async Task<ActionResult<MemberDetailDto>> GetMemberByEmail(string email)
        {
            var member = await _memberRepository.GetMemberByEmailAsync(email);
            if (member == null) return NotFound();
            
            var memberDto = await _memberRepository.GetMemberDetailAsync(member.MemberId);
            if (memberDto == null) return NotFound();
            return Ok(memberDto);
        }

        // POST: api/Member/register
        [HttpPost("register")]
        public async Task<ActionResult<MemberDetailDto>> RegisterMember([FromBody] RegisterMemberDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var memberDto = await _memberRepository.RegisterMemberAsync(dto);
                return CreatedAtAction(nameof(GetMember), new { id = memberDto.MemberId }, memberDto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while registering the member.", error = ex.Message });
            }
        }

        // GET: api/Member/approvers/{affiliationId}
        [HttpGet("approvers/{affiliationId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetApprovers(int affiliationId)
        {
            var approvers = await _memberRepository.GetApproversAsync(affiliationId);
            return Ok(approvers);
        }

        // GET: api/Member/{memberId}/affiliations
        [HttpGet("{memberId}/affiliations")]
        public async Task<ActionResult<IEnumerable<MemberAffiliationDto>>> GetMemberAffiliations(int memberId)
        {
            try
            {
                var affiliations = await _memberRepository.GetMemberAffiliationsAsync(memberId);
                return Ok(affiliations);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        // PUT: api/Member/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMember(int id, Member member)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _memberRepository.UpdateMemberAsync(id, member);
                return NoContent();
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        // DELETE: api/Member/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMember(int id)
        {
            var member = await _memberRepository.GetByIdAsync(id);
            if (member == null)
            {
                return NotFound();
            }

            await _memberRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}

