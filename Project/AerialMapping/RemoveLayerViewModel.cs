//-----------------------------------------------------------------------
// <copyright file="RemoveLayerModel.cs" company="CSCE 482: Aerial Mapping">
//     Copyright (c) CSCE 482 Aerial Mapping Design Team
// </copyright>
//-----------------------------------------------------------------------

namespace AerialMapping
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Input;

    public class RemoveLayerViewModel : INotifyPropertyChanged
    {
        public RemoveLayerViewModel(ObservableCollection<MenuItem> treeViewItems)
        {
            this.Items = treeViewItems;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<MenuItem> Items
        {
            get;
            set;
        }

        public void OnCheck()
        {
            Debug.WriteLine("test");
            foreach (MenuItem parent in this.Items)
            {
                foreach (MenuItem child in parent.Items)
                {
                    child.Checked = true;
                }
            }

            this.NotifiyPropertyChanged("Items");
        }

        private void NotifiyPropertyChanged(string property)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
