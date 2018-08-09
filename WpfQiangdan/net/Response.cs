using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfQiangdan.net
{

    class Response<T>
    {
        public int code { get; set; }
        public T data { get; set; }
        public string message { get; set; }
        public long timestamp { get; set; }

        public static Response<T> error(int code, string message)
        {
            Response<T> response = new Response<T>();
            return response;
        }
    }

}
