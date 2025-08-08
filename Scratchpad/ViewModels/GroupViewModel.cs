using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media; // Added for SolidColorBrush
using Scratchpad.Models;
using Scratchpad.ViewModels;

namespace Scratchpad.ViewModels {
    public class GroupViewModel : INotifyPropertyChanged {
        private string _name;
        private string _newNoteText;
        private SolidColorBrush _groupColor; // New property for the group's color
        private SolidColorBrush[] _groupColors = new SolidColorBrush[] {
            new SolidColorBrush(Color.FromArgb(255, 46, 46, 46)), // Default color
            new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)), // Red
            new SolidColorBrush(Color.FromArgb(255, 0, 255, 0)), // Green
            new SolidColorBrush(Color.FromArgb(255, 0, 0, 255)), // Blue
            new SolidColorBrush(Color.FromArgb(255, 255, 255, 0)), // Yellow
            new SolidColorBrush(Color.FromArgb(255, 255, 165, 0)), // Orange
            new SolidColorBrush(Color.FromArgb(255, 128, 0, 128)) // Purple
        };
        private int _currentColorIndex = 0; // Index to track the current color

        public ObservableCollection<NoteViewModel> Notes { get; set; }

        public ICommand DeleteNoteCommand { get; private set; }
        public ICommand NewNoteCommand { get; private set; }
        public ICommand ToggleColorCommand { get; private set; }

        public string Name {
            get => _name;
            set {
                if (_name != value) {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }
        public string NewNoteText {
            get => _newNoteText;
            set {
                if (_newNoteText != value) {
                    _newNoteText = value;
                    OnPropertyChanged(nameof(NewNoteText));
                }
            }
        }

        // New property for the group's color, with a default value
        public SolidColorBrush GroupColor {
            get => _groupColor;
            set {
                if (_groupColor != value) {
                    _groupColor = value;
                    OnPropertyChanged(nameof(GroupColor));
                }
            }
        }


        public GroupViewModel() {
            Initialize();
        }

        public GroupViewModel(string name) {
            Name = name;
            Initialize();
        }

        private void Initialize() {
            Notes = new ObservableCollection<NoteViewModel>();
            DeleteNoteCommand = new RelayCommand<NoteViewModel>(DeleteNote);
            NewNoteCommand = new RelayCommand(CreateNoteFromTextBox);
            GroupColor = new SolidColorBrush(Color.FromArgb(255, 46, 46, 46)); // Default color
            ToggleColorCommand = new RelayCommand(ToggleColor); // Initialize the toggle color command
        }

        public void AddNote(string noteText) => CreateNote(noteText);

        private void CreateNoteFromTextBox(object parameter) => CreateNote(NewNoteText);

        private void CreateNote(string noteText) {
            if (!string.IsNullOrWhiteSpace(noteText)) {
                var newNote = new NoteViewModel(new Note { Text = noteText, IsMinimized = false });
                Notes.Add(newNote);
                NewNoteText = "";
                newNote.IsFocused = true;
            }
        }

        private void DeleteNote(NoteViewModel noteToDelete) {
            if (noteToDelete != null) {
                Notes.Remove(noteToDelete);
            }
        }

        private void ToggleColor(object parameter) {
            // Logic to toggle color can be added here if needed
            _currentColorIndex = (_currentColorIndex + 1) % _groupColors.Length;
            GroupColor = _groupColors[_currentColorIndex];
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}