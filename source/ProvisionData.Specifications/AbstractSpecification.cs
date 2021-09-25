﻿/*******************************************************************************
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

namespace ProvisionData.Specifications
{
	using System;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Text.Json.Serialization;

	public abstract class AbstractSpecification<TDomainModel> : IQuerySpecification<TDomainModel>
	{
		private Func<TDomainModel, Boolean>? _isSatisfiedBy;

		/// <summary>
		/// Inheritors may choose to not supply a Specification or Predicate, but instead set the <see cref="Predicate"/> property directly. If the Predicate is not set, <see cref="IsSatisfiedBy(TDomainModel)"/> will always return <see langword="false"/>.
		/// </summary>
		protected AbstractSpecification() { Predicate = null!; }

		public AbstractSpecification(IQuerySpecification<TDomainModel> specification)
			: this(specification?.Predicate ?? throw new InvalidOperationException("Must supply a Specification with a valid (not null) Predicate.")) { }

		public AbstractSpecification(Expression<Func<TDomainModel, Boolean>> predicate)
			=> Predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));


		/// <summary>
		/// Pass this to <see cref="Queryable.Where{TSource}(IQueryable{TSource}, Expression{Func{TSource, Boolean}})"/> and other methods that accept a predicate.
		/// </summary>
		[JsonIgnore]
		public Expression<Func<TDomainModel, Boolean>> Predicate { get; protected set; }

		/// <summary>
		/// Returns <see langword="true"/> if the <see cref="Predicate"/> matches <typeparamref name="TDomainModel"/>; otherwise <see langword="false"/>. If the Predicate is <see langword="null"/>, <see cref="IsSatisfiedBy(TDomainModel)"/> will always return <see langword="false"/>.
		/// </summary>
		public Boolean IsSatisfiedBy(TDomainModel entity)
		{
			if (_isSatisfiedBy is null)
			{
				_isSatisfiedBy = Predicate?.Compile();
			}

			return _isSatisfiedBy is not null && _isSatisfiedBy(entity);
		}

		public override String ToString() => GetType().Name;

		public static implicit operator Expression<Func<TDomainModel, Boolean>>(AbstractSpecification<TDomainModel> spec)
			=> spec.Predicate;

		public static AbstractSpecification<TDomainModel> operator &(AbstractSpecification<TDomainModel> left, AbstractSpecification<TDomainModel> right)
			=> CombineSpecification(left, right, Expression.AndAlso);

		public static AbstractSpecification<TDomainModel> operator |(AbstractSpecification<TDomainModel> left, AbstractSpecification<TDomainModel> right)
			=> CombineSpecification(left, right, Expression.OrElse);

		public static AbstractSpecification<TDomainModel> operator !(AbstractSpecification<TDomainModel> spec)
		{
			var predicate = spec.Predicate;
			var newExpr = Expression.Lambda<Func<TDomainModel, Boolean>>(Expression.Not(predicate.Body), predicate.Parameters[0]);
			return new ConstructedSpecification<TDomainModel>(newExpr);
		}

		protected static AbstractSpecification<TDomainModel> CombineSpecification(AbstractSpecification<TDomainModel> left, AbstractSpecification<TDomainModel> right, Func<Expression, Expression, BinaryExpression> combiner)
		{
			var lExpr = left.Predicate;
			var rExpr = right.Predicate;
			var param = Expression.Parameter(typeof(TDomainModel));
			var combined = combiner.Invoke(
					new ReplaceParameterVisitor { { lExpr.Parameters.Single(), param } }.Visit(lExpr.Body),
					new ReplaceParameterVisitor { { rExpr.Parameters.Single(), param } }.Visit(rExpr.Body)
				);
			return new ConstructedSpecification<TDomainModel>(Expression.Lambda<Func<TDomainModel, Boolean>>(combined, param));
		}

		protected class ConstructedSpecification<T> : AbstractSpecification<T>
		{
			public ConstructedSpecification(Expression<Func<T, Boolean>> specificationExpression)
				: base(specificationExpression)
			{
			}
		}
	}
}
