using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
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

        private readonly List<Chapter> chapterList =  new List<Chapter>();

        public object Parse(Stream inputStream, object state)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            book = state as Book;
            var content1 = EncodingHelper.FromGBKToUnicode(inputStream);
            var decodeTime = stopWatch.ElapsedMilliseconds;

            inputStream.Close();
            var doc = new HtmlDocument();
            doc.LoadHtml(content1);
            var body = hh.GetSingleChildByTypeChain(doc.DocumentNode, new string[] { "html", "body" });


            if (body == null) body = doc.DocumentNode;
            CleanHtmlTree(body);

            var cleanTreeTime = stopWatch.ElapsedMilliseconds - decodeTime;

            var indexNode = GetIndexContentNode(body);
            var hyperLinkNodes = new List<HtmlNode>();
            hh.GetAllHyperlinkElementWithFilter(indexNode, hyperLinkNodes);

            var getAllHyperLinkTime = stopWatch.ElapsedMilliseconds - cleanTreeTime - decodeTime;

            cleanHyperLinkNode(hyperLinkNodes);

            var cleanHyperLinkTime = stopWatch.ElapsedMilliseconds - cleanTreeTime - decodeTime - getAllHyperLinkTime;

            Debug.Assert(hyperLinkNodes.Count > 0);

            foreach (var link in hyperLinkNodes)
            {
                ParseIndexContent(link);
            }

            var parseIndexContentTime = stopWatch.ElapsedMilliseconds - cleanTreeTime - decodeTime - getAllHyperLinkTime -
                                        cleanHyperLinkTime;

            if (book.Chapters == null )
            {
                book.Chapters = chapterList.ToArray();
            }
            else
            {
                var oldList = book.Chapters.ToList();

                if (oldList.Count < chapterList.Count)
                {
                   for ( var i = oldList.Count; i< chapterList.Count ; i++ )
                   {
                       oldList.Add(chapterList[i]);
                   }
                }

                book.Chapters = oldList.ToArray(); 
            }

            var totalTime = stopWatch.ElapsedMilliseconds;
            stopWatch.Stop();

            var time =
                String.Format(
                    "cleanTreeTime {0}\n getAllHyperLinkTime {1}\n decodeTime {2}\n cleanHyperLinkTime {3}\n parseIndexContentTime {4}\n Totaltime {5}\n",
                    cleanTreeTime, getAllHyperLinkTime, decodeTime, cleanHyperLinkTime, parseIndexContentTime, totalTime);

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
                var baseUri = new Uri(book.RootUrl);
                chapter.ChapterUri = new Uri(baseUri , node.Attributes["href"].Value);   //Take care of the double "//" problem
            }
            chapter.Book = book;
            chapterList.Add(chapter);
        }

        private HtmlNode GetIndexContentNode ( HtmlNode body)
        {
            var indexNode = hh.FindElementById(body, "BookText");
            if (indexNode != null) return indexNode;
            return body;
        }

        public event EventHandler ParsingCompleted;
    }
}
