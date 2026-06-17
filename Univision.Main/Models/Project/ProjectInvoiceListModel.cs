using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Univision.Core.Models;
using Univision.Core.Models.DTO;

namespace Univision.Main.Models.Project
{
    public class ProjectInvoiceListModel : EntityListViewModel
    {
        public int p_seq { get; set; }
        public List<pjt_invoice_info> list { get; set; }

    }
}