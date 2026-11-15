// SPDX-FileCopyrighText: 2026 Tayra Sakurai
// SPDX-License-Identifier: GPL-3.0-or-later

using Cedar.Contexts;
using Cedar.Models;
using CommunityToolkit.Mvvm.ComponentModel;
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
    }
}
