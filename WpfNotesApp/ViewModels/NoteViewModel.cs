// NoteViewModel.cs
using System.ComponentModel;
using WpfNotesApp.Models;
using System.Windows; // Required for Visibility enum

namespace WpfNotesApp.ViewModels {
    public class NoteViewModel : INotifyPropertyChanged {
        private Note _note;
        private bool _isHovered; // New property to track hover state
        private bool _isFocused; // New property to track focus state

        public NoteViewModel(Note note) {
            _note = note;
            _note.PropertyChanged += (sender, e) => OnPropertyChanged(e.PropertyName);
        }

        public string Text {
            get => _note.Text;
            set {
                if (_note.Text != value) {
                    _note.Text = value;
                    OnPropertyChanged(nameof(Text));
                    OnPropertyChanged(nameof(TextWrappingMode));
                    OnPropertyChanged(nameof(AcceptsReturnMode));
                    OnPropertyChanged(nameof(TextBoxHeight));
                    OnPropertyChanged(nameof(ShowMinimizeButton));
                }
            }
        }

        public bool IsMinimized {
            get => _note.IsMinimized;
            set {
                if (_note.IsMinimized != value) {
                    _note.IsMinimized = value;
                    OnPropertyChanged(nameof(IsMinimized));
                    OnPropertyChanged(nameof(TextWrappingMode));
                    OnPropertyChanged(nameof(AcceptsReturnMode));
                    OnPropertyChanged(nameof(TextBoxHeight));
                }
            }
        }

        // New property to track hover state for the specific note
        public bool IsHovered {
            get => _isHovered;
            set {
                if (_isHovered != value) {
                    _isHovered = value;
                    OnPropertyChanged(nameof(IsHovered));
                    OnPropertyChanged(nameof(PanelVisibility)); // Notify that visibility might change
                }
            }
        }

        public bool IsFocused {
            get => _isFocused;
            set {
                if (_isFocused != value) {
                    _isFocused = value;
                    OnPropertyChanged(nameof(IsFocused));
                }
            }
        }

        // Derived property for the visibility of the panels
        public Visibility PanelVisibility {
            get => IsHovered ? Visibility.Visible : Visibility.Hidden;
        }

        public System.Windows.TextWrapping TextWrappingMode {
            get => IsMinimized ? System.Windows.TextWrapping.NoWrap : System.Windows.TextWrapping.Wrap;
        }

        public bool AcceptsReturnMode {
            get => !IsMinimized;
        }

        public double TextBoxHeight {
            get => IsMinimized ? 24 : double.NaN;
        }

        public Visibility ShowMinimizeButton {
            get {
                bool noteTextIsMultiline = Text.Contains("\n") || Text.Length > 50;
                return noteTextIsMultiline ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}