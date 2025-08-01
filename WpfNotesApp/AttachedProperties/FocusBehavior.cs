using System.Windows;

namespace WpfNotesApp.AttachedProperties {
    public static class FocusBehavior {
        public static readonly DependencyProperty FocusOnLoadProperty =
            DependencyProperty.RegisterAttached(
                "FocusOnLoad",
                typeof(bool),
                typeof(FocusBehavior),
                new PropertyMetadata(false, OnFocusOnLoadChanged));

        public static bool GetFocusOnLoad(DependencyObject obj) {
            return (bool)obj.GetValue(FocusOnLoadProperty);
        }

        public static void SetFocusOnLoad(DependencyObject obj, bool value) {
            obj.SetValue(FocusOnLoadProperty, value);
        }

        private static void OnFocusOnLoadChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            UIElement element = d as UIElement;
            if (element != null && (bool)e.NewValue) {
                element.Focus();
                // Reset the property to false to prevent future unintended focus
                element.Dispatcher.BeginInvoke(
                    new System.Action(() => SetFocusOnLoad(element, false)),
                    System.Windows.Threading.DispatcherPriority.Input);
            }
        }
    }
}