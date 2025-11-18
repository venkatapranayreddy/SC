using Microsoft.AspNetCore.Mvc;
using DepartmentManager.Server.Models;
using DepartmentManager.Server.Reposistory;

namespace DepartmentManager.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AffilationController : ControllerBase
    {
        private readonly IRepository<Affiliation> _repository;

        public AffilationController(IRepository<Affiliation> repository)
        {
            _repository = repository;
        }

        // GET: api/Affilation
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Affiliation>>> GetAffiliations()
        {
            var affiliations = await _repository.GetAllAsync();
            return Ok(affiliations);
        }

        // GET: api/Affilation/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Affiliation>> GetAffiliation(int id)
        {
            var affiliation = await _repository.GetByIdAsync(id);

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

            var createdAffiliation = await _repository.AddAsync(affiliation);
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

            var existingAffiliation = await _repository.GetByIdAsync(id);
            if (existingAffiliation == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _repository.UpdateAsync(affiliation);
            return NoContent();
        }

        // DELETE: api/Affilation/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAffiliation(int id)
        {
            var affiliation = await _repository.GetByIdAsync(id);
            if (affiliation == null)
            {
                return NotFound();
            }

            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}

