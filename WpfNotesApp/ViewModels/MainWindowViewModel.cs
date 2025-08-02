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
        public NotesDisplayViewModel NotesDisplay { get; set; }
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
                }
            }
        }

        // Commands for file operations
        public ICommand NewFileCommand { get; private set; }
        public ICommand OpenFileCommand { get; private set; }
        public ICommand SaveFileCommand { get; private set; }
        public ICommand SaveFileAsCommand { get; private set; }
        public ICommand ExitApplicationCommand { get; private set; }

        public MainWindowViewModel() {
            NotesDisplay = new NotesDisplayViewModel();
            NotesDisplay.Notes.Add(new NoteViewModel(new Note { Text = "First note\nwith new line\nwith new line\nwith new line\nwith new line", IsMinimized = false }));
            NotesDisplay.Notes.Add(new NoteViewModel(new Note { Text = "Second note", IsMinimized = false }));

            Tracker = new TrackerViewModel();
            Tracker.RoomsSoldCount = 30;
            Tracker.AdultsCount = 45;
            Tracker.ChildrenCount = 0;
            Tracker.ArrivalsCount = 20;

            // Initialize commands
            NewFileCommand = new RelayCommand(_ => NewFile());
            OpenFileCommand = new RelayCommand(_ => OpenFile());
            SaveFileCommand = new RelayCommand(_ => SaveFile(false)); // False means not "Save As"
            SaveFileAsCommand = new RelayCommand(_ => SaveFile(true)); // True means "Save As"
            ExitApplicationCommand = new RelayCommand(_ => ExitApplication());

            // Set initial file name
            CurrentFileName = "Untitled";
        }

        private void NewFile() {
            // Clear all data
            NotesDisplay.Notes.Clear();
            Tracker.RoomsSoldCount = 0;
            Tracker.AdultsCount = 0;
            Tracker.ChildrenCount = 0;
            Tracker.ArrivalsCount = 0;
            _currentFilePath = null;
            CurrentFileName = "Untitled";
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
                        string[] trackerParts = lines[0].Split(',');
                        if (trackerParts.Length == 4) {
                            Tracker.RoomsSoldCount = int.Parse(trackerParts[0].Split(' ')[0]);
                            Tracker.AdultsCount = int.Parse(trackerParts[1].Trim().Split(' ')[0]);
                            Tracker.ChildrenCount = int.Parse(trackerParts[2].Trim().Split(' ')[0]);
                            Tracker.ArrivalsCount = int.Parse(trackerParts[3].Trim().Split(' ')[0]);
                        }

                        // Clear existing notes and parse the rest of the file for notes
                        NotesDisplay.Notes.Clear();
                        // Join lines from the second line onwards, then split by double newline
                        string notesContent = string.Join("\n", lines.Skip(2));
                        string[] noteTexts = notesContent.Split(new string[] { "\n\n" }, System.StringSplitOptions.None);

                        foreach (var noteText in noteTexts) {
                            if (!string.IsNullOrEmpty(noteText.Trim())) { // Trim to handle empty lines from splitting
                                NotesDisplay.Notes.Add(new NoteViewModel(new Note { Text = noteText, IsMinimized = false }));
                            }
                        }
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

        private void SaveFile(bool saveAs) {
            if (string.IsNullOrEmpty(_currentFilePath) || saveAs) {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                saveFileDialog.FileName = CurrentFileName == "Untitled" ? "MyNotes" : CurrentFileName; // Suggest a default name
                if (saveFileDialog.ShowDialog() == true) {
                    _currentFilePath = saveFileDialog.FileName;
                    CurrentFileName = Path.GetFileNameWithoutExtension(_currentFilePath);
                }
                else {
                    return; // User cancelled save
                }
            }

            if (!string.IsNullOrEmpty(_currentFilePath)) {
                try {
                    // Construct the tracker data string
                    string trackerData = $"{Tracker.RoomsSoldCount} ROOMS SOLD, {Tracker.AdultsCount} ADULTS, {Tracker.ChildrenCount} CHILDREN, {Tracker.ArrivalsCount} ARRIVALS";

                    // Construct the notes data string, separated by double newlines
                    string notesData = string.Join("\n\n", NotesDisplay.Notes.Select(note => note.Text));

                    // Combine and save to the file
                    string fileContent = $"{trackerData}\n\n{notesData}";
                    File.WriteAllText(_currentFilePath, fileContent);
                }
                catch (Exception ex) {
                    MessageBox.Show($"Error saving file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ExitApplication() {
            Application.Current.Shutdown();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
