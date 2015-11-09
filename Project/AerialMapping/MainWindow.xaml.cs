//-----------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="CSCE 482: Aerial Mapping">
//     Copyright (c) CSCE 482 Aerial Mapping Design Team
// </copyright>
//-----------------------------------------------------------------------
//        _ _    _  _____ _______   _____   ____    _____ _______ 
//       | | |  | |/ ____|__   __| |  __ \ / __ \  |_   _|__   __|
//       | | |  | | (___    | |    | |  | | |  | |   | |    | |   
//   _   | | |  | |\___ \   | |    | |  | | |  | |   | |    | |   
//  | |__| | |__| |____) |  | |    | |__| | |__| |  _| |_   | |   
//   \____/ \____/|_____/   |_|    |_____/ \____/  |_____|  |_|  

﻿using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Symbology;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.IO;
using System.Collections.ObjectModel;

namespace AerialMapping
{
    public partial class MainWindow : Window
    {
        // Member Variables
        public double kmzZoomDelaySec = 3;
        private double currAngle = 0;
        private Viewpoint centerPoint;
        KmlLayer kmllayerTest;
        private List<Dataset> datasetList;
        
        public MainViewModel mainViewModel;

        // Constructor
        public MainWindow()
        {
            mainViewModel = new MainViewModel();
            this.DataContext = mainViewModel;
            InitializeComponent();
            
            //mainViewModel.m_MapView.MaxScale = 2100; // set the maximum zoom value
            datasetList = new List<Dataset>();
        }

        // Event triggered when the mapview has a new layer that is loaded.
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

        // Event triggered when the mapview is initialized.
        private void BaseMapView_Initialized(object sender, EventArgs e)
        {
            mainViewModel.MapView = (MapView)sender;
            mainViewModel.MapView.MaxScale = 1; // set the maximum zoom value
            //mainViewModel.LoadKml("../../../../Data/SampleOne/TestData.kml", true, true);
        }

        // Button callback to allow the user to open a file.
        private void bOpenFileDialog_Click(object sender, RoutedEventArgs e)
        {
            mainViewModel.AddLayer(this);
        }

        // Callback for the quit button.
        private void bExitProgram_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // The following are buttons for rotation
        private void bRotateLeft_Click(object sender, RoutedEventArgs e)
        {
            currAngle = (currAngle += 10) % 360;
            // apply the rotation to the map
            mainViewModel.MapView.SetRotationAsync(currAngle);
        }

        private void bRotateDefault_Click(object sender, RoutedEventArgs e)
        {
            // apply the rotation to the map
            currAngle = 0;
            mainViewModel.MapView.SetRotationAsync(currAngle);
        }
        private void bRotateRight_Click(object sender, RoutedEventArgs e)
        {
            currAngle = (currAngle -= 10) % 360;
            // apply the rotation to the map
            mainViewModel.MapView.SetRotationAsync(currAngle);
        }

        // The following are buttons for zoom
        private void bZoomIn_Click(object sender, RoutedEventArgs e)
        {
            mainViewModel.MapView.ZoomAsync(1.2);
            bZoomSlider.Value = mainViewModel.MapView.Scale;
        }

        private void bZoomOut_Click(object sender, RoutedEventArgs e)
        {
            mainViewModel.MapView.ZoomAsync(0.8);
            bZoomSlider.Value = mainViewModel.MapView.Scale;
        }

        // The following is for zoom slider action
        private void bZoomSlider_Click(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double zoomScale = e.NewValue;
            if (mainViewModel != null && mainViewModel.MapView != null)
            {
                mainViewModel.MapView.ZoomToScaleAsync(zoomScale);
            }
            
        }

        // Add the given layer to the tree.
        public void AddLayerToTree(Layer layer)
        {
            MenuItem root = new MenuItem() { Title = "Location" };
            root.Items.Add(new MenuItem() { Title = "Time" });
            //TreeViewItems.Add(root);
            //LayerView.Items.Add(root);
        }

        // The following is for the time slider.
        private void bTimeSlider_Click(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // this is for testing only.
            if (e.NewValue == 3) mainViewModel.MapView.Map.Layers.Add(kmllayerTest);
            else if (e.NewValue == 2) mainViewModel.MapView.Map.Layers.Remove(kmllayerTest);
            // m_MapView.Map.Layers.Move(layerVectorThing[e.NewValue], 0);
        }

        // This will update the time slider with new range and tick marks
        private void updateTimeSlider(int numLayers, int currLayer)
        {
            bTimeSlider.Maximum = numLayers - 1;
            bTimeSlider.Value = currLayer;
        }

        // Callback for button to center the view.
        private void bCenterView_Click(object sender, RoutedEventArgs e)
        {
            //((MapView)sender).SetViewAsync(((KmlLayer)e.Layer).RootFeature.Viewpoint, TimeSpan.FromSeconds(m_KmzZoomDelaySec));
            mainViewModel.MapView.SetViewAsync(centerPoint);
        }


        // Callback for the button that shows/hides the popout menu on the right side.
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