using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AltSourceWebBanking.Models
{
    public interface IHttpHandler
    {
        Task<string> Get(string uri);
        Task<string> Post(string uri);
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
                if( instance == null )
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
        public async Task<string> Post(string uri)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("", "login")
            });

            var result = await client.PostAsync(uri, content);
            var resultContent = await result.Content.ReadAsStringAsync();
            return resultContent;
        }

        /// <summary>
        /// Add custom authorization header for api token
        /// </summary>
        /// <param name="headerVal">value to set header to</param>
        public void addAuthHeader(string headerVal)
        {
            this.client.DefaultRequestHeaders.Add("Authorization", "Bearer: " + headerVal);
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
            catch( Exception ex )
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
    }
}