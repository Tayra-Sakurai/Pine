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
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cedar.ViewModels
{
    [ObservableRecipient]
    public partial class SmallestCategoryViewModel : ObservableValidator, IRecipient<SmallestCategoryInvokedMessage>
    {
        private readonly IDbContextFactory<CedarContext> factory;

        public SmallestCategoryViewModel(IDbContextFactory<CedarContext> factory)
        {
            Messenger = WeakReferenceMessenger.Default;

            smallestCategory = new();

            this.factory = factory;

            ErrorsChanged += SmallestCategoryViewModel_ErrorsChanged;
        }

        private void SmallestCategoryViewModel_ErrorsChanged(object? sender, System.ComponentModel.DataErrorsChangedEventArgs e)
        {
            SaveCommand.NotifyCanExecuteChanged();
        }

        private SmallestCategory smallestCategory;

        [Required]
        public string Name
        {
            get => smallestCategory.Name;
            set => SetProperty(smallestCategory.Name, value, smallestCategory, (m, v) => m.Name = v, true);
        }

        [Range(0d, double.MaxValue)]
        public double Budget
        {
            get => smallestCategory.Budget;
            set => SetProperty(smallestCategory.Budget, value, smallestCategory, (m, v) => m.Budget = v, true);
        }

        public void InitializeForExistingValue(SmallestCategory smallestCategory)
        {
            this.smallestCategory = smallestCategory;

            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Budget));
        }

        public void Receive(SmallestCategoryInvokedMessage message)
        {
            InitializeForExistingValue(message.Value);
        }

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanSave))]
        private async Task SaveAsync()
        {
            if (HasErrors)
                return;

            smallestCategory.Parent = null;
            int parentId = smallestCategory.ParentId;
            smallestCategory.Items.Clear();
            bool isIncome = smallestCategory.IsIncome;

            using (CedarContext context = await factory.CreateDbContextAsync())
            {
                context.Update(smallestCategory);

                await context.SaveChangesAsync();
            }

            WeakReferenceMessenger.Default.Send(new SmallestCategoryUpdatedMessage(smallestCategory));

            smallestCategory = new()
            {
                IsIncome = isIncome,
                ParentId = parentId,
            };

            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Budget));
        }

        private bool CanSave()
        {
            return !HasErrors;
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task RemoveAsync()
        {
            smallestCategory.Parent = null;
            smallestCategory.Items.Clear();

            int parentId = smallestCategory.ParentId;
            bool isIncome = smallestCategory.IsIncome;

            using (CedarContext context = await factory.CreateDbContextAsync())
            {
                context.Remove(smallestCategory);

                await context.SaveChangesAsync();
            }

            WeakReferenceMessenger.Default.Send(new SmallestCategoryUpdatedMessage(smallestCategory));

            smallestCategory = new()
            {
                IsIncome = isIncome,
                ParentId = parentId,
            };

            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Budget));
        }
    }
}
