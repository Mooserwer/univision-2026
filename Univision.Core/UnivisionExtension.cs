using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core
{

  public static class UnivisionExtensions
  {


    public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> enumerable, Func<TSource, TKey> keySelector, int descending)
    {
      if (enumerable == null)
      {
        return null;
      }

      if (descending == 2)
      {
        return enumerable.OrderByDescending(keySelector);
      }

      return enumerable.OrderBy(keySelector);
    }

    public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> enumerable, Func<TSource, TKey> keySelector, string descending)
    {
      if (enumerable == null)
      {
        return null;
      }

      if (descending == "DESC")
      {
        return enumerable.OrderByDescending(keySelector);
      }

      return enumerable.OrderBy(keySelector);
    }

    public static IOrderedQueryable<TSource> OrderBy<TSource, TKey>(this IQueryable<TSource> enumerable, System.Linq.Expressions.Expression<Func<TSource, TKey>> keySelector, int descending)
    {
      if (enumerable == null)
      {
        return null;
      }

      if (descending == 2)
      {
        return enumerable.OrderByDescending(keySelector);
      }

      return enumerable.OrderBy(keySelector);
    }

    public static IOrderedQueryable<TSource> OrderBy<TSource, TKey>(this IQueryable<TSource> enumerable, System.Linq.Expressions.Expression<Func<TSource, TKey>> keySelector, string descending)
    {
      if (enumerable == null)
      {
        return null;
      }

      if (descending == "DESC")
      {
        return enumerable.OrderByDescending(keySelector);
      }

      return enumerable.OrderBy(keySelector);
    }


    public static IOrderedEnumerable<TSource> OrderBy<TSource>(this IEnumerable<TSource> enumerable, Func<TSource, IComparable> keySelector1, Func<TSource, IComparable> keySelector2, params Func<TSource, IComparable>[] keySelectors)
    {
      if (enumerable == null)
      {
        return null;
      }

      IEnumerable<TSource> current = enumerable;

      if (keySelectors != null)
      {
        for (int i = keySelectors.Length - 1; i >= 0; i--)
        {
          current = current.OrderBy(keySelectors[i]);
        }
      }

      current = current.OrderBy(keySelector2);

      return current.OrderBy(keySelector1);
    }

    public static IOrderedEnumerable<TSource> OrderBy<TSource>(this IEnumerable<TSource> enumerable, int descending, Func<TSource, IComparable> keySelector, params Func<TSource, IComparable>[] keySelectors)
    {
      if (enumerable == null)
      {
        return null;
      }

      IEnumerable<TSource> current = enumerable;

      if (keySelectors != null)
      {
        for (int i = keySelectors.Length - 1; i >= 0; i--)
        {
          current = current.OrderBy(keySelectors[i], descending);
        }
      }

      return current.OrderBy(keySelector, descending);
    }

    public static IOrderedEnumerable<TSource> OrderBy<TSource>(this IEnumerable<TSource> enumerable, string descending, Func<TSource, IComparable> keySelector, params Func<TSource, IComparable>[] keySelectors)
    {
      if (enumerable == null)
      {
        return null;
      }

      IEnumerable<TSource> current = enumerable;

      if (keySelectors != null)
      {
        for (int i = keySelectors.Length - 1; i >= 0; i--)
        {
          current = current.OrderBy(keySelectors[i], descending);
        }
      }

      return current.OrderBy(keySelector, descending);
    }
  }
}
