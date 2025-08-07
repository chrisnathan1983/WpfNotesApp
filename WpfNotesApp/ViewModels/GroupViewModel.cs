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
    public class GroupViewModel : INotifyPropertyChanged {
        private string _name; // Property for the group name
        private string _newNoteText; // Property for the new note text box

        // This collection will be bound to your ItemsControl in MainWindow.xaml
        public ObservableCollection<NoteViewModel> Notes { get; set; }

        // Commands for UI interactions
        public ICommand DeleteNoteCommand { get; private set; }
        public ICommand NewNoteCommand { get; private set; } // Command for Enter key in new note box

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


        public GroupViewModel() {
            // Initialize the collection
            Notes = new ObservableCollection<NoteViewModel>();

            // Initialize commands
            DeleteNoteCommand = new RelayCommand<NoteViewModel>(DeleteNote);
            NewNoteCommand = new RelayCommand(CreateNoteFromTextBox);
        }


        public GroupViewModel(string name) {
            // Initialize the group name
            Name = name;
            // Initialize the collection
            Notes = new ObservableCollection<NoteViewModel>();

            // Initialize commands
            DeleteNoteCommand = new RelayCommand<NoteViewModel>(DeleteNote);
            NewNoteCommand = new RelayCommand(CreateNoteFromTextBox);
        }

        public void AddNote(string noteText) => CreateNote(noteText);

        private void CreateNoteFromTextBox(object parameter) => CreateNote(NewNoteText);

        private void CreateNote(string noteText) {
            if (!string.IsNullOrWhiteSpace(noteText)) {
                var newNote = new NoteViewModel(new Note { Text = noteText, IsMinimized = false });
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
