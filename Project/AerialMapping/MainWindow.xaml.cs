//-----------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="CSCE 482: Aerial Mapping">
//     Copyright (c) CSCE 482 Aerial Mapping Design Team
// </copyright>
//-----------------------------------------------------------------------

namespace AerialMapping
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media.Animation;
    using System.IO;
    using System.Collections.ObjectModel;
    ﻿using Esri.ArcGISRuntime.Controls;
    using Esri.ArcGISRuntime.Geometry;
    using Esri.ArcGISRuntime.Layers;
    using Esri.ArcGISRuntime.Symbology;
    using Microsoft.Win32;

    public partial class MainWindow : Window
    {
        /// <summary>
        /// The following are constant variables.
        /// </summary>
        public const int minScale = 1;
        public const int maxScale = 40000000;
        public const int mapRotateDelta = 10;
        public const double mapZoomScaleDelta = 0.2;

        /// <summary>
        /// The following are other member variables.
        /// </summary>
        public double kmzZoomDelaySec = 3;
        private double currAngle = 0;
        private Viewpoint centerPoint;
        KmlLayer kmllayerTest;
        private List<Dataset> datasetList;
        
        public MainViewModel mainViewModel;

        /// <summary>
        /// Constructor for the MainWindow. 
        /// </summary>
        public MainWindow()
        {
            mainViewModel = new MainViewModel();
            this.DataContext = mainViewModel;
            this.MouseWheel += zoomSlider_MouseWheel;
            InitializeComponent();
            datasetList = new List<Dataset>();
        }

        /// <summary>
        /// The event that is triggered when the MapView has a new layer that is loaded. 
        /// </summary>
        /// <param name="sender">MapView object</param>
        /// <param name="e">Layer Loaded event args</param>
        private void BaseMapView_LayerLoaded(object sender, LayerLoadedEventArgs e)
        {
            if (e.LoadError != null)
            {
                Debug.WriteLine(string.Format("(MainWindows{BaseMapView_LayerLoaded}) Error while loading layer : {0} - {1}", e.Layer.ID, e.LoadError.Message));
                return;
            }
            if (mainViewModel.IdToZoomOn != "" && e.Layer.ID == mainViewModel.IdToZoomOn)
            {
                mainViewModel.IdToZoomOn = "";
                centerPoint = ((KmlLayer)e.Layer).RootFeature.Viewpoint;
                ((MapView)sender).SetViewAsync(centerPoint, TimeSpan.FromSeconds(kmzZoomDelaySec));
            }
            AddLayerToTree(e.Layer);
        }

        /// <summary>
        /// The event that is triggered when the MapView is initialized.
        /// </summary>
        /// <param name="sender">MapView object</param>
        /// <param name="e">MapView Initiaized event args</param>
        private void BaseMapView_Initialized(object sender, EventArgs e)
        {
            mainViewModel.MapView = (MapView)sender;
            mainViewModel.MapView.MaxScale = minScale; // set the maximum zoom value
            //mainViewModel.LoadKml("../../../../Data/SampleOne/TestData.kml", true, true);
        }

        /// <summary>
        /// The button callback that allows the user to open a file. 
        /// </summary>
        /// <param name="sender">Open File Button</param>
        /// <param name="e">Button Click events</param>
        private void bOpenFileDialog_Click(object sender, RoutedEventArgs e)
        {
            mainViewModel.AddLayer(this);
        }

        /// <summary>
        /// The button callback that quits the program. 
        /// </summary>
        /// <param name="sender">Exit Button</param>
        /// <param name="e">Button Click events</param>
        private void bExitProgram_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// The button callback for the Rotate Left button.
        /// Rotates the map to the left. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bRotateLeft_Click(object sender, RoutedEventArgs e)
        {
            currAngle = (currAngle += mapRotateDelta) % 360;
            // apply the rotation to the map
            mainViewModel.MapView.SetRotationAsync(currAngle);
        }

        /// <summary>
        /// The button callback for the North Up button.
        /// Resets the map rotation to North Up. 
        /// </summary>
        /// <param name="sender">RReset Rotation Button</param>
        /// <param name="e">Button Click events</param>
        private void bRotateDefault_Click(object sender, RoutedEventArgs e)
        {
            // apply the rotation to the map
            currAngle = 0;
            mainViewModel.MapView.SetRotationAsync(currAngle);
        }

        /// <summary>
        /// The button callback for the Rotate Right button.
        /// Rotates the map to the right.
        /// </summary>
        /// <param name="sender">Rotate Right Button</param>
        /// <param name="e">Button CLick events args</param>
        private void bRotateRight_Click(object sender, RoutedEventArgs e)
        {
            currAngle = (currAngle -= mapRotateDelta) % 360;
            // apply the rotation to the map
            mainViewModel.MapView.SetRotationAsync(currAngle);
        }

        /// <summary>
        /// The button callback for the Zoom In (+) button.
        /// Zooms in the map by a scale. 
        /// </summary>
        /// <param name="sender">Zoom In Button</param>
        /// <param name="e">Button Click event args</param>
        private void bZoomIn_Click(object sender, RoutedEventArgs e)
        {
            mainViewModel.MapView.ZoomAsync(1 + mapZoomScaleDelta);
            updateZoomSlider();
        }

        /// <summary>
        /// The button callback for the Zoom Out (-) button.
        /// Zooms out the map by a scale. 
        /// </summary>
        /// <param name="sender">Zoom Out Button</param>
        /// <param name="e">Button Click event args</param>
        private void bZoomOut_Click(object sender, RoutedEventArgs e)
        {
            mainViewModel.MapView.ZoomAsync(1 - mapZoomScaleDelta);
            updateZoomSlider();
        }

        /// <summary>
        /// The callback for the Zoom Slider operation.
        /// </summary>
        /// <param name="sender">Zoom Slider</param>
        /// <param name="e">Event args for change in the zoom slider</param>
        private void bZoomSlider_Click(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double zoomScale = logZoomSlider(e.NewValue);
            if (mainViewModel != null && mainViewModel.MapView != null)
            {
                mainViewModel.MapView.ZoomToScaleAsync(zoomScale);
            }
        }

        /// <summary>
        /// The callback for when the mouse wheel is moved.
        /// It is supposed to update the zoom slider position. 
        /// </summary>
        /// <param name="sender">Mouse Wheel Event</param>
        /// <param name="e">Mouse Whell Event args</param>
        private void zoomSlider_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            // this invokes some kinda witchcraft
            // updateZoomSlider();
        }

        /// <summary>
        /// This function determines the actual scale given the zoom slider value
        /// </summary>
        /// <param name="value">The zoom slider value</param>
        /// <returns>The actual zoom scale</returns>
        private double logZoomSlider(double value)
        {
            double sliderMin = 0;
            double sliderMax = 100;

            double zoomMin = Math.Log(minScale);
            double zoomMax = Math.Log(maxScale);

            double resultScale = (zoomMax - zoomMin) / (sliderMax - sliderMin);

            return Math.Exp(zoomMin + resultScale * (value - sliderMin));
        }

        /// <summary>
        /// This function determines the zoom slider value given the actual scale
        /// </summary>
        /// <param name="value">The actual zoom scale</param>
        /// <returns>The zoom slider value</returns>
        private double logZoomScale(double value)
        {
            double sliderMin = 0;
            double sliderMax = 100;

            double zoomMin = Math.Log(minScale);
            double zoomMax = Math.Log(maxScale);

            double resultScale = (zoomMax - zoomMin) / (sliderMax - sliderMin);

            return (Math.Log(value) - zoomMin) / resultScale + sliderMin;
        }

        /// <summary>
        /// This function updates the position of the zoom slider. 
        /// </summary>
        private void updateZoomSlider()
        {
            bZoomSlider.Value = logZoomScale(mainViewModel.MapView.Scale);
        }

        /// <summary>
        /// This function adds a given layer to the tree. 
        /// </summary>
        /// <param name="layer">The layer that is being added. </param>
        public void AddLayerToTree(Layer layer)
        {
            MenuItem root = new MenuItem() { Title = "Location" };
            root.Items.Add(new MenuItem() { Title = "Time" });
            //TreeViewItems.Add(root);
            //LayerView.Items.Add(root);
        }

        /// <summary>
        /// The following is for the time slider.
        /// </summary>
        /// <param name="sender">Time Slider</param>
        /// <param name="e">Time Slider changed event args</param>
        private void bTimeSlider_Click(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // this is for testing only.
            if (e.NewValue == 3) mainViewModel.MapView.Map.Layers.Add(kmllayerTest);
            else if (e.NewValue == 2) mainViewModel.MapView.Map.Layers.Remove(kmllayerTest);
            // m_MapView.Map.Layers.Move(layerVectorThing[e.NewValue], 0);
        }

        /// <summary>
        /// This will update the time slider with new range and tick marks
        /// </summary>
        /// <param name="numLayers">Number of ticks</param>
        /// <param name="currLayer">Which tick to be on</param>
        private void updateTimeSlider(int numLayers, int currLayer)
        {
            bTimeSlider.Maximum = numLayers - 1;
            bTimeSlider.Value = currLayer;
        }

        /// <summary>
        /// Callback for button to center the view.
        /// </summary>
        /// <param name="sender">Center View Button</param>
        /// <param name="e">Button Click event args</param>
        private void bCenterView_Click(object sender, RoutedEventArgs e)
        {
            //((MapView)sender).SetViewAsync(((KmlLayer)e.Layer).RootFeature.Viewpoint, TimeSpan.FromSeconds(m_KmzZoomDelaySec));
            mainViewModel.MapView.SetViewAsync(centerPoint);
        }

        /// <summary>
        /// Callback for the button that shows/hides the popout menu on the right side.
        /// </summary>
        /// <param name="sender">Show/Hide Menu Button</param>
        /// <param name="e">Button Click event args</param>
        private void bRightMenuShowHide_Click(object sender, RoutedEventArgs e)
        {
            Button rightMenuToggle = sender as Button;
            
            // Menu is hidden
            if (String.Equals(rightMenuToggle.Content.ToString(), "<", StringComparison.InvariantCultureIgnoreCase))
            {
                Storyboard sb = Resources["sbShowRightMenu"] as Storyboard;
                sb.Begin(pnlRightMenu);

                rightMenuToggle.Content = ">";
            }
            // Menu is out
            else
            {
                Storyboard sb = Resources["sbHideRightMenu"] as Storyboard;
                sb.Begin(pnlRightMenu);

                rightMenuToggle.Content = "<";
            }
        }

    }
}