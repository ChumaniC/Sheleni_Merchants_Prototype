using MvvmHelpers;
using Sheleni_Merchants.Models;
using Sheleni_Merchants.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace Sheleni_Merchants.ViewModels
{
    public class InventorySpecificPageViewModel : BaseViewModel
    {
        private List<Item> allItems; // Keep a copy of all items

        private ObservableCollection<Item> filteredItems;

        public ObservableCollection<Item> FilteredItems
        {
            get { return filteredItems; }
            set { SetProperty(ref filteredItems, value); }
        }

        public string PageTitle { get; set; }

        public ICommand ManageInventoryCommand { get; }

        public InventorySpecificPageViewModel()
        {
            allItems = new List<Item>();
            // Set FilteredItems initially to an empty collection
            FilteredItems = new ObservableCollection<Item>();

            ManageInventoryCommand = new MvvmHelpers.Commands.Command<Item>(OnManageInventoryAsync);
        }

        public InventorySpecificPageViewModel(List<Item> items, string pageTitle)
        {
            allItems = items;
            FilterItems();

            PageTitle = pageTitle;

            Title = PageTitle + " Inventory";

            ManageInventoryCommand = new MvvmHelpers.Commands.Command<Item>(OnManageInventoryAsync);
        }

        private void FilterItems()
        {
            // If the search bar is empty, show all items
            if (string.IsNullOrWhiteSpace(SearchedItem))
            {
                FilteredItems = new ObservableCollection<Item>(allItems);
            }
            else
            {
                // Filter items based on the search criteria
                FilteredItems = new ObservableCollection<Item>(
                    allItems.Where(item => item.ItemName.ToLower().Contains(SearchedItem.ToLower()))
                );
            }
        }

        private string searchedItem;

        public string SearchedItem
        {
            get { return searchedItem; }
            set
            {
                if (SetProperty(ref searchedItem, value))
                {
                    FilterItems();
                }
            }
        }

        private async void OnManageInventoryAsync(Item selectedItem)
        {
            ManageInventoryItemPageViewModel manageInventoryViewModel = new ManageInventoryItemPageViewModel(selectedItem);

            ManageInventoryItemPage manageInventoryItemPage = new ManageInventoryItemPage { BindingContext = manageInventoryViewModel };

            await Application.Current.MainPage.Navigation.PushAsync(manageInventoryItemPage);
        }
    }
}
