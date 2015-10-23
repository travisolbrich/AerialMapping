using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AerialMapping
{
    // This class represents the data for a map layer.
    public class Dataset
    {
        public string Location { get; set; }

        public DateTime Time { get; set; }
        //public string Time { get; set; }

        public string FilePath { get; set; }
    }
}
