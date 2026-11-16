// SPDX-FileCopyrighText: 2026 Tayra Sakurai
// SPDX-License-Identifier: GPL-3.0-or-later

using Cedar.Contexts;
using Cedar.Messages;
using Cedar.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cedar.ViewModels
{
    [ObservableRecipient]
    public partial class CategoryViewModel : ObservableValidator, IRecipient<SmallestCategoryUpdatedMessage>
    {
        private readonly IDbContextFactory<CedarContext> factory;
        private LargerCategory largerCategory;

        public CategoryViewModel(IDbContextFactory<CedarContext> factory)
        {
            Messenger = WeakReferenceMessenger.Default;
            this.factory = factory;
            largerCategory = new();

            ErrorsChanged += CategoryViewModel_ErrorsChanged;
        }

        private void CategoryViewModel_ErrorsChanged(object? sender, System.ComponentModel.DataErrorsChangedEventArgs e)
        {
            SaveCommand.NotifyCanExecuteChanged();
        }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(InvokeSmallestCategoryCommand))]
        public partial SmallestCategory? SmallestCategory { get; set; }

        public ObservableCollection<LargerCategory?> LargerCategories { get; } = [];
        public ObservableCollection<SmallestCategory> SmallestCategories { get; } = [];

        [RelayCommand(AllowConcurrentExecutions = false)]
        public async Task LoadAsync()
        {
            using CedarContext context = await factory.CreateDbContextAsync();

            LargerCategories.Clear();
            LargerCategories.Add(null);

            IAsyncEnumerable<LargerCategory> largerCategories = context.LargerCategories
                .Where(lc => lc.IsIncome == largerCategory.IsIncome)
                .Where(lc => lc.Id != largerCategory.Id)
                .OrderBy(lc => lc.ParentId)
                .ThenBy(lc => lc.Id)
                .AsNoTracking()
                .AsAsyncEnumerable();

            await foreach (LargerCategory largerCategory1 in largerCategories)
                LargerCategories.Add(largerCategory1);
        }

        public async Task InitializeExistingValueAsync(LargerCategory largerCategory)
        {
            using CedarContext context = await factory.CreateDbContextAsync();

            context.Attach(largerCategory);

            Stack<LargerCategory> lcStack = new();
            lcStack.Push(largerCategory);

            HashSet<int> visitedIds = [largerCategory.Id];

            while (lcStack.Count > 0)
            {
                LargerCategory lc = lcStack.Pop();

                if (visitedIds.Contains(lc.Id))
                    throw new InvalidOperationException("A loop dependency has been detected.");

                visitedIds.Add(lc.Id);

                EntityEntry<LargerCategory> entityEntry = context.Entry(lc);
                await entityEntry
                    .Collection(c => c.LargerCategories)
                    .LoadAsync();
                await entityEntry
                    .Collection(c => c.SmallestCategories)
                    .LoadAsync();

                foreach (SmallestCategory smallestCategory in lc.SmallestCategories)
                    await context.Entry(smallestCategory)
                        .Collection(sc => sc.Items)
                        .LoadAsync();

                foreach (LargerCategory lc1 in lc.LargerCategories)
                    lcStack.Push(lc1);
            }

            SmallestCategories.Clear();

            foreach (SmallestCategory smallestCategory1 in largerCategory.SmallestCategories)
                SmallestCategories.Add(smallestCategory1);

            SmallestCategory = null;

            this.largerCategory = largerCategory;
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Parent));
            OnPropertyChanged(nameof(Budget));
        }

        public LargerCategory? Parent
        {
            get => LargerCategories.FirstOrDefault(lc => lc?.Id == largerCategory.ParentId);
            set
            {
                if (value?.Id != largerCategory.ParentId)
                {
                    largerCategory.ParentId = value?.Id;
                    OnPropertyChanged();
                }
            }
        }

        [Range(0d, double.MaxValue)]
        public double Budget
        {
            get => largerCategory.Budget;
            set => SetProperty(largerCategory.Budget, value, largerCategory, (m, v) => m.Budget = v, true);
        }

        [Required]
        public string Name
        {
            get => largerCategory.Name;
            set => SetProperty(largerCategory.Name, value, largerCategory, (m, v) => m.Name = v, true);
        }

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanExecuteUpdates))]
        private async Task SaveAsync()
        {
            if (HasErrors)
                return;

            using (CedarContext context = await factory.CreateDbContextAsync())
            {
                largerCategory.LargerCategories.Clear();
                largerCategory.Parent = null;
                largerCategory.SmallestCategories.Clear();

                context.Update(largerCategory);
                await context.SaveChangesAsync();
            }

            WeakReferenceMessenger.Default.Send(new LargerCategoryUpdatedMessage(largerCategory));
        }

        private bool CanExecuteUpdates()
        {
            return !HasErrors;
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task RemoveAsync()
        {
            using (CedarContext context = await factory.CreateDbContextAsync())
            {
                largerCategory.LargerCategories.Clear();
                largerCategory.Parent = null;
                largerCategory.SmallestCategories.Clear();

                context.Remove(largerCategory);
                await context.SaveChangesAsync();
            }

            WeakReferenceMessenger.Default.Send(new LargerCategoryUpdatedMessage(largerCategory));
        }

        public async void Receive(SmallestCategoryUpdatedMessage message)
        {
            largerCategory.SmallestCategories.Clear();
            SmallestCategories.Clear();

            using CedarContext context = await factory.CreateDbContextAsync();

            EntityEntry<LargerCategory> entityEntry = context.Attach(largerCategory);
            await entityEntry.Collection(e => e.SmallestCategories).LoadAsync();

            foreach (SmallestCategory smallestCategory in largerCategory.SmallestCategories)
                SmallestCategories.Add(smallestCategory);
        }

        [RelayCommand(CanExecute = nameof(CanInvoke))]
        private void InvokeSmallestCategory()
        {
            if (SmallestCategory is null)
                return;

            WeakReferenceMessenger.Default.Send(new SmallestCategoryUpdatedMessage(SmallestCategory));
        }

        private bool CanInvoke()
        {
            return SmallestCategory is not null;
        }

        [RelayCommand]
        private void AddSmallestCategory()
        {
            SmallestCategory smallest = new()
            {
                ParentId = largerCategory.Id,
                IsIncome = largerCategory.IsIncome,
            };

            WeakReferenceMessenger.Default.Send(new SmallestCategoryInvokedMessage(smallest));
        }
    }
}
