//-----------------------------------------------------------------------
// <copyright file="AddLocation.xaml.cs" company="CSCE 482: Aerial Mapping">
//     Copyright (c) CSCE 482 Aerial Mapping Design Team
// </copyright>
//-----------------------------------------------------------------------

namespace AerialMapping
{
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

    /// <summary>
    /// Interaction logic for Add_Location.xaml
    /// </summary>
    public partial class AddLocation : Window
    {
        /// <summary>
        /// Initializes a new instance of the AddLocation class.
        /// </summary>
        public AddLocation()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the Location entered by the user.
        /// </summary>
        public string Location
        {
            get;
            set;
        }

        /// <summary>
        /// Button callback for the Add button.
        /// Saves the value in the textbox and hides the window.
        /// </summary>
        /// <param name="sender">Add button</param>
        /// <param name="e">Button press event args</param>
        private void BAddLocation_Click(object sender, RoutedEventArgs e)
        {
            this.Location = NewLocation.Text;
            this.Hide();
        }
    }
}
