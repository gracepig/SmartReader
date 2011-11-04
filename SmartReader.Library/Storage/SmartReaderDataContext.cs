using System;
using System.Data.Linq;
using System.Linq;
using SmartReader.Library.DataContract;

namespace SmartReader.Library.Storage
{
    public class SmartReaderDataContext : DataContext
    {

        // Specify the connection string as a static, used in main page and app.xaml.
        public static string DBConnectionString = "Data Source=isostore:/SmartReader.sdf";

        // Pass the connection string to the base class.
        public SmartReaderDataContext(string connectionString) : base(connectionString) { }

        // Specify a single table for the to-do items.
        public Table<Book> Books;

        public Table<WebSite> WebSites;

        public Table<Chapter> Chapters;

        public Table<ArticleImage> ArticleImages;
    }
}
