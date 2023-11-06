using Sheleni_Merchants.Models;
using Sheleni_Merchants.Services;
using Sheleni_Merchants.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows.Input;
using Xamarin.Forms;

namespace Sheleni_Merchants.ViewModels
{
    public class InventoryPageViewModel : BaseViewModel
    {
        public ObservableCollection<InventoryGroup> InventoryGroups { get; set; }
        public ICommand SubcategoryFrameTappedCommand { get; }

        public InventoryPageViewModel()
        {
            Title = "Inventory Category";

            InventoryGroups = new ObservableCollection<InventoryGroup>();

            SubcategoryFrameTappedCommand = new MvvmHelpers.Commands.Command<InventoryGroup>(OnSubcategorySelected);

            // Load service names when the ViewModel is constructed (you can do this differently based on your app's logic)
            try
            {
                LoadInventoryDataAsync();
            }
            catch (Exception ex)
            {
            }
        }

        private async void LoadInventoryDataAsync()
        {
            try
            {
                // Fetch subcategories and items from your data source
                // You need to structure your items in a way that they are grouped by subcategory

                // use your data fetching logic here):
                var inventoryData = GetInventoryData();

                // Process data into groups
                foreach (var group in inventoryData)
                {
                    InventoryGroups.Add(group);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
            }
        }

        public List<InventoryGroup> GetInventoryData()
        {
            List<InventoryGroup> inventoryGroups = new List<InventoryGroup>();

            try
            {
                // Open database connection
                DB_Connection conn = new DB_Connection();
                SqlConnection dbConn = conn.Sheleni_Db_Connection();

                // Retrieve subcategory names from the Inventory table
                string selectItemQuery = "SELECT * FROM dbo.Inventory";
                SqlCommand commandItem = new SqlCommand(selectItemQuery, dbConn);
                SqlDataReader readerItem = commandItem.ExecuteReader();

                while (readerItem.Read())
                {
                    string _subcategory = readerItem["Subcategory"].ToString();

                    // Check if there's an existing group for the subcategory
                    var existingGroup = inventoryGroups.Find(group => group.Subcategory == _subcategory);
                    if (existingGroup != null)
                    {
                        // Group for this subcategory already exists, add the item to it
                        existingGroup.Items.Add(new Item
                        {
                            // Fill the item details accordingly

                            ItemName = readerItem["ItemName"].ToString(),
                            Quantity = Convert.ToInt32(readerItem["Quantity"]),
                            ItemIcon = readerItem["ItemName"].ToString().ToLower(),
                            SubcategoryIcon = readerItem["Subcategory"].ToString().ToLower() + ".png"
                        });
                    }
                    else
                    {
                        // Group for this subcategory doesn't exist, create a new group
                        InventoryGroup newGroup = new InventoryGroup
                        {
                            Subcategory = _subcategory,
                            Items = new List<Item>
                            {
                                new Item
                                {
                                    // Fill the item details accordingly

                                    ItemName = readerItem["ItemName"].ToString(),
                                    Quantity = Convert.ToInt32(readerItem["Quantity"]),
                                    ItemIcon = readerItem["ItemName"].ToString().ToLower(),
                                    SubcategoryIcon = readerItem["Subcategory"].ToString().ToLower() + ".png"
                                }
                            }
                        };
                        inventoryGroups.Add(newGroup);
                    }
                }
                readerItem.Close();
                dbConn.Close();
            }
            catch (Exception ex)
            {
                // Handle the exception
            }

            return inventoryGroups;
        }

        private void OnSubcategorySelected(InventoryGroup selectedGroup)
        {
            NavigateToSelectedSubcategoryPage(selectedGroup);
        }

        private async void NavigateToSelectedSubcategoryPage(InventoryGroup selectedGroup)
        {
            try
            {
                var groupsubcategory = selectedGroup.Subcategory;
                var groupItems = selectedGroup.Items;

                // Create the ViewModel instance and set its properties
                var inventorySpecificPageViewModel = new InventorySpecificPageViewModel(groupItems, groupsubcategory);

                // Create an instance of InventorySpecificPage and set its BindingContext to the ViewModel
                InventorySpecificPage inventorySpecificPage = new InventorySpecificPage();
                inventorySpecificPage.BindingContext = inventorySpecificPageViewModel;

                await Application.Current.MainPage.Navigation.PushAsync(inventorySpecificPage);
            }
            catch (Exception ex)
            {
                // Handle exceptions
            }
        }
    }
}