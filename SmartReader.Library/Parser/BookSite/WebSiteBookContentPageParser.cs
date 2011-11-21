using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using HtmlAgilityPack;
using ImageTools.IO.Gif;
using ImageTools.IO.Png;
using SmartReader.Library.DataContract;
using SmartReader.Library.Helper;
using SmartReader.Library.Interface;
using SmartReader.Library.Network;
using SmartReader.Library.Storage;
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
            bool getIndexPage = false;
            if (Metadata == null )
            {
                book = state as Book;
                getIndexPage = true;
            }

            var content1 = EncodingHelper.FromGBKToUnicode(inputStream);
            inputStream.Close();
            var doc = new HtmlDocument();
            doc.LoadHtml(content1);
            var body = hh.GetSingleChildByTypeChain(doc.DocumentNode, new[] {"html", "body"});
            if (body == null) body = doc.DocumentNode;

            if (getIndexPage && book != null )
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
                if (Metadata != null) 
                {
                    var content = NormalizeContentNode(textNodes);

                    if (content.Length > 4000)
                    {
                        Metadata.SaveContent1 = content.Substring(0, 4000);
                        Metadata.SaveContent2 = content.Substring(4000, content.Length - 4000);
                    }
                    else
                    {
                        Metadata.SaveContent1 = content;
                    }
                }
            }

            Deployment.Current.Dispatcher.BeginInvoke(() => 
                        { Metadata.Downloaded = true; }
                );
            return Metadata;
        }

        private void GetImageContents(List<HtmlNode> imageNodes)
        {
            List<string> urlList = new List<string>();

            foreach (var imageNode in imageNodes)
            {
                if (imageNode.Attributes["src"] != null)
                {
                    var url = imageNode.Attributes["src"].Value;

                    if (!url.Contains("http:"))
                    {
                        var rootUrl = Metadata.Book.RootUrl;
                        var subUrl = url.StartsWith("/") ? url.Substring(1) : url;
                        //Process if image path is relative path
                        url = rootUrl.EndsWith("/") ? rootUrl + url : rootUrl + "/" + subUrl;
                    }
                    urlList.Add(url);
                }
            }

            foreach (var imageUrl in urlList)
            {
                var downloader = new HttpContentDownloader();
                downloader.Download(new Uri(imageUrl, UriKind.Absolute), ar =>
                {
                    try
                    {
                        //At this step, we can get the index page in the search engine 
                        var state = (RequestState)ar.AsyncState;
                        var response = (HttpWebResponse)state.Request.EndGetResponse(ar);
                        var imageStream = response.GetResponseStream();
                        Metadata.IsImageContent = true;
                        
                        var image = new ArticleImage {Chapter = Metadata, ImageUrl =  response.ResponseUri.ToString()};
                        //var image = new ArticleImage();

                        var gd = new GifDecoder();
                        var img = new ImageTools.ExtendedImage();

                        gd.Decode(img, imageStream);

                        var png = new PngEncoder();
                        
                        using (var memStream = new MemoryStream())
                        {
                            png.Encode(img, memStream);
                            byte[] byteArray = memStream.GetBuffer();
                            image.ImageBytes = byteArray;
                        }
                        
                        PhoneStorage.GetPhoneStorageInstance().SaveArticleImage(image);

                        Metadata.Downloaded = true;
                    }
                    catch (WebException ex)
                    {
                        throw ex;
                    }
                });
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
                if (line != null && (line.Contains(indexText1) || line.Contains(indexText2) || line.Contains(indexText3)))
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

        public static byte[] ConvertToBytes(BitmapImage bitmapImage)
        {
            byte[] data;
            using (var stream = new MemoryStream())
            {
                var wBitmap = new WriteableBitmap(bitmapImage);
                wBitmap.SaveJpeg(stream, wBitmap.PixelWidth, wBitmap.PixelHeight, 0, 100);
                stream.Seek(0, SeekOrigin.Begin);
                data = stream.GetBuffer();
            }
            return data;
        }

        public event EventHandler ParsingCompleted;
    }
}
