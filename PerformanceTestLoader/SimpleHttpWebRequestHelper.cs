using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace PerformanceTestLoader
{
    public static class SimpleHttpWebRequestHelper
    {
        private const string REQUEST_ENCODING = "utf-8";
        private const string RESPONSE_ENCODING = "utf-8";
        private const string USER_AGENT = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/32.0.1700.72 Safari/537.36";
        private const string CONTENT_TYPE = "application/x-www-form-urlencoded";

        /// <summary>
        /// Post data到url
        /// </summary>
        /// <param name="data">要post的数据</param>
        /// <param name="url">目标url</param>
        /// <returns>服务器响应</returns>
        public static string PostDataToUrl(string url, string data)
        {
            Encoding encoding = Encoding.GetEncoding(REQUEST_ENCODING);
            byte[] bytesToPost = encoding.GetBytes(data);
            return PostDataToUrl(url,bytesToPost);
        }

        /// <summary>
        /// Post data到url
        /// </summary>
        /// <param name="data">要post的数据</param>
        /// <param name="url">目标url</param>
        /// <returns>服务器响应</returns>
        public static string PostDataToUrl(string url, byte[] data)
        {
            #region 创建httpWebRequest对象

            WebRequest webRequest = WebRequest.Create(url);
            HttpWebRequest httpRequest = webRequest as HttpWebRequest;
            if (httpRequest == null)
            {
                throw new ApplicationException(
                    string.Format("Invalid url string: {0}", url)
                    );
            }

            #endregion

            #region 填充httpWebRequest的基本信息

            httpRequest.UserAgent = USER_AGENT;
            httpRequest.ContentType = CONTENT_TYPE;
            httpRequest.Method = "POST";

            #endregion

            #region 填充要post的内容

            httpRequest.ContentLength = data.Length;
            Stream requestStream = httpRequest.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();

            #endregion

            #region 发送post请求到服务器并读取服务器返回信息

            Stream responseStream;
            try
            {
                responseStream = httpRequest.GetResponse().GetResponseStream();
            }
            catch (Exception e)
            {
                // log error
                Console.WriteLine(
                    string.Format("POST操作发生异常：{0}", e.Message)
                    );
                throw e;
            }

            #endregion

            #region 读取服务器返回信息

            string stringResponse = string.Empty;
            using (StreamReader responseReader =
                new StreamReader(responseStream, Encoding.GetEncoding(RESPONSE_ENCODING)))
            {
                stringResponse = responseReader.ReadToEnd();
            }
            responseStream.Close();

            #endregion

            return stringResponse;
        }
    }
}
