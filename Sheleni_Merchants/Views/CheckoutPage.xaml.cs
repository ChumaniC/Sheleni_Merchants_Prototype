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
	public partial class CheckoutPage : ContentPage
	{
        public CheckoutPageViewModel ViewModel { get; set; }

        public CheckoutPage(CheckoutPageViewModel viewModel)
        {
            InitializeComponent();

            ViewModel = viewModel;
            BindingContext = ViewModel;
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();

            // Call the method to save the selected items in the ViewModel
            await ViewModel.OnDisappearing();
        }
    }
}