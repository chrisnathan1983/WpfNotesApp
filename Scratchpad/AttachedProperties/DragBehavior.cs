// AttachedProperties/DragBehavior.cs
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Scratchpad.ViewModels; // Make sure this is included

namespace Scratchpad.AttachedProperties {
    public static class DragBehavior {
        public static readonly DependencyProperty IsDragSourceProperty =
            DependencyProperty.RegisterAttached(
                "IsDragSource",
                typeof(bool),
                typeof(DragBehavior),
                new PropertyMetadata(false, OnIsDragSourceChanged));

        public static bool GetIsDragSource(DependencyObject obj) {
            return (bool)obj.GetValue(IsDragSourceProperty);
        }

        public static void SetIsDragSource(DependencyObject obj, bool value) {
            obj.SetValue(IsDragSourceProperty, value);
        }

        private static void OnIsDragSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            // Cast to FrameworkElement, as DataContext is on FrameworkElement
            FrameworkElement uiElement = d as FrameworkElement;
            if (uiElement == null) return;

            if ((bool)e.NewValue == true) {
                uiElement.PreviewMouseLeftButtonDown += UIElement_PreviewMouseLeftButtonDown;
            }
            else {
                uiElement.PreviewMouseLeftButtonDown -= UIElement_PreviewMouseLeftButtonDown;
            }
        }

        // Make these internal static fields so MainWindow.xaml.cs can access them
        internal static NoteViewModel _draggedNote;
        internal static Point _startDragPoint;
        internal static bool _isDragging = false; // Add _isDragging here as well

        private static void UIElement_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            _startDragPoint = e.GetPosition(null);

            // Cast to FrameworkElement to access DataContext
            FrameworkElement element = sender as FrameworkElement;
            if (element != null && element.DataContext is NoteViewModel noteViewModel) {
                _draggedNote = noteViewModel;
            }
            else {
                _draggedNote = null;
            }
            e.Handled = true; // Mark the event as handled
        }
    }
}