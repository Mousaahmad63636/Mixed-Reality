﻿using PoultryPOS.Models;
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
                txtNetWeight.Text = truck.NetWeight.ToString("F2");
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
                MessageBox.Show("يرجى إدخال اسم الشاحنة.", "خطأ في التحقق", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtCurrentLoad.Text, out int currentLoad) || currentLoad < 0)
            {
                MessageBox.Show("يرجى إدخال حمولة حالية صالحة.", "خطأ في التحقق", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtNetWeight.Text, out decimal netWeight) || netWeight < 0)
            {
                MessageBox.Show("يرجى إدخال وزن صافي صالح.", "خطأ في التحقق", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var truck = new Truck
                {
                    Name = txtTruckName.Text.Trim(),
                    PlateNumber = string.IsNullOrWhiteSpace(txtPlateNumber.Text) ? null : txtPlateNumber.Text.Trim(),
                    CurrentLoad = currentLoad,
                    NetWeight = netWeight
                };

                _truckService.Add(truck);
                LoadTrucks();
                ClearTruckForm();
                MessageBox.Show("تم إضافة الشاحنة بنجاح!", "نجح", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في إضافة الشاحنة: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnUpdateTruck_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedTruck == null)
            {
                MessageBox.Show("يرجى اختيار شاحنة للتحديث.", "خطأ في التحديد", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTruckName.Text))
            {
                MessageBox.Show("يرجى إدخال اسم الشاحنة.", "خطأ في التحقق", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtCurrentLoad.Text, out int currentLoad) || currentLoad < 0)
            {
                MessageBox.Show("يرجى إدخال حمولة حالية صالحة.", "خطأ في التحقق", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtNetWeight.Text, out decimal netWeight) || netWeight < 0)
            {
                MessageBox.Show("يرجى إدخال وزن صافي صالح.", "خطأ في التحقق", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                _selectedTruck.Name = txtTruckName.Text.Trim();
                _selectedTruck.PlateNumber = string.IsNullOrWhiteSpace(txtPlateNumber.Text) ? null : txtPlateNumber.Text.Trim();
                _selectedTruck.CurrentLoad = currentLoad;
                _selectedTruck.NetWeight = netWeight;

                _truckService.Update(_selectedTruck);
                LoadTrucks();
                ClearTruckForm();
                MessageBox.Show("تم تحديث الشاحنة بنجاح!", "نجح", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحديث الشاحنة: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDeleteTruck_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedTruck == null)
            {
                MessageBox.Show("يرجى اختيار شاحنة للحذف.", "خطأ في التحديد", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"هل أنت متأكد من حذف الشاحنة '{_selectedTruck.Name}'؟",
                                       "تأكيد الحذف", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _truckService.Delete(_selectedTruck.Id);
                    LoadTrucks();
                    ClearTruckForm();
                    MessageBox.Show("تم حذف الشاحنة بنجاح!", "نجح", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"خطأ في حذف الشاحنة: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
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
            txtNetWeight.Text = "0";
            _selectedTruck = null;
            dgTrucks.SelectedItem = null;
        }

        private void BtnAddDriver_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDriverName.Text))
            {
                MessageBox.Show("يرجى إدخال اسم السائق.", "خطأ في التحقق", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                MessageBox.Show("تم إضافة السائق بنجاح!", "نجح", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في إضافة السائق: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnUpdateDriver_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedDriver == null)
            {
                MessageBox.Show("يرجى اختيار سائق للتحديث.", "خطأ في التحديد", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDriverName.Text))
            {
                MessageBox.Show("يرجى إدخال اسم السائق.", "خطأ في التحقق", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                _selectedDriver.Name = txtDriverName.Text.Trim();
                _selectedDriver.Phone = string.IsNullOrWhiteSpace(txtDriverPhone.Text) ? null : txtDriverPhone.Text.Trim();

                _driverService.Update(_selectedDriver);
                LoadDrivers();
                ClearDriverForm();
                MessageBox.Show("تم تحديث السائق بنجاح!", "نجح", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحديث السائق: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDeleteDriver_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedDriver == null)
            {
                MessageBox.Show("يرجى اختيار سائق للحذف.", "خطأ في التحديد", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"هل أنت متأكد من حذف السائق '{_selectedDriver.Name}'؟",
                                       "تأكيد الحذف", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _driverService.Delete(_selectedDriver.Id);
                    LoadDrivers();
                    ClearDriverForm();
                    MessageBox.Show("تم حذف السائق بنجاح!", "نجح", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"خطأ في حذف السائق: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
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