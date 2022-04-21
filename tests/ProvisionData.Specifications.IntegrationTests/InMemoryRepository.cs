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

using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ProvisionData.Specifications.Internal;

public sealed class InMemoryRepository : IRepository<Person>
{
	private readonly ConcurrentDictionary<Guid, Person> _users = new();

	public InMemoryRepository()
	{
		AddInternal(Guid.NewGuid(), "Doug", new DateTime(1974, 10, 16), Gender.Male);
		AddInternal(Guid.NewGuid(), "Charmaine", new DateTime(1975, 11, 11), Gender.Female);
		AddInternal(Guid.NewGuid(), "Shyloh", new DateTime(2007, 10, 15), Gender.Male);
		AddInternal(Guid.NewGuid(), "Piper", new DateTime(2008, 5, 19), Gender.Female);
		AddInternal(Guid.NewGuid(), "Geordi", new DateTime(2011, 9, 14), Gender.Male);
	}

	public IEnumerable<Person> Users => _users.Values;

	public void Add(Person entity)
	{
		if (entity is null)
			throw new ArgumentNullException(nameof(entity));

		_users.TryAdd(entity.Id, entity);
	}

	public void AddRange(IEnumerable<Person> entities) => throw new NotImplementedException();

	public Task<Boolean> AnyAsync(IQuery<Person> specification = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public Task<Boolean> AnyAsync(Expression<Func<Person, Boolean>> predicate, CancellationToken cancellationToken = default) => throw new NotImplementedException();

	public Task<Int32> CountAsync(IQuery<Person> specification = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public Task<Int32> CountAsync(Expression<Func<Person, Boolean>> predicate, CancellationToken cancellationToken = default) => throw new NotImplementedException();

	public async IAsyncEnumerable<Person> FindAsync(IQuery<Person> specification)
	{
		foreach (var u in _users.Values.Where(x => specification.IsSatisfiedBy(x)))
		{
			await Task.Delay(1);
			yield return u;
		}
	}

	public Task<Person> GetAsync(Guid id) => Task.FromResult(_users[id]);

	public Task<Person> GetAsync(IQuery<Person> specification)
	{

		var result = _users.Values.Where(x => specification.IsSatisfiedBy(x)).FirstOrDefault();
		return Task.FromResult(result);
	}

	public Task<Person> GetAsync(IQuery<Person> specification, CancellationToken cancellationToken = default) => throw new NotImplementedException();

	public Task<IReadOnlyCollection<Person>> QueryAsync(IQuery<Person> specification = null)
	{
		if (specification is null)
		{
			IReadOnlyCollection<Person> v = _users.Values.ToList();
			return Task.FromResult(v);
		}

		IReadOnlyCollection<Person> result = _users.Values.Where(x => specification.IsSatisfiedBy(x)).ToList();
		return Task.FromResult(result);
	}

	public void Remove(Person entity) => _users.Remove(entity.Id, out _);

	public void RemoveRange(IEnumerable<Person> entities) => throw new NotImplementedException();

	public void Update(Person entity)
	{
		_users.AddOrUpdate(entity.Id, entity, (g, o) => entity);
	}

	private void AddInternal(Guid uid, String name, DateTime dateOfBirth, Gender gender)
		=> Add(new Person(uid, name, dateOfBirth, gender));

	public void Dispose() { }
}
