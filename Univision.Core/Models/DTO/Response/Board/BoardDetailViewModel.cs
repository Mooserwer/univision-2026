using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Board
{
    public class BoardDetailViewModel
    {
        public board data { get; set; }
        public List<board_file> boardFileList { get; set; }
    }

    public class BoardReplyList : EntityListViewModel
    {
        public int b_seq { get; set; }
        public List<board_reply> ReplyList { get; set; }
    }
}
