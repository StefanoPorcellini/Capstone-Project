using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestioneOrdini.Interface
{
    public interface IGenericService<TEntity> where TEntity : class
    {
        Task<TEntity> CreateAsync(TEntity entity);
        Task<TEntity?> GetByIdAsync(int id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(int id);
    }
}
