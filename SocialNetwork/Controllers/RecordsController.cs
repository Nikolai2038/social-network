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
            if (id == null) // если специальное имя пользователя не указано
            {
                return RedirectToAction("Index", "Users"); // перенаправляем пользователя
            }

            users viewing_user = users.getUserFromUserSpecialName(id); // пользователь, страница которого открыта

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

                objects object_record = new objects();
                object_record.object_type_id = Convert.ToInt32(ObjectsTypes.RECORD);
                object_record.user_id_from = user.id;
                object_record.creation_datetime_int = Convert.ToInt32((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds);
                MyFunctions.database.objects.Add(object_record);
                MyFunctions.database.SaveChanges(); // сохраняем изменения, чтобы установился id для объекта

                records record = new records();
                record.object_id = object_record.id;
                record.user_id_to = viewing_user.id;
                record.text = ViewBag.text;
                record.attached_record_id = -1;
                MyFunctions.database.records.Add(record);
                MyFunctions.database.SaveChanges();

                return RedirectToAction("Viewing", "Users", new { id = id }); // перенаправляем пользователя
            }

            return View();
        }

        public ActionResult Edit(string id, int record_id)
        {
            if (id == null) // если специальное имя пользователя не указано
            {
                return RedirectToAction("Index", "Users"); // перенаправляем пользователя
            }

            users viewing_user = users.getUserFromUserSpecialName(id); // пользователь, страница которого открыта

            if (viewing_user == null) // если указанного пользователя не существует
            {
                return RedirectToAction("Index", "Users"); // перенаправляем пользователя
            }

            ViewBag.ViewingUser = viewing_user;

            users user = users.getUserFromUserId(Convert.ToInt32(Session["id"])); // пользователь, просматривающий страницу (может совпадать с пользователем, страница которого открыта)

            ViewBag.User = user;
            
            records record = MyFunctions.database.records.Where(p => (p.id == record_id)).FirstOrDefault();
            Dictionary<SocialNetwork.Models.PermissionsToObject, bool> userPermissionsToRecord = SocialNetwork.Models.users.getUserPermissionsToObject(user, record);

            if (userPermissionsToRecord[SocialNetwork.Models.PermissionsToObject.CAN_EDIT] == false)
            {
                return RedirectToAction("Viewing", "Users", new { id = id }); // перенаправляем пользователя
            }
            
            if (Request.Form["ok"] != null) // если была нажата кнопка добавления записи
            {
                ViewBag.text = Request.Form["text"];
                
                record.text = ViewBag.text;
                MyFunctions.database.SaveChanges();

                return RedirectToAction("Viewing", "Users", new { id = id }); // перенаправляем пользователя на страницу настроек
            }
            else
            {
                ViewBag.text = record.text;
            }

            return View();
        }

        public ActionResult Viewing(string id, int record_id, string action)
        {
            if (id == null) // если специальное имя пользователя не указано
            {
                return RedirectToAction("Index", "Users"); // перенаправляем пользователя
            }

            users viewing_user = users.getUserFromUserSpecialName(id); // пользователь, страница которого открыта

            if (viewing_user == null) // если указанного пользователя не существует
            {
                return RedirectToAction("Index", "Users"); // перенаправляем пользователя
            }

            ViewBag.ViewingUser = viewing_user;

            users user = users.getUserFromUserId(Convert.ToInt32(Session["id"])); // пользователь, просматривающий страницу (может совпадать с пользователем, страница которого открыта)

            ViewBag.User = user;
                        
            records record = MyFunctions.database.records.Where(p => (p.id == record_id)).FirstOrDefault();
            Dictionary<SocialNetwork.Models.PermissionsToObject, bool> userPermissionsToRecord = SocialNetwork.Models.users.getUserPermissionsToObject(user, record);

            if (userPermissionsToRecord[SocialNetwork.Models.PermissionsToObject.CAN_SEE] == false)
            {
                return RedirectToAction("Viewing", "Users", new { id = id }); // перенаправляем пользователя
            }

            ViewBag.User = user;
            ViewBag.ViewingUser = viewing_user;
            ViewBag.Record = record;

            return View();
        }

        public ActionResult Delete(string id, int record_id)
        {
            if (id == null) // если специальное имя пользователя не указано
            {
                return RedirectToAction("Index", "Users"); // перенаправляем пользователя
            }

            users viewing_user = users.getUserFromUserSpecialName(id); // пользователь, страница которого открыта

            if (viewing_user == null) // если указанного пользователя не существует
            {
                return RedirectToAction("Index", "Users"); // перенаправляем пользователя
            }

            ViewBag.ViewingUser = viewing_user;

            users user = users.getUserFromUserId(Convert.ToInt32(Session["id"])); // пользователь, просматривающий страницу (может совпадать с пользователем, страница которого открыта)

            ViewBag.User = user;

            records record = MyFunctions.database.records.Where(p => (p.id == record_id)).FirstOrDefault();
            Dictionary<SocialNetwork.Models.PermissionsToObject, bool> userPermissionsToRecord = SocialNetwork.Models.users.getUserPermissionsToObject(user, record);

            if (userPermissionsToRecord[SocialNetwork.Models.PermissionsToObject.CAN_DELETE] == false)
            {
                return RedirectToAction("Viewing", "Users", new { id = id }); // перенаправляем пользователя
            }

            ViewBag.text = Request.Form["text"];
            
            MyFunctions.database.records.Remove(record);
            MyFunctions.database.SaveChanges();

            return RedirectToAction("Viewing", "Users", new { id = id }); // перенаправляем пользователя
        }
    }
}