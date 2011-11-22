using System;
using System.Windows;
using SmartReader.Helper;
using SmartReader.Library.Storage;

namespace SmartReader
{
    public partial class MainPage : IShell
    {

        // Constructor
        public MainPage()
        {
            CrossThreadHelper.UIThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId; 
            InitializeComponent();
            this.DataContext = this;
            PageManager.RegisterRootPage(this);
        }


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            //var booksInDB = from Book book in smartReaderDataContext.Books
            //                    select book;

            //this.Books = new ObservableCollection<Book>(booksInDB);

            base.OnNavigatedTo(e);
            PageManager.Navigate(PageManager.BookListPage);
        }


        private void GotoParserTestPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Test/TestParser.xaml", UriKind.Relative));
        }

        private void GotoStorageTestPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Test/TestStorage.xaml", UriKind.Relative));
        }

        private void GotoParserAndStorageTestPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Test/ParserAndStorageIntegrationTest.xaml", UriKind.Relative));
        }

        private void GotoTestViewPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Test/ViewTest.xaml", UriKind.Relative));
        }

        public void ShowExceptionError(Exception ex)
        {
            //this.ErrorMessage.IsOpen = true;
           // MessageBox.Show(ex.Message);
        }

        public void Navigate (Uri uri)
        {
            NavigationService.Navigate(uri);
        }

        private void ClearDB(object sender, RoutedEventArgs e)
        {
            using (var db1 = new SmartReaderDataContext("isostore:/SmartReader.sdf"))
            {
                if (db1.DatabaseExists() == false)
                {
                    db1.DeleteDatabase();
                    db1.CreateDatabase();
                }
            }
        }
    }
}
