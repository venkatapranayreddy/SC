using Microsoft.AspNetCore.Mvc;
using DepartmentManager.Server.Models;
using DepartmentManager.Server.Reposistory;

namespace DepartmentManager.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CityController : ControllerBase
    {
        private readonly IRepository<City> _repository;

        public CityController(IRepository<City> repository)
        {
            _repository = repository;
        }

        // GET: api/City
        [HttpGet]
        public async Task<ActionResult<IEnumerable<City>>> GetCities()
        {
            var cities = await _repository.GetAllAsync();
            return Ok(cities);
        }

        // GET: api/City/5
        [HttpGet("{id}")]
        public async Task<ActionResult<City>> GetCity(int id)
        {
            var city = await _repository.GetByIdAsync(id);

            if (city == null)
            {
                return NotFound();
            }

            return Ok(city);
        }

        // POST: api/City
        [HttpPost]
        public async Task<ActionResult<City>> CreateCity(City city)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdCity = await _repository.AddAsync(city);
            return CreatedAtAction(nameof(GetCity), new { id = createdCity.CityId }, createdCity);
        }

        // PUT: api/City/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCity(int id, City city)
        {
            if (id != city.CityId)
            {
                return BadRequest();
            }

            var existingCity = await _repository.GetByIdAsync(id);
            if (existingCity == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _repository.UpdateAsync(city);
            return NoContent();
        }

        // DELETE: api/City/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCity(int id)
        {
            var city = await _repository.GetByIdAsync(id);
            if (city == null)
            {
                return NotFound();
            }

            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}

