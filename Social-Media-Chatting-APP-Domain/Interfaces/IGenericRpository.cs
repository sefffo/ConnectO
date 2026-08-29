using System.Linq.Expressions;
using Social_Media_Chatting_APP_Domain.Entities;
using Social_Media_Chatting_APP_Domain.Specifications;

namespace Social_Media_Chatting_APP_Domain.Interfaces
{
    public interface IGenericRepository<TEntity, T> where TEntity : BaseEntity<T>
    {
        Task<IEnumerable<TEntity>> GetAllAsync();

        Task<TEntity> GetByIdAsync(T id);

        Task AddAsync(TEntity entity);

        void Remove(TEntity entity);

        void Update(TEntity entity);

        // ── Tracked reads (use when mutation follows) ────────────────────────
        Task<TEntity?> FindAsync(ISpecification<TEntity> specifications);
        Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TEntity>> FindAllAsync(ISpecification<TEntity> specifications);
        Task<TEntity?> GetByIdAsync(ISpecification<TEntity> specifications);
        Task<int> CountAsync(ISpecification<TEntity> specifications);

        // ── No-tracking reads (use for pure queries — no mutation after fetch) ─
        Task<TEntity?> FindNoTrackingAsync(ISpecification<TEntity> specifications);
        Task<TEntity?> FindNoTrackingAsync(Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TEntity>> FindAllNoTrackingAsync(ISpecification<TEntity> specifications);
        Task<IEnumerable<TEntity>> FindAllNoTrackingAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
