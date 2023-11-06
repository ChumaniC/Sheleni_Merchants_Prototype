using Sheleni_Merchants.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Sheleni_Merchants.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BuyPage : ContentPage
	{
        private BuyPageViewModel viewModel;
        public BuyPage ()
		{
			InitializeComponent ();
            viewModel = new BuyPageViewModel();
            BindingContext = viewModel;
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();

            // Call the method to save the selected items in the ViewModel
            await viewModel.OnDisappearing();
        }
    }
}