using System.ComponentModel;

namespace PoultryPOS.Models
{
    public class SaleItem : INotifyPropertyChanged
    {
        private decimal _grossWeight;
        private int _numberOfCages;
        private decimal _singleCageWeight;
        private decimal _invoicePricePerKg;

        public decimal GrossWeight
        {
            get => _grossWeight;
            set
            {
                _grossWeight = value;
                OnPropertyChanged(nameof(GrossWeight));
                CalculateValues();
            }
        }

        public int NumberOfCages
        {
            get => _numberOfCages;
            set
            {
                _numberOfCages = value;
                OnPropertyChanged(nameof(NumberOfCages));
                CalculateValues();
            }
        }

        public decimal SingleCageWeight
        {
            get => _singleCageWeight;
            set
            {
                _singleCageWeight = value;
                OnPropertyChanged(nameof(SingleCageWeight));
                CalculateValues();
            }
        }

        public decimal InvoicePricePerKg
        {
            get => _invoicePricePerKg;
            set
            {
                _invoicePricePerKg = value;
                OnPropertyChanged(nameof(InvoicePricePerKg));
                CalculateValues();
            }
        }

        public decimal TotalCageWeight => NumberOfCages * SingleCageWeight;

        public decimal NetWeight { get; private set; }

        public decimal TotalAmount { get; private set; }

        private void CalculateValues()
        {
            var totalCageWeight = TotalCageWeight;
            NetWeight = GrossWeight - totalCageWeight;
            TotalAmount = NetWeight * InvoicePricePerKg;

            OnPropertyChanged(nameof(TotalCageWeight));
            OnPropertyChanged(nameof(NetWeight));
            OnPropertyChanged(nameof(TotalAmount));
        }

        public void UpdatePrice(decimal pricePerKg)
        {
            InvoicePricePerKg = pricePerKg;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}