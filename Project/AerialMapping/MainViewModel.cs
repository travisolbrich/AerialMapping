//-----------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="CSCE 482: Aerial Mapping">
//     Copyright (c) CSCE 482 Aerial Mapping Design Team
// </copyright>
//-----------------------------------------------------------------------

namespace AerialMapping
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows; 
    using Esri.ArcGISRuntime.Controls;
    using Esri.ArcGISRuntime.Layers;   

    /// <summary>
    /// This is the MainViewModel class which displays the map
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        // Member Variables
        private string idToZoomOn = string.Empty;

        private List<Dataset> datasetList; // Holds all of the layer data.

        private KmlLayer kmllayerTest; // need to reconcile this with the one in the mainwindow code behind

        private int timeSliderMax;

        private int timeSliderValue;

        private MenuItem currentLocation;

        private ObservableCollection<MenuItem> treeViewItems;

        private string timeSliderToolTip;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel" /> class.
        /// This is the constructor for the MainViewModel.
        /// When the view model initializes, it reads the map from the App.xaml resources.
        /// </summary>
        public MainViewModel()
        {
            this.Map = App.Current.Resources["IncidentMap"] as Map;
            this.TreeViewItems = new ObservableCollection<MenuItem>();
            MenuItem root = new MenuItem()
            { 
                Title = "Test Location",
                FilePath = "../../../../Data/SampleOne/TestData.kml"
            };
            root.Items.Add(new MenuItem()
            { 
                Title = "01-01-2015",
                FilePath = "../../../../Data/SampleOne/TestData.kml"
            });
            root.Items.Add(new MenuItem()
            {
                Title = "02-01-2015",
                FilePath = "../../../../Data/SampleOne/TestData.kml"
            });
            UpdateCurrentLocation(root);
            this.TreeViewItems.Add(root);

            this.datasetList = new List<Dataset>();
            this.AddLayerCommand = new DelegateCommand(this.AddLayer);
            this.RemoveLayerCommand = new DelegateCommand(this.RemoveLayer);

            LoadKml("../../../../Data/SampleOne/TestData.kml", true, true);
            LoadKml("../../../../Data/SampleOne/TestData.kml", true, true);
        }        

        /// <summary>
        /// The Layer ID on which to zoom upon loading the map.
        /// </summary>
        public string IdToZoomOn
        {
            get 
            { 
                return this.idToZoomOn; 
            }

            set 
            { 
                this.idToZoomOn = value; 
            }
        }

        /// <summary>
        /// The ArcGIS MapView object that holds all of the map
        /// and layer information.
        /// </summary>
        public MapView MapView
        {
            get;
            set;
        }

        /// <summary>
        /// The ArcGIS Map object that represents the primary map.
        /// </summary>
        public Map Map
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Reference to the ArcGIS Map application resource used in
        /// creating the Map.
        /// </summary>
        public Map IncidentMap
        {
            get
            {
                return this.Map;
            }

            set
            {
                this.Map = value;
            }
        }

        /// <summary>
        /// Holds the number of ticks for the time slider.
        /// </summary>
        public int TimeSliderMax
        {
            get
            {
                return this.timeSliderMax;
            }

            set
            {
                this.timeSliderMax = value;
                this.NotifiyPropertyChanged("TimeSliderMax");
            }
        }

        /// <summary>
        /// Holds the tooltip for the time slider.
        /// </summary>
        public string TimeSliderToolTip
        {
            get
            {
                return this.timeSliderToolTip;
            }

            set
            {
                this.timeSliderToolTip = value;
                this.NotifiyPropertyChanged("TimeSliderToolTip");
            }
        }

        /// <summary>
        /// The current value of the time slider.
        /// </summary>
        public int TimeSliderValue
        {
            get
            {
                return this.timeSliderValue;
            }

            set
            {
                this.timeSliderValue = value;
                this.NotifiyPropertyChanged("TimeSliderValue");
                TimeSliderChanged();
            }
        }

        /// <summary>
        /// Holds the items that are in the Layers treeview on the
        /// pop out menu.
        /// </summary>
        public ObservableCollection<MenuItem> TreeViewItems
        {
            get
            {
                return this.treeViewItems;
            }

            set
            {
                this.treeViewItems = value;
                this.NotifiyPropertyChanged("TreeViewItems");
            }
        }

        /// <summary>
        /// Used for capturing the Add Layer Button Callback.
        /// </summary>
        public DelegateCommand AddLayerCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Used for capturing the Remove Layers Button Callback.
        /// </summary>
        public DelegateCommand RemoveLayerCommand
        {
            get;
            set;
        }

        /// <summary>
        /// This function loads a KML layer to the map. 
        /// </summary>
        /// <param name="path">The path of the .kml file.</param>
        /// <param name="zoomTo">Whether we should zoom to the new area (true) or not (false).</param>
        /// <param name="relativePath">Specifies if the path is relative (true) or absolute (false).</param>
        public void LoadKml(string path, bool zoomTo, bool relativePath)
        {
            Debug.WriteLine("Path: " + path);
            try
            {
                Uri dataPath = new Uri(path, relativePath ? UriKind.Relative : UriKind.Absolute);
                KmlLayer kmllayer = new KmlLayer(dataPath);
                kmllayer.ID = path;
                this.idToZoomOn = zoomTo ? path : string.Empty;

                //this.MapView.Map.Layers.Add(kmllayer);
                Map.Layers.Add(kmllayer);
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
            if (!this.MapView.Map.Layers.Remove(item.FilePath))
            {
                Debug.WriteLine("Failed to remove layer with filepath: " + item.FilePath); 
            }
        }

        private void UpdateCurrentLocation(MenuItem newLocation)
        {
            currentLocation = newLocation;
            TimeSliderMax = currentLocation.Items.Count - 1;
            TimeSliderValue = 0;
        }

        /// <summary>
        /// INotifyPropertyChanged event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// INotifyPropertyChanged method for updating the UI.
        /// </summary>
        /// <param name="property">Name of the UI element to update.</param>
        private void NotifiyPropertyChanged(string property)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        /// <summary>
        /// "Add Layer" button callback
        /// Pops up a window that gets the user to input the necessary information
        /// for adding a new layer and then adds the layer.
        /// </summary>
        /// <param name="parameter">The window being passed.</param>
        public void AddLayer(object parameter)
        {
            // Generate list of location names
            ObservableCollection<string> locations = new ObservableCollection<string>();
            foreach (MenuItem item in treeViewItems)
            {
                locations.Add(item.Title);
            }

            // Popup a window to get the layer information
            AddLayer addLayer = new AddLayer(locations);
            addLayer.Owner = (Window)parameter; // In the XAML we pass the window as the parameter
            addLayer.ShowDialog();

            // Get the data from the AddLayer window
            Dataset newLayer = addLayer.DatasetToAdd;
            addLayer.Close();

            if (!string.IsNullOrEmpty(newLayer.FilePath))
            {
                // Save the new dataset
                this.datasetList.Add(newLayer);

                // See if the Location already exists
                bool locationExists = false;

                foreach (MenuItem location in this.TreeViewItems)
                {
                    // If so, then add the new layer as a child of that Location
                    if (location.Title == newLayer.Location)
                    {
                        MenuItem newChild = new MenuItem(newLayer.Time.ToShortDateString(), newLayer.FilePath);
                        location.Items.Add(newChild);

                        // Sort the children based on time
                        location.Items = new ObservableCollection<MenuItem>(location.Items.OrderBy(time => time.Title).ToList());

                        UpdateCurrentLocation(location);
                        locationExists = true;
                        break;
                    }
                }

                // If not, then we also need to add the Location to the treeview
                if (!locationExists)
                {
                    // Add it to the TreeView on the UI
                    MenuItem root = new MenuItem() { Title = newLayer.Location };
                    root.Items.Add(new MenuItem() { Title = newLayer.Time.ToShortDateString() });
                    this.TreeViewItems.Add(root);
                    UpdateCurrentLocation(root);
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
        /// <param name="parameter">Layer to be removed</param>
        private void RemoveLayer(object parameter)
        {
            RemoveLayers win = new RemoveLayers(RemoveLayersViewModel.CreateViewModels(this.TreeViewItems));
            win.ShowDialog();

            List<MenuItem> itemsList = win.GetMenuItems();

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

        /// <summary>
        /// Method executed when the time slider is moved by the user.
        /// </summary>
        private void TimeSliderChanged()
        {
            Debug.WriteLine("Time slider value: " + TimeSliderValue);            

            // Get all of the times for the current Location
            List<MenuItem> currentLocationItems = currentLocation.Items.ToList();

            // Update the slider tooltip
            this.TimeSliderToolTip = currentLocationItems[TimeSliderValue].Title;

            // Set the selected time to visible and the rest to invisible.
            for (int i = 0; i < currentLocationItems.Count; i++)
            {
                Layer currentLayer = Map.Layers[currentLocationItems[i].FilePath];
                if (currentLayer != null)
                {
                    if (i == TimeSliderValue)
                    {                    
                        currentLayer.IsVisible = true;
                    }
                    else
                    {
                        currentLayer.IsVisible = false;
                    }
                }
                
            }
        }
    }
}
