using System;
using System.Text;
//using Assets.Plugins.Scripts.DownUpdate;
//using Assets.Plugins.Scripts.Util;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.ComTool
{
    class ELog
    {
        public static void Print(string msg)
        {
            Output("【提示】",msg);
        }

        public static void Warring(string msg)
        {
            Output("【警告】",msg);
        }

        public static void Error(string msg)
        {
            Output("【错误】",msg);
            throw new Exception(msg);
        }

        private static void Output(string name,string msg)
        {
            var message = string.Format("[{0}] {1}：{2}", name, DateTime.Now, msg);
            Debug.Log(message);
            Console.WriteLine(message);
        }

        public static void SendM18(string title, string msg)
        {
            SendRtx(new []{ "M18发版弹窗组" },title,msg,true);
        }

        public static void SendRtx(string[] roleLs, string title, string msg, bool isGroup = false)
        {
            int min = Math.Min(msg.Length, 400);
            msg = msg.Substring(0, min);
            msg = msg.Replace(" ", "+");
            msg = msg.Replace("\n", "%0A");
            msg = msg.Replace("#", "%23");
            msg = B64Encode(B64Encode(msg));
            title = B64Encode(B64Encode(title));
            // string rootUrl = "https://oa.aef.com/api/sendRTX.php";
            // string superKey = "YgG6I$GrLu2h$Dn0";
            long now = Now();
            foreach (string role in roleLs)
            {
                //string sign, url;
                //if (isGroup)
                //{
                //    string roleEncode = B64Encode(B64Encode(role));
                //    //sign = Md5Util.CreateMd5(roleEncode + msg + title + superKey + now);
                //    url = rootUrl + "?rtxgroupname=" + roleEncode + "&msg=" + msg + "&time=" + now + "&sign=" + sign + "&title=" + title;
                //}
                //else
                //{
                //    //sign = Md5Util.CreateMd5(role + msg + title + superKey + now);
                //    url = rootUrl + "?receiver=" + role + "&msg=" + msg + "&time=" + now + "&sign=" + sign + "&title=" + title;
                //}
                HttpDeal();
                //WebHttp.HttpGetString(url);
            }
        }


        private static void HttpDeal()
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback +=
            delegate (object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
               System.Security.Cryptography.X509Certificates.X509Chain chain,
               System.Net.Security.SslPolicyErrors sslPolicyErrors)
            {
                return true; // **** Always accept
            };
        }

        public static string B64Encode(string msg)
        {
            Encoding encode = Encoding.UTF8;
            byte[] bytedata = encode.GetBytes(msg);
            return Convert.ToBase64String(bytedata, 0, bytedata.Length);
        }

        public static long Now()
        {
            DateTime start = new DateTime(1970, 1, 1);
            return (DateTime.Now.Ticks - start.Ticks) / 10000000 - 8 * 60 * 60;
        }
    }
}
