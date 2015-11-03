namespace AerialMapping
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Input;

    public partial class Window1 : Window
    {
        public FooViewModel root;

        public Window1(List<FooViewModel> viewModels)
        {
            this.InitializeComponent();

            this.DataContext = viewModels;

            //FooViewModel root = this.tree.Items[0] as FooViewModel;
            this.root = viewModels[0];

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

            List<FooViewModel> locations = this.root.Children;
            foreach (FooViewModel location in locations)
            {
                if (location.IsChecked != true)
                {
                    MenuItem loc = new MenuItem(location.Name, location.FilePath);
                    foreach (FooViewModel time in location.Children)
                    {
                        if (time.IsChecked != true)
                        {
                            MenuItem t = new MenuItem(time.Name, time.FilePath);
                            loc.Items.Add(t);
                        }
                    }

                    locationsToKeep.Add(loc);
                }
            }

            return locationsToKeep;
        }

        private void bRemove_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}