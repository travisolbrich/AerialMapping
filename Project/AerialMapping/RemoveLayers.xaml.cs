//-----------------------------------------------------------------------
// <copyright file="RemoveLayers.xaml.cs" company="CSCE 482: Aerial Mapping">
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
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;

    /// <summary>
    /// Interaction logic for RemoveLayers.xaml
    /// </summary>
    public partial class RemoveLayers : Window
    {
        private RemoveLayerViewModel viewModel;
        //public ObservableCollection<MenuItem> OC { get; set; }

        public RemoveLayers(RemoveLayerViewModel removeLayerViewModel)
        {
            //OC = TreeViewItems;
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
              this.InitializeComponent();
              this.DataContext = removeLayerViewModel;
              this.viewModel = removeLayerViewModel;
        }

        /// <summary>
        /// This function specifies the action when the checkbox is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            viewModel.OnCheck();
        }

        //public void OnCheck()
        //{
            
        //    foreach (MenuItem item in OC)
        //    {
        //        if (item.Checked == true)
        //        {                    
        //            foreach (MenuItem subitem in item.Items)
        //            {
        //                subitem.Checked = true;
        //            }         
        //        }
        //        else
        //        {
        //            bool bAllChildrenChecked = true;

        //            foreach (MenuItem subItem in item.Items)
        //            {
        //                if (subItem.Checked == false)
        //                {
        //                    bAllChildrenChecked = false;
        //                    break;
        //                }
        //            }

        //            if (bAllChildrenChecked)
        //            {
        //                item.Checked = true;
        //            }
        //        }
        //    }
        //    CommandManager.InvalidateRequerySuggested();
            
        //}

        //private void CheckBox_Click(object sender, RoutedEventArgs e) { OnCheck(); }
        //private void CheckBox_Loaded(object sender, RoutedEventArgs e) { OnCheck(); }

        /// <summary>
        /// This function specifies the action when the Remove button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }
    }
}