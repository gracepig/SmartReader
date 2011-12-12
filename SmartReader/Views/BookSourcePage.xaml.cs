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

        private void BookSelected(object sender, RoutedEventArgs e)
        {
            if (ModelManager.GetBookIndexModel().Book != null )
            {
                ModelManager.GetBookIndexModel().Book.DisplayingChapters = null;    
            }

            var book = ((Button) sender).DataContext as Book;

            Book targetBook = Model.CheckBookExists(book);
            Model.GetBookIndexPageCompleted += GetBookIndexPageCompleted;
           
            if (targetBook != null)
            {
                targetBook.DisplayingChapters = null;
                if (targetBook.Chapters == null )
                {
                    targetBook.Chapters = PhoneStorage.GetPhoneStorageInstance().GetChaptersByBook(targetBook);
                }
                Model.GetBookSiteBookIndexPageLink(targetBook);
            }
            else
            {
                Model.GetBookSiteBookIndexPageLink(book);
            }

            ProgressIndicatorHelper.StartProgressIndicator(true);
        }

        private void GetBookIndexPageCompleted(object sender, EventArgs e)
        {
            ProgressIndicatorHelper.StopProgressIndicator();
            CrossThreadHelper.CrossThreadMethodCall(() => 
                NavigationService.Navigate(new Uri("/Views/ChatperDownload.xaml", UriKind.Relative))
                );
        }
    }
}