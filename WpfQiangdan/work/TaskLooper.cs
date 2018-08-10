using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;



namespace WpfQiangdan.work
{
    class TaskLooper
    {
        private string key;
        private Action action;
        private CancellationTokenSource taskCancel;
        private int LoopTime = 100;

        public string Key
        {
            get
            {
                return key;
            }
        }

        public TaskLooper(string key, Action action)
        {
            this.key = key;
            this.action = action;
            taskCancel = new CancellationTokenSource();
        }

        public TaskLooper(int loopTime, string key, Action action)
            : this(key, action)
        {
            this.LoopTime = loopTime <= 0 ? LoopTime : loopTime;

        }
        private void doTask()
        {
            Task.Run(() =>
            {
                if (!taskCancel.Token.IsCancellationRequested)
                {
                    action();
                }

            }, taskCancel.Token);
        }

        public void cancelAll()
        {
            taskCancel.Cancel();
        }

        public void execute()
        {
            Task.Run(() =>
               {
                   while (!taskCancel.Token.IsCancellationRequested)
                   {
                       doTask();
                       System.Threading.Thread.Sleep(LoopTime);
                   }

               }, taskCancel.Token);
        }
    }
}
