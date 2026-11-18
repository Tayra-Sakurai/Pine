// SPDX-FileCopyrighText: 2026 Tayra Sakurai
// SPDX-License-Identifier: GPL-3.0-or-later

using Cedar.Contexts;
using Cedar.Messages;
using Cedar.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Cedar.ViewModels
{
    public partial class ItemsViewModel : ObservableObject
    {
        private readonly IDbContextFactory<CedarContext> factory;

        public ItemsViewModel(IDbContextFactory<CedarContext> factory)
        {
            this.factory = factory;
        }

        public ObservableCollection<Item> Items { get; } = [];

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(InvokeItemCommand))]
        public partial Item? SelectedItem { get; set; }

        [RelayCommand(AllowConcurrentExecutions = false)]
        public async Task LoadAsync()
        {
            using CedarContext context = await factory.CreateDbContextAsync();

            Items.Clear();

            await foreach (
                Item item in
                context.Items
                .Include(i => i.SmallestCategory)
                .Include(i => i.PaymentMethod)
                .Include(i => i.Store)
                .OrderByDescending(i => i.DateTime)
                .ThenBy(i => i.Id)
                .AsNoTracking()
                .AsAsyncEnumerable())
                Items.Add(item);

            SelectedItem = null;
        }

        [RelayCommand(CanExecute = nameof(CanInvoke))]
        private void InvokeItem()
        {
            if (SelectedItem == null)
                return;

            WeakReferenceMessenger.Default.Send(new ItemInvokedMessage(SelectedItem));
        }

        private bool CanInvoke()
        {
            return SelectedItem is not null;
        }

        /// <summary>
        /// Asynchronously applies the specified filter query.
        /// </summary>
        /// <param name="query">The filter query.</param>
        /// <returns>A <see cref="Task"/> instance that represents the asynchronous filter operation.</returns>
        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanFilter))]
        protected virtual async Task FilterAsync(string? query)
        {
            if (string.IsNullOrWhiteSpace(query)) return;

            using CedarContext context = await factory.CreateDbContextAsync();

            Items.Clear();

            string[] splittedQuery = query.Split(' ');

            bool FilterFunc(Item item) => splittedQuery.Any(ph => item.Name.Contains(ph)) || splittedQuery.Any(ph => item.SmallestCategory?.Name.Contains(ph) ?? false) || splittedQuery.Any(ph => item.PaymentMethod?.Name.Contains(ph) ?? false) || splittedQuery.Any(ph => item.Details.Any(dtl => dtl.Name.Contains(ph)));

            List<Item> items = await context.Items
                .Include(i => i.SmallestCategory)
                .Include(i => i.PaymentMethod)
                .Include(i => i.Store)
                .OrderByDescending(i => i.DateTime)
                .ThenBy(i => i.Id)
                .AsNoTracking()
                .ToListAsync();

            foreach (
                Item item in
                items
                .Where(FilterFunc))
                Items.Add(item);

            SelectedItem = null;
        }

        private bool CanFilter(string? query) => !string.IsNullOrWhiteSpace(query);
    }
}
