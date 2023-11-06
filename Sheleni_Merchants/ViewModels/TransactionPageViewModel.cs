using Rg.Plugins.Popup.Services;
using Sheleni_Merchants.Models;
using Sheleni_Merchants.Services;
using Sheleni_Merchants.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Sheleni_Merchants.ViewModels
{
    public class TransactionPageViewModel : BaseViewModel
    {
        private readonly SheleniHttpClient _httpClient;

        public ICommand ServiceFrameTappedCommand { get; }
        public TransactionPageViewModel()
        {
            Title = "Customer Dashboard";

            Services = new ObservableCollection<Service>();

            ServiceFrameTappedCommand = new MvvmHelpers.Commands.Command<Service>(OnServiceSelected);

            // Initialize the LogoutCommand with a command that executes the logout logic
            LogoutCommand = new Xamarin.Forms.Command(OnLogoutClicked);

            // Initialize the HttpClient
            _httpClient = new SheleniHttpClient();

            // Load service names when the ViewModel is constructed (you can do this differently based on your app's logic)
            LoadServiceNamesAsync();
        }

        private Service selectedService;
        public Service SelectedService
        {
            get { return selectedService; }
            set { SetProperty(ref selectedService, value); }
        }

        private void OnServiceSelected(Service service)
        {
            NavigateToSelectedServicePage(service);
        }

        private async void NavigateToSelectedServicePage(Service selectedService)
        {
            Page pageToNavigate = null;

            switch (selectedService.ServiceName.ToLower())
            {
                case "payment":
                    pageToNavigate = new RequestToPayPage();
                    break;
                case "transfer":
                    pageToNavigate = new TransferPage();
                    break;
                // Add more cases for other services as needed
                default:
                    // Handle the case where no matching page is found
                    break;
            }

            if (pageToNavigate != null)
            {
                await Application.Current.MainPage.Navigation.PushAsync(pageToNavigate);
            }
        }

        public ObservableCollection<Service> Services { get; set; }

        private async void OnLogoutClicked()
        {
            bool logoutConfirmed = await DisplayConfirmationAlert("Logout", "Are you sure you want to logout?");

            if (logoutConfirmed)
            {
                // Application.Current.MainPage = new NavigationPage(new LandingPage());
            }
            else if (!logoutConfirmed)
            {
                return;
            }
        }

        public ICommand LogoutCommand { get; }

        private bool _confirmationResult;

        public bool ConfirmationResult
        {
            get { return _confirmationResult; }
            set { SetProperty(ref _confirmationResult, value); }
        }
        public async Task<bool> DisplayConfirmationAlert(string title, string message)
        {
            var customPopup = new ConfirmationMessageBoxPage();
            var popupPageViewModel = new ConfirmationMessageBoxViewModel
            {
                Title = title,
                Message = message
            };
            customPopup.BindingContext = popupPageViewModel;

            await PopupNavigation.Instance.PushAsync(customPopup);
            bool confirmationResult = await popupPageViewModel.GetConfirmationResult();

            ConfirmationResult = confirmationResult;

            return confirmationResult;
        }

        private void LoadServiceNamesAsync()
        {
            try
            {
                // Open database connection 
                DB_Connection conn = new DB_Connection();
                SqlConnection dbConn = conn.Sheleni_Db_Connection();

                // Retrieve location names from database
                string selectServiceQuery = "SELECT * FROM dbo.CustomerServices";
                SqlCommand commandService = new SqlCommand(selectServiceQuery, dbConn);
                SqlDataReader readerService = commandService.ExecuteReader();

                string _serviceName;

                // Retrieve Location Information
                while (readerService.Read())
                {
                    _serviceName = readerService["ServiceName"].ToString();
                    Services.Add(new Service
                    {
                        ServiceName = _serviceName,
                        ServiceIcon = _serviceName.ToLower() + ".png"
                    });
                }
                readerService.Close();

                dbConn.Close();
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log or show an error message)
            }
        }
    }
}
