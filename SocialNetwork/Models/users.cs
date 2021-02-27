//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SocialNetwork.Models
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;

    public partial class users
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public users()
        {
            this.authorization_history = new HashSet<authorization_history>();
            this.bans = new HashSet<bans>();
            this.bans1 = new HashSet<bans>();
            this.black_list = new HashSet<black_list>();
            this.black_list1 = new HashSet<black_list>();
            this.dialogs = new HashSet<dialogs>();
            this.friends_and_subscriptions = new HashSet<friends_and_subscriptions>();
            this.friends_and_subscriptions1 = new HashSet<friends_and_subscriptions>();
            this.messages = new HashSet<messages>();
            this.objects_with_name_to_showcases = new HashSet<objects_with_name_to_showcases>();
            this.privacy_settings_to_users = new HashSet<privacy_settings_to_users>();
            this.ratings_to_objects_with_rating = new HashSet<ratings_to_objects_with_rating>();
            this.records = new HashSet<records>();
            this.users_to_dialogs = new HashSet<users_to_dialogs>();
        }

        public int id { get; set; }
        public string login { get; set; }
        public string name { get; set; }
        public string special_name { get; set; }
        public Nullable<byte> permissions_rank { get; set; }
        public string password_sha512 { get; set; }
        public string secret_question { get; set; }
        public string secret_answer_sha512 { get; set; }
        public string avatar_file_url { get; set; }
        public Nullable<int> registration_datetime_int { get; set; }
        public Nullable<int> last_activity_datetime_int { get; set; }
        public string status { get; set; }
        public string info { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<authorization_history> authorization_history { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<bans> bans { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<bans> bans1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<black_list> black_list { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<black_list> black_list1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dialogs> dialogs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<friends_and_subscriptions> friends_and_subscriptions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<friends_and_subscriptions> friends_and_subscriptions1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<messages> messages { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<objects_with_name_to_showcases> objects_with_name_to_showcases { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<privacy_settings_to_users> privacy_settings_to_users { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ratings_to_objects_with_rating> ratings_to_objects_with_rating { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<records> records { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<users_to_dialogs> users_to_dialogs { get; set; }

        public static string generateSha512(string text)
        {
            SHA512 sha512 = SHA512.Create();
            byte[] bytes = sha512.ComputeHash(Encoding.ASCII.GetBytes(text));
            string result = "";
            foreach (byte b in bytes)
            {
                result += b.ToString("x2");
            }
            return result;
        }

        public static void SaveSession(users user)
        {
            HttpContext.Current.Session["login"] = user.login;
            HttpContext.Current.Session["password_sha512"] = user.password_sha512;

            users user_found = MyFunctions.database.users.Where(p => (p.login == user.login) && (p.password_sha512 == user.password_sha512)).FirstOrDefault();

            authorization_history authorizationHistory = new authorization_history();
            authorizationHistory.user_id = user_found.id;
            authorizationHistory.authorization_datetime_int = (int)((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds);
            authorizationHistory.ip_address = HttpContext.Current.Request.UserHostAddress;
            MyFunctions.database.authorization_history.Add(authorizationHistory);
            MyFunctions.database.SaveChanges();

            HttpContext.Current.Session["id"] = user_found.id;
        }

        public static void ResetSession()
        {
            HttpContext.Current.Session["login"] = null;
            HttpContext.Current.Session["password_sha512"] = null;
            HttpContext.Current.Session["id"] = null;
        }

        public static bool isUserLoggedIn()
        {
            if (HttpContext.Current.Session["login"] != null)
            {
                if (HttpContext.Current.Session["password_sha512"] != null)
                {
                    string login = HttpContext.Current.Session["login"].ToString();
                    string password_sha512 = HttpContext.Current.Session["password_sha512"].ToString();

                    users user = MyFunctions.database.users.Where(p => (p.login == login) && (p.password_sha512 == password_sha512)).FirstOrDefault();

                    if (user == null || user.isBanned())
                    {
                        ResetSession(); // сбрасываем сессию пользователя
                        return false;
                    }
                    else
                    {
                        user.last_activity_datetime_int = (int)((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds);
                        MyFunctions.database.SaveChanges();
                        return true;
                    }
                }
                else // если в сессии сохранён логин, но не пароль
                {
                    ResetSession(); // сбрасываем сессию пользователя
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static users getUserFromUserId(int id)
        {
            users user_found = MyFunctions.database.users.Where(p => (p.id == id)).FirstOrDefault();
            return user_found;
        }

        public static users getUserFromUserSpecialName(string special_name)
        {
            users user_found = MyFunctions.database.users.Where(p => (p.special_name == special_name)).FirstOrDefault();
            return user_found;
        }

        public List<users> getFriends()
        {
            List<friends_and_subscriptions> friendsAndSubscriptions = MyFunctions.database.friends_and_subscriptions.Where(p => ((p.user_id_from == id) && (p.is_accepted == 1))).ToList();

            List<users> result = new List<users>();
            foreach (friends_and_subscriptions _friendsAndSubscriptions in friendsAndSubscriptions)
            {
                users user = MyFunctions.database.users.Where(p => (p.id == _friendsAndSubscriptions.user_id_to)).FirstOrDefault();
                result.Add(user);
            }
            return result;
        }

        public List<users> getSubscribers()
        {
            List<friends_and_subscriptions> friendsAndSubscriptions = MyFunctions.database.friends_and_subscriptions.Where(p => ((p.user_id_to == id) && (p.is_accepted == 0))).ToList();

            List<users> result = new List<users>();
            foreach (friends_and_subscriptions _friendsAndSubscriptions in friendsAndSubscriptions)
            {
                users user = MyFunctions.database.users.Where(p => (p.id == _friendsAndSubscriptions.user_id_from)).FirstOrDefault();
                result.Add(user);
            }
            return result;
        }

        public List<users> getSubscriptions()
        {
            List<friends_and_subscriptions> friendsAndSubscriptions = MyFunctions.database.friends_and_subscriptions.Where(p => ((p.user_id_from == id) && (p.is_accepted == 0))).ToList();

            List<users> result = new List<users>();
            foreach (friends_and_subscriptions _friendsAndSubscriptions in friendsAndSubscriptions)
            {
                users user = MyFunctions.database.users.Where(p => (p.id == _friendsAndSubscriptions.user_id_to)).FirstOrDefault();
                result.Add(user);
            }
            return result;
        }

        public int getFriendsCount()
        {
            return MyFunctions.database.friends_and_subscriptions.Where(p => ((p.user_id_from == id) && (p.is_accepted == 1))).Count();
        }

        public int getSubscribersCount()
        {
            return MyFunctions.database.friends_and_subscriptions.Where(p => ((p.user_id_to == id) && (p.is_accepted == 0))).Count();
        }

        public int getSubscriptionsCount()
        {
            return MyFunctions.database.friends_and_subscriptions.Where(p => ((p.user_id_from == id) && (p.is_accepted == 0))).Count();
        }

        public List<bans> getBans()
        {
            return MyFunctions.database.bans.Where(p => ((p.user_id_to == id))).ToList();
        }
        public List<records_simple> getRecords()
        {
            return MyFunctions.database.records_simple.Where(p => ((p.user_id_to == id))).ToList();
        }

        public int getBansCount()
        {
            return MyFunctions.database.bans.Where(p => ((p.user_id_to == id))).Count();
        }

        public bool isFriendToUser(users user)
        {
            List<users> friends_of_user = user.getFriends();
            if (friends_of_user.Find(p => (p == this)) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool isFriendOfFriendToUser(users user)
        {
            List<users> friends_of_user = user.getFriends();
            foreach (users friend in friends_of_user)
            {
                List<users> friends_of_friend = friend.getFriends();
                if (friends_of_friend.Find(p => (p == this)) != null)
                {
                    return true;
                }
            }
            return false;
        }

        public bool isSubscriberToUser(users user)
        {
            List<users> subscribers_of_user = user.getSubscribers();
            if (subscribers_of_user.Find(p => (p == this)) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool isSubscriptionToUser(users user)
        {
            List<users> subscriprions_of_user = user.getSubscriptions();
            if (subscriprions_of_user.Find(p => (p == this)) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static Dictionary<PermissionsToUser, bool> getUserPermissionsToUser(users user_from, users user_to)
        {
            Dictionary<PermissionsToUser, bool> result = new Dictionary<PermissionsToUser, bool>();

            List<privacy_settings_to_users> privacySettingsToUsers = MyFunctions.database.privacy_settings_to_users.Where(p => (p.user_id == user_to.id)).ToList();

            int privacy_settings_count = MyFunctions.database.privacy_settings.Count();

            for (int i = 0; i < (int)PermissionsToUser.COUNT; i++)
            {
                result.Add((PermissionsToUser)i, false);
            }

            bool is_friend = false;
            bool is_friend_of_friend = false;
            bool is_subscriber = false;
            bool is_subscription = false;
            bool is_other = true;

            if (user_from == null) // если первый пользователь не авторизован
            {
                for (int i = 0; i < 11; i++) // (начиная с 11 права - необходим аккаунт, поэтому незарегистрированному пользователю автоматически будет всё запрещено)
                {
                    result[(PermissionsToUser)i] = (is_other && (privacySettingsToUsers.ElementAt(i).for_others == 1));
                }
            }
            else // если первый пользователь авторизован
            {
                if (user_from.id == user_to.id) // права пользователя к самому себе
                {
                    for (int i = 0; i < privacy_settings_count; i++)
                    {
                        result[(PermissionsToUser)i] = true;
                    }
                    result[PermissionsToUser.CAN_SEE_AUTHORIZATION_HISTORY] = true; // пользователю разрешено просматривать историю своей авторизации в любом случае
                }
                else
                {
                    is_friend = user_from.isFriendToUser(user_to);
                    is_friend_of_friend = user_from.isFriendOfFriendToUser(user_to);
                    is_subscriber = user_from.isSubscriberToUser(user_to);
                    is_subscription = user_from.isSubscriptionToUser(user_to);

                    if (user_from.permissions_rank > user_to.permissions_rank) // если текущий пользователь по рангу выше указанного - ему разрешено всё
                    {
                        for (int i = 0; i < privacy_settings_count; i++)
                        {
                            result[(PermissionsToUser)i] = true;
                        }

                        if (user_from.permissions_rank >= 2) // от администратора и выше
                        {
                            result[PermissionsToUser.CAN_BAN_AND_UNBAN] = true;
                            result[PermissionsToUser.CAN_SEE_AUTHORIZATION_HISTORY] = true;
                        }

                        black_list blackList = MyFunctions.database.black_list.Where(p => ((p.user_id_blocked == user_to.id) && (p.user_id_from == user_from.id))).FirstOrDefault(); // блокировка от нас к другому
                        if (blackList != null) // если блокировка есть
                        {
                            result[PermissionsToUser.CAN_REMOVE_FROM_BLACK_LIST] = true;
                        }
                        else // если блокировки нет
                        {
                            result[PermissionsToUser.CAN_ADD_TO_BLACK_LIST] = true;
                        }
                    }
                    else // если ранги совпадают, либо даже наоборот
                    {
                        black_list blackList = MyFunctions.database.black_list.Where(p => ((p.user_id_blocked == user_from.id) && (p.user_id_from == user_to.id))).FirstOrDefault(); // проверка блокировки нас другим пользователем

                        if (blackList == null) // если текущий пользователь не находится в чёрном списке у другого
                        {
                            for (int i = 0; i < privacy_settings_count; i++)
                            {
                                result[(PermissionsToUser)i] = (
                                        (is_friend && (privacySettingsToUsers.ElementAt(i).for_friends == 1)) ||
                                    (is_friend_of_friend && (privacySettingsToUsers.ElementAt(i).for_friends_of_friends == 1)) ||
                                    (is_subscriber && (privacySettingsToUsers.ElementAt(i).for_subcribers == 1)) ||
                                    (is_subscription && (privacySettingsToUsers.ElementAt(i).for_subscriptions == 1)) ||
                                    (is_other && (privacySettingsToUsers.ElementAt(i).for_others == 1))
                                    );
                            }
                        }

                        if (user_from.permissions_rank == user_to.permissions_rank) //  случай одинаковых рангов
                        {
                            black_list blackList2 = MyFunctions.database.black_list.Where(p => ((p.user_id_blocked == user_to.id) && (p.user_id_from == user_from.id))).FirstOrDefault(); // блокировка от нас к другому
                            if (blackList2 != null) // если блокировка есть
                            {
                                result[PermissionsToUser.CAN_REMOVE_FROM_BLACK_LIST] = true;
                            }
                            else // если блокировки нет
                            {
                                result[PermissionsToUser.CAN_ADD_TO_BLACK_LIST] = true;
                            }
                        }
                    }

                    if (user_from.permissions_rank >= 3)
                    {
                        result[PermissionsToUser.CAN_CHANGE_SPECIAL_PERMISSIONS] = true;
                    }
                }

                if (user_from.id == user_to.id) // сообщения к самому себе запрещены
                {
                    result[PermissionsToUser.CAN_MESSAGE_ME] = false;
                }
                else
                {
                    if (user_from.isFriendToUser(user_to)) // если заявка принята с обеих сторон (отправлена одной, принята другой, принятие генерирует автоматическое создание второй заявки в обратном направлении и автоматическое её принятие)
                    {
                        result[PermissionsToUser.CAN_ADD_TO_BLACK_LIST] = false; // для занесения в ЧС необходимо удалить из друзей
                        result[PermissionsToUser.CAN_UNACCEPT_FRIEND] = true;
                    }

                    if (user_to.isSubscriberToUser(user_from)) // если есть входящая заявка от другого пользователя
                    {
                        result[PermissionsToUser.CAN_ACCEPT_SUBSCRIBER] = true;
                    }

                    if (user_from.isSubscriberToUser(user_to)) // если есть исходящая завка к другому пользователю
                    {
                        result[PermissionsToUser.CAN_UNSUBSCRIBE] = true;
                    }
                    else if (!user_to.isSubscriberToUser(user_from) && !user_from.isFriendToUser(user_to))
                    {
                        result[PermissionsToUser.CAN_SUBSCRIBE] = true;
                    }
                }
            }

            result[PermissionsToUser.CAN_SEE_ACTIONS_TR] = (
                result[PermissionsToUser.CAN_ACCEPT_SUBSCRIBER] ||
                result[PermissionsToUser.CAN_UNACCEPT_FRIEND] ||
                result[PermissionsToUser.CAN_SUBSCRIBE] ||
                result[PermissionsToUser.CAN_UNSUBSCRIBE] ||
                result[PermissionsToUser.CAN_ADD_TO_BLACK_LIST] ||
                result[PermissionsToUser.CAN_REMOVE_FROM_BLACK_LIST] ||
                result[PermissionsToUser.CAN_SEE_AUTHORIZATION_HISTORY] ||
                result[PermissionsToUser.CAN_CHANGE_SPECIAL_PERMISSIONS]
                ); // будет ли отображаться строчка с кнопками - если ни одна кнопка отображаться не будет, то нет смысла выводить пустую строчку таблицы

            return result;
        }

        public string getLastActivityStatusAsString()
        {
            string result = "";

            int total_datetime_int = (int)((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds);

            if (total_datetime_int - this.last_activity_datetime_int <= 60 * 5)
            {
                result += "<td colspan=\"6\" class=\"status_online\">";
                result += "<span>Онлайн</span>";
            }
            else
            {
                result += "<td colspan=\"6\" class=\"status_offline\">";
                result += "<span>Оффлайн</span>";
            }
            result += "<br />";
            result += "[" + getDatetimeStringFromDatetimeInt(Convert.ToInt32(this.last_activity_datetime_int)) + "]";
            result += "</td>";

            return result;
        }

        public static string getDatetimeStringFromDatetimeInt(int datetime_int)
        {
            string datetime_string = "";

            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(datetime_int).ToLocalTime();

            datetime_string += dateTime.ToLongDateString() + " в " + dateTime.ToLongTimeString();

            return datetime_string;
        }

        public bool isBanned()
        {
            int total_datetime_int = (int)((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds);
            bans _bans = MyFunctions.database.bans.Where(p => (p.user_id_to == this.id) && (p.unban_datetime_int > total_datetime_int)).FirstOrDefault();
            return (_bans != null);
        }
    }
}
