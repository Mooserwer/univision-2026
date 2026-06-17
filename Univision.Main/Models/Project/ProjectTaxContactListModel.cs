using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Univision.Core.Models.DTO;

namespace Univision.Main.Models.Project
{
    public class ProjectTaxContactListModel
    {
        public int p_seq { get; set; }
        public List<client_tax_contact> list { get; set;}
    }
}