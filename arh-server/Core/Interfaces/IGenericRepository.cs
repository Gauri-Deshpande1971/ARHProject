using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Entities;
using Core.Specifications;

#nullable enable

namespace Core.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<T> GetByIdAsync(int id);
        
        Task<T> GetByNameAsync(string PropertyName, string PropertyValue);

         Task<IReadOnlyList<T>> ListAllAsync();

         Task<T> GetEntityWithSpec(ISpecification<T> spec);

         Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);

         Task<int> CountAsync(ISpecification<T> spec);

         void Add(T entity);

         void Update(T entity);

         void Delete(T entity);

         void DeleteRange(List<T> entity);

         Task<int> Complete();

        int MaxIdAsync();
        Task<int> MaxNumericPrefixFromStringFieldAsync(
    Expression<Func<T, string>> fieldSelector,
    Expression<Func<T, bool>>? filter = null,
    char separator = '-');

        IEnumerable<TResult>GetGrouped<TKey, TResult>(
            Expression<Func<T, TKey>> groupingKey,
            Expression<Func<IGrouping<TKey, T>, TResult>> resultSelector,
            Expression<Func<T, bool>>? filter = null);

        TKey? MaxOf<TKey>(Expression<Func<T, TKey>> OfField,
            Expression<Func<T, bool>>? filter = null);
        
        IEnumerable<TResult> GetSelectColumns<TResult>(
            Expression<Func<T, TResult>> resultSelector,
            Expression<Func<T, bool>>? filter = null);
    }
}
