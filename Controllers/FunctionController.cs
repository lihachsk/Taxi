using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using Taxi.Controllers;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Taxi.Controllers
{
    using Taxi.Models;
    public partial class AjaxController : Controller
    {
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

        private Array getErrors(List<string[]> points, JsonModels.data data)
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
        public String getUrl(JsonModels.data data)
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
        public int getDistance(JsonModels.parserItem parther)
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
        public List<String[]> getPoints(JsonModels.parserItem parther)
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
            string responseFromServer = "";
            try
            {
                var request = WebRequest.Create(url);

                request.Credentials = CredentialCache.DefaultCredentials;

                var response = request.GetResponse();
                var responseStatus = ((HttpWebResponse)response).StatusDescription;
                var dataStream = response.GetResponseStream();
                var reader = new StreamReader(dataStream);
                responseFromServer = reader.ReadToEnd();

                reader.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                return "error: " + ex.Message;
            }
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
                    using (taxi db = new taxi())
                    {                     
                        reg = db.addrobj61.Where(x => x.offname == parther && x.worksarea == true)
                        .Select(x => x.regioncode)
                        .First();
                    }
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
                        using (taxi db = new taxi())
                        {
                            cityServerVal = db.addrobj61.Where(x => x.aolevel == 4 && x.regioncode == reg.ToString())
                                .OrderByDescending(x => x.centstatus)
                                .Select(x => x.offname)
                                .First()
                                .ToString().Trim();
                        }
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

        static string GetMd5Hash(string input)
        {
            var md5Hash = MD5.Create();
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        // Verify a hash against a string.
        static bool VerifyMd5Hash(string input, string hash)
        {
            var md5Hash = MD5.Create();
            // Hash the input.
            string hashOfInput = GetMd5Hash(input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
        public static string CapText(Match m)
        {
            string x = m.ToString();
            if (char.IsLower(x[0]))
            {
                return char.ToUpper(x[0]) + x.Substring(1, x.Length - 1);
            }
            return x;
        }
        public IEnumerable<res> getListAdress(String cityName)
        {
            String regioncode = Session["regioncode"].ToString();
            IEnumerable<res> result = null;
            string cityNameVal = Regex.Replace(cityName, "[^-:bА-я0-9/().,]*", "");
            if (cityNameVal.Length > 0)
            {
                Regex rx = new Regex(@"\w+");
                cityNameVal = rx.Replace(cityNameVal, new MatchEvaluator(AjaxController.CapText));
                switch (regioncode)
                {
                    case "Ростов-на-Дону":
                        {
                            using (taxi db = new taxi())
                            {
                                result = db.addrobj61
                                 .Join(
                                     db.addrobj61
                                         .Select(y => new { y.aoguid, y.formalname, y.shortname, y.worksarea, y.aolevel })
                                         .Where(y => y.worksarea == true)
                                         .Where(y => y.aolevel == 4),
                                         x => x.parentguid,
                                         y => y.aoguid,
                                         (x, y) => new res()
                                         {
                                             name = x.formalname.Trim() + " " + x.shortname.Trim() + ".",
                                             city = ", " + y.formalname.Trim(),
                                             order = x.centstatus
                                         })
                                 .OrderByDescending(x => x.order)
                                 .Where(x => x.name.Contains(cityNameVal))
                                 .Take(10)
                                 .ToList();
                            }
                        } break;
                }
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
        public void writeLog(String msg)
        {
            try
            {
                //Добавить таблицу и запись в лог
            }
            catch
            {
            }
        }
        public int checkUser(String tel)
        {
            var parsTel = Regex.Replace(tel, "[^0-9]*", "");
            if (parsTel.Length != 10)
            {
                return -1;
            }
            else
            {
                using (taxi db = new taxi())
                {
                    var user = db.users.Select(x => x.tel == tel).ToArray();
                    if (user.Count() > 0)
                    {
                        return 1;
                    }
                    else
                    {
                        return -2;
                    }
                }
            }
        }
        public int sendMsg(String tel)
        {
            var pass = CreatePassword(5);
            string hashPass = GetMd5Hash(pass);
            var fl = VerifyMd5Hash(pass, hashPass);
            var url = "http://sms.ru/sms/send?api_id=5dd0221a-45f0-7f34-5d9c-4223cdf4ff72&to=7"+tel+"&text=prostotaxi.ru Ваш пароль: "+pass;
            return 1;
        }
    }
}
