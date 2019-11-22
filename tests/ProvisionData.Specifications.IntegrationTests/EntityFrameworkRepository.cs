﻿/*******************************************************************************
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;

    public sealed class EntityFrameworkRepository : IRepository<User>
    {
        private readonly SqlConnection _connection = new SqlConnection("Server=.;Database=SpecificationsTest;Integrated Security=true;MultipleActiveResultSets=True;");
        private readonly DbContextOptions<EntityFrameworkContext> _options;

        public EntityFrameworkRepository()
        {
            _connection.Open();

            _options = new DbContextOptionsBuilder<EntityFrameworkContext>()
                    .UseSqlServer(_connection)
                    .Options;

            using (var context = new EntityFrameworkContext(_options))
            {
                context.Database.EnsureCreated();
            }
        }

        public async Task<IReadOnlyList<User>> ListAsync(IQueryableSpecification<User> specification = null)
        {
            using (var ctx = new EntityFrameworkContext(_options))
            {
                var query = ctx.Users as IQueryable<User>;
                if (specification != null)
                {
                    query = query.Where(specification.Predicate);
                }
                return await query.ToListAsync().ConfigureAwait(false);
            }
        }

        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
        }
    }
}
