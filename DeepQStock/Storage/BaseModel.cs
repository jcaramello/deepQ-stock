using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock.Storage
{
    public class BaseModel
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }
    }
}
