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

    public partial class Window1 : Window
    {
        private FooViewModel root;

        public Window1(List<FooViewModel> viewModels)
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

        public FooViewModel Root
        {
            get;
            set;
        }

        public List<MenuItem> GetMenuItems()
        {
            List<MenuItem> locationsToKeep = new List<MenuItem>();

            List<FooViewModel> locations = this.root.Children;
            foreach (FooViewModel location in locations)
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
                foreach (FooViewModel time in location.Children)
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