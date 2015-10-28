using System;
using System.Web;
using Newtonsoft.Json;

namespace Taxi.Models
{
    public class TaxiModels : IHttpModule
    {
        /// <summary>
        /// You will need to configure this module in the Web.config file of your
        /// web and register it with IIS before being able to use it. For more information
        /// see the following link: http://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpModule Members

        public void Dispose()
        {
            //clean-up code here.
        }

        public void Init(HttpApplication context)
        {
            // Below is an example of how you can handle LogRequest event and provide 
            // custom logging implementation for it
            context.LogRequest += new EventHandler(OnLogRequest);
        }

        #endregion

        public void OnLogRequest(Object source, EventArgs e)
        {
            //custom logging logic can go here
        }
        public class pointsItem
        {
            public String street;
            public String streetNum;
            public String houseNum;
        }
        public class data
        {
            public String city;
            public String comment;
            public pointsItem[] points;
        }
        public class dataModel
        {
            public data data;
        }
        public class coord
        {
            public String x { get; set; }
            public String y { get; set; }
        }

        public class coordsRegister
        {
            public coord coordsA { get; set; }
            public coord coordsB { get; set; }
            public coordsRegister() {
                this.coordsA = new coord();
                this.coordsB = new coord();
            }
        }

        public class distanceItem
        {
            public String text;
            public int value;
        }

        public class elementsElement
        {
            public distanceItem distance;
            public distanceItem duration;
            public String status;

        }
        public class rowsItem
        {
            public elementsElement[] elements;
        }
        public class parserItem
        {
            public String[] destination_addresses;
            public String[] origin_addresses;
            public rowsItem[] rows;
            public String status;
        }
        public static class Serializer
        {
            internal static parserItem Deseriaize(String jsonCode)
            {
                parserItem res = null;
                try
                {
                    res = JsonConvert.DeserializeObject<parserItem>(jsonCode);
                }
                catch (Exception ex)
                {
                    //записать ошибку в лог
                }
                return res;
            }
            internal static data DeseriaizeData(String jsonCode)
            {
                data res = null;
                try
                {
                    res = JsonConvert.DeserializeObject<data>(jsonCode);
                }
                catch (Exception ex)
                {
                    //записать ошибку в лог
                }
                return res;
            }
        }
    }
}
