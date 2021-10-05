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

namespace ProvisionData.Specifications
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;

	public static class QueryableExtensions
	{
		const String OrderBy = "OrderBy";
		const String ThenBy = "ThenBy";
		const String OrderByDesc = "OrderByDescending";
		const String ThenByDesc = "ThenByDescending";

		public static IQueryable<T> SortBy<T>(this IQueryable<T> source, IEnumerable<SortBy> sortModels)
		{
			var expression = source.Expression;
			Int32 count = 0;
			foreach (var item in sortModels)
			{
				try
				{
					if (String.IsNullOrWhiteSpace(item.PropertyName))
						continue;

					var parameter = Expression.Parameter(typeof(T), "x");
					var selector = Expression.PropertyOrField(parameter, item.PropertyName);
					var method = item.Descending ?
						(count == 0 ? OrderByDesc : ThenByDesc) :
						(count == 0 ? OrderBy : ThenBy);
					expression = Expression.Call(typeof(Queryable), method,
						new Type[] { source.ElementType, selector.Type }, expression,
						Expression.Quote(Expression.Lambda(selector, parameter)));
					count++;
				}
				catch (ArgumentException)
				{
					// Likely thrown by the call to Expression.PropertyOrField(parameter, item.PropertyName)
					// ArgumentNullException => expression or propertyOrFieldName is null.
					// ArgumentException => No property or field named propertyOrFieldName is defined in
					// expression.Type or its base types.

					// Either way, it's safe to ignore. Hopefully the developer will realize their mistake
					// but lets not make life hard for them by throwing an exception from this deep in
					// their code.
				}
			}
			return count > 0 ? source.Provider.CreateQuery<T>(expression) : source;
		}
	}
}
