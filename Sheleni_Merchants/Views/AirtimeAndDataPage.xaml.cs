using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.UI.Views.Options;
using Sheleni_Merchants.ViewModels;

namespace Sheleni_Merchants.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AirtimeAndDataPage : ContentPage
	{
		public AirtimeAndDataPage ()
		{
			InitializeComponent ();
		}

        public string SelectedBundle { get; set; }

        async public void Frame_Clicked(object sender, EventArgs e)
        {
            ToastOptions toastOptions = new ToastOptions
            {
                MessageOptions = new MessageOptions()
                {
                    Foreground = Color.White,
                    Message = "Bundle added to cart",
                },
                BackgroundColor = Color.Green,
                Duration = TimeSpan.FromSeconds(2)
            };

            await this.DisplayToastAsync(toastOptions);
        }
    }
}