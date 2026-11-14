// SPDX-FileCopyrighText: 2026 Tayra Sakurai
// SPDX-License-Identifier: GPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Text;

namespace Cedar.Models
{
    public class SmallestCategory : Category
    {
        public int ParentId { get; set; }
        public LargerCategory? Parent { get; set; }
        public HashSet<Item> Items { get; } = [];
        public double TotalAmountMonthly => Items.Where(item => item.DateTime.Month == DateTime.Now.Month).Sum(item => Math.Abs(item.Amount));
        public double TotalAmount => Items.Sum(item => Math.Abs(item.Amount));
    }
}
