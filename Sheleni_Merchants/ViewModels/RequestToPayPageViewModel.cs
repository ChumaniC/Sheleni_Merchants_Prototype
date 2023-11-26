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
using System.Data.SqlClient;
using Sheleni_Merchants.Views;
using Rg.Plugins.Popup.Services;

namespace Sheleni_Merchants.ViewModels
{
    public class RequestToPayPageViewModel : BaseViewModel
    {
        private SheleniHttpClient SheleniClient;

        public ObservableCollection<Item> PurchasedItems { get; set; }
        public decimal TotalPrice { get; set; }
        public ZXingBarcodeImageView QrCodeImage { get; set; }

        public ICommand ExecutePaymentCommand { get; set; }

        public ICommand NavigateToPayAndBuyCommand { get; }

        public RequestToPayPageViewModel()
        {
            InitializeViewModel();
        }

        public RequestToPayPageViewModel(ObservableCollection<Item> purchasedItems, decimal totalPrice)
        {
            InitializeViewModel();

            Title = "Payment";

            NavigateToPayAndBuyCommand = new MvvmHelpers.Commands.Command(NavigateToPayAndBuy);

            PurchasedItems = new ObservableCollection<Item>(purchasedItems);
            InventoryItem = new Item();
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

        private string description;

        public string Description
        {
            get { return description; }
            set { SetProperty(ref description, value); }
        }

        public DateTime TransactionDate { get; set; } = DateTime.Now;


        private TimeSpan transactionTime;

        public TimeSpan TransactionTime
        {
            get { return transactionTime; }
            set { SetProperty(ref transactionTime, value); }
        }

        private string transactionType;

        public string TransactionType
        {
            get { return transactionType; }
            set { SetProperty(ref transactionType, value); }
        }

        private int customerId;

        public int CustomerId
        {
            get { return customerId; }
            set { SetProperty(ref customerId, value); }
        }

        private int income;

        public int Income
        {
            get { return income; }
            set { SetProperty(ref income, value); }
        }

        public Item InventoryItem { get; set; }


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
                decimal purchaseAmount = TotalPrice; // Replace with actual amount

                string paymentResult = await SheleniClient.RequestToPayAsync(customerMobileNumber,purchaseAmount);

                // Handle the payment result
                if (!string.IsNullOrEmpty(paymentResult))
                {
                    // Payment was successful
                    TransactionTime = DateTime.Now.TimeOfDay;
                    Description = "Purchasing of various goods and services";
                    TransactionType = "Purchase Transaction";
                    CustomerId = 1;
                    Income = 1;
                    InsertMerchantTransaction(MerchantId, TotalPrice, TransactionDate, TransactionTime,
                        Description, TransactionType, CustomerId, Income);
                    UpdateWalletBalance(MerchantId, TotalPrice);
                    UpdateInventoryQuantities();
                    Application.Current.Properties.Remove("SelectedItems");
                    Application.Current.Properties.Remove("CheckoutSelectedItems");
                    await DisplayCustomAlert("success.png", "Payment Result", "Payment was successful.");
                    NavigateToDashboard();
                }
                else
                {
                    // Payment failed or there was an issue
                    await DisplayCustomAlert("error.png", "Payment Result", "Payment was unsuccessful.");
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                await DisplayCustomAlert("error.png", "Payment Error", ex.Message);
            }
        }

        public void InsertMerchantTransaction(int merchantId, 
            decimal amount, DateTime transactionDate, TimeSpan transactionTime,
            string description, string transactionType, int customerId, decimal income)
        {
            DB_Connection con = new DB_Connection();

            try
            {
                using (SqlConnection connection = con.Sheleni_Db_Connection())
                {

                    string insertQuery = @"
                    INSERT INTO MerchantTransactionHistory 
                    (merchant_id, amount, transaction_date, transaction_time, description, transaction_type, customer_id, income)
                    VALUES 
                    (@MerchantId, @Amount, @Transaction_date, @Transaction_time, @Description, @TransactionType, @CustomerId, @Income)";

                    using (SqlCommand command = new SqlCommand(insertQuery, connection))
                    {
                        // Add parameters to the SQL query
                        command.Parameters.AddWithValue("@MerchantId", merchantId);
                        command.Parameters.AddWithValue("@Amount", amount);
                        command.Parameters.AddWithValue("@Transaction_date", transactionDate);
                        command.Parameters.AddWithValue("@Transaction_time", transactionTime);
                        command.Parameters.AddWithValue("@Description", description);
                        command.Parameters.AddWithValue("@TransactionType", transactionType);
                        command.Parameters.AddWithValue("@CustomerId", customerId);
                        command.Parameters.AddWithValue("@Income", income);

                        // Execute the SQL query
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                Console.WriteLine($"Error inserting into MerchantTransactionHistory: {ex.Message}");
            }
        }

        public void UpdateWalletBalance(int ownerId, decimal totalPrice)
        {
            DB_Connection con = new DB_Connection();

            try
            {
                using (SqlConnection connection = con.Sheleni_Db_Connection())
                {
                    // Fetch the current balance for the specified owner_id
                    string selectBalanceQuery = "SELECT balance FROM MerchantWallet WHERE owner_id = @ownerId";
                    using (SqlCommand selectBalanceCommand = new SqlCommand(selectBalanceQuery, connection))
                    {
                        selectBalanceCommand.Parameters.AddWithValue("@ownerId", ownerId);

                        // Execute the query to get the current balance
                        decimal currentBalance = (decimal)selectBalanceCommand.ExecuteScalar();

                        // Calculate the new balance after subtracting the totalPrice

                        // WHEN SCALING CHECK FOR SUFFICIENT FUNDS
                        decimal newBalance = currentBalance + totalPrice;

                        // Update the balance in the database
                        string updateBalanceQuery = "UPDATE MerchantWallet SET balance = @newBalance WHERE owner_id = @ownerId";
                        using (SqlCommand updateBalanceCommand = new SqlCommand(updateBalanceQuery, connection))
                        {
                            updateBalanceCommand.Parameters.AddWithValue("@newBalance", newBalance);
                            updateBalanceCommand.Parameters.AddWithValue("@ownerId", ownerId);

                            // Execute the update query
                            updateBalanceCommand.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception as needed (e.g., log the error)
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        public void UpdateInventoryQuantities()
        {
            foreach (var purchasedItem in PurchasedItems)
            {
                if (purchasedItem.ItemIcon != "bundle.png")
                {
                    // Find the corresponding item in the inventory
                    var inventoryItem = GetInventoryItemById(purchasedItem.ItemID);

                    // Update the quantity in the inventory
                    if (inventoryItem != null)
                    {
                        inventoryItem.Quantity -= purchasedItem.CurrentQuantity;
                        UpdateInventoryItemQuantity(inventoryItem.Quantity, purchasedItem.ItemID);
                    }
                }
            }
        }

        public Item GetInventoryItemById(int itemId)
        {
           DB_Connection con = new DB_Connection();

            try
            {
                using (SqlConnection connection = con.Sheleni_Db_Connection())
                {

                    string selectQuery = "SELECT * FROM Inventory WHERE ItemID = @ItemId";
                    using (SqlCommand command = new SqlCommand(selectQuery, connection))
                    {
                        command.Parameters.AddWithValue("@ItemId", itemId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Item class with properties matching your table columns

                                InventoryItem.ItemID = Convert.ToInt32(reader["ItemID"]);
                                InventoryItem.Quantity = Convert.ToInt32(reader["Quantity"]);
                                
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception as needed (logging, displaying an error message, etc.)
                Console.WriteLine($"An error occurred while fetching inventory item: {ex.Message}");
            }

            return InventoryItem;
        }

        public void UpdateInventoryItemQuantity(int newQuantity, int itemId)
        {
            DB_Connection con = new DB_Connection();

            try
            {
                using (SqlConnection connection = con.Sheleni_Db_Connection())
                {
                    /// Update the quantity in the database
                    string updateQuantityQuery = "UPDATE Inventory SET Quantity = @newQuantity WHERE ItemID = @ItemID";
                    using (SqlCommand updateQuantityCommand = new SqlCommand(updateQuantityQuery, connection))
                    {
                        updateQuantityCommand.Parameters.AddWithValue("@newQuantity", newQuantity);
                        updateQuantityCommand.Parameters.AddWithValue("@ItemID", itemId);

                        // Execute the update query
                        updateQuantityCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception as needed (e.g., log the error)
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private void NavigateToDashboard()
        {
            var dashboardViewModel = new DashboardPageViewModel()
            {
                LoggedInUserId = MerchantId,
                Username = MerchantName
            };

            // Assuming your DashboardPage is the target page for navigation
            Application.Current.MainPage.Navigation.PushAsync(new DashboardPage { BindingContext = dashboardViewModel });
        }

        private async Task<bool> DisplayCustomAlert(string icon, string title, string message)
        {
            var customPopup = new CustomAlertPage(); // Create an instance of your custom popup
            var popupPageViewModel = new CustomAlertPageViewModel
            {
                Icon = icon,
                Title = title,
                Message = message
            };
            customPopup.BindingContext = popupPageViewModel;

            await PopupNavigation.Instance.PushAsync(customPopup);
            return await popupPageViewModel.GetConfirmationResult();
        }
    }
}
