using Encog.Util.Arrayutil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock.Utils
{
    public static class Normalizers
    {

        public const int Max_Range = 100000;

        /// <summary>
        /// Gets or sets the capital normalized.
        /// </summary>        
        public static NormalizedField Capital = new NormalizedField(NormalizationAction.Normalize, "Capital", Max_Range, 0.0, 0.0, 1.0);

        /// <summary>
        /// Gets or sets the price normalized.
        /// See http://www.investopedia.com/stock-analysis/2012/7-of-the-highest-stock-prices-in-history-brk-b-aapl-seb-nvr1114.aspx
        /// </summary>        
        public static NormalizedField Price = new NormalizedField(NormalizationAction.Normalize, "Price", 5000, 0.0, 0.0, 1.0);

        /// <summary>
        /// Gets or sets the price normalized.
        /// </summary>        
        public static NormalizedField Position = new NormalizedField(NormalizationAction.Normalize, "Position", Max_Range, 0.0, 0.0, 1.0);

        /// <summary>
        /// Gets or sets the volumen.
        /// </summary>
        public static NormalizedField Volume = new NormalizedField(NormalizationAction.Normalize, "Volume", int.MaxValue, 0.0, 0.0, 1.0);

        /// <summary>
        /// Gets or sets the volumen.
        /// </summary>
        public static NormalizedField Reward = new NormalizedField(NormalizationAction.Normalize, "Reward", 10, 0.0, 0.0, 1.0);

        /// <summary>
        /// The dmi
        /// </summary>
        public static NormalizedField DMI = new NormalizedField(NormalizationAction.Normalize, "DMI", 100, 0.0, 0.0, 1.0);

        /// <summary>
        /// The RSI
        /// </summary>
        public static NormalizedField RSI = new NormalizedField(NormalizationAction.Normalize, "RSI", 100, 0.0, 0.0, 1.0);
    }
}
