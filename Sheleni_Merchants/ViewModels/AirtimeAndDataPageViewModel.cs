using MvvmHelpers.Commands;
using MvvmHelpers.Interfaces;
using Newtonsoft.Json;
using Sheleni_Merchants.Models;
using Sheleni_Merchants.Services;
using Sheleni_Merchants.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.UI.Views.Options;
using Xamarin.Forms;

namespace Sheleni_Merchants.ViewModels
{
    public class AirtimeAndDataPageViewModel : BaseViewModel
    {
        private bool isAirtimeSelected;
        private bool isDataSelected;
        private ObservableCollection<string> airtimeTypes;
        private ObservableCollection<string> frequencies;
        private ObservableCollection<Item> bundles;
        public ObservableCollection<Item> SelectedBundles { get; }
        public ICommand AddToCartCommand { get; }

        private bool showListView;

        public bool ShowListView
        {
            get => showListView;
            set => SetProperty(ref showListView, value);
        }

        public bool IsAirtimeSelected
        {
            get => isAirtimeSelected;
            set
            {
                if (value)
                {
                    SetProperty(ref isAirtimeSelected, value);
                    IsDataSelected = false;
                    SelectedAirtimeType = null; // Clear selected airtime type
                    SelectedFrequency = null; // Clear selected frequency
                    ShowListView = false;
                    LoadAirtimeData();
                }
            }
        }

        public bool IsDataSelected
        {
            get => isDataSelected;
            set
            {
                if (value)
                {
                    SetProperty(ref isDataSelected, value);
                    IsAirtimeSelected = false;
                    SelectedAirtimeType = null; // Clear selected airtime type
                    SelectedFrequency = null; // Clear selected frequency
                    ShowListView = false;
                    LoadDataData();
                }
            }
        }

        public ObservableCollection<string> AirtimeTypes
        {
            get => airtimeTypes;
            set => SetProperty(ref airtimeTypes, value);
        }

        public ObservableCollection<string> Frequencies
        {
            get => frequencies;
            set => SetProperty(ref frequencies, value);
        }

        public ObservableCollection<Item> Bundles
        {
            get => bundles;
            set => SetProperty(ref bundles, value);
        }

        private string selectedAirtimeType;

        public string SelectedAirtimeType
        {
            get => selectedAirtimeType;
            set
            {
                SetProperty(ref selectedAirtimeType, value);
                FetchSelectedBundles();
            }
        }

        private string selectedFrequency;

        public string SelectedFrequency
        {
            get => selectedFrequency;
            set
            {
                SetProperty(ref selectedFrequency, value);
                FetchSelectedBundles();
            }
        }

        public IAsyncCommand CheckoutCommand { get; }

        public ICommand NavigateToPayAndBuyCommand { get; }

        public AirtimeAndDataPageViewModel()
        {
            Title = "Airtime & Data Bundles";

            CheckoutCommand = new AsyncCommand(CheckoutAsync);

            NavigateToPayAndBuyCommand = new MvvmHelpers.Commands.Command(NavigateToPayAndBuy);

            Bundles = new ObservableCollection<Item>();

            SelectedBundles = new ObservableCollection<Item>();

            AddToCartCommand = new MvvmHelpers.Commands.Command<Item>(OnAddToCart);
        }

        private async void NavigateToPayAndBuy()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new PayAndBuyPage());
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

        public List<Item> ItemsInCart { get; } = new List<Item>();

        private void OnAddToCart(Item selectedItem)
        {
            if (selectedItem != null && !string.IsNullOrEmpty(selectedItem.Price))
            {
                if (decimal.TryParse(selectedItem.Price, out decimal priceValue))
                {
                    var modifiedItem = new Item();

                    modifiedItem.CurrentPrice = priceValue;
                    modifiedItem.Price = selectedItem.ItemName;
                    modifiedItem.CurrentQuantity = 1;
                    modifiedItem.ItemName = selectedItem.ItemName + " bundle";
                    modifiedItem.ItemIcon = "bundle.png";

                    ItemsInCart.Add(modifiedItem);

                    var serializedItems = JsonConvert.SerializeObject(ItemsInCart);
                    Application.Current.Properties["SelectedItems"] = serializedItems;
                    Application.Current.SavePropertiesAsync();

                    SelectedBundles.Add(modifiedItem);
                }
                else
                {
                    // Handle the case where the Price string couldn't be parsed to a double
                    // Notify user or log an error
                    // e.g., Show an error message or log that the Price format is invalid
                }

                // UpdateBundlesListView();
            }
        }

        private async Task CheckoutAsync()
        {
            if (ItemsInCart.Any())
            {
                var checkoutPageViewModel = new CheckoutPageViewModel(ItemsInCart);
                checkoutPageViewModel.MerchantId = MerchantId;
                checkoutPageViewModel.MerchantName = MerchantName;
                checkoutPageViewModel.SaveSelectedItems();
                await Application.Current.MainPage.Navigation.PushAsync(new CheckoutPage(checkoutPageViewModel));
            }
        }

        private async void LoadAirtimeData()
        {
            DB_Connection conn = new DB_Connection();

            try
            {
                using (var connection = conn.Sheleni_Db_Connection())
                {
                    using (var command = new SqlCommand("SELECT DISTINCT BundleType, Frequency FROM AirtimeBundle", connection))
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var airtimeTypes = new ObservableCollection<string>();
                        var frequencies = new ObservableCollection<string>();
                        while (await reader.ReadAsync())
                        {
                            airtimeTypes.Add(reader["BundleType"].ToString());
                            frequencies.Add(reader["Frequency"].ToString());
                        }

                        var uniqueAirtimeTypes = airtimeTypes.Distinct().ToList();
                        var uniqueFrequencies = frequencies.Distinct().ToList();

                        AirtimeTypes = new ObservableCollection<string>(uniqueAirtimeTypes);
                        Frequencies = new ObservableCollection<string>(uniqueFrequencies);
                        Bundles = new ObservableCollection<Item>(); // Clear Bundles
                        ShowListView = false; // Hide ListView
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Unable to load Airtime data", "OK");
            }
        }

        private async void LoadDataData()
        {
            try
            {
                DB_Connection conn = new DB_Connection();

                using (var connection = conn.Sheleni_Db_Connection())
                {
                    using (var command = new SqlCommand("SELECT DISTINCT BundleType, Frequency FROM DataBundle", connection)) // Modify the SQL query
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var bundleTypes = new ObservableCollection<string>();
                        var frequencies = new ObservableCollection<string>();
                        while (await reader.ReadAsync())
                        {
                            bundleTypes.Add(reader["BundleType"].ToString());
                            frequencies.Add(reader["Frequency"].ToString());
                        }

                        var uniqueBundleTypes = bundleTypes.Distinct().ToList();
                        var uniqueFrequencies = frequencies.Distinct().ToList();

                        AirtimeTypes = new ObservableCollection<string>(uniqueBundleTypes);
                        Frequencies = new ObservableCollection<string>(uniqueFrequencies);
                        Bundles = new ObservableCollection<Item>(); // Clear Bundles
                        ShowListView = false; // Hide ListView
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Unable to load Data data", "OK");
            }
        }

        private async void FetchSelectedBundles()
        {
            try
            {
                if (IsAirtimeSelected || IsDataSelected)
                {
                    if (!string.IsNullOrEmpty(SelectedAirtimeType) && !string.IsNullOrEmpty(SelectedFrequency))
                    {
                        var selectedBundles = GetSelectedBundles();

                        if (selectedBundles != null && selectedBundles.Any())
                        {
                            Bundles = new ObservableCollection<Item>(selectedBundles);
                            ShowListView = true;
                        }
                        else
                        {
                            Bundles = new ObservableCollection<Item>(); // Clear Bundles
                            ShowListView = false; // Hide ListView
                        }
                    }
                    else
                    {
                        Bundles = new ObservableCollection<Item>(); // Clear Bundles
                        ShowListView = false; // Hide ListView
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private List<Item> GetSelectedBundles()
        {
            List<Item> bundles = new List<Item>();

            if (IsAirtimeSelected || IsDataSelected)
            {
                DB_Connection conn = new DB_Connection();
                string commandText = "";
                string itemNameColumn = "";
                string bundleTypeColumn = "";
                string priceColumn = "";

                if (IsAirtimeSelected)
                {
                    commandText = "SELECT * FROM AirtimeBundle WHERE BundleType = @BundleType AND Frequency = @Frequency";
                    itemNameColumn = "AirtimeName";
                    bundleTypeColumn = "BundleType";
                    priceColumn = "AirtimeName";
                }
                else if (IsDataSelected)
                {
                    commandText = "SELECT * FROM DataBundle WHERE BundleType = @BundleType AND Frequency = @Frequency";
                    itemNameColumn = "BundleName";
                    bundleTypeColumn = "BundleType";
                    priceColumn = "Amount";
                }

                if (!string.IsNullOrEmpty(commandText) && !string.IsNullOrEmpty(itemNameColumn) && !string.IsNullOrEmpty(bundleTypeColumn))
                {
                    try
                    {
                        using (var connection = conn.Sheleni_Db_Connection())
                        {
                            using (var command = new SqlCommand(commandText, connection))
                            {
                                command.Parameters.AddWithValue("@BundleType", SelectedAirtimeType);
                                command.Parameters.AddWithValue("@Frequency", SelectedFrequency);

                                using (var reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        string itemName = reader[itemNameColumn].ToString();

                                        // Remove the leading "R" character if present for the Price
                                        string priceStr = reader[priceColumn].ToString();
                                        if (IsDataSelected || priceStr.StartsWith("R"))
                                        {
                                            priceStr = priceStr.Substring(1);
                                        }

                                        bundles.Add(new Item
                                        {
                                            ItemName = itemName,
                                            Description = reader[bundleTypeColumn].ToString(),
                                            Price = Convert.ToDouble(priceStr).ToString()
                                        });
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle error
                        Application.Current.MainPage.DisplayAlert("Error", "Unable to retrieve selected bundles", "OK");
                    }
                }
            }

            return bundles;
        }
    }
}