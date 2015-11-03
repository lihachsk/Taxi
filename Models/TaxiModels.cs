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
        public virtual DbSet<users> users { get; set; }

    }
    [Table("users", Schema = "path")]
    public partial class users
    {
        [Key]
        [Column(Order = 0)]
        public int idUser { get; set; }
        public string tel { get; set; }
        public string pass { get; set; }
    }
    [Table("orders", Schema = "path")]
    public partial class orders
    {
        [Key]
        [Column(Order = 0)]
        public int idOrder { get; set; }
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