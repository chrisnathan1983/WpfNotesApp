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

        // --- Drag and Drop Logic ---

        private void NotesItemsControl_PreviewMouseMove(object sender, MouseEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed && DragBehavior._draggedNote != null && !DragBehavior._isDragging) {
                Point currentPosition = e.GetPosition(null);

                Vector diff = DragBehavior._startDragPoint - currentPosition;

                if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance) {

                    DragBehavior._isDragging = true;

                    DataObject dragData = new DataObject("NoteViewModel", DragBehavior._draggedNote);

                    DragDrop.DoDragDrop(sender as DependencyObject, dragData, DragDropEffects.Move);

                    DragBehavior._isDragging = false;
                    DragBehavior._draggedNote = null;
                }
            }
        }

        private void NotesItemsControl_Drop(object sender, DragEventArgs e) {
            DragBehavior._isDragging = false;
            DragBehavior._draggedNote = null;

            if (e.Data.GetDataPresent("NoteViewModel")) {
                NoteViewModel droppedNote = e.Data.GetData("NoteViewModel") as NoteViewModel;

                // Explicitly cast e.OriginalSource to UIElement, then walk up and cast to Border
                UIElement dropTarget = e.OriginalSource as UIElement;
                while (dropTarget != null && !(dropTarget is Border)) {
                    dropTarget = VisualTreeHelper.GetParent(dropTarget) as UIElement;
                }

                // The result of the loop needs to be explicitly cast to Border before accessing Tag
                NoteViewModel targetNote = (dropTarget as Border)?.Tag as NoteViewModel; // <-- Fix is here

                if (droppedNote != null && targetNote != null && droppedNote != targetNote) {
                    var viewModel = DataContext as MainWindowViewModel;
                    if (viewModel != null) {
                        int oldIndex = viewModel.Notes.IndexOf(droppedNote);
                        int newIndex = viewModel.Notes.IndexOf(targetNote);

                        if (oldIndex != -1 && newIndex != -1) {
                            viewModel.Notes.Move(oldIndex, newIndex);
                        }
                    }
                }
            }
        }
    }
}