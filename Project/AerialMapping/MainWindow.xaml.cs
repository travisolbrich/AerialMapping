/*
 *        _ _    _  _____ _______   _____   ____    _____ _______ 
 *       | | |  | |/ ____|__   __| |  __ \ / __ \  |_   _|__   __|
 *       | | |  | | (___    | |    | |  | | |  | |   | |    | |   
 *   _   | | |  | |\___ \   | |    | |  | | |  | |   | |    | |   
 *  | |__| | |__| |____) |  | |    | |__| | |__| |  _| |_   | |   
 *   \____/ \____/|_____/   |_|    |_____/ \____/  |_____|  |_|  
 */

﻿using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Symbology;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.IO;
using System.Collections.ObjectModel;

namespace AerialMapping
{
    public partial class MainWindow : Window
    {
        public double m_KmzZoomDelaySec = 3;
        private double currAngle = 0;
        private string m_IdToZoomOn = "";
        private MapView m_MapView;
        KmlLayer kmllayerTest;

        public class MenuItem
        {
            public MenuItem()
            {
                this.Items = new ObservableCollection<MenuItem>();
            }

            public string Title { get; set; }

            public ObservableCollection<MenuItem> Items { get; set; }
        }

        public MainWindow()
        {
            InitializeComponent();
            m_MapView.MaxScale = 2100; // set the maximum zoom value
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

        private void BaseMapView_LayerLoaded(object sender, LayerLoadedEventArgs e)
        {
            if (e.LoadError != null)
            {
                Debug.WriteLine(string.Format("(MainWindows{BaseMapView_LayerLoaded}) Error while loading layer : {0} - {1}", e.Layer.ID, e.LoadError.Message));
                return;
            }
            if (m_IdToZoomOn != "" && e.Layer.ID == m_IdToZoomOn) 
            {
                m_IdToZoomOn = "";
                ((MapView)sender).SetViewAsync(((KmlLayer)e.Layer).RootFeature.Viewpoint, TimeSpan.FromSeconds(m_KmzZoomDelaySec));
            }
            AddLayerToTree(e.Layer);
        }

        private void BaseMapView_Initialized(object sender, EventArgs e)
        {
            m_MapView = (MapView) sender;
            LoadKml("../../../../Data/SampleOne/TestData.kml", true, true);
        }

        private void bOpenFileDialog_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Map Data|*.kml;*.kmz";
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                LoadKml(filePath, true, false);
            }
        }

        // The following are buttons for rotation
        private void bRotateLeft_Click(object sender, RoutedEventArgs e)
        {
            currAngle = (currAngle += 10) % 360;
            // apply the rotation to the map
            m_MapView.SetRotationAsync(currAngle);
        }

        private void bRotateDefault_Click(object sender, RoutedEventArgs e)
        {
            // apply the rotation to the map
            currAngle = 0;
            m_MapView.SetRotationAsync(currAngle);
        }
        private void bRotateRight_Click(object sender, RoutedEventArgs e)
        {
            currAngle = (currAngle -= 10) % 360;
            // apply the rotation to the map
            m_MapView.SetRotationAsync(currAngle);
        }

        // The following are buttons for zoom
        private void bZoomIn_Click(object sender, RoutedEventArgs e)
        {
            m_MapView.ZoomAsync(1.2);
            bZoomSlider.Value = m_MapView.Scale;
        }

        private void bZoomOut_Click(object sender, RoutedEventArgs e)
        {
            m_MapView.ZoomAsync(0.8);
            bZoomSlider.Value = m_MapView.Scale;
        }

        // The following is for zoom slider action
        private void bZoomSlider_Click(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double zoomScale = e.NewValue;
            m_MapView.ZoomToScaleAsync(zoomScale);
        }

        public void AddLayerToTree(Layer layer)
        {
            MenuItem root = new MenuItem() { Title = "Test" };
            root.Items.Add(new MenuItem() { Title = "Test2" });
            LayerView.Items.Add(root);
        }
        
        
        // The following is for the time slider
        private void bTimeSlider_Click(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // this is for testing only.
            if (e.NewValue == 3) m_MapView.Map.Layers.Add(kmllayerTest);
            else if (e.NewValue == 2) m_MapView.Map.Layers.Remove(kmllayerTest);
           // m_MapView.Map.Layers.Move(layerVectorThing[e.NewValue], 0);
        }

        // This will update the time slider with new range and tick marks
        private void updateTimeSlider(int numLayers, int currLayer)
        {
            bTimeSlider.Maximum = numLayers - 1;
            bTimeSlider.Value = currLayer;
        }
    }
}
