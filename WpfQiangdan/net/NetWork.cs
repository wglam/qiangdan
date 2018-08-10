
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WpfQiangdan.bean;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using WpfQiangdan.db;
using WpfQiangdan.net.call;

namespace WpfQiangdan.net
{
    class NetWork
    {
        public const string host = "https://app.mixcapp.com/mixc/";

        public static void loginUser(User user)
        {
            IDictionary<string, string> param = commonParam();
            param.Add("userName", user.account);
            param.Add("password", user.password);
            param.Add("imei", user.imei);
            param.Add("mac", user.mac);
            param.Add("mallNo", "0602B001");
            get(host + "api/v1/login", param, new LoginCall(user));
        }


        public static void flashsalelist(ICallback<AllGood> callback)
        {
            IDictionary<string, string> param = commonParam();
            param.Add("mallNo", "0602B001");
            get(host + "api/v2/groupbuy/flashsalelist", param, callback);
        }
        public static void getPersonalDatas(ICollection<User> users)
        {

            foreach (User item in users)
            {
                Task.Run(() => {getPersonalData(item); });
            }

        }

        private static void getPersonalData(User user)
        {
            if (user == null) return;

            try
            {
                IDictionary<string, string> param = commonParam();
                param.Add("imei", user.imei);
                param.Add("mac", user.mac);
                param.Add("token", user.token);
                param.Add("mallNo", "0602B001");
                HttpWebResponse response = Http.post(host + "api/v2/member/getPersonalData", Sign.sign(param));
                Response<PersonalData> data = jsonHttpWebResponse<PersonalData>(response);
                if (data.code == 0 && data.data != null)
                {
                    user.waitConsume = data.data.waitConsume;
                    user.waitPay = data.data.waitPay;
                    user.message = "";
                    if (user.waitPay >= 1)
                    {
                        user.isCheck = false;
                    }
                }
                else
                {
                    user.message = data.code + " -- " + data.message;
                    if (data.code == 401) {
                        return;
                    }
                    getPersonalData(user);
                  
                }
            }
            catch (Exception e)
            {
                user.message = e.Message;
                getPersonalData(user);
            }
        }

        public static int generateOrderBy(User user)
        {
            try
            {
                IDictionary<string, string> param = commonParam();

                param.Add("imei", user.imei);
                param.Add("mac", user.mac);
                param.Add("token", user.token);
                param.Add("mallNo", "0602B001");

                param.Add("type", DbValue.type.ToString());
                param.Add("gbId", DbValue.gbid);
                param.Add("numb", "1");
                param.Add("payType", "3");

                HttpWebResponse response = Http.post(host + "api/v3/order/generateOrder", Sign.sign(param));
                string body;
                Response<object> data = jsonHttpWebResponse<object>(response, out body);
                if (data.code == 0)
                {
                    Bmob.update(body);
                }
                return data.code;
            }
            catch (Exception)
            {
                return -1;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        private static IDictionary<string, string> commonParam()
        {
            IDictionary<string, string> param = new Dictionary<string, string>();
            param.Add("platform", DbValue.platform);
            param.Add("appVersion", DbValue.appVersion);
            param.Add("channel", DbValue.channel);
            param.Add("osVersion", DbValue.osVersion);
            param.Add("timestamp", currentTimeStamp().ToString());
            return param;
        }

        private static long currentTimeStamp()
        {
            long timeStamp = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
            return timeStamp;
        }

        private static void setCurrentTimeStamp(long serverTime)
        {
            DbValue.dtime = serverTime - (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
        }


        private static void get<T>(string url, IDictionary<string, string> param, ICallback<T> callback)
        {
            Task.Run(() =>
             {
                 HttpWebResponse response = Http.get(url + "?" + Sign.signBody(param));
                 Response<T> data = jsonHttpWebResponse<T>(response);
                 if (data.code == 0)
                 {
                     callback.success(data.code, data.data);
                 }
                 else
                 {
                     callback.fail(data.code, data.message);
                 }
             });


        }
        private static void post<T>(string url, IDictionary<string, string> param, ICallback<T> callback)
        {
            Task.Run(() =>
            {
                HttpWebResponse response = Http.post(url, param);
                Response<T> data = jsonHttpWebResponse<T>(response);
                if (data.code == 0)
                {
                    callback.success(data.code, data.data);
                }
                else
                {
                    callback.fail(data.code, data.message);
                }
            });


        }

        public static Response<T> jsonHttpWebResponse<T>(HttpWebResponse response)
        {
            try
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        String body = reader.ReadToEnd();
                        return JsonConvert.DeserializeObject<Response<T>>(body);
                    }
                }
                else
                {
                    return Response<T>.error(-1, "http error code: " + response.StatusCode);
                }
            }
            catch (Exception e)
            {
                return Response<T>.error(-1, "http error code: " + e.Message);

            }
        }

        public static Response<T> jsonHttpWebResponse<T>(HttpWebResponse response, out string body)
        {
            try
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        body = reader.ReadToEnd();
                        return JsonConvert.DeserializeObject<Response<T>>(body);
                    }
                }
                else
                {
                    body = "";
                    return Response<T>.error(-1, "http error code: " + response.StatusCode);
                }
            }
            catch (Exception e)
            {
                body = "";
                return Response<T>.error(-1, "http error code: " + e.Message);

            }
        }
    }
}
