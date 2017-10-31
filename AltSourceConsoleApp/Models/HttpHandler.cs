using System;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace AltSourceConsoleApp.Models
{
    public interface IHttpHandler
    {
        Task<string> Get(string uri);
        Task<string> Post(string uri, string content);
    }

    /// <summary>
    /// Small singleton to handle http requests
    /// </summary>
    public class HttpHandler : IHttpHandler
    {
        private CookieContainer cookies;
        private HttpClientHandler handler;
        private HttpClient client;
        private static HttpHandler instance;

        /// <summary>
        /// Set up our handler and cookies for the client
        /// </summary>
        private HttpHandler()
        {
            this.cookies = new CookieContainer();
            this.handler = new HttpClientHandler();
            this.handler.CookieContainer = this.cookies;
            this.client = new HttpClient(this.handler);
        }

        /// <summary>
        /// Instance property to get the singleton instance
        /// </summary>
        public static HttpHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new HttpHandler();
                }
                return instance;
            }
        }

        /// <summary>
        /// Perform a POST to uri
        /// </summary>
        /// <param name="uri">the URI to send the request to</param>
        /// <returns>json string</returns>
        public async Task<string> Post(string uri, string content)
        {
            StringContent postContent;
            if (content == null)
            {
                postContent = null;
            }
            else
            { 
                postContent = new StringContent(content.ToString(), Encoding.UTF8, "application/json");
            }
            try
            {
                var result = client.PostAsync(uri, postContent).Result;
                result.EnsureSuccessStatusCode();
                return await result.Content.ReadAsStringAsync();
            }
            catch( Exception hre )
            {
                return hre.Message.ToString();
            }
        }

        /// <summary>
        /// Construct a request message to post with custom header definition
        /// </summary>
        /// <param name="uri">the uri to post to</param>
        /// <param name="headerVal">the user's api key</param>
        /// <param name="content">string representation of data (json)</param>
        /// <returns>returns request value</returns>
        public async Task<string> Post(string uri, string headerVal, string content)
        {
            try
            {
                //build our request
                HttpRequestMessage request = GetPostMessage(uri, headerVal, content);
                //get and send result back to client
                var result = client.SendAsync(request).Result;
                result.EnsureSuccessStatusCode();
                return await result.Content.ReadAsStringAsync();
            }
            catch( Exception hre)
            {
                return hre.Message.ToString();
            }
        }

        /// <summary>
        /// Build a requestmessage of Get type with custom header definition support
        /// </summary>
        /// <param name="uri">uri to send request to</param>
        /// <param name="headerVal">user's api key</param>
        /// <returns>prepared request</returns>
        private HttpRequestMessage GetGetMessage(string uri, string headerVal)
        {
            var request = new HttpRequestMessage
            {
                Content = null,
                RequestUri = new Uri(uri),
                Method = HttpMethod.Get
            };

            if (headerVal != null && headerVal != "")
                request.Headers.TryAddWithoutValidation("Authorization", "Bearer: " + headerVal);

            return request;
        }

        /// <summary>
        /// Build a requestmessage of Get type with custom header definition support
        /// </summary>
        /// <param name="uri">uri to send request to</param>
        /// <param name="headerVal">user's api key</param>
        /// <param name="content">the json string to send in the request</param>
        /// <returns>prepared request</returns>
        private HttpRequestMessage GetPostMessage(string uri, string headerVal, string content)
        {
            StringContent postContent;

            if (content != null)
                postContent = new StringContent(content.ToString(), Encoding.UTF8, "application/json");
            else
                postContent = null;

            var request = new HttpRequestMessage
            {
                Content = postContent,
                RequestUri = new Uri(uri),
                Method = HttpMethod.Post
            };

            if( content != null )
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json; charset=utf-8");

            if( headerVal != null && headerVal != "")
                request.Headers.TryAddWithoutValidation("Authorization", "Bearer: " + headerVal);

            return request;
        }

        /// <summary>
        /// Add custom authorization header for api token
        /// </summary>
        /// <param name="headerVal">value to set header to</param>
        public void addAuthHeader(string headerVal)
        {
            if (this.client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer: " + headerVal))
                BankApp.WriteToConsole("Added authorization header");
            else
                BankApp.WriteToConsole("Failed to add authorization header");

        }

        /// <summary>
        /// remove authorization header
        /// </summary>
        /// <param name="headerVal">name of header to remove</param>
        public void removeAuthHeader(string headerVal)
        {
            try
            {
                this.client.DefaultRequestHeaders.Remove("Authorization");
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// simple method to perform GET on a URI
        /// </summary>
        /// <param name="uri">URI to perform GET action on</param>
        /// <returns>json string</returns>
        public async Task<string> Get(string uri)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var resultString = await response.Content.ReadAsStringAsync();
                    return resultString;
                }
                else
                {
                    return response.StatusCode.ToString();
                }
            }
            catch( Exception ex )
            {
                return ex.Message.ToString();
            }
        }

        /// <summary>
        /// Perform a get using a requestmessage supporting custom header definition
        /// </summary>
        /// <param name="uri">uri to send request to</param>
        /// <param name="headerVal">user's api key</param>
        /// <returns>server's return value</returns>
        public async Task<string> Get(string uri, string headerVal)
        {
            try
            {
                HttpRequestMessage request = GetGetMessage(uri, headerVal);

                var result = client.SendAsync(request).Result;
                result.EnsureSuccessStatusCode();
                return await result.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
    }
}
