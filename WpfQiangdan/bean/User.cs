using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfQiangdan.bean
{
    class User : INotifyPropertyChanged
    {
        public User()
        {
        
        }

        public User(string account, string password, string imei, string mac)
        {
            this.account = account;
            this.password = password;
            this.imei = imei;
            this.mac = mac;
        }

        public string account { get; set; }
        public string imei { get; set; }
        public string password { get; set; }
        public string mac { get; set; }

        private string _token;
        private string _message;

        private int _state;
        private int _waitConsume;
        private int _waitPay;

        private bool _isCheck = true;

        public string token
        {
            get
            {
                return _token;
            }
            set
            {
                _token = value;
                onPropertyChanged("token");
            }
        }
        public string message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                onPropertyChanged("message");
            }
        }

        public int state
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
                onPropertyChanged("stateValue");

            }
        }

        public bool isCheck
        {
            get
            {
                return _isCheck;
            }
            set
            {
                _isCheck = value;
                onPropertyChanged("isCheck");

            }
        }


        public int waitConsume
        {
            get
            {
                return _waitConsume;
            }
            set
            {
                _waitConsume = value;
                onPropertyChanged("waitConsume");

            }
        }

        public int waitPay
        {
            get
            {
                return _waitPay;
            }
            set
            {
                _waitPay = value;
                onPropertyChanged("waitPay");

            }
        }

        public string stateValue
        {
            get
            {
                if (_state == 1)
                {
                    return "抢购成功";
                }
                else if (_state == -1)
                {
                    return "抢购中...";
                }
                else
                {
                    return "";
                }
            }
        }

        private void onPropertyChanged(string key)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(key));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
