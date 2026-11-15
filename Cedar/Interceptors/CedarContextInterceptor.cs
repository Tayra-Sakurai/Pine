// SPDX-FileCopyrighText: 2026 Tayra Sakurai
// SPDX-License-Identifier: GPL-3.0-or-later

using Cedar.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cedar.Interceptors
{
    public partial class CedarContextInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            DbContext? context = eventData.Context;

            if (context == null)
                return base.SavingChanges(eventData, result);

            IEnumerable<EntityEntry<LargerCategory>> entries = context.ChangeTracker.Entries<LargerCategory>()
                .Where(e => e.State == EntityState.Modified || e.State == EntityState.Added);

            foreach (EntityEntry<LargerCategory> entry in entries)
            {
                int lcId = entry.Entity.Id;
                int? parentId = entry.Entity.ParentId;

                if (parentId.HasValue && parentId.Value == lcId)
                    throw new InvalidOperationException("A cyclic relation has been detected.");

                int? nextParentId = parentId;
                HashSet<int> visitedIds = [lcId];

                while (nextParentId.HasValue)
                {
                    if (visitedIds.Contains(nextParentId.Value))
                        throw new InvalidOperationException("A cyclic relation has been detected.");

                    visitedIds.Add(nextParentId.Value);

                    EntityEntry<LargerCategory>? parentValue = context.ChangeTracker.Entries<LargerCategory>()
                        .FirstOrDefault(e => e.Entity.Id == nextParentId);

                    if (parentValue == null)
                        nextParentId = context.Set<LargerCategory>()
                            .Where(e => e.Id == nextParentId.Value)
                            .Select(e => e.Id)
                            .FirstOrDefault();
                    else
                        nextParentId = parentValue.Entity.ParentId;
                }
            }

            return base.SavingChanges(eventData, result);
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            DbContext? context = eventData.Context;

            if (context == null)
                return await base.SavingChangesAsync(eventData, result);

            IEnumerable<EntityEntry<LargerCategory>> entries = context.ChangeTracker.Entries<LargerCategory>()
                .Where(e => e.State == EntityState.Modified || e.State == EntityState.Added);

            foreach (EntityEntry<LargerCategory> entry in entries)
            {
                int lcId = entry.Entity.Id;
                int? parentId = entry.Entity.ParentId;

                if (parentId.HasValue && parentId.Value == lcId)
                    throw new InvalidOperationException("A cyclic relation has been detected.");

                int? nextParentId = parentId;
                HashSet<int> visitedIds = [lcId];

                while (nextParentId.HasValue)
                {
                    if (visitedIds.Contains(nextParentId.Value))
                        throw new InvalidOperationException("A cyclic relation has been detected.");

                    visitedIds.Add(nextParentId.Value);

                    EntityEntry<LargerCategory>? parentValue = context.ChangeTracker.Entries<LargerCategory>()
                        .FirstOrDefault(e => e.Entity.Id == nextParentId);

                    if (parentValue == null)
                        nextParentId = await context.Set<LargerCategory>()
                            .Where(e => e.Id == nextParentId.Value)
                            .Select(e => e.Id)
                            .FirstOrDefaultAsync();
                    else
                        nextParentId = parentValue.Entity.ParentId;
                }
            }

            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
