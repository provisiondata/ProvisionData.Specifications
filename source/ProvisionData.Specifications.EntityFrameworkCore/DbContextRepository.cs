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

namespace ProvisionData.Specifications.EntityFrameworkCore
{
	using Microsoft.EntityFrameworkCore;
	using ProvisionData.Specifications;
	using ProvisionData.Specifications.Internal;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	public class DbContextRepository<TEntity, TContext> : IReadOnlyRepository<TEntity>
		where TEntity : class
		where TContext : DbContext
	{
		private readonly TContext _dbContext;
		private Boolean _disposed;

		public DbContextRepository(TContext dbContext)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
		}

		public async Task<IReadOnlyCollection<TEntity>> QueryAsync(IQuerySpecification<TEntity> specification)
		{
			var query = _dbContext.Set<TEntity>().AsNoTracking();
			if (specification != null)
			{
				query = query.Where(specification.Predicate);
			}

			if (specification is IPagedSpecification<TEntity> paged)
			{
				if (paged.Skip >= 0)
				{
					query = query.Skip(paged.Skip.Value);
				}

				if (paged.Take > 0)
				{
					query = query.Take(paged.Take.Value);
				}

				if (paged.SortOrder.Count > 0)
				{
					query = query.SortBy(paged.SortOrder);
				}
			}

			return await query.ToListAsync();
		}

		public async Task<TEntity> GetAsync(IQuerySpecification<TEntity> specification)
		{
			var query = _dbContext.Set<TEntity>() as IQueryable<TEntity>;
			if (specification != null)
			{
				query = query.Where(specification.Predicate);
			}
			return await query.SingleOrDefaultAsync();
		}

		protected virtual void Dispose(Boolean disposing)
		{
			if (!_disposed)
			{
				//if (disposing)
				//{
				//}

				_disposed = true;
			}
		}

		public void Dispose() => Dispose(true);
	}
}
