using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HtmlAgilityPack;
using SmartReader.Library.DataContract;
using SmartReader.Library.Helper;
using SmartReader.Library.Interface;
using hh = SmartReader.Library.Helper.HtmlParseHelper;

namespace SmartReader.Library.Parser.BookSite
{
    public class WebSiteBookContentPageParser : IParser 
    {

        public Chapter Metadata;

        //As for state we need to parse book metadata to here
        public object Parse(Stream inputStream, object state)
        {
            Metadata = state as Chapter;

            Book book = null;
            bool GetIndexPage = false;
            if (Metadata == null )
            {
                book = state as Book;
                GetIndexPage = true;
            }

            var content1 = EncodingHelper.FromGBKToUnicode(inputStream);
            inputStream.Close();
            var doc = new HtmlDocument();
            doc.LoadHtml(content1);
            var body = hh.GetSingleChildByTypeChain(doc.DocumentNode, new[] {"html", "body"});
            if (body == null) body = doc.DocumentNode;

            if (GetIndexPage && book != null )
            {
               GetIndexPageLink(body, book);
                return null;
            }
            

            CleanHtmlTree(body);

            var contentNode = GetContentNode(body);

            if (IsContentImage(contentNode))
            {
                var imageNodes = new List<HtmlNode>();
                hh.GetAllImageElementWithFilter(contentNode, imageNodes);

                GetImageContents(imageNodes);
            }
            else
            {
                var textNodes = new List<HtmlNode>();
                hh.GetAllTextElement(contentNode, textNodes);
                Metadata.Content = NormalizeContentNode(textNodes);
            }

            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() => 
                        { Metadata.Downloaded = true; }
                );
            return Metadata;
        }

        private void GetImageContents(List<HtmlNode> imageNodes)
        {
            foreach (var imageNode in imageNodes)
            {
                if (imageNode.Attributes["src"] != null)
                {
                    var url = imageNode.Attributes["src"].Value;

                    if (!url.Contains("http:"))
                    {
                        //Process if image path is relative path
                    }
                }
            }
        }

        private bool IsContentImage(HtmlNode contentNode)
        {
            var imageNode = hh.FindElementByClass(contentNode, "divimage");
            if (imageNode != null) return true;

            var nodesList = new List<HtmlNode>();
            hh.GetAllImageElementWithFilter(contentNode, nodesList);
            if (nodesList.Count > 0) return true;
            return false;
        }

        private static void CleanHtmlTree(HtmlNode body)
        {
            var nodesToBeDeleted = new List<HtmlNode>();
            hh.RemoveNodeByType(body, "script", nodesToBeDeleted);
            hh.RemoveNodeByType(body, "a", nodesToBeDeleted);
            hh.RemoveNodeByClassName(body, "toolbar", nodesToBeDeleted);
            hh.RemoveNodeById(body, "welcome", nodesToBeDeleted);
            hh.RemoveNonDisplayNode(body, nodesToBeDeleted);

            foreach (var deleteNode in nodesToBeDeleted)
            {
                deleteNode.Remove();
            }
        }

        private string NormalizeContentNode(IEnumerable<HtmlNode> textNodes)
        {
            var sb = new StringBuilder();
            foreach ( var textnode in textNodes)
            {
                sb.AppendLine(textnode.InnerText.Trim());
            }

            var content = sb.ToString().Replace("&nbsp;", " ");

            if (content.IndexOf(Metadata.ChapterName) > 0)
                content = content.Substring(content.IndexOf(Metadata.ChapterName));
            return content;
        }


        /// <summary>
        /// Get Url of the website book index page
        /// </summary>
        /// <param name="node">expect body node here</param>
        /// <param name="item"></param>
        public void GetIndexPageLink (HtmlNode node, Book item)
        {
            const string indexText1 = "回目录";
            const string indexText2 = "回书目";
            const string indexText3 = "目 录";

            var reader = new StringReader(node.InnerHtml);
            var sb = new StringBuilder();
            while (reader.Peek() > 0)
            {
                var line = reader.ReadLine();
                sb.Append(line);
                if (line.Contains(indexText1) || line.Contains(indexText2) || line.Contains(indexText3))
                {

                    var startIndex = sb.ToString().LastIndexOf("<a");
                    var endIndex = sb.ToString().LastIndexOf("</a>");
                    var htmlLink = sb.ToString().Substring(startIndex, endIndex - startIndex + 4);

                    while (!(htmlLink.Contains(indexText1) || htmlLink.Contains(indexText2) || htmlLink.Contains(indexText3)))
                    {
                        var temp = sb.ToString().Substring(0, startIndex);
                        startIndex = temp.LastIndexOf("<a");
                        endIndex = temp.LastIndexOf("</a>");

                        if (startIndex > -1 && endIndex > -1)
                        {
                            htmlLink = temp.Substring(startIndex, endIndex - startIndex + 4);    
                        }
                        
                    }

                    var linkNode = new HtmlDocument();
                    linkNode.LoadHtml(htmlLink);
                    var aNode = hh.GetSingleDirectChildByType(linkNode.DocumentNode, "a");
                    var link = aNode.Attributes["href"].Value;
                    if (link.Contains("http"))
                    {
                        item.IndexPage = new Uri(link, UriKind.Absolute);
                        //item.IndexPageUri = new Uri(link, UriKind.Absolute);
                        return;
                    }
                    else
                    {
                        if (item.RootUrl.EndsWith(link))
                        {  
                            item.IndexPage = new Uri(item.RootUrl, UriKind.Absolute);
                            return;
                        }
                        
                        item.IndexPage = new Uri(item.RootUrl +  link, UriKind.Absolute);
                        //item.IndexPageUri = new Uri(item.PageRootUri + link, UriKind.Absolute);
                        return;
                    }
                }
            }
        }

        public HtmlNode GetContentNode (HtmlNode body)
        {
            var contentNode = hh.FindElementById(body, "content");

            if (contentNode != null) return contentNode;

            //For HualuoWenxue
            contentNode = hh.FindElementById(body, "box");

            if (contentNode != null)
            {
                return contentNode;
            }
            return body;
        }


        public event EventHandler ParsingCompleted;
    }
}
