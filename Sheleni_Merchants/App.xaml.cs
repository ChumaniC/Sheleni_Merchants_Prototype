using Sheleni_Merchants.Services;
using Sheleni_Merchants.Views;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Sheleni_Merchants
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            // Set the MainPage to the SplashPage initially
            MainPage = new NavigationPage(new SplashPage());
        }

        protected async override void OnStart()
        {
            // Simulate some work or delay if needed before navigating to the LandingPage
            await Task.Delay(5000); // 3 seconds delay

            // After the delay, navigate to the LandingPage without the NavigationBar
            MainPage = new AppShell();
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
