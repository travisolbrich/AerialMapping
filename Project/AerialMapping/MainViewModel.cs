using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Layers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;

namespace AerialMapping
{
    class MainViewModel : INotifyPropertyChanged
    {
        public MapView m_MapView;

        private Map map;
        public Map IncidentMap
        {
            get { return this.map; }
            set { this.map = value; }
        }

        private List<Dataset> m_DatasetList;

        KmlLayer kmllayerTest; // need to reconcile this with the one in the mainwindow code behind
        public string m_IdToZoomOn = "";

        public ObservableCollection<MenuItem> TreeViewItems { get; set; }

        public DelegateCommand AddLayerCommand { get; set; }

        public DelegateCommand RemoveLayerCommand { get; set; }


        public MainViewModel()
        {
            // when the view model initializes, read the map from the App.xaml resources
            this.map = App.Current.Resources["IncidentMap"] as Map;
            TreeViewItems = new ObservableCollection<MenuItem>();
            MenuItem root = new MenuItem() { Title = "Location" };
            root.Items.Add(new MenuItem() { Title = "Time" });
            TreeViewItems.Add(root);

            m_DatasetList = new List<Dataset>();
            AddLayerCommand = new DelegateCommand(AddLayer);
            RemoveLayerCommand = new DelegateCommand(RemoveLayer);
        }

        private void AddLayer(object parameter)
        {
            // Popup a window to get the layer information
            AddLayer addLayer = new AddLayer();
            addLayer.Owner = (Window)parameter; // In the XAML we pass the window as the parameter
            addLayer.ShowDialog();

            Dataset newLayer = addLayer.DatasetToAdd;
            addLayer.Close();

            if (!String.IsNullOrEmpty(newLayer.FilePath))
            {
                // Save the new dataset
                m_DatasetList.Add(newLayer);

                // Add it to the TreeView on the UI
                MenuItem root = new MenuItem() { Title = newLayer.Location };
                root.Items.Add(new MenuItem() { Title = newLayer.Time.ToShortDateString() });
                TreeViewItems.Add(root);

                // Open the new layer
                LoadKml(newLayer.FilePath, true, false);
            }

            Debug.WriteLine("Location: " + newLayer.Location);
            Debug.WriteLine("Time: " + newLayer.Time);
            Debug.WriteLine("File Path: " + newLayer.FilePath);
        }

        private void RemoveLayer(object parameter)
        {
            RemoveLayers removeLayers = new RemoveLayers(TreeViewItems);
            removeLayers.Owner = (Window)parameter;

            //removeLayers.OC = TreeViewItems;

            removeLayers.ShowDialog();
        }


        public void LoadKml(string path, bool bZoomTo, bool bRelativePath)
        {
            Debug.WriteLine("Path: " + path);
            try
            {
                Uri dataPath = new Uri(path, bRelativePath ? UriKind.Relative : UriKind.Absolute);
                KmlLayer kmllayer = new KmlLayer(dataPath);
                kmllayer.ID = path;
                m_IdToZoomOn = bZoomTo ? path : "";

                m_MapView.Map.Layers.Add(kmllayer);
                kmllayerTest = kmllayer;
            }
            catch
            {
                Debug.WriteLine(string.Format("(MainWindows{LoadKml}) Could not load KML with path {0}", path));
            }
        }

        void NotifiyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
