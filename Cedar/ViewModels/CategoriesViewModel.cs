// SPDX-FileCopyrighText: 2026 Tayra Sakurai
// SPDX-License-Identifier: GPL-3.0-or-later

using Cedar.Contexts;
using Cedar.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Cedar.ViewModels
{
    public partial class CategoriesViewModel : ObservableObject
    {
        private readonly IDbContextFactory<CedarContext> factory;

        public CategoriesViewModel(IDbContextFactory<CedarContext> factory)
        {
            this.factory = factory;
            ExpenseCategories = [];
            IncomeCategories = [];
        }

        public ObservableCollection<LargerCategory> ExpenseCategories { get; }
        public ObservableCollection<LargerCategory> IncomeCategories { get; }

        [ObservableProperty]
        public partial Category? SelectedCategory { get; set; }

        [RelayCommand(AllowConcurrentExecutions = false)]
        public async Task LoadAsync()
        {
            using CedarContext context = await factory.CreateDbContextAsync();

            ExpenseCategories.Clear();
            IncomeCategories.Clear();

            IAsyncEnumerable<LargerCategory> expenseCategories = context.LargerCategories
                .Where(lc => !lc.IsIncome)
                .Include(lc => lc.Parent)
                .OrderBy(lc => lc.Name)
                .ThenBy(lc => lc.Id)
                .AsNoTracking()
                .AsAsyncEnumerable();

            await foreach (
                LargerCategory expenseCategory in expenseCategories)
                ExpenseCategories.Add(expenseCategory);

            IAsyncEnumerable<LargerCategory> incomeCategories = context.LargerCategories
                .Where(lc => lc.IsIncome)
                .Include(lc => lc.Parent)
                .OrderBy(lc => lc.Name)
                .ThenBy(lc => lc.Id)
                .AsNoTracking()
                .AsAsyncEnumerable();

            await foreach (
                LargerCategory incomeCategory in incomeCategories)
                IncomeCategories.Add(incomeCategory);

            SelectedCategory = null;
        }
    }
}
