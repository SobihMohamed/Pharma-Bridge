using Microsoft.EntityFrameworkCore;
using PharmaBridge.Domain.Contracts;
using PharmaBridge.Domain.Contracts.SpecificationPattern;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Persistence.Evaluator
{
    public static class SpecificationEvaluator // select * from TEntity where criteria order by orderby skip skip take take 
    {
        public static IQueryable<TEntity> GenerateQuery<TEntity,TKey>
            (IQueryable<TEntity> BaseQuery , ISpecifications<TEntity,TKey> specifications)
             where TEntity : class , IEntity<TKey>
        {
            var query = BaseQuery; // select * from product

            // where criteria
            if (specifications.Criteria != null)
                query = query.Where(specifications.Criteria); // select * from product where price > 100

            // join with other tables
            if(specifications.Includes is not null && specifications.Includes.Any()) // include category and include supplier
            {
                foreach (var includeExpression in specifications.Includes)
                {
                    query = query.Include(includeExpression); // select * from product include category include supplier
                }
            }

            //  Nested Includes
            if (specifications.IncludeStrings is not null && specifications.IncludeStrings.Any())
            {
                foreach (var includeString in specifications.IncludeStrings)
                {
                    query = query.Include(includeString); // include nested properties like "PharmaOwner.ApplicationUser"
                }
            }

            // order by
            if (specifications.OrderByExpressions is not null && specifications.OrderByExpressions.Any()) // order by price desc, order by name asc
            {
                // 1- get the first orderby 
                var firstOrderBy = specifications.OrderByExpressions.FirstOrDefault();
                //check if the orderby is desc or asc
                IOrderedQueryable<TEntity> orderQuery = firstOrderBy.IsDescending ?
                    query.OrderByDescending(firstOrderBy.OrderByExpression)
                    : query.OrderBy(firstOrderBy.OrderByExpression);
                // select * from product include category include supplier order by price desc

                for(int i = 1; i < specifications.OrderByExpressions.Count; i++)
                {
                    var nextOrderBy = specifications.OrderByExpressions[i];
                    // Check if the orderby is desc or asc
                    orderQuery = nextOrderBy.IsDescending ?
                        orderQuery.ThenByDescending(nextOrderBy.OrderByExpression)
                        : orderQuery.ThenBy(nextOrderBy.OrderByExpression);

                    // select * from product include category include supplier order by price desc, order by name asc
                }

                query = orderQuery; // select * from product include category include supplier order by price desc, order by name asc
            }
            // pagination
            if (specifications.IsPagingEnabled)
                query = query.Skip(specifications.Skip).Take(specifications.Take); // select * from product include category include supplier order by price desc, order by name asc skip 10 take 10

            return query;
        }
    }
}
