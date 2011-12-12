using System;

namespace SmartReader
{
    public class PageManager
    {
        private static IShell Shell ;
        public static Uri ErrorPage = new Uri("/Views/Error.xaml", UriKind.Relative);
        public static Uri ChapterViewPage = new Uri("/Views/ChapterViewPage.xaml", UriKind.Relative);
        public static Uri BookIndexPage = new Uri("/Views/BookIndexPage.xaml", UriKind.Relative);
        public static Uri BookListPage = new Uri("/BookListPage.xaml", UriKind.Relative);
        public static Uri MainPage = new Uri("/MainPage.xaml", UriKind.Relative);

        public static void RegisterRootPage(IShell shell)
        {
            Shell = shell;
        }

        public static void Navigate (Uri uri)
        {
            Shell.Navigate(uri);
        }
    }
}

