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
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq.Expressions;

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
