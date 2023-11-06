using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;
using Sheleni_Merchants.Models;
using Sheleni_Merchants.Services;
using System.Linq;
using System.Threading.Tasks;
using ZXing.Net.Mobile.Forms;
using System.Collections.ObjectModel;

namespace Sheleni_Merchants.ViewModels
{
    public class RequestToPayPageViewModel : BaseViewModel
    {
        private SheleniHttpClient SheleniClient;

        public ObservableCollection<Item> PurchasedItems { get; set; }
        public double TotalPrice { get; set; }
        public ZXingBarcodeImageView QrCodeImage { get; set; }

        public ICommand ExecutePaymentCommand { get; set; }

        public RequestToPayPageViewModel()
        {
            InitializeViewModel();
        }

        public RequestToPayPageViewModel(ObservableCollection<Item> purchasedItems, double totalPrice)
        {
            InitializeViewModel();

            Title = "Payment";

            PurchasedItems = new ObservableCollection<Item>(purchasedItems);
            TotalPrice = totalPrice;

            // Generate the QR code
            QrCodeImage = GenerateQrCode();
        }

        private void InitializeViewModel()
        {
            SheleniClient = new SheleniHttpClient(); // Initialize your SheleniHttpClient here

            // Initialize the payment command
            ExecutePaymentCommand = new Command(async () => await ExecutePaymentAsync());
        }

        public ZXingBarcodeImageView GenerateQrCode()
        {
            var qrCodeData = $"Items: {string.Join(", ", PurchasedItems.Select(item => item.ItemName))}, Total: R{TotalPrice:0.00}";

            var qrCodeImage = new ZXingBarcodeImageView
            {
                BarcodeFormat = ZXing.BarcodeFormat.QR_CODE,
                BarcodeValue = qrCodeData
            };

            return qrCodeImage;
        }

        private async Task ExecutePaymentAsync()
        {
            try
            {
                // Replace these with actual data
                string customerMobileNumber = "0792949804"; // Replace with actual mobile number
              //decimal purchaseAmount = Convert.ToDecimal(PurchasedItems.Select(item => item.Price)); // Replace with actual amount
                decimal purchaseAmount = 100.00m; // Replace with actual amount

                string paymentResult = await SheleniClient.RequestToPayAsync(customerMobileNumber,purchaseAmount);

                // Handle the payment result
                if (!string.IsNullOrEmpty(paymentResult))
                {
                    // Payment was successful
                    DisplayPaymentSuccessMessage();
                }
                else
                {
                    // Payment failed or there was an issue
                    DisplayPaymentFailureMessage();
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                DisplayPaymentErrorMessage(ex.Message);
            }
        }

        private void DisplayPaymentSuccessMessage()
        {
            // Display a success message to the user
            string successMessage = "Payment successful!";
            DisplayAlert("Payment Result", successMessage, "OK");
        }

        private void DisplayPaymentFailureMessage()
        {
            // Display a failure message to the user
            string failureMessage = "Payment failed. Please try again.";
            DisplayAlert("Payment Result", failureMessage, "OK");
        }

        private void DisplayPaymentErrorMessage(string errorMessage)
        {
            // Display an error message to the user
            DisplayAlert("Payment Error", errorMessage, "OK");
        }

        private async void DisplayAlert(string title, string message, string cancel)
        {
            await Application.Current.MainPage.DisplayAlert(title, message, cancel);
        }
    }
}
