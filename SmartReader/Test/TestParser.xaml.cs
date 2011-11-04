using System;
using System.Net;
using System.Windows;
using Microsoft.Phone.Controls;
using SmartReader.Library.DataContract;
using SmartReader.Library.Network;
using SmartReader.Library.Parser.BookSite;
using SmartReader.Library.Parser.Sodu;

namespace SmartReader.Test
{
    public partial class TestParser : PhoneApplicationPage
    {
        public TestParser()
        {
            InitializeComponent();
        }

        private void CallSearchPage(object sender, RoutedEventArgs e)
        {
            var keyword = "官神";
            HttpContentDownloader downloader = new HttpContentDownloader();
            var uri = new Uri(String.Format("http://search.sodu.org/searchname.aspx?wd={0}", HttpUtility.UrlEncode(keyword)), UriKind.Absolute);
            //  downloader.Download(uri, keyword);
        }

        private void CallSoduLatestUpdatePage(object sender, RoutedEventArgs e)
        {
            var downloader = new HttpContentDownloader();
            var uri = new Uri(String.Format("http://www.sodu.org/mulu_160730.html"), UriKind.Absolute);
            downloader.Download(uri, null, new SoduBookLastestUpdatePageParser());
        }

        private void CallWebsiteSearchIndexPage(object sender, RoutedEventArgs e)
        {
            var downloader = new HttpContentDownloader();
            var uri = new Uri(String.Format("http://go.sodu.tv/gogourl.aspx?obid=03g0q&cid=385&bid=1613&tid=1&zid=1713549"), UriKind.Absolute);
            downloader.Download(uri, null, new WebSiteBookContentPageParser());
        }

        private void CallBookContentPage(object sender, RoutedEventArgs e)
        {
            var downloader = new HttpContentDownloader();
            //var uri1 = new Uri(String.Format("http://www.opny.net/Html/Book/23/18992/4274362.html"), UriKind.Absolute);
            //var uri2 = new Uri(String.Format("http://www.qu24read.com/book/64/64383/5620931.html"), UriKind.Absolute);
            //var uri3 = new Uri(String.Format("http://www.wu69.org/files/article/html/4/4144/2111331.html"), UriKind.Absolute);
            //var uri4 = new Uri(String.Format("http://www.oaixs.org/files/article/html/5/5929/4478629.html"), UriKind.Absolute);
            //var uri5 = new Uri(String.Format("http://www.lwzw.com/files/article/html/5/5703/2274760.html"), UriKind.Absolute);
            //var uri6 = new Uri(String.Format("http://www.q520.org/1/1846/1061784.html"), UriKind.Absolute);
            //var uri7 = new Uri(String.Format("http://www.dazhouhuangzu.net/mulu/0/175/1789564.html"), UriKind.Absolute);
            //var uri8 = new Uri(String.Format("http://www.baihuawen.com/xiaoshuo/27/27183/9662281.html"), UriKind.Absolute);
            //var uri9 = new Uri(String.Format("http://www.xiaoshuoread.com/read/0/853/2119114.html"), UriKind.Absolute); 
            var uri10 = new Uri(String.Format("http://www.xiaoshuo999.org/files/article/html/0/421/1077782.html"), UriKind.Absolute);

            //downloader.Download(uri1, new Chapter { ChapterUri = uri1, ChapterName = "望河异论希（四）" }, new WebSiteBookContentPageParser());
            //downloader.Download(uri2, new Chapter { ChapterUri = uri2, ChapterName = "望河异论希（四）" }, new WebSiteBookContentPageParser());
            //downloader.Download(uri3, new Chapter { ChapterUri = uri3, ChapterName = "望河异论希（四）" }, new WebSiteBookContentPageParser());
            //downloader.Download(uri4, new Chapter { ChapterUri = uri4, ChapterName = "望河异论希（四）" }, new WebSiteBookContentPageParser());
            //downloader.Download(uri5, new Chapter { ChapterUri = uri5, ChapterName = "望河异论希（四）" }, new WebSiteBookContentPageParser());
            //downloader.Download(uri6, new Chapter { ChapterUri = uri6, ChapterName = "望河异论希（四）" }, new WebSiteBookContentPageParser());
            //downloader.Download(uri7, new Chapter { ChapterUri = uri7, ChapterName = "望河异论希（四）" }, new WebSiteBookContentPageParser());
            //downloader.Download(uri8, new Chapter { ChapterUri = uri8, ChapterName = "望河异论希（四）" }, new WebSiteBookContentPageParser());
            //downloader.Download(uri9, new Chapter { ChapterUri = uri9, ChapterName = "望河异论希（四）" }, new WebSiteBookContentPageParser());
            downloader.Download(uri10, new Chapter { ChapterUri = uri10, ChapterName = "望河异论希（四）" }, new WebSiteBookContentPageParser());
        }

        private void CallWebSiteBookSearchPage(object sender, RoutedEventArgs e)
        {
            var downloader = new HttpContentDownloader();
            var uri1 = new Uri(String.Format("http://www.xinduba.com/Html/Book/18/18798/"), UriKind.Absolute); // BookText
            var uri2 = new Uri(String.Format("http://www.xiaoshuo999.org/files/article/html/0/421/index.html"), UriKind.Absolute); //Id Content
            var uri3 = new Uri(String.Format("http://www.yjxs.net/files/article/html/92/92146/"), UriKind.Absolute); //Id Content
            var uri4 = new Uri(String.Format("http://www.geiliwx.com/GeiLi/4/4703/"), UriKind.Absolute);
            var uri5 = new Uri(String.Format("http://tieshu.net/lishi/25769/"), UriKind.Absolute);
            var uri6 = new Uri(String.Format("http://www.zhantianzw.com/mulu/0/431/"), UriKind.Absolute);
            var uri7 = new Uri(String.Format("http://www.du1du.com/book/71/70333/"), UriKind.Absolute);
            var uri8 = new Uri(String.Format("http://www.hxqwx.com/xiaoshuo/71/70757/"), UriKind.Absolute);
            var uri9 = new Uri(String.Format("http://www.06sk.com/xiaoshuo/0/9/"), UriKind.Absolute);
            var uri10 = new Uri(String.Format("http://www.qu24read.com/book/64/64383/index.html"), UriKind.Absolute);


            downloader.Download(uri1, new Book { Name = "宰执天下", IndexPage = uri1, RootUrl = "http://www.abc.com/"}, new WebsiteBookIndexPageParser());
            downloader.Download(uri2, new Book { Name = "宰执天下", IndexPage = uri2, RootUrl = "http://www.abc.com/" }, new WebsiteBookIndexPageParser());
            downloader.Download(uri3, new Book { Name = "宰执天下", IndexPage = uri3, RootUrl = "http://www.abc.com/" }, new WebsiteBookIndexPageParser());
            downloader.Download(uri4, new Book { Name = "宰执天下", IndexPage = uri4, RootUrl = "http://www.abc.com/" }, new WebsiteBookIndexPageParser());
            downloader.Download(uri5, new Book { Name = "宰执天下", IndexPage = uri5, RootUrl = "http://www.abc.com/" }, new WebsiteBookIndexPageParser());
            downloader.Download(uri6, new Book { Name = "宰执天下", IndexPage = uri6, RootUrl = "http://www.abc.com/" }, new WebsiteBookIndexPageParser());
            downloader.Download(uri7, new Book { Name = "宰执天下", IndexPage = uri7, RootUrl = "http://www.abc.com/" }, new WebsiteBookIndexPageParser());
            downloader.Download(uri8, new Book { Name = "宰执天下", IndexPage = uri8, RootUrl = "http://www.abc.com/" }, new WebsiteBookIndexPageParser());
            downloader.Download(uri9, new Book { Name = "宰执天下", IndexPage = uri9, RootUrl = "http://www.abc.com/" }, new WebsiteBookIndexPageParser());
            downloader.Download(uri10, new Book { Name = "宰执天下", IndexPage = uri10, RootUrl = "http://www.abc.com/" }, new WebsiteBookIndexPageParser());
        }
    }
}