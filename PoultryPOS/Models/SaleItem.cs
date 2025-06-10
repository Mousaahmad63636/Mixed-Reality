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
                if (_grossWeight != value)
                {
                    _grossWeight = value;
                    OnPropertyChanged(nameof(GrossWeight));
                    CalculateValues();
                }
            }
        }

        public int NumberOfCages
        {
            get => _numberOfCages;
            set
            {
                if (_numberOfCages != value)
                {
                    _numberOfCages = value;
                    OnPropertyChanged(nameof(NumberOfCages));
                    CalculateValues();
                }
            }
        }

        public decimal SingleCageWeight
        {
            get => _singleCageWeight;
            set
            {
                if (_singleCageWeight != value)
                {
                    _singleCageWeight = value;
                    OnPropertyChanged(nameof(SingleCageWeight));
                    CalculateValues();
                }
            }
        }

        public decimal InvoicePricePerKg
        {
            get => _invoicePricePerKg;
            set
            {
                if (_invoicePricePerKg != value)
                {
                    _invoicePricePerKg = value;
                    OnPropertyChanged(nameof(InvoicePricePerKg));
                    CalculateValues();
                }
            }
        }

        public decimal TotalCageWeight => NumberOfCages * SingleCageWeight;

        public decimal NetWeight { get; private set; }

        public decimal TotalAmount { get; private set; }

        private void CalculateValues()
        {
            var totalCageWeight = TotalCageWeight;
            NetWeight = Math.Max(0, GrossWeight - totalCageWeight);
            TotalAmount = Math.Max(0, NetWeight * InvoicePricePerKg);

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