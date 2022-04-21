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

using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ProvisionData.Specifications.Internal;

public interface IRepository<TEntity> : IDisposable
	where TEntity : class
{
	IAsyncEnumerable<TEntity> FindAsync(IQuery<TEntity> specification);

	Task<TEntity?> GetAsync(IQuery<TEntity> specification, CancellationToken cancellationToken = default);

	void Add(TEntity entity);
	void AddRange(IEnumerable<TEntity> entities);

	void Remove(TEntity entity);
	void RemoveRange(IEnumerable<TEntity> entities);

	void Update(TEntity entity);

	Task<Boolean> AnyAsync(IQuery<TEntity> specification, CancellationToken cancellationToken = default);
	Task<Boolean> AnyAsync(Expression<Func<TEntity, Boolean>> predicate, CancellationToken cancellationToken = default);

	Task<Int32> CountAsync(IQuery<TEntity> specification, CancellationToken cancellationToken = default);
	Task<Int32> CountAsync(Expression<Func<TEntity, Boolean>> predicate, CancellationToken cancellationToken = default);
}
