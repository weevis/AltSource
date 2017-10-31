using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using AltSourceConsoleApp.Models;
using System.Net;
using System.Net.Http;
using AltSourceConsoleApp;

namespace AltSourceConsoleApp.Controllers
{
    /// <summary>
    /// Our command class invoked via reflection
    /// </summary>
    public static class Users
    {
        private static HttpHandler handler;
        /// <summary>
        /// Create a user
        /// </summary>
        /// <param name="firstName">First Name</param>
        /// <param name="LastName">Last Name</param>
        /// <param name="Email">Email</param>
        /// <param name="UserName">UserName</param>
        /// <param name="Password">Password</param>
        /// <returns>on success a valid user in json form</returns>
        public static async Task<string> Create(string firstName, string LastName, string Email, string UserName, string Password)
        {
            handler = HttpHandler.Instance;
            User user = new User();
            user.FirstName = firstName;
            user.LastName = LastName;
            user.Email = Email;
            user.UserName = UserName;
            user.Password = Password;

            //post our user data to the API to register, hopefully get a valid user back (json)
            try
            {
                string uri = "http://localhost:8000/api/account/create";
                string json = JsonConvert.SerializeObject(user);
                string message = await handler.Post(uri, json);
                if (message == null)
                    return null;

                return message;
            }
            catch( HttpRequestException hre )
            {
                return hre.Message.ToString();
            }
        }

        /// <summary>
        /// Login to the API
        /// </summary>
        /// <param name="UserName">UserName</param>
        /// <param name="Email">Email</param>
        /// <param name="Password">Password</param>
        /// <returns>on success a User object populated with new API key</returns>
        public static async Task<string> Login(string UserName, string Email, string Password)
        {
            handler = HttpHandler.Instance;
            User user = new User();
            user.UserName = UserName;
            user.Email = Email;
            user.Password = Password;

            //Post our user data to the endpoint, get API key back, mark as logged in
            try
            {
                string uri = "http://localhost:8000/api/account/login";
                string json = JsonConvert.SerializeObject(user);
                string message = await handler.Post(uri, json);

                if (message == null)
                    return null;

                BankApp.user = JsonConvert.DeserializeObject<User>(message);
                BankApp.user.logged_in = true;

                return message;
            }
            catch( HttpRequestException hre )
            {
                return hre.Message.ToString();
            }
            catch( Exception ex )
            {
                return ex.Message.ToString();
            }
        }

        /// <summary>
        /// Get a user's balance
        /// </summary>
        /// <returns>current balance</returns>
        public static async Task<string> GetBalance()
        {
            if (BankApp.user.logged_in == false)
                return "Please log in";

            //send a get request and get the balance
            handler = HttpHandler.Instance;
            try
            {
                string uri = "http://localhost:8000/api/account/balance";
                string message = await handler.Get(uri, BankApp.user.api_key);
                return message;
            }
            catch( HttpRequestException hre )
            {
                return hre.Message.ToString();
            }
        }

        /// <summary>
        /// Deposit funds into an account
        /// </summary>
        /// <param name="amount">double amount</param>
        /// <returns>returns current balance after deposit</returns>
        public static async Task<string> Deposit(double amount )
        {
            if (BankApp.user.logged_in == false)
                return "Please log in";

            handler = HttpHandler.Instance;
            try
            {
                string uri = "http://localhost:8000/api/account/deposit/" + amount;
                string message = await handler.Post(uri, BankApp.user.api_key, null);
                return message;
            }
            catch( HttpRequestException hre )
            {
                return hre.Message.ToString();
            }
        }

        /// <summary>
        /// withdraw from a user account
        /// </summary>
        /// <param name="amount">double amount</param>
        /// <returns>current balance after withdrawal</returns>
        public static async Task<string> Withdraw(double amount)
        {
            if (BankApp.user.logged_in == false)
                return "Please log in";

            handler = HttpHandler.Instance;
            try
            {
                string uri = "http://localhost:8000/api/account/withdraw/" + amount;
                string message = await handler.Post(uri, BankApp.user.api_key, null);
                return message;
            }
            catch (HttpRequestException hre)
            {
                return hre.Message.ToString();
            }
        }

        /// <summary>
        /// get list of transactions
        /// </summary>
        /// <returns>json string of transactions</returns>
        public static async Task<string> Transactions()
        {
            if (BankApp.user.logged_in == false)
                return "Please log in";

            handler = HttpHandler.Instance;
            try
            {
                string uri = "http://localhost:8000/api/account/transactions";
                string message = await handler.Get(uri, BankApp.user.api_key);
                return message;
            }
            catch( HttpRequestException hre )
            {
                return hre.Message.ToString();
            }
        }

        /// <summary>
        /// log a user out
        /// </summary>
        /// <returns>success</returns>
        public static async Task<string> Logout()
        {
            if (BankApp.user.logged_in == false)
                return "No need to logout";

            handler = HttpHandler.Instance;
            try
            {
                string uri = "http://localhost:8000/api/account/logout";
                string message = await handler.Get(uri, BankApp.user.api_key);

                BankApp.user.logged_in = false;
                BankApp.user.api_key = "";
                return message;
            }
            catch( HttpRequestException hre )
            {
                return hre.Message.ToString();
            }
        }
    }
}
