// SPDX-FileCopyrighText: 2026 Tayra Sakurai
// SPDX-License-Identifier: GPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Cedar.Models
{
    public class LargerCategory : Category
    {
        public ObservableCollection<Category> Children { get; } = [];
        public int? ParentId { get; set; } = null;
        public LargerCategory? Parent { get; set; }
        public double Total => Children.OfType<SmallestCategory>().Sum(sc => sc.TotalAmountMonthly) + Children.OfType<LargerCategory>().Sum(lc => lc.Total);
    }
}
