//-----------------------------------------------------------------------
// <copyright file="MenuItem.cs" company="CSCE 482: Aerial Mapping">
//     Copyright (c) CSCE 482 Aerial Mapping Design Team
// </copyright>
//-----------------------------------------------------------------------

namespace AerialMapping
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This class is used for the elements in the Layers TreeView on the ride side pop-out menu.
    /// </summary>
    public class MenuItem : INotifyPropertyChanged
    {
        private bool isChecked;

        /// <summary>
        /// Initializes a new instance of the MenuItem class.
        /// </summary>
        public MenuItem()
        {
            this.Items = new ObservableCollection<MenuItem>();
        }

        /// <summary>
        /// Initializes a new instance of the MenuItem class.
        /// </summary>
        /// <param name="title">The title of the item</param>
        /// <param name="filePath">The filepath to the item</param>
        /// <param name="treeFilePath">The filepath to the tree canopy image</param>
        public MenuItem(string title, string filePath, string treeFilePath)
        {
            this.FilePath = filePath;
            this.TreeCanopyFilePath = treeFilePath;
            this.Title = title;
            this.Checked = false;
            this.Items = new ObservableCollection<MenuItem>();
        }

        /// <summary>
        /// Initializes a new instance of the MenuItem class.
        /// </summary>
        /// <param name="title">The title of the item</param>
        /// <param name="filePath">The filepath to the item</param>
        /// <param name="treeFilePath">The filepath to the tree canopy image</param>
        /// <param name="check">Whether the check is checked</param>
        public MenuItem(string title, string filePath, string treeFilePath, bool check)
        {
            this.FilePath = filePath;
            this.TreeCanopyFilePath = treeFilePath;
            this.Title = title;
            this.Checked = check;
            this.Items = new ObservableCollection<MenuItem>();
        }

        /// <summary>
        /// INotifyPropertyChanged event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the file path to the related image.
        /// </summary>
        public string FilePath 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets or sets the file path to the tree canopy image.
        /// </summary>
        public string TreeCanopyFilePath
        {
            get; 
            set;
        }

        /// <summary>
        /// Gets or sets the title shown on the treeviews. Will be either
        /// the location or datetime.
        /// </summary>
        public string Title 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets or sets a value indicating whether this is selected
        /// in the treeview.
        /// </summary>
        public bool Checked 
        {
            get
            {
                return this.isChecked;
            }

            set
            {
                this.isChecked = value;
                this.NotifiyPropertyChanged("Checked");
            }
        }

        /// <summary>
        /// Gets or sets the Items that make up the children nodes.
        /// </summary>
        public ObservableCollection<MenuItem> Items 
        { 
            get; 
            set;
        }

        /// <summary>
        /// A method to compare if two MenuItems are the same.
        /// </summary>
        /// <param name="otherItem">The other MenuItem to compare with.</param>
        /// <returns>Bool indicating if they are the same.</returns>
        public bool Equals(MenuItem otherItem)
        {
            if (this.Title == otherItem.Title && this.FilePath == otherItem.FilePath)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Converts the title string into a date time.
        /// </summary>
        /// <returns>The date time</returns>
        public DateTime TimeAsDateTime()
        {
            return Convert.ToDateTime(this.Title);
        }

        /// <summary>
        /// INotifyPropertyChanged method for updating the UI.
        /// </summary>
        /// <param name="property">Name of the UI element to update.</param>
        private void NotifiyPropertyChanged(string property)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}