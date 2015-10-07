using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Symbology;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.IO;

namespace AerialMapping
{
    public partial class MainWindow : Window
    {
        public double m_KmzZoomDelaySec = 3;

        private string m_IdToZoomOn = "";
        private MapView m_MapView;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void LoadKml(string path, bool bZoomTo, bool bRelativePath)
        {
            try
            {
                Uri dataPath = new Uri(path, bRelativePath ? UriKind.Relative : UriKind.Absolute);
                KmlLayer kmllayer = new KmlLayer(dataPath);
                kmllayer.ID = path;
                m_IdToZoomOn = bZoomTo ? path : "";

                m_MapView.Map.Layers.Add(kmllayer);
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
        }

        private void BaseMapView_Initialized(object sender, EventArgs e)
        {
            m_MapView = (MapView) sender;
            LoadKml("../../../../Data/SampleOne/TestData.kml", true, true);
        }
    }
}
