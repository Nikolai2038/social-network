using SocialNetwork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SocialNetwork.Controllers
{
    public class SettingsController : Controller
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

        public ActionResult Account()
        {
            if (users.isUserLoggedIn() != true) // если пользователь не авторизован
            {
                return RedirectToAction("Authorization", "Main"); // перенаправляем пользователя на страницу авторизации
            }
            else
            {
                ViewBag.name = Request.Form["name"];
                ViewBag.special_name = Request.Form["special_name"];
                ViewBag.new_password = Request.Form["new_password"];
                ViewBag.new_password_2 = Request.Form["new_password_2"];
                ViewBag.new_secret_question = Request.Form["new_secret_question"];
                ViewBag.new_secret_answer = Request.Form["new_secret_answer"];

                int user_id = Convert.ToInt32(Session["id"]);
                users user = users.getUserFromUserId(user_id);

                List<string> errors = new List<string>();

                if (Request.Form["ok"] != null) // если была нажата кнопка сохранения настроек
                {
                    string name = Request.Form["name"];
                    string special_name = Request.Form["special_name"];
                    MyFunctions.checkName(name, ref errors);
                    MyFunctions.checkSpecialName(special_name, ref errors);

                    string new_password = Request.Form["new_password"];
                    string new_password_2 = Request.Form["new_password_2"];
                    if (new_password.Length != 0)
                    {
                        MyFunctions.checkPassword(new_password, ref errors);
                        MyFunctions.checkPassword2(new_password, new_password_2, ref errors);
                    }
                    else
                    {
                        if (new_password_2.Length != 0)
                        {
                            errors.Add("Для подтверждения пароля введите сам пароль!");
                        }
                    }

                    string new_secret_question = Request.Form["new_secret_question"];
                    string new_secret_answer = Request.Form["new_secret_answer"];
                    MyFunctions.checkSecretQuestion(new_secret_question, new_secret_answer, ref errors);

                    string password = Request.Form["password"];
                    bool is_password_ok = MyFunctions.checkTotalPassword(password, ref errors);

                    if (is_password_ok == true)
                    {
                        string password_sha512 = users.generateSha512(password);
                        if (password_sha512 != user.password_sha512)
                        {
                            errors.Add("Неверный пароль!");
                        }
                    }

                    if (errors.Count == 0)
                    {
                        user.name = name;
                        user.special_name = special_name;
                        if (new_password.Length != 0)
                        {
                            user.password_sha512 = users.generateSha512(new_password);
                        }
                        if (new_secret_question.Length != 0)
                        {
                            user.secret_question = new_secret_question;
                            user.secret_answer_sha512 = users.generateSha512(new_secret_answer);
                        }

                        MyFunctions.database.SaveChanges(); // сохраняем БД

                        return RedirectToAction("Index", "Settings"); // перенаправляем пользователя в его профиль
                    }
                }
                else
                {
                    ViewBag.name = user.name;
                    ViewBag.special_name = user.special_name;
                    ViewBag.new_password = "";
                    ViewBag.new_password_2 = "";
                    ViewBag.new_secret_question = "";
                    ViewBag.new_secret_answer = "";
                }

                ViewBag.Errors = errors;
                return View();
            }
        }

        public ActionResult Private()
        {
            if (users.isUserLoggedIn() != true) // если пользователь не авторизован
            {
                return RedirectToAction("Authorization", "Main"); // перенаправляем пользователя на страницу авторизации
            }
            else
            {
                int user_id = Convert.ToInt32(Session["id"]);
                users user = users.getUserFromUserId(user_id);

                List<privacy_settings> privacySettings = MyFunctions.database.privacy_settings.ToList();

                List<privacy_settings_to_users> privacySettingsToUsers = MyFunctions.database.privacy_settings_to_users.Where(p => (p.user_id == user_id)).ToList();

                List<string> varieties = privacy_settings_to_users.getVarieties();

                string[,] checkBoxesNames = new string[privacySettings.Count, varieties.Count];
                Dictionary<string, int> checkBoxesValues = new Dictionary<string, int>();
                for (int privacySettingId = 0; privacySettingId < privacySettings.Count; privacySettingId++)
                {
                    for (int varietyId = 0; varietyId < varieties.Count; varietyId++)
                    {
                        checkBoxesNames[privacySettingId, varietyId] = "privacy_setting_" + privacySettings.ElementAt(privacySettingId).id + "_" + varieties.ElementAt(varietyId);
                    }

                    if (Request.Form["ok"] == null) // если ещё не была нажата кнопка сохранения настроек - загружаем настройки из БД
                    {
                        checkBoxesValues.Add(checkBoxesNames[privacySettingId, 0], privacySettingsToUsers.ElementAt(privacySettingId).for_friends);
                        checkBoxesValues.Add(checkBoxesNames[privacySettingId, 1], privacySettingsToUsers.ElementAt(privacySettingId).for_friends_of_friends);
                        checkBoxesValues.Add(checkBoxesNames[privacySettingId, 2], privacySettingsToUsers.ElementAt(privacySettingId).for_subscriptions);
                        checkBoxesValues.Add(checkBoxesNames[privacySettingId, 3], privacySettingsToUsers.ElementAt(privacySettingId).for_subcribers);
                        checkBoxesValues.Add(checkBoxesNames[privacySettingId, 4], privacySettingsToUsers.ElementAt(privacySettingId).for_others);
                    }
                    else // если была нажата кнопка сохранения - сохраняем настройки в БД
                    {
                        privacySettingsToUsers.ElementAt(privacySettingId).for_friends = Convert.ToByte(Request.Form[checkBoxesNames[privacySettingId, 0]] == "on");
                        privacySettingsToUsers.ElementAt(privacySettingId).for_friends_of_friends = Convert.ToByte(Request.Form[checkBoxesNames[privacySettingId, 1]] == "on");
                        privacySettingsToUsers.ElementAt(privacySettingId).for_subscriptions = Convert.ToByte(Request.Form[checkBoxesNames[privacySettingId, 2]] == "on");
                        privacySettingsToUsers.ElementAt(privacySettingId).for_subcribers = Convert.ToByte(Request.Form[checkBoxesNames[privacySettingId, 3]] == "on");
                        privacySettingsToUsers.ElementAt(privacySettingId).for_others = Convert.ToByte(Request.Form[checkBoxesNames[privacySettingId, 4]] == "on");
                    }
                }

                if (Request.Form["ok"] == null) // если ещё не была нажата кнопка сохранения настроек - загружаем настройки из БД
                {

                    ViewBag.CheckBoxesNames = checkBoxesNames;
                    ViewBag.CheckBoxesValues = checkBoxesValues;

                    return View();
                }
                else
                {
                    MyFunctions.database.SaveChanges();
                    return RedirectToAction("Index", "Settings"); // перенаправляем пользователя на страницу настроек
                }
            }
        }

        public ActionResult BlackList()
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

        public ActionResult Page()
        {
            if (users.isUserLoggedIn() != true) // если пользователь не авторизован
            {
                return RedirectToAction("Authorization", "Main"); // перенаправляем пользователя на страницу авторизации
            }
            else
            {
                int user_id = Convert.ToInt32(Session["id"]);
                users user = users.getUserFromUserId(user_id);

                if (Request.Form["ok"] != null) // если была нажата кнопка сохранения настроек
                {
                    ViewBag.status = Request.Form["status"];
                    ViewBag.info = Request.Form["info"];
                    ViewBag.avatar_file_url = Request.Form["avatar_file_url"];

                    user.status = ViewBag.status;
                    user.info = ViewBag.info;
                    user.avatar_file_url = ViewBag.avatar_file_url;

                    return RedirectToAction("Index", "Settings"); // перенаправляем пользователя на страницу настроек
                }
                else
                {
                    ViewBag.status = user.status;
                    ViewBag.info = user.info;
                    ViewBag.avatar_file_url = user.avatar_file_url;
                }

                return View();
            }
        }
    }
}