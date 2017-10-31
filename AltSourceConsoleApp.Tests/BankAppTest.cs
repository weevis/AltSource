using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AltSourceConsoleApp;
using AltSourceConsoleApp.Controllers;

namespace AltSourceConsoleApp.Tests
{
    [TestClass]
    public class BankAppTest
    {
        public static BankApp bankApp;
        [TestMethod]
        public void TestCreate()
        {
            string FirstName = "Test";
            string LastName = "Test";
            string Email = "test@test.com";
            string UserName = "test";
            string Password = "T3sting!.";

            Assert.IsNotNull(Users.Create(FirstName, LastName, Email, UserName, Password));
        }

        [TestMethod]
        public void TestLogin()
        {
            string UserName = "test";
            string Email = "test@test.com";
            string Password = "T3sting!.";

            Assert.IsNotNull(Users.Login(UserName, Email, Password));
        }

        [TestMethod]
        public void TestGetBalance()
        {
            Assert.IsNotNull(Users.GetBalance());
        }

        [TestMethod]
        public void TestDeposit()
        {
            double amount = 123.45;
            Assert.IsNotNull(Users.Deposit(amount));
        }

        [TestMethod]
        public void TestWithdraw()
        {
            double amount = 123.45;
            Assert.IsNotNull(Users.Withdraw(amount));
        }

        [TestMethod]
        public void TestTransactions()
        {
            Assert.IsNotNull(Users.Transactions());
        }

        [TestMethod]
        public void TestLogout()
        {
            Assert.IsNotNull(Users.Logout());
        }
    }
}
