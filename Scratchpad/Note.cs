using System.ComponentModel;

namespace Scratchpad.Models {
    public class Note : INotifyPropertyChanged {
        private string _text;
        private bool _isMinimized;

        public string Text {
            get => _text;
            set { _text = value; OnPropertyChanged(nameof(Text)); }
        }

        public bool IsMinimized {
            get => _isMinimized;
            set { _isMinimized = value; OnPropertyChanged(nameof(IsMinimized)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
