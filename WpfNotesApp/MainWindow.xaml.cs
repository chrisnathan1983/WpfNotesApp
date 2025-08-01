// MainWindow.xaml.cs
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WpfNotesApp.Models;
using WpfNotesApp.ViewModels;
using WpfNotesApp.AttachedProperties;

namespace WpfNotesApp {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }
    }
}