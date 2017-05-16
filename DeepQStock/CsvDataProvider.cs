using DeepQStock.Enums;
using LINQtoCSV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock
{
    public class CsvDataProvider : IDataProvider
    {
        #region << Private Properties >> 

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the size of the batch.
        /// </summary>
        public int BatchSize { get; set; }

        /// <summary>
        /// Get or sets the periods
        /// </summary>
        private IEnumerable<Period> Data { get; set; }

        /// <summary>
        /// Gets or sets the period loaded.
        /// </summary>
        public int PeriodLoaded { get; set; }

        #endregion

        #region << Constructor >>

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvDataProvider"/> class.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        public CsvDataProvider(string filePath, int batchSize,  DateTime? startDate = null)
        {
            StartDate = startDate;
            FilePath = filePath;
            GetAllDataFromCsv(startDate);
        }

        #endregion


        #region << IDataProvider Members >> 


        /// <summary>
        /// Get the next the period.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="size"></param>
        /// <returns></returns>
        public IEnumerable<Period> NextDays()
        {            
            var result = Data.Skip(PeriodLoaded).Take(BatchSize);
            PeriodLoaded += BatchSize;

            return result;
        }

        #endregion

        #region << Private Methods >>

        /// <summary>
        /// Get all Period
        /// </summary>
        /// <returns></returns>
        private void GetAllDataFromCsv(DateTime? startDate)
        {
            CsvFileDescription descriptor = new CsvFileDescription
            {
                SeparatorChar = ',',
                FirstLineHasColumnNames = true,
                FileCultureInfo = System.Globalization.CultureInfo.InvariantCulture
            };

            CsvContext ctx = new CsvContext();

            Data = ctx.Read<Period>(FilePath, descriptor);
            
            if (startDate.HasValue)
            {
                Data = Data.Where(d => d.Date >= startDate.Value);
            }            

            Data.OrderBy(d => d.Date);
        }

        #endregion

    }
}
