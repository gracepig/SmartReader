using System;
using System.Windows;
using System.Windows.Controls;
using Coding4Fun.Phone.Controls;
using SmartReader.Controls;
using SmartReader.Helper;
using SmartReader.Library.DataContract;
using SmartReader.ViewModel;

namespace SmartReader.Views
{
    public partial class ChatperDownload
    {
        private BookIndexViewModel _model;
        public BookIndexViewModel Model
        {
             set 
             { 
                 _model = value;
                 this.DataContext = Model;
             }
            get { return _model; }
        }

        private bool firstTimeLoad = true;

        public ChatperDownload()
        {
            InitializeComponent();
            Model = ModelManager.GetBookIndexModel();
        }

        private void DownloadSetting(object sender, EventArgs e)
        {
            Model.DownloadStartIndex = 1;
            Model.ChapterToBeDownloadedCount = 10;

            var messagePrompt = new AboutPrompt
                                    {
                                        Title = "下载设置",
                                        Body = new DownloadSetting(),
                                        IsAppBarVisible = true,
                                        DataContext = Model,
                                    };
            messagePrompt.Show();
        }

        private void NextPage(object sender, EventArgs e)
        {
            Model.NextPage();
            DataContext = Model;
        }

        private void PreviousPage(object sender, EventArgs e)
        {
            Model.PreviousPage();
        }

        private void FirstPage(object sender, EventArgs e)
        {
            Model.FirstPage();
        }

        private void LastPage(object sender, EventArgs e)
        {
            Model.LastPage();
        }

        private void Download(object sender, EventArgs e)
        {
            Model.DownloadBookContents();
        }

        private void ShowChapter(object sender, RoutedEventArgs e)
        {
            //Init Chapter ViewModel first 
            var x = ModelManager.GetChapterViewModel();
            x.CurrentBook = Model.Book;
            x.CurrentChapter = ((Button)sender).DataContext as Chapter;
            
            //Navigate to new page 
            CrossThreadHelper.CrossThreadMethodCall(() =>
                NavigationService.Navigate(new Uri("/Views/ChapterViewPage.xaml", UriKind.Relative))
            );
        }

        private void ChapterListPageLayoutUpdated(object sender, EventArgs e)
        {
            if (firstTimeLoad)
            {
                ProgressIndicatorHelper.StartProgressIndicator(true, "读取目录内容");
                firstTimeLoad = false;
            }else
            {
                ProgressIndicatorHelper.StopProgressIndicator();
            }
        }

        private void BackToBookListPage(object sender, EventArgs e)
        {
            PageManager.Navigate(PageManager.BookListPage);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            Model.CancelRunningConnections();
            base.OnNavigatedFrom(e);
            firstTimeLoad = true;
            ProgressIndicatorHelper.StopProgressIndicator();
        }
    }
}