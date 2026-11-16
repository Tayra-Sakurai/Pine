// SPDX-FileCopyrighText: 2026 Tayra Sakurai
// SPDX-License-Identifier: GPL-3.0-or-later

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

// To add another templated control to this project (which already has a
// Themes\Generic.xaml), run with `--IncludeDefaultStyle false` or `-I false`:
//
//     dotnet new winui-templatedcontrol -n NewName --IncludeDefaultStyle false
//
// Then open Themes\Generic.xaml and add a <Style TargetType="local:NewName">
// block. Without it, the new control will render blank at runtime.

namespace Pine.Controls
{
    [ContentProperty(Name = "Control")]
    [TemplatePart(Name = "SuperTextBlock", Type = typeof(TextBlock))]
    public sealed partial class ValidationInteractiveControl : Control
    {
        private TextBlock? textBlock;

        public ValidationInteractiveControl()
        {
            DefaultStyleKey = typeof(ValidationInteractiveControl);
        }

        private void NotifyDataErrorInfo_ErrorsChanged(object? sender, DataErrorsChangedEventArgs e)
        {
            DisplayError();
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            textBlock = (TextBlock)GetTemplateChild("SuperTextBlock");
        }

        public IList<UIElement> Control
        {
            get => (IList<UIElement>)GetValue(ControlProperty);
            set => SetValue(ControlProperty, value);
        }

        private static readonly DependencyProperty ControlProperty = DependencyProperty.Register(
            nameof(Control),
            typeof(IList<UIElement>),
            typeof(ValidationInteractiveControl),
            null);

        public string PropertyName
        {
            get => (string)GetValue(PropertyNameProperty);
            set => SetValue(PropertyNameProperty, value);
        }

        public static DependencyProperty PropertyNameProperty { get; } = DependencyProperty.Register(
            nameof(PropertyName),
            typeof(string),
            typeof(ValidationInteractiveControl),
            new(string.Empty, OnPropertyNamePropertyChanged));

        public INotifyDataErrorInfo NotifyDataErrorInfo
        {
            get => (INotifyDataErrorInfo)GetValue(NotifyDataErrorInfoProperty);
            set => SetValue(NotifyDataErrorInfoProperty, value);
        }

        public static DependencyProperty NotifyDataErrorInfoProperty { get; } = DependencyProperty.Register(
            nameof(NotifyDataErrorInfo),
            typeof(INotifyDataErrorInfo),
            typeof(ValidationInteractiveControl),
            new(null, OnNotifyDataErrorInfoPropertyChanged));

        private static void OnNotifyDataErrorInfoPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ValidationInteractiveControl control = (ValidationInteractiveControl)d;

            if (e.OldValue is INotifyDataErrorInfo notifyDataError)
                notifyDataError.ErrorsChanged -= control.NotifyDataErrorInfo_ErrorsChanged;

            if (e.NewValue is INotifyDataErrorInfo notifyDataErrorInfo)
                notifyDataErrorInfo.ErrorsChanged += control.NotifyDataErrorInfo_ErrorsChanged;

            control.DisplayError();
        }

        private static void OnPropertyNamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ValidationInteractiveControl control = (ValidationInteractiveControl)d;

            control.DisplayError();
        }

        private void DisplayError()
        {
            if (textBlock is null ||
                string.IsNullOrEmpty(PropertyName) ||
                NotifyDataErrorInfo is null)
                return;

            ValidationResult? result = NotifyDataErrorInfo.GetErrors(PropertyName).OfType<ValidationResult>().FirstOrDefault();

            textBlock.Text = result?.ErrorMessage ?? string.Empty;
        }
    }
}
