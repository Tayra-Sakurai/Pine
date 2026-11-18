// SPDX-FileCopyrighText: 2026 Tayra Sakurai
// SPDX-License-Identifier: GPL-3.0-or-later

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cedar.Models
{
    public class ItemDetail
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Quantity { get; set; } = 0d;
        public int ItemId { get; set; }
    }
}
