using Sheleni_Merchants.Models;
using Sheleni_Merchants.Services;
using Sheleni_Merchants.Views;
using System;
using System.Data.SqlClient;
using System.Windows.Input;
using Xamarin.Forms;

namespace Sheleni_Merchants.ViewModels
{
    public class LoginPageViewModel : BaseViewModel
    {
        public string PhoneNumber { get; set; }

        public ICommand LoginCommand { get; set; }
        public ICommand RegisterCommand { get; set; }

        public LoginPageViewModel()
        {
            Title = "Login";

            LoginCommand = new Command(OnLogin);
        }

        private void OnLogin()
        {
            if (!string.IsNullOrEmpty(PhoneNumber))
            {
                // Here, validate the user login based on the PhoneNumber
                if (IsCustomerLogin(PhoneNumber))
                {
                    // Fetch customer details from the database
                    Customer customer = GetCustomerDetails(PhoneNumber);

                    // Proceed with the customer login
                    // Navigate or perform further actions after successful login
                    HandleCustomerLogin(customer);
                }
                else if (IsMerchantLogin(PhoneNumber))
                {
                    // Fetch merchant details from the database
                    Merchant merchant = GetMerchantDetails(PhoneNumber);

                    // Proceed with the merchant login
                    // Navigate or perform further actions after successful login
                    HandleMerchantLogin(merchant);
                }
                else
                {
                    // Handle scenario where no user is found
                    // e.g., show an error message for invalid login details
                }
            }
        }

        // Check if the user is a customer
        public bool IsCustomerLogin(string phoneNumber)
        {
            bool isCustomer = false;
            DB_Connection con = new DB_Connection();

            try
            {
                using (SqlConnection connection = con.Sheleni_Db_Connection())
                {
                    string query = "SELECT COUNT(*) FROM Customer WHERE mobile_number = @mobile_number";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@mobile_number", phoneNumber);

                        int count = (int)command.ExecuteScalar(); // Execute the query

                        if (count > 0)
                        {
                            isCustomer = true; // Customer found
                            connection.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle the error as required
                // For instance, log the error:
                Console.WriteLine("An error occurred: " + ex.Message);
                // Or handle it as needed for your application
            }

            return isCustomer;
        }

        // Check if the user is a merchant
        public bool IsMerchantLogin(string phoneNumber)
        {
            bool isMerchant = false;

            DB_Connection con = new DB_Connection();

            try
            {
                using (SqlConnection connection = con.Sheleni_Db_Connection())
                {
                    string query = "SELECT COUNT(*) FROM Merchant WHERE contact_number = @contact_number";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@contact_number", phoneNumber);

                        int count = (int)command.ExecuteScalar(); // Execute the query

                        if (count > 0)
                        {
                            isMerchant = true; // Merchant found
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle the error as required
                // For instance, log the error:
                Console.WriteLine("An error occurred: " + ex.Message);
                // Or handle it as needed for your application
            }

            return isMerchant;
        }

        // Retrieve customer details
        public Customer GetCustomerDetails(string phoneNumber)
        {
            Customer customer = null;

            DB_Connection con = new DB_Connection();

            try
            {
                using (SqlConnection connection = con.Sheleni_Db_Connection())
                {
                    string query = "SELECT * FROM Customer WHERE mobile_number = @mobile_number";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@mobile_number", phoneNumber);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Map the retrieved data to the Customer object
                                customer = new Customer
                                {
                                    CustomerId = (int)reader["customer_id"],
                                    Name = (string)reader["customer_name"],
                                    PhoneNumber = (string)reader["mobile_number"],
                                    // Include other properties as needed
                                    // For instance: Name, Address, etc.
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle the error as required
                Console.WriteLine("An error occurred: " + ex.Message);
                // Or handle it as needed for your application
            }

            return customer;
        }

        // Retrieve merchant details

        public Merchant GetMerchantDetails(string phoneNumber)
        {
            Merchant merchant = null;

            DB_Connection con = new DB_Connection();

            try
            {
                using (SqlConnection connection = con.Sheleni_Db_Connection())
                {
                    string query = "SELECT * FROM Merchant WHERE contact_number = @contact_number";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@contact_number", phoneNumber);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Map the retrieved data to the Merchant object
                                merchant = new Merchant
                                {
                                    MerchantId = (int)reader["merchant_id"],
                                    Name = (string)reader["merchant_name"],
                                    PhoneNumber = (string)reader["contact_number"],
                                    // Include other properties as needed
                                    // For instance: Name, BusinessName, etc.
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle the error as required
                Console.WriteLine("An error occurred: " + ex.Message);
                // Or handle it as needed for your application
            }

            return merchant;
        }

        // Perform actions after successful customer login
        private void HandleCustomerLogin(Customer customer)
        {
            // Perform further actions after a successful customer login
            // For the prototype, you can navigate to the customer dashboard or perform other necessary actions

            // For example, navigating to the customer's dashboard page
            // Make sure your navigation logic and page names match your application structure
            // This code assumes that your navigation is within the Xamarin.Forms framework
            // Replace "CustomerDashboardPage" with the actual name of your customer's dashboard page
            Application.Current.MainPage.Navigation.PushAsync(new TransactionsPage());
        }

        // Perform actions after successful merchant login
        private void HandleMerchantLogin(Merchant merchant)
        {
            // Perform further actions after a successful merchant login
            // For the prototype, you can navigate to the merchant's dashboard or perform other necessary actions

            // For example, navigating to the merchant's dashboard page
            // Make sure your navigation logic and page names match your application structure
            // This code assumes that your navigation is within the Xamarin.Forms framework
            // Replace "MerchantDashboardPage" with the actual name of your merchant's dashboard page
            Application.Current.MainPage.Navigation.PushAsync(new DashboardPage());
        }
    }
}