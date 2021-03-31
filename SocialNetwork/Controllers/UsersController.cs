using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Security.Cryptography;
using System.Text;
using SocialNetwork.Models;
using ClosedXML.Excel;
using System.IO;

namespace SocialNetwork.Controllers
{
    public class UsersController : Controller
    {
        private List<object> getUsersSearch(List<users> list, string sort_key, string sort_asc, string search_key, string search_text = "", int total_page_id = 1, int elements_on_page = 30)
        {
            if (search_key == "id")
            {
                list = list.Where(p => (p.id.ToString().Contains(search_text))).ToList();
            }
            else if (search_key == "name")
            {
                list = list.Where(p => (p.name.ToString().Contains(search_text))).ToList();
            }
            else if (search_key == "special_name")
            {
                list = list.Where(p => (p.special_name.ToString().Contains(search_text))).ToList();
            }

            if (sort_asc == "asc")
            {
                if (sort_key == "id")
                {
                    list = list.OrderBy(p => p.id).ToList();
                }
                else if (sort_key == "name")
                {
                    list = list.OrderBy(p => p.name).ToList();
                }
                else if (sort_key == "special_name")
                {
                    list = list.OrderBy(p => p.special_name).ToList();
                }
                else if (sort_key == "registration_datetime_int")
                {
                    list = list.OrderBy(p => p.registration_datetime_int).ToList();
                }
                else if (sort_key == "last_activity_datetime_int")
                {
                    list = list.OrderBy(p => p.last_activity_datetime_int).ToList();
                }
            }
            else if (sort_asc == "desc")
            {
                if (sort_key == "id")
                {
                    list = list.OrderByDescending(p => p.id).ToList();
                }
                else if (sort_key == "name")
                {
                    list = list.OrderByDescending(p => p.name).ToList();
                }
                else if (sort_key == "special_name")
                {
                    list = list.OrderByDescending(p => p.special_name).ToList();
                }
                else if (sort_key == "registration_datetime_int")
                {
                    list = list.OrderByDescending(p => p.registration_datetime_int).ToList();
                }
                else if (sort_key == "last_activity_datetime_int")
                {
                    list = list.OrderByDescending(p => p.last_activity_datetime_int).ToList();
                }
            }

            List<object> list_object = list.ToList<object>();

            return MyFunctions.pageNavigation_getListOnPage(list_object, ref elements_on_page, ref total_page_id);
        }

        private ActionResult UsersSearch(string view_name, List<users> list, string sort_key, string sort_asc, string search_key, string search_text = "", int total_page_id = 1, int elements_on_page = 30)
        {
            ViewBag.ListOnPage = getUsersSearch(list, sort_key, sort_asc, search_key, search_text, total_page_id, elements_on_page);

            ViewBag.ElementsOnPage = elements_on_page;
            ViewBag.TotalPageId = total_page_id;
            ViewBag.SortKey = sort_key;
            ViewBag.SortAsc = sort_asc;

            ViewBag.SearchText = search_text;
            ViewBag.SearchKey = search_key;

            ViewBag.User = users.getUserFromUserId(Convert.ToInt32(Session["id"]));

            if (view_name == null)
            {
                return View("UsersSearch");
            }
            else
            {
                return View(view_name);
            }
        }

        public ActionResult Index(string sort_key = "", string sort_asc = "", string search_key = "", string search_text = "", int total_page_id = 1, int elements_on_page = 30, bool is_download = false)
        {
            List<users> list = MyFunctions.database.users.Where(p => (p.id >= 1)).ToList();

            if (is_download == true) // выгрузка в файл
            {
                List<object> searched_list = getUsersSearch(list, sort_key, sort_asc, search_key, search_text, total_page_id, elements_on_page);
                List<users> searched_list_users = new List<users>();
                foreach (object user_obj in searched_list)
                {
                    searched_list_users.Add(user_obj as users);
                }

                using (XLWorkbook workbook = new XLWorkbook(XLEventTracking.Disabled))
                {
                    // Примечание: нумерация строк и столбцов начинается с индекса 1 (не 0)

                    var worksheet = workbook.Worksheets.Add("Пользователи");

                    // первая строчка - заголовки
                    worksheet.Cell("A1").Value = "ID";
                    worksheet.Cell("B1").Value = "Имя пользователя";
                    worksheet.Cell("C1").Value = "Специальное имя пользователя";
                    worksheet.Cell("D1").Value = "Дата регистрации";
                    worksheet.Cell("E1").Value = "Дата последнего онлайна";
                    worksheet.Cell("F1").Value = "Общий рейтинг";

                    worksheet.Column(1).Width = 7;
                    worksheet.Column(2).Width = 17;
                    worksheet.Column(3).Width = 30;
                    worksheet.Column(4).Width = 30;
                    worksheet.Column(5).Width = 30;
                    worksheet.Column(6).Width = 15;

                    worksheet.Row(1).Style.Font.Bold = true;

                    for (int i = 0; i < searched_list_users.Count; i++)
                    {
                        worksheet.Cell(i + 2, 1).Value = searched_list_users[i].id;
                        worksheet.Cell(i + 2, 2).Value = searched_list_users[i].name;
                        worksheet.Cell(i + 2, 3).Value = searched_list_users[i].special_name;
                        worksheet.Cell(i + 2, 4).Value = users.getDatetimeStringFromDatetimeInt(Convert.ToInt32(searched_list_users[i].registration_datetime_int));
                        worksheet.Cell(i + 2, 5).Value = users.getDatetimeStringFromDatetimeInt(Convert.ToInt32(searched_list_users[i].last_activity_datetime_int));
                        worksheet.Cell(i + 2, 6).Value = searched_list_users[i].getRating();
                    }

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        stream.Flush();

                        int total_datetime_int = (int)((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds);
                        return new FileContentResult(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                        {
                            FileDownloadName = "Данные пользователей от " + users.getDatetimeStringFromDatetimeInt(total_datetime_int) + ".xlsx"
                        };
                    }
                }
            }

            return UsersSearch("Index", list, sort_key, sort_asc, search_key, search_text, total_page_id, elements_on_page);
        }

        public ActionResult Viewing(string id, string tr_action = null, string sort_key = "", string sort_asc = "", string search_key = "", string search_text = "", int total_page_id = 1, int elements_on_page = 30)
        {
            if (id == null) // если специальное имя пользователя не указано - будем использовать id пользователя текущей сессии
            {
                if (users.isUserLoggedIn() != true) // если пользователь не находится в аккаунте
                {
                    return RedirectToAction("Authorization", "Main"); // перенаправляем пользователя на страницу авторизации
                }
                else
                {
                    int user_id = Convert.ToInt32(Session["id"]);
                    ViewBag.ViewingUser = users.getUserFromUserId(user_id); // пользователь, страница которого открыта
                }
            }
            else
            {
                ViewBag.ViewingUser = users.getUserFromUserSpecialName(id); // пользователь, страница которого открыта
            }
            
            if (ViewBag.ViewingUser == null) // если не понятно, профиль какого пользователя открывается
            {
                return RedirectToAction("Index", "Users"); // перенаправляем пользователя
            }

            ViewBag.User = null; // пользователь, просматривающий страницу (может совпадать с пользователем, страница которого открыта)
            if (users.isUserLoggedIn())
            {
                ViewBag.User = users.getUserFromUserId(Convert.ToInt32(Session["id"]));
            }

            users user_from = ViewBag.User;
            users user_to = ViewBag.ViewingUser;

            Dictionary<PermissionsToUser, bool> userPermissionsToUser = users.getUserPermissionsToUser(user_from, user_to);

            if (tr_action != null)
            {
                if ((tr_action == "accept_subscriber") && (userPermissionsToUser[PermissionsToUser.CAN_ACCEPT_SUBSCRIBER]))
                {
                    friends_and_subscriptions friendsAndSubscriptions = MyFunctions.database.friends_and_subscriptions.Where(p => ((p.user_id_from == user_to.id) && (p.user_id_to == user_from.id))).FirstOrDefault();
                    friendsAndSubscriptions.is_accepted = 1;
                    MyFunctions.database.SaveChanges();

                    friends_and_subscriptions friendsAndSubscriptions2 = new friends_and_subscriptions();
                    friendsAndSubscriptions2.user_id_from = user_from.id;
                    friendsAndSubscriptions2.user_id_to = user_to.id;
                    friendsAndSubscriptions2.is_accepted = 1;
                    MyFunctions.database.friends_and_subscriptions.Add(friendsAndSubscriptions2);
                    MyFunctions.database.SaveChanges();
                }
                else if ((tr_action == "unaccept_friend") && (userPermissionsToUser[PermissionsToUser.CAN_UNACCEPT_FRIEND]))
                {
                    friends_and_subscriptions friendsAndSubscriptions = MyFunctions.database.friends_and_subscriptions.Where(p => ((p.user_id_from == user_to.id) && (p.user_id_to == user_from.id))).FirstOrDefault();
                    friendsAndSubscriptions.is_accepted = 0;
                    MyFunctions.database.SaveChanges();

                    friends_and_subscriptions friendsAndSubscriptions2 = MyFunctions.database.friends_and_subscriptions.Where(p => ((p.user_id_from == user_from.id) && (p.user_id_to == user_to.id))).FirstOrDefault();
                    MyFunctions.database.friends_and_subscriptions.Remove(friendsAndSubscriptions2);
                    MyFunctions.database.SaveChanges();
                }
                else if ((tr_action == "subscribe") && (userPermissionsToUser[PermissionsToUser.CAN_SUBSCRIBE]))
                {
                    friends_and_subscriptions friendsAndSubscriptions = new friends_and_subscriptions();
                    friendsAndSubscriptions.user_id_from = user_from.id;
                    friendsAndSubscriptions.user_id_to = user_to.id;
                    friendsAndSubscriptions.is_accepted = 0;
                    MyFunctions.database.friends_and_subscriptions.Add(friendsAndSubscriptions);
                    MyFunctions.database.SaveChanges();
                }
                else if ((tr_action == "unsubscribe") && (userPermissionsToUser[PermissionsToUser.CAN_UNSUBSCRIBE]))
                {
                    friends_and_subscriptions friendsAndSubscriptions = MyFunctions.database.friends_and_subscriptions.Where(p => ((p.user_id_from == user_from.id) && (p.user_id_to == user_to.id))).FirstOrDefault();
                    MyFunctions.database.friends_and_subscriptions.Remove(friendsAndSubscriptions);
                    MyFunctions.database.SaveChanges();
                }
                else if ((tr_action == "add_to_black_list") && (userPermissionsToUser[PermissionsToUser.CAN_ADD_TO_BLACK_LIST]))
                {
                    black_list blackList = new black_list();
                    blackList.user_id_from = user_from.id;
                    blackList.user_id_blocked = user_to.id;
                    blackList.block_datetime_int = (int)((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds);
                    MyFunctions.database.black_list.Add(blackList);
                    MyFunctions.database.SaveChanges();
                }
                else if ((tr_action == "remove_from_black_list") && (userPermissionsToUser[PermissionsToUser.CAN_REMOVE_FROM_BLACK_LIST]))
                {
                    black_list blackList = MyFunctions.database.black_list.Where(p => ((p.user_id_from == user_from.id) && (p.user_id_blocked == user_to.id))).FirstOrDefault();
                    MyFunctions.database.black_list.Remove(blackList);
                    MyFunctions.database.SaveChanges();
                }

                return RedirectToAction("Viewing", "Users", new { id = id }); // перенаправляем пользователя
            }
            
            List<records> list = user_to.getRecords();

            if (search_key == "id")
            {
                list = list.Where(p => (p.id.ToString().Contains(search_text))).ToList();
            }
            else if (search_key == "text")
            {
                list = list.Where(p => (p.text.ToString().Contains(search_text))).ToList();
            }

            if (sort_asc == "asc")
            {
                if (sort_key == "id")
                {
                    list = list.OrderBy(p => p.id).ToList();
                }
                else if (sort_key == "text")
                {
                    list = list.OrderBy(p => p.text).ToList();
                }
            }
            else if (sort_asc == "desc")
            {
                if (sort_key == "id")
                {
                    list = list.OrderByDescending(p => p.id).ToList();
                }
                else if (sort_key == "text")
                {
                    list = list.OrderByDescending(p => p.text).ToList();
                }
            }

            List<object> list_object = list.ToList<object>();

            ViewBag.ListOnPage = MyFunctions.pageNavigation_getListOnPage(list_object, ref elements_on_page, ref total_page_id);

            ViewBag.ElementsOnPage = elements_on_page;
            ViewBag.TotalPageId = total_page_id;
            ViewBag.SortKey = sort_key;
            ViewBag.SortAsc = sort_asc;

            ViewBag.SearchText = search_text;
            ViewBag.SearchKey = search_key;

            return View();
        }

        public ActionResult SpecialPermissions(string id, string specials_permissions_action = null)
        {
            if (users.isUserLoggedIn() == false) // если пользователь не авторизован
            {
                return RedirectToAction("Authorization", "Main"); // перенаправляем пользователя на страницу авторизации
            }
            else
            {
                if (id == null) // если специальное имя пользователя не указано
                {
                    return RedirectToAction("Index", "Users"); // перенаправляем пользователя
                }

                users viewing_user = users.getUserFromUserSpecialName(id); // пользователь, страница которого открыта

                users user = users.getUserFromUserId(Convert.ToInt32(Session["id"])); // пользователь, просматривающий страницу (может совпадать с пользователем, страница которого открыта)

                if (viewing_user == null) // если указанного пользователя не существует
                {
                    return RedirectToAction("Index", "Users"); // перенаправляем пользователя
                }

                if (user.permissions_rank <= viewing_user.permissions_rank) // ранг для изменения прав обязательно должен быть выше ранга изменяемого пользователя
                {
                    return RedirectToAction("Viewing", "Users", new { id = id }); // перенаправляем пользователя
                }

                if (specials_permissions_action != null) // если указано действие изменения специальных прав
                {
                    if (specials_permissions_action == "set_user") // снятие всех прав
                    {
                        viewing_user.permissions_rank = 0;
                    }
                    else if (specials_permissions_action == "set_moderator") // назначение модератором
                    {
                        viewing_user.permissions_rank = 1;
                    }
                    else if (specials_permissions_action == "set_administrator") // назначение администратором
                    {
                        viewing_user.permissions_rank = 2;
                    }
                    MyFunctions.database.SaveChanges();

                    return RedirectToAction("SpecialPermissions", "Users", new { id = id }); // перенаправляем пользователя
                }

                ViewBag.ViewingUser = viewing_user;
                ViewBag.User = user;

                return View();
            }
        }

        public ActionResult AuthorizationHistory(string id, string sort_key = "", string sort_asc = "", int total_page_id = 1, int elements_on_page = 30)
        {
            if (users.isUserLoggedIn() == false) // если пользователь не авторизован
            {
                return RedirectToAction("Authorization", "Main"); // перенаправляем пользователя на страницу авторизации
            }
            else
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

                Dictionary<PermissionsToUser, bool> userPermissionsToUser = users.getUserPermissionsToUser(user, viewing_user);

                if (!userPermissionsToUser[PermissionsToUser.CAN_SEE_AUTHORIZATION_HISTORY])
                {
                    return RedirectToAction("Viewing", "Users", new { id = id }); // перенаправляем пользователя
                }
                else
                {
                    List<authorization_history> list = MyFunctions.database.authorization_history.Where(p => (p.user_id == viewing_user.id)).ToList();

                    if (sort_asc == "asc")
                    {
                        if (sort_key == "id")
                        {
                            list = list.OrderBy(p => p.id).ToList();
                        }
                        else if (sort_key == "authorization_datetime_int")
                        {
                            list = list.OrderBy(p => p.authorization_datetime_int).ToList();
                        }
                        else if (sort_key == "ip_address")
                        {
                            list = list.OrderBy(p => p.ip_address).ToList();
                        }
                    }
                    else if (sort_asc == "desc")
                    {
                        if (sort_key == "id")
                        {
                            list = list.OrderByDescending(p => p.id).ToList();
                        }
                        else if (sort_key == "authorization_datetime_int")
                        {
                            list = list.OrderByDescending(p => p.authorization_datetime_int).ToList();
                        }
                        else if (sort_key == "ip_address")
                        {
                            list = list.OrderByDescending(p => p.ip_address).ToList();
                        }
                    }

                    List<object> list_object = list.ToList<object>();

                    ViewBag.ListOnPage = MyFunctions.pageNavigation_getListOnPage(list_object, ref elements_on_page, ref total_page_id);

                    ViewBag.ElementsOnPage = elements_on_page;
                    ViewBag.TotalPageId = total_page_id;
                    ViewBag.SortKey = sort_key;
                    ViewBag.SortAsc = sort_asc;
                }
            }
            return View();
        }

        public ActionResult Friends(string id, string sort_key = "", string sort_asc = "", string search_key = "", string search_text = "", int total_page_id = 1, int elements_on_page = 30)
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

            Dictionary<PermissionsToUser, bool> userPermissionsToUser = users.getUserPermissionsToUser(user, viewing_user);

            if (!userPermissionsToUser[PermissionsToUser.CAN_SEE_MY_FRIENDS])
            {
                return RedirectToAction("Viewing", "Users", new { id = id }); // перенаправляем пользователя
            }
            else
            {
                List<users> list = viewing_user.getFriends();
                return UsersSearch("Friends", list, sort_key, sort_asc, search_key, search_text, total_page_id, elements_on_page);
            }
        }

        public ActionResult Subscribers(string id, string sort_key = "", string sort_asc = "", string search_key = "", string search_text = "", int total_page_id = 1, int elements_on_page = 30)
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

            Dictionary<PermissionsToUser, bool> userPermissionsToUser = users.getUserPermissionsToUser(user, viewing_user);

            if (!userPermissionsToUser[PermissionsToUser.CAN_SEE_MY_SUBSCRIBERS])
            {
                return RedirectToAction("Viewing", "Users", new { id = id }); // перенаправляем пользователя
            }
            else
            {
                List<users> list = viewing_user.getSubscribers();
                return UsersSearch("Subscribers", list, sort_key, sort_asc, search_key, search_text, total_page_id, elements_on_page);
            }
        }

        public ActionResult Subscriptions(string id, string sort_key = "", string sort_asc = "", string search_key = "", string search_text = "", int total_page_id = 1, int elements_on_page = 30)
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

            Dictionary<PermissionsToUser, bool> userPermissionsToUser = users.getUserPermissionsToUser(user, viewing_user);

            if (!userPermissionsToUser[PermissionsToUser.CAN_SEE_MY_SUBSCRIPTIONS])
            {
                return RedirectToAction("Viewing", "Users", new { id = id }); // перенаправляем пользователя
            }
            else
            {
                List<users> list = viewing_user.getSubscriptions();
                return UsersSearch("Subscriptions", list, sort_key, sort_asc, search_key, search_text, total_page_id, elements_on_page);
            }
        }
    }
}