using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Univision.Core.Models;
using Univision.Core.Models.DTO;
using Univision.Core.Models.DTO.Request;
using Univision.Core.Models.DTO.Response;

namespace Univision.Main.Models.Vacation
{
  public class MyVacationModel : EntityListViewModel
  {
    public VacationSearchModel search { get; set; }

    public List<YearMonthModel> yearList { get; set; }

    public List<uv_user> userList { get; set; }

    public uv_vacation status { get; set; }
    public List<uv_vacation_history> list { get; set; }
  }
}
