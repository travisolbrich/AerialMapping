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
    /// Interaction logic for AddLayer.xaml
    /// </summary>
    public partial class AddLayer : Window
    {
        public Dataset DatasetToAdd { get; set; }

        public AddLayer()
        {
            InitializeComponent();
            DatasetToAdd = new Dataset();
        }

        private void bFilePathLookup_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Map Data|*.kml;*.kmz";
            if (openFileDialog.ShowDialog() == true)
            {
                FilePathInput.Text = openFileDialog.FileName;
            }
        }

        private void bAdd_Click(object sender, RoutedEventArgs e)
        {
            DatasetToAdd.Location = LocationInput.Text;
            
            if (DateTimeInput.Value.HasValue)
            {
                DatasetToAdd.Time = DateTimeInput.Value.Value;
            }

            DatasetToAdd.FilePath = FilePathInput.Text;
            this.Hide();
        }
    }
}
