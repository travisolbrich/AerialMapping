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

namespace AerialMapping
{
    /// <summary>
    /// Interaction logic for Add_Location.xaml
    /// </summary>
    public partial class AddLocation : Window
    {
        /// <summary>
        /// The Location entered by the user.
        /// </summary>
        public String Location 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public AddLocation()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Button callback for the Add button.
        /// Saves the value in the textbox and hides the window.
        /// </summary>
        /// <param name="sender">Add button</param>
        /// <param name="e">Button press event args</param>
        private void BAddLocation_Click(object sender, RoutedEventArgs e)
        {
            Location = NewLocation.Text;
            this.Hide();
        }
    }
}
