using System.Collections.Generic;
using System.Threading.Tasks;
using DepartmentManager.Server.Models;
using DepartmentManager.Server.Reposistory;

namespace DepartmentManager.Server.Reposistory.Interface
{
    public interface ICityRepository : IRepository<City>
    {
        Task<IEnumerable<City>> GetAllCitiesAsync();
        Task<City?> GetCityByIdAsync(int id);
        Task<City> AddCityAsync(City city);
    }
}