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
    public class VehicleMarker : GMarkerGoogle
    {
        public string Name { get; set; }
        public string VehicleType { get; set; }

        public string Description { get; set; }
        new public readonly GMarkerGoogleType Type;
        public VehicleMarker(PointLatLng point, Bitmap bitmap) : base(point, bitmap)
        {
            Bitmap = bitmap;
            base.Size = new Size(bitmap.Width, bitmap.Height);
            base.Offset = new Point(-base.Size.Width / 2, -base.Size.Height);
        }
        public VehicleMarker(PointLatLng point, GMarkerGoogleType type) : base(point, type)
        {
            Type = type;
            if (type != 0)
            {
            }
        }
    }
}
