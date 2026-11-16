// SPDX-FileCopyrighText: 2026 Tayra Sakurai
// SPDX-License-Identifier: GPL-3.0-or-later

using Cedar.Models;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cedar.Messages
{
    public class LargerCategoryUpdatedMessage : ValueChangedMessage<LargerCategory>
    {
        public LargerCategoryUpdatedMessage(LargerCategory value)
            : base(value)
        { }
    }
}
