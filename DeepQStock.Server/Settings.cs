using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock.Server
{
    public static class Settings
    {
        /// <summary>
        /// Get the redis connection string
        /// </summary>
        public static string RedisConnectionString
        {
            get
            {
                return ConfigurationManager.AppSettings["RedisConnectionString"];
            }
        }

        /// <summary>
        /// Path of the directory where are store it the csv companies files
        /// </summary>
        public static string CsvDataDirectory
        {
            get {
                return ConfigurationManager.AppSettings["CsvDataDirectory"];
            }
        }
    }
}
