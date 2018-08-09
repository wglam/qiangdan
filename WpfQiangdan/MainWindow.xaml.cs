﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using WpfQiangdan.net.call;

namespace WpfQiangdan
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            listView.MouseDoubleClick += listView_MouseDoubleClick;

        }

        private void listView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            GoodItem item = listView.SelectedItem as GoodItem;
            if (item == null) return;
            string content = item.title + "   [  " + item.gbid + "  ]  ";
            MessageBoxResult result = MessageBox.Show(this, content, item.title, MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK) {
                idLabel.Content = content;
                DbValue.gbid = item.gbid;
                DbValue.type = item.type;
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            NetWork.flashsalelist(new GoodsCall(listView));
        }

    }
}
