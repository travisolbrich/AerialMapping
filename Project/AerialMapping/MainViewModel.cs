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

        private ObservableCollection<MenuItem> _treeViewItems;
        public ObservableCollection<MenuItem> TreeViewItems
        {
            get { return _treeViewItems; }
            set 
            { 
                _treeViewItems = value;
                NotifiyPropertyChanged("TreeViewItems");
            }
        }


        public DelegateCommand AddLayerCommand { get; set; }

        public DelegateCommand RemoveLayerCommand { get; set; }


        // Constructor
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


        // "Add Layer" button callback
        // Pops up a window that gets the user to input the necessary information
        // for adding a new layer and then adds the layer.
        private void AddLayer(object parameter)
        {
            // Popup a window to get the layer information
            AddLayer addLayer = new AddLayer();
            addLayer.Owner = (Window)parameter; // In the XAML we pass the window as the parameter
            addLayer.ShowDialog();

            // Get the data from the AddLayer window
            Dataset newLayer = addLayer.DatasetToAdd;
            addLayer.Close();

            if (!String.IsNullOrEmpty(newLayer.FilePath))
            {
                // Save the new dataset
                m_DatasetList.Add(newLayer);

                // See if the Location already exists
                bool bLocationExists = false;

                foreach (MenuItem location in TreeViewItems)
                {
                    // If so, then add the new layer as a child of that Location
                    if (location.Title == newLayer.Location)
                    {
                        MenuItem newChild = new MenuItem(newLayer.Time.ToShortDateString(), newLayer.FilePath);
                        location.Items.Add(newChild);
                        bLocationExists = true;
                        break;
                    }
                }

                // If not, then we also need to add the Location to the treeview
                if (!bLocationExists)
                {
                    // Add it to the TreeView on the UI
                    MenuItem root = new MenuItem() { Title = newLayer.Location };
                    root.Items.Add(new MenuItem() { Title = newLayer.Time.ToShortDateString() });
                    TreeViewItems.Add(root);
                }                

                // Open the new layer
                LoadKml(newLayer.FilePath, true, false);
            }

            Debug.WriteLine("Location: " + newLayer.Location);
            Debug.WriteLine("Time: " + newLayer.Time);
            Debug.WriteLine("File Path: " + newLayer.FilePath);
        }


        // "Remove Layers" button callback.
        // Pops up a window which allows the user to select which layers
        // they wish to removes. The treeview of layers is updated accordingly.
        private void RemoveLayer(object parameter)
        {
            RemoveLayers removeLayers = new RemoveLayers(TreeViewItems);
            removeLayers.Owner = (Window)parameter;

            TreeViewItems = removeLayers.OC;

            removeLayers.ShowDialog();

            List<MenuItem> itemsList = removeLayers.OC.ToList<MenuItem>();

            // For each parent
            for (int i = itemsList.Count - 1; i >= 0; i--)
            {
                // Remove all selected children
                for (int j = itemsList[i].Items.Count - 1; j >= 0; j--)
                {
                    if (itemsList[i].Items[j].Checked)
                    {
                        UnloadKML(itemsList[i].Items[j]);
                        itemsList[i].Items.Remove(itemsList[i].Items[j]);
                    }
                }

                // Remove the parent if necessary
                if (itemsList[i].Checked || itemsList[i].Items.Count == 0)
                {
                    // KML layers are loaded and unloaded at the child level
                    // so we do not need an UnloadKML here.
                    itemsList.Remove(itemsList[i]);
                }
            }

            TreeViewItems = new ObservableCollection<MenuItem>(itemsList);

            removeLayers.Close();
        }


        // Loads a KML layer to the map
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


        // Unloads a KML layer based on the filepath in the MenuItem.
        public void UnloadKML(MenuItem item)
        {
            if (!m_MapView.Map.Layers.Remove(item.FilePath))
            {
                Debug.WriteLine("Failed to remove layer with filepath: " + item.FilePath); 
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
