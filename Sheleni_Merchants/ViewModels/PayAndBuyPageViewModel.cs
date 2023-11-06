using Sheleni_Merchants.Models;
using Sheleni_Merchants.Services;
using Sheleni_Merchants.Views;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Input;
using Xamarin.Forms;

namespace Sheleni_Merchants.ViewModels
{
    public class PayAndBuyPageViewModel : BaseViewModel
    {
        public List<PayAndBuyServices> PayAndBuyServices { get; set; }

        public ICommand ServiceFrameTappedCommand { get; }

        public PayAndBuyPageViewModel()
        {
            Title = "Pay & Buy";

            PayAndBuyServices = new List<PayAndBuyServices>();

            ServiceFrameTappedCommand = new MvvmHelpers.Commands.Command<PayAndBuyServices>(OnServiceSelected);

            LoadServiceNamesAsync();
        }

        private PayAndBuyServices selectedService;

        public PayAndBuyServices SelectedService
        {
            get { return selectedService; }
            set { SetProperty(ref selectedService, value); }
        }

        private void OnServiceSelected(PayAndBuyServices service)
        {
            NavigateToSelectedServicePage(service);
        }

        private async void NavigateToSelectedServicePage(PayAndBuyServices selectedService)
        {
            //ApiUserDetails userDetails = await _httpClient.GetApiUserDetailsAsync("d3a1478b-6913-4bf3-9c73-e6791b335dc6");

            try
            {
                Page pageToNavigate = null;

                switch (selectedService.ServiceName.ToLower())
                {
                    case "airtime":
                        pageToNavigate = new AirtimeAndDataPage();
                        break;

                    case "bills":
                        pageToNavigate = new BillsPage();
                        break;

                    case "electricity":
                        pageToNavigate = new ElectricityPage();
                        break;

                    case "groceries":
                        pageToNavigate = new BuyPage();
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

        private void LoadServiceNamesAsync()
        {
            try
            {
                // Open database connection
                DB_Connection conn = new DB_Connection();
                SqlConnection dbConn = conn.Sheleni_Db_Connection();

                // Retrieve location names from database
                string selectServiceQuery = "SELECT * FROM dbo.PayAndBuyServices";
                SqlCommand commandService = new SqlCommand(selectServiceQuery, dbConn);
                SqlDataReader readerService = commandService.ExecuteReader();

                string _serviceName;
                string _serviceDescription;

                // Retrieve Location Information
                while (readerService.Read())
                {
                    _serviceName = readerService["ServiceName"].ToString();
                    _serviceDescription = readerService["ServiceDescription"].ToString();
                    PayAndBuyServices.Add(new PayAndBuyServices
                    {
                        ServiceName = _serviceName,
                        ServiceIcon = _serviceName.ToLower() + ".png",
                        ServiceDescription = _serviceDescription
                    });
                }
                readerService.Close();

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
    }
}