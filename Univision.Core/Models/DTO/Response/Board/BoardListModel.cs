using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Board
{
    
    public class BoardListSearch 
    {
        
        public int b_type { get; set; }

        public int b_type_sub1 { get; set; }

        public int b_type_sub2 { get; set; }

        public string search_option { get; set; } = "";

        public string search_txt { get; set; } = "";

    }

    public class BoardListModel : EntityListViewModel
    {
        public BoardListSearch search { get; set; }
        //문서게시판 카테고리 리스트
        public List<DocumentTypeModel> DocumentTypeList { get; set; }
        //상단 공지사항 표시용 리스트
        public List<board> TopBoardList { get; set; }
        //일반 게시글 리스트
        public List<board> BoardList { get; set; }
    }
}
