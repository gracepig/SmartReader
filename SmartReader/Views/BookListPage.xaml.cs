using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using SmartReader.Library.DataContract;
using SmartReader.Library.Storage;
using SmartReader.ViewModel;

namespace SmartReader.Views
{
    public partial class BookListPage : PhoneApplicationPage
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

        private void BookSelected(object sender, RoutedEventArgs e)
        {
            ModelManager.GetBookIndexModel().Book = ((Button) sender).DataContext as Book;

            NavigationService.Navigate(new Uri("/Views/BookIndexPage.xaml", UriKind.Relative));
        }
    }
}