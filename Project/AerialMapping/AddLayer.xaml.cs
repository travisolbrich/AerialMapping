//-----------------------------------------------------------------------
// <copyright file="AddLayer.xaml.cs" company="CSCE 482: Aerial Mapping">
//     Copyright (c) CSCE 482 Aerial Mapping Design Team
// </copyright>
//-----------------------------------------------------------------------
namespace AerialMapping
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
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
    /// Interaction logic for AddLayer.xaml
    /// </summary>
    public partial class AddLayer : Window
    {
        // Locations in the Locations ComboBox
        private ObservableCollection<string> comboBoxLocations;

        /// <summary>
        /// Initializes a new instance of the AddLayer class.
        /// Adds a layer to the Window
        /// </summary>
        /// <param name="locations">List of location names</param>
        public AddLayer(ObservableCollection<string> locations)
        {
            this.InitializeComponent();
            this.DatasetToAdd = new Dataset();

            // Populate locations combobox
            LocationComboBox.ItemsSource = locations;
            if (locations.Count >= 0)
            {
                LocationComboBox.SelectedIndex = 0;
            }

            this.comboBoxLocations = locations;
        }

        /// <summary>
        /// Gets or sets the Dataset that we want to add
        /// </summary>
        public Dataset DatasetToAdd 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// The function for the button that looks up the file path. 
        /// </summary>
        /// <param name="sender">The File Lookup button</param>
        /// <param name="e">Button press event args</param>
        private void BFilePathLookup_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Map Data|*.kml;*.kmz";
            if (openFileDialog.ShowDialog() == true)
            {
                FilePathInput.Text = openFileDialog.FileName;
            }
        }

        /// <summary>
        /// The function for the button "Add"
        /// </summary>
        /// <param name="sender">The Add button</param>
        /// <param name="e">Button press event args</param>
        private void BAdd_Click(object sender, RoutedEventArgs e)
        {
            // Read the data from the boxes on screen.
            this.DatasetToAdd.Location = LocationComboBox.Text;
            
            if (DateTimeInput.Value.HasValue)
            {
                this.DatasetToAdd.Time = DateTimeInput.Value.Value;
            }

            this.DatasetToAdd.FilePath = FilePathInput.Text;

            // Close this window.
            this.Hide();
        }

        /// <summary>
        /// The function to add a location.
        /// </summary>
        /// <param name="sender">Add location button</param>
        /// <param name="e">Button press event args</param>
        private void BAddLocation_Click(object sender, RoutedEventArgs e)
        {
            // Open the window for the user to enter a new location.
            AddLocation addLocation = new AddLocation();
            addLocation.Owner = this;
            addLocation.ShowDialog();

            // Get the new location entered by the user
            string newLocation = addLocation.Location;

            addLocation.Close();

            // Add that new location to the combo box and select it.
            this.comboBoxLocations.Add(newLocation);
            LocationComboBox.ItemsSource = this.comboBoxLocations;
            LocationComboBox.SelectedIndex = LocationComboBox.Items.Count - 1;
        }
    }
}
