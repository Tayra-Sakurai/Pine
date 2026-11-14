// SPDX-FileCopyrighText: 2026 Tayra Sakurai
// SPDX-License-Identifier: GPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Text;

namespace Cedar.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime DateTime { get; set; } = DateTime.Now;
        public string Description { get; set; } = string.Empty;
        public int SmallestCategoryId { get; set; }
        public SmallestCategory? SmallestCategory { get; set; }
        public double Amount { get; set; } = 0d;
        public int PaymentMethodId { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
    }
}
