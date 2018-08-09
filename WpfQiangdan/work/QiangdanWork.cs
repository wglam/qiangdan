using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfQiangdan.bean;
using WpfQiangdan.net;

namespace WpfQiangdan.work
{
    class QiangdanWork
    {

        public static TaskLooper qiangDanTaskLooper(User user)
        {
            if (user == null) return null;
            if (String.IsNullOrWhiteSpace(user.account)) return null;
            if (String.IsNullOrWhiteSpace(user.token)) return null;

            return new TaskLooper(user.account, () =>
            {
                if (NetWork.generateOrderBy(user))
                {
                    user.state = 1;
                    stop(user.account);
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
                            item.state = -1;
                            taskLooper.execute();
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
                    if (taskMap.ContainsKey(item.account)&&item.state==-1)
                    {
                        try
                        {
                            TaskLooper task;
                            taskMap.TryGetValue(item.account, out task);
                            if (task != null)
                            {
                                taskMap.Remove(item.account);
                                task.cancelAll();
                                item.state = 0;
                            }
                        }
                        catch { 
                        }
                    }
                }
            });
        }
    }
}
