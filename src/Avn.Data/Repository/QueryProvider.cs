using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Avn.Data.Repository;

//public class QueryProvider<TEntity> : IAsyncQueryProvider where TEntity : class
//{
//    private readonly DbSet<TEntity> _targetDbSet;

//    public QueryProvider(DbSet<TEntity> targetDbSet)
//        => _targetDbSet = targetDbSet;

//    public IQueryable<TResult> CreateQuery<TResult>(Expression expression)
//        => _targetDbSet.AsQueryable()
//                       .Provider
//                       .CreateQuery<TResult>(new ExpressionRootReplacing<TEntity>(_targetDbSet)
//                                                 .Visit(expression));

//    public IQueryable CreateQuery(Expression expression)
//           => throw new NotSupportedException();

//    public object Execute(Expression expression)
//           => throw new NotSupportedException();

//    public TResult Execute<TResult>(Expression expression)
//        => throw new NotSupportedException();

//    public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
//    {
//        if (_targetDbSet.AsQueryable().Provider is IAsyncQueryProvider provider)
//            return provider.ExecuteAsync<TResult>(
//               new ExpressionRootReplacing<TEntity>(_targetDbSet)
//                                                 .Visit(expression),
//                cancellationToken);
//        throw new InvalidOperationException("IQueryableProviderNotAsync");
//    }
//}