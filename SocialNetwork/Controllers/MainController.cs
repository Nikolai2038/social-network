using SocialNetwork.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SocialNetwork.Controllers
{
    public class MainController : Controller
    {
        private static Entities_Database_SocialNetwork m_database = new Entities_Database_SocialNetwork();

        public ActionResult Index()
        {
            if (users.isUserLoggedIn() != true) // если пользователь не находится в аккаунте
            {
                ViewBag.ViewingUser = null;
            }
            else
            {
                int user_id = Convert.ToInt32(Session["id"]);
                ViewBag.ViewingUser = users.getUserFromUserId(user_id);
            }
            return View();
        }

        public ActionResult Registration()
        {
            if (users.isUserLoggedIn() == true) // если пользователь находится в аккаунте
            {
                return RedirectToAction("Viewing", "Users"); // перенаправляем пользователя в его профиль
            }
            else
            {
                List<string> errors = new List<string>();

                if (Request.Form["ok"] != null)
                {
                    string login = Request.Form["login"];
                    string password = Request.Form["password"];
                    string password_2 = Request.Form["password_2"];
                    string secret_question = Request.Form["secret_question"];
                    string secret_answer = Request.Form["secret_answer"];

                    MyFunctions.checkLogin(login, ref errors);
                    MyFunctions.checkPassword(password, ref errors);
                    MyFunctions.checkPassword2(password, password_2, ref errors);
                    MyFunctions.checkSecretQuestion(secret_question, secret_answer, ref errors);
                    MyFunctions.checkSecretAnswer(secret_answer, false, ref errors);

                    if (errors.Count == 0)
                    {
                        MyFunctions.checkLoginNotExistInDatabase(login, ref errors);

                        if (errors.Count == 0)
                        {
                            users user = new users(); // новый пользователь
                            user.login = login;
                            user.password_sha512 = users.generateSha512(password);
                            if (secret_question != "") // если был указан секретный вопрос - сохраняем и его, ответ на него
                            {
                                user.secret_question = secret_question;
                                user.secret_answer_sha512 = users.generateSha512(secret_answer);
                            }
                            user.permissions_rank = 0; // ранг: 0 – пользователь; 1 – модератор; 2 – администратор; 3 – главный администратор
                            user.registration_datetime_int = (int)((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds);

                            m_database.users.Add(user); // добавляем пользователя в БД
                            m_database.SaveChanges(); // сохраняем БД

                            users.SaveSession(user);

                            // после добавления пользователя в БД, добавим для него имя и уникальное специальное имя
                            users user_found = m_database.users.Where(p => (p.login == user.login) && (p.password_sha512 == user.password_sha512)).FirstOrDefault();
                            user_found.special_name = "id" + Session["id"];
                            user_found.name = user_found.special_name;

                            m_database.SaveChanges(); // сохраняем БД

                            users.SaveSession(user);

                            return RedirectToAction("Viewing", "Users"); // перенаправляем пользователя в его профиль
                        }
                    }
                }

                ViewBag.Errors = errors;
                return View();
            }
        }

        public ActionResult Authorization()
        {
            if (users.isUserLoggedIn() == true) // если пользователь находится в аккаунте
            {
                return RedirectToAction("Viewing", "Users"); // перенаправляем пользователя в его профиль
            }
            else
            {
                List<string> errors = new List<string>();

                if (Request.Form["ok"] != null)
                {
                    string login = Request.Form["login"];
                    string password = Request.Form["password"];

                    MyFunctions.checkLogin(login, ref errors);
                    MyFunctions.checkPassword(password, ref errors);

                    if (errors.Count == 0)
                    {
                        string password_sha512 = users.generateSha512(password);

                        users user_found = MyFunctions.checkLoginAndPasswordSha512ExistInDatabase(login, password_sha512, ref errors);

                        if (errors.Count == 0)
                        {
                            users.SaveSession(user_found); // сохраняем сессию
                            return RedirectToAction("Viewing", "Users"); // перенаправляем пользователя в его профиль
                        }
                    }
                }

                ViewBag.Errors = errors;
                return View();
            }
        }

        public ActionResult PasswordRecovery()
        {
            if (users.isUserLoggedIn() == true) // если пользователь находится в аккаунте
            {
                return RedirectToAction("Viewing", "Users"); // перенаправляем пользователя в его профиль
            }
            else
            {
                List<string> errors = new List<string>();

                // стадия ввода логина
                if ((Request.Form["next"] != null) || (Request.Form["next_2"] != null) || (Request.Form["ok"] != null))
                {
                    string login = Request.Form["login"];

                    MyFunctions.checkLogin(login, ref errors);

                    if (errors.Count == 0)
                    {
                        users user_found = MyFunctions.checkLoginExistInDatabase(login, ref errors);
                        ViewBag.IsLoginCorrect = false;
                        if (errors.Count == 0)
                        {
                            ViewBag.IsLoginCorrect = true;

                            ViewBag.SecretQuestion = user_found.secret_question; // получение текста секретного вопроса из БД

                            // стадия ввода ответа на секретный вопрос
                            if ((Request.Form["next_2"] != null) || (Request.Form["ok"] != null))
                            {
                                string secret_answer = Request.Form["secret_answer"];
                                string secret_answer_sha512 = users.generateSha512(secret_answer);

                                MyFunctions.checkSecretAnswer(secret_answer, true, ref errors);

                                ViewBag.IsSecretAnswerCorrect = false;
                                if (errors.Count == 0)
                                {
                                    user_found = MyFunctions.checkSecretAnswerSha512InDatabase(secret_answer_sha512, login, ref m_database, ref errors);
                                    if (errors.Count == 0)
                                    {
                                        ViewBag.IsSecretAnswerCorrect = true;

                                        // стадия ввода нового пароля
                                        if (Request.Form["ok"] != null)
                                        {
                                            string password = Request.Form["password"];
                                            string password_2 = Request.Form["password_2"];

                                            MyFunctions.checkPassword(password, ref errors);
                                            MyFunctions.checkPassword2(password, password_2, ref errors);
                                            if (errors.Count == 0)
                                            {
                                                string password_sha512 = users.generateSha512(password);
                                                user_found.password_sha512 = password_sha512; // меняем пароль пользователя в БД
                                                m_database.SaveChanges(); // сохраняем БД
                                                users.SaveSession(user_found); // сохраняем сессию
                                                return RedirectToAction("Viewing", "Users"); // перенаправляем пользователя в его профиль
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                }

                ViewBag.Errors = errors;
                return View();
            }
        }

        public ActionResult Logout()
        {
            if (users.isUserLoggedIn() == true) // если пользователь находится в аккаунте
            {
                users.ResetSession(); // сбрасываем сессию пользователя
                return RedirectToAction("Authorization", "Main"); // перенаправляем пользователя на страницу авторизации
            }
            else
            {
                return RedirectToAction("Viewing", "Users"); // перенаправляем пользователя в его профиль
            }
        }
    }
}