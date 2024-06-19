using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PropertyChanged;
using System.ComponentModel;

namespace ParkingSystem.MVVM.Models
{
    [AddINotifyPropertyChangedInterface]
    public class ParkingStatus
    {
        private int _spacesAvailable = 10;
        private DoorStatus entryDoorStatus = DoorStatus.Closed;
        private DoorStatus exitDoorStatus = DoorStatus.Closed;

        public int SpacesAvailable
        {
            get => _spacesAvailable;
            set
            {
                if (value >= 0 && value <= 10)
                {
                    _spacesAvailable = value;
                    PropertyChangedSaveDB?.Invoke(this, new PropertyChangedEventArgs(nameof(SpacesAvailable)));
                }
            }
        }
        public int SpacesOccupied
        {
            get { return 10 - _spacesAvailable; }
        }

        public DoorStatus EntryDoorStatus
        {
            get => entryDoorStatus;
            set
            {
                entryDoorStatus = value;
                PropertyChangedSaveDB?.Invoke(this, new PropertyChangedEventArgs(nameof(SpacesAvailable)));
            }
        }
        public DoorStatus ExitDoorStatus
        {
            get => exitDoorStatus;
            set
            {
                exitDoorStatus = value;
                PropertyChangedSaveDB?.Invoke(this, new PropertyChangedEventArgs(nameof(SpacesAvailable)));
            }
        }
        #region UI Properties
        public string TrafficLightBackgroundColor
        {
            get
            {
                if (SpacesOccupied <= 5)
                    return "Green";
                else if (SpacesOccupied >= 6 && SpacesOccupied <= 9)
                    return "Yellow";
                else
                    return "Red";
            }
        }

        public string TrafficLightStatusText
        {
            get
            {
                if (SpacesOccupied <= 5)
                    return "Bajo";
                else if (SpacesOccupied >= 6 && SpacesOccupied <= 9)
                    return "Moderado";
                else
                    return "Lleno";
            }
        }

        public string EntryDoorStatusBackgroundColor
        {
            get
            {
                if (EntryDoorStatus == DoorStatus.Open)
                    return "Green";
                else
                    return "Red";
            }
        }

        public string ExitDoorStatusBackgroundColor
        {
            get
            {
                if (ExitDoorStatus == DoorStatus.Open)
                    return "Green";
                else
                    return "Red";
            }
        }
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChangedSaveDB;
        #endregion
    }
}