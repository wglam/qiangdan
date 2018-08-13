using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfQiangdan.bean;
using WpfQiangdan.db;
using WpfQiangdan.net;

namespace WpfQiangdan.work
{
    class QiangdanWork
    {

        public static TaskLooper qiangDanTaskLooper(User user)
        {
            if (user == null) return null;
            if (String.IsNullOrWhiteSpace(user.account)) return null;
            if (String.IsNullOrWhiteSpace(user.token))
            {
                user.state = 2;
                user.message = "TOKEN 为空";
                return null;
            }

            return new TaskLooper(DbValue.loopDelay, user.account, () =>
            {
                Response<object> code = NetWork.generateOrderBy(user);
                /*   if (code.code == 0)
                   {
                       //     user.state = 1;
                       //   user.isCheck = false;
                       //   stop(user.account);
                       user.message = String.IsNullOrWhiteSpace(code.message) ? "code 0" : code.message;
                   }
                   else
                       */
                if (code.code == 401)
                {
                    user.state = 2;
                    user.message = "401";
                    stop(user.account);
                }
                else
                {
                    user.message = "code: " + code.code + " message: " + code.message;
                }
            });
        }

        private static IDictionary<string, TaskLooper> taskMap = new ConcurrentDictionary<string, TaskLooper>();

        private static void stop(string key)
        {
            try
            {
                if (taskMap.ContainsKey(key))
                {
                    TaskLooper looper;
                    taskMap.TryGetValue(key, out looper);
                    looper.cancelAll();
                    taskMap.Remove(key);
                }
            }
            catch (Exception)
            {
            }
        }


        public static void start(ICollection<User> users)
        {
            if (users == null) return;
            if (users.Count <= 0) return;

            Task.Run(() =>
            {
                foreach (User item in users)
                {
                    if (!taskMap.ContainsKey(item.account))
                    {
                        TaskLooper taskLooper = qiangDanTaskLooper(item);
                        if (taskLooper != null)
                        {
                            taskMap.Add(item.account, taskLooper);
                            taskLooper.execute();
                            item.state = -1;
                        }
                    }
                    else if (item.state == 0 || item.state == 2)
                    {
                        TaskLooper taskLooper;
                        taskMap.TryGetValue(item.account, out taskLooper);
                        if (taskLooper != null)
                        {
                            taskLooper.execute();
                            item.state = -1;
                        }
                    }
                }
            });

        }

        public static void stop(ICollection<User> users)
        {
            if (taskMap.Count <= 0) return;
            Task.Run(() =>
            {
                foreach (User item in users)
                {
                    if (taskMap.ContainsKey(item.account) && item.state == -1)
                    {
                        try
                        {
                            TaskLooper task;
                            taskMap.TryGetValue(item.account, out task);
                            if (task != null)
                            {
                                taskMap.Remove(item.account);
                                task.cancelAll();
                                item.state = 2;
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            });
        }
    }
}
