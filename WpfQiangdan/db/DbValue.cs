using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfQiangdan.bean;

namespace WpfQiangdan.db
{
    class DbValue
    {
        public const string platform = "android";
        public const string appVersion = "2.8.0";
        public const string channel = "C000";
        public const string osVersion = "6.0.1";



        public static string gbid { get; set; }
        public static int type { get; set; }

        public static long dtime { get; set; }

        private static IDictionary<string, User> users = new ConcurrentDictionary<string, User>();

        public static void add(User user)
        {
            if (user == null) return;
            if (users.ContainsKey(user.account)) return;
            users.Add(user.account, user);
        }

        public static ObservableCollection<User> getUsersObservable()
        {
            return new ObservableCollection<User>(users.Values);
        }

        public static void checkAllUser()
        {
            foreach (User user in users.Values)
            {
                user.isCheck = true;
            }
        }

        public static void checkReverseUser()
        {
            foreach (User user in users.Values)
            {
                user.isCheck = !user.isCheck;
            }
        }

        public static void checkUser(string account, bool isCheck)
        {
            if (users.ContainsKey(account))
            {
                User user;
                users.TryGetValue(account, out user);
                user.isCheck = isCheck;
            }

        }
        public static User getUser(string account)
        {
            if (users.ContainsKey(account))
            {
                User user;
                users.TryGetValue(account, out user);
                return user;
            }

            return null;
        }

        public static ICollection<User> getAllUser()
        {
            return users.Values;
        }

        public static ICollection<User> getCheckedUser(bool checkToken)
        {
            List<User> ret = new List<User>();
            foreach (User user in users.Values)
            {

                if (user.isCheck)
                {

                    if (checkToken && String.IsNullOrWhiteSpace(user.token))
                    {
                        user.isCheck = false;
                        user.message = "没有获取到TOKEN";
                    }
                    else
                    {
                        ret.Add(user);
                    }

                }

            }
            return ret;
        }
    }
}
