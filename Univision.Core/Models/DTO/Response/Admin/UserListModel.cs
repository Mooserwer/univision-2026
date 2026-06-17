using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Admin
{
    
    public class UserListModel : EntityListViewModel
    {
        //사용자 리스트
        public List<uv_user> UserList { get; set; }
    }
    
}
