using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AerialMapping
{
    // This class is used for the elements in the Layers Treeview on
    // the right side popout menu.
    public class MenuItem
    {
        public MenuItem()
        {
            this.Items = new ObservableCollection<MenuItem>();
        }

        public string Title { get; set; }

        public bool Checked { get; set; }

        public ObservableCollection<MenuItem> Items { get; set; }
    }
}
