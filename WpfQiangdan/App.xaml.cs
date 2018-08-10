
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace WpfQiangdan
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            this.Startup += new StartupEventHandler(Application_OnStartup);
        }

        private void Application_OnStartup(object sender, StartupEventArgs e)
        {
            Bmob.bmob.initialize("e16c2ef42761397914bb02ef7a61060f", "2813053c853bb63b221d3bdc249e1300");
        }
    }
}
