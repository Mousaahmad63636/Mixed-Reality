using PoultryPOS.Models;
using PoultryPOS.Services;
using System.Windows;
using System.Windows.Controls;

namespace PoultryPOS.Views
{
    public partial class TruckDriverView : Page
    {
        private readonly TruckService _truckService;
        private readonly DriverService _driverService;
        private Truck _selectedTruck;
        private Driver _selectedDriver;

        public TruckDriverView()
        {
            InitializeComponent();
            _truckService = new TruckService();
            _driverService = new DriverService();
            LoadData();
        }

        private void LoadData()
        {
            LoadTrucks();
            LoadDrivers();
        }

        private void LoadTrucks()
        {
            dgTrucks.ItemsSource = _truckService.GetAll();
        }

        private void LoadDrivers()
        {
            dgDrivers.ItemsSource = _driverService.GetAll();
        }

        private void DgTrucks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgTrucks.SelectedItem is Truck truck)
            {
                _selectedTruck = truck;
                txtTruckName.Text = truck.Name;
                txtPlateNumber.Text = truck.PlateNumber ?? "";
                txtCurrentLoad.Text = truck.CurrentLoad.ToString();
            }
        }

        private void DgDrivers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgDrivers.SelectedItem is Driver driver)
            {
                _selectedDriver = driver;
                txtDriverName.Text = driver.Name;
                txtDriverPhone.Text = driver.Phone ?? "";
            }
        }

        private void BtnAddTruck_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTruckName.Text))
            {
                MessageBox.Show("Please enter truck name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtCurrentLoad.Text, out int currentLoad) || currentLoad < 0)
            {
                MessageBox.Show("Please enter a valid current load.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var truck = new Truck
                {
                    Name = txtTruckName.Text.Trim(),
                    PlateNumber = string.IsNullOrWhiteSpace(txtPlateNumber.Text) ? null : txtPlateNumber.Text.Trim(),
                    CurrentLoad = currentLoad
                };

                _truckService.Add(truck);
                LoadTrucks();
                ClearTruckForm();
                MessageBox.Show("Truck added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding truck: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnUpdateTruck_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedTruck == null)
            {
                MessageBox.Show("Please select a truck to update.", "Selection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTruckName.Text))
            {
                MessageBox.Show("Please enter truck name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtCurrentLoad.Text, out int currentLoad) || currentLoad < 0)
            {
                MessageBox.Show("Please enter a valid current load.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                _selectedTruck.Name = txtTruckName.Text.Trim();
                _selectedTruck.PlateNumber = string.IsNullOrWhiteSpace(txtPlateNumber.Text) ? null : txtPlateNumber.Text.Trim();
                _selectedTruck.CurrentLoad = currentLoad;

                _truckService.Update(_selectedTruck);
                LoadTrucks();
                ClearTruckForm();
                MessageBox.Show("Truck updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating truck: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDeleteTruck_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedTruck == null)
            {
                MessageBox.Show("Please select a truck to delete.", "Selection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete truck '{_selectedTruck.Name}'?",
                                       "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _truckService.Delete(_selectedTruck.Id);
                    LoadTrucks();
                    ClearTruckForm();
                    MessageBox.Show("Truck deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting truck: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnClearTruck_Click(object sender, RoutedEventArgs e)
        {
            ClearTruckForm();
        }

        private void ClearTruckForm()
        {
            txtTruckName.Clear();
            txtPlateNumber.Clear();
            txtCurrentLoad.Text = "0";
            _selectedTruck = null;
            dgTrucks.SelectedItem = null;
        }

        private void BtnAddDriver_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDriverName.Text))
            {
                MessageBox.Show("Please enter driver name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var driver = new Driver
                {
                    Name = txtDriverName.Text.Trim(),
                    Phone = string.IsNullOrWhiteSpace(txtDriverPhone.Text) ? null : txtDriverPhone.Text.Trim()
                };

                _driverService.Add(driver);
                LoadDrivers();
                ClearDriverForm();
                MessageBox.Show("Driver added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding driver: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnUpdateDriver_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedDriver == null)
            {
                MessageBox.Show("Please select a driver to update.", "Selection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDriverName.Text))
            {
                MessageBox.Show("Please enter driver name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                _selectedDriver.Name = txtDriverName.Text.Trim();
                _selectedDriver.Phone = string.IsNullOrWhiteSpace(txtDriverPhone.Text) ? null : txtDriverPhone.Text.Trim();

                _driverService.Update(_selectedDriver);
                LoadDrivers();
                ClearDriverForm();
                MessageBox.Show("Driver updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating driver: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDeleteDriver_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedDriver == null)
            {
                MessageBox.Show("Please select a driver to delete.", "Selection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete driver '{_selectedDriver.Name}'?",
                                       "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _driverService.Delete(_selectedDriver.Id);
                    LoadDrivers();
                    ClearDriverForm();
                    MessageBox.Show("Driver deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting driver: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnClearDriver_Click(object sender, RoutedEventArgs e)
        {
            ClearDriverForm();
        }

        private void ClearDriverForm()
        {
            txtDriverName.Clear();
            txtDriverPhone.Clear();
            _selectedDriver = null;
            dgDrivers.SelectedItem = null;
        }
    }
}