using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace PerformanceTestLoader
{
    public static class AsyncWebClientRequestHelper
    {
        private const string REQUEST_ENCODING = "utf-8";
        private const string RESPONSE_ENCODING = "utf-8";
        private const string USER_AGENT = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/32.0.1700.72 Safari/537.36";
        private const string CONTENT_TYPE = "application/x-www-form-urlencoded";

        private static DateTime _firstRequestTime = DateTime.MinValue;
        private static DateTime _lastRequestTime = DateTime.MinValue;
        private static int _requestCounts = 0;

        private static readonly object _locker = new object();

        public static void AsyncSendRequest(string url, string postData)
        {
            var webClient = new WebClient();
            webClient.Encoding = Encoding.GetEncoding(REQUEST_ENCODING);
            webClient.Headers.Add("Content-Type", CONTENT_TYPE);
            webClient.Headers.Add("User-Agent", USER_AGENT);
            //webClient.Headers.Add("Content-Length", postData.Length.ToString());

            byte[] sendData = Encoding.GetEncoding(REQUEST_ENCODING).GetBytes(postData);
            webClient.UploadDataAsync(new Uri(url), "POST", sendData);

            webClient.UploadDataCompleted += (sender, args) =>
                                                 {
                                                     
                                                     if (_firstRequestTime == DateTime.MinValue)
                                                         _firstRequestTime = DateTime.Now;
                                                     _lastRequestTime = DateTime.Now;
                                                     _requestCounts++;
                                                     Console.WriteLine(CaculateSpentTime());
                                                 };
        }

        public static string CaculateSpentTime()
        {
            return string.Format("Total request counts:{0}, total time spent:{1}",_requestCounts, (_lastRequestTime - _firstRequestTime).ToString());
        }

        
    }
}
