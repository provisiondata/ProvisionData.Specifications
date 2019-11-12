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

namespace ProvisionData.Specifications.Internal
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public sealed class InMemoryRepository : IRepository<User>
    {
        private readonly IDictionary<Guid, User> _users = new ConcurrentDictionary<Guid, User>();

        public InMemoryRepository()
        {
            AddInternal(Guid.NewGuid(), "Doug", new DateTime(1974, 10, 16), Gender.Male);
            AddInternal(Guid.NewGuid(), "Charmaine", new DateTime(1975, 11, 11), Gender.Female);
            AddInternal(Guid.NewGuid(), "Shyloh", new DateTime(2007, 10, 15), Gender.Male);
            AddInternal(Guid.NewGuid(), "Piper", new DateTime(2008, 5, 19), Gender.Female);
            AddInternal(Guid.NewGuid(), "Geordi", new DateTime(2011, 9, 14), Gender.Male);
        }

        public Task AddAsync(User domainModel)
        {
            if (domainModel == null)
                throw new ArgumentNullException(nameof(domainModel));

            _users.Add(domainModel.Id, domainModel);

            return Task.CompletedTask;
        }

        public Task DeleteAsync(User domainModel)
        {
            _users.Remove(domainModel.Id);
            return Task.CompletedTask;
        }

        public void Dispose() { }

        public Task<User> GetAsync(Guid id) => Task.FromResult(_users[id]);

        public Task<IReadOnlyList<User>> ListAsync(ISpecification<User> specification = null)
        {
            if (specification is null)
            {
                IReadOnlyList<User> v = _users.Values.ToList();
                return Task.FromResult(v);
            }

            IReadOnlyList<User> result = _users.Values.Where(x => specification.IsSatisfiedBy(x)).ToList();
            return Task.FromResult(result);
        }

        public Task UpdateAsync(User domainModel)
        {
            lock (_users)
            {
                return Task.Run(() => DeleteAsync(domainModel))
                           .ContinueWith(_ => AddAsync(domainModel), TaskScheduler.Default);
            }
        }

        private void AddInternal(Guid uid, String name, DateTime dateOfBirth, Gender gender)
            => _users.Add(uid, new User(uid, name, dateOfBirth, gender));
    }
}
