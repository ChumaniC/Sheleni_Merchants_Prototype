using Newtonsoft.Json;
using Sheleni_Merchants.Models;
using Sheleni_Merchants.Services;
using Sheleni_Merchants.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Sheleni_Merchants.ViewModels
{
    public class CheckoutPageViewModel : BaseViewModel
    {
        public ICommand ProceedToPaymentCommand { get; }
        public ObservableCollection<Item> ItemsInCart { get; set; }

        public ICommand CheckoutItemFrameTappedCommand { get; }

        public CheckoutPageViewModel()
        {
            Title = "Checkout";
        }

        public CheckoutPageViewModel(List<Item> itemsInCart)
        {
            Title = "Checkout";

            ItemsInCart = new ObservableCollection<Item>(itemsInCart);

            CheckoutItemFrameTappedCommand = new MvvmHelpers.Commands.Command<Item>(OnCheckoutItemFrameTapped);

            TotalAmountDue = CalculateTotalAmount(new ObservableCollection<Item>(itemsInCart));

            ProceedToPaymentCommand = new MvvmHelpers.Commands.Command(ProceedToPayment);

            LoadSelectedItems();
        }

        // After an item is removed from the checkout
        private void OnCheckoutItemFrameTapped(Item selectedItem)
        {
            if (selectedItem != null)
            {
                ItemsInCart.Remove(selectedItem);

                // Remove the selected item from the application properties
                if (Application.Current.Properties.ContainsKey("CheckoutSelectedItems"))
                {
                    var serializedItems = Application.Current.Properties["CheckoutSelectedItems"].ToString();
                    var selectedItems = JsonConvert.DeserializeObject<List<Item>>(serializedItems);

                    var itemToRemove = selectedItems.FirstOrDefault(i => i.ItemName == selectedItem.ItemName);
                    if (itemToRemove != null)
                    {
                        selectedItems.Remove(itemToRemove);
                        var updatedSerializedItems = JsonConvert.SerializeObject(selectedItems);
                        Application.Current.Properties["CheckoutSelectedItems"] = updatedSerializedItems;
                        Application.Current.SavePropertiesAsync();
                    }
                }

                SaveSelectedItems();
                UpdateTotalAmount();
                DataService.RemoveItem(selectedItem);
                OnPropertyChanged(nameof(ItemsInCart)); // Notify that ItemsInCart collection has changed
            }
        }

        private void LoadSelectedItems()
        {
            if (Application.Current.Properties.ContainsKey("CheckoutSelectedItems"))
            {
                var serializedItems = Application.Current.Properties["CheckoutSelectedItems"].ToString();
                var selectedItems = JsonConvert.DeserializeObject<List<Item>>(serializedItems);

                if (selectedItems != null && selectedItems.Any())
                {
                    foreach (var selectedItem in selectedItems)
                    {
                        var existingItem = ItemsInCart.FirstOrDefault(i => i.ItemName == selectedItem.ItemName);
                        if (existingItem != null)
                        {
                            // Update the current quantity of the existing item
                            existingItem.CurrentQuantity = selectedItem.CurrentQuantity;
                        }
                        else
                        {
                            // Add the item to the cart if it doesn't exist
                            ItemsInCart.Add(selectedItem);
                        }
                    }
                }
                TotalAmountDue = CalculateTotalAmount(ItemsInCart);
            }
        }

        public void SaveSelectedItems()
        {
            var selectedItems = ItemsInCart.Where(item => item.CurrentQuantity > 0).ToList();
            var serializedItems = JsonConvert.SerializeObject(selectedItems);

            Application.Current.Properties["CheckoutSelectedItems"] = serializedItems;
            Application.Current.SavePropertiesAsync(); // Save the properties
        }

        private async void ProceedToPayment()
        {
            // Add any logic you need before navigating to the payment page

            // Pass the purchased items and total price to the RequestToPayPage
            var requestToPayPageViewModel = new RequestToPayPageViewModel(ItemsInCart, TotalAmountDue);
            var requestToPayPage = new RequestToPayPage(requestToPayPageViewModel);

            // Navigate to the RequestToPayPage
            await Application.Current.MainPage.Navigation.PushAsync(requestToPayPage);
        }

        // Override the default OnAppearing and OnDisappearing methods in the CheckoutPage code-behind
        public void OnAppearing()
        {
            // Load selected items when the page appears
            LoadSelectedItems();
        }

        public async Task OnDisappearing()
        {
            // Save selected items when the page disappears (when the user navigates away)
            SaveSelectedItems();
        }

        public double TotalAmountDue { get; private set; }

        private double CalculateTotalAmount(ObservableCollection<Item> itemsInCart)
        {
            try
            {
                return itemsInCart.Sum(item => item.CurrentPrice);
            }
            catch (Exception ex)
            {
                return 0.0;
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
                TotalAmountDue = 0.0;
            }
            // Notify the UI that the TotalAmountDue property has changed
            OnPropertyChanged(nameof(TotalAmountDue));
        }
    }
}