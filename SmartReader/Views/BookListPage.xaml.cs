using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using SmartReader.Library.DataContract;
using SmartReader.ViewModel;

namespace SmartReader.Views
{
    public partial class BookListPage
    {
        private BookListModel _model;
        public BookListModel Model
        {
            set 
            { 
                _model = value;
                DataContext = _model;
            }

            get { return _model; }
        }

        public BookListPage()
        {
            InitializeComponent();
            Model = ModelManager.GetBookListModel();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Model.RefreshBookList();
        }

        private void BookSelected(object sender, RoutedEventArgs e)
        {
            ModelManager.GetBookIndexModel().Book = ((Button) sender).DataContext as Book;

            NavigationService.Navigate(new Uri("/Views/BookIndexPage.xaml", UriKind.Relative));
        }

        private void SettingClicked(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/SettingsPage.xaml", UriKind.Relative));
        }

        private void SearchClicked(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/SearchInputView.xaml", UriKind.Relative));
        }

        private Grid _temp;
        private void AnimateOneItem(object sender, GestureEventArgs e)
        {
            var grid = sender as Grid;
            if (null != grid)
            {
                var ct = new CompositeTransform();
                ct.TranslateX = 10;
                grid.RenderTransform = ct;
                _temp = grid;
            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (_temp != null )
            {
                var ct = new CompositeTransform();
                ct.TranslateX = -10;
                _temp.RenderTransform = ct;
                _temp = null;
            }
            base.OnNavigatedFrom(e);
        }
    }
}