namespace AerialMapping
{
    using Esri.ArcGISRuntime.Controls;
    using Esri.ArcGISRuntime.Layers;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;    

    /// <summary>
    /// This is the MainViewModel class which displays the map
    /// </summary>
    class MainViewModel : INotifyPropertyChanged
    {
        public MapView m_MapView;

        private Map map;

        public Map IncidentMap
        {
            get 
            { 
                return this.map; 
            }

            set 
            { 
                this.map = value; 
            }
        }

        private List<Dataset> m_DatasetList;

        KmlLayer kmllayerTest; // need to reconcile this with the one in the mainwindow code behind
        public string m_IdToZoomOn = string.Empty;

        private ObservableCollection<MenuItem> _treeViewItems;

        public ObservableCollection<MenuItem> TreeViewItems
        {
            get 
            { 
                return this._treeViewItems; 
            }

            set 
            { 
                this._treeViewItems = value;
                this.NotifiyPropertyChanged("TreeViewItems");
            }
        }

        public DelegateCommand AddLayerCommand 
        { 
            get; 
            set; 
        }

        public DelegateCommand RemoveLayerCommand 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel" /> class.
        /// This is the constructor for the MainViewModel.
        /// When the view model initializes, it reads the map from the App.xaml resources.
        /// </summary>
        public MainViewModel()
        {
            this.map = App.Current.Resources["IncidentMap"] as Map;
            this.TreeViewItems = new ObservableCollection<MenuItem>();
            MenuItem root = new MenuItem() { Title = "Test Location" };
            root.Items.Add(new MenuItem() { Title = "01-01-2015" });
            this.TreeViewItems.Add(root);

            this.m_DatasetList = new List<Dataset>();
            this.AddLayerCommand = new DelegateCommand(this.AddLayer);
            this.RemoveLayerCommand = new DelegateCommand(this.RemoveLayer);
        }

        /// <summary>
        /// This function loads a KML layer to the map. 
        /// </summary>
        /// <param name="path">The path of the .kml file.</param>
        /// <param name="bZoomTo">Whether we should zoom to the new area (true) or not (false).</param>
        /// <param name="bRelativePath">Specifies if the path is relative (true) or absolute (false).</param>
        public void LoadKml(string path, bool bZoomTo, bool bRelativePath)
        {
            Debug.WriteLine("Path: " + path);
            try
            {
                Uri dataPath = new Uri(path, bRelativePath ? UriKind.Relative : UriKind.Absolute);
                KmlLayer kmllayer = new KmlLayer(dataPath);
                kmllayer.ID = path;
                this.m_IdToZoomOn = bZoomTo ? path : string.Empty;

                this.m_MapView.Map.Layers.Add(kmllayer);
                this.kmllayerTest = kmllayer;
            }
            catch
            {
                Debug.WriteLine(string.Format("(MainWindows{LoadKml}) Could not load KML with path {0}", path));
            }
        }

        /// <summary>
        /// Unloads a KML layer based on the file path in the MenuItem.
        /// </summary>
        /// <param name="item">The MenuItem item</param>
        public void UnloadKML(MenuItem item)
        {
            if (!this.m_MapView.Map.Layers.Remove(item.FilePath))
            {
                Debug.WriteLine("Failed to remove layer with filepath: " + item.FilePath); 
            }
        }

        void NotifiyPropertyChanged(string property)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// "Add Layer" button callback
        /// Pops up a window that gets the user to input the necessary information
        /// for adding a new layer and then adds the layer.
        /// </summary>
        /// <param name="parameter">The window being passed.</param>
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
                this.m_DatasetList.Add(newLayer);

                // See if the Location already exists
                bool bLocationExists = false;

                foreach (MenuItem location in this.TreeViewItems)
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
                    this.TreeViewItems.Add(root);
                }

                // Open the new layer
                this.LoadKml(newLayer.FilePath, true, false);
            }

            Debug.WriteLine("Location: " + newLayer.Location);
            Debug.WriteLine("Time: " + newLayer.Time);
            Debug.WriteLine("File Path: " + newLayer.FilePath);
        }

        /// <summary>
        /// "Remove Layers" button callback. 
        /// This pops up a window which allows the user to select which layers they wish to remove.
        /// The TreeView of layers is updated accordingly. 
        /// </summary>
        /// <param name="parameter"></param>
        private void RemoveLayer(object parameter)
        {
            Window1 win = new Window1(FooViewModel.CreateFoos(this.TreeViewItems));
            win.ShowDialog();

            List<MenuItem> itemsList = win.GetMenuItems();

            //RemoveLayerViewModel removeViewModel = new RemoveLayerViewModel(TreeViewItems);
            //RemoveLayers removeLayers = new RemoveLayers(removeViewModel);
            //removeLayers.Owner = (Window)parameter;

            //TreeViewItems = removeViewModel.Items;

            //removeLayers.ShowDialog();

            //List<MenuItem> itemsList = removeViewModel.Items.ToList<MenuItem>();

            // For each parent
            for (int i = itemsList.Count - 1; i >= 0; i--)
            {
                // Remove all selected children
                for (int j = itemsList[i].Items.Count - 1; j >= 0; j--)
                {
                    if (itemsList[i].Items[j].Checked)
                    {
                        this.UnloadKML(itemsList[i].Items[j]);
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

            this.TreeViewItems = new ObservableCollection<MenuItem>(itemsList);

            win.Close();
        }
    }
}
