using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Scratchpad.AttachedProperties {
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

                // If the element is a TextBox, move the cursor to the end of the text.
                if (element is TextBox textBox) {
                    // Use Dispatcher.BeginInvoke to ensure the action is performed after the UI has been updated.
                    textBox.Dispatcher.BeginInvoke(
                        new System.Action(() => {
                            textBox.CaretIndex = textBox.Text.Length;
                            SetFocusOnLoad(element, false); // Reset the property to prevent unintended focus changes later
                        }),
                        DispatcherPriority.Input);
                }
                else {
                    // For other elements, just reset the property immediately.
                    element.Dispatcher.BeginInvoke(
                        new System.Action(() => SetFocusOnLoad(element, false)),
                        DispatcherPriority.Input);
                }
            }
        }
    }
}