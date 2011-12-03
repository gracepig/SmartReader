using System;
using System.Windows;
using System.Windows.Controls;
using SmartReader.Helper;
using SmartReader.Library.DataContract;
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
            var book = ((Button) sender).DataContext as Book;

            Book targetBook = Model.CheckBookExists(book);

            if (book != null)
            {
                Model.GetBookIndexPageCompleted += GetBookIndexPageCompleted;
                Model.GetBookSiteBookIndexPageLink(targetBook);
                ProgressIndicatorHelper.StartProgressIndicator(true);
            }
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