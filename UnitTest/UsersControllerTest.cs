using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocialNetwork;
using SocialNetwork.Controllers;
using SocialNetwork.Models;
using System.Web.Mvc;

namespace UnitTest
{
    [TestClass]
    public class UsersControllerTest
    {
        private UsersController controller;
        private ViewResult result;

        [TestInitialize]
        public void SetupContext()
        {
            controller = new UsersController();
            //MyFunctions.database = new Entities();
            ActionResult r = controller.Index();
            //result = controller.Index() as ViewResult;
        }

        [TestMethod]
        public void IndexViewResultNotNull()
        {
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void IndexViewEqualIndexCshtml()
        {
            Assert.AreEqual("Index", result.ViewName);
        }

        [TestMethod]
        public void IndexStringInViewbag()
        {
            Assert.AreEqual("Hello world!", result.ViewBag.Message);
        }

    }
}