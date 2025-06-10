using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.EntityFrameworkCore;

#nullable enable
namespace Infrastructure.Data
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly DBServerContext _context;
        public GenericRepository(DBServerContext context)
        {
            _context = context;
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<T> GetByNameAsync(string PropertyName, string PropertyValue)
        {
            var type = typeof(T);

            var property = type.GetProperty(PropertyName);

            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var constantValue = Expression.Constant(PropertyValue);

            var equality = Expression.Equal(propertyAccess, constantValue);

            var criteria = Expression.Lambda<Func<T, bool>>(equality, parameter);   //  .compile()
            var spec = new BaseSpecification<T>(criteria);

            return await GetEntityWithSpec(spec);
        }

        public async Task<IReadOnlyList<T>> ListAllAsync()
        {
            return await _context.Set<T>()
                //  .AsNoTracking()
                .Where(x => x.IsDeleted == false)
                .ToListAsync();
        }

        public async Task<T> GetEntityWithSpec(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).Where(x => x.IsDeleted == false).FirstOrDefaultAsync() ;
        }

        public async  Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec)
        {
            try
            {
                return await ApplySpecification(spec)
                    //  .AsNoTracking()
                    .Where(x => x.IsDeleted == false)
                    .ToListAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.InnerException);
            }
            return null;
        }

        public IQueryable<T> ListIQueryable(ISpecification<T> spec)
        {
            try
            {
                return ApplySpecification(spec).Where(x => x.IsDeleted == false);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.InnerException);
            }
            return null;
        }

        public async Task<int> CountAsync()
        {
            return await _context.Set<T>()
                .AsNoTracking()
                .Where(x => x.IsDeleted == false)
                .CountAsync();
        }

        public async Task<int> CountAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).CountAsync();
        }

        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return SpecificationEvaluator<T>.GetQuery(_context.Set<T>().AsQueryable()
                    .Where(x => x.IsDeleted == false), spec);
        }

        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            _context.Set<T>().Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
            
        }

        public void DeleteRange(List<T> entity)
        {
            _context.Set<T>().RemoveRange(entity);
            
        }

        public async Task<int> Complete()
        {
            return await _context.SaveChangesAsync();
        }

        public int MaxIdAsync()
        {
            return _context.Set<T>().Max(x => x.Id);
        }

        public IEnumerable<TResult> GetGrouped<TKey, TResult>(
            Expression<Func<T, TKey>> groupingKey,
            Expression<Func<IGrouping<TKey, T>, TResult>> resultSelector,
            Expression<Func<T, bool>>? filter = null)
        {
            var query = _context.Set<T>().AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query.GroupBy(groupingKey).Select(resultSelector);
        }

        public TKey? MaxOf<TKey>(Expression<Func<T, TKey>> OfField,
            Expression<Func<T, bool>>? filter = null)
        {
            var query = _context.Set<T>().AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query.Max(OfField);
        }

        public IEnumerable<TResult> GetSelectColumns<TResult>(
            Expression<Func<T, TResult>> resultSelector,
            Expression<Func<T, bool>>? filter = null)
        {
            var query = _context.Set<T>().AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query.Select(resultSelector);
        }

    }
}
