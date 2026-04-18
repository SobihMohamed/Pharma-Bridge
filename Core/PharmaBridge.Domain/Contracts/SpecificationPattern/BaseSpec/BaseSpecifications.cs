using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Text;

namespace PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec
{
    public abstract class BaseSpecifications<TEntity, TKey>
        : ISpecifications<TEntity, TKey> where TEntity : class, IEntity<TKey>
    {
        protected BaseSpecifications()
        {
            Criteria = x => true; // default criteria that always returns true
        }
        #region Apply Criteria
        public Expression<Func<TEntity, bool>> Criteria {  get; private set; }
        public BaseSpecifications(Expression<Func<TEntity, bool>> _criteria) // where clause => p => p.Name == "test"
            => Criteria = _criteria;
        #endregion

        #region Apply Includes
        public List<Expression<Func<TEntity, object>>> Includes { get; private set; } = new List<Expression<Func<TEntity, object>>>(); // category supplier 
        protected void AddInclude(Expression<Func<TEntity, object>> includeExpression)
            => Includes.Add(includeExpression);
        #endregion

        #region nested include strings
        public List<string> IncludeStrings { get; private set; } = new List<string>();
        protected void AddInclude(string includeString)
            => IncludeStrings.Add(includeString);
        #endregion

        #region Apply OrderBy
        public List<OrderExpressionInfo<TEntity>> OrderByExpressions { get; private set; } = new List<OrderExpressionInfo<TEntity>>(); // OrderBy(p => p.Name) , OrderByDescending(p => p.Price)
        protected void AddOrderBy(Expression<Func<TEntity,object>> orderByExpression , bool isDescending = false)
            => OrderByExpressions.Add(new OrderExpressionInfo<TEntity>
            {
                OrderByExpression = orderByExpression,
                IsDescending = isDescending
            });
        #endregion

        #region Apply Pagination
        public int Take { get; private set; }

        public int Skip { get; private set; }

        public bool IsPagingEnabled { get; private set; }

        protected void ApplyPaging(int PageSize , int PageIndex) // PageSize = 10 , PageIndex = 3 => Skip = 20 , Take = 10
        {
            Skip = (PageIndex - 1) * PageSize;
            Take = PageSize;
            IsPagingEnabled = true;
        }
        #endregion
    }
}
