using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using ImageTools.IO.Gif;
using ImageTools.IO.Png;
using Microsoft.Phone.Controls;
using SmartReader.Library.DataContract;
using SmartReader.Library.Storage;

namespace SmartReader.Test
{
    public partial class TestStorage : PhoneApplicationPage
    {
        public TestStorage()
        {
            InitializeComponent();
        }

        private SmartReaderDataContext smartReaderDataContext;

        private ObservableCollection<Book> _books;
        public ObservableCollection<Book> Books
        {
            get { return _books; }
            set { _books = value; }
        }

        private void TestLocalDB(object sender, RoutedEventArgs e)
        {

            //AddOneRecord();

            //ReadBooksFromDB();
            ReadBooksFromDB();

            UpdateOneRecord();

            ReadBooksFromDB();
        }


        private void AddOneRecord()
        {
            var book = new Book();
            book.Author = "Charlie";
            book.IndexPage = new Uri(String.Format("http://www.xiaoshuo999.org/files/article/html/0/421/1077782.html"), UriKind.Absolute);
            book.LastReadChapterId = 0;
            book.Name = "宰执天下";
            book.LastUpdateTime = DateTime.Today;
            book.WebSite = new WebSite() { LandingPage = book.IndexPage.ToString(), SearchEntry = null, WebSiteName = "起点" };
            //db.WebSites.InsertOnSubmit(book.WebSite);
            //db.Books.InsertOnSubmit(book);
            //db.SubmitChanges();
        }

        private void UpdateOneRecord()
        {
            book.Name = "赘婿";
            //db.SubmitChanges();
        }

        private Book book;

        void ReadBooksFromDB()
        {
            //Books = new ObservableCollection<Book>(db.Books);
            //book = db.Books.First();
            //book.Name = "无尽武装";
            //db.SubmitChanges();
        }

        private PhoneStorage Storage;

        public void PrepareTempDB()
        {

            using (var db1 = new SmartReaderDataContext("isostore:/SmartReader.sdf"))
            {
                if (db1.DatabaseExists() == false)
                {
                    db1.CreateDatabase();
                }
            }

            Storage = PhoneStorage.GetPhoneStorageInstance();

            WebSite website = CreateFakeWebSite();
            Book book = CreateFakeBook(website);
            Chapter chapter = CreateFakeChapter(book);
            CreateFakeArticleImage(chapter);
        }

        private WebSite CreateFakeWebSite()
        {
            var WebSite = new WebSite() { LandingPage = "Fake", SearchEntry = null, WebSiteName = "起点" };
            Storage.SaveWebSite(WebSite);
            return WebSite;
        }

        private Book CreateFakeBook(WebSite webSite)
        {
            var book = new Book();
            book.Author = "Charlie";
            book.IndexPage = new Uri(String.Format("http://www.xiaoshuo999.org/files/article/html/0/421/1077782.html"), UriKind.Absolute);
            book.LastReadChapterId = 0;
            book.Name = "宰执天下";
            book.LastUpdateTime = DateTime.Today;
            book.WebSite = webSite;
            Storage.SaveBook(book);
            //db.Books.InsertOnSubmit(book);
            return book;
        }

        private Chapter CreateFakeChapter(Book book)
        {
            var chapter = new Chapter();
            chapter.Book = book;
            chapter.Content = "String Content";
            chapter.ChapterUri = new Uri("http://www.tszw.net/files/article/html/79/79194/2417331.html", UriKind.Absolute);
            chapter.ChapterName = "敌手的面目";
            chapter.LastUpdateTime = DateTime.Now;
            chapter.IsImageContent = true;
            //db.Chapters.InsertOnSubmit(chapter);
            Storage.SaveChapters(new Chapter[] { chapter });
            return chapter;
        }

        private ArticleImage CreateFakeArticleImage(Chapter chapter)
        {
            var image = new ArticleImage();
            image.Chapter = chapter;

            var bitMapImage = new BitmapImage();

            GifDecoder gd = new GifDecoder();
            var img = new ImageTools.ExtendedImage();

            gd.Decode(img, Application.GetResourceStream(
                new Uri("/SmartReader.Library;component/Resource/8004887.gif", UriKind.Relative)
                ).Stream);

            var png = new PngEncoder();
            BitmapImage bitmap = new BitmapImage();
            bitmap.CreateOptions = BitmapCreateOptions.None;
            using (MemoryStream stream = new MemoryStream())
            {
                png.Encode(img, stream);
                bitmap.SetSource(stream);
            }

            image.ImageBytes = ConvertToBytes(bitmap);
            Storage.SaveArticleImages(new[] { image });

            return image;
        }

        private void PrepareDB(object sender, RoutedEventArgs e)
        {
            PrepareTempDB();
        }

        public static byte[] ConvertToBytes(BitmapImage bitmapImage)
        {
            byte[] data = null;
            using (MemoryStream stream = new MemoryStream())
            {
                WriteableBitmap wBitmap = new WriteableBitmap(bitmapImage);
                wBitmap.SaveJpeg(stream, wBitmap.PixelWidth, wBitmap.PixelHeight, 0, 100);
                stream.Seek(0, SeekOrigin.Begin);
                data = stream.GetBuffer();
            }

            return data;
        }

        public static BitmapImage ConvertToBitmapImage(byte[] bytes)
        {
            MemoryStream stream = new MemoryStream(bytes);
            BitmapImage image = new BitmapImage();
            image.SetSource(stream);

            return image;
        }

        private void LoadImage(object sender, RoutedEventArgs e)
        {
            var image = Storage.GetAllArticleImages().First();

            this.ImageContainer.Source = ConvertToBitmapImage(image.ImageBytes);
        }
    }
}