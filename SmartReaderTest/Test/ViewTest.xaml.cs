using System;
using System.Net;
using System.Windows;
using ImageTools.IO.Gif;
using Microsoft.Phone.Controls;
using SmartReader.Library.DataContract;
using SmartReader.Library.Network;
using SmartReader.Library.Parser.BookSite;
using SmartReader.ViewModel;

namespace SmartReader.Test
{
    public partial class ViewTest : PhoneApplicationPage
    {

        private Chapter chapter { set; get; }

        public ViewTest()
        {
            InitializeComponent();

            var task = new BookIndexViewModel.DownloadTask();

            chapter = new Chapter();
            chapter.ChapterName = "人类已经不能阻挡我了";
            chapter.ChapterUri = new Uri("http://www.83k.com/Html/Book/12/12342/4020652.shtml",
                                         UriKind.Absolute);
            task.TaskChapter = chapter;

            //DownloadChapter(task);
        }

        private void ViewChapter(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/ChapterViewPage.xaml", UriKind.Relative));
        }

        public void DownloadChapter(object s)
        {
            var cha = (BookIndexViewModel.DownloadTask) s;

            var downloader = new HttpContentDownloader();
            downloader.Download(cha.TaskChapter.ChapterUri, ar =>
                                                                {
                                                                    //At this step, we can get the index page in the search engine 
                                                                    var state = (RequestState) ar.AsyncState;
                                                                    var response =
                                                                        (HttpWebResponse)
                                                                        state.Request.EndGetResponse(ar);
                                                                    response.GetResponseStream();

                                                                    var parser = new WebSiteBookContentPageParser();
                                                                    parser.Parse(response.GetResponseStream(),
                                                                                 cha.TaskChapter);
                                                                    CommonModels.TestChapter = chapter;
                                                                });
        }

        private void SearchView(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/SearchInputView.xaml", UriKind.Relative));
        }

        private void ViewBooks(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/BookListPage.xaml", UriKind.Relative));
        }

        private void ViewImageChapter(object sender, RoutedEventArgs e)
        {
            var x = ModelManager.GetChapterViewModel();
            x.CurrentBook = TestDBHelper.GetTestDBInstance().TestBook;
            x.CurrentChapter = TestDBHelper.GetTestDBInstance().TestTextChapter;
            NavigationService.Navigate(new Uri("/Views/ChapterViewPage.xaml", UriKind.Relative));
        }



        private void ViewOnlineImage(object sender, RoutedEventArgs e)
        {
            ImageTools.IO.Decoders.AddDecoder<GifDecoder>();
            var ImageSource = new Uri("http://shuzong.com/DownFiles/Book/0/51/2011/12/1/20111201124952361.gif");
            this.GifImageContainer.DataContext = ImageSource;
        }
    }
}