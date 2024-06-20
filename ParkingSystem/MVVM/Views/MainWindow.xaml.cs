using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using ParkingSystem.Entities;
using ParkingSystem.MVVM.ViewModels;
using ParkingSystem.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Appearance;


namespace ParkingSystem
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly ParkingContext _context;
        private MainWindowsViewModel viewModel = new MainWindowsViewModel();
        private SerialPort serialPort;

        public MainWindow(ParkingContext dbContext)
        {
            InitializeComponent();
            ApplicationThemeManager.Apply(this);
            DataContext = viewModel;
            _context = dbContext;
            _ = InitializeData();
            viewModel.ParkingStatus.PropertyChangedSaveDB += ParkingStatus_PropertyChanged;

            // Carga el historial desde la db
            viewModel.ParkingHistory = GetHistory();
            // Inicializa el puerto serial
            InitializeSerialPort();
        }

        private async Task InitializeData()
        {
            var parkingStatusDB = await _context.ParkingStatuses.FirstOrDefaultAsync(x => x.Id == 1);
            if (parkingStatusDB == null)
            {
                ParkingStatus parkingStatus = new ParkingStatus
                {
                    SpacesAvailable = 10,
                    ExitDoorStatus = 1,
                    EntryDoorStatus = 1,
                };
                _context.ParkingStatuses.Add(parkingStatus);
                await _context.SaveChangesAsync();

                parkingStatusDB = parkingStatus;
            }

            viewModel.ParkingStatus.SpacesAvailable = parkingStatusDB.SpacesAvailable;
            viewModel.ParkingStatus.EntryDoorStatus = (DoorStatus)parkingStatusDB.EntryDoorStatus;
            viewModel.ParkingStatus.ExitDoorStatus = (DoorStatus)parkingStatusDB.ExitDoorStatus;
        }

        private void InitializeSerialPort()
        {
            try
            {
                serialPort = new SerialPort("COM20", 9600); // Ajusta el nombre del puerto según corresponda
                serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                serialPort.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error connect arduino", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadLine();
            Application.Current.Dispatcher.Invoke(() => ProcessData(indata));
        }

        private void ProcessData(string data)

        {

            if (data.StartsWith("Entrada:"))
            {
                viewModel.ParkingStatus.SpacesAvailable--;
                AddHistory(new ParkingHistory
                {
                    IsEnter = true,
                    Date = DateTime.Now.ToString(),
                    SpaceAvailable = viewModel.ParkingStatus.SpacesAvailable
                });
            }
            else if (data.StartsWith("Salida:"))
            {
                viewModel.ParkingStatus.SpacesAvailable++;
                AddHistory(new ParkingHistory
                {
                    IsEnter = false,
                    Date = DateTime.Now.ToString(),
                    SpaceAvailable = viewModel.ParkingStatus.SpacesAvailable
                });
            }
            else if (data.StartsWith("Entrada abierta"))
            {
                viewModel.ParkingStatus.EntryDoorStatus = DoorStatus.Open;
            }
            else if (data.StartsWith("Entrada cerrada"))
            {
                viewModel.ParkingStatus.EntryDoorStatus = DoorStatus.Closed;
            }
            else if (data.StartsWith("Salida abierta"))
            {
                viewModel.ParkingStatus.ExitDoorStatus = DoorStatus.Open;
            }
            else if (data.StartsWith("Salida cerrada"))
            {
                viewModel.ParkingStatus.ExitDoorStatus = DoorStatus.Closed;
            }
        }


        private async void SimulateDoorClose()
        {
            await Task.Delay(3000); // Simula el tiempo de espera
            viewModel.ParkingStatus.EntryDoorStatus = DoorStatus.Closed;
            viewModel.ParkingStatus.ExitDoorStatus = DoorStatus.Closed;
        }

        public void AddHistory(ParkingHistory history)
        {
            // Agrega un nuevo elemento
            _context.ParkingHistories.Add(history);
            _context.SaveChanges();

            // Carga el historial en el ViewModel
            viewModel.ParkingHistory = _context.ParkingHistories.OrderByDescending(x => x.Date).ToList();
        }

        public List<ParkingHistory> GetHistory()
        {
            return _context.ParkingHistories.OrderByDescending(x => x.Date).ToList();
        }

        #region Events
        private void ParkingStatus_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var parkingStatusDB = _context.ParkingStatuses.FirstOrDefault(x => x.Id == 1);
            if (parkingStatusDB != null)
            {
                parkingStatusDB.SpacesAvailable = viewModel.ParkingStatus.SpacesAvailable;
                parkingStatusDB.EntryDoorStatus = (int)viewModel.ParkingStatus.EntryDoorStatus;
                parkingStatusDB.ExitDoorStatus = (int)viewModel.ParkingStatus.ExitDoorStatus;

                _context.ParkingStatuses.Update(parkingStatusDB);
                _context.SaveChanges();
            }
        }

        private void ReportType_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var ReportTypeSelected = (ComboBoxItem)ReportType.SelectedValue;
            if (ReportTypeSelected != null)
            {
                SecondaryFilter.Visibility = Visibility.Hidden;
                SecondaryFilterDay.Visibility = Visibility.Hidden;

                if (ReportTypeSelected.Content.ToString() == "Semanal")
                {
                    SecondaryFilter.Visibility = Visibility.Visible;
                    SecondaryFilterTitle.Text = "Semana";


                    int currentWeekNumber = DateUtils.CurrentWeekNumber();
                    viewModel.SecondaryFilter = Enumerable.Range(1, currentWeekNumber)
                                                          .Select(x => $"Semana {x:D2}")
                                                          .OrderDescending()
                                                          .ToList();
                }

                if (ReportTypeSelected.Content.ToString() == "Mensual")
                {
                    SecondaryFilter.Visibility = Visibility.Visible;
                    SecondaryFilterTitle.Text = "Mes";

                    int currentMonth = DateTime.Now.Month;
                    viewModel.SecondaryFilter = DateUtils.monthNames.Take(currentMonth).ToList();

                }

                if (ReportTypeSelected.Content.ToString() == "Dia")
                {
                    SecondaryFilterDay.Visibility = Visibility.Visible;
                }

            }

        }

        private void SecondaryFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var ReportTypeSelected = (ComboBoxItem)ReportType.SelectedValue;

            //Filter Semanal and Mensual
            var SecondaryItemSelected = (string)SecondaryFilterComboBox.SelectedValue;
            if (ReportTypeSelected != null && SecondaryItemSelected != null)
            {
                if (ReportTypeSelected.Content.ToString() == "Semanal")
                {
                    int weekNumberSelected = int.Parse(SecondaryItemSelected.Replace("Semana ", string.Empty) ?? "0");
                    viewModel.ParkingHistory = GetHistory().Where(x => DateUtils.IsDateInWeek(x.Date, weekNumberSelected)).ToList();
                }

                if (ReportTypeSelected.Content.ToString() == "Mensual")
                {
                    int monthNumberSelected = DateUtils.monthNames.ToList().FindIndex(x => x == SecondaryItemSelected) + 1;
                    viewModel.ParkingHistory = GetHistory().Where(x => DateUtils.IsDateInMonth(x.Date, monthNumberSelected)).ToList();
                }
            }


        }

        private void SecondaryFilterCalendarDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var ReportTypeSelected = (ComboBoxItem)ReportType.SelectedValue;
            //Filter Dia
            var SecondaryItemCalendarSelected = SecondaryFilterCalendarDate.SelectedDate;
            if (ReportTypeSelected != null && SecondaryItemCalendarSelected != null)
            {
                if (ReportTypeSelected.Content.ToString() == "Dia")
                {
                    viewModel.ParkingHistory = GetHistory().Where(x => DateUtils.IsDateInDate(x.Date, (DateTime)SecondaryItemCalendarSelected)).ToList();
                }
            }
        }

        private void Report_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel Files|*.xlsx",
                Title = "Guardar Reporte",
                FileName = "Reporte.xlsx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                // Obtener la ruta seleccionada
                string rutaArchivo = saveFileDialog.FileName;

                // Obtener los datos desde la base de datos
                var parkingHistory = viewModel.ParkingHistory;

                // Exportar a Excel
                ExcelUtils.ParkingHistoryExportExcel(parkingHistory, rutaArchivo);
            }
        }
        private void ClearFilter_Click(object sender, RoutedEventArgs e)
        {
            ReportType.SelectedIndex = -1;
            SecondaryFilterComboBox.SelectedIndex = -1;
            SecondaryFilterCalendarDate.SelectedDate = null;
            SecondaryFilterTitle.Text = "";
            SecondaryFilter.Visibility = Visibility.Visible;
            SecondaryFilterDay.Visibility = Visibility.Hidden;
            viewModel.SecondaryFilter = new List<string>();
            viewModel.ParkingHistory = GetHistory();
        }
        private async void SyncArduino_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (serialPort != null && serialPort.IsOpen)
                {
                    // Obtener el valor actual de SpacesAvailable
                    int currentSpacesAvailable = 10 - viewModel.ParkingStatus.SpacesAvailable;

                    // Enviar el valor al Arduino
                    serialPort.WriteLine($"SET_COUNT:{currentSpacesAvailable}");

                    // Espera a que el Arduino responda
                    await Task.Delay(500);

                    // Mostrar mensaje de sincronización exitosa
                    MessageBox.Show("Sincronización exitosa!", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Error: Puerto serial no está abierto o no inicializado.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        #endregion
    }
}