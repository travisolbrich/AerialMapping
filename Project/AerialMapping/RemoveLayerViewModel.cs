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
        public ObservableCollection<MenuItem> Items 
        { 
            get; 
            set; 
        }

        //public DelegateCommand OnCheckCommand { get; set; }

        public RemoveLayerViewModel(ObservableCollection<MenuItem> TreeViewItems)
        {
            Items = TreeViewItems;

            //OnCheckCommand = new DelegateCommand(OnCheck);
        }

        public void OnCheck()
        {
            Debug.WriteLine("test");
            foreach (MenuItem parent in Items)
            {
                foreach (MenuItem child in parent.Items)
                {
                    child.Checked = true;
                }
            }

            NotifiyPropertyChanged("Items");
        }

        void NotifiyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
