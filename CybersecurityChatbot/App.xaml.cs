﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CybersecurityChatbot
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
   
        public partial class App : Application
        {
            protected override void OnStartup(StartupEventArgs e)
            {
                base.OnStartup(e);

                // Set up global exception handling
                this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            }

            private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
            {
                MessageBox.Show($"An unexpected error occurred: {e.Exception.Message}",
                              "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                e.Handled = true;
            }
        }
    }

