using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AltSourceWebBanking;
using AltSourceWebBanking.Controllers;

namespace AltSourceWebBanking.Tests.Controllers
{
    [TestClass]
    public class BankControllerTest
    {
        [TestMethod]
        public async void Index()
        {
            // Arrange
            BankController controller = new BankController();

            // Act
            ViewResult result = await controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void About()
        {
            // Arrange
            BankController controller = new BankController();

            // Act
            ViewResult result = controller.About() as ViewResult;

            // Assert
            Assert.AreEqual("Your application description page.", result.ViewBag.Message);
        }

        [TestMethod]
        public void Register()
        {
            BankController controller = new BankController();

            ViewResult result = controller.Register() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Login()
        {
            BankController controller = new BankController();

            ViewResult result = controller.Login() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Logout()
        {
            BankController controller = new BankController();

            RedirectResult result = controller.Logout() as RedirectResult;

            Assert.Equals(result.Url, "Index");
        }

        [TestMethod]
        public void Transactions()
        {
            BankController controller = new BankController();

            ViewResult result = controller.Transactions() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Deposit()
        {
            BankController controller = new BankController();

            ViewResult result = controller.Deposit() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Withdraw()
        {
            BankController controller = new BankController();

            ViewResult result = controller.Withdraw() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Balance()
        {
            BankController controller = new BankController();

            ViewResult result = controller.Balance() as ViewResult;

            Assert.IsNotNull(result);
        }
    }
}
