using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;

namespace AerialMapping
{
    public partial class Window1 : Window
    {
        public Window1(List<FooViewModel> viewModels)
        {
            InitializeComponent();

            DataContext = viewModels;

            //FooViewModel root = this.tree.Items[0] as FooViewModel;
            FooViewModel root = viewModels[0];

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
    }
}