﻿//-----------------------------------------------------------------------
// <copyright file="Dataset.cs" company="CSCE 482: Aerial Mapping">
//     Copyright (c) CSCE 482 Aerial Mapping Design Team
// </copyright>
//-----------------------------------------------------------------------

namespace AerialMapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This class represents the data for a map layer
    /// </summary>
    public class Dataset
    {
        /// <summary>
        /// Gets or sets the location for the Dataset.
        /// </summary>
        public string Location 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets or sets the date and time for the Dataset. 
        /// </summary>
        public DateTime Time 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets or sets the path of the image. 
        /// </summary>
        public string FilePath 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets or sets the path of the tree canopy file. 
        /// </summary>
        public string TreeCanopyFilePath
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to zoom to this image.
        /// </summary>
        public bool ZoomTo
        {
            get; 
            set;
        }
    }
}
