using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models
{
  public class PgList<T>
  {
    public int totalCount { get; set; }
    public List<PagingGroup> group_count { get; set; } = new List<PagingGroup>();

    public List<T> Items { get; set; }
  }

  public class PagingGroup
  {
    public int code { get; set; }
    public string name { get; set; }
    public int count { get; set; }
    public string etc1 { get; set; }
  }

    public class PagingList<T>
  {

    public int PageIndex { get; set; }

    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int totalCount { get; set; }
    public List<T> Items { get; set; }

    public PagingList(List<T> items, int count, int pageIndex, int pageSize)
    {
      totalCount = count;
      PageIndex = pageIndex;
      PageSize = pageSize;
      TotalPages = (int)Math.Ceiling(count / (double)pageSize);
      this.Items = new List<T>();
      this.Items.AddRange(items);
    }


    public static async Task<PagingList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
    {
      var count = await source.CountAsync();
      var items = await source.Skip((pageIndex) * pageSize).Take(pageSize).ToListAsync();
      return new PagingList<T>(items, count, pageIndex, pageSize);
    }
  }
}
