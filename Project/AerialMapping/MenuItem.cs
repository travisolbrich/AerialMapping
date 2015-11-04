//-----------------------------------------------------------------------
// <copyright file="MenuItems.cs" company="CSCE 482: Aerial Mapping">
//     Copyright (c) CSCE 482 Aerial Mapping Design Team
// </copyright>
//-----------------------------------------------------------------------

namespace AerialMapping
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This class is used for the elements in the Layers TreeView on the ride side pop-out menu.
    /// </summary>
    public class MenuItem
    {
        /// <summary>
        /// The default constructor for a MenuItem
        /// </summary>
        public MenuItem()
        {
            this.Items = new ObservableCollection<MenuItem>();
        }

        /// <summary>
        /// The overloaded constructor for a MenuItem
        /// </summary>
        /// <param name="title">The title of the item</param>
        /// <param name="filePath">The filepath to the item</param>
        public MenuItem(string title, string filePath)
        {
            this.FilePath = filePath;
            this.Title = title;
            this.Checked = false;
            this.Items = new ObservableCollection<MenuItem>();
        }

        /// <summary>
        /// The overloaded constructor for a MenuItem
        /// </summary>
        /// <param name="title">The title of the item</param>
        /// <param name="filePath">The filepath to the item</param>
        /// <param name="check">Whether the check is checked</param>
        public MenuItem(string title, string filePath, bool check)
        {
            this.FilePath = filePath;
            this.Title = title;
            this.Checked = check;
            this.Items = new ObservableCollection<MenuItem>();
        }

        public string FilePath 
        { 
            get; 
            set; 
        }

        public string Title 
        { 
            get; 
            set; 
        }

        public bool Checked 
        { 
            get; 
            set; 
        }

        public ObservableCollection<MenuItem> Items 
        { 
            get; 
            set; 
        }
    }
}