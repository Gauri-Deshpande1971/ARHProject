using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Core.Interfaces;

namespace Core.Specifications
{
    public class BaseSpecification<T> : ISpecification<T>
    {
        public BaseSpecification()
        {
        }

        public BaseSpecification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }

        public Expression<System.Func<T, bool>> Criteria { get; }

        public List<Expression<System.Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();

        public Expression<Func<T, object>> OrderBy { get; private set;}

        public Expression<Func<T, object>> OrderByDescending { get; set;}

        public int Take { get; private set;}

        public int Skip { get; private set;}

        public bool IsPagingEnabled { get; private set;}

        protected void AddInclude(Expression<Func<T, object>> includeExpression) 
        {
            Includes.Add(includeExpression);
        }

        protected void AddOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }

        protected void AddOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
        {
            OrderByDescending = orderByDescendingExpression;
        }

        protected void ApplyPaging(int skip, int take)
        {
            Skip = skip;
            Take = take;
            IsPagingEnabled = true;
        }
    }
}