// MainWindow.xaml.cs
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Scratchpad.Models;
using Scratchpad.ViewModels;
using Scratchpad.AttachedProperties;

namespace Scratchpad {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            var viewModel = DataContext as MainWindowViewModel;
            if (viewModel != null) {
                // We must handle the closing confirmation here as well.
                // If PromptToSaveAndExit() returns false, it means the user cancelled.
                if (!viewModel.PromptToSaveAndExit()) {
                    e.Cancel = true;
                }
            }
        }
    }
}