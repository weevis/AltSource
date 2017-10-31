using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AltSourceBankAppAPI.Models;
using Microsoft.AspNetCore.Identity;
using AltSourceBankAppAPI.Entity;
using AltSourceBankAppAPI.Contexts;
using AltSourceBankAppAPI.Service;
using Microsoft.AspNetCore.Cors;
using System.Security.Cryptography;
using AltSourceBankAppAPI.Attribute;

namespace AltSourceBankAppAPI.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        public readonly ApiContext context;
        private readonly UserManager<UserEntity> userManager;
        private readonly SignInManager<UserEntity> signinManager;
        private readonly IPasswordHasher<UserEntity> passwordHasher;
        private readonly ITransactionService transactionService;

        /// <summary>
        /// Our base controller for our banking API
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="userManager">Managing users (finding/creating)</param>
        /// <param name="signInManager">managing sign ins</param>
        /// <param name="passwordHasher">we don't want plain text passwords</param>
        /// <param name="transService">this is our registered transaction service to record activity</param>
        public AccountController(ApiContext context, UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager, IPasswordHasher<UserEntity> passwordHasher, ITransactionService transService)
        {
            this.context = context;
            this.userManager = userManager;
            this.signinManager = signInManager;
            this.passwordHasher = passwordHasher;
            this.transactionService = transService;
        }

        /// <summary>
        /// POST method to CREATE a user
        /// </summary>
        /// <param name="users">Users model populated from POST</param>
        /// <returns>Created(201) on success</returns>
        // POST: api/account/create
        [HttpPost("create")]
        [EnableCors("CorsPolicy")]
        public async Task<IActionResult> Create([FromBody] Users users)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(
                    ModelState.Values.SelectMany(m => m.Errors)
                        .Select(modelError => modelError.ErrorMessage)
                        .ToList());
            }

            var user = new UserEntity { UserName = users.UserName, Email = users.Email };

            var result = await this.userManager.CreateAsync(user, users.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(err => err.Description).ToList());
            }
            users.Password = this.passwordHasher.HashPassword(user, users.Password);
            context.Add(users);
            await context.SaveChangesAsync();

            var trans = new Transactions
            {
                TransactionTime = DateTime.Now,
                TransactionType = TransactionTypes.REGISTER,
                EndBalance = users.Balance,
                Description = "Register Event",
                ChangeAmount = 0,
                UserID = users.Id,
                StartBalance = 0
            };

            transactionService.Add(trans);
            await this.signinManager.SignOutAsync();

            return Created("/Bank/Login", users);
        }

        /// <summary>
        /// POST LOGIN method
        /// </summary>
        /// <param name="users">Username, Email, Password from POST body</param>
        /// <returns>Ok(user) with populated API key on success</returns>
        // POST: api/account/login
        [HttpPost("login")]
        [EnableCors("CorsPolicy")]
        public async Task<IActionResult> Login([FromBody] Users users)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.Values.SelectMany(m => m.Errors)
                        .Select(modelError => modelError.ErrorMessage)
                        .ToList());
            }

            // find our user by their email using the user manager
            UserEntity u = await this.userManager.FindByEmailAsync(users.Email);
            if (u == null)
                return BadRequest();

            //sign the user in
            var result = await this.signinManager.PasswordSignInAsync(u.UserName, users.Password, false, false);

            //populate the users model object
            var user = getUserByName(u.UserName);

            if (user == null)
                return NotFound();

            if (!result.Succeeded)
            {
                var badtrans = new Transactions
                {
                    TransactionTime = DateTime.Now,
                    TransactionType = TransactionTypes.LOGIN_ATTEMPT,
                    EndBalance = user.Balance,
                    Description = "Login Failure",
                    ChangeAmount = 0,
                    UserID = user.Id,
                    StartBalance = user.Balance
                };
                return BadRequest();
            }

            //create new API key on every login
            user.API_KEY = generateAPIKey();
            context.Users.Add(user);
            context.Entry(user).State = EntityState.Modified;
            await context.SaveChangesAsync();
            var trans = new Transactions
            {
                TransactionTime = DateTime.Now,
                TransactionType = TransactionTypes.LOGIN,
                EndBalance = user.Balance,
                Description = "Login Event",
                ChangeAmount = 0,
                UserID = user.Id,
                StartBalance = user.Balance
            };
            transactionService.Add(trans);
            user.Password = "";
            return Ok(user);
        }

        /// <summary>
        /// Fill a Users model from the provided Identity Name
        /// </summary>
        /// <param name="UserName">string UserName</param>
        /// <returns>Users model object or null</returns>
        private Users getUserByName(string UserName)
        {
            Users u = this.context.Users.Where(m => m.UserName == UserName).FirstOrDefault();

            if (u == null)
                return null;
            else
                return u;

        }

        /// <summary>
        /// Find a user by their API KEY
        /// </summary>
        /// <param name="key">the API KEY</param>
        /// <returns>Users model object or null</returns>
        private Users getUserByKey(string key)
        {
            Users u = this.context.Users.Where(m => m.API_KEY == key).FirstOrDefault();

            if (u == null)
                return null;
            else
                return u;
        }

        /// <summary>
        /// Fetch API Key from Request Header
        /// </summary>
        /// <returns>Authorization API KEY or null</returns>
        private string getAPIKey()
        {
            if (Request.Headers["Authorization"] != "")
            {
                var authToken = Request.Headers["Authorization"];
                return authToken.ToString().Split()[1];
            }
            return null;
        }

        /// <summary>
        /// Generate "random" API KEY and store as base64
        /// </summary>
        /// <returns>base64 encoded API KEY</returns>
        private string generateAPIKey()
        {
            var key = new byte[32];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(key);
            }
            var apiKey = Convert.ToBase64String(key);
            return apiKey;
        }

        /// <summary>
        /// GET Balance method
        /// </summary>
        /// <returns>Users account balance</returns>
        // GET: api/account/balance
        [HttpGet("balance")]
        [EnableCors("CorsPolicy")]
        [APIAuthorized]
        public IActionResult Balance()
        {
            var user = this.getUserByKey(getAPIKey());

            if (user == null)
                return BadRequest();

            Transactions trans = new Transactions
            {
                TransactionTime = DateTime.Now,
                TransactionType = TransactionTypes.BALANCE_CHECK,
                Description = "Balance Check",
                ChangeAmount = 0,
                StartBalance = user.Balance,
                UserID = user.Id,
                EndBalance = user.Balance
            };
            transactionService.Add(trans);
            return Ok(user.Balance);
        }

        /// <summary>
        /// Get list of transactions for User
        /// </summary>
        /// <returns>Ok(JSON transaction list)</returns>
        // GET api/transactions
        [HttpGet("transactions")]
        [EnableCors("CorsPolicy")]
        [APIAuthorized]
        public IActionResult Transactions()
        {
            var user = this.getUserByKey(getAPIKey());

            if (user == null)
                return BadRequest();

            var transactionList = transactionService.GetAllByID(user.Id);
            
            if( transactionList.Count<Transactions>() > 0 )
                return Ok(transactionList);

            return NotFound();
        }

        /// <summary>
        /// Withdraw some money
        /// </summary>
        /// <param name="amount">Amount to withdraw</param>
        /// <returns>Ok(Balance) on success</returns>
        // POST: api/account/Withdraw
        [HttpPost("withdraw/{amount}")]
        [EnableCors("CorsPolicy")]
        [APIAuthorized]
        public async Task<IActionResult> Withdraw(double amount)
        {
            var u = this.getUserByKey(getAPIKey());

            if (u == null)
                return BadRequest();

            if (amount > u.Balance)
            {
                Transactions badtrans = new Transactions
                {
                    TransactionTime = DateTime.Now,
                    TransactionType = TransactionTypes.WITHDRAWAL_ATTEMPT,
                    Description = "Failed Withdrawal Attempt",
                    EndBalance = u.Balance,
                    ChangeAmount = amount,
                    StartBalance = u.Balance,
                    UserID = u.Id
                };
                transactionService.Add(badtrans);
                return Forbid("Not allowed to withdraw more funds than available.");
            }

            var startBalance = u.Balance;
            u.Balance = u.Balance - amount;
            this.context.Entry(u).State = EntityState.Modified;
            int x = await this.context.SaveChangesAsync();

            Transactions trans = new Transactions
            {
                TransactionTime = DateTime.Now,
                TransactionType = TransactionTypes.WITHDRAWAL,
                Description = "Withdrawal",
                EndBalance = u.Balance,
                ChangeAmount = amount,
                StartBalance = startBalance,
                UserID = u.Id
            };
            transactionService.Add(trans);

            return Ok(u.Balance);
        }

        /// <summary>
        /// Log user out of system
        /// </summary>
        /// <returns>Ok on success</returns>
        // GET: api/logout
        [HttpGet("logout")]
        [EnableCors("CorsPolicy")]
        [APIAuthorized]
        public async Task<IActionResult> Logout()
        {
            var u = this.getUserByKey(getAPIKey());

            if (u == null)
                return BadRequest();

            Transactions trans = new Transactions
            {
                TransactionTime = DateTime.Now,
                TransactionType = TransactionTypes.LOGOFF,
                Description = "Logoff",
                UserID = u.Id,
                ChangeAmount = 0,
                StartBalance = u.Balance,
                EndBalance = u.Balance
            };

            transactionService.Add(trans);
            await this.signinManager.SignOutAsync();
            return Ok();
        }

        /// <summary>
        /// Deposit some money
        /// </summary>
        /// <param name="amount">Amount to deposit, double</param>
        /// <returns>Ok(balance)</returns>
        // POST: api/account/deposit
        [HttpPost("deposit/{amount}")]
        [EnableCors("CorsPolicy")]
        [APIAuthorized]
        public async Task<IActionResult> Deposit(double amount)
        {
            var u = this.getUserByKey(getAPIKey());

            if (u == null)
                return BadRequest();

            var startBalance = u.Balance;
            u.Balance = u.Balance + amount;
            this.context.Entry(u).State = EntityState.Modified;
            await this.context.SaveChangesAsync();

            Transactions trans = new Transactions
            {
                TransactionTime = DateTime.Now,
                TransactionType = TransactionTypes.DEPOSIT,
                Description = "Deposit Type",
                StartBalance = startBalance,
                EndBalance = u.Balance,
                ChangeAmount = amount,
                UserID = u.Id
            };

            transactionService.Add(trans);
            return Ok(u.Balance);
        }

        /// <summary>
        /// See if a user exists
        /// </summary>
        /// <param name="id">user id guid</param>
        /// <returns>true or false</returns>
        private bool UsersExists(string id)
        {
            return context.Users.Any(e => e.Id == id);
        }
    }
}
