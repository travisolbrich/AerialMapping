﻿using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;

namespace AerialMapping
{
    public partial class Window1 : Window
    {
        public FooViewModel root;

        public Window1(List<FooViewModel> viewModels)
        {
            InitializeComponent();

            DataContext = viewModels;

            //FooViewModel root = this.tree.Items[0] as FooViewModel;
            root = viewModels[0];

            base.CommandBindings.Add(
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
                        e.CanExecute = (root.IsChecked != false);
                    }));

            this.tree.Focus();
        }

        public List<MenuItem> GetMenuItems()
        {
            List<MenuItem> locationsToKeep = new List<MenuItem>();

            List<FooViewModel> locations = root.Children;
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
                        
                }
                locationsToKeep.Add(loc);
                
            }

            return locationsToKeep;
        }

        private void bRemove_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }
    }
}