using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using SmartReader.Library.DataContract;
using SmartReader.Library.Interface;
using hh = SmartReader.Library.Helper.HtmlParseHelper;

namespace SmartReader.Library.Parser.Xiaoelang
{
    public class XiaoelangSearchResultPageParser : IParser 
    {
        public object Parse(System.IO.Stream inputStream, object state)
        {
            var SearchResultUrlList = new List<SearchResult>();

            var doc = new HtmlDocument();
            doc.Load(inputStream);
            var body = hh.GetSingleChildByTypeChain(doc.DocumentNode, new[] {"html", "body"});
            var table = hh.GetSingleDirectChildByTypeAndIndex(body, "table",3);

            var trElements = table.DescendantNodes().Where(n => n.Name == "tr").Skip(1);
            trElements = trElements.Reverse().Skip(1);
            trElements = trElements.Reverse();

            foreach (var trElement in trElements)
            {
                ProcessBookElement(trElement, SearchResultUrlList);
            }
            return SearchResultUrlList;
        }

        private void ProcessBookElement(HtmlNode trElement, List<SearchResult> SearchResultUrlList)
        {
            if (trElement.Name != "tr") return;

            var item = new SearchResult();

            var tdElements = trElement.DescendantNodes().Where(n => n.Name == "td").ToArray();

            if (tdElements.Length == 5)
            {
                //Parse Book name and index page url
                var aElement = tdElements[1].DescendantNodes().Where(n => n.Name == "a").First();

                if (aElement != null)
                {
                    item.IndexPageUri = new Uri("http://www.xiaoelang.com" + aElement.Attributes["href"].Value, UriKind.Absolute);
                    item.BookName = aElement.InnerText;
                    item.Book = new Book();
                    item.Book.Name = item.BookName;
                }

                //Parse last update chapter name
                aElement = tdElements[2].DescendantNodes().Where(n => n.Name == "a").First();

                if (aElement != null)
                {
                    item.LastUpdateChapterName = aElement.InnerText;
                }
                //Parse last update time
                //<td class="xt" align="center" valign="middle" width="78">2011-08-26</td>
            }

            SearchResultUrlList.Add(item);
        }

        public event EventHandler ParsingCompleted;
    }
}
