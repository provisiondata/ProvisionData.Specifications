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

using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ProvisionData.Specifications;

public class SpecificationEvaluator<TEntity> where TEntity : class
{
	public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> query, IQuery<TEntity> spec)
	{
		if (spec.Predicate != null)
		{
			query = query.Where(spec.Predicate);
		}

		query = spec.Includes.Aggregate(query, (c, i) => c.Include(i));

		if (spec.OrderBy != null)
		{
			query = query.OrderBy(spec.OrderBy);
		}
		else if (spec.OrderByDescending != null)
		{
			query = query.OrderByDescending(spec.OrderByDescending);
		}

		if (spec.GroupBy != null)
		{
			query = query.GroupBy(spec.GroupBy).SelectMany(x => x);
		}

		// Apply paging if enabled
		if (spec.IsPagingEnabled)
		{
			query = query.Skip(spec.Skip)
						 .Take(spec.Take);
		}

		return query;
	}
}