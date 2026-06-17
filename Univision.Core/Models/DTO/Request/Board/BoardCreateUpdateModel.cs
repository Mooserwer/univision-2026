using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Request.Board
{
    public class BoardCreateUpdateModel
    {
        public board data { get; set; } = new board();
        public List<board_file> boardFileList { get; set; } = new List<board_file>();

        public List<board_file> deleteFileList { get; set; } = new List<board_file>();
    }

    
}
