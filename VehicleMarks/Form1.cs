using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET.MapProviders;
using GMap.NET;
using GMap;
using GMap.NET.WindowsForms;
using GMap.NET.ObjectModel;
using GMap.NET.WindowsForms.Markers;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows;
using GMap.NET.WindowsForms.Properties;
namespace VehicleMarks
{
    public partial class Form1 : Form
    {
        DBLayer _layer;
        List<VehicleMarker> _initMarkerList;
        List<VehicleMarker> _currentMarkerList;
        public Form1()
        {
           
            InitializeComponent();
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            DefaultSettings();  

        }
        private bool isLeftButtonDown = false;
        private VehicleMarker currentMarker = null;
        private void gMap_MouseDown(object sender, MouseEventArgs e)
        {
            
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                isLeftButtonDown = true;
                if(currentMarker != null)
                {
                        if (_currentMarkerList.Contains(currentMarker))
                            _currentMarkerList.Remove(currentMarker);
                }
            }
        }
        private void DefaultSettings()
        {
            gMap.Overlays.Clear();
            _layer = new DBLayer();
            _initMarkerList = _layer.GetVehicleMarkers();
            gMap.ShowCenter = false;
            gMap.IgnoreMarkerOnMouseWheel = true;
            _currentMarkerList = _layer.GetVehicleMarkers();
            GMapOverlay markers = new GMapOverlay("markers");
            foreach (VehicleMarker marker in _currentMarkerList)
            {
                markers.Markers.Add(marker);
            }
            gMap.Overlays.Add(markers);
            RefreshMap();
            gMap.MouseWheelZoomEnabled = true;
            gMap.MaxZoom = 20;
            gMap.MinZoom = 0;
            gMap.DragButton = MouseButtons.Left;
            gMap.MapProvider = GMapProviders.GoogleMap;
            gMap.OnMapDrag += (() =>
            {
                txtLat.Text = gMap.Position.Lat.ToString();
                txtLong.Text = gMap.Position.Lng.ToString();
            });
            gMap.OnMapZoomChanged += (() =>
            {
                txtZoom.Text = gMap.Zoom.ToString();
            });
        }
        private void button1_Click(object sender, EventArgs e)
        {
            double latitude;
            Double.TryParse(txtLat.Text, out latitude);
            double longitude;
            Double.TryParse(txtLong.Text, out longitude);
            gMap.Position = new PointLatLng(latitude, longitude);
            double zoom;
            Double.TryParse(txtZoom.Text, out zoom);
            gMap.Zoom = zoom;
            RefreshMap();

        }
        private void Form1_OnFormClosing(Object sender, FormClosingEventArgs e)
        {
            RefreshDb();
            Application.Exit();
        }
        private void RefreshMap()
        {
            gMap.Zoom-=0.01;
            gMap.Zoom+=0.01;
        }
        private void RefreshDb()
        {
            _layer.RefreshDb(_initMarkerList, _currentMarkerList);
            DefaultSettings();
            RefreshMap();
        }
        private void gMap_Load(object sender, EventArgs e)
        {
            RefreshMap();
        }
        private void gMap_OnMarkerClick_1(GMapMarker item, MouseEventArgs e)
        {

        }
        private void gMap_MouseUp(object sender, MouseEventArgs e)
        {
            bool reload = false;
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (currentMarker != null)
                {
                    if (!_currentMarkerList.Contains(currentMarker))
                        _currentMarkerList.Add(currentMarker);
                    currentMarker = null;
                    reload = true;
                }
                isLeftButtonDown = false;
                
            }
            if(reload)
                RefreshDb();
        }
        private void gMap_OnMapClick(PointLatLng pointClick, MouseEventArgs e)
        {

        }
        private void gMap_MouseMove(object sender, MouseEventArgs e)
        {
            txtLat.Text = gMap.FromLocalToLatLng(e.X, e.Y).Lat.ToString();
            txtLong.Text = gMap.FromLocalToLatLng(e.X, e.Y).Lng.ToString();
            if(isLeftButtonDown && currentMarker!=null)
            {
                currentMarker.Position = gMap.FromLocalToLatLng(e.X, e.Y);
            }
        }
        private void gMap_OnMarkerEnter(GMapMarker item)
        {
            if (currentMarker == null || currentMarker == (VehicleMarker)item)
            {
                currentMarker = (VehicleMarker)item;
                
            }

        }
        private void gMap_OnMarkerLeave(GMapMarker item)
        {
            if (currentMarker == (VehicleMarker)item && item is VehicleMarker)
            {
                currentMarker = null;
            }
        }
    }
}