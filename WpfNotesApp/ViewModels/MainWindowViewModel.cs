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
        public NotesDisplayViewModel NotesDisplay { get; set; }
        public TrackerViewModel Tracker { get; set; } // Assuming you have a TrackerViewModel for the tracker

        public MainWindowViewModel() {
            NotesDisplay = new NotesDisplayViewModel(); // Initialize the notes display view model
            NotesDisplay.Notes.Add(new NoteViewModel(new Note { Text = "First note\nwith new line\nwith new line\nwith new line\nwith new line", IsMinimized = false }));
            NotesDisplay.Notes.Add(new NoteViewModel(new Note { Text = "Second note", IsMinimized = false }));

            Tracker = new TrackerViewModel(); // Initialize the tracker view model
            Tracker.RoomsSoldCount = 30;
            Tracker.AdultsCount = 45;
            Tracker.ChildrenCount = 0;
            Tracker.ArrivalsCount = 20;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}