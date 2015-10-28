using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using Taxi.Controllers;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO;

namespace Taxi.Controllers
{
    using Taxi.Models;
    public partial class AjaxController : Controller
    {
        taxi db = new taxi();
        private int totalError(Array array)
        {
            var row = array.Rank;
            var col = array.Length/2;
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    var tmpItem = Convert.ToInt32(array.GetValue(i, j));
                    if (tmpItem != 0)
                    {
                        return 0;
                    }
                }
            }
            return 1;
        }

        private Array getErrors(List<string[]> points, TaxiModels.data data)
        {
            var it = points.Count;
            int[,] errors = new int[it, 3];
            for (int i = 0; i < it; i++)
            {
                var count = points[i].Length;
                if(count>1)
                {
                    var parserStr = points[i][0];
                    var index = parserStr.IndexOf(" ");
                    if(index>0)
                        parserStr = parserStr.Substring(0, parserStr.IndexOf(" "));
                    if(data.points[i].street.Contains(parserStr))
                    {
                        errors[i,0] = 0;
                    }
                    else
                    {
                        errors[i, 0] = 1;//проблемы с улицей
                    }
                    if(count>2)
                    {
                        if(data.points[i].streetNum.ToUpper() == points[i][1])
                        {
                            errors[i, 1] = 0;
                        }
                        else
                        {
                            errors[i, 1] = 2;
                        }
                    }
                    else
                    {
                        errors[i, 1] = 2;//проблемы с номером дома
                    }
                }
                else
                {
                    errors[i, 0] = 1;
                    errors[i, 1] = 2;
                }
                errors[i, 2] = 0;
            }
            return errors;
        }

        private double getCost(int distance)
        {
            if (0 < distance && distance < 2000)
            {
                return 70;
            }
            else
            {
                if (distance <= 0)
                {
                    return 0;
                }
                else
                {
                    if (distance > 2000)
                    {
                        var cost = Math.Ceiling((double)(distance - 2000) / 1000) * 15 + 70;
                        return cost;
                    }
                }
            }
            return -1;
        }
        public String getUrl(TaxiModels.data data)
        {
            var destinations = "";
            var origins = "";
            string url = "https://maps.googleapis.com/maps/api/distancematrix/json?language=ru";
            var length = data.points.Length;
            if(length<2)
            {
                return null;
            }
            for (int i = 0; i < length && i < 10; i++)
            {
                if(i%2==0)
                {
                    destinations = destinations + "Россия," + data.points[i].street + "," + data.points[i].streetNum;
                    if(length-i>2)
                    {
                        destinations = destinations + "|";
                    }
                }
                else
                {
                    origins = origins + "Россия," + data.points[i].street + "," + data.points[i].streetNum;
                    if(length-i>2)
                    {
                        origins = origins + "|";
                    }
                }
            }
            return url + "&origins=" + origins + "&destinations=" + destinations;
        }
        public int getDistance(TaxiModels.parserItem parther)
        {
            var distance = 0;
            var row = parther.rows.Count();
            for (int i = 0; i < row; i++)
            {
                var col = parther.rows[i].elements.Count();
                for (int j = i; j < i + 2 && j < col; j++)
                {
                    if (parther.rows[i].elements[j].status == "OK")
                    {
                        distance = distance + parther.rows[i].elements[j].distance.value;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            return distance;
        }
        public List<String[]> getPoints(TaxiModels.parserItem parther)
        {
            List<String[]> points = new List<String[]>();
            var row = parther.destination_addresses.Count();
            var col = parther.origin_addresses.Count();
            bool fl = true;
            for (int i = 0; i < row; i++)
            {
                if(fl)
                {
                    points.Add(Regex.Split(parther.destination_addresses[i--], ", "));
                    fl = false;
                }
                else
                {
                    if(i<col)
                    {
                        points.Add(Regex.Split(parther.origin_addresses[i], ", "));
                        fl = true;
                    }
                }
            }
            return points;
        }
        public string getRoutParametrs(string url)
        {
            var request = WebRequest.Create(url);

            request.Credentials = CredentialCache.DefaultCredentials;

            var response = request.GetResponse();
            var responseStatus = ((HttpWebResponse)response).StatusDescription;
            var dataStream = response.GetResponseStream();
            var reader = new StreamReader(dataStream);
            var responseFromServer = reader.ReadToEnd();
            var parther = TaxiModels.Serializer.Deseriaize(responseFromServer);

            reader.Close();
            response.Close();

            return responseFromServer;
        }
        public String getCity(String city)
        {
            if (city != null)
            {
                var parther = Regex.Replace(city, "[^:bА-я0-9/-]*", "");
                String reg = null;
                try
                {
                    reg = db.ADDROBJ.Where(x => x.OFFNAME == parther && x.WORKSAREA == true)
                        .Select(x => x.REGIONCODE)
                        .First();
                }
                catch (Exception ex)
                {
                    //log ex
                    reg = null;
                }
                if (reg != null)
                {
                    String cityServerVal = null;
                    try
                    {
                        cityServerVal = db.ADDROBJ.Where(x => x.AOLEVEL == 4 && x.REGIONCODE == reg.ToString())
                            .OrderByDescending(x => x.CENTSTATUS)
                            .Select(x => x.OFFNAME)
                            .First()
                            .ToString().Trim();
                        Session["regioncode"] = cityServerVal;
                        return cityServerVal;
                    }
                    catch (Exception ex)
                    {
                        //error log
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }
        public class res
        {
            public String name { get; set; }
            public String city { get; set; }
            public double? order { get; set; }
        }
        public IEnumerable<res> getListAdress(String cityName)
        {
            String regioncode = Session["regioncode"].ToString();
            IEnumerable<res> result = null;
            var cityNameVal = Regex.Replace(cityName, "[^-:bА-я0-9/().,]*", "");
            switch(regioncode)
            {
                case "Ростов-на-Дону": {
                result = db.ADDROBJ61
                 .Join(
                     db.ADDROBJ61
                         .Select(y => new { y.AOGUID, y.FORMALNAME, y.SHORTNAME, y.WORKSAREA, y.AOLEVEL })
                         .Where(y => y.WORKSAREA == true)
                         .Where(y => y.AOLEVEL == 4),
                         x => x.PARENTGUID,
                         y => y.AOGUID,
                         (x, y) => new res()
                         {
                             name = x.FORMALNAME.Trim() + " " + x.SHORTNAME.Trim() + ".",
                             city = ", " + y.FORMALNAME.Trim(),
                             order = x.CENTSTATUS
                         })
                 .OrderByDescending(x => x.order)
                 .Where(x => x.name.Contains(cityNameVal))
                 .Take(10)
                 .ToList();
                } break;
            }
            return result;
        }
        public String getStatus(String city)
        {
            if (city != "")
            {
                return "OK";
            }
            else
            {
                return "FAIL";
            }
        }
    }
}
