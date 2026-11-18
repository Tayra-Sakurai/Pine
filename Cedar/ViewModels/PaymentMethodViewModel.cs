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
    public partial class PaymentMethodViewModel : ObservableValidator, IRecipient<PaymentMethodInvokedMessage>
    {
        private readonly IDbContextFactory<CedarContext> factory;
        private PaymentMethod paymentMethod;

        public PaymentMethodViewModel(IDbContextFactory<CedarContext> factory)
        {
            Messenger = WeakReferenceMessenger.Default;
            this.factory = factory;

            paymentMethod = new();
            ErrorsChanged += PaymentMethodViewModel_ErrorsChanged;
        }

        private void PaymentMethodViewModel_ErrorsChanged(object? sender, System.ComponentModel.DataErrorsChangedEventArgs e)
        {
            SaveCommand.NotifyCanExecuteChanged();
        }

        public void InitializeForExistingValue(PaymentMethod paymentMethod)
        {
            this.paymentMethod = paymentMethod;

            OnPropertyChanged(nameof(Name));
            ValidateAllProperties();
        }

        [Required]
        public string Name
        {
            get => paymentMethod.Name;
            set => SetProperty(paymentMethod.Name, value, paymentMethod, (m, v) => m.Name = v, true);
        }

        public void Receive(PaymentMethodInvokedMessage message)
        {
            InitializeForExistingValue(message.Value);
        }

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanSave))]
        private async Task SaveAsync()
        {
            using (CedarContext context = await factory.CreateDbContextAsync())
            {
                paymentMethod.Items.Clear();

                context.Update(paymentMethod);
                await context.SaveChangesAsync();
            }

            WeakReferenceMessenger.Default.Send(new PaymentMethodUpdatedMessage(paymentMethod));
            paymentMethod = new();

            OnPropertyChanged(nameof(Name));
            ValidateAllProperties();
        }

        private bool CanSave()
        {
            return !HasErrors;
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task RemoveAsync()
        {
            using (CedarContext context = await factory.CreateDbContextAsync())
            {
                paymentMethod.Items.Clear();

                context.Remove(paymentMethod);
                await context.SaveChangesAsync();
            }

            WeakReferenceMessenger.Default.Send(new PaymentMethodUpdatedMessage(paymentMethod));

            paymentMethod = new();
            OnPropertyChanged(nameof(Name));
            ValidateAllProperties();
        }
    }
}
