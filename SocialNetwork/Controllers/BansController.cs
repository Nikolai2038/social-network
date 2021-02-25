using SocialNetwork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SocialNetwork.Controllers
{
    public class BansController : Controller
    {
        public ActionResult Index(string id, string sort_key, string sort_asc, int total_page_id = 1, int elements_on_page = 10)
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

            List<bans> list = MyFunctions.database.bans.Where(p => (p.user_id_to == viewing_user.id)).ToList();

            if (sort_asc == "asc")
            {
                if (sort_key == "id")
                {
                    list = list.OrderBy(p => p.id).ToList();
                }
                else if (sort_key == "user_id_from")
                {
                    list = list.OrderBy(p => p.user_id_from).ToList();
                }
                else if (sort_key == "ban_datetime_int")
                {
                    list = list.OrderBy(p => p.ban_datetime_int).ToList();
                }
                else if (sort_key == "is_permanent")
                {
                    list = list.OrderBy(p => p.is_permanent).ToList();
                }
                else if (sort_key == "unban_datetime_int")
                {
                    list = list.OrderBy(p => p.unban_datetime_int).ToList();
                }
            }
            else if (sort_asc == "desc")
            {
                if (sort_key == "id")
                {
                    list = list.OrderByDescending(p => p.id).ToList();
                }
                else if (sort_key == "user_id_from")
                {
                    list = list.OrderByDescending(p => p.user_id_from).ToList();
                }
                else if (sort_key == "ban_datetime_int")
                {
                    list = list.OrderByDescending(p => p.ban_datetime_int).ToList();
                }
                else if (sort_key == "is_permanent")
                {
                    list = list.OrderByDescending(p => p.is_permanent).ToList();
                }
                else if (sort_key == "unban_datetime_int")
                {
                    list = list.OrderByDescending(p => p.unban_datetime_int).ToList();
                }
            }

            List<object> list_object = list.ToList<object>();

            ViewBag.ListOnPage = MyFunctions.pageNavigation_getListOnPage(list_object, ref elements_on_page, ref total_page_id);

            ViewBag.ElementsOnPage = elements_on_page;
            ViewBag.TotalPageId = total_page_id;
            ViewBag.SortKey = sort_key;
            ViewBag.SortAsc = sort_asc;

            return View();
        }

        public ActionResult Ban(string id)
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

            if (!userPermissionsToUser[PermissionsToUser.CAN_BAN_AND_UNBAN])
            {
                return RedirectToAction("Viewing", "Users", new { id = id }); // перенаправляем пользователя
            }

            if (Request.Form["ok"] != null) // если была нажата кнопка бана
            {
                ViewBag.is_permanent = Request.Form["is_permanent"];
                ViewBag.ban_hours = Request.Form["ban_hours"];

                bans ban = new bans();
                ban.user_id_from = user.id;
                ban.user_id_to = viewing_user.id;
                ban.ban_datetime_int = (int)((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds);
                ban.unban_datetime_int = ban.ban_datetime_int + Convert.ToInt32(ViewBag.ban_hours) * 60 * 60;
                MyFunctions.database.bans.Add(ban);
                MyFunctions.database.SaveChanges();

                return RedirectToAction("Index", "Bans", new { id = id }); // перенаправляем пользователя на страницу настроек
            }

            return View();
        }

        public ActionResult Unban(string id, int ban_id)
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

            if (!userPermissionsToUser[PermissionsToUser.CAN_BAN_AND_UNBAN])
            {
                return RedirectToAction("Viewing", "Users", new { id = id }); // перенаправляем пользователя
            }

            bans ban = MyFunctions.database.bans.Where(p => ((p.user_id_to == user_id) && (p.id == ban_id))).FirstOrDefault();
            ban.unban_datetime_int = (int)((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds);
            MyFunctions.database.SaveChanges();

            return RedirectToAction("Index", "Bans", new { id = id }); // перенаправляем пользователя
        }

        public ActionResult Delete(string id, int ban_id)
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

            if (!userPermissionsToUser[PermissionsToUser.CAN_BAN_AND_UNBAN])
            {
                return RedirectToAction("Viewing", "Users", new { id = id }); // перенаправляем пользователя
            }

            bans ban = MyFunctions.database.bans.Where(p => ((p.user_id_to == user_id) && (p.id == ban_id))).FirstOrDefault();
            MyFunctions.database.bans.Remove(ban);
            MyFunctions.database.SaveChanges();

            return RedirectToAction("Index", "Bans", new { id = id }); // перенаправляем пользователя
        }
    }
}