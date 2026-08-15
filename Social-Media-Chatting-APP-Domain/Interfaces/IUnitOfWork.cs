using System.Linq.Expressions;
using Social_Media_Chatting_APP_Domain.Entities;

namespace Social_Media_Chatting_APP_Domain.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();

        IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : BaseEntity<TKey>;
        
        // for junction tables that don't extend BaseEntity
        Task<TEntity?> FindAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;
        Task AddAsync<TEntity>(TEntity entity) where TEntity : class;
        void Remove<TEntity>(TEntity entity) where TEntity : class;
    }
}
