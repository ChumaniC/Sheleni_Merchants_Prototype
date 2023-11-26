using MvvmHelpers;
using MvvmHelpers.Interfaces;
using Newtonsoft.Json;
using Sheleni_Merchants.Models;
using Sheleni_Merchants.Services;
using Sheleni_Merchants.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Sheleni_Merchants.ViewModels
{
    public class BuyPageViewModel : BaseViewModel
    {
        private readonly SheleniHttpClient _httpClient;

        public ICommand ItemFrameTappedCommand { get; }
        public ICommand NavigateToPayAndBuyCommand { get; }

        public ObservableCollection<Item> ItemsInCart { get; set; }

        public BuyPageViewModel()
        {
            Title = "Add To Cart";

            Items = new ObservableCollection<Item>();

            SelectedItems = new ObservableCollection<Item>();

            ItemsInCart = new ObservableCollection<Item>();

            ItemFrameTappedCommand = new MvvmHelpers.Commands.Command<Service>(OnItemSelected);

            IncrementQuantityCommand = new MvvmHelpers.Commands.Command<Item>(IncrementQuantity);

            DecrementQuantityCommand = new MvvmHelpers.Commands.Command<Item>(DecrementQuantity);

            CheckoutCommand = new MvvmHelpers.Commands.AsyncCommand(CheckoutAsync);
            
            NavigateToPayAndBuyCommand = new MvvmHelpers.Commands.Command(NavigateToPayAndBuy);

            DataService.ItemRemoved += (sender, item) => ResetQuantityInBuyPage(item);

            // Initialize the HttpClient
            _httpClient = new SheleniHttpClient();

            // Load service names when the ViewModel is constructed (you can do this differently based on your app's logic)
            LoadItems();

            // Load previously selected items when the BuyPage is instantiated
            LoadSelectedItems();
        }

        private async void NavigateToPayAndBuy()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new PayAndBuyPage());
        }

        private int merchantId;

        public int MerchantId
        {
            get { return merchantId; }
            set { SetProperty(ref merchantId, value); }
        }

        private string merchantName;

        public string MerchantName
        {
            get { return merchantName; }
            set { SetProperty(ref merchantName, value); }
        }

        private ObservableCollection<Item> filteredItems;

        public ObservableCollection<Item> FilteredItems
        {
            get { return filteredItems; }
            set { SetProperty(ref filteredItems, value); }
        }

        private string searchedItem;

        public string SearchedItem
        {
            get { return searchedItem; }
            set
            {
                if (SetProperty(ref searchedItem, value))
                {
                    FilterItems();
                }
            }
        }

        private void FilterItems()
        {
            if (string.IsNullOrWhiteSpace(SearchedItem))
            {
                // If the search bar is empty, show all items
                FilteredItems = Items;
            }
            else
            {
                // Filter items based on the search criteria
                FilteredItems = new ObservableCollection<Item>(
                    Items.Where(item => item.ItemName.ToLower().Contains(SearchedItem.ToLower()))
                );
            }
        }
        private void ResetQuantityInBuyPage(Item deletedItem)
        {
            var itemInBuyPage = Items.FirstOrDefault(item => item.ItemName == deletedItem.ItemName);

            if (itemInBuyPage != null)
            {
                itemInBuyPage.CurrentQuantity = 0;
                OnPropertyChanged(nameof(Items));
            }
        }

        private void LoadSelectedItems()
        {
            if (Application.Current.Properties.ContainsKey("SelectedItems"))
            {
                var serializedItems = Application.Current.Properties["SelectedItems"].ToString();
                var selectedItems = JsonConvert.DeserializeObject<List<Item>>(serializedItems);

                if (selectedItems != null && selectedItems.Any())
                {
                    foreach (var selectedItem in selectedItems)
                    {
                        var existingItem = Items.FirstOrDefault(i => i.ItemName == selectedItem.ItemName);
                        if (existingItem != null)
                        {
                            // Update the current quantity of the existing item
                            existingItem.CurrentQuantity = selectedItem.CurrentQuantity;
                        }
                        else
                        {
                            // Add the item to the list if it doesn't exist
                            SelectedItems.Add(selectedItem);
                        }
                    }
                }
            }
        }

        private void SaveSelectedItems()
        {
            var selectedItems = Items.Where(item => item.CurrentQuantity > 0).ToList();
            if (selectedItems.Any())
            {
                try
                {
                    var serializedItems = JsonConvert.SerializeObject(selectedItems);
                    Application.Current.Properties["SelectedItems"] = serializedItems;
                    Application.Current.SavePropertiesAsync(); // Save the properties
                }
                catch (Exception ex)
                {
                    // Handle any potential serialization exceptions
                    Console.WriteLine("Serialization error: " + ex.Message);
                }
            }
        }

        // Override the default OnDisappearing method in the BuyPage code-behind
        public async Task OnDisappearing()
        {
            // Save selected items when the page disappears (when the user navigates away)
            SaveSelectedItems();
        }

        private void IncrementQuantity(Item item)
        {
            if (item != null)
            {
                item.CurrentQuantity++;
                ItemsInCart.Add(item);
                UpdateCartCount();
                UpdateTotalAmount();
            }
        }

        private void DecrementQuantity(Item item)
        {
            if (item != null && item.CurrentQuantity > 0)
            {
                item.CurrentQuantity--;
                ItemsInCart.Remove(item);
                UpdateCartCount();
                UpdateTotalAmount();
            }
        }


        private async Task CheckoutAsync()
        {
            // Create a list of items with quantity greater than 0
            var itemsInCart = Items.Where(item => item.CurrentQuantity > 0).ToList();

            // Calculate CurrentPrice for each item before sending to CheckoutPage
            foreach (var item in itemsInCart)
            {
                string price = item.Price;
                decimal currentPrice;

                if (price.StartsWith("R"))
                {
                    // Remove the leading "R" character before converting to double
                    if (decimal.TryParse(price.Substring(1), out currentPrice))
                    {
                        item.CurrentPrice = currentPrice * item.CurrentQuantity; 
                        
                    }
                    else
                    {
                        // Handling case where conversion fails
                        // For instance, invalid number format after "R"
                    }
                }
            }

            var checkoutPageViewModel = new CheckoutPageViewModel(itemsInCart);
            checkoutPageViewModel.MerchantId = merchantId;
            checkoutPageViewModel.MerchantName = merchantName;
            await Application.Current.MainPage.Navigation.PushAsync(new CheckoutPage(checkoutPageViewModel));
        }
        private void UpdateCartCount()
        {
            // Calculate the total quantity in the cart by summing the quantities of all items
            int totalQuantity = Items.Sum(item => item.CurrentQuantity);

            // Update the cart label/icon text with the total quantity
            CartCount = totalQuantity;
        }

        private int cartCount;

        public int CartCount
        {
            get { return cartCount; }
            set { SetProperty(ref cartCount, value); }
        }

        // Method adds the selected item to the cart for the merchant to compute the total amount due from the customer
        private void OnItemSelected(Service service)
        {
            throw new NotImplementedException();
        }

        private Item selectedItem;

        public Item SelectedItem
        {
            get { return selectedItem; }
            set { SetProperty(ref selectedItem, value); }
        }

        public ObservableCollection<Item> Items { get; set; }
        public ObservableCollection<Item> SelectedItems { get; set; }
        public ObservableCollection<Grouping<string, Item>> ItemCategories { get; set; }

        public ICommand IncrementQuantityCommand { get; }
        public ICommand DecrementQuantityCommand { get; }
        public IAsyncCommand CheckoutCommand { get; }

        private void LoadItems()
        {
            FilteredItems = Items;

            // Open database connection
            DB_Connection conn = new DB_Connection();
            SqlConnection dbConn = conn.Sheleni_Db_Connection();

            // Retrieve location names from database
            string selectItemQuery = "SELECT * FROM dbo.Inventory";
            SqlCommand commandItem = new SqlCommand(selectItemQuery, dbConn);
            SqlDataReader readerItem = commandItem.ExecuteReader();

            int _itemID;
            string _itemName;
            string _description;
            string _subcategory;
            string _price;

            // Retrieve Item Information
            while (readerItem.Read())
            {
                _itemID = (int)readerItem["ItemID"];
                _itemName = readerItem["ItemName"].ToString();
                _description = readerItem["Description"].ToString();
                _subcategory = readerItem["Subcategory"].ToString();
                _price = readerItem["UnitPrice"].ToString();
                Items.Add(new Item
                {
                    ItemID = _itemID,
                    ItemName = _itemName,
                    Description = _description,
                    Subcategory = _subcategory,
                    Price = _price,
                    ItemIcon = _itemName.ToLower() + ".png"
                });
            }
            //RefreshCommand = new AsyncCommand(Refresh);
            readerItem.Close();

            dbConn.Close();
        }

        public decimal TotalAmountDue { get; private set; }

        private decimal CalculateTotalAmount(ObservableCollection<Item> itemsInCart)
        {
            try
            {
                return itemsInCart.Sum(item => item.CurrentPrice);
            }
            catch (Exception ex)
            {
                return Convert.ToDecimal(0.00);
            }
        }

        private void UpdateTotalAmount()
        {
            try
            {
                TotalAmountDue = CalculateTotalAmount(ItemsInCart);
            }
            catch (Exception ex)
            {
                TotalAmountDue = Convert.ToDecimal(0.00);
            }
            // Notify the UI that the TotalAmountDue property has changed
            OnPropertyChanged(nameof(TotalAmountDue));
        }
    }
}