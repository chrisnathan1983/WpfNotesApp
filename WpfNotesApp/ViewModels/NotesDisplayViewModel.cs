using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfNotesApp.Models;

namespace WpfNotesApp.ViewModels {
    public class NotesDisplayViewModel : INotifyPropertyChanged {
        private string _newNoteText; // Property for the new note text box

        // This collection will be bound to your ItemsControl in MainWindow.xaml
        public ObservableCollection<NoteViewModel> Notes { get; set; }

        // Commands for UI interactions
        public ICommand CreateNewNoteCommand { get; private set; }
        public ICommand DeleteNoteCommand { get; private set; }
        public ICommand ToggleMinimizeCommand { get; private set; }
        public ICommand CopyNoteCommand { get; private set; } // New command for copying note text
        public ICommand NewNoteEnterKeyCommand { get; private set; } // Command for Enter key in new note box

        public string NewNoteText {
            get => _newNoteText;
            set {
                if (_newNoteText != value) {
                    _newNoteText = value;
                    OnPropertyChanged(nameof(NewNoteText));
                }
            }
        }


        public NotesDisplayViewModel() {
            // Initialize the collection
            Notes = new ObservableCollection<NoteViewModel>();

            // Initialize commands
            CreateNewNoteCommand = new RelayCommand(CreateNewNote);
            DeleteNoteCommand = new RelayCommand<NoteViewModel>(DeleteNote);
            ToggleMinimizeCommand = new RelayCommand<NoteViewModel>(ToggleMinimize);
            CopyNoteCommand = new RelayCommand<NoteViewModel>(CopyNote);
            NewNoteEnterKeyCommand = new RelayCommand(CreateNoteFromTextBox);
        }

        private void CreateNewNote(object parameter) {
            var newNote = new NoteViewModel(new Note { Text = "", IsMinimized = false });
            Notes.Add(newNote);
        }

        private void CreateNoteFromTextBox(object parameter) {
            if (!string.IsNullOrWhiteSpace(NewNoteText)) {
                var newNote = new NoteViewModel(new Note { Text = NewNoteText, IsMinimized = false });
                Notes.Add(newNote);
                NewNoteText = ""; // Clear the textbox after creating a note
                newNote.IsFocused = true; // Set focus on the new note
            }
        }

        private void DeleteNote(NoteViewModel noteToDelete) {
            if (noteToDelete != null) {
                Notes.Remove(noteToDelete);
            }
        }

        private void ToggleMinimize(NoteViewModel noteToToggle) {
            if (noteToToggle != null) {
                noteToToggle.IsMinimized = !noteToToggle.IsMinimized;
            }
        }

        private void CopyNote(NoteViewModel noteToCopy) {
            if (noteToCopy != null) {
                System.Windows.Clipboard.SetText(noteToCopy.Text);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}