using PoultryPOS.Models;
using PoultryPOS.Services;
using System.Windows;
using System.Windows.Controls;

namespace PoultryPOS.Views
{
    public partial class VarianceReportView : Page
    {
        private readonly TruckLoadingSessionService _loadingSessionService;
        private readonly TruckService _truckService;
        private List<TruckLoadingSession> _allSessions;

        public VarianceReportView()
        {
            InitializeComponent();
            _loadingSessionService = new TruckLoadingSessionService();
            _truckService = new TruckService();

            LoadData();
            LoadVarianceReport();
        }

        private void LoadData()
        {
            var trucks = _truckService.GetAll();
            cmbTruckFilter.Items.Clear();
            cmbTruckFilter.Items.Add(new ComboBoxItem { Content = "جميع الشاحنات", Tag = -1 });

            foreach (var truck in trucks)
            {
                cmbTruckFilter.Items.Add(new ComboBoxItem { Content = truck.Name, Tag = truck.Id });
            }

            cmbTruckFilter.SelectedIndex = 0;
        }

        private void LoadVarianceReport()
        {
            _allSessions = _loadingSessionService.GetAllSessions();
            dgVarianceReport.ItemsSource = _allSessions;
            UpdateStatistics(_allSessions);
        }

        private void UpdateStatistics(List<TruckLoadingSession> sessions)
        {
            var completedSessions = sessions.Where(s => s.IsCompleted).ToList();
            var totalVariance = completedSessions.Sum(s => s.WeightVariance ?? 0);
            var averageVariance = completedSessions.Count > 0 ? totalVariance / completedSessions.Count : 0;

            lblTotalSessions.Text = sessions.Count.ToString();
            lblCompletedSessions.Text = completedSessions.Count.ToString();
            lblTotalVariance.Text = totalVariance.ToString("F2");
            lblAverageVariance.Text = averageVariance.ToString("F2");
        }

        private void FilterVarianceReport(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            if (_allSessions == null) return;

            var filteredSessions = _allSessions.AsEnumerable();

            if (dpFromDate.SelectedDate.HasValue)
            {
                filteredSessions = filteredSessions.Where(s => s.LoadDate.Date >= dpFromDate.SelectedDate.Value.Date);
            }

            if (dpToDate.SelectedDate.HasValue)
            {
                filteredSessions = filteredSessions.Where(s => s.LoadDate.Date <= dpToDate.SelectedDate.Value.Date);
            }

            if (cmbTruckFilter.SelectedItem is ComboBoxItem truckItem && (int)truckItem.Tag != -1)
            {
                var selectedTruckId = (int)truckItem.Tag;
                filteredSessions = filteredSessions.Where(s => s.TruckId == selectedTruckId);
            }

            var filteredList = filteredSessions.ToList();
            dgVarianceReport.ItemsSource = filteredList;
            UpdateStatistics(filteredList);
        }

        private void BtnToday_Click(object sender, RoutedEventArgs e)
        {
            dpFromDate.SelectedDate = DateTime.Today;
            dpToDate.SelectedDate = DateTime.Today;
        }

        private void BtnClearFilter_Click(object sender, RoutedEventArgs e)
        {
            dpFromDate.SelectedDate = null;
            dpToDate.SelectedDate = null;
            cmbTruckFilter.SelectedIndex = 0;
            LoadVarianceReport();
        }
    }
}