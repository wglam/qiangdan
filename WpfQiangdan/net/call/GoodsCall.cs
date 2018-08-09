using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WpfQiangdan.bean;

namespace WpfQiangdan.net.call
{
    class GoodsCall : ICallback<AllGood>
    {
        private System.Windows.Controls.ListView view;

        public GoodsCall(System.Windows.Controls.ListView view)
        {
            this.view = view;
        }

        public void success(int code, AllGood data)
        {
            if (code == 0)
            {
                ObservableCollection<GoodItem> sell = new ObservableCollection<GoodItem>(data.sell);
                foreach (GoodItem item in data.presell)
                { sell.Add(item);
                }
                this.view.Dispatcher.Invoke(new Action(delegate {
                    this.view.ItemsSource = sell; 
                
                }));
            }

        }

        public void fail(int code, string error)
        {

        }
    }
}
