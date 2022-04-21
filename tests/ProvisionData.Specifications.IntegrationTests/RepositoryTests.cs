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

using FluentAssertions;
using ProvisionData.Specifications.Internal;
using System.Threading.Tasks;
using Xunit;

namespace ProvisionData.Specifications.IntegrationTests;

public abstract class RepositoryTests : IDisposable
{
	private Boolean _disposed;

	public abstract IRepository<Person> GetRepository();

	[Fact]
	public async Task And()
	{
		using (var repo = GetRepository())
		{
			var spec = new UserIsAgeOfMajority(18).And(new UserHasGender(Gender.Male));

			Int32 count = await FetchAsync(repo, spec);
			count.Should().Be(1);
		}
	}

	[Fact]
	public async Task Not()
	{
		using (var repo = GetRepository())
		{
			var spec = !new UserIsAgeOfMajority(18);

			Int32 count = await FetchAsync(repo, spec);
			count.Should().Be(3);
		}
	}

	[Fact]
	public async Task Or()
	{
		using (var repo = GetRepository())
		{
			var spec = new UserIsAgeOfMajority(18).Or(new UserHasGender(Gender.Male));

			Int32 count = await FetchAsync(repo, spec);
			count.Should().Be(4);
		}
	}

	[Fact]
	public async Task UserIsAgeOfMajority()
	{
		using (var repo = GetRepository())
		{
			var spec = new UserIsAgeOfMajority(18);

			Int32 count = await FetchAsync(repo, spec);
			count.Should().Be(2);
		}
	}

	[Fact]
	public async Task UserHasGender()
	{
		using (var repo = GetRepository())
		{
			var spec = new UserHasGender(Gender.Female);

			Int32 count = await FetchAsync(repo, spec);
			count.Should().Be(2);
		}
	}

	private static async Task<Int32> FetchAsync(IRepository<Person> repo, Query<Person> spec)
	{
		var count = 0;
		await foreach (var user in repo.FindAsync(spec))
			count++;
		return count;
	}

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
	// ~RepositoryTests()
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
