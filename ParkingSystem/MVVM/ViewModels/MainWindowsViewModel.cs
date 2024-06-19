using ParkingSystem.Entities;
using ParkingSystem.MVVM.Models;
using PropertyChanged;

namespace ParkingSystem.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MainWindowsViewModel
    {
        public Models.ParkingStatus ParkingStatus { get; set; } = new Models.ParkingStatus();
        public List<ParkingHistory> ParkingHistory { get; set; } = new List<ParkingHistory>();
        public List<string> SecondaryFilter { get; set; } = new List<string>();
    }
}