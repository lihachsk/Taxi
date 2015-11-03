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
namespace Taxi
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;

    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;

    public partial class taxi : DbContext
    {
        public taxi() : base(nameOrConnectionString: "taxi") { }

        public virtual DbSet<addrobj> addrobj { get; set; }
        public virtual DbSet<addrobj61> addrobj61 { get; set; }
        public virtual DbSet<orders> orders { get; set; }

    }
    [Table("orders", Schema = "path")]
    public partial class orders
    {
        [Key]
        [Column(Order = 0)]
        public int id { get; set; }
        public DateTime timeOrder { get; set; }
        public DateTime time { get; set; }
        public string comment { get; set; }
        public float cost { get; set; }
        public int pointid { get; set; }
        public int addid { get; set; }
        public int status { get; set; }

    }
    [Table("addrobj", Schema = "path")]
    public partial class addrobj
    {
        [Key]
        [Column(Order = 0)]
        public string aoguid { get; set; }
        public Nullable<double> actstatus { get; set; }
        public string aoid { get; set; }
        public Nullable<double> aolevel { get; set; }
        public string areacode { get; set; }
        public string autocode { get; set; }
        public Nullable<double> centstatus { get; set; }
        public string citycode { get; set; }
        public string code { get; set; }
        public Nullable<double> currstatus { get; set; }
        public Nullable<System.DateTime> enddate { get; set; }
        public string formalname { get; set; }
        public string ifnsfl { get; set; }
        public string ifnsul { get; set; }
        public string nextid { get; set; }
        public string offname { get; set; }
        public string okato { get; set; }
        public string oktmo { get; set; }
        public Nullable<double> operstatus { get; set; }
        public string parentguid { get; set; }
        public string placecode { get; set; }
        public string plaincode { get; set; }
        public string postalcode { get; set; }
        public string previd { get; set; }
        public string regioncode { get; set; }
        public string shortname { get; set; }
        public Nullable<System.DateTime> startdate { get; set; }
        public string streetcode { get; set; }
        public string terrifnsfl { get; set; }
        public string terrifnsul { get; set; }
        public Nullable<System.DateTime> updatedate { get; set; }
        public string ctarcode { get; set; }
        public string extrcode { get; set; }
        public string sextcode { get; set; }
        public Nullable<double> livestatus { get; set; }
        public string normdoc { get; set; }
        public bool worksarea { get; set; }
    }
    [Table("addrobj61", Schema = "path")]
    public partial class addrobj61
    {
        [Key]
        [Column(Order = 0)]
        public string aoguid { get; set; }
        public Nullable<double> actstatus { get; set; }
        public string aoid { get; set; }
        public Nullable<double> aolevel { get; set; }
        public string areacode { get; set; }
        public string autocode { get; set; }
        public Nullable<double> centstatus { get; set; }
        public string citycode { get; set; }
        public string code { get; set; }
        public Nullable<double> currstatus { get; set; }
        public Nullable<System.DateTime> enddate { get; set; }
        public string formalname { get; set; }
        public string ifnsfl { get; set; }
        public string ifnsul { get; set; }
        public string nextid { get; set; }
        public string offname { get; set; }
        public string okato { get; set; }
        public string oktmo { get; set; }
        public Nullable<double> operstatus { get; set; }
        public string parentguid { get; set; }
        public string placecode { get; set; }
        public string plaincode { get; set; }
        public string postalcode { get; set; }
        public string previd { get; set; }
        public string regioncode { get; set; }
        public string shortname { get; set; }
        public Nullable<System.DateTime> startdate { get; set; }
        public string streetcode { get; set; }
        public string terrifnsfl { get; set; }
        public string terrifnsul { get; set; }
        public Nullable<System.DateTime> updatedate { get; set; }
        public string ctarcode { get; set; }
        public string extrcode { get; set; }
        public string sextcode { get; set; }
        public Nullable<double> livestatus { get; set; }
        public string normdoc { get; set; }
        public bool worksarea { get; set; }
    }
}