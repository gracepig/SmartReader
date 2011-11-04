using System;
using System.Windows;
using Coding4Fun.Phone.Controls;
using Microsoft.Phone.Controls;
using SmartReader.Controls;
using SmartReader.Helper;
using SmartReader.ViewModel;

namespace SmartReader.Views
{
    public partial class ChatperDownload : PhoneApplicationPage
    {

        private BookIndexViewModel _model;
        public BookIndexViewModel Model
        {
             set { _model = value;
                 this.DataContext = value;
             }

            get { return _model; }
        }

        public ChatperDownload()
        {
            InitializeComponent();
            Model = ModelManager.GetBookIndexModel();
        }


        private void DownloadSetting(object sender, EventArgs e)
        {
            Model.DownloadStartIndex = 1;
            Model.ChapterToBeDownloadedCount = 10;

            var messagePrompt = new AboutPrompt()
                                    {
                                        Title = "下载设置", 
                                        Body = new DownloadSetting(), 
                                        IsAppBarVisible = true, 
                                    };

            messagePrompt.DataContext = Model;
            messagePrompt.Show();
            
            
        }

        private void NextPage(object sender, EventArgs e)
        {
            Model.NextPageChapters();
        }

        private void PreviousPage(object sender, EventArgs e)
        {
            Model.PreviousPageChapters();
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
            ProgressIndicatorHelper.StartProgressIndicator(false);
            ProgressIndicatorHelper.SetIndicatorValue(0);
            Model.DownloadBookContents();
        }

        private void ShowChapter(object sender, RoutedEventArgs e)
        {
            //Init Chapter ViewModel first 

            var x = ModelManager.GetChapterViewModel();
            x.CurrentBook = Model.Book;

            //Navigate to new page 

            CrossThreadHelper.CrossThreadMethodCall(() =>
                NavigationService.Navigate(new Uri("/Views/ChapterViewPage.xaml", UriKind.Relative))
            );
        }
    }
}