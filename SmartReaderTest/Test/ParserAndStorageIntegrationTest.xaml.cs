using System;
using System.Windows;
using System.Windows.Data;
using Microsoft.Phone.Shell;
using SmartReader.Library.DataContract;
using SmartReader.Library.Network;
using SmartReader.Library.Parser.BookSite;
using SmartReader.ViewModel;

namespace SmartReader.Test
{
    public partial class ParserAndStorageIntegrationTest
    {

        private SearchViewModel _model;
        public SearchViewModel Model
        {
            set {
                _model = value;
                this.DataContext = value;
            }
            get { return _model; }
        }

        public ParserAndStorageIntegrationTest()
        {
            Model = new SearchViewModel();
            InitializeComponent();

            
        }

        private void SaveSingleChapter(object sender, RoutedEventArgs e)
        {
            var downloader = new HttpContentDownloader();
            var uri1 = new Uri(String.Format("http://www.tianhen.com/tian/Book/0/547/3165604.shtml"), UriKind.Absolute);

            //downloader.Download(uri1, new Chapter { ChapterUri = uri1, ChapterName = "望河异论希（四）" }, new WebSiteBookContentPageParser());
            //var uri10 = new Uri(String.Format("http://www.ruyu.org/bk/0/200/1821260.html"), UriKind.Absolute);
            //downloader.Download(uri10, new Chapter { ChapterUri = uri10, ChapterName = "望河异论希（四）" }, new WebSiteBookContentPageParser());
        }

        
        public static ProgressIndicator _progressIndicator;
        private void SearchSoduTest(object sender, RoutedEventArgs e)
        {
            if (null == _progressIndicator)
            {
                _progressIndicator = new ProgressIndicator();
                _progressIndicator.IsVisible = true;

                Binding binding = new Binding("Downloading") { Source = Model };

                BindingOperations.SetBinding(
                    _progressIndicator, ProgressIndicator.IsVisibleProperty, binding);

                binding = new Binding("Downloading") { Source = Model };

                BindingOperations.SetBinding(
                    _progressIndicator, ProgressIndicator.IsIndeterminateProperty, binding);

                SystemTray.ProgressIndicator = _progressIndicator;
            }
            _progressIndicator.IsIndeterminate = true;
            Model.Search("宰执天下");
        }

        private void GetWebSiteBookInfo(object sender, RoutedEventArgs e)
        {
            //vm.Search("官神");
            Model.GetBookSiteBookIndexPageLink(Model.BookList[0]);
        }


        private void GetChapters(object sender, RoutedEventArgs e)
        {
            var bookVm = new BookIndexViewModel(Model.BookList[0]);

            bookVm.DownloadBookContents();
        }
    }
}