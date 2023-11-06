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
	public partial class RequestToPayPage : ContentPage
	{
		public RequestToPayPage ()
		{
			InitializeComponent ();
		}
        public RequestToPayPage(RequestToPayPageViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
