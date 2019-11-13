/*******************************************************************************
 * MIT License
 *
 * Copyright 2020 Provision Data Systems Inc.  https://provisiondata.com
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a 
 * copy of this software and associated documentation files (the "Software"),
 * to deal in the Software without restriction, including without limitation
 * the rights to use, copy, modify, merge, publish, distribute, sublicense,
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
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public abstract class AbstractSpecification<TDomainModel> : IQueryableSpecification<TDomainModel>
    {
        private Func<TDomainModel, Boolean> _isSatisfiedBy;

        public abstract Expression<Func<TDomainModel, Boolean>> SpecificationExpression { get; }

        public Boolean IsSatisfiedBy(TDomainModel entity)
        {
            if (_isSatisfiedBy is null)
                _isSatisfiedBy = SpecificationExpression.Compile();

            return _isSatisfiedBy(entity);
        }

        public static implicit operator Expression<Func<TDomainModel, Boolean>>(AbstractSpecification<TDomainModel> spec)
            => spec.SpecificationExpression;

        public static AbstractSpecification<TDomainModel> operator &(AbstractSpecification<TDomainModel> left, AbstractSpecification<TDomainModel> right)
            => CombineSpecification(left, right, Expression.AndAlso);

        public static AbstractSpecification<TDomainModel> operator |(AbstractSpecification<TDomainModel> left, AbstractSpecification<TDomainModel> right)
            => CombineSpecification(left, right, Expression.OrElse);

        public static AbstractSpecification<TDomainModel> operator !(AbstractSpecification<TDomainModel> spec)
        {
            Expression<Func<TDomainModel, Boolean>> inverted = model => !spec.IsSatisfiedBy(model);
            var param = Expression.Parameter(typeof(TDomainModel));
            var expr = new ReplaceParameterVisitor { { inverted.Parameters.Single(), param } }.Visit(inverted.Body);
            var lambda = Expression.Lambda<Func<TDomainModel, Boolean>>(expr, param);
            return new ConstructedSpecification<TDomainModel>(lambda);
        }

        protected static AbstractSpecification<TDomainModel> CombineSpecification(AbstractSpecification<TDomainModel> left, AbstractSpecification<TDomainModel> right, Func<Expression, Expression, BinaryExpression> combiner)
        {
            var lExpr = left.SpecificationExpression;
            var rExpr = right.SpecificationExpression;
            var param = Expression.Parameter(typeof(TDomainModel));
            var combined = combiner.Invoke(
                    new ReplaceParameterVisitor { { lExpr.Parameters.Single(), param } }.Visit(lExpr.Body),
                    new ReplaceParameterVisitor { { rExpr.Parameters.Single(), param } }.Visit(rExpr.Body)
                );
            return new ConstructedSpecification<TDomainModel>(Expression.Lambda<Func<TDomainModel, Boolean>>(combined, param));
        }

        protected class ConstructedSpecification<T> : AbstractSpecification<T>
        {
            private readonly Expression<Func<T, Boolean>> _expr;

            public ConstructedSpecification(Expression<Func<T, Boolean>> specificationExpression)
            {
                _expr = specificationExpression;
            }

            public override Expression<Func<T, Boolean>> SpecificationExpression => _expr;
        }
    }

    internal class ReplaceParameterVisitor : ExpressionVisitor, IEnumerable<KeyValuePair<ParameterExpression, ParameterExpression>>
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> _map = new Dictionary<ParameterExpression, ParameterExpression>();

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (_map.TryGetValue(node, out var newValue))
                return newValue;

            return node;
        }

        public void Add(ParameterExpression parameterToReplace, ParameterExpression replaceWith)
            => _map.Add(parameterToReplace, replaceWith);

        public IEnumerator<KeyValuePair<ParameterExpression, ParameterExpression>> GetEnumerator()
            => _map.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
