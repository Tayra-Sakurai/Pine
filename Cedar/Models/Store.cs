// SPDX-FileCopyrighText: 2026 Tayra Sakurai
// SPDX-License-Identifier: GPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Text;

namespace Cedar.Models
{
    public class Store
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public HashSet<Item> Items { get; } = [];
    }
}
