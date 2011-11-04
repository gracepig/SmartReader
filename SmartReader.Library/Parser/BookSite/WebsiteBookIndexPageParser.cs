using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using HtmlAgilityPack;
using SmartReader.Library.DataContract;
using SmartReader.Library.Helper;
using SmartReader.Library.Interface;
using hh = SmartReader.Library.Helper.HtmlParseHelper;

namespace SmartReader.Library.Parser.BookSite
{
    public class WebsiteBookIndexPageParser : IParser
    {
        private Book book; 

        private List<Chapter> chapterList =  new List<Chapter>();

        public object Parse(Stream inputStream, object state)
        {
            book = state as Book;

            var content1 = EncodingHelper.FromGBKToUnicode(inputStream);
            inputStream.Close();
            var doc = new HtmlDocument();
            doc.LoadHtml(content1);
            var body = hh.GetSingleChildByTypeChain(doc.DocumentNode, new string[] { "html", "body" });
            if (body == null) body = doc.DocumentNode;
            CleanHtmlTree(body);
            
            var indexNode = GetIndexContentNode(body);
            var hyperLinkNodes = new List<HtmlNode>();
            hh.GetAllHyperlinkElementWithFilter(indexNode, hyperLinkNodes);

            cleanHyperLinkNode(hyperLinkNodes);

            Debug.Assert(hyperLinkNodes.Count > 0);

            foreach (var link in hyperLinkNodes)
            {
                ParseIndexContent(link);
            }

            book.Chapters = chapterList.ToArray();

            return book;
        }

        private void cleanHyperLinkNode(List<HtmlNode> hyperLinkNodes)
        {
            var deletedNodes = new List<HtmlNode>();
            var absoluteUrlCount = new List<HtmlNode>();
            var urlPatternPartsCount = new List<HtmlNode>[5] { new List<HtmlNode>(), new List<HtmlNode>(), new List<HtmlNode>(), new List<HtmlNode>(), new List<HtmlNode>() };

            foreach (var node in hyperLinkNodes)
            {
                var url = node.Attributes["href"].Value;
                if (url.ToLower().Contains("http")) absoluteUrlCount.Add(node);

                switch (url.Split('/').Length)
                {
                    case 0: urlPatternPartsCount[0].Add(node); break;
                    case 1: urlPatternPartsCount[1].Add(node); break;
                    case 2: urlPatternPartsCount[2].Add(node); break;
                    case 3: urlPatternPartsCount[3].Add(node); break;
                    case 4: urlPatternPartsCount[4].Add(node); break;
                }
                
            }

            if (absoluteUrlCount.Count < hyperLinkNodes.Count /2)
            {
                deletedNodes.AddRange(absoluteUrlCount);
            }

            foreach (var list in urlPatternPartsCount)
            {
                if (list.Count < hyperLinkNodes.Count / 2)
                {
                    deletedNodes.AddRange(list);
                }
            }

            foreach (var node in deletedNodes)
            {
                hyperLinkNodes.Remove(node);
            }
        }

        private static void CleanHtmlTree(HtmlNode body)
        {
            var nodesToBeDeleted = new List<HtmlNode>();
            hh.RemoveNodeByType(body, "script", nodesToBeDeleted);
            hh.RemoveNodeByClassName(body, "toolbar", nodesToBeDeleted);

            foreach (var deleteNode in nodesToBeDeleted)
            {
                deleteNode.Remove();
            }
        }

        private void ParseIndexContent ( HtmlNode node )
        {
            //<a href="3939265.html" title="第一章 大梦初醒 更新时间：2010-03-22 00:17">第一章 大梦初醒</a>
            var chapter = new Chapter { LastUpdateTime = DateTime.Now};
            chapter.ChapterName = node.InnerText;
            if (node.Attributes["href"].Value.ToLower().Contains("http"))
            {
                chapter.ChapterUri = new Uri(node.Attributes["href"].Value, UriKind.Absolute);
            }
            else
            {
                chapter.ChapterUri = new Uri(book.RootUrl + node.Attributes["href"].Value, UriKind.Absolute);    
            }
            chapter.Book = book;
            chapterList.Add(chapter);
        }

        private HtmlNode GetIndexContentNode ( HtmlNode body)
        {
            //var indexNode = hh.FindElementById(body, "Content");
            //if (indexNode != null) return indexNode;

            //var indexNode = hh.FindElementByClass(body, "Content");
            //if (indexNode != null) return indexNode;

            var indexNode = hh.FindElementById(body, "BookText");

            if (indexNode != null) return indexNode;

            return body;

        }


        public event EventHandler ParsingCompleted;
    }
}
