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

public class Specification<T> : ISpecification<T>
{
	private Func<T, Boolean>? _isSatisfiedBy;

	/// <summary>
	/// Inheritors may choose to not supply a Predicate, but instead set the <see cref="Predicate"/> property directly. 
	/// If the Predicate is not set, <see cref="IsSatisfiedBy(TEntity)"/> will always return <see langword="false"/>.
	/// </summary>
	protected Specification() { }

	public Specification(Expression<Func<T, Boolean>> predicate)
		=> Predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));

	/// <summary>
	/// Pass this to <see cref="Queryable.Where{TSource}(IQueryable{TSource}, Expression{Func{TSource, Boolean}})"/> and other methods that accept a predicate.
	/// </summary>
	public virtual Expression<Func<T, Boolean>> Predicate { get; protected set; } = (t) => false;

	protected static Specification<T> CombineSpecification(Specification<T> left, Specification<T> right, Func<Expression, Expression, BinaryExpression> combiner)
	{
		var lExpr = left.Predicate;
		var rExpr = right.Predicate;
		var param = Expression.Parameter(typeof(T));
		var combined = combiner.Invoke(
				new ReplaceParameterVisitor { { lExpr.Parameters.Single(), param } }.Visit(lExpr.Body),
				new ReplaceParameterVisitor { { rExpr.Parameters.Single(), param } }.Visit(rExpr.Body)
			);
		return new ConstructedSpecification(Expression.Lambda<Func<T, Boolean>>(combined, param));
	}

	/// <summary>
	/// Returns <see langword="true"/> if the <see cref="Predicate"/> matches <typeparamref name="T"/>; otherwise <see langword="false"/>. If the Predicate is <see langword="null"/>, <see cref="IsSatisfiedBy(T)"/> will always return <see langword="false"/>.
	/// </summary>
	public Boolean IsSatisfiedBy(T entity)
	{
		if (_isSatisfiedBy is null)
		{
			_isSatisfiedBy = Predicate?.Compile() 
				?? throw new InvalidOperationException("Predicate is null when it must not be null. Did you forget to set or override it in your implementation?");
		}

		return _isSatisfiedBy(entity);
	}

	public override String ToString() => GetType().Name;

	public static implicit operator Expression<Func<T, Boolean>>(Specification<T> spec) => spec.Predicate;

	public static Specification<T> operator &(Specification<T> left, Specification<T> right)
		=> CombineSpecification(left, right, Expression.AndAlso);

	public static Specification<T> operator |(Specification<T> left, Specification<T> right)
		=> CombineSpecification(left, right, Expression.OrElse);

	public static Specification<T> operator !(Specification<T> spec)
	{
		var predicate = spec.Predicate;
		var newExpr = Expression.Lambda<Func<T, Boolean>>(Expression.Not(predicate.Body), predicate.Parameters[0]);
		return new ConstructedSpecification(newExpr);
	}

	protected class ConstructedSpecification : Specification<T>
	{
		public ConstructedSpecification(Expression<Func<T, Boolean>> specificationExpression)
		{
			Predicate = specificationExpression;
		}

		public override Expression<Func<T, Boolean>> Predicate { get; protected set; }
	}
}
