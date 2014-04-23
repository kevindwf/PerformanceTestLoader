using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace PerformanceTestLoader
{
    public static class SqlHelper
    {
        private const string STR_CONNECTION = @"server=CNS-ETDBDEVVS1;database=SchoolPlatform;integrated security=SSPI;";

        public static IEnumerable<TResult> QuerySingleColumnFromDB<TResult>(string sql)
        {
            var results = new List<TResult>();

            using (var con = new SqlConnection(STR_CONNECTION))
            {
                con.Open();
                using (var cmd = new SqlCommand(sql, con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            results.Add((TResult)reader[0]);
                        }
                    }
                }
            }

            return results;
        }
    }
}
