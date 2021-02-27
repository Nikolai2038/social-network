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

    public enum PermissionsToUser
    {
        CAN_SEE_MY_FRIENDS,
        CAN_SEE_MY_SUBSCRIBERS,
        CAN_SEE_MY_SUBSCRIPTIONS,
        CAN_SEE_MY_SHOWCASE,
        CAN_SEE_MY_IMAGES,
        CAN_SEE_MY_VIDEOS,
        CAN_SEE_MY_AUDIOS,
        CAN_SEE_MY_DOCUMENTS,
        CAN_SEE_MY_ARTICLES,
        CAN_SEE_MY_COLLECTIONS,
        CAN_SEE_RECORDS_ON_MY_PAGE,
        CAN_MESSAGE_ME, // отправка сообщений
        CAN_CREATE_RECORDS_ON_MY_PAGE,
        CAN_COMMENT_MY_IMAGES,
        CAN_COMMENT_MY_VIDEOS,
        CAN_COMMENT_MY_AUDIOS,
        CAN_COMMENT_MY_DOCUMENTS,
        CAN_COMMENT_MY_ARTICLES,
        CAN_COMMENT_MY_COLLECTIONS,
        CAN_COMMENT_RECORDS_ON_MY_PAGE,
        CAN_SHARE_MY_IMAGES,
        CAN_SHARE_MY_VIDEOS,
        CAN_SHARE_MY_AUDIOS,
        CAN_SHARE_MY_DOCUMENTS,
        CAN_SHARE_MY_ARTICLES,
        CAN_SHARE_MY_COLLECTIONS,
        CAN_SHARE_RECORDS_ON_MY_PAGE,

        CAN_BAN_AND_UNBAN,

        CAN_ACCEPT_SUBSCRIBER,
        CAN_UNACCEPT_FRIEND,
        CAN_SUBSCRIBE,
        CAN_UNSUBSCRIBE,
        CAN_ADD_TO_BLACK_LIST,
        CAN_REMOVE_FROM_BLACK_LIST,
        CAN_SEE_AUTHORIZATION_HISTORY,
        CAN_CHANGE_SPECIAL_PERMISSIONS,

        CAN_SEE_ACTIONS_TR,

        COUNT
    }

    public enum PermissionsToObject
    {
        CAN_SEE,
        CAN_EDIT,
        CAN_DELETE,
        CAN_COMMENT,
        CAN_SHARE,

        COUNT
    }

    public partial class privacy_settings_to_users
    {
        public int id { get; set; }
        public byte privacy_setting_id { get; set; }
        public int user_id { get; set; }
        public byte for_friends { get; set; }
        public byte for_friends_of_friends { get; set; }
        public byte for_subcribers { get; set; }
        public byte for_subscriptions { get; set; }
        public byte for_others { get; set; }

        public virtual privacy_settings privacy_settings { get; set; }
        public virtual users users { get; set; }

        public static List<string> getVarieties()
        {
            List<string> varieties = new List<string>();
            varieties.Add("for_friends");
            varieties.Add("for_friends_of_friends");
            varieties.Add("for_subscribers");
            varieties.Add("for_subscriptions");
            varieties.Add("for_others");
            return varieties;
        }
    }
}
