using Microsoft.Win32; // For OpenFileDialog and SaveFileDialog
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO; // For file I/O
using System.Linq;
using System.Windows; // For Application.Current.Shutdown()
using System.Windows.Input;
using WpfNotesApp.Models;
using WpfNotesApp.ViewModels;

namespace WpfNotesApp.ViewModels {
    public class MainWindowViewModel : INotifyPropertyChanged {
        // This collection will be bound to your ItemsControl in MainWindow.xaml
        public GroupViewModel GroupView { get; set; }
        public TrackerViewModel Tracker { get; set; }

        private string _currentFilePath;
        private string _currentFileName;

        // Property to display the current file name in the menu
        public string CurrentFileName {
            get => _currentFileName;
            set {
                if (_currentFileName != value) {
                    _currentFileName = value;
                    OnPropertyChanged(nameof(CurrentFileName));
                    OnPropertyChanged(nameof(DisplayFileName));
                }
            }
        }

        private bool _isUnsaved;
        public bool IsUnsaved {
            get => _isUnsaved;
            set {
                if (_isUnsaved != value) {
                    _isUnsaved = value;
                    OnPropertyChanged(nameof(IsUnsaved));
                    OnPropertyChanged(nameof(DisplayFileName));
                }
            }
        }

        // New derived property
        public string DisplayFileName {
            get => IsUnsaved ? $"{CurrentFileName}*" : CurrentFileName;
        }

        // Commands for file operations
        public ICommand NewFileCommand { get; private set; }
        public ICommand OpenFileCommand { get; private set; }
        public ICommand SaveFileCommand { get; private set; }
        public ICommand SaveFileAsCommand { get; private set; }
        public ICommand ExitApplicationCommand { get; private set; }

        public MainWindowViewModel() {
            GroupView = new GroupViewModel("#Untagged");
            GroupView.AddNote("First note\nwith new line\nwith new line\nwith new line\nwith new line");
            GroupView.AddNote("Second note");

            // Subscribe to events when the NotesDisplay is initialized
            GroupView.Notes.CollectionChanged += NotesCollection_CollectionChanged;
            foreach (var note in GroupView.Notes) {
                note.PropertyChanged += Note_PropertyChanged;
            }

            Tracker = new TrackerViewModel();
            Tracker.RoomsSoldCount = 30;
            Tracker.AdultsCount = 45;
            Tracker.ChildrenCount = 0;
            Tracker.ArrivalsCount = 20;

            // Subscribe to Tracker property changes
            Tracker.PropertyChanged += Tracker_PropertyChanged;

            // Initialize commands
            NewFileCommand = new RelayCommand(_ => NewFile());
            OpenFileCommand = new RelayCommand(_ => OpenFile());
            SaveFileCommand = new RelayCommand(_ => SaveFile(false)); // False means not "Save As"
            SaveFileAsCommand = new RelayCommand(_ => SaveFile(true)); // True means "Save As"
            ExitApplicationCommand = new RelayCommand(_ => ExitApplication());

            // Set initial file name
            CurrentFileName = "File";
            IsUnsaved = true;
        }

        private void NewFile() {
            // Clear all data
            GroupView.Notes.Clear();
            Tracker.RoomsSoldCount = 0;
            Tracker.AdultsCount = 0;
            Tracker.ChildrenCount = 0;
            Tracker.ArrivalsCount = 0;
            _currentFilePath = null;
            CurrentFileName = "File";
            IsUnsaved = false;
        }

        private void OpenFile() {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true) {
                try {
                    _currentFilePath = openFileDialog.FileName;
                    CurrentFileName = Path.GetFileNameWithoutExtension(_currentFilePath);

                    string[] lines = File.ReadAllLines(_currentFilePath);

                    if (lines.Length > 0) {
                        // Parse the first line for tracker data
                        bool parsed = false;
                        string[] trackerParts = lines[0].Split(',');
                        if (trackerParts.Length == 4) {
                            parsed = true;
                            Tracker.RoomsSoldCount = int.Parse(trackerParts[0].Split(' ')[0]);
                            Tracker.AdultsCount = int.Parse(trackerParts[1].Trim().Split(' ')[0]);
                            Tracker.ChildrenCount = int.Parse(trackerParts[2].Trim().Split(' ')[0]);
                            Tracker.ArrivalsCount = int.Parse(trackerParts[3].Trim().Split(' ')[0]);
                        }

                        // Clear existing notes and parse the rest of the file for notes
                        GroupView.Notes.Clear();
                        // Join lines from the second line onwards, then split by double newline
                        string notesContent = parsed ? string.Join("\n", lines.Skip(2)) : string.Join("\n", lines);
                        string[] noteTexts = notesContent.Split(new string[] { "\n\n" }, System.StringSplitOptions.None);

                        foreach (var noteText in noteTexts) {
                            if (!string.IsNullOrEmpty(noteText.Trim())) { // Trim to handle empty lines from splitting
                                GroupView.Notes.Add(new NoteViewModel(new Note { Text = noteText, IsMinimized = false }));
                            }
                        }
                        IsUnsaved = false; // Reset unsaved state after loading
                    }
                    else {
                        // Handle empty file: clear everything
                        NewFile();
                    }
                }
                catch (Exception ex) {
                    MessageBox.Show($"Error opening file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        internal bool SaveFile(bool saveAs) {
            if (string.IsNullOrEmpty(_currentFilePath) || saveAs) {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                saveFileDialog.FileName = CurrentFileName == "File" ? "MyNotes" : CurrentFileName; // Suggest a default name
                if (saveFileDialog.ShowDialog() == true) {
                    _currentFilePath = saveFileDialog.FileName;
                    CurrentFileName = Path.GetFileNameWithoutExtension(_currentFilePath);
                }
                else {
                    return false; // User cancelled save
                }
            }

            if (!string.IsNullOrEmpty(_currentFilePath)) {
                try {
                    // Construct the tracker data string
                    string trackerData = $"{Tracker.RoomsSoldCount} ROOMS SOLD, {Tracker.AdultsCount} ADULTS, {Tracker.ChildrenCount} CHILDREN, {Tracker.ArrivalsCount} ARRIVALS";

                    // Construct the notes data string, separated by double newlines
                    string notesData = string.Join("\n\n", GroupView.Notes.Select(note => note.Text));

                    // Combine and save to the file
                    string fileContent = $"{trackerData}\n\n{notesData}";
                    File.WriteAllText(_currentFilePath, fileContent);
                    IsUnsaved = false; // Reset unsaved state after saving
                    return true; // Indicate success
                }
                catch (Exception ex) {
                    MessageBox.Show($"Error saving file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false; // Indicate failure
                }
            }
            return false; // If no file path is set, return false
        }
        internal bool PromptToSaveAndExit() {
            if (IsUnsaved) {
                MessageBoxResult result = MessageBox.Show(
                    "You have unsaved changes. Do you want to save before closing?",
                    "Save Changes",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes) {
                    return SaveFile(false); // Only proceed if save is successful
                }
                else if (result == MessageBoxResult.No) {
                    return true; // Discard changes and proceed
                }
                else {
                    return false; // Cancel
                }
            }
            return true; // No unsaved changes, so proceed
        }

        internal void ExitApplication() {
            if (PromptToSaveAndExit()) {
                Application.Current.Shutdown();
            }
        }

        private void NotesCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            IsUnsaved = true;

            // Also subscribe to new items and unsubscribe from old items
            if (e.NewItems != null) {
                foreach (NoteViewModel note in e.NewItems) {
                    note.PropertyChanged += Note_PropertyChanged;
                }
            }

            if (e.OldItems != null) {
                foreach (NoteViewModel note in e.OldItems) {
                    note.PropertyChanged -= Note_PropertyChanged;
                }
            }
        }

        private void Tracker_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            // The only properties that change are the counts, so we can always set IsDirty to true
            IsUnsaved = true;
        }

        private void Note_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(NoteViewModel.Text)) {
                IsUnsaved = true;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
