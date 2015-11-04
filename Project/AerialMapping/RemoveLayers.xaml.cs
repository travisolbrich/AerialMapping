//-----------------------------------------------------------------------
// <copyright file="Window1.xaml.cs" company="CSCE 482: Aerial Mapping">
//     Copyright (c) CSCE 482 Aerial Mapping Design Team
// </copyright>
//-----------------------------------------------------------------------

namespace AerialMapping
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Input;

    public partial class RemoveLayers : Window
    {
        private RemoveLayersViewModel root;

        public RemoveLayers(List<RemoveLayersViewModel> viewModels)
        {
            this.InitializeComponent();

            this.DataContext = viewModels;

            //FooViewModel root = this.tree.Items[0] as FooViewModel;
            this.root = viewModels[0];

            CommandBindings.Add(
                new CommandBinding(
                    ApplicationCommands.Undo,
                    (sender, e) => // Execute
                    {                        
                        e.Handled = true;
                        root.IsChecked = false;
                        this.tree.Focus();
                    },
                    (sender, e) => // CanExecute
                    {
                        e.Handled = true;
                        e.CanExecute = root.IsChecked != false;
                    }));

            this.tree.Focus();
        }

        public RemoveLayersViewModel Root
        {
            get;
            set;
        }

        /// <summary>
        /// This method converts all of the information from the remove layers
        /// window and returns it as a list of menuitems. This is how the
        /// remove layers window returns the results of the users actions.
        /// </summary>
        /// <returns>List of MenuItems including the user's checkbox selections.</returns>
        public List<MenuItem> GetMenuItems()
        {
            List<MenuItem> locationsToKeep = new List<MenuItem>();

            List<RemoveLayersViewModel> locations = this.root.Children;
            foreach (RemoveLayersViewModel location in locations)
            {
                MenuItem loc;
                if (location.IsChecked != true)
                {
                   loc = new MenuItem(location.Name, location.FilePath, true);
                }
                else
                {
                    loc = new MenuItem(location.Name, location.FilePath, false);
                }
                foreach (RemoveLayersViewModel time in location.Children)
                {
                    MenuItem t;
                    if (time.IsChecked != true)
                    {
                        t = new MenuItem(time.Name, time.FilePath, true);
                    }
                    else
                    {
                        t = new MenuItem(time.Name, time.FilePath, false);
                    }

                    loc.Items.Add(t);
                    //locationsToKeep.Add(loc);

                }
                locationsToKeep.Add(loc);
                
            }

            return locationsToKeep;
        }

        private void BRemove_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}