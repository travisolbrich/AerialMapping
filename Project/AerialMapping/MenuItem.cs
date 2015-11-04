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
        public MenuItem()
        {
            this.Items = new ObservableCollection<MenuItem>();
        }

        public MenuItem(string title, string filePath)
        {
            this.FilePath = filePath;
            this.Title = title;
            this.Checked = false;
            this.Items = new ObservableCollection<MenuItem>();
        }


        public MenuItem(string title, string filePath, bool check)
        {
            FilePath = filePath;
            Title = title;
            Checked = check;
            Items = new ObservableCollection<MenuItem>();
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
