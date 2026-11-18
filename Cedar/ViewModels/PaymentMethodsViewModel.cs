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
using System.Collections.ObjectModel;
using System.Text;

namespace Cedar.ViewModels
{
    public partial class PaymentMethodsViewModel : ObservableRecipient, IRecipient<PaymentMethodUpdatedMessage>
    {
        private readonly IDbContextFactory<CedarContext> factory;

        public PaymentMethodsViewModel(IDbContextFactory<CedarContext> factory)
            : base()
        {
            this.factory = factory;
        }

        public PaymentMethod? SelectedMethod
        {
            get => field;
            set
            {
                if (SetProperty(ref field, value) && value is not null)
                    WeakReferenceMessenger.Default.Send(new PaymentMethodInvokedMessage(value));
            }
        }

        public ObservableCollection<PaymentMethod> PaymentMethods { get; } = [];

        [RelayCommand(AllowConcurrentExecutions = false)]
        public async Task LoadAsync()
        {
            using CedarContext context = await factory.CreateDbContextAsync();

            PaymentMethods.Clear();

            await foreach (
                PaymentMethod paymentMethod in
                context.PaymentMethods
                .Include(pm => pm.Items)
                .OrderBy(pm => pm.Name)
                .ThenBy(pm => pm.Id)
                .AsNoTracking()
                .AsAsyncEnumerable())
                PaymentMethods.Add(paymentMethod);

            SelectedMethod = new();
        }

        public async void Receive(PaymentMethodUpdatedMessage message)
        {
            await LoadAsync();
        }

        [RelayCommand]
        private static void Add()
        {
            PaymentMethod paymentMethod = new();

            WeakReferenceMessenger.Default.Send(new PaymentMethodInvokedMessage(paymentMethod));
        }
    }
}
