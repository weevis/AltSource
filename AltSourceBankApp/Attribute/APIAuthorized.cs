using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using AltSourceBankAppAPI.Models;
using AltSourceBankAppAPI.Controllers;
using AltSourceBankAppAPI.Contexts;

namespace AltSourceBankAppAPI.Attribute
{
    /* Adding a custom attribute decorator to our routes to verify API key */
    public class APIAuthorizedAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Find a user by their API token
        /// </summary>
        /// <param name="token">The API Token</param>
        /// <param name="db">Database Context</param>
        /// <returns>Users object</returns>
        private Users getUserByToken(string token, ApiContext db)
        {
            Users u = db.Users.Where(m => m.API_KEY == token).FirstOrDefault();

            if (u == null)
                return null;

            return u;
        }

        /// <summary>
        /// When the filter executes we will pull the header and check it, we will 
        /// change context status code if necessary
        /// </summary>
        /// <param name="context">The current context</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            var authToken = "";
            var realToken = "";

            var db = ((AccountController)context.Controller).context;
            if (context.HttpContext.Request.Headers["Authorization"] != "")
            {
                authToken = context.HttpContext.Request.Headers["Authorization"];
            }
            else
            {
                context.HttpContext.Response.StatusCode = 400;
                context.Result = new ContentResult()
                {
                    Content = "Missing authorization header"
                };
                return;
            }
            if( authToken == null )
            {
                context.HttpContext.Response.StatusCode = 400;
                context.Result = new ContentResult()
                {
                    Content = "Missing authorization header"
                };
                return;
            }
            else if (authToken.Length > 0 && authToken.Contains("Bearer:") && authToken.Contains(" "))
            {
                realToken = authToken.Split()[1];

                if( getUserByToken(realToken, db) == null )
                {
                    context.HttpContext.Response.StatusCode = 403;
                    context.Result = new ContentResult()
                    {
                        Content = "Wrong API Key"
                    };
                    return;
                }
            }
            else
            {
                context.HttpContext.Response.StatusCode = 400;
                context.Result = new ContentResult()
                {
                    Content = "Missing or malformed API Key"
                };
                return;
            }
            

        }
    }
}
