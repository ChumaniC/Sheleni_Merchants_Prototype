using Auth0.AuthenticationApi.Models;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Sheleni_Merchants.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Sheleni_Merchants.Services
{
    public class SheleniHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;
        private readonly string _apiUrlUser;
        private string _accessToken; // Add an access token for MTN MoMo API authentication

        public SheleniHttpClient()
        {
            // Use the base URL where your API is hosted
            _apiUrl = "https://sandbox.momodeveloper.mtn.com/collection/v1_0"; // Replace with MTN MoMo API endpoint
            _apiUrlUser = "https://sandbox.momodeveloper.mtn.com/v1_0"; // Replace with MTN MoMo API endpoint
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_apiUrlUser);
        }

        // Method to create API user
        // Method to create API user and generate secret
        public async Task<(string apiUserId, string apiSecret)> CreateApiUserAsync(string referenceId, string subscriptionKey)
        {
            try
            {
                // Construct the API endpoint URL for creating API User
                string apiUrl = $"{_apiUrlUser}/apiuser";

                // Create request headers
                //_httpClient.DefaultRequestHeaders.Clear();
                //_httpClient.DefaultRequestHeaders.Add("X-Reference-Id", referenceId);
                //_httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

                //// Send an HTTP POST request to create API User
                //HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, null);

                //if (response.IsSuccessStatusCode)
                //{
                //    // API User created successfully

                //    string apiUserId = Guid.NewGuid().ToString();
                //    string apiSecret = Guid.NewGuid().ToString(); // Generate API secret

                //    return (apiUserId, apiSecret);
                //}
                //else
                //{
                //    // Handle other HTTP status codes
                //    return (null, null); // or throw an exception or handle it as needed
                //}

                /** The following code is used for prototype purposes 
                * as a way to generate the apiUserId, since the MOMO sandbox
                * is experiencing issue withh resolving the host address
                **/

                // API User created successfully

                string apiUserId = Guid.NewGuid().ToString();
                string apiSecret = Guid.NewGuid().ToString(); // Generate API secret

                return (apiUserId, apiSecret);

                /**  Enf of prototype test code 
                 **/
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                return (null, null); // or throw an exception or handle it as needed
            }
        }

        // Method to create API Key
        public async Task<string> CreateApiKeyAsync(string apiUserId, string subscriptionKey)
        {
            try
            {
                // Construct the API endpoint URL for creating API Key
                //string apiUrl = $"{_apiUrlUser}/apiuser{apiUserId}/apikey";

                //// Create request headers
                //_httpClient.DefaultRequestHeaders.Clear();
                //_httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

                //// Send an HTTP POST request to create API Key
                //HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, null);

                //if (response.IsSuccessStatusCode)
                //{
                //    // Extract the API Key
                //    string apiKey = Guid.NewGuid().ToString();

                //    // API Key created successfully
                //    return apiKey;
                //}
                //else
                //{
                //    // Handle other HTTP status codes
                //    return null; // or throw an exception or handle it as needed
                //}

                /** The following code is used for prototype purposes 
                * as a way to generate the apiKey, since the MOMO sandbox
                * is experiencing issue withh resolving the host address
                **/

                // API User created successfully

                // Extract the API Key
                string apiKey = Guid.NewGuid().ToString();

                // API Key created successfully
                return apiKey;

                /**  Enf of prototype test code 
                 **/
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                return null; // or throw an exception or handle it as needed
            }
        }

        public async Task<string> GetAccessTokenAsync(string apiKey, string apiUser, string apiSecret)
        {
            try
            {
                var clientId = apiKey;
                var clientSecret = apiUser + ":" + apiSecret;

                // var base64Auth = Convert.ToBase64String(Encoding.UTF8.GetBytes(clientId + ":" + clientSecret));

                //        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64Auth);

                //        var content = new FormUrlEncodedContent(new[]
                //        {
                //    new KeyValuePair<string, string>("grant_type", "client_credentials"),
                //});
                //        var apiUrl = $"{_apiUrl}token/";

                //        var response = await _httpClient.PostAsync(apiUrl, content);


                //        if (response.IsSuccessStatusCode)
                //        {
                //            var responseContent = await response.Content.ReadAsStringAsync();
                //            // Parse the JSON response to extract the access token.
                //            // Extract the access token and return it for use in your API requests.
                //            var tokenObject = JsonConvert.DeserializeObject<AccessTokenResponse>(responseContent);
                //            string accessToken = tokenObject.AccessToken;
                //            return accessToken;
                //        }
                //        else
                //        {
                //            // Handle error cases here.
                //        }

                //        return null;

                /** The following code is used for prototype purposes 
                * as a way to generate the accessToken, since the MOMO sandbox
                * is experiencing issue withh resolving the host address
                **/

                // Create claims for the token (these represent the user's identity and other information)
                var claims = new[]
                {
        new Claim(JwtRegisteredClaimNames.Sub, apiUser),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        // Add more claims as needed
    };

                // Create the token
                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(30), // Token expiration time
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(clientSecret)), SecurityAlgorithms.HmacSha256)
                );

                string accessToken = new JwtSecurityTokenHandler().WriteToken(token);
                return accessToken;
                /**  Enf of prototype test code 
 **/
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<string> RequestToPayAsync(string customerMobileNumber, decimal purchaseAmount)
        {
            try
            {
                // Generate a reference ID
                var referenceId = Guid.NewGuid().ToString();

                //// Obtain the API key and API user ID
                //(string apiUserId, string apiSecret) = await CreateApiUserAsync(referenceId, "c73400d2785048cc9567da9c7b792c0a");
                //string apiKey = await CreateApiKeyAsync(apiUserId, "c73400d2785048cc9567da9c7b792c0a");

                //// Use the API key and API user ID for access token
                //string accessToken = await GetAccessTokenAsync(apiKey, apiUserId, apiSecret);

                //// Construct the API endpoint URL for Request to Pay
                //string apiUrl = $"{_apiUrl}/requesttopay";

                //// Create the request body
                //var requestContent = new
                //{
                //    amount = purchaseAmount,
                //    currency = "ZAR", // Replace with the appropriate currency
                //    externalId = referenceId, // Use the generated reference ID
                //    payer = new
                //    {
                //        partyIdType = "MSISDN", // Mobile number is the party ID type
                //        partyId = customerMobileNumber // Customer's mobile number
                //    },
                //    payerMessage = "Payment for purchase", // Customize as needed
                //    payeeNote = "Thank you for your purchase" // Customize as needed
                //};

                //// Serialize the request body to JSON
                //var requestBody = JsonConvert.SerializeObject(requestContent);

                //// Create an HTTP request message with headers
                //var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
                //request.Headers.Add("Authorization", $"Bearer {_accessToken}");
                //request.Headers.Add("X-Reference-Id", referenceId); // Add the reference ID
                //request.Headers.Add("X-Target-Environment", "sandbox"); // Replace with "production" for the live environment
                //request.Content = new StringContent(requestBody, System.Text.Encoding.UTF8, "application/json");

                //// Send an HTTP POST request to the MTN MoMo API
                //HttpResponseMessage response = await _httpClient.SendAsync(request);

                //if (response.IsSuccessStatusCode)
                //{
                //    // Payment request sent successfully
                //    return "Payment request sent successfully";
                //}
                //else
                //{
                //    // Handle the case where the payment request failed
                //    return "Payment request failed";
                //}

                /** The following code is used for prototype purposes 
              * as a way to generate the accessToken, since the MOMO sandbox
              * is experiencing issue withh resolving the host address
              **/

                // Payment request sent successfully
                return "Payment request sent successfully";
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                return null; // or throw an exception or handle it as needed
            }
        }

        public async Task<List<string>> GetAllServiceNamesAsync()
        {
            try
            {
                // Construct the API endpoint URL
                string apiUrl = $"{_apiUrl}/api/services/all_services";

                // Send an HTTP GET request to the API
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    // Read the response content as a string
                    string content = await response.Content.ReadAsStringAsync();

                    // Deserialize the JSON string to a list of strings
                    List<string> serviceNames = JsonConvert.DeserializeObject<List<string>>(content);

                    return serviceNames;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    // Handle 404 Not Found error
                    return null; // or throw an exception or handle it as needed
                }
                else
                {
                    // Handle other HTTP status codes
                    return null; // or throw an exception or handle it as needed
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                return null; // or throw an exception or handle it as needed
            }
        }

        // Method to get API user details
        public async Task<ApiUserDetails> GetApiUserDetailsAsync(string apiUserId)
        {
            try
            {
                // Construct the API endpoint URL for getting API user details
                string apiUrl = $"{_apiUrl}/api/momo/getApiUserDetails/{apiUserId}";

                // Send an HTTP GET request to the API
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    // Read the response content as a JSON string
                    string content = await response.Content.ReadAsStringAsync();

                    // Deserialize the JSON response to get API user details
                    ApiUserDetails userDetails = JsonConvert.DeserializeObject<ApiUserDetails>(content);

                    return userDetails;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    // Handle 404 Not Found error
                    return null; // or throw an exception or handle it as needed
                }
                else
                {
                    // Handle other HTTP status codes
                    string errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"API request failed with status code {(int)response.StatusCode}. Response content: {errorContent}");
                }
            }
            catch (HttpRequestException ex)
            {
                // Handle HTTP request exceptions (e.g., network issues)
                // Log or handle the exception as needed
                throw ex;
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                // Log or handle the exception as needed
                throw ex;
            }
        }
    }
}