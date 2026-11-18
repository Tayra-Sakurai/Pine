// SPDX-FileCopyrighText: 2026 Tayra Sakurai
// SPDX-License-Identifier: GPL-3.0-or-later

using Cedar.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cedar.Contexts
{
    public partial class CedarContext : DbContext
    {
        public DbSet<Item> Items { get; set; }
        public DbSet<LargerCategory> LargerCategories { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<SmallestCategory> SmallestCategories { get; set; }
        public DbSet<Store> Stores { get; set; }

        public CedarContext(DbContextOptions<CedarContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>().OwnsMany(
                item => item.Details);
            modelBuilder.Entity<LargerCategory>()
                .HasBaseType<Category>();
            modelBuilder.Entity<PaymentMethod>();
            modelBuilder.Entity<SmallestCategory>()
                .HasBaseType<Category>();
            modelBuilder.Entity<Store>()
                .HasIndex(s => s.Name)
                .IsUnique();
        }
    }
}
