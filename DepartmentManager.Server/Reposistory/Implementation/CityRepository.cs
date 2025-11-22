using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DepartmentManager.Server.Models;
using DepartmentManager.Server.Reposistory.Interface;
using Microsoft.EntityFrameworkCore;

namespace DepartmentManager.Server.Reposistory.Implementation
{
    public class CityRepository : Repository<City>, ICityRepository
    {
        public CityRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<City>> GetAllCitiesAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .OrderBy(city => city.CityName)
                .ToListAsync();
        }

        public async Task<City?> GetCityByIdAsync(int id)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(city => city.CityId == id);
        }

        public async Task<City> AddCityAsync(City city)
        {
            return await AddAsync(city);
        }
    }
}
