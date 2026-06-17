using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Univision.Main.Models.KONAN
{
    public class Location
    {
        public string netaddr { get; set; }
        public string volume { get; set; }
        public string table { get; set; }
        public int rowid { get; set; }
    }

    public class Row<T>
    {
        public T fields { get; set; }
        public Location location { get; set; }
        public int copyof { get; set; }
        public List<object> sortkey { get; set; }
    }

    public class Result<T>
    {
        public int total_count { get; set; }
        public List<Row<T>> rows { get; set; }
    }

    public class EntityKonanListModel<T>
    {
        public string status { get; set; }
        public string message { get; set; }
        public Result<T> result { get; set; }
    }
}