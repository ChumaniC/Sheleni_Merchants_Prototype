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

        public ObservableCollection<CarouselItems> CarouselItems { get; set; }

        public InventoryPageViewModel()
        {
            Title = "Inventory Category";

            InventoryGroups = new ObservableCollection<InventoryGroup>();

            CarouselItems = new ObservableCollection<CarouselItems>();

            SubcategoryFrameTappedCommand = new MvvmHelpers.Commands.Command<InventoryGroup>(OnSubcategorySelected);

            LoadCarouselImages();

            // Load service names when the ViewModel is constructed (you can do this differently based on your app's logic)
            try
            {
                LoadInventoryDataAsync();
            }
            catch (Exception ex)
            {
            }
        }

        private void LoadCarouselImages()
        {
            try
            {
                // Open database connection
                DB_Connection conn = new DB_Connection();
                SqlConnection dbConn = conn.Sheleni_Db_Connection();

                // Retrieve location names from database
                string selectImageQuery = "SELECT * FROM dbo.InventoryMessages";
                SqlCommand commandImage = new SqlCommand(selectImageQuery, dbConn);
                SqlDataReader readerImage = commandImage.ExecuteReader();

                string _imageName;
                string _message;

                // Retrieve Location Information
                while (readerImage.Read())
                {
                    _imageName = readerImage["heading"].ToString();
                    _message = readerImage["message"].ToString();

                    CarouselItems.Add(new CarouselItems
                    {
                        ImageName = _imageName.ToLower() + ".png",
                        Message = _message
                    });
                }
                readerImage.Close();

                dbConn.Close();
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log or show an error message)
            }
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
                            ItemID = (int)readerItem["ItemID"],
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
                                    ItemID = (int)readerItem["ItemID"],
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