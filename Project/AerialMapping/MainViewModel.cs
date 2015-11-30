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
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows; 
    using Esri.ArcGISRuntime.Controls;
    using Esri.ArcGISRuntime.Layers;
    using Newtonsoft.Json;   

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

            // Read JSON file to get all existing layers
            List<Dataset> layers;
            string json;
            using (StreamReader r = new StreamReader("../../Layers.json"))
            {
                json = r.ReadToEnd();
                layers = JsonConvert.DeserializeObject<List<Dataset>>(json);
            }

            // Backup to manually set up layers in case Layers.json is empty
            if (layers == null)
            {
                MenuItem root = new MenuItem()
                { 
                    Title = "Test Location",
                    FilePath = "../../../../Data/SampleOne/TestData.kml",
                    Checked = true
                };
                root.Items.Add(new MenuItem()
                { 
                    Title = "01-01-2015",
                    FilePath = "../../../../Data/SampleOne/TestData.kml",
                    Checked = true
                });
                /*root.Items.Add(new MenuItem()
                {
                    Title = "02-01-2015",
                    FilePath = "../../../../Data/SampleOne/TestData.kml"
                });*/
                
                this.TreeViewItems.Add(root);

                this.LoadKml("../../../../Data/SampleOne/TestData.kml", true, true);
                ////this.LoadKml("../../../../Data/SampleOne/TestData.kml", true, true);

                // This needs to be after the initial KML load in order for the timeslider
                // logic to update properly based off of the initial layers.
                this.UpdateCurrentLocation(root);
            }
            else
            {
                // Create MenuItems for all locations
                List<MenuItem> locations = new List<MenuItem>();
                foreach (Dataset ds in layers)
                {
                    // If location has not been added already
                    if (locations.Count == 0 || locations.Last().Title != ds.Location)
                    {
                        // Construct new root level node
                        MenuItem node = new MenuItem()
                        {
                            Title = ds.Location,
                            FilePath = ds.FilePath
                        };

                        // Add first subnode to new root level node
                        node.Items.Add(new MenuItem()
                        {
                            Title = ds.Time.ToString(),
                            FilePath = ds.FilePath
                        });
                        locations.Add(node);
                    }
                    else
                    {
                        // If location has been added already
                        // Add another subnode to exisiting root level node
                        locations.Last().Items.Add(new MenuItem()
                        {
                            Title = ds.Time.ToString(),
                            FilePath = ds.FilePath
                        });
                    }
                }

                // Add root level nodes to tree
                foreach (MenuItem item in locations)
                {
                    this.TreeViewItems.Add(item);
                }

                this.UpdateCurrentLocation(locations.First());
                foreach (MenuItem item in locations.First().Items)
                {
                    this.LoadKml(item.FilePath, true, true);
                }
            }
            
            this.datasetList = new List<Dataset>();
            this.AddLayerCommand = new DelegateCommand(this.AddLayer);
            this.RemoveLayerCommand = new DelegateCommand(this.RemoveLayer);
        }

        /// <summary>
        /// INotifyPropertyChanged event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;        

        /// <summary>
        /// Gets or sets the Layer ID on which to zoom upon loading the map.
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
        /// Gets or sets the ArcGIS MapView object that holds all of the map
        /// and layer information.
        /// </summary>
        public MapView MapView
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the ArcGIS Map object that represents the primary map.
        /// </summary>
        public Map Map
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets or sets the reference to the ArcGIS Map application resource used in
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
        /// Gets or sets the number of ticks for the time slider.
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
        /// Gets or sets the tooltip for the time slider.
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
        /// Gets or sets the current value of the time slider.
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
                this.TimeSliderChanged();
            }
        }

        /// <summary>
        /// Gets or sets the items that are in the Layers treeview on the
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
        /// Gets or sets the DelegateCommand used for capturing 
        /// the Add Layer Button Callback.
        /// </summary>
        public DelegateCommand AddLayerCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the DelegateCommand used for capturing 
        /// the Remove Layers Button Callback.
        /// </summary>
        public DelegateCommand RemoveLayerCommand
        {
            get;
            set;
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
            foreach (MenuItem item in this.treeViewItems)
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

            // Read JSON file with existing layers
            List<Dataset> layers;
            string json;
            using (StreamReader r = new StreamReader("../../Layers.json"))
            {
                json = r.ReadToEnd();
                layers = JsonConvert.DeserializeObject<List<Dataset>>(json);
            }

            // Save new layer to flat JSON file
            if (layers == null)
            {
                layers = new List<Dataset>();
            }

            layers.Add(newLayer);
            json = JsonConvert.SerializeObject(layers.ToArray());
            System.IO.File.WriteAllText("../../Layers.json", json);

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

                        this.UpdateCurrentLocation(location, newChild);
                        locationExists = true;
                        break;
                    }
                }

                // If not, then we also need to add the Location to the treeview
                if (!locationExists)
                {
                    // Add it to the TreeView on the UI
                    MenuItem root = new MenuItem(newLayer.Location, newLayer.FilePath);
                    root.Items.Add(new MenuItem(newLayer.Time.ToLongDateString(), newLayer.FilePath));
                    this.TreeViewItems.Add(root);
                    this.UpdateCurrentLocation(root);
                }

                // Open the new layer
                this.LoadKml(newLayer.FilePath, true, false);
            }

            Debug.WriteLine("Location: " + newLayer.Location);
            Debug.WriteLine("Time: " + newLayer.Time);
            Debug.WriteLine("File Path: " + newLayer.FilePath);
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

        /// <summary>
        /// This method is called when the user clicks on an item in
        /// the layers treeview in the popout right side menu. It
        /// updates the currently selected image on the map as the
        /// one that the user clicked on.
        /// </summary>
        /// <param name="newlySelectedItem">The new images selected by 
        /// the user to show on the map.</param>
        /// <param name="updateLocation">Flag to determine if we should set this location as the current
        /// location active on the time bar.</param>
        public void UpdateSelectedItem(MenuItem newlySelectedItem, bool updateLocation = true)
        {
            List<MenuItem> treeViewItemList = this.TreeViewItems.ToList();

            // Iterate through each location.
            foreach (MenuItem location in treeViewItemList)
            {
                bool activeLocation = false;

                // First, check to see if the user clicked a location
                if (location.Equals(newlySelectedItem))
                {
                    // If so, then select its first child
                    if (updateLocation)
                    {
                        this.UpdateCurrentLocation(location);
                    }

                    location.Items.First().Checked = true;
                    location.Checked = true;
                    activeLocation = true;
                    this.NotifiyPropertyChanged("TreeViewItems");
                }
                else
                {
                    // Else, iterate through this location's children to see if
                    // one of them was selected.
                    foreach (MenuItem time in location.Items)
                    {
                        // If this child was the selected item,
                        // then set it as the active image.
                        if (time.Equals(newlySelectedItem))
                        {
                            if (updateLocation)
                            {
                                this.UpdateCurrentLocation(location, time);
                            }

                            time.Checked = true;
                            location.Checked = true;
                            activeLocation = true;
                            this.NotifiyPropertyChanged("TreeViewItems");
                        }
                        else
                        {
                            time.Checked = false;
                        }
                    }
                }

                // If this location, nor any of its children
                // were selected, then make sure it is not highlighted.
                if (!activeLocation)
                {
                    location.Checked = false;
                }
            }
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

            // Update the current location.
            // First, check to see if the current location still exists.
            bool locationStillExists = false;
            foreach (MenuItem location in this.treeViewItems)
            {
                if (this.currentLocation.Title == location.Title)
                {
                    // The location still exists, so update it
                    // in case any DateTimes were removed from it.
                    this.UpdateCurrentLocation(location);
                    locationStillExists = true;
                    break;
                }
            }

            // Or, if the location does not exist, then default to the first one.
            if (!locationStillExists)
            {
                this.currentLocation = this.treeViewItems.FirstOrDefault();
            }
        }

        /// <summary>
        /// Method executed when the time slider is moved by the user.
        /// </summary>
        private void TimeSliderChanged()
        {
            Debug.WriteLine("Time slider value: " + this.TimeSliderValue);

            // Get all of the times for the current Location
            List<MenuItem> currentLocationItems = this.currentLocation.Items.ToList();

            if (this.TimeSliderValue == 0)
            {
                this.TimeSliderToolTip = "GIS Data Only";

                for (int i = 0; i < currentLocationItems.Count; i++)
                {
                    Map.Layers[currentLocationItems[i].FilePath].IsVisible = false;
                }

                // Not sure if this is needed.
                //this.UpdateSelectedItem(this.currentLocation);
            }
            else
            {
                // Update the slider tooltip
                this.TimeSliderToolTip = currentLocationItems[this.TimeSliderValue - 1].Title;

                // Set the selected time to visible and the rest to invisible.
                for (int i = 0; i < currentLocationItems.Count; i++)
                {
                    Layer currentLayer = Map.Layers[currentLocationItems[i].FilePath];
                    if (currentLayer != null)
                    {
                        if (i == this.TimeSliderValue - 1)
                        {
                            currentLayer.IsVisible = true;
                        }
                        else
                        {
                            currentLayer.IsVisible = false;
                        }
                    }
                }

                this.UpdateSelectedItem(currentLocationItems[this.TimeSliderValue - 1], false);
            }
        }

        /// <summary>
        /// Update the currentLocation object in order to setup the
        /// time slider for the images of the selected location.
        /// </summary>
        /// <param name="newLocation">New location value for currentLocation.</param>
        private void UpdateCurrentLocation(MenuItem newLocation)
        {
            this.currentLocation = newLocation;
            this.TimeSliderMax = this.currentLocation.Items.Count;
            this.TimeSliderValue = 1;
        }

        /// <summary>
        /// Update the currentLocation object in order to setup the
        /// time slider for the images of the selected location.
        /// </summary>
        /// <param name="newLocation">New location value for currentLocation.</param>
        /// <param name="childToSelect">Child image for the location that should be visible.</param>
        private void UpdateCurrentLocation(MenuItem newLocation, MenuItem childToSelect)
        {
            this.currentLocation = newLocation;
            this.TimeSliderMax = this.currentLocation.Items.Count;

            // Set the TimeSlider to show the newly added child on the map
            bool foundChild = false;
            for (int i = 0; i < newLocation.Items.Count; i++)
            {
                if (newLocation.Items[i].Title == childToSelect.Title)
                {
                    this.TimeSliderValue = i + 1;
                    foundChild = true;
                    break;
                }
            }

            // Default value just in case.
            if (!foundChild)
            {
                this.TimeSliderValue = 1;
            }
        }

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
    }
}
