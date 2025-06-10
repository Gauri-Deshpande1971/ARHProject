using System.Linq;
using Core.Entities;
using Core.Interfaces;
//  using Core.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class SpecificationEvaluator<TEntity> where TEntity : BaseEntity
    {
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, 
            ISpecification<TEntity> spec)
            {
                var query = inputQuery;

                if (spec.Criteria != null)
                {
                    query = query.Where(spec.Criteria);
                }

                query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));

                if (spec.OrderBy != null)
                {
                    query = query.OrderBy(spec.OrderBy);
                }
                else if (spec.OrderByDescending != null)
                {
                    query = query.OrderByDescending(spec.OrderByDescending);
                }

                // if (spec.GroupBy != null)
                // {
                //     query = query.GroupBy(spec.GroupBy).SelectMany(x => x);
                // }

                if (spec.IsPagingEnabled)
                {
                    query = query.Skip(spec.Skip)
                        .Take(spec.Take);
                }

                return query;
            }
        
    }
}