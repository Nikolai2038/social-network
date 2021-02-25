using SocialNetwork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SocialNetwork.Controllers
{
    public class RecordsController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create(string id)
        {
            int user_id = -1;
            if (id == null) // если id пользователя не указан
            {
                return RedirectToAction("Index", "Users"); // перенаправляем пользователя
            }
            else
            {
                user_id = Convert.ToInt32(id);
            }

            users viewing_user = users.getUserFromUserId(user_id); // пользователь, страница которого открыта

            if (viewing_user == null) // если указанного пользователя не существует
            {
                return RedirectToAction("Index", "Users"); // перенаправляем пользователя
            }

            ViewBag.ViewingUser = viewing_user;

            users user = users.getUserFromUserId(Convert.ToInt32(Session["id"])); // пользователь, просматривающий страницу (может совпадать с пользователем, страница которого открыта)

            ViewBag.User = user;

            Dictionary<PermissionsToUser, bool> userPermissionsToUser = users.getUserPermissionsToUser(user, viewing_user);

            if (!userPermissionsToUser[PermissionsToUser.CAN_CREATE_RECORDS_ON_MY_PAGE])
            {
                return RedirectToAction("Viewing", "Users", new { id = id }); // перенаправляем пользователя
            }

            if (Request.Form["ok"] != null) // если была нажата кнопка добавления записи
            {
                ViewBag.text = Request.Form["text"];

                records_simple record = new records_simple();
                record.text = users.getDatetimeStringFromDatetimeInt((int)((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds)) + "<br/>" + user.name + "<br/>" + ViewBag.text;
                record.user_id_to = viewing_user.id;
                MyFunctions.database.records_simple.Add(record);
                MyFunctions.database.SaveChanges();

                return RedirectToAction("Viewing", "Users", new { id = id }); // перенаправляем пользователя на страницу настроек
            }

            return View();
        }

        public ActionResult Edit(string id, int record_id)
        {
            int user_id = -1;
            if (id == null) // если id пользователя не указан
            {
                return RedirectToAction("Index", "Users"); // перенаправляем пользователя
            }
            else
            {
                user_id = Convert.ToInt32(id);
            }

            users viewing_user = users.getUserFromUserId(user_id); // пользователь, страница которого открыта

            if (viewing_user == null) // если указанного пользователя не существует
            {
                return RedirectToAction("Index", "Users"); // перенаправляем пользователя
            }

            ViewBag.ViewingUser = viewing_user;

            users user = users.getUserFromUserId(Convert.ToInt32(Session["id"])); // пользователь, просматривающий страницу (может совпадать с пользователем, страница которого открыта)

            ViewBag.User = user;

            Dictionary<PermissionsToUser, bool> userPermissionsToUser = users.getUserPermissionsToUser(user, viewing_user);

            if (!userPermissionsToUser[PermissionsToUser.CAN_CREATE_RECORDS_ON_MY_PAGE])
            {
                return RedirectToAction("Viewing", "Users", new { id = id }); // перенаправляем пользователя
            }

            if (Request.Form["ok"] != null) // если была нажата кнопка добавления записи
            {
                ViewBag.text = Request.Form["text"];

                records_simple record = MyFunctions.database.records_simple.Where(p => (p.id == record_id)).FirstOrDefault();
                record.text = ViewBag.text;
                MyFunctions.database.SaveChanges();

                return RedirectToAction("Viewing", "Users", new { id = id }); // перенаправляем пользователя на страницу настроек
            }
            else
            {
                records_simple record = MyFunctions.database.records_simple.Where(p => (p.id == record_id)).FirstOrDefault();
                ViewBag.text = record.text;
            }

            return View();
        }

        public ActionResult Viewing()
        {
            return View();
        }

        public ActionResult Delete(string id, int record_id)
        {
            int user_id = -1;
            if (id == null) // если id пользователя не указан
            {
                return RedirectToAction("Index", "Users"); // перенаправляем пользователя
            }
            else
            {
                user_id = Convert.ToInt32(id);
            }

            users viewing_user = users.getUserFromUserId(user_id); // пользователь, страница которого открыта

            if (viewing_user == null) // если указанного пользователя не существует
            {
                return RedirectToAction("Index", "Users"); // перенаправляем пользователя
            }

            ViewBag.ViewingUser = viewing_user;

            users user = users.getUserFromUserId(Convert.ToInt32(Session["id"])); // пользователь, просматривающий страницу (может совпадать с пользователем, страница которого открыта)

            ViewBag.User = user;

            Dictionary<PermissionsToUser, bool> userPermissionsToUser = users.getUserPermissionsToUser(user, viewing_user);

            if (!userPermissionsToUser[PermissionsToUser.CAN_CREATE_RECORDS_ON_MY_PAGE])
            {
                return RedirectToAction("Viewing", "Users", new { id = id }); // перенаправляем пользователя
            }

            ViewBag.text = Request.Form["text"];

            records_simple record = MyFunctions.database.records_simple.Where(p => (p.id == record_id)).FirstOrDefault();
            MyFunctions.database.records_simple.Remove(record);
            MyFunctions.database.SaveChanges();

            return RedirectToAction("Viewing", "Users", new { id = id }); // перенаправляем пользователя на страницу настроек

            return View();
        }
    }
}