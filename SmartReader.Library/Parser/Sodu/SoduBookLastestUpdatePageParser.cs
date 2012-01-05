using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HtmlAgilityPack;
using SmartReader.Library.DataContract;
using SmartReader.Library.Interface;
using hh = SmartReader.Library.Helper.HtmlParseHelper;

namespace SmartReader.Library.Parser.Sodu
{
    public class SoduBookLastestUpdatePageParser : IParser 
    {
        private List<Book> items = new List<Book>();
        public Book metaData;

        /// <summary>
        /// Get the book and website pair list information
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public object Parse(Stream inputStream, object state)
        {
            metaData = state as Book;
            var doc = new HtmlDocument();
            doc.Load(inputStream);

            var body = hh.GetSingleChildByTypeChain(doc.DocumentNode, new[] { "html", "body" });

            var table = hh.GetSingleDirectChildByTypeAndIndex(body, "table", 4);
            var x = hh.GetDirectChildrenByType(table, "tr").ToArray();
            var chapterRows = hh.SubArray(x, 2, 9 ).ToArray();

            foreach (var tr in chapterRows)
            {
                ParseChapterRow(tr);
            }
            return items;
        }

        /***
         <tr height="30">
	        <td class="xt">&nbsp;&nbsp;
		        <a href="http://go.sodu.tv/gogourl.aspx?obid=03g0q&cid=452&bid=405&tid=&zid=1118953.shtml" target="_blank">周一求票，关于更新和情节的话，请入内一观！</a>
		        <td class="xt">
			        <font color="#123b8d">六月文学</font>
			        <td class="xt">2011-8-29</td>
		        </td>
	        </td>
        </tr> 
         ***/

        private const string WebsiteFilter1 = "起点";
        private const string WebsiteFilter2 = "纵横";

        private void ParseChapterRow(HtmlNode tr)
        {
            var item = new Book();

            var topTd = hh.GetSingleDirectChildByType(tr, "td");
            var chapterUrl = hh.GetSingleDirectChildByType(topTd, "a");
            var lowerTd = hh.GetSingleDirectChildByType(topTd, "td");

            item.Name = metaData.Name.Trim();
            item.IndexPage = new Uri(chapterUrl.Attributes["href"].Value, UriKind.Absolute);
            item.LastestUpdateChapterName = chapterUrl.InnerText.Trim();
            item.WebSite = new WebSite();
            item.WebSite.WebSiteName = hh.GetSingleDirectChildByType(lowerTd, "font").InnerText.Trim();
            item.LastUpdateTime = DateTime.Parse(hh.GetSingleDirectChildByType(lowerTd, "td").InnerText);
            
            if (item.WebSite.WebSiteName.Contains(WebsiteFilter1) 
                || item.WebSite.WebSiteName.Contains(WebsiteFilter2))
                return;

            var websiteBookPairAlreadyExists = (from i in items
                                                where i.WebSite.WebSiteName == item.WebSite.WebSiteName
                                                select i).FirstOrDefault();

            if (websiteBookPairAlreadyExists == null )
            {
                items.Add(item);
            }
        }

        public event EventHandler ParsingCompleted;
    }
}
