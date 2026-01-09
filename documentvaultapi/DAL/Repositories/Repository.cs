//using Microsoft.EntityFrameworkCore.Storage;
//using Microsoft.EntityFrameworkCore;
//using System.Linq.Expressions;

//namespace documentvaultapi.DAL.Repositories
//{
//    public class Repository
//    {
//    }
//}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;
using documentvaultapi.DAL.Entities;
using documentvaultapi.DAL.Repositories.Interfaces;

namespace documentvaultapi.DAL.Repositories
{
    public abstract class Repository<T, Tcontext> : IRepository<T> where T : class where Tcontext : DbContext
    {
        protected readonly Tcontext DocumentVaultDbContext = null;

        public Repository(Tcontext context)
        {
            this.DocumentVaultDbContext = context;
        }


        public IExecutionStrategy GetExecutionStrategy()
        {
            return this.DocumentVaultDbContext.Database.CreateExecutionStrategy();
        }


        public IQueryable<T> GetAllByCondition(Expression<Func<T, bool>> condition)
        {
            IQueryable<T> result = this.DocumentVaultDbContext.Set<T>();
            if (condition != null)
            {
                result = result.Where(condition);
            }

            return result;
        }

        public async Task<ICollection<T>> GetAllByConditionAsync(Expression<Func<T, bool>> condition)
        {
            IQueryable<T> result = this.DocumentVaultDbContext.Set<T>();
            if (condition != null)
            {
                result = result.Where(condition);
            }

            return await result.ToListAsync();
        }

        public IQueryable<T> GetAll()
        {
            IQueryable<T> result = this.DocumentVaultDbContext.Set<T>();
            return result;
        }

        public async Task<ICollection<T>> GetAllAsync()
        {
            IQueryable<T> result = this.DocumentVaultDbContext.Set<T>();
            return await result.ToListAsync();
        }

        public async Task<ICollection<TResult>> GetSelectedColumnAsync<TResult>(Expression<Func<T, TResult>> selectExpression)
        {
            IQueryable<TResult> result = this.DocumentVaultDbContext.Set<T>().Select(selectExpression);
            return await result.ToListAsync();
        }

        public async Task<ICollection<TResult>> GetSelectedColumnByConditionAsync<TResult>(Expression<Func<T, bool>> filterExpression, Expression<Func<T, TResult>> selectExpression)
        {
            IQueryable<TResult> result = this.DocumentVaultDbContext.Set<T>()
                                    .Where(filterExpression)
                                    .Select(selectExpression);

            return await result.ToListAsync();
        }
        public async Task<(long TotalCount, ICollection<TResult> Data)> GetSelectedColumnByConditionAsyncWithOffsetLimitAndTotalCount<TResult>(
                Expression<Func<T, bool>> filterExpression,
                Expression<Func<T, TResult>> selectExpression,
                int? limit = null,
                int? offset = null)
        {
            // Query for counting total records
            long totalCount = await this.DocumentVaultDbContext.Set<T>()
                                    .Where(filterExpression)
                                    .CountAsync();

            // Query for fetching limited data
            IQueryable<T> query = this.DocumentVaultDbContext.Set<T>().Where(filterExpression);

            // Apply offset if specified
            if (offset.HasValue)
            {
                query = query.Skip(offset.Value);
            }

            // Apply limit if specified
            if (limit.HasValue)
            {
                query = query.Take(limit.Value);
            }

            // Select the desired columns
            IQueryable<TResult> result = query.Select(selectExpression);

            // Fetch the data
            var data = await result.ToListAsync();

            // Return the total count and the data
            return (TotalCount: totalCount, Data: data);
        }
        public async Task<ICollection<TResult>> GetSelectedColumnByConditionAsyncWithOffsetLimit<TResult>(
                Expression<Func<T, bool>> filterExpression,
                Expression<Func<T, TResult>> selectExpression,
                int? limit = null,
                int? offset = null)
        {

            // Query for fetching limited data
            IQueryable<T> query = this.DocumentVaultDbContext.Set<T>().Where(filterExpression);

            // Apply offset if specified
            if (offset.HasValue)
            {
                query = query.Skip(offset.Value);
            }

            // Apply limit if specified
            if (limit.HasValue)
            {
                query = query.Take(limit.Value);
            }

            // Select the desired columns
            IQueryable<TResult> result = query.Select(selectExpression);

            // Fetch the data
            var data = await result.ToListAsync();

            // Return the total count and the data
            return data;
        }

        public async Task<Dictionary<TKey, List<TResult>>> GetSelectedColumnGroupByConditionAsync<TKey, TResult>(
            Expression<Func<T, bool>> filterExpression,
            Expression<Func<T, TKey>> groupByKeySelector,
            Expression<Func<T, TResult>> selectExpression)
        {
            var data = await this.DocumentVaultDbContext.Set<T>()
            .Where(filterExpression)
            .ToListAsync();
            var groupedResult = data
                .GroupBy(groupByKeySelector.Compile())
                .ToDictionary(group => group.Key, group => group.Select(selectExpression.Compile()).ToList());

            return groupedResult;

        }

        public async Task<TResult> GetSingleSelectedColumnByConditionAsync<TResult>(
       Expression<Func<T, bool>> filterExpression,
       Expression<Func<T, TResult>> selectExpression)
        {
            TResult result = await this.DocumentVaultDbContext.Set<T>()
                                        .Where(filterExpression)
                                        .Select(selectExpression)
                                        .FirstOrDefaultAsync();

            return result;
        }

        public T GetSingle(Expression<Func<T, bool>> condition)
        {
            return this.DocumentVaultDbContext.Set<T>().Where(condition).FirstOrDefault();
        }

        public async Task<T> GetSingleAysnc(Expression<Func<T, bool>> condition)
        {
            var retValue = await this.DocumentVaultDbContext.Set<T>().Where(condition).SingleOrDefaultAsync();

            return retValue;
        }

        public async Task<object> ExecuteQuery(string sqlQuery, object parameters)
        {
            return await this.DocumentVaultDbContext.Database.ExecuteSqlRawAsync(sqlQuery, parameters);
        }


        public bool Add(T entity)
        {
            this.DocumentVaultDbContext.Set<T>().Add(entity);
            return true;
        }

        public bool Update(T entity)
        {
            this.DocumentVaultDbContext.Entry(entity).State = EntityState.Modified;
            return true;
        }

        public bool Delete(T entity)
        {
            this.DocumentVaultDbContext.Set<T>().Remove(entity);
            return true;
        }

        public bool DeleteRange(IEnumerable<T> entities)
        {
            //if (entities == null || !entities.Any())
            //    throw new ArgumentException("Entities collection cannot be null or empty.");

            this.DocumentVaultDbContext.Set<T>().RemoveRange(entities);
            return true;
        }

        public void SaveChangesManaged()
        {
            this.DocumentVaultDbContext.SaveChanges();
        }


        public bool IsTransactionRunning()
        {
            return this.DocumentVaultDbContext.Database.CurrentTransaction == null ? false : true;
        }

        private IDbContextTransaction BeginTran()
        {
            return this.DocumentVaultDbContext.Database.BeginTransaction();
        }

        public void AddRange(IEnumerable<T> entities)
        {
            this.DocumentVaultDbContext.Set<T>().AddRange(entities);
        }
        public IQueryable<T> GetFiltered(Expression<Func<T, bool>> predicate)
        {
            return this.DocumentVaultDbContext.Set<T>().Where(predicate);
        }

        public async Task<T?> GetSingleAsync(Expression<Func<T, bool>> condition)
        {
            return await this.DocumentVaultDbContext.Set<T>()
                .Where(condition)
                .SingleOrDefaultAsync();
        }


    }
}

