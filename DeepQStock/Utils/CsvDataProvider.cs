using DeepQStock.Domain;
using LINQtoCSV;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeepQStock.Utils
{
    public class CsvDataProvider : IDataProvider
    {
        #region << Private Properties >> 

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// get or set the end date
        /// </summary>
        public DateTime? EndDate { get; set; }

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
        public CsvDataProvider(string filePath, int batchSize)
        {         
            BatchSize = batchSize;
            FilePath = filePath;
            Seek();
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


        /// <summary>
        /// Return the min date in the file
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Period> GetAll()
        {
            return Data; 
        }


        /// <summary>
        /// Initialize the provider
        /// </summary>
        /// <returns></returns>
        public void Seek(DateTime? startDate = null, DateTime? endDate = null)
        {
            StartDate = startDate;
            EndDate = endDate;
            PeriodLoaded = 0;     

            CsvFileDescription descriptor = new CsvFileDescription
            {
                SeparatorChar = ',',
                FirstLineHasColumnNames = true,
                FileCultureInfo = System.Globalization.CultureInfo.InvariantCulture
            };

            CsvContext ctx = new CsvContext();

            Data = ctx.Read<Period>(FilePath, descriptor);

            if (StartDate.HasValue)
            {
                Data = Data.Where(d => d.Date >= StartDate.Value);
            }

            if (EndDate.HasValue)
            {
                Data = Data.Where(d => d.Date < EndDate);
            }
          
            Data = Data.OrderBy(d => d.Date);
        }

        #endregion

    }
}
