//-----------------------------------------------------------------------
// <copyright file="RemoveLayers.xaml.cs" company="CSCE 482: Aerial Mapping">
//     Copyright (c) CSCE 482 Aerial Mapping Design Team
// </copyright>
//-----------------------------------------------------------------------

namespace AerialMapping
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// The Code Behind for the RemoveLayers Window.
    /// </summary>
    public partial class RemoveLayers : Window
    {
        private RemoveLayersViewModel root;

        /// <summary>
        /// Initializes a new instance of the RemoveLayers class.
        /// Takes the viewModels and sets up
        /// the treeview for the window.
        /// </summary>
        /// <param name="viewModels">Data for the treeview.</param>
        public RemoveLayers(List<RemoveLayersViewModel> viewModels)
        {
            this.InitializeComponent();

            this.DataContext = viewModels;

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

        /// <summary>
        /// Gets or sets the root element of the tree structure.
        /// </summary>
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

            // Don't include the root, which is the "All" selection.
            List<RemoveLayersViewModel> locations = this.root.Children;

            foreach (RemoveLayersViewModel location in locations)
            {
                // Add the location to the list.
                MenuItem loc;
                if (location.IsChecked == true)
                {
                   loc = new MenuItem(location.Name, location.FilePath, true);
                }
                else
                {
                    loc = new MenuItem(location.Name, location.FilePath, false);
                }

                // Add each of the times for that location to that children of that location.
                foreach (RemoveLayersViewModel time in location.Children)
                {
                    MenuItem t;
                    if (time.IsChecked == true)
                    {
                        t = new MenuItem(time.Name, time.FilePath, true);
                    }
                    else
                    {
                        t = new MenuItem(time.Name, time.FilePath, false);
                    }

                    loc.Items.Add(t);                    
                }

                locationsToKeep.Add(loc);                
            }

            return locationsToKeep;
        }

        /// <summary>
        /// Remove button callback. Closes the window so that the
        /// mainviewmodel can perform the actual remove options
        /// on the map.
        /// </summary>
        /// <param name="sender">The remove button.</param>
        /// <param name="e">Button clicked event args.</param>
        private void BRemove_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}