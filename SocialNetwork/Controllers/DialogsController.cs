using SocialNetwork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SocialNetwork.Controllers
{
    public class DialogsController : Controller
    {
        public ActionResult Index()
        {
            if (users.isUserLoggedIn() != true) // если пользователь не авторизован
            {
                return RedirectToAction("Authorization", "Main"); // перенаправляем пользователя на страницу авторизации
            }
            else
            {
                return View();
            }
        }

        public ActionResult Create()
        {
            if (users.isUserLoggedIn() != true) // если пользователь не авторизован
            {
                return RedirectToAction("Authorization", "Main"); // перенаправляем пользователя на страницу авторизации
            }
            else
            {
                return View();
            }
        }

        public ActionResult CreateGroup()
        {
            if (users.isUserLoggedIn() != true) // если пользователь не авторизован
            {
                return RedirectToAction("Authorization", "Main"); // перенаправляем пользователя на страницу авторизации
            }
            else
            {
                return View();
            }
        }

        public ActionResult EditGroup()
        {
            if (users.isUserLoggedIn() != true) // если пользователь не авторизован
            {
                return RedirectToAction("Authorization", "Main"); // перенаправляем пользователя на страницу авторизации
            }
            else
            {
                return View();
            }
        }

        public ActionResult Viewing()
        {
            if (users.isUserLoggedIn() != true) // если пользователь не авторизован
            {
                return RedirectToAction("Authorization", "Main"); // перенаправляем пользователя на страницу авторизации
            }
            else
            {
                return View();
            }
        }

        public ActionResult Delete()
        {
            if (users.isUserLoggedIn() != true) // если пользователь не авторизован
            {
                return RedirectToAction("Authorization", "Main"); // перенаправляем пользователя на страницу авторизации
            }
            else
            {
                return View();
            }
        }

        public ActionResult Search()
        {
            if (users.isUserLoggedIn() != true) // если пользователь не авторизован
            {
                return RedirectToAction("Authorization", "Main"); // перенаправляем пользователя на страницу авторизации
            }
            else
            {
                return View();
            }
        }
    }
}