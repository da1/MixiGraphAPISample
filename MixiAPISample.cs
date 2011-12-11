using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using Codeplex.Data;

namespace MixiAPISample {
    class MixiApiSample {
        public static String CONSUMER_KEY = "";
        public static String CONSUMER_SECRET = "";
        public static String REDIRECT_URI = "";

        public static String Authorization_Code = "";
        public static String access_token = "";
        public static String refresh_token = "";

        public String GetAuthorizationCodeURL() {
            return "https://mixi.jp/connect_authorize.pl?client_id=" + CONSUMER_KEY + "&response_type=code&scope=r_profile%20r_voice";
        }

        public String AuthorizationCode {
            set {
                Authorization_Code = value;
            }
            get {
                return Authorization_Code;
            }
        }

        public String AccessToken {
            set {
                access_token = value;
            }
            get {
                return access_token;
            }
        }

        public String RefreshToken {
            set {
                refresh_token = value;
            }
            get {
                return refresh_token;
            }
        }

        public String auth() {
            Hashtable ht = new Hashtable();
            ht.Add("grant_type", "authorization_code");
            ht.Add("client_id", CONSUMER_KEY);
            ht.Add("client_secret", CONSUMER_SECRET);
            ht.Add("code", Authorization_Code);
            ht.Add("redirect_uri", REDIRECT_URI);
            Byte[] data = toByte(ht);

            return Post(data);
        }

        public String Refresh() {
            Hashtable ht = new Hashtable();
            ht.Add("grant_type", "refresh_token");
            ht.Add("client_id", CONSUMER_KEY);
            ht.Add("client_secret", CONSUMER_SECRET);
            ht.Add("refresh_token", refresh_token);
            Byte[] data = toByte(ht);

            return Post(data);
        }

        public String People() {
            return Get("/people/@me/@self");
        }

        private String Post(Byte[] data) {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://secure.mixi-platform.com/2/token");
            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";
            req.ContentLength = data.Length;

            Stream reqStream = req.GetRequestStream();
            reqStream.Write(data, 0, data.Length);
            reqStream.Close();

            WebResponse res = req.GetResponse();
            Stream resStream = res.GetResponseStream();
            StreamReader reader = new StreamReader(resStream, Encoding.UTF8);
            String json = reader.ReadToEnd();
            reader.Close();
            resStream.Close();

            return json;
        }

        private String Get(String para) {
            String url = "http://api.mixi-platform.com/2" + para + "?oauth_token=" + access_token;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

            WebResponse res = req.GetResponse();
            Stream resStream = res.GetResponseStream();
            StreamReader reader = new StreamReader(resStream, Encoding.UTF8);
            String json = reader.ReadToEnd();
            reader.Close();
            resStream.Close();
            return json;
        }

        private Byte[] toByte(Hashtable ht) {
            String param = "";
            foreach (String key in ht.Keys) {
                param += String.Format("{0}={1}&", key, ht[key]);
            }
            return Encoding.ASCII.GetBytes(param);
        }
    }

    class Program {
        static void Main(string[] args) {
            var sample = new MixiApiSample();
            Console.WriteLine(sample.GetAuthorizationCodeURL());
            Console.WriteLine("input Authorization_Cod:");
            sample.AuthorizationCode = Console.ReadLine();

            String auth = sample.auth();
            printJson(DynamicJson.Parse(auth));
            Console.WriteLine("input access token:");
            sample.AccessToken = Console.ReadLine();

            String people = sample.People();
            printJson(DynamicJson.Parse(people));

            Console.ReadLine();
        }

        public static void printJson(dynamic arrayJson) {
            foreach (KeyValuePair<String, Object> item in arrayJson) {
                if (item.Value is String || item.Value is ValueType) {
                    Console.WriteLine(item.Key + ":" + item.Value);
                } else {
                    printJson((dynamic)item.Value);
                }
            }
        }
    }
}
