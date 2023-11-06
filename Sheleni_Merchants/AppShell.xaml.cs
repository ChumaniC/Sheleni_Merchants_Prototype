using Sheleni_Merchants.ViewModels;
using Sheleni_Merchants.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Sheleni_Merchants
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Set the initial MainPage to the LandingPage without the NavigationBar
            SetMainPageWithoutNavigationBar(new DashboardPage());
        }

        public void SetMainPageWithoutNavigationBar(Page page)
        {
            Application.Current.MainPage = page;
            NavigationPage.SetHasNavigationBar(page, false);
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
