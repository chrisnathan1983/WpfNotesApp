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
using WpfNotesApp.AttachedProperties;
using WpfNotesApp.ViewModels;

namespace WpfNotesApp {
    /// <summary>
    /// Interaction logic for GroupView.xaml
    /// </summary>
    public partial class GroupView : UserControl {
        public GroupView() {
            InitializeComponent();
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
                    var viewModel = DataContext as GroupViewModel;
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
