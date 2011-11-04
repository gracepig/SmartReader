using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using HtmlAgilityPack;
using SmartReader.Library.DataContract;
using SmartReader.Library.Interface;

namespace SmartReader.Library.Parser.Sodu
{
    /// <summary>
    /// This parser will parse solu seach result 
    /// and return the website/book pair back
    /// next step is to parse the book latest update page on solu
    /// </summary>
    public class SoduSearchResultPageParser : IParser
    {

        public void Parse(string content)
        {
        }

        public object Parse(Stream inputStream, object keyWord)
        {
            var SearchResultUrlList = new List<SearchResult>();

            var doc = new HtmlDocument();
            doc.Load(inputStream);
            var body = doc.DocumentNode.Descendants()
                                .Where(n => n.Name == "body")
                                .FirstOrDefault();

            var tables = body.DescendantNodes().Where(n => n.Name == "table");
            var bookListTable = tables.ElementAt(3);

            var trElements = bookListTable.DescendantNodes().Where(n => n.Name == "tr").Skip(2);

            trElements = trElements.Reverse().Skip(2);
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

            if (tdElements.Length == 3)
            {
                //Parse Book name and index page url
                var aElement = tdElements[0].DescendantNodes().Where(n => n.Name == "a").First();

                if (aElement != null)
                {
                    item.IndexPageUri = new Uri(aElement.Attributes["href"].Value, UriKind.Absolute);
                    item.BookName = aElement.InnerText;
                    item.Book = new Book();
                    item.Book.Name = item.BookName;
                }

                //Parse last update chapter name
                aElement = tdElements[1].DescendantNodes().Where(n => n.Name == "a").First();

                if (aElement != null)
                {
                    item.LastUpdateChapterName = aElement.InnerText;
                }

                //Parse last update time
                //<td class="xt" align="center" valign="middle" width="78">2011-08-26</td>
                var dateElement = tdElements[2];

                if (dateElement != null)
                {
                    item.LastUpdateDate = DateTime.Parse(dateElement.InnerText);
                }
            }

            SearchResultUrlList.Add(item);
        }


        public event EventHandler ParsingCompleted;
    }
}
