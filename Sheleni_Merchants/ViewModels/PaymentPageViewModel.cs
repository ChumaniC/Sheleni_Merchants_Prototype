using Sheleni_Merchants.Models;
using Sheleni_Merchants.Services;
using Sheleni_Merchants.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Sheleni_Merchants.ViewModels
{
    public class PaymentPageViewModel : BaseViewModel
    {
        public ICommand PaymentServiceFrameTappedCommand { get; }

        public ObservableCollection<CarouselItems> CarouselItems { get; set; }
        public PaymentPageViewModel()
        {
            Title = "Pay";

            CarouselItems = new ObservableCollection<CarouselItems>();

            PaymentServiceFrameTappedCommand = new MvvmHelpers.Commands.Command<PaymentService>(OnPaymentServiceSelected);

            PaymentServices = new ObservableCollection<PaymentService>();

            LoadCarouselImages();
            
            LoadPaymentServiceNamesAsync();
        }

        public ObservableCollection<PaymentService> PaymentServices { get; set; }


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

        private void LoadCarouselImages()
        {
            try
            {
                // Open database connection
                DB_Connection conn = new DB_Connection();
                SqlConnection dbConn = conn.Sheleni_Db_Connection();

                // Retrieve location names from database
                string selectImageQuery = "SELECT * FROM dbo.PaymentMessages";
                SqlCommand commandImage = new SqlCommand(selectImageQuery, dbConn);
                SqlDataReader readerImage = commandImage.ExecuteReader();

                string _imageName;
                string _message;

                // Retrieve Location Information
                while (readerImage.Read())
                {
                    _imageName = readerImage["heading"].ToString();
                    _message = readerImage["message"].ToString();

                    CarouselItems.Add(new CarouselItems
                    {
                        ImageName = _imageName.ToLower() + ".png",
                        Message = _message
                    });
                }
                readerImage.Close();

                dbConn.Close();
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log or show an error message)
            }
        }

        private void LoadPaymentServiceNamesAsync()
        {
            try
            {
                // Open database connection
                DB_Connection conn = new DB_Connection();
                SqlConnection dbConn = conn.Sheleni_Db_Connection();

                // Retrieve location names from database
                string selectPaymentServiceQuery = "SELECT * FROM dbo.PaymentServices";
                SqlCommand commandPaymentService = new SqlCommand(selectPaymentServiceQuery, dbConn);
                SqlDataReader readerPaymentService = commandPaymentService.ExecuteReader();

                string _paymentServiceName;

                // Retrieve Location Information
                while (readerPaymentService.Read())
                {
                    _paymentServiceName = readerPaymentService["payment_serv_name"].ToString();
                    PaymentServices.Add(new PaymentService
                    {
                        PaymentServiceName = _paymentServiceName,
                        PaymentServiceIcon = _paymentServiceName.ToLower() + ".png"
                    });
                }
                readerPaymentService.Close();

                dbConn.Close();

                //var serviceNames = await _httpClient.GetAllServiceNamesAsync();
                //if (serviceNames != null)
                //{
                //    // Update the ServiceNames property with the retrieved service names
                //    ServiceNames = serviceNames;
                //}
                //else
                //{
                //    // Handle the case where service names couldn't be retrieved
                //}
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log or show an error message)
            }
        }

        private void OnPaymentServiceSelected(PaymentService paymentService)
        {
            NavigateToSelectedPaymentServicePage(paymentService);
        }

        private async void NavigateToSelectedPaymentServicePage(PaymentService selectedPaymentService)
        {
            try
            {

                Page pageToNavigate = null;

                    switch (selectedPaymentService.PaymentServiceName.ToLower())
                    {
                        case "recipients":
                            var recipientsViewModel = new RecipientsPageViewModel
                            {
                                MerchantId = MerchantId,
                                MerchantName = MerchantName
                            };
                            pageToNavigate = new RecipientsPage { BindingContext = recipientsViewModel };
                            break;

                        case "ewallet":
                            var eWalletViewModel = new EWalletPageViewModel                            {
                                MerchantId = MerchantId,
                                MerchantName = MerchantName
                            };
                            pageToNavigate = new EWalletPage { BindingContext = eWalletViewModel }; ;
                            break;

                        case "instant":
                            pageToNavigate = new TransferPage();
                            break;

                        case "onceOff":
                            pageToNavigate = new InventoryPage ();
                            break;

                        case "scan":
                            pageToNavigate = new DepositPage();
                            break;

                        case "chatPay":
                            pageToNavigate = new LoginPage();
                            break;

                        // Add more cases for other services as needed
                        default:
                            // Handle the case where no matching page is found
                            break;
                    }

                if (pageToNavigate != null)
                {
                    // Set the BindingContext if needed
                    //if (pageToNavigate.BindingContext is YourViewModel viewModel)
                    //{
                    //    // Initialize or set properties in your ViewModel based on the selected service if needed
                    //}

                    await Application.Current.MainPage.Navigation.PushAsync(pageToNavigate);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
