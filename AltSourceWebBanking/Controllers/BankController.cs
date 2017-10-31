using System;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using AltSourceWebBanking.Models;

namespace AltSourceWebBanking.Controllers
{
    /// <summary>
    /// Transaction Types, in case we want to tostring them
    /// </summary>
    public enum TransactionTypes
    {
        REGISTER,
        DEPOSIT,
        WITHDRAWAL,
        WITHDRAWAL_ATTEMPT,
        BALANCE_CHECK,
        LOGIN,
        LOGIN_ATTEMPT,
        LOGOFF
    };

    /// <summary>
    /// Handle all of our views for our banking
    /// </summary>
    public class BankController : Controller
    {
        // small http client to get/post
        private HttpHandler client;

        /// <summary>
        /// our index view
        /// </summary>
        /// <returns>Index.cshtml</returns>
        public async Task<ActionResult> Index()
        {
            return View();
        }

        /// <summary>
        /// An About page
        /// </summary>
        /// <returns>About.cshtml</returns>
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        /// <summary>
        /// Register a user
        /// </summary>
        /// <returns>Register.cshtml</returns>
        public ActionResult Register()
        {
            ViewBag.Message = "The Registration Page";
            return View();
        }

        /// <summary>
        /// Log a user in
        /// </summary>
        /// <returns>Login.cshtml</returns>
        public ActionResult Login()
        {
            ViewBag.Message = "The Login Page";
            return View();
        }

        /// <summary>
        /// Check a user balance
        /// </summary>
        /// <returns>Balance.cshtml</returns>
        public ActionResult Balance()
        {
            ViewBag.Message = "The Balance Page";
            return View();
        }

        /// <summary>
        /// Deposit some fundage
        /// </summary>
        /// <returns>Deposit.cshtml</returns>
        public ActionResult Deposit()
        {
            return View();
        }

        /// <summary>
        /// Withdraw some dough
        /// </summary>
        /// <returns>Withdraw.cshtml</returns>
        public ActionResult Withdraw()
        {
            return View();
        }

        /// <summary>
        /// List of all your transactions
        /// </summary>
        /// <returns>Transactions.cshtml</returns>
        public ActionResult Transactions()
        {
            return View();
        }

        /// <summary>
        /// Log a user out
        /// </summary>
        /// <returns>Redirect to Index</returns>
        public ActionResult Logout()
        {
            try
            {
                this.client = HttpHandler.Instance;
                if (Request.Cookies["logged_in"] != null )
                {
                    HttpCookie loggedCookie = new HttpCookie("logged_in");
                    loggedCookie.Expires = DateTime.Now.AddDays(-1d);
                    Response.Cookies.Add(loggedCookie);
                }
                if(Request.Cookies["api_key"] != null )
                {
                    var cookieVal = Request.Cookies["api_key"].Value;
                    client.addAuthHeader(cookieVal);
                    var response = client.Get(HttpContext.Application["api_address"] + "/api/account/logout");
                    HttpCookie apiCookie = new HttpCookie("api_key");
                    apiCookie.Expires = DateTime.Now.AddDays(-1d);
                    Response.Cookies.Add(apiCookie);
                    client.removeAuthHeader(cookieVal);
                }
            }
            catch( Exception ex )
            {

            }
            return RedirectToAction("Index");
        }
    }
}