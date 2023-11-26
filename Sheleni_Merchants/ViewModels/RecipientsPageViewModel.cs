using System;
using System.Collections.Generic;
using System.Text;

namespace Sheleni_Merchants.ViewModels
{
    public class RecipientsPageViewModel : BaseViewModel
    {
        public RecipientsPageViewModel()
        {
            Title = "Recipients";
        }

        private int merchantId;

        public int MerchantId
        {
            get { return merchantId; }
            set { SetProperty(ref merchantId, value); }
        }

        private string merchantName;

        public string MerchantName
        {
            get { return merchantName; }
            set { SetProperty(ref merchantName, value); }
        }
    }
}
