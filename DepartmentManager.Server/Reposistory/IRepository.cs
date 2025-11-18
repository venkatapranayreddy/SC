using System.Linq.Expressions;

namespace DepartmentManager.Server.Reposistory
{
    public interface IRepository<T> where T : class
    {
        // Get all entities
        Task<IEnumerable<T>> GetAllAsync();
        
        // Get entity by ID
        Task<T?> GetByIdAsync(int id);
        
        // Find entities by condition
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        
        // Get single entity by condition
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        
        // Add new entity
        Task<T> AddAsync(T entity);
        
        // Add multiple entities
        Task AddRangeAsync(IEnumerable<T> entities);
        
        // Update entity
        Task UpdateAsync(T entity);
        
        // Delete entity by ID
        Task DeleteAsync(int id);
        
        // Delete entity
        Task DeleteAsync(T entity);
        
        // Delete multiple entities
        Task DeleteRangeAsync(IEnumerable<T> entities);
        
        // Check if entity exists
        Task<bool> ExistsAsync(int id);
        
        // Count entities
        Task<int> CountAsync();
        
        // Count entities by condition
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
    }
}

