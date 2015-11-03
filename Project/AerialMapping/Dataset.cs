using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AerialMapping
{
    /// <summary>
    /// This class represents the data for a map layer
    /// </summary>
    public class Dataset
    {
        /// <summary>
        /// This function specifies the location for the Dataset.
        /// </summary>
        public string Location 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// This function specifies the date and time for the Dataset. 
        /// </summary>
        public DateTime Time 
        { 
            get; 
            set; 
        }
        //public string Time { get; set; }

        /// <summary>
        /// This function specifies the path of the Dataset file. 
        /// </summary>
        public string FilePath 
        { 
            get; 
            set; 
        }
    }
}
