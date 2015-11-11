//-----------------------------------------------------------------------
// <copyright file="AddLayer.xaml.cs" company="CSCE 482: Aerial Mapping">
//     Copyright (c) CSCE 482 Aerial Mapping Design Team
// </copyright>
//-----------------------------------------------------------------------
namespace AerialMapping
{
    using System;
    using System.Collections.ObjectModel;
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
    /// Interaction logic for AddLayer.xaml
    /// </summary>
    public partial class AddLayer : Window
    {
        /// <summary>
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
        }

        /// <summary>
        /// Gets or sets the Dataset that we want to add
        /// </summary>
        public Dataset DatasetToAdd { 
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
    }
}
