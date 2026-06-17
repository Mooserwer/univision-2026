using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Board
{

    public class DocumentTypeModel
    {

        public string text { get; set; }
        public string href { get; set; }
        public bool is_select { get; set; }
        public List<string> tags  { get; set; }
        public List<DocumentTypeModel> nodes { get; set; }
    }
}
