using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocialNetwork.Controllers;
using SocialNetwork.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace UnitTest
{
    [TestClass]
    public class UsersControllerTest
    {
        private UsersController controller;
        private ViewResult view;

        [TestInitialize]
        public void SetupContext()
        {
            controller = new UsersController();
            view = controller.Index() as ViewResult;
        }

        [TestMethod]
        public void IndexViewResultNotNull()
        {
            Assert.IsNotNull(view);
        }

        [TestMethod]
        public void IndexViewEqualIndexCshtml()
        {
            Assert.AreEqual("Index", view.ViewName);
        }

        [TestMethod]
        public void IndexListOnPageInViewBag()
        {
            List<object> list = view.ViewBag.ListOnPage as List<object>;
            Assert.IsNotNull(list);
            foreach (object obj in list)
            {
                Assert.IsNotNull(obj);
                users user = null;
                try
                {
                    user = obj as users;
                }
                catch
                {
                    throw new Exception("Ошибка преобразования объекта списка пользователей!");
                }
                Assert.IsNotNull(user);
            }
        }

    }
}