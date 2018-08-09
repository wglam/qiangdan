using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfQiangdan.bean;
using WpfQiangdan.db;
using WpfQiangdan.net;
using WpfQiangdan.work;

namespace WpfQiangdan
{
    /// <summary>
    /// UserToken.xaml 的交互逻辑
    /// </summary>
    public partial class UserToken : UserControl
    {
        public UserToken()
        {
            InitializeComponent();
        }



        private void importUsers_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "Excel Files (*.txt)|*.txt"
            };
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                var filename = openFileDialog.FileName;
                Read(filename);
            }
        }

        private void Read(string path)
        {
            StreamReader sr = new StreamReader(path, Encoding.Default);
            String line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] array = line.Replace("----", ",").Split(',');
                if (array.Length == 4)
                {
                    User user = new User(array[0], array[1], array[2], array[3]);
                    DbValue.add(user);
                }
                else
                {
                    MessageBox.Show("账号格式错误!");
                    break;
                }

            }
            userListView.ItemsSource = DbValue.getUsersObservable();
        }


        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox ck = sender as CheckBox;
            string account = ck.Tag as string;
            DbValue.checkUser(account, ck.IsChecked == true);
        }

        private void chooseAll_Click(object sender, RoutedEventArgs e)
        {
            DbValue.checkAllUser();
        }

        private void chooseReverse_Click(object sender, RoutedEventArgs e)
        {
            DbValue.checkReverseUser();
        }

        private void getToken_Click(object sender, RoutedEventArgs e)
        {
            foreach (User user in DbValue.getCheckedUser(false))
            {
                NetWork.loginUser(user);
            }
        }

        private void startWork_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(DbValue.gbid))
            {
                MessageBox.Show("未选择抢购商品，请在获取商品双击选择抢购商品!");
                return;
            }
            ICollection<User> users = DbValue.getCheckedUser(true);

            if (users.Count <= 0)
            {
                MessageBox.Show("未选中有效账号!");
                return;
            }

            QiangdanWork.start(users);
        }

        private void stopWork_Click(object sender, RoutedEventArgs e)
        {
            QiangdanWork.stop(DbValue.getCheckedUser(true));
        }

        private void queryWork_Click(object sender, RoutedEventArgs e)
        {
            NetWork.getPersonalDatas(DbValue.getCheckedUser(true));
        }
    }
}
