using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Phone.Shell;
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
        }

        private void SearchBtnClicked(object sender, RoutedEventArgs e)
        {
            var keyword = KeywordInput.Text.Trim();

            Model.Search(keyword);
        }

        private void BookSelected(object sender, RoutedEventArgs e)
        {
            var searchresult = ((Button) sender).DataContext as SearchResult;

            Model.GetBookSiteLink(searchresult);

            if (searchresult != null )
            {
                NavigationService.Navigate(new Uri("/Views/BookSourcePage.xaml", UriKind.Relative));
            }
        }
    }
}