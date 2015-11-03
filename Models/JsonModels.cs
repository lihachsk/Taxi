using System;
using System.Web;
using Newtonsoft.Json;
using Help.Models;
namespace Taxi.Models
{
    public class JsonModels
    {
        public class data
        {
            public String city;
            public String comment;
            public pointsItem[] points;
            public data(String text)
            {
                this.Deseriaize(text);
            }
            internal bool Deseriaize(String jsonCode)
            {
                data res = null;
                try
                {
                    res = JsonConvert.DeserializeObject<data>(jsonCode);
                }
                catch (Exception ex)
                {
                    //записать ошибку в лог
                    return false;
                }
                city = res.city;
                comment = res.comment;
                points = res.points;
                return true;
            }
        }
        public class parserItem
        {
            public String[] destination_addresses;
            public String[] origin_addresses;
            public rowsItem[] rows;
            public String status;
            public parserItem(String text)
            {
                this.Deseriaize(text);
            }
            internal bool Deseriaize(String jsonCode)
            {
                parserItem res = null;
                try
                {
                    res = JsonConvert.DeserializeObject<parserItem>(jsonCode);
                }
                catch (Exception ex)
                {
                    //записать ошибку в лог
                    return false;
                }
                destination_addresses = res.destination_addresses;
                origin_addresses = res.origin_addresses;
                rows = res.rows;
                status = res.status;
                return true;
            }
        }
    }
}