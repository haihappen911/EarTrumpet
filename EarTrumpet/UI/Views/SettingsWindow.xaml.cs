﻿using EarTrumpet.DataModel;
using EarTrumpet.Extensions;
using EarTrumpet.Interop.Helpers;
using EarTrumpet.UI.Helpers;
using EarTrumpet.UI.ViewModels;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

namespace EarTrumpet.UI.Views
{
    public partial class SettingsWindow : Window
    {
        private bool _isClosing;

        public SettingsWindow()
        {
            Trace.WriteLine("SettingsWindow .ctor");

            InitializeComponent();

            SourceInitialized += SettingsWindow_SourceInitialized;

            this.FlowDirection = SystemSettings.IsRTL ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

            DataContextChanged += SettingsWindow_DataContextChanged;
        }

        private void SettingsWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is IWindowHostedViewModel)
            {
                var vm = (IWindowHostedViewModel)e.NewValue;
                vm.Close += () => SafeClose();
                Closing += (_, __) => vm.OnClosing();
            }
        }

        private void SettingsWindow_SourceInitialized(object sender, System.EventArgs e)
        {
            Trace.WriteLine("SettingsWindow SettingsWindow_SourceInitialized");

            this.Cloak();
            AccentPolicyLibrary.SetWindowBlur(this, true, true);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine("CloseButton_Click SafeClose");
            e.Handled = true;

            if (DataContext is IWindowHostedViewModel)
            {
                var vm = (IWindowHostedViewModel)DataContext;
                vm.OnClosing();
            }
        }

        public void SafeClose()
        {
            Trace.WriteLine("SettingsWindow SafeClose");

            if (!_isClosing)
            {
                // Ensure we don't double-animate if the user is able to close us multiple ways before the window stops accepting input.
                _isClosing = true;
                WindowAnimationLibrary.BeginWindowExitAnimation(this, () => this.Close());
            }
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = (WindowState == WindowState.Maximized) ?
                WindowState.Normal : WindowState.Maximized;
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            ((SettingsViewModel)DataContext).Selected = null;
        }
    }
}
