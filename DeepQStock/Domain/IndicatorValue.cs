using DeepQStock.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock.Domain
{
    public class IndicatorValue : BaseModel
    {
        #region << Public Properties >>

        public string Name { get; set; }

        public string ValuesString { get; set; }

        [JsonIgnore]
        public Period Period { get; set; }

        [NotMapped]
        public IEnumerable<double> Values
        {
            get
            {
                return Array.ConvertAll(ValuesString.Split(';'), Double.Parse);
            }
            set
            {

                ValuesString = String.Join(";", value.Select(p => p.ToString()).ToArray());
            }
        }

        #endregion

        #region << Constructor >>

        public IndicatorValue() { }

        public IndicatorValue(string name, IEnumerable<double> values)
        {
            Name = name;
            Values = values;
        }

        #endregion
    }
}
