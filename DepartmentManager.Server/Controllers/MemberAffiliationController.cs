using Microsoft.AspNetCore.Mvc;
using DepartmentManager.Server.Models;
using DepartmentManager.Server.Models.DTOs;
using DepartmentManager.Server.Services.Interface;

namespace DepartmentManager.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MemberAffiliationController : ControllerBase
    {
        private readonly IMemberAffiliationService _memberAffiliationService;

        public MemberAffiliationController(IMemberAffiliationService memberAffiliationService)
        {
            _memberAffiliationService = memberAffiliationService;
        }

        // GET: api/MemberAffiliation/pending-approvals/{approverId}
        [HttpGet("pending-approvals/{approverId}")]
        public async Task<ActionResult<IEnumerable<MemberAffiliationDto>>> GetPendingApprovals(int approverId, [FromQuery] RequestType? requestType = null)
        {
            var result = await _memberAffiliationService.GetPendingApprovalsAsync(approverId, requestType);
            return Ok(result);
        }

        // GET: api/MemberAffiliation/member-approvals/{approverId}
        [HttpGet("member-approvals/{approverId}")]
        public async Task<ActionResult<IEnumerable<MemberAffiliationDto>>> GetMemberApprovals(int approverId)
        {
            return await GetPendingApprovals(approverId, RequestType.MemberApproval);
        }

        // GET: api/MemberAffiliation/affiliation-approvals/{managerId}
        [HttpGet("affiliation-approvals/{managerId}")]
        public async Task<ActionResult<IEnumerable<MemberAffiliationDto>>> GetAffiliationApprovals(int managerId)
        {
            return await GetPendingApprovals(managerId, RequestType.AffiliationApproval);
        }

        // GET: api/MemberAffiliation/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<MemberAffiliationDto>> GetMemberAffiliation(int id)
        {
            var dto = await _memberAffiliationService.GetMemberAffiliationDtoAsync(id);
            if (dto == null)
            {
                return NotFound();
            }

            return Ok(dto);
        }

        // POST: api/MemberAffiliation/approve?approverId={approverId}
        [HttpPost("approve")]
        public async Task<IActionResult> ApproveOrReject([FromBody] ApprovalActionDto dto, [FromQuery] int approverId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (success, message) = await _memberAffiliationService.ProcessApprovalAsync(
                dto.MemberAffiliationId, 
                approverId, 
                dto.IsApproved, 
                dto.RejectionReason);

            if (!success)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message });
        }
    }
}

