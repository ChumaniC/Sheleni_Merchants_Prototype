using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using Sheleni_Merchants.Models;
using Sheleni_Merchants.Services;
using Sheleni_Merchants.Views;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Sheleni_Merchants.ViewModels
{
    public class EWalletPageViewModel: BaseViewModel
    {
        public MvvmHelpers.Commands.Command SendCommand { get; }
        public EWalletPageViewModel()
        {
            Title = "eWallet";

            LoadWallet();

            SendCommand = new MvvmHelpers.Commands.Command(ProcessPaymentAsync);
        }

        private Decimal amount;

        public Decimal Amount
        {
            get { return amount; }
            set { SetProperty(ref amount, value); }
        }

        private int phoneNumber;

        public int PhoneNumber
        {
            get { return phoneNumber; }
            set { SetProperty(ref phoneNumber, value); }
        }

        private int walletId;

        public int WalletId
        {
            get { return walletId; }
            set { SetProperty(ref walletId, value); }
        }

        private int ownerId;

        public int OwnerId
        {
            get { return ownerId; }
            set { SetProperty(ref ownerId, value); }
        }

        private decimal balance;

        public decimal Balance
        {
            get { return balance; }
            set { SetProperty(ref balance, value); }
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

        private int customerId;

        public int CustomerId
        {
            get { return customerId; }
            set { SetProperty(ref customerId, value); }
        }

        private string customerName;

        public string CustomerName
        {
            get { return customerName; }
            set { SetProperty(ref customerName, value); }
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

        private string description;

        public string Description
        {
            get { return description; }
            set { SetProperty(ref description, value); }
        }

        private int income;

        public int Income
        {
            get { return income; }
            set { SetProperty(ref income, value); }
        }

        private void LoadWallet()
        {
            // Open database connection
            DB_Connection conn = new DB_Connection();
            SqlConnection dbConn = conn.Sheleni_Db_Connection();

            // Retrieve location names from database
            string selectWalletQuery = "SELECT * FROM dbo.MerchantWallet";
            SqlCommand commandWallet = new SqlCommand(selectWalletQuery, dbConn);
            SqlDataReader readerWallet = commandWallet.ExecuteReader();

            int _walletId;
            int _ownerId;
            decimal _balance;

            // Retrieve Item Information
            while (readerWallet.Read())
            {
                _walletId = (int)readerWallet["wallet_id"];
                _ownerId = (int)readerWallet["owner_id"];
                _balance = (decimal)readerWallet["balance"];

                WalletId = _walletId;
                OwnerId = _ownerId;
                Balance = _balance;
            }
            //RefreshCommand = new AsyncCommand(Refresh);
            readerWallet.Close();

            dbConn.Close();
        }

        private async void ProcessPaymentAsync()
        {
            // Check if the entered phone number exists in the Customer table
            if (CheckCustomerExists(PhoneNumber))
            {
                // Retrieve the customer ID
                RetrieveCustomerId(PhoneNumber);

                // Proceed with the payment process
                // Call the method to save the entered amount to the TransactionHistory table
                SaveTransactionToHistory();

                UpdateWalletBalance(MerchantId);

                await DisplayCustomAlert("success.png", "Payment Result", "eWallet payment is successful.");
            }
            else
            {
                // Handle the case where the customer does not exist
                // Display an error message or take appropriate action
            }
        }

        private bool CheckCustomerExists(int phoneNumber)
        {
            string selectCustomerQuery = $"SELECT COUNT(*) FROM dbo.Customer WHERE mobile_number = {phoneNumber}";

            DB_Connection con = new DB_Connection();

            var dbConn = con.Sheleni_Db_Connection();
            try
            {
                using (SqlCommand commandCustomer = new SqlCommand(selectCustomerQuery, dbConn))
                {
                    int count = (int)commandCustomer.ExecuteScalar(); // Execute the query

                    if (count > 0)
                    {
                        dbConn.Close();
                        return true;
                    }
                    else
                    {
                        // display error message
                    }
                    return false;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void RetrieveCustomerId(int phoneNumber)
        {
            string selectCustomerIdQuery = $"SELECT customer_id, customer_name FROM dbo.Customer WHERE mobile_number = {phoneNumber}";
            string selectCustomerNameQuery = $"SELECT customer_name FROM dbo.Customer WHERE mobile_number = {phoneNumber}";

            DB_Connection con = new DB_Connection();

            var dbConn = con.Sheleni_Db_Connection();

            using (SqlCommand commandCustomerDetails = new SqlCommand(selectCustomerIdQuery, dbConn))
            {
                // Execute the query to retrieve the customer ID
                CustomerId = (int)commandCustomerDetails.ExecuteScalar();
            } 
            
            using (SqlCommand commandCustomerDetails = new SqlCommand(selectCustomerNameQuery, dbConn))
            {
                // Execute the query to retrieve the customer Name

                CustomerName = commandCustomerDetails.ExecuteScalar().ToString();
                dbConn.Close();
            }
        }

        private void SaveTransactionToHistory()
        {
            DB_Connection con = new DB_Connection();

            var dbConn = con.Sheleni_Db_Connection();

            TransactionTime = DateTime.Now.TimeOfDay;
            Description = $"eWallet Payment to {CustomerName}";
            TransactionType = "eWallet Transaction";
            CustomerId = CustomerId;
            Income = 0;

            string insertTransactionQuery = @"
                    INSERT INTO MerchantTransactionHistory 
                    (merchant_id, amount, transaction_date, transaction_time, description, transaction_type, customer_id, income)
                    VALUES 
                    (@MerchantId, @Amount, @Transaction_date, @Transaction_time, @Description, @TransactionType, @CustomerId, @Income)";

            using (SqlCommand commandInsertTransaction = new SqlCommand(insertTransactionQuery, dbConn))
            {
                // Add parameters to the SqlCommand
                commandInsertTransaction.Parameters.AddWithValue("@MerchantId", merchantId);
                commandInsertTransaction.Parameters.AddWithValue("@Amount", Convert.ToDecimal(Amount));
                commandInsertTransaction.Parameters.AddWithValue("@Transaction_date", TransactionDate);
                commandInsertTransaction.Parameters.AddWithValue("@Transaction_time", transactionTime);
                commandInsertTransaction.Parameters.AddWithValue("@Description", description);
                commandInsertTransaction.Parameters.AddWithValue("@TransactionType", transactionType);
                commandInsertTransaction.Parameters.AddWithValue("@CustomerId", CustomerId);
                commandInsertTransaction.Parameters.AddWithValue("@Income", income);

                // Execute the query to insert the transaction
                commandInsertTransaction.ExecuteNonQuery();
            }

            dbConn.Close();
        }

        public void UpdateWalletBalance(int ownerId)
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
                        decimal newBalance = currentBalance - Amount;

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
        private async Task<bool> DisplayCustomAlert(string icon, string title, string message)
        {
            LoadWallet();
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
