using Rg.Plugins.Popup.Pages;
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
    public partial class ConfirmationMessageBoxPage : PopupPage
    {

        public ConfirmationMessageBoxPage()
        {
            InitializeComponent();

        }

        private void OnNoButtonClicked(object sender, System.EventArgs e)
        {
            var viewModel = (ConfirmationMessageBoxViewModel)BindingContext;
            viewModel.OnNoButtonClicked();
        }

        private void OnYesButtonClicked(object sender, System.EventArgs e)
        {
            var viewModel = (ConfirmationMessageBoxViewModel)BindingContext;
            viewModel.OnYesButtonClicked();
        }
    }
}