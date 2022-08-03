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
    public class DBLayer
    {
        private string _connectionString;
        private SqlConnection _connection;
        public DBLayer()
        {
            _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            _connection = new SqlConnection(_connectionString);
        }

        public void RefreshDb(List<VehicleMarker> initList, List<VehicleMarker> newList)
        {
            List<VehicleMarker> listToAdd = new List<VehicleMarker>();
            List<VehicleMarker> listToDelete = new List<VehicleMarker>();
            if (initList.Count != newList.Count)
            {
                throw new NotImplementedException(); // todo: add marker
            }
            _connection.Open();
            string sql;

            for (int i = 0; i < initList.Count; i++) // todo: optimize (скорее всего, это не самый эффективный алгоритм, сложность O(N^2), где N = количество элементов в одном списке)
            {
                double lng = initList[i].Position.Lng;
                double lat = initList[i].Position.Lat;
                bool toDelete = true;
                for (int j = 0; j < newList.Count; j++)
                {
                    if (lng == newList[j].Position.Lng && lat == newList[j].Position.Lat)
                    {
                        toDelete = false;
                        break;
                    }
                }
                if (toDelete)
                {
                    listToDelete.Add(initList[i]);
                }
            }// Если маркер с некоторыми координатами есть в начальном списке, но в обновлённом его нет, значит его нужно удалить из БД

            for (int i = 0; i < newList.Count; i++) // todo: optimize (скорее всего, это не самый эффективный алгоритм, сложность O(N^2), где N = количество элементов в одном списке)
            {
                double lng = newList[i].Position.Lng;
                double lat = newList[i].Position.Lat;
                bool toAdd = true;
                for (int j = 0; j < initList.Count; j++)
                {
                    if (lng == initList[j].Position.Lng && lat == initList[j].Position.Lat)
                    {
                        toAdd = false;
                        break;
                    }
                }
                if (toAdd)
                {
                    listToAdd.Add(newList[i]);
                }
            }//Если в изначальном списке маркера не было, но он появился в обновлённом, то его нужно добавить в БД

            foreach (VehicleMarker vm in listToDelete)
            {
                sql = $"DELETE FROM [dbo].[vehicleCoords] WHERE name = '{vm.Name}' AND type = '{vm.VehicleType}' AND additional_info = '{vm.Description}' AND lat = {vm.Position.Lat} AND lng = {vm.Position.Lng};";
                SqlCommand command = new SqlCommand(sql, _connection);
                command.ExecuteNonQuery();
            }
            foreach (VehicleMarker vm in listToAdd)
            {
                sql = $@"INSERT INTO [vehicleCoords] 
                      ([name],[type],[additional_info],[lat],[lng])
                      VALUES ('{vm.Name}','{vm.VehicleType}','{vm.Description}',{vm.Position.Lat},{vm.Position.Lng})";
                SqlCommand command = new SqlCommand(sql, _connection);
                command.ExecuteNonQuery();
            }
            _connection.Close();

        }
        public List<VehicleMarker> GetVehicleMarkers()
        {
            List<VehicleMarker> result = new List<VehicleMarker>();
            _connection.Open();
            string sql = "SELECT * FROM vehicleCoords;";
            SqlCommand command = new SqlCommand(sql, _connection);
            SqlDataReader dataReader = command.ExecuteReader();
            int count = 0;
            while (dataReader.Read())
            {
                try
                {
                    VehicleMarker marker = new VehicleMarker(new PointLatLng(Convert.ToDouble(dataReader[3].ToString()), Convert.ToDouble(dataReader[4].ToString())), GMarkerGoogleType.red);
                    marker.Name = dataReader[0].ToString();
                    marker.VehicleType = dataReader[1].ToString();
                    marker.Description = dataReader[2].ToString();
                    marker.ToolTipText = marker.Name.Trim(' ') + $"\nWith type: ({marker.VehicleType.Trim(' ')})\n\n" + marker.Description.Trim(' ');
                    marker.Tag = count;
                    result.Add(marker);
                }
                catch
                {

                }
                finally
                {
                    count++;
                }
            }
            dataReader.Close();
            _connection.Close();
            return result;
        }
    }
}
