using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;

namespace PerformanceTestLoader
{
    public class SendRequestManager
    {

        private static string QUERY_URL = ConfigurationManager.AppSettings["RequestUrl"];
        private const string POST_DATA = "q=course_content!{0}";

        public delegate void SendRequestDelegate(int activityId);

        public static void SendMultiThreadRequest(List<int> activityIds, int threadCounts)
        {
            SendMultiThreadRequest(activityIds, threadCounts, SendHttpWebRequest);
        }

        public static void SendAsyncRequest(List<int> acitivityIds)
        {
            SendMultiThreadRequest(acitivityIds, 1, SendAsyncWebClientRequest);
        }

        private static void SendMultiThreadRequest(List<int> acitivityIds, int threadCounts, SendRequestDelegate sendRequestDelegate)
        {
            var startTime = DateTime.Now;

            var eventWaitHandles = new AutoResetEvent[threadCounts];

            int perThreadItems = acitivityIds.Count / threadCounts == 0
                                    ? acitivityIds.Count / threadCounts
                                    : acitivityIds.Count / threadCounts + 1;
            for (int i = 0; i < threadCounts; i++)
            {
                var items = acitivityIds.GetRange(perThreadItems * i,
                                                  i == threadCounts - 1
                                                      ? acitivityIds.Count - perThreadItems * i
                                                      : perThreadItems);
                
                eventWaitHandles[i] = new AutoResetEvent(false);
                Console.WriteLine("thread count:{0}, i:{1}", threadCounts, i);

                //why need to define another variable i1?
                int i1 = i;
                var thread = new Thread(() => SendRequestBySingleThread(items, sendRequestDelegate, eventWaitHandles[i1]))
                                 {Name = i.ToString()};

                thread.Start();
            }

            WaitHandle.WaitAll(eventWaitHandles);
            var endTime = DateTime.Now;
            Console.WriteLine("done!");
            Console.WriteLine("total time spent:{0}",  endTime - startTime);
        }

        private static void SendRequestBySingleThread(List<int> activityIds, SendRequestDelegate sendRequest, AutoResetEvent eventWaitHandle)
        {
            var startTime = DateTime.Now;
            
            foreach (var activityId in activityIds)
            {
                Console.WriteLine(string.Format("Thread:{0}, Id:{1}", Thread.CurrentThread.Name, activityId.ToString()));
                sendRequest(activityId);
                //Thread.Sleep(1000);
            }

            eventWaitHandle.Set();

            var endTime = DateTime.Now;
            Console.WriteLine("thread {0} request count:{1}, time spent:{2}", Thread.CurrentThread.Name, activityIds.Count, endTime - startTime);
            
        }

        private static void SendAsyncWebClientRequest(int activityId)
        {
            AsyncWebClientRequestHelper.AsyncSendRequest(QUERY_URL, string.Format(POST_DATA, activityId));

        }

        private static void SendHttpWebRequest(int activityId)
        {
            SimpleHttpWebRequestHelper.PostDataToUrl(QUERY_URL, string.Format(POST_DATA, activityId));

        }

    }
}
