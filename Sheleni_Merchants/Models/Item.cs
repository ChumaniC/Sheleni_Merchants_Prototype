using MvvmHelpers;
using System;
using System.Windows.Input;

namespace Sheleni_Merchants.Models
{
    public class Item : ObservableObject
    {
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Subcategory { get; set; }
        public int Quantity { get; set; }
        public string Price { get; set; }
        public string ItemIcon { get; set; }
        public string SubcategoryIcon { get; set; }

        public bool IsSelectedForCheckout { get; set; }

        private int _currentQuantity;
        public int CurrentQuantity
        {
            get { return _currentQuantity; }
            set
            {
                if (_currentQuantity != value)
                {
                    _currentQuantity = value;
                    CalculateCurrentPrice();
                    OnPropertyChanged();
                }
            }
        }

        private decimal _currentPrice;
        public decimal CurrentPrice
        {
            get { return _currentPrice; }
            set
            {
                if (_currentPrice != value)
                {
                    _currentPrice = value;
                    OnPropertyChanged();
                }
            }
        }

        private void CalculateCurrentPrice()
        {
            // Remove "R" and parse the Price
            if (decimal.TryParse(Price.Replace("R", ""), out decimal priceValue))
            {
                CurrentPrice = priceValue * CurrentQuantity;
            }
        }
    }
}





