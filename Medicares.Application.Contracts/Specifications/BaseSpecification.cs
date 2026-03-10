using System.Linq.Expressions;

namespace Medicares.Application.Contracts.Specifications
{
    public abstract class BaseSpecification<T> : ISpecification<T> where T : class
    {
        public Expression<Func<T, bool>>? Criteria { get; protected set; }
        public List<Expression<Func<T, object>>> Includes { get; } = new();
        public List<string> IncludeStrings { get; } = new();
        public string? OrderBy { get; protected set; }
        public string? OrderByDescending { get; protected set; }

        protected BaseSpecification(Expression<Func<T, bool>>? criteria = null)
        {
            Criteria = criteria;
        }

        public void AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString);
        }

        public void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }

        public Expression<Func<T, bool>> And(Expression<Func<T, bool>> query)
        {
            if (Criteria == null)
                return query;

            ParameterExpression parameter = Expression.Parameter(typeof(T));
            BinaryExpression body = Expression.AndAlso(
                Expression.Invoke(Criteria, parameter),
                Expression.Invoke(query, parameter)
            );
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        public Expression<Func<T, bool>> Or(Expression<Func<T, bool>> query)
        {
            if (Criteria == null)
                return query;

            ParameterExpression parameter = Expression.Parameter(typeof(T));
            BinaryExpression body = Expression.OrElse(
                Expression.Invoke(Criteria, parameter),
                Expression.Invoke(query, parameter)
            );
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }
    }
}
