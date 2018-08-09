using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfQiangdan.net
{
    interface ICallback<T>
    {
        void success(int code, T data);
        void fail(int code, string error);
    }
}
