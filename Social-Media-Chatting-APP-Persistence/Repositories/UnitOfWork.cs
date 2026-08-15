using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Social_Media_Chatting_APP_Domain.Entities;
using Social_Media_Chatting_APP_Domain.Interfaces;
using Social_Media_Chatting_APP_Persistence.DbContext;

namespace Social_Media_Chatting_APP_Persistence.Repositories
{
    public class UnitOfWork(Social_Media_Chatting_APP_DbContext context) : IUnitOfWork
    {

        private readonly Dictionary<Type, object> repositories = [];
        public IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : BaseEntity<TKey>
        {

            //check the dictionary if the repository for the given entity type already exists, if it does return it,
            //otherwise create a new one and add it to the dictionary before returning it
            var EntityType = typeof(TEntity);
            if (repositories.TryGetValue(EntityType, out object? repository))
                return (IGenericRepository<TEntity, TKey>)repository;


            var newRepository = new GenericRepository<TEntity, TKey>(context);
            repositories.Add(EntityType, newRepository);
            return newRepository;

        }




        public async Task<int> SaveChangesAsync()
           => await context.SaveChangesAsync();
        
        
        public async Task<TEntity?> FindAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) 
            where TEntity : class
            => await context.Set<TEntity>().FirstOrDefaultAsync(predicate);

        public async Task AddAsync<TEntity>(TEntity entity) 
            where TEntity : class
            => await context.Set<TEntity>().AddAsync(entity);

        public void Remove<TEntity>(TEntity entity) 
            where TEntity : class
            => context.Set<TEntity>().Remove(entity);


    }
}
