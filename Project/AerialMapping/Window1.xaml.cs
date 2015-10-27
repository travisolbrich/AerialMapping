using System.Windows;
using System.Windows.Input;

namespace AerialMapping
{
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();

            FooViewModel root = this.tree.Items[0] as FooViewModel;

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