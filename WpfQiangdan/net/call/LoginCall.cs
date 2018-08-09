using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfQiangdan.bean;
using WpfQiangdan.db;

namespace WpfQiangdan.net.call
{
    class LoginCall : ICallback<User>
    {
        private User user;
        public LoginCall(User user)
        {
            this.user = user;
        }
        public void success(int code, User data)
        {
            if (user != null && code == 0 && data != null)
            {
                user.token = data.token;
            }
        
        }

        public void fail(int code, string error)
        {
            if (user != null) user.message = error;
        }


    }
}
