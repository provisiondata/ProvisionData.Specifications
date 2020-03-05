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

namespace ProvisionData.Specifications.IntegrationTests
{
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using ProvisionData.Specifications.Internal;
    using Xunit;

    public abstract class RepositoryTests
    {
        public abstract IRepository<User> GetRepository();

        [Fact]
        public async Task All()
        {
            using (var repo = GetRepository())
            {
                (await repo.ListAsync().ConfigureAwait(false)).Count().Should().Be(5);
            }
        }

        [Fact]
        public async Task And()
        {
            using (var repo = GetRepository())
            {
                var spec = new UserIsAgeOfMajority(18).And(new UserHasGender(Gender.Male));

                (await repo.ListAsync(spec).ConfigureAwait(false)).Count().Should().Be(1);
            }
        }

        [Fact]
        public async Task Not()
        {
            using (var repo = GetRepository())
            {
                var spec = !new UserIsAgeOfMajority(18);

                (await repo.ListAsync(spec).ConfigureAwait(false)).Count().Should().Be(3);
            }
        }

        [Fact]
        public async Task Or()
        {
            using (var repo = GetRepository())
            {
                var spec = new UserIsAgeOfMajority(18).Or(new UserHasGender(Gender.Male));

                (await repo.ListAsync(spec).ConfigureAwait(false)).Count().Should().Be(4);
            }
        }

        [Fact]
        public async Task UserIsAgeOfMajority()
        {
            using (var repo = GetRepository())
            {
                (await repo.ListAsync(new UserIsAgeOfMajority(18)).ConfigureAwait(false)).Count().Should().Be(2);
            }
        }

        [Fact]
        public async Task UserHasGender()
        {
            using (var repo = GetRepository())
            {
                (await repo.ListAsync(new UserHasGender(Gender.Female)).ConfigureAwait(false)).Count().Should().Be(2);
            }
        }
    }
}
