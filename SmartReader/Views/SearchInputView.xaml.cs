using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Shell;
using SmartReader.Helper;
using SmartReader.Library.DataContract;
using SmartReader.ViewModel;

namespace SmartReader.Views
{
    public partial class SearchInputPage
    {

        private SearchViewModel _model;
        public SearchViewModel Model
        {
            set 
            { 
                _model = value;
                DataContext = _model;
            }
            get { return _model; }
        }

        public static ProgressIndicator _progressIndicator;

        public SearchInputPage()
        {
            InitializeComponent();
            Model = ModelManager.GetSearchResultModel();
        }

        private void SearchBtnClicked(object sender, RoutedEventArgs e)
        {
            var keyword = KeywordInput.Text.Trim();

            Model.Search(keyword);

            ProgressIndicatorHelper.StartProgressIndicator(true,"搜索中");
        }

        private void BookSelected(object sender, RoutedEventArgs e)
        {
            var searchresult = ((Button) sender).DataContext as SearchResult;
            Model.BookList = null;
            Model.GetBookSiteLink(searchresult);

            if (searchresult != null )
            {
                NavigationService.Navigate(new Uri("/Views/BookSourcePage.xaml", UriKind.Relative));
            }
        }
    }
}