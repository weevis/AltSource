using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using AltSourceBankAppAPI.Controllers;
using AltSourceBankAppAPI.Models;
using AltSourceBankAppAPI.Entity;
using AltSourceBankAppAPI.Contexts;
using AltSourceBankAppAPI.Service;

namespace AltSourceBankApp.Tests
{
    public class AccountControllerTest
    {
        [Fact]
        public async void TestValidLogin()
        {
            var mockDatabase = new Mock<ApiContext>();
            var userManager = new Mock<UserManager<UserEntity>>();
            var signInManager = new Mock<SignInManager<UserEntity>>();
            var passwordHasher = new Mock<IPasswordHasher<UserEntity>>();
            var transactionService = new Mock<ITransactionService>();

            AccountController controller = new AccountController(mockDatabase.Object, userManager.Object, signInManager.Object, passwordHasher.Object, transactionService.Object);
            Users u = new Users()
            {
                Email = "test@test.com",
                UserName = "test",
                Password = "T3sting!."
            };

            var result = await controller.Login(u);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async void TestInvalidLogin()
        {
            var mockDatabase = new Mock<ApiContext>();
            var userManager = new Mock<UserManager<UserEntity>>();
            var signInManager = new Mock<SignInManager<UserEntity>>();
            var passwordHasher = new Mock<IPasswordHasher<UserEntity>>();
            var transactionService = new Mock<ITransactionService>();

            AccountController controller = new AccountController(mockDatabase.Object, userManager.Object, signInManager.Object, passwordHasher.Object, transactionService.Object);
            Users u = new Users();

            var result = await controller.Login(u);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async void TestInvalidLoginNotFound()
        {
            var mockDatabase = new Mock<ApiContext>();
            var userManager = new Mock<UserManager<UserEntity>>();
            var signInManager = new Mock<SignInManager<UserEntity>>();
            var passwordHasher = new Mock<IPasswordHasher<UserEntity>>();
            var transactionService = new Mock<ITransactionService>();

            AccountController controller = new AccountController(mockDatabase.Object, userManager.Object, signInManager.Object, passwordHasher.Object, transactionService.Object);
            Users u = new Users()
            {
                Email = "test@test.com",
                UserName = "test22",
                Password = "T3sting!."
            };

            var result = await controller.Login(u);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void TestRegister()
        {
            var mockDatabase = new Mock<ApiContext>();
            var userManager = new Mock<UserManager<UserEntity>>();
            var signInManager = new Mock<SignInManager<UserEntity>>();
            var passwordHasher = new Mock<IPasswordHasher<UserEntity>>();
            var transactionService = new Mock<ITransactionService>();

            AccountController controller = new AccountController(mockDatabase.Object, userManager.Object, signInManager.Object, passwordHasher.Object, transactionService.Object);
            Users u = new Users()
            {
                FirstName = "Test",
                LastName = "Test,",
                Email = "test@test.com",
                UserName = "test",
                Password = "T3sting!."
            };

            var result = await controller.Create(u);

            Assert.IsType<CreatedResult>(result);
        }

        [Fact]
        public async void TestFailedRegister()
        {
            var mockDatabase = new Mock<ApiContext>();
            var userManager = new Mock<UserManager<UserEntity>>();
            var signInManager = new Mock<SignInManager<UserEntity>>();
            var passwordHasher = new Mock<IPasswordHasher<UserEntity>>();
            var transactionService = new Mock<ITransactionService>();

            AccountController controller = new AccountController(mockDatabase.Object, userManager.Object, signInManager.Object, passwordHasher.Object, transactionService.Object);
            Users u = new Users()
            {
                FirstName = "Test",
                LastName = "Test,",
                UserName = "test",
                Password = "T3sting!."
            };

            var result = await controller.Create(u);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async void TestDeposit()
        {
            var mockDatabase = new Mock<ApiContext>();
            var userManager = new Mock<UserManager<UserEntity>>();
            var signInManager = new Mock<SignInManager<UserEntity>>();
            var passwordHasher = new Mock<IPasswordHasher<UserEntity>>();
            var transactionService = new Mock<ITransactionService>();

            AccountController controller = new AccountController(mockDatabase.Object, userManager.Object, signInManager.Object, passwordHasher.Object, transactionService.Object);

            double amount = 123.45;
            var result = await controller.Deposit(amount);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async void TestFailedDeposit()
        {
            var mockDatabase = new Mock<ApiContext>();
            var userManager = new Mock<UserManager<UserEntity>>();
            var signInManager = new Mock<SignInManager<UserEntity>>();
            var passwordHasher = new Mock<IPasswordHasher<UserEntity>>();
            var transactionService = new Mock<ITransactionService>();

            AccountController controller = new AccountController(mockDatabase.Object, userManager.Object, signInManager.Object, passwordHasher.Object, transactionService.Object);

            double amount = 123.45;
            var result = await controller.Deposit(amount);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async void TestWithdraw()
        {
            var mockDatabase = new Mock<ApiContext>();
            var userManager = new Mock<UserManager<UserEntity>>();
            var signInManager = new Mock<SignInManager<UserEntity>>();
            var passwordHasher = new Mock<IPasswordHasher<UserEntity>>();
            var transactionService = new Mock<ITransactionService>();

            AccountController controller = new AccountController(mockDatabase.Object, userManager.Object, signInManager.Object, passwordHasher.Object, transactionService.Object);

            double amount = 123.45;
            var result = await controller.Withdraw(amount);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async void TestFailedWithdraw()
        {
            var mockDatabase = new Mock<ApiContext>();
            var userManager = new Mock<UserManager<UserEntity>>();
            var signInManager = new Mock<SignInManager<UserEntity>>();
            var passwordHasher = new Mock<IPasswordHasher<UserEntity>>();
            var transactionService = new Mock<ITransactionService>();

            AccountController controller = new AccountController(mockDatabase.Object, userManager.Object, signInManager.Object, passwordHasher.Object, transactionService.Object);

            double amount = 1231235.45;
            var result = await controller.Withdraw(amount);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void TestBalance()
        {
            var mockDatabase = new Mock<ApiContext>();
            var userManager = new Mock<UserManager<UserEntity>>();
            var signInManager = new Mock<SignInManager<UserEntity>>();
            var passwordHasher = new Mock<IPasswordHasher<UserEntity>>();
            var transactionService = new Mock<ITransactionService>();

            AccountController controller = new AccountController(mockDatabase.Object, userManager.Object, signInManager.Object, passwordHasher.Object, transactionService.Object);

            var result = controller.Balance();

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async void TestLogout()
        {
            var mockDatabase = new Mock<ApiContext>();
            var userManager = new Mock<UserManager<UserEntity>>();
            var signInManager = new Mock<SignInManager<UserEntity>>();
            var passwordHasher = new Mock<IPasswordHasher<UserEntity>>();
            var transactionService = new Mock<ITransactionService>();

            AccountController controller = new AccountController(mockDatabase.Object, userManager.Object, signInManager.Object, passwordHasher.Object, transactionService.Object);

            var result = await controller.Logout();

            Assert.IsType<OkResult>(result);
        }
    }
}
