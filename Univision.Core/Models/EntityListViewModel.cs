using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Univision.Core.Models
{
    public class EntityListViewModel
    {
        public PagingInfo PagingInfo { get; set; }
    }

    public class PagingInfo
    {
        public long TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages
        {
            get
            {
                var totalPage = (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage);

                return totalPage > 0 ? totalPage : 1; ;
            }
        }

        public PagingInfo()
        {
            ItemsPerPage = 40;
        }
    }

    public class AppPaging
    {
        public const int PageLengthSmall = 5;
        public const int PageLengthMedium = 10;
        public const int PageLength15 = 15;
        public const int PageLengthLarge = 20;
        public const int PageLengthXLarge = 30;
        public const int PageLengthXXLarge = 50;
        public const int PageHundread = 100;
        public const int PageHundread3 = 300;
        public const int PageThousand = 1000;
        public const int Page10Thousand = 10000;

        public const int NbPageSmall = 5;
        public const int NbPageMedium = 9;
    }
}