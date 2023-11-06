using MvvmHelpers.Interfaces;
using Sheleni_Merchants.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Sheleni_Merchants.ViewModels
{
    public class BillsPageViewModel : BaseViewModel
    {
        public IAsyncCommand CheckoutCommand { get; }
        public BillsPageViewModel()
        {
            Title = "Account & Bills";
            Title = "Airtime & Data Bundles";

            CheckoutCommand = new MvvmHelpers.Commands.AsyncCommand(CheckoutAsync);
        }

        private async Task CheckoutAsync()
        {
            // Create a list of items with quantity greater than 0
            //var itemsInCart = Items.Where(item => item.CurrentQuantity > 0).ToList();

            var checkoutPageViewModel = new CheckoutPageViewModel();
            await Application.Current.MainPage.Navigation.PushAsync(new CheckoutPage(checkoutPageViewModel));
        }
    }
}
