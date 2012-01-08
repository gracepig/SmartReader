using System;
using System.Windows;
using System.Windows.Controls;
using SmartReader.Helper;
using SmartReader.Library.DataContract;
using SmartReader.Library.Storage;
using SmartReader.ViewModel;

namespace SmartReader.Views
{
    public partial class BookSourcePage
    {
        private SearchViewModel _model;
        public SearchViewModel Model
        {
            set 
            { 
                _model = value;
                DataContext = value;
            }
            get { return _model; }
        }

        public BookSourcePage()
        {
            InitializeComponent();
            Model = ModelManager.GetSearchResultModel();
        }

        private Book targetBook;

        private void BookSelected(object sender, RoutedEventArgs e)
        {
            ProgressIndicatorHelper.StartProgressIndicator(true, "�����鼮Ŀ¼����");

            var book = ((Button) sender).DataContext as Book;

            targetBook = Model.CheckBookExists(book);
            Model.GetBookIndexPageCompleted += GetBookIndexPageCompleted;
           
            if (targetBook != null)
            {
                if (targetBook.Chapters == null )
                {
                    targetBook.Chapters = PhoneStorage.GetPhoneStorageInstance().GetChaptersByBook(targetBook);
                }
                Model.GetBookSiteBookIndexPageLink(targetBook);
            }
            else
            {
                targetBook = book;
                Model.GetBookSiteBookIndexPageLink(book);
            }
        }

        private void GetBookIndexPageCompleted(object sender, EventArgs e)
        {
            ProgressIndicatorHelper.StopProgressIndicator();
            CrossThreadHelper.CrossThreadMethodCall(() =>
                              {
                                  ProgressIndicatorHelper.StartProgressIndicator(true,"�����鼮Ŀ¼");
                                  NavigationService.Navigate(new Uri("/Views/ChatperDownload.xaml", UriKind.Relative));
                              }
               );
        
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            Model.CancelRunningConnections();
            base.OnNavigatedFrom(e);
            ProgressIndicatorHelper.StopProgressIndicator();
        }
    }
}