using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Api
{
    public class ResponseAlramList
    {
        public List<ResponseAlramModel> list { get; set; }
    }

    public class ResponseAlramModel
    {
        public int am_seq { get; set; }
        public string message { get; set; }
        public string href_url { get; set; }
        public DateTime create_dt { get; set; }

        public int uv_seq { get; set; }
        public int is_read { get; set; }
        public DateTime? read_date { get; set; }
    }

}
