using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HtmlAgilityPack;
using SmartReader.Library.DataContract;
using SmartReader.Library.Helper;
using SmartReader.Library.Interface;

namespace SmartReader.Library.Parser.Xiaoelang
{
    public class XiaoelangBookLastestUpdatePageParser : IParser 
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

            var body = HtmlParseHelper.GetSingleChildByTypeChain(doc.DocumentNode, new[] { "html", "body" });

            var table = HtmlParseHelper.GetSingleDirectChildByTypeAndIndex(body, "table", 2);
            var x = HtmlParseHelper.GetDirectChildrenByType(table, "tr").ToArray();
            var chapterRows = HtmlParseHelper.SubArray(x, 1, 1).ToArray();

            foreach (var tr in chapterRows)
            {
                ParseChapterRow(tr);
            }

            return items;
        }

        /***
        <td class="xt"><a href="/books/chapter/12548324" target="_blank">正文 第1532章 总算来了（求推荐票！）</a>    </td>
        <td class="xt"><a href="http://www.duzheju.com" target="_blank">读者居小说网</a>    </td>
        <td class="xt"><a href="/books/mulu/47/15518" target="_blank">查看</a></td>
        <td class="xt"><script>document.write(getLocalTime(1321511348));</script></td> 
         ***/

        private string websiteFilter1 = "起点";
        private string websiteFilter2 = "纵横";
        private void ParseChapterRow(HtmlNode tr)
        {
            var item = new Book();

            var chaperTd = HtmlParseHelper.GetSingleDirectChildByType(tr, "td");
            var chaperUrl = HtmlParseHelper.GetSingleDirectChildByType(chaperTd, "a");
            var websiteTd = HtmlParseHelper.GetSingleDirectChildByTypeAndIndex(tr, "td", 1);
            var websiteName = HtmlParseHelper.GetSingleDirectChildByType(websiteTd, "a");
            var websiteIndexTd = HtmlParseHelper.GetSingleDirectChildByTypeAndIndex(tr, "td", 2);
            var websiteIndexUrl = HtmlParseHelper.GetSingleDirectChildByType(websiteIndexTd, "a");

            item.LastUpdateTime = DateTime.Now;


            item.Name = metaData.Name;
            item.IndexPage = new Uri("http://www.xiaoelang.com" + websiteIndexUrl.Attributes["href"].Value, UriKind.Absolute);
            item.LastestUpdateChapterName = chaperUrl.InnerText;
            item.WebSite = new WebSite();
            item.WebSite.WebSiteName = websiteName.InnerText;
            //item.LastUpdateTime = DateTime.Parse(HtmlParseHelper.GetSingleDirectChildByType(lowerTd, "td").InnerText);


            if (item.WebSite.WebSiteName.Contains(websiteFilter1) || item.WebSite.WebSiteName.Contains(websiteFilter2))
                return;

            var websiteBookPairAlreadyExists = (from i in items
                                                where i.WebSite.WebSiteName == item.WebSite.WebSiteName
                                                select i).FirstOrDefault();

            if (websiteBookPairAlreadyExists == null)
            {
                items.Add(item);
            }
        }
        public event EventHandler ParsingCompleted;
    }
}
