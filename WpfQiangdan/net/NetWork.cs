
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


        public static void groupbuyinfo(string gbId, ICallback<GoodItem> callback)
        {
            IDictionary<string, string> param = commonParam();
            param.Add("gbId", gbId);
            get(host + "/api/v2/groupbuy/info", param, callback);
        }

        private static GoodItem groupbuyinfo(string gbId)
        {
            IDictionary<string, string> param = commonParam();
            param.Add("gbId", gbId);
            HttpWebResponse response = Http.post(host + "/api/v2/groupbuy/info", Sign.sign(param));
            Response<GoodItem> data = jsonHttpWebResponse<GoodItem>(response);
            return data.data != null ? data.data : groupbuyinfo(gbId);
        }

        public static void homepage(ICallback<List<GoodItem>> callback)
        {
            Task.Run(() =>
            {
                IDictionary<string, string> param = commonParam();
                param.Add("mallNo", "0602B001");
                HttpWebResponse response = Http.post(host + "/api/v3/homepage", param);

                Response<Home> data = jsonHttpWebResponse<Home>(response);
                if (data.code == 0)
                {
                    Home h = data.data;

                    List<GoodItem> ret = new List<GoodItem>();
                    if (h.onsalelist != null)
                    {
                        string checKey = "mixc://app/groupbuyDetail?gbId=";
                        foreach (Banner item in h.banners)
                        {
                            if (!String.IsNullOrWhiteSpace(item.url) && item.url.StartsWith(checKey))
                            {
                                ret.Add(groupbuyinfo(item.url.Replace(checKey, "")));
                            }

                        }
                    }
                    if (h.onsalelist != null)
                    {
                        foreach (GoodItem item in h.onsalelist)
                        {
                            if (!String.IsNullOrWhiteSpace(item.gbid))
                            {
                                ret.Add(groupbuyinfo(item.gbid));
                            }
                        }
                    }
                    callback.success(0, ret);
                }
                else
                {
                    callback.fail(-1, data.message);
                }
            });

        }

        public static void getPersonalDatas(ICollection<User> users)
        {

            foreach (User item in users)
            {
                Task.Run(() => { getPersonalData(item); });
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
                    if (data.code == 401)
                    {
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

        public static Response<object> generateOrderBy(User user)
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
                return data;
            }
            catch (Exception e)
            {
                return Response<object>.error(-1, e.Message);
            }
        }


        public static void generateOrderPngs(string rootPath, ICollection<User> users)
        {
            Task.Run(() =>
             {
                 foreach (User user in users)
                 {
                     generateOrderPng(rootPath, user);
                 }
             });
        }
        public static void generateOrderPng(string rootPath, User user)
        {
            Task.Run(() =>
            {
                IDictionary<string, string> param = commonParam();

                param.Add("imei", user.imei);
                param.Add("mac", user.mac);
                param.Add("token", user.token);
                param.Add("mallNo", "0602B001");

                param.Add("pageNum", "1");
                param.Add("orderType", "2");

                HttpWebResponse response = Http.post(host + "api/v2/order/list", Sign.sign(param));
                Response<Pager<Order>> data = jsonHttpWebResponse<Pager<Order>>(response);
                if (data.code == 0)
                {
                    List<Order> orderList = data.data.list;
                    if (orderList != null && orderList.Count >= 1)
                    {
                        foreach (Order item in orderList)
                        {
                            if (!String.IsNullOrWhiteSpace(item.orderNo)) {
                                getOrderDetail(rootPath, user, item.orderNo);
                            }
                        }
                    }

                }
                else if (data.code != 401)
                {
                    generateOrderPng(rootPath, user);
                }

            });
        }

        private static void getOrderDetail(string rootPath, User user, string orderNo)
        {
            IDictionary<string, string> param = commonParam();

            param.Add("imei", user.imei);
            param.Add("mac", user.mac);
            param.Add("token", user.token);
            param.Add("mallNo", "0602B001");

            param.Add("orderNo", orderNo);
            HttpWebResponse response = Http.post(host + "api/v2/order/detail", Sign.sign(param));
            Response<Order> data = jsonHttpWebResponse<Order>(response);

            if (data.code == 0)
            {
                string path = rootPath + "/" + user.account + "-" + data.data.title + "-" + data.data.consumeInfo.consumeCode + ".jpg";
                Util.base64ToPng(path, data.data.consumeInfo.consumePic);
            }
            else if (data.code != 401)
            {
                getOrderDetail(rootPath, user, orderNo);
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

        public static long currentTimeStamp()
        {
            long timeStamp = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
            return timeStamp + DbValue.dtime;
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
                     setCurrentTimeStamp(data.timestamp);
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
