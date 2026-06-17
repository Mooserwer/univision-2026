using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Univision.Core.Models;
using Univision.Main.Models;

namespace Univision.Main.HtmlHelpers
{
    public static class PagingHelper
    {

        public static MvcHtmlString PageAjaxLinks(this HtmlHelper html, PagingInfo pagingInfo, string rawUrl, Func<int, string> pageUrl)
        {
            var prevPage = pagingInfo.CurrentPage == 1 ? 1 : pagingInfo.CurrentPage - 1;
            var nextPage = pagingInfo.CurrentPage == pagingInfo.TotalPages ? pagingInfo.TotalPages : pagingInfo.CurrentPage + 1;

            var pageInter = (int)Math.Floor((double)(AppPaging.NbPageMedium - 1) / 2);
            int minPage = 0,
                corrMin = 0,
                maxPage = 0,
                corrMax = 0;

            if (pagingInfo.CurrentPage > pageInter)
            {
                minPage = pagingInfo.CurrentPage - pageInter;
            }
            else
            {
                minPage = 1;
                corrMax = pageInter - pagingInfo.CurrentPage + 1;
            }


            if (pagingInfo.CurrentPage + pageInter < pagingInfo.TotalPages)
            {
                maxPage = pagingInfo.CurrentPage + pageInter;
            }
            else
            {
                maxPage = pagingInfo.TotalPages;
                corrMin = pagingInfo.TotalPages - (pagingInfo.CurrentPage + pageInter);
            }
            if (corrMin > 0)
                minPage += corrMin;
            if (corrMax > 0)
                maxPage += corrMax;

            maxPage = Math.Min(maxPage, pagingInfo.TotalPages);

            TagBuilder div = new TagBuilder("div");
            div.AddCssClass("row col-md-12 justify-content-center");

            TagBuilder nav = new TagBuilder("nav");

            TagBuilder ul = new TagBuilder("ul");
            ul.AddCssClass("pagination pagination-no-border");

            TagBuilder liF = new TagBuilder("li");
            liF.AddCssClass("page-item");

            TagBuilder aF = new TagBuilder("a");
            aF.Attributes.Add("aria-label", "Previous");
            aF.AddCssClass("page-link");
            

            if (maxPage > 1)
                aF.MergeAttribute("href", _getNewUrl_NoDecode(pageUrl(prevPage), rawUrl));

            TagBuilder sF = new TagBuilder("span");
            sF.Attributes.Add("aria-hidden", "true");
            sF.InnerHtml = "&laquo;";

            aF.InnerHtml = sF.ToString();

            liF.InnerHtml = aF.ToString();
            if (pagingInfo.CurrentPage == 1)
            {
                liF.AddCssClass("disabled");
            }
            ul.InnerHtml += liF.ToString();

            for (int i = minPage; i <= maxPage; i++)
            {
                TagBuilder li = new TagBuilder("li");
                li.AddCssClass("page-item");

                TagBuilder a = new TagBuilder("a");
                a.InnerHtml = i.ToString();
                a.AddCssClass("page-link");

                if (maxPage > 1)
                    a.MergeAttribute("href", _getNewUrl_NoDecode(pageUrl(i), rawUrl));

                if (i == pagingInfo.CurrentPage)
                {
                    li.AddCssClass("active");

                    TagBuilder sN = new TagBuilder("span");
                    sN.AddCssClass("sr-only");
                    sN.InnerHtml = "(current)";

                    a.InnerHtml += sN.ToString();
                }
                
                li.InnerHtml = a.ToString();
                ul.InnerHtml += li.ToString();
            }

            TagBuilder liL = new TagBuilder("li");
            liL.AddCssClass("page-item");

            TagBuilder aL = new TagBuilder("a");
            aL.AddCssClass("page-link");
            aL.Attributes.Add("aria-label", "Next");

            TagBuilder sL = new TagBuilder("span");
            sL.Attributes.Add("aria-hidden", "true");
            sL.InnerHtml = "&raquo;";

            aL.InnerHtml = sL.ToString();

            if (maxPage > 1)
                aL.MergeAttribute("href", _getNewUrl_NoDecode(pageUrl(nextPage), rawUrl));
            liL.InnerHtml = aL.ToString();
            if (pagingInfo.CurrentPage == pagingInfo.TotalPages)
            {
                liL.AddCssClass("disabled");
            }
            ul.InnerHtml += liL.ToString();
            nav.InnerHtml = ul.ToString();

            div.InnerHtml = nav.ToString();

            return MvcHtmlString.Create(div.ToString());

        }

        public static MvcHtmlString PageAjaxLinksWithFunction(this HtmlHelper html, PagingInfo pagingInfo, string rawUrl, Func<int, string> pageUrl)
        {
            var prevPage = pagingInfo.CurrentPage == 1 ? 1 : pagingInfo.CurrentPage - 1;
            var nextPage = pagingInfo.CurrentPage == pagingInfo.TotalPages ? pagingInfo.TotalPages : pagingInfo.CurrentPage + 1;

            var pageInter = (int)Math.Floor((double)(AppPaging.NbPageMedium - 1) / 2);
            int minPage = 0,
                corrMin = 0,
                maxPage = 0,
                corrMax = 0;

            if (pagingInfo.CurrentPage > pageInter)
            {
                minPage = pagingInfo.CurrentPage - pageInter;
            }
            else
            {
                minPage = 1;
                corrMax = pageInter - pagingInfo.CurrentPage + 1;
            }


            if (pagingInfo.CurrentPage + pageInter < pagingInfo.TotalPages)
            {
                maxPage = pagingInfo.CurrentPage + pageInter;
            }
            else
            {
                maxPage = pagingInfo.TotalPages;
                corrMin = pagingInfo.TotalPages - (pagingInfo.CurrentPage + pageInter);
            }
            if (corrMin > 0)
                minPage += corrMin;
            if (corrMax > 0)
                maxPage += corrMax;

            maxPage = Math.Min(maxPage, pagingInfo.TotalPages);

            TagBuilder div = new TagBuilder("div");
            div.AddCssClass("row col-md-12 justify-content-center paginationDiv");
            div.Attributes.Add("style", "overflow-x:auto");

            TagBuilder nav = new TagBuilder("nav");

            TagBuilder ul = new TagBuilder("ul");
            ul.AddCssClass("pagination pagination-no-border");

            TagBuilder liF = new TagBuilder("li");
            liF.AddCssClass("page-item");

            TagBuilder aF = new TagBuilder("a");
            aF.Attributes.Add("aria-label", "Previous");
            aF.AddCssClass("page-link");


            if (maxPage > 1)
                aF.MergeAttribute("href", pageUrl(prevPage));

            TagBuilder sF = new TagBuilder("span");
            sF.Attributes.Add("aria-hidden", "true");
            sF.InnerHtml = "&laquo;";

            aF.InnerHtml = sF.ToString();

            liF.InnerHtml = aF.ToString();
            if (pagingInfo.CurrentPage == 1)
            {
                liF.AddCssClass("disabled");
            }
            ul.InnerHtml += liF.ToString();

            for (int i = minPage; i <= maxPage; i++)
            {
                TagBuilder li = new TagBuilder("li");
                li.AddCssClass("page-item");

                TagBuilder a = new TagBuilder("a");
                a.InnerHtml = i.ToString();
                a.AddCssClass("page-link");

                if (maxPage > 1)
                    a.MergeAttribute("href", pageUrl(i));

                if (i == pagingInfo.CurrentPage)
                {
                    li.AddCssClass("active");

                    TagBuilder sN = new TagBuilder("span");
                    sN.AddCssClass("sr-only");
                    sN.InnerHtml = "(current)";

                    a.InnerHtml += sN.ToString();
                }

                li.InnerHtml = a.ToString();
                ul.InnerHtml += li.ToString();
            }

            TagBuilder liL = new TagBuilder("li");
            liL.AddCssClass("page-item");

            TagBuilder aL = new TagBuilder("a");
            aL.AddCssClass("page-link");
            aL.Attributes.Add("aria-label", "Next");

            TagBuilder sL = new TagBuilder("span");
            sL.Attributes.Add("aria-hidden", "true");
            sL.InnerHtml = "&raquo;";

            aL.InnerHtml = sL.ToString();

            if (maxPage > 1)
                aL.MergeAttribute("href", pageUrl(nextPage));
            liL.InnerHtml = aL.ToString();
            if (pagingInfo.CurrentPage == pagingInfo.TotalPages)
            {
                liL.AddCssClass("disabled");
            }
            ul.InnerHtml += liL.ToString();
            nav.InnerHtml = ul.ToString();

            div.InnerHtml = nav.ToString();

            return MvcHtmlString.Create(div.ToString());

        }

        private static string _getNewUrl(string pageUrl, string oldUrl)
        {
            var pagingParams = pageUrl.GetParams();
            string newUrl = oldUrl;
            foreach (string key in pagingParams)
            {
                newUrl = newUrl.SetUrlParameter(key, pagingParams[key]);
            }

            Uri uri = new Uri(newUrl);

            newUrl = uri.AbsolutePath + HttpUtility.UrlDecode(uri.Query);
            return newUrl;
        }

        private static string _getNewUrl_NoDecode(string pageUrl, string oldUrl)
        {
            var pagingParams = pageUrl.GetParams();
            string newUrl = oldUrl;
            foreach (string key in pagingParams)
            {
                newUrl = newUrl.SetUrlParameter(key, pagingParams[key]);
            }

            Uri uri = new Uri(newUrl);

            newUrl = uri.AbsolutePath + uri.Query;
            return newUrl;
        }
    }

    public static class QueryUtilies
    {
        public static NameValueCollection GetParams(this string input)
        {
            if (String.IsNullOrEmpty(input))
            {
                return new NameValueCollection();
            }

            int indexOf = input.IndexOf("?");
            if (indexOf < 0)
            {
                return new NameValueCollection();
            }

            return HttpUtility.ParseQueryString(input.Substring(indexOf));
        }
        public static Dictionary<string, string> GetParams2(this string query)
        {
            return Regex.Matches(query, "([^?=&]+)(=([^&]*))?").Cast<Match>().ToDictionary(x => x.Groups[1].Value, x => x.Groups[3].Value);
        }
    }

    public static class UrlExtensions
    {
        public static string SetUrlParameter(this string url, string paramName, string value)
        {
            return new Uri(url).SetParameter(paramName, value).ToString();
        }

        public static Uri SetParameter(this Uri url, string paramName, string value)
        {
            var queryParts = HttpUtility.ParseQueryString(url.Query);
            queryParts[paramName] = value;
            return new Uri(url.AbsoluteUriExcludingQuery() + '?' + queryParts.ToString());
        }

        public static string AbsoluteUriExcludingQuery(this Uri url)
        {
            return url.AbsoluteUri.Split('?').FirstOrDefault() ?? String.Empty;
        }
    }

}