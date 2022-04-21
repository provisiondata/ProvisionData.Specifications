/*******************************************************************************
 * MIT License
 *
 * Copyright 2021 Provision Data Systems Inc.  https://provisiondata.com
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a 
 * copy of this software and associated documentation files (the "Software"),
 * to deal in the Software without restriction, including without limitation
 * the rights to use, copy, modify, merge, publish, distribute, sub-license,
 * and/or sell copies of the Software, and to permit persons to whom the 
 * Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
 * DEALINGS IN THE SOFTWARE.
 *
 *******************************************************************************/

using System.Linq;
using System.Linq.Expressions;

namespace ProvisionData.Specifications;

public class Query<TEntity> : IQuery<TEntity> where TEntity : class
{
	private Func<TEntity, Boolean>? _isSatisfiedBy;

	/// <summary>
	/// Inheritors may choose to not supply a Predicate, but instead set the <see cref="Predicate"/> property directly. 
	/// If the Predicate is not set, <see cref="IsSatisfiedBy(TEntity)"/> will always return <see langword="false"/>.
	/// </summary>
	protected Query() { }

	public Query(Expression<Func<TEntity, Boolean>> predicate)
		=> Predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));

	public List<Expression<Func<TEntity, Object>>> Includes { get; } = new();
	public Expression<Func<TEntity, Object>>? OrderBy { get; private set; }
	public Expression<Func<TEntity, Object>>? OrderByDescending { get; private set; }
	public Expression<Func<TEntity, Object>>? GroupBy { get; private set; }

	public Int32 Take { get; private set; }
	public Int32 Skip { get; private set;  }
	public Boolean IsPagingEnabled { get; private set; } = false;

	protected virtual void ApplyGroupBy(Expression<Func<TEntity, Object>> groupByExpression) => GroupBy = groupByExpression;

	protected virtual void AddInclude(Expression<Func<TEntity, Object>> includeExpression) => Includes.Add(includeExpression);

	protected virtual void ApplyOrderBy(Expression<Func<TEntity, Object>> orderByExpression) => OrderBy = orderByExpression;

	protected virtual void ApplyOrderByDescending(Expression<Func<TEntity, Object>> orderByDescendingExpression) => OrderByDescending = orderByDescendingExpression;

	protected virtual void ApplyPaging(Int32 skip, Int32 take, Expression<Func<TEntity, Object>> orderByExpression)
	{
		Skip = skip;
		Take = take;
		OrderBy = orderByExpression;
		IsPagingEnabled = true;
	}

	/// <summary>
	/// Pass this to <see cref="Queryable.Where{TSource}(IQueryable{TSource}, Expression{Func{TSource, Boolean}})"/> and other methods that accept a predicate.
	/// </summary>
	public virtual Expression<Func<TEntity, Boolean>> Predicate { get; protected set; } = (t) => false;

	protected static Query<TEntity> CombineSpecification(Query<TEntity> left, Query<TEntity> right, Func<Expression, Expression, BinaryExpression> combiner)
	{
		var lExpr = left.Predicate;
		var rExpr = right.Predicate;
		var param = Expression.Parameter(typeof(TEntity));
		var combined = combiner.Invoke(
				new ReplaceParameterVisitor { { lExpr.Parameters.Single(), param } }.Visit(lExpr.Body),
				new ReplaceParameterVisitor { { rExpr.Parameters.Single(), param } }.Visit(rExpr.Body)
			);

		return new ConstructedSpecification(Expression.Lambda<Func<TEntity, Boolean>>(combined, param));
	}

	/// <summary>
	/// Returns <see langword="true"/> if the <see cref="Predicate"/> matches <typeparamref name="TEntity"/>; otherwise <see langword="false"/>. If the Predicate is <see langword="null"/>, <see cref="IsSatisfiedBy(TEntity)"/> will always return <see langword="false"/>.
	/// </summary>
	public Boolean IsSatisfiedBy(TEntity entity)
	{
		if (_isSatisfiedBy is null)
		{
			_isSatisfiedBy = Predicate?.Compile()
				?? throw new InvalidOperationException("Predicate is null when it must not be null. Did you forget to set or override it in your implementation?");
		}

		return _isSatisfiedBy(entity);
	}

	public override String ToString() => GetType().Name;

	public static implicit operator Expression<Func<TEntity, Boolean>>(Query<TEntity> spec) => spec.Predicate;

	public static Query<TEntity> operator &(Query<TEntity> left, Query<TEntity> right)
		=> CombineSpecification(left, right, Expression.AndAlso);

	public static Query<TEntity> operator |(Query<TEntity> left, Query<TEntity> right)
		=> CombineSpecification(left, right, Expression.OrElse);

	public static Query<TEntity> operator !(Query<TEntity> spec)
	{
		var predicate = spec.Predicate;
		var newExpr = Expression.Lambda<Func<TEntity, Boolean>>(Expression.Not(predicate.Body), predicate.Parameters[0]);
		return new ConstructedSpecification(newExpr);
	}

	protected class ConstructedSpecification : Query<TEntity>
	{
		public ConstructedSpecification(Expression<Func<TEntity, Boolean>> specificationExpression)
		{
			Predicate = specificationExpression;
		}

		public override Expression<Func<TEntity, Boolean>> Predicate { get; protected set; }
	}
}
