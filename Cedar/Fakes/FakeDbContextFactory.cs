// SPDX-FileCopyrighText: 2026 Tayra Sakurai
// SPDX-License-Identifier: GPL-3.0-or-later

using Cedar.Contexts;
using Cedar.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cedar.Fakes
{
    public partial class FakeDbContextFactory : IDesignTimeDbContextFactory<CedarContext>
    {
        public CedarContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<CedarContext> optionsBuilder = new();

            optionsBuilder.UseSqlite("Data Source=Database.sqlite3");
            optionsBuilder.AddInterceptors(new CedarContextInterceptor());

            return new(optionsBuilder.Options);
        }
    }
}
