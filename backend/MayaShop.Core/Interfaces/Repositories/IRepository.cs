namespace MayaShop.Core.Interfaces.Repositories;

// Contrato base del repositorio genérico.
// Proporciona operaciones CRUD comunes para todas las entidades.
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}
