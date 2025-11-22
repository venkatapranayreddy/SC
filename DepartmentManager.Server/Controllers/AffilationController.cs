using Microsoft.AspNetCore.Mvc;
using DepartmentManager.Server.Models;
using DepartmentManager.Server.Reposistory.Interface;

namespace DepartmentManager.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AffilationController : ControllerBase
    {
        private readonly IAffiliationRepository _affiliationRepository;

        public AffilationController(IAffiliationRepository affiliationRepository)
        {
            _affiliationRepository = affiliationRepository;
        }

        // GET: api/Affilation
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Affiliation>>> GetAffiliations([FromQuery] int? cityId = null)
        {
            if (cityId.HasValue)
            {
                var affiliations = await _affiliationRepository.GetByCityIdAsync(cityId.Value);
                return Ok(affiliations);
            }
            var allAffiliations = await _affiliationRepository.GetAllAsync();
            return Ok(allAffiliations);
        }

        // GET: api/Affilation/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Affiliation>> GetAffiliation(int id)
        {
            var affiliation = await _affiliationRepository.GetByIdAsync(id);

            if (affiliation == null)
            {
                return NotFound();
            }

            return Ok(affiliation);
        }

        // POST: api/Affilation
        [HttpPost]
        public async Task<ActionResult<Affiliation>> CreateAffiliation(Affiliation affiliation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdAffiliation = await _affiliationRepository.AddAsync(affiliation);
            return CreatedAtAction(nameof(GetAffiliation), new { id = createdAffiliation.AffiliationId }, createdAffiliation);
        }

        // PUT: api/Affilation/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAffiliation(int id, Affiliation affiliation)
        {
            if (id != affiliation.AffiliationId)
            {
                return BadRequest();
            }

            var existingAffiliation = await _affiliationRepository.GetByIdAsync(id);
            if (existingAffiliation == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _affiliationRepository.UpdateAsync(affiliation);
            return NoContent();
        }

        // DELETE: api/Affilation/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAffiliation(int id)
        {
            var affiliation = await _affiliationRepository.GetByIdAsync(id);
            if (affiliation == null)
            {
                return NotFound();
            }

            await _affiliationRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}

