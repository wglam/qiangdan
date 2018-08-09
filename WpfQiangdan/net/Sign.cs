using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace WpfQiangdan.net
{
    class Sign
    {
        //"3061b746b58d492baaf373cb" + "9e14fe4b";
        public const string token_key = "3061b746b58d492baaf373cb9e14fe4b";

        public static string signBody(IDictionary<string, string> src)
        {
            StringBuilder builder = new StringBuilder();
            src.Add("sign", createSign(src));
            int i = 0;
            foreach (KeyValuePair<string, string> item in src)
            {
                if (i >= 1)
                {
                    builder.Append("&");
                }
                builder.Append(item.Key);
                builder.Append("=");
                builder.Append(item.Value);
                i++;
            }
            return builder.ToString();
        }

        public static IDictionary<string, string> sign(IDictionary<string, string> src)
        {
            src.Add("sign", createSign(src));
            return src;
        }

        private static string createSign(IDictionary<string, string> map)
        {
            StringBuilder builder = new StringBuilder();
            String[] keys = map.Keys.ToArray();
            Array.Sort(keys);
            string ov = "";
            foreach (string item in keys)
            {
                builder.Append(item);
                builder.Append("=");
                map.TryGetValue(item, out ov);
                builder.Append(ov);
                builder.Append("&");
            }
            builder.Append(token_key);
            return md5(builder.ToString());
        }

        private static string md5(string value)
        {

            byte[] result = Encoding.UTF8.GetBytes(value);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            StringBuilder ret = new StringBuilder();

            for (int i = 0; i < output.Length; ++i)
            {
                int var3 = output[i] & 255;
                if (var3 < 16)
                {
                    ret.Append("0");
                }
                ret.Append(String.Format("{0:X}", var3));
            }

            return ret.ToString().ToLower();
        }
    }
}
