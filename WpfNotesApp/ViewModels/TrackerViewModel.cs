using System.ComponentModel;
using System.Windows.Input;
using WpfNotesApp.Models;
using WpfNotesApp.ViewModels;

namespace WpfNotesApp.ViewModels {
    public class TrackerViewModel : INotifyPropertyChanged {
        // New properties for the tracker
        private int _roomsSoldCount;
        public int RoomsSoldCount {
            get => _roomsSoldCount;
            set {
                if (_roomsSoldCount != value) {
                    _roomsSoldCount = value;
                    OnPropertyChanged(nameof(RoomsSoldCount));
                }
            }
        }

        private int _adultsCount;
        public int AdultsCount {
            get => _adultsCount;
            set {
                if (_adultsCount != value) {
                    _adultsCount = value;
                    OnPropertyChanged(nameof(AdultsCount));
                }
            }
        }

        private int _childrenCount;
        public int ChildrenCount {
            get => _childrenCount;
            set {
                if (_childrenCount != value) {
                    _childrenCount = value;
                    OnPropertyChanged(nameof(ChildrenCount));
                }
            }
        }

        private int _arrivalsCount;
        public int ArrivalsCount {
            get => _arrivalsCount;
            set {
                if (_arrivalsCount != value) {
                    _arrivalsCount = value;
                    OnPropertyChanged(nameof(ArrivalsCount));
                }
            }
        }

        // New commands for the tracker buttons
        public ICommand IncreaseRoomsSoldCommand { get; private set; }
        public ICommand DecreaseRoomsSoldCommand { get; private set; }
        public ICommand IncreaseAdultsCommand { get; private set; }
        public ICommand DecreaseAdultsCommand { get; private set; }
        public ICommand IncreaseChildrenCommand { get; private set; }
        public ICommand DecreaseChildrenCommand { get; private set; }
        public ICommand IncreaseArrivalsCommand { get; private set; }
        public ICommand DecreaseArrivalsCommand { get; private set; }

        public TrackerViewModel() {
            // Initialize new commands
            IncreaseRoomsSoldCommand = new RelayCommand(_ => RoomsSoldCount++);
            DecreaseRoomsSoldCommand = new RelayCommand(_ => { if (RoomsSoldCount > 0) RoomsSoldCount--; });
            IncreaseAdultsCommand = new RelayCommand(_ => AdultsCount++);
            DecreaseAdultsCommand = new RelayCommand(_ => { if (AdultsCount > 0) AdultsCount--; });
            IncreaseChildrenCommand = new RelayCommand(_ => ChildrenCount++);
            DecreaseChildrenCommand = new RelayCommand(_ => { if (ChildrenCount > 0) ChildrenCount--; });
            IncreaseArrivalsCommand = new RelayCommand(_ => ArrivalsCount++);
            DecreaseArrivalsCommand = new RelayCommand(_ => { if (ArrivalsCount > 0) ArrivalsCount--; });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}