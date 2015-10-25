using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace AerialMapping
{
    /// <summary>
    /// Interaction logic for RemoveLayers.xaml
    /// </summary>
    public partial class RemoveLayers : Window
    {
        
        public ObservableCollection<MenuItem> OC { get; set; }
        ObservableCollection<MenuItem> ChildOC { get; set; }
        public RemoveLayers(ObservableCollection<MenuItem> TreeViewItems)
        {
            OC = TreeViewItems;
            //OC = new ObservableCollection<MenuItem>()
            //{
            //    new MenuItem(){Title="Item1", Checked=true,
            //      Items = new ObservableCollection<MenuItem>()
            //      {            
            //        new MenuItem(){Title="SubItem11", Checked=false},
            //        new MenuItem(){Title="SubItem12", Checked=false},
            //        new MenuItem(){Title="SubItem13", Checked=false}
            //      }
            //    },
            //    new MenuItem(){Title="Item2", Checked=true,
            //      Items = new ObservableCollection<MenuItem>()
            //      {
            //        new MenuItem(){Title="SubItem21", Checked=true},            
            //        new MenuItem(){Title="SubItem22", Checked=true},
            //        new MenuItem(){Title="SubItem23", Checked=true}
            //      }},
            //    new MenuItem(){Title="Item3", Checked=true,
            //      Items = new ObservableCollection<MenuItem>()
            //      {
            //        new MenuItem(){Title="SubItem31", Checked=false},
            //        new MenuItem(){Title="SubItem32", Checked=false},            
            //        new MenuItem(){Title="SubItem33", Checked=false}
            //      }},
            //    new MenuItem(){Title="Item4", Checked=true,
            //      Items = new ObservableCollection<MenuItem>()
            //      {
            //        new MenuItem(){Title="SubItem41", Checked=false},
            //        new MenuItem(){Title="SubItem42", Checked=false},
            //        new MenuItem(){Title="SubItem43", Checked=false}
            //      }
            //    }
            //  };
              InitializeComponent();
              this.DataContext = OC;
        }

        public void OnCheck()
        {
            ChildOC = new ObservableCollection<MenuItem>() { };
            foreach (MenuItem item in OC)
            {
                if (item.Checked == true)
                {
                    ChildOC.Add(item);
                    foreach (MenuItem subitem in item.Items)
                    {
                        if (subitem.Checked == true)
                        {
                            ChildOC.Add(subitem);
                        }
                    }         
                }
            }
            //listbox.ItemsSource = ChildOC;
        }
        private void CheckBox_Click(object sender, RoutedEventArgs e) { OnCheck(); }
        private void CheckBox_Loaded(object sender, RoutedEventArgs e) { OnCheck(); }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            List<MenuItem> itemsList = OC.ToList<MenuItem>();

            foreach (MenuItem item in itemsList)
            {
                if (item.Checked)
                {
                    itemsList.Remove(item);
                }
                else
                {
                    foreach (MenuItem child in item.Items)
                    {
                        if (child.Checked)
                        {
                            item.Items.Remove(child);
                        }
                    }
                }
            }

            OC = new ObservableCollection<MenuItem>(itemsList);
        }
    }
    //public class CustomItem
    //{
    //    public string Name { get; set; }
    //    public bool Checked { get; set; }
    //    public ObservableCollection<CustomItem> Items { get; set; }
    //}
}

