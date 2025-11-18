using Microsoft.AspNetCore.Mvc;
using DepartmentManager.Server.Models;
using DepartmentManager.Server.Reposistory;

namespace DepartmentManager.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MemberController : ControllerBase
    {
        private readonly IRepository<Member> _repository;

        public MemberController(IRepository<Member> repository)
        {
            _repository = repository;
        }

        // GET: api/Member
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Member>>> GetMembers()
        {
            var members = await _repository.GetAllAsync();
            return Ok(members);
        }

        // GET: api/Member/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Member>> GetMember(int id)
        {
            var member = await _repository.GetByIdAsync(id);

            if (member == null)
            {
                return NotFound();
            }

            return Ok(member);
        }

        // POST: api/Member
        [HttpPost]
        public async Task<ActionResult<Member>> CreateMember(Member member)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdMember = await _repository.AddAsync(member);
            return CreatedAtAction(nameof(GetMember), new { id = createdMember.MemberId }, createdMember);
        }

        // PUT: api/Member/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMember(int id, Member member)
        {
            if (id != member.MemberId)
            {
                return BadRequest();
            }

            var existingMember = await _repository.GetByIdAsync(id);
            if (existingMember == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _repository.UpdateAsync(member);
            return NoContent();
        }

        // DELETE: api/Member/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMember(int id)
        {
            var member = await _repository.GetByIdAsync(id);
            if (member == null)
            {
                return NotFound();
            }

            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}

