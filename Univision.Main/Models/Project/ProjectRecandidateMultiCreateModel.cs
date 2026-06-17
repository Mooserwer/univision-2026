using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Univision.Core.Models;
using Univision.Core.Models.DTO;
using Univision.Core.Models.DTO.Response.Project;

namespace Univision.Main.Models.Project
{
  public class ProjectRecandidateMultiCreateModel : EntityListViewModel
  {
    public int p_seq { get; set; }
    public List<pjt_recandidate_history> list { get; set; } = new List<pjt_recandidate_history>();
  }

  public class ProjectRecandidateMultiMoveModel : EntityListViewModel
  {
    public int p_seq { get; set; }
    public int p_seq_target { get; set; }
    public int is_new { get; set; }
    public List<pjt_recandidate_history> list { get; set; } = new List<pjt_recandidate_history>();
  }

}