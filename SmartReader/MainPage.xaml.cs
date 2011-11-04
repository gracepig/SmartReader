using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ImageTools.Controls;
using ImageTools.IO.Gif;
using ImageTools.IO.Png;
using Microsoft.Phone.Controls;
using SmartReader.Library.DataContract;
using SmartReader.Library.Interface;
using SmartReader.Library.Network;
using SmartReader.Library.Parser.BookSite;
using SmartReader.Library.Parser.Sodu;
using SmartReader.Library.Storage;

namespace SmartReader
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            

            this.DataContext = this;
        }


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            //var booksInDB = from Book book in smartReaderDataContext.Books
            //                    select book;

            //this.Books = new ObservableCollection<Book>(booksInDB);


            base.OnNavigatedTo(e);
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
    }
}
