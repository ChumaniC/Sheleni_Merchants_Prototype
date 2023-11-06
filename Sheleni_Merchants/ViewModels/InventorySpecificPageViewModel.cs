using Sheleni_Merchants.Models;
using System.Collections.Generic;

namespace Sheleni_Merchants.ViewModels
{
    public class InventorySpecificPageViewModel : BaseViewModel
    {
        public List<Item> Items { get; set; }
        public string PageTitle { get; set; }

        public InventorySpecificPageViewModel()
        {
        }

        public InventorySpecificPageViewModel(List<Item> items, string pageTitle)
        {
            Items = items;

            PageTitle = pageTitle;

            Title = PageTitle + " Inventory";
        }
    }
}