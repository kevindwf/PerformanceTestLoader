using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace PerformanceTestLoader
{
    internal class Program
    {

        private const string STR_SQL_QUERY_ACTIVITYIDS =
            @"SELECT DISTINCT u.CourseUnit_id FROM SchoolPlatform..course c
INNER JOIN SchoolPlatform..courselevel l ON l.Course_id=c.Course_id AND l.IsActive=1
INNER JOIN SchoolPlatform..CourseUnit u ON u.CourseLevel_id=l.CourseLevel_id AND u.IsActive=1
WHERE c.CourseTypeCode='ge' AND CourseName LIKE '%13%'";
        


        private static void Main(string[] args)
        {
            System.Net.ServicePointManager.DefaultConnectionLimit = 512;

            var activityIds = GetActivityIds();
            int threadCounts;
            int.TryParse(ConfigurationManager.AppSettings["ThreadCounts"], out threadCounts);



            SendRequestManager.SendMultiThreadRequest(activityIds, threadCounts);
            //SendRequestManager.SendAsyncRequest(activityIds);

            
            Console.ReadLine();
        }


        private static List<int> GetActivityIds()
        {
            return SqlHelper.QuerySingleColumnFromDB<int>(STR_SQL_QUERY_ACTIVITYIDS).ToList();
        }
   
    }
}

