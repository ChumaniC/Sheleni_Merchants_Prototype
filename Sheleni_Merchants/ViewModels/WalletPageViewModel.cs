using Sheleni_Merchants.Models;
using Sheleni_Merchants.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Sheleni_Merchants.ViewModels
{
    public class WalletPageViewModel : BaseViewModel
    {
        public List<TransactionHistory> TransactionHistory { get; set; }
        public List<TransactionHistory> ReversedTransactionHistory { get; set; }
        public WalletPageViewModel()
        {
            Title = "Merchant Wallet";

            DateTime currentDate = DateTime.Now;

            FormattedDate = currentDate.ToString("dd MMM yyyy");

            TransactionHistory = new List<TransactionHistory>();
            ReversedTransactionHistory = new List<TransactionHistory>();

            LoadWallet();

            LoadTransactionHistory();
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

        private string formattedDate;

        public string FormattedDate
        {
            get { return formattedDate; }
            set { SetProperty(ref formattedDate, value); }
        }

        private string walletUsername;

        public string WalletUsername
        {
            get { return walletUsername; }
            set { SetProperty(ref walletUsername, value); }
        }

        private string transactionColour;

        public string TransactionColour
        {
            get { return transactionColour; }
            set { SetProperty(ref transactionColour, value); }
        }
        
        private string transactionIcon;

        public string TransactionIcon
        {
            get { return transactionIcon; }
            set { SetProperty(ref transactionIcon, value); }
        }

        private void LoadWallet()
        {
            // Open database connection
            DB_Connection conn = new DB_Connection();
            SqlConnection dbConn = conn.Sheleni_Db_Connection();

            // Retrieve location names from database
            string selectWalletQuery = "SELECT * FROM dbo.MerchantWallet";
            SqlCommand commandWallet= new SqlCommand(selectWalletQuery, dbConn);
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

            private void LoadTransactionHistory()
            {
                try
                {
                    // Open database connection
                    DB_Connection conn = new DB_Connection();
                    SqlConnection dbConn = conn.Sheleni_Db_Connection();

                    // Retrieve location names from database
                    string selectTransactionQuery = "SELECT * FROM dbo.MerchantTransactionHistory WHERE merchant_id = @MerchantId";
                    SqlCommand commandTransaction = new SqlCommand(selectTransactionQuery, dbConn);
                    commandTransaction.Parameters.AddWithValue("@MerchantId", OwnerId);
                    SqlDataReader readerTransaction = commandTransaction.ExecuteReader();

                    decimal _transactionAmount;
                    DateTime _transactionDate;
                    TimeSpan _transactionTime;
                    string _transactionType;
                    int _income;

                    // Retrieve Location Information
                    while (readerTransaction.Read())
                    {
                        _transactionAmount = Convert.ToDecimal(readerTransaction["amount"]);
                        _transactionDate = Convert.ToDateTime(readerTransaction["transaction_date"]);
                        _transactionTime = (TimeSpan)(readerTransaction["transaction_time"]);
                        _transactionType = readerTransaction["transaction_type"].ToString();
                        _income = Convert.ToInt16(readerTransaction["income"]);

                        if (_income == 1)
                        {
                            TransactionColour = "Green";
                        }
                        else
                        {
                            TransactionColour = "Red";
                        }
                    
                        if (_transactionType == "Purchase Transaction")
                        {
                            TransactionIcon = "purchase.png";
                        }
                        else if (_transactionType == "eWallet Transaction")
                        {
                            TransactionIcon = "eWallet.png";
                        }

                        TransactionHistory.Add(new TransactionHistory
                        {
                            Amount = _transactionAmount,
                            TransactionColour = TransactionColour,
                            TransactionDate = _transactionDate,
                            TransactionTime = _transactionTime,
                            TransactionType = _transactionType,
                            TransactionIcon = TransactionIcon,
                            Income = _income
                        });
                    }
                    readerTransaction.Close();

                ReversedTransactionHistory = TransactionHistory.OrderByDescending(item => item.TransactionDate)
                                            .ThenByDescending(item => item.TransactionTime)
                                            .ToList();


                dbConn.Close();
                }
                catch (Exception ex)
                {
                    // Handle exceptions (e.g., log or show an error message)
                }
            }
    }
}
