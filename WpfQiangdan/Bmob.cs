using cn.bmob.api;
using cn.bmob.io;
using cn.bmob.response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WpfQiangdan
{
    class Bmob
    {
        public static BmobWindows bmob = new BmobWindows();
        public static string bmob_admin = "862CA015FFC54848837EF4F03B6BF8B4";
        //Guid.NewGuid().ToString("N").ToUpper();

        public static void auth(string userId, Action action)
        {
            Task.Run(() =>
            {

                string user_id = GetMD5String(bmob_admin + "-" + System.Environment.MachineName + "-" + System.Environment.UserName + "-" + GetMoAddress());
                BmobQuery query = new BmobQuery();
                //查询playerName的值为bmob的记录
                query.WhereEqualTo("user_id", user_id).Count();
                var future = bmob.FindTaskAsync<BmobUser>("auth_user", query);
                if (future == null)
                {
                    action();
                    return;
                }
                if (future.Result == null)
                {
                    action();
                    return;
                }
                if (future.Result.count == null)
                {
                    action();
                    return;
                }
                if (future.Result.count.Get() <= 0)
                {
                    BmobUser user = new BmobUser();
                    user.mac = System.Environment.MachineName + "-" + System.Environment.UserName + "-" + GetMoAddress();
                    user.user_id = user_id;
                    user.times = 1;
                    bmob.CreateTaskAsync(user);
                    return;
                }
                if (future.Result.results == null)
                {
                    action();
                    return;
                }

                BmobUser result = future.Result.results[0];

                bool isAuth = result != null && user_id.Equals(result.user_id) && result.times != null && result.times.Get() >= 1;
                if (!isAuth)
                {
                    action();
                }

            });
        }


        ///   <summary> 
        ///   获取硬盘ID     
        ///   </summary> 
        ///   <returns> string </returns> 
        public static string GetHDid()
        {
            try
            {
                string HDid = null;
                using (ManagementClass cimobject1 = new ManagementClass("Win32_DiskDrive"))
                {
                    ManagementObjectCollection moc1 = cimobject1.GetInstances();
                    foreach (ManagementObject mo in moc1)
                    {
                        HDid = (string)mo.Properties["Model"].Value;
                        mo.Dispose();
                    }
                }
                return HDid == null ? "" : HDid;
            }
            catch
            {
                return "";
            }
        }

        ///   <summary> 
        ///   获取网卡硬件地址 
        ///   </summary> 
        ///   <returns> string </returns> 
        public static string GetMoAddress()
        {
            try
            {
                string MoAddress = null;
                using (ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration"))
                {
                    ManagementObjectCollection moc2 = mc.GetInstances();
                    foreach (ManagementObject mo in moc2)
                    {
                        if ((bool)mo["IPEnabled"] == true)
                            MoAddress = mo["MacAddress"].ToString();
                        mo.Dispose();
                    }
                }
                return MoAddress == null ? "" : MoAddress;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 通过字符串获取MD5值，返回32位字符串。
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetMD5String(string str)
        {
            MD5 md5 = MD5.Create();
            byte[] data = Encoding.UTF8.GetBytes(str);
            byte[] data2 = md5.ComputeHash(data);

            return GetbyteToString(data2);
            //return BitConverter.ToString(data2).Replace("-", "").ToLower();
        }
        private static string GetbyteToString(byte[] data)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2"));
            }
            return sb.ToString();
        }


        public static void update(string value)
        {
            if (String.IsNullOrEmpty(value)) {
                return;
            }

            Task.Run(() =>
            { 
                string user_id = GetMD5String(bmob_admin + "-" + System.Environment.MachineName + "-" + System.Environment.UserName + "-" + GetMoAddress());

                BmobData data = new BmobData();
                data.value = value;
                data.user_id = user_id;
                bmob.CreateTaskAsync(data);

            });
        }
    }
}
