using SocialNetwork.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace SocialNetwork
{
    public class MyFunctions
    {
        public static Entities_Database_SocialNetwork database = new Entities_Database_SocialNetwork();

        private class Range
        {
            public int Min { get; }
            public int Max { get; }
            public Range(int _Min, int _Max)
            {
                this.Min = _Min;
                this.Max = _Max;
            }
        }

        private static readonly string SYMBOLS_EN               = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static readonly string SYMBOLS_RU               = "абвгдеёжзийклмнопрстуфхцчшщъыьэюяАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
        private static readonly string SYMBOLS_NUMBERS          = "0123456789";
        private static readonly string SYMBOLS_DASH             = "-";
        private static readonly string SYMBOLS_SPECIAL_SYMBOLS  = "!$%^*()-=+<>.,{}[];; ";

        private static readonly string ALLOWED_SYMBOLS_FOR_USER_LOGIN            = SYMBOLS_EN + SYMBOLS_NUMBERS + SYMBOLS_DASH;
        private static readonly string ALLOWED_SYMBOLS_FOR_USER_PASSWORD         = SYMBOLS_EN + SYMBOLS_NUMBERS + SYMBOLS_SPECIAL_SYMBOLS;
        private static readonly string ALLOWED_SYMBOLS_FOR_USER_SECRET_QUESTION  = SYMBOLS_EN + SYMBOLS_RU + SYMBOLS_NUMBERS + SYMBOLS_DASH;
        private static readonly string ALLOWED_SYMBOLS_FOR_USER_SECRET_ANSWER    = SYMBOLS_EN + SYMBOLS_RU + SYMBOLS_NUMBERS + SYMBOLS_DASH;
        private static readonly string ALLOWED_SYMBOLS_FOR_USER_NAME             = SYMBOLS_EN + SYMBOLS_RU + SYMBOLS_NUMBERS + SYMBOLS_DASH;
        private static readonly string ALLOWED_SYMBOLS_FOR_USER_SPECIAL_NAME     = SYMBOLS_EN + SYMBOLS_NUMBERS + SYMBOLS_DASH;

        private static readonly string ALLOWED_SYMBOLS_FOR_USER_LOGIN_DESCRIPTION            =  "Разрешены только английские буквы, цифры и символ дефиса.";
        private static readonly string ALLOWED_SYMBOLS_FOR_USER_PASSWORD_DESCRIPTION         = $"Разрешены только английские буквы, цифры и специальные символы ({SYMBOLS_SPECIAL_SYMBOLS})";
        private static readonly string ALLOWED_SYMBOLS_FOR_USER_SECRET_QUESTION_DESCRIPTION  =  "Разрешены только английские и русские буквы, цифры и символ дефиса.";
        private static readonly string ALLOWED_SYMBOLS_FOR_USER_SECRET_ANSWER_DESCRIPTION    =  "Разрешены только английские и русские буквы, цифры и символ дефиса.";
        private static readonly string ALLOWED_SYMBOLS_FOR_USER_NAME_DESCRIPTION             =  "Разрешены только английские и русские буквы, цифры и символ дефиса.";
        private static readonly string ALLOWED_SYMBOLS_FOR_USER_SPECIAL_NAME_DESCRIPTION     =  "Разрешены только английские буквы, цифры и символ дефиса.";

        private static readonly Range ALLOWED_RANGE_FOR_USER_LOGIN           = new Range(3, 32);
        private static readonly Range ALLOWED_RANGE_FOR_USER_PASSWORD        = new Range(4, 128);
        private static readonly Range ALLOWED_RANGE_FOR_USER_SECRET_QUESTION = new Range(4, 128);
        private static readonly Range ALLOWED_RANGE_FOR_USER_SECRET_ANSWER   = new Range(4, 128);
        private static readonly Range ALLOWED_RANGE_FOR_USER_NAME            = new Range(3, 64);
        private static readonly Range ALLOWED_RANGE_FOR_USER_SPECIAL_NAME    = new Range(3, 32);

        private static bool isValueSymbolsOk(string value, string allowed_symbols)
        {
            foreach (char symbol in value)
            {
                bool is_symbol_allowed = false;
                foreach (char allowed_symbol in allowed_symbols)
                {
                    if (symbol == allowed_symbol)
                    {
                        is_symbol_allowed = true;
                        break;
                    }
                }
                if (!is_symbol_allowed)
                {
                    return false;
                }
            }
            return true;
        }

        private static bool isValueRangeOk(string value, Range allowed_range)
        {
            if ((value.Length < allowed_range.Min) ||
                (value.Length > allowed_range.Max))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static void checkLogin(string login, ref List<string> errors)
        {
            Range allowed_range = null;

            if (login.Length == 0)
            {
                errors.Add("Введите логин!");
            }
            else if (!MyFunctions.isValueRangeOk(login, allowed_range = MyFunctions.ALLOWED_RANGE_FOR_USER_LOGIN))
            {
                errors.Add($"Длина логина - от {allowed_range.Min} до {allowed_range.Max} символов (включительно)!");
            }
            if (!MyFunctions.isValueSymbolsOk(login, MyFunctions.ALLOWED_SYMBOLS_FOR_USER_LOGIN))
            {
                errors.Add("Логин содержит недопустимые символы! " + MyFunctions.ALLOWED_SYMBOLS_FOR_USER_LOGIN_DESCRIPTION);
            }
        }

        // проверка, не существует ли логина в БД - если существует, то ошибка
        public static void checkLoginNotExistInDatabase(string login, ref List<string> errors)
        {
            users user_found = database.users.Where(p => (p.login == login)).FirstOrDefault();

            if (user_found != null) // если пользователь найден в БД
            {
                errors.Add("Пользователь с таким логином уже зарегистрирован!");
            }
        }

        // проверка, существует ли логин в БД - если не существует, то ошибка
        public static users checkLoginExistInDatabase(string login, ref List<string> errors)
        {
            users user_found = database.users.Where(p => (p.login == login)).FirstOrDefault();

            if (user_found == null) // если пользователь не найден в БД
            {
                errors.Add("Пользователь с таким логином не найден!");
            }
            return user_found;
        }

        public static users checkLoginAndPasswordSha512ExistInDatabase(string login, string password_sha512, ref List<string> errors)
        {
            users user_found = database.users.Where(p => (p.login == login) && (p.password_sha512 == password_sha512)).FirstOrDefault();

            if (user_found == null) // если пользователь не найден в БД или пароль неверен
            {
                errors.Add("Неверный логин или пароль!");
            }
            else if (user_found.isBanned())
            {
                errors.Add("Данный аккаунт забанен!");
            }

            return user_found;
        }

        public static void checkPassword(string password, ref List<string> errors)
        {
            Range allowed_range = null;

            if (password.Length == 0)
            {
                errors.Add("Введите пароль!");
            }
            else if (!MyFunctions.isValueRangeOk(password, allowed_range = MyFunctions.ALLOWED_RANGE_FOR_USER_PASSWORD))
            {
                errors.Add($"Длина пароля - от {allowed_range.Min} до {allowed_range.Max} символов (включительно)!");
            }
            if (!MyFunctions.isValueSymbolsOk(password, MyFunctions.ALLOWED_SYMBOLS_FOR_USER_PASSWORD))
            {
                errors.Add("Пароль содержит недопустимые символы! " + MyFunctions.ALLOWED_SYMBOLS_FOR_USER_PASSWORD_DESCRIPTION);
            }
        }

        public static void checkPassword2(string password, string password_2, ref List<string> errors)
        {
            if (password_2.Length == 0)
            {
                errors.Add("Введите подтверждение пароля!");
            }
            else if (password != password_2)
            {
                errors.Add("Пароли не совпадают!");
            }
        }

        public static void checkSecretQuestion(string secret_question, string secret_answer, ref List<string> errors)
        {
            Range allowed_range = null;

            if (secret_question.Length != 0)
            {
                if (!MyFunctions.isValueRangeOk(secret_question, allowed_range = MyFunctions.ALLOWED_RANGE_FOR_USER_SECRET_QUESTION))
                {
                    errors.Add($"Длина секретного вопроса - от {allowed_range.Min} до {allowed_range.Max} символов (включительно)!");
                }
                if (!MyFunctions.isValueSymbolsOk(secret_question, MyFunctions.ALLOWED_SYMBOLS_FOR_USER_SECRET_QUESTION))
                {
                    errors.Add("Секретный вопрос содержит недопустимые символы! " + MyFunctions.ALLOWED_SYMBOLS_FOR_USER_SECRET_QUESTION_DESCRIPTION);
                }
            }
            else
            {
                if (secret_answer.Length != 0)
                {
                    errors.Add("Для ответа на секретный вопрос введите сам вопрос!");
                }
            }
        }

        public static void checkSecretAnswer(string secret_answer, bool is_required, ref List<string> errors)
        {
            Range allowed_range = null;

            if (is_required || secret_answer.Length != 0)
            {
                if (!MyFunctions.isValueRangeOk(secret_answer, allowed_range = MyFunctions.ALLOWED_RANGE_FOR_USER_SECRET_ANSWER))
                {
                    errors.Add($"Длина ответа на секретный вопрос - от {allowed_range.Min} до {allowed_range.Max} символов (включительно)!");
                }
                if (!MyFunctions.isValueSymbolsOk(secret_answer, MyFunctions.ALLOWED_SYMBOLS_FOR_USER_SECRET_ANSWER))
                {
                    errors.Add("Ответ на секретный вопрос содержит недопустимые символы! " + MyFunctions.ALLOWED_SYMBOLS_FOR_USER_SECRET_ANSWER_DESCRIPTION);
                }
            }
        }
        public static users checkSecretAnswerSha512InDatabase(string secret_answer_sha512, string login, ref List<string> errors)
        {
            users user_found = database.users.Where(p => (p.login == login) && (p.secret_answer_sha512 == secret_answer_sha512)).FirstOrDefault();

            if (user_found == null) // если пользователь не найден в БД или пароль неверен
            {
                errors.Add("Неверный ответ на секретный вопрос!");
            }
            return user_found;
        }

        public static void checkName(string name, ref List<string> errors)
        {
            Range allowed_range = null;

            if (name.Length == 0)
            {
                errors.Add("Введите имя пользователя!");
            }
            else if (!MyFunctions.isValueRangeOk(name, allowed_range = MyFunctions.ALLOWED_RANGE_FOR_USER_NAME))
            {
                errors.Add($"Длина имени пользователя - от {allowed_range.Min} до {allowed_range.Max} символов (включительно)!");
            }
            if (!MyFunctions.isValueSymbolsOk(name, MyFunctions.ALLOWED_SYMBOLS_FOR_USER_NAME))
            {
                errors.Add("Имя пользователя содержит недопустимые символы! " + MyFunctions.ALLOWED_SYMBOLS_FOR_USER_NAME_DESCRIPTION);
            }
        }

        public static void checkSpecialName(string special_name, ref List<string> errors)
        {
            Range allowed_range = null;

            if (special_name.Length == 0)
            {
                errors.Add("Введите специальное имя пользователя!");
            }
            else if (!MyFunctions.isValueRangeOk(special_name, allowed_range = MyFunctions.ALLOWED_RANGE_FOR_USER_SPECIAL_NAME))
            {
                errors.Add($"Длина специального имени пользователя - от {allowed_range.Min} до {allowed_range.Max} символов (включительно)!");
            }
            if (!MyFunctions.isValueSymbolsOk(special_name, MyFunctions.ALLOWED_SYMBOLS_FOR_USER_SPECIAL_NAME))
            {
                errors.Add("Специальное имя пользователя содержит недопустимые символы! " + MyFunctions.ALLOWED_SYMBOLS_FOR_USER_SPECIAL_NAME_DESCRIPTION);
            }
        }
        public static bool checkTotalPassword(string password, ref List<string> errors)
        {
            Range allowed_range = null;

            if (password.Length == 0)
            {
                errors.Add("Введите текущий пароль!");
                return false;
            }
            else if (!MyFunctions.isValueRangeOk(password, allowed_range = MyFunctions.ALLOWED_RANGE_FOR_USER_PASSWORD))
            {
                errors.Add($"Длина текущего пароля - от {allowed_range.Min} до {allowed_range.Max} символов (включительно)!");
                return false;
            }
            if (!MyFunctions.isValueSymbolsOk(password, MyFunctions.ALLOWED_SYMBOLS_FOR_USER_PASSWORD))
            {
                errors.Add("Текущий пароль содержит недопустимые символы! " + MyFunctions.ALLOWED_SYMBOLS_FOR_USER_PASSWORD_DESCRIPTION);
                return false;
            }
            return true;
        }

        // возвращает список записей на текущей странице из списка всех записей
        public static List<object> pageNavigation_getListOnPage(List<object> list, ref int elements_on_page, ref int total_page_id)
        {
            int min_element_id = 0;
            int max_element_id = list.Count - 1;

            int min_page_id = 1;
            int max_page_id = max_element_id / elements_on_page + 1;

            if (total_page_id < min_page_id)
            {
                total_page_id = min_page_id;
            }
            else if (total_page_id > max_page_id)
            {
                total_page_id = max_page_id;
            }

            int min_id_element_on_page = (total_page_id - 1) * elements_on_page;
            int max_id_element_on_page = total_page_id * elements_on_page - 1;
            if (max_id_element_on_page > max_element_id)
            {
                max_id_element_on_page = max_element_id;
            }

            List<object> list_on_page = new List<object>();
            for (int i = min_id_element_on_page; i <= max_id_element_on_page; i++)
            {
                list_on_page.Add(list.ElementAt(i));
            }

            return list_on_page;
        }

        public static void pageNavigation_printFormWithPages(TextWriter writer, int total_page_id, int elements_on_page, Dictionary<string, string> option_names, string sort_key, string sort_asc)
        {
            writer.WriteLine("Отсортировать по столбцу ");
            writer.WriteLine("<select name=\"sort_key\">");
            foreach (string key in option_names.Keys)
            {
                string value = option_names[key];
                writer.Write($"<option value=\"{key}\"");
                if (key == sort_key)
                {
                    writer.Write(" selected");
                }
                writer.WriteLine(">");
                writer.WriteLine(value);
                writer.WriteLine("</option>");
            }
            writer.WriteLine("</select>");
            writer.WriteLine(" по ");
            writer.WriteLine("<select name=\"sort_asc\">");
            writer.Write("<option value=\"asc\"");
            if (sort_asc == "asc")
            {
                writer.Write(" selected");
            }
            writer.WriteLine(">");
            writer.WriteLine("по возрастанию");
            writer.WriteLine("</option>");
            writer.WriteLine("<option value=\"desc\"");
            if (sort_asc == "desc")
            {
                writer.Write(" selected");
            }
            writer.WriteLine(">");
            writer.WriteLine("по убыванию");
            writer.WriteLine("</option>");
            writer.WriteLine("</select>");
            writer.WriteLine(", перейти на ");
            writer.WriteLine($"<input type=\"text\" name=\"total_page_id\" value=\"{total_page_id}\"/>");
            writer.WriteLine(" страницу, показать ");
            writer.WriteLine($"<input type=\"text\" name=\"elements_on_page\" value=\"{elements_on_page}\"/>");
            writer.WriteLine(" результатов: ");
            writer.WriteLine("<input type=\"submit\" name=\"page_ok\" value=\"Обновить\"/>");
        }

        public static void printUsersSearch(TextWriter writer, string search_text, Dictionary<string, string> option_names, string search_key)
        {
            writer.WriteLine("Найти все включения строчки ");
            writer.WriteLine($"<input type=\"text\" name=\"search_text\" value=\"{search_text}\" class=\"search_text\"/>");
            writer.WriteLine(" по полю ");
            writer.WriteLine("<select name=\"search_key\">");
            foreach (string key in option_names.Keys)
            {
                string value = option_names[key];
                writer.Write($"<option value=\"{key}\"");
                if (key == search_key)
                {
                    writer.Write(" selected");
                }
                writer.WriteLine(">");
                writer.WriteLine(value);
                writer.WriteLine("</option>");
            }
            writer.WriteLine("</select>");
        }
    }
}