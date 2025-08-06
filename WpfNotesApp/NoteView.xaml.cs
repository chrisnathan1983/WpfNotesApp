using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfNotesApp.ViewModels;

namespace WpfNotesApp {
    /// <summary>
    /// Interaction logic for NoteView.xaml
    /// </summary>
    public partial class NoteView : UserControl {
        public NoteView() {
            InitializeComponent();
        }


        private void NoteBorder_MouseEnter(object sender, MouseEventArgs e) {
            // Explicitly cast sender to Border using 'as' operator
            Border border = sender as Border;
            if (border != null) {
                NoteViewModel noteViewModel = border.DataContext as NoteViewModel;
                if (noteViewModel != null) {
                    noteViewModel.IsHovered = true;
                }
            }
        }

        private void NoteBorder_MouseLeave(object sender, MouseEventArgs e) {
            // Explicitly cast sender to Border using 'as' operator
            Border border = sender as Border; // <-- Fix is here
            if (border != null) {
                NoteViewModel noteViewModel = border.DataContext as NoteViewModel;
                if (noteViewModel != null) {
                    noteViewModel.IsHovered = false;
                }
            }
        }
    }
}
