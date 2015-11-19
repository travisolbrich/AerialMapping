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
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This class is used for the elements in the Layers TreeView on the ride side pop-out menu.
    /// </summary>
    public class MenuItem : INotifyPropertyChanged
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

        /// <summary>
        /// Converts the title string into a date time.
        /// </summary>
        /// <returns>The date time</returns>
        public DateTime TimeAsDateTime()
        {
            return Convert.ToDateTime(Title);
        }

        /// <summary>
        /// File path to the related image.
        /// </summary>
        public string FilePath 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// The title shown on the treeviews. Will be either
        /// the location or datetime.
        /// </summary>
        public string Title 
        { 
            get; 
            set; 
        }

        private bool _checked;

        /// <summary>
        /// Whether it is checked in the remove window screen.
        /// </summary>
        public bool Checked 
        {
            get
            {
                return _checked;
            }
            set
            {
                _checked = value;
                NotifiyPropertyChanged("Checked");
            }
        }

        /// <summary>
        /// The Items that make up the children nodes.
        /// </summary>
        public ObservableCollection<MenuItem> Items 
        { 
            get; 
            set; 
        }

        public bool Equals(MenuItem otherItem)
        {
            if (Title == otherItem.Title && FilePath == otherItem.FilePath)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// INotifyPropertyChanged event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

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