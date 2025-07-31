using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input; // For ICommand

// Assuming your Note model and NoteViewModel are in these namespaces
using WpfNotesApp.Models;
using WpfNotesApp.ViewModels;

namespace WpfNotesApp.ViewModels {
    public class MainWindowViewModel : INotifyPropertyChanged {
        // This collection will be bound to your ItemsControl in MainWindow.xaml
        public ObservableCollection<NoteViewModel> Notes { get; set; }

        // Commands for UI interactions
        public ICommand CreateNewNoteCommand { get; private set; }
        public ICommand DeleteNoteCommand { get; private set; }
        public ICommand ToggleMinimizeCommand { get; private set; }
        public ICommand CopyNoteCommand { get; private set; } // New command for copying note text

        public MainWindowViewModel() {
            // Initialize the collection
            Notes = new ObservableCollection<NoteViewModel>();

            // Initialize sample notes
            Notes.Add(new NoteViewModel(new Note { Text = "First note\nwith new line\nwith new line\nwith new line\nwith new line", IsMinimized = false }));
            Notes.Add(new NoteViewModel(new Note { Text = "Second note", IsMinimized = false }));

            // Initialize commands
            CreateNewNoteCommand = new RelayCommand(CreateNewNote);
            DeleteNoteCommand = new RelayCommand<NoteViewModel>(DeleteNote);
            ToggleMinimizeCommand = new RelayCommand<NoteViewModel>(ToggleMinimize);
            CopyNoteCommand = new RelayCommand<NoteViewModel>(CopyNote);
        }

        private void CreateNewNote(object parameter) {
            var newNote = new NoteViewModel(new Note { Text = "", IsMinimized = false });
            Notes.Add(newNote);
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