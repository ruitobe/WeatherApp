using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;

namespace WeatherApp.Models
{
   // WeatherData class shall be public, otherwise, Runtime reports exception:
   // System.Runtime.Serialization.InvalidDataContractException
    
    public class WeatherData
    {
        
        public class RootObject
        {
            public string cod { get; set; }
            public double message { get; set; }
            public City city { get; set; }
            public int cnt { get; set; }
            
            public List<Day> list { get; set; }
        }

        
        public class Coord
        {
            public double lon { get; set; }
            public double lat { get; set; }
        }

        
        public class City
        {
            public int id { get; set; }
            public string name { get; set; }
            public Coord coord { get; set; }
            public string country { get; set; }
            public int population { get; set; }
        }

        
        public class Temp
        {
            private double _day;
            public double day
            {
                get { return _day; }

                set { _day = value - 273.15; }
            }

            private double _min;
            public double min
            {
                get { return _min; }

                set { _min = value - 273.15; }
            }

            private double _max;
            public double max
            {
                get { return _max; }

                set { _max = value - 273.15; }
            }

            private double _night;
            public double night
            {
                get { return _night; }

                set { _night = value - 273.15; }
            }

            private double _eve;
            public double eve
            {
                get { return _eve; }

                set { _eve = value - 273.15; }
            }

            private double _morn;
            public double morn
            {
                get { return _morn; }

                set { _morn = value - 273.15; }
            }

        }

        
        public class Weather
        {
            public int id { get; set; }
            
            public string main { get; set; }
            public string description { get; set; }
            public string icon { get; set; }
        }

       
        public class Day
        {
            //public int dt { get; set; }
            private int _dt;
            public int dt
            {
                get { return _dt; }
                set
                {
                    _dt = value;
                    var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    time = epoch.AddSeconds(value);
                }
            }

            private DateTime _time;
            public DateTime time
            {
                get { return _time; }
                set { _time = value; }
            }

            public string IconPath
            {
                get
                {
                    return "/Icons/" + weather[0].icon + ".png";
                }
            }

            public Temp temp { get; set; }
            public double pressure { get; set; }
            public int humidity { get; set; }
            public List<Weather> weather { get; set; }
            public double speed { get; set; }
            public int deg { get; set; }
            public int clouds { get; set; }
            public double? rain { get; set; }
            public double? snow { get; set; }
        }
    }

    
}
