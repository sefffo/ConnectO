using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Social_Media_Chatting_APP_Domain.Entities;
using Social_Media_Chatting_APP_Domain.Interfaces;
using Social_Media_Chatting_APP_Domain.Specifications;
using Social_Media_Chatting_APP_Persistence.DbContext;
using Social_Media_Chatting_APP_Persistence.Specifications;

namespace Social_Media_Chatting_APP_Persistence.Repositories
{
    public class GenericRepository<TEntity, TKey>(Social_Media_Chatting_APP_DbContext context)
        : IGenericRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        // ── Base queryable shortcuts ─────────────────────────────────────────
        private IQueryable<TEntity> Tracked => context.Set<TEntity>();
        private IQueryable<TEntity> ReadOnly => context.Set<TEntity>().AsNoTracking();

        // ── Write / tracked reads ────────────────────────────────────────────
        public async Task<IEnumerable<TEntity>> GetAllAsync()
            => await Tracked.ToListAsync();

        public async Task<TEntity> GetByIdAsync(TKey id)
            => await context.Set<TEntity>().FindAsync(id);

        public async Task AddAsync(TEntity entity)
            => await context.Set<TEntity>().AddAsync(entity);

        public void Remove(TEntity entity)
            => context.Set<TEntity>().Remove(entity);

        public void Update(TEntity entity)
            => context.Set<TEntity>().Update(entity);

        public async Task<TEntity?> FindAsync(ISpecification<TEntity> specifications)
            => await SpecificationEvaluator<TEntity>
                .GetQuery(Tracked, specifications)
                .FirstOrDefaultAsync();

        public async Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate)
            => await Tracked.FirstOrDefaultAsync(predicate);

        public async Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate)
            => await Tracked.Where(predicate).ToListAsync();

        public async Task<IEnumerable<TEntity>> FindAllAsync(ISpecification<TEntity> specifications)
            => await SpecificationEvaluator<TEntity>
                .GetQuery(Tracked, specifications)
                .ToListAsync();

        public async Task<TEntity?> GetByIdAsync(ISpecification<TEntity> specifications)
            => await SpecificationEvaluator<TEntity>
                .GetQuery(Tracked, specifications)
                .FirstOrDefaultAsync();

        public Task<int> CountAsync(ISpecification<TEntity> specifications)
            => SpecificationEvaluator<TEntity>
                .GetQuery(Tracked, specifications)
                .CountAsync();

        // ── No-tracking reads ─────────────────────────────────────────────────
        public async Task<TEntity?> FindNoTrackingAsync(ISpecification<TEntity> specifications)
            => await SpecificationEvaluator<TEntity>
                .GetQuery(ReadOnly, specifications)
                .FirstOrDefaultAsync();

        public async Task<TEntity?> FindNoTrackingAsync(Expression<Func<TEntity, bool>> predicate)
            => await ReadOnly.FirstOrDefaultAsync(predicate);

        public async Task<IEnumerable<TEntity>> FindAllNoTrackingAsync(ISpecification<TEntity> specifications)
            => await SpecificationEvaluator<TEntity>
                .GetQuery(ReadOnly, specifications)
                .ToListAsync();

        public async Task<IEnumerable<TEntity>> FindAllNoTrackingAsync(Expression<Func<TEntity, bool>> predicate)
            => await ReadOnly.Where(predicate).ToListAsync();
    }
}
