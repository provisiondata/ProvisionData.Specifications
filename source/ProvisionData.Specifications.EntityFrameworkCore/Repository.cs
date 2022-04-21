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
using ProvisionData.Specifications.Internal;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ProvisionData.Specifications.EntityFrameworkCore;

public class Repository<TEntity> : IRepository<TEntity>, IDisposable
	where TEntity : class
{
	private readonly DbContext _context;
	private Boolean _disposed;

	public Repository(DbContext context)
	{
		_context = context ?? throw new ArgumentNullException(nameof(context));
	}

	public IAsyncEnumerable<TEntity> FindAsync(IQuery<TEntity> specification)
		=> ApplySpecification(specification).AsAsyncEnumerable();

	public async Task<TEntity?> GetAsync(IQuery<TEntity> specification, CancellationToken cancellationToken = default)
		=> await ApplySpecification(specification).SingleOrDefaultAsync(cancellationToken);

	public async Task<Boolean> AnyAsync(IQuery<TEntity> specification, CancellationToken cancellationToken = default)
		=> await ApplySpecification(specification).AnyAsync(cancellationToken);

	public async Task<Boolean> AnyAsync(Expression<Func<TEntity, Boolean>> predicate, CancellationToken cancellationToken = default)
		=> await _context.Set<TEntity>().AnyAsync(predicate, cancellationToken);

	public async Task<Int32> CountAsync(IQuery<TEntity> specification, CancellationToken cancellationToken = default)
		=> await ApplySpecification(specification).CountAsync(cancellationToken);

	public async Task<Int32> CountAsync(Expression<Func<TEntity, Boolean>> predicate, CancellationToken cancellationToken = default)
		=> await _context.Set<TEntity>().CountAsync(predicate, cancellationToken);

	public void Add(TEntity entity) => _context.Set<TEntity>().Add(entity);

	public void AddRange(IEnumerable<TEntity> entities) => _context.Set<TEntity>().AddRange(entities);

	public void Remove(TEntity entity) => _context.Set<TEntity>().Remove(entity);

	public void RemoveRange(IEnumerable<TEntity> entities) => _context.Set<TEntity>().RemoveRange(entities);

	public void Update(TEntity entity)
	{
		_context.Set<TEntity>().Attach(entity);
		_context.Entry(entity).State = EntityState.Modified;
	}

	private IQueryable<TEntity> ApplySpecification(IQuery<TEntity> spec) => SpecificationEvaluator<TEntity>.GetQuery(_context.Set<TEntity>().AsQueryable(), spec);

	protected virtual void Dispose(Boolean disposing)
	{
		if (!_disposed)
		{
			if (disposing)
			{
				// TODO: dispose managed state (managed objects)
			}

			// TODO: free unmanaged resources (unmanaged objects) and override finalizer
			// TODO: set large fields to null
			_disposed = true;
		}
	}

	// // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
	// ~DbContextRepository()
	// {
	//     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
	//     Dispose(disposing: false);
	// }

	public void Dispose()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
