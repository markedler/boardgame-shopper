using System;
using System.Linq;
using System.Linq.Expressions;

namespace BoardGameShopper.Website.App 
{
    public static class QueryHelper 
    {   
        private static IQueryable<TEntity> OrderByThing<TEntity>(this IQueryable<TEntity> source, string order,
                          string direction) 
        {
            string command = direction == "desc" ? "OrderByDescending" : "OrderBy";
            var type = typeof(TEntity);
            var property = type.GetProperty(order);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                        source.Expression, Expression.Quote(orderByExpression));
            return source.Provider.CreateQuery<TEntity>(resultExpression);
        }
    }
}