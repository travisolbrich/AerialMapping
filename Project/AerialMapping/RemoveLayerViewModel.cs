using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AerialMapping
{
    public class RemoveLayerViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<MenuItem> Items { get; set; }

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
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
