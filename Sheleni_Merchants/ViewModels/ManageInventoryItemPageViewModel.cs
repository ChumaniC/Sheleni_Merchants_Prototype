using Sheleni_Merchants.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sheleni_Merchants.ViewModels
{
    public class ManageInventoryItemPageViewModel : BaseViewModel
    {
        public ManageInventoryItemPageViewModel()
        {
                
        }

        public ManageInventoryItemPageViewModel(Item selectedItem)
        {
            // Use the 'selectedItem' parameter to access the details of the selected item
            // For example, you can set properties in this ViewModel based on the selected item
            ItemId = selectedItem.ItemID;
            ItemName = selectedItem.ItemName;
            SubCategory = selectedItem.Subcategory;
            ItemIcon = ItemName.ToLower();

            Title = ItemName + " Inventory";
        }

        private int itemId;

        public int ItemId
        {
            get { return itemId; }
            set { SetProperty(ref itemId, value); }
        }
        private string itemName;

        public string ItemName
        {
            get { return itemName; }
            set { SetProperty(ref itemName, value); }
        }
        private string subCategory;

        public string SubCategory
        {
            get { return subCategory; }
            set { SetProperty(ref subCategory, value); }
        }
        private string itemIcon;

        public string ItemIcon
        {
            get { return itemIcon; }
            set { SetProperty(ref itemIcon, value); }
        }

        public void OnManageSelectedInventoryItem()
        {
            
        }
    }
}
