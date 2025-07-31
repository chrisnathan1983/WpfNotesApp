// Converters/ReferenceEqualityConverter.cs
using System;
using System.Globalization;
using System.Windows.Data;

namespace WpfNotesApp.Converters {
    public class ReferenceEqualityConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            // value is MainWindowViewModel.NoteToFocus
            // parameter is the current NoteViewModel of the TextBox
            return ReferenceEquals(value, parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}