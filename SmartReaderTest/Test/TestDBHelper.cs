using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ImageTools.IO.Gif;
using ImageTools.IO.Png;
using SmartReader.Library.DataContract;
using SmartReader.Library.Storage;

namespace SmartReader.Test
{
    public class TestDBHelper
    {

        private TestDBHelper ()
        {
        }

        private static TestDBHelper dbHelper;
        public static TestDBHelper GetTestDBInstance ()
        {
            if (dbHelper == null )
                dbHelper = new TestDBHelper();

            return dbHelper;
        }

        public Chapter TestTextChapter = null;
        public Chapter TestImageChapter = null;
        public WebSite TestWebSite = null;
        public Book TestBook = null;

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

            TestWebSite = GetFakeWebSite();
            TestBook = GetFakeBook(TestWebSite);
            TestTextChapter = GetFakeTextChapter(TestBook);
            TestImageChapter = GetFakeImageChapter(TestBook);
            TestBook.Chapters = new[]{ TestTextChapter, TestImageChapter};

            Storage.SaveWebSite(TestWebSite);
            Storage.SaveBook(TestBook);
            Storage.SaveChapters(TestBook.Chapters);
            //CreateFakeArticleImage(chapter);
        }

        private WebSite GetFakeWebSite()
        {
            var WebSite = new WebSite() { LandingPage = "Fake", SearchEntry = null, WebSiteName = "起点" };
            //Storage.SaveWebSite(WebSite);
            return WebSite;
        }

        private Book GetFakeBook(WebSite webSite)
        {
            var book = new Book();
            book.Author = "Charlie";
            book.IndexPage = new Uri(String.Format("http://www.xiaoshuo999.org/files/article/html/0/421/1077782.html"), UriKind.Absolute);
            book.LastReadChapterId = 0;
            book.Name = "宰执天下";
            book.LastUpdateTime = DateTime.Today;
            book.WebSite = webSite;
            book.RootUrl = "http://www.wanshuba.com";
            //Storage.SaveBook(book);
            //db.Books.InsertOnSubmit(book);
            return book;
        }

        private Chapter GetFakeTextChapter(Book book)
        {
            var chapter = new Chapter();
            chapter.Book = book;
            chapter.SaveContent1 = "String Content";
            chapter.ChapterUri = new Uri("http://www.tszw.net/files/article/html/79/79194/2417331.html", UriKind.Absolute);
            chapter.ChapterName = "敌手的面目";
            chapter.LastUpdateTime = DateTime.Now;
            chapter.IsImageContent = false;
            //db.Chapters.InsertOnSubmit(chapter);
            return chapter;
        }

        private Chapter GetFakeImageChapter(Book book)
        {
            var chapter = new Chapter();
            chapter.Book = book;
            chapter.SaveContent1 = "String Content";
            chapter.ChapterUri = new Uri("http://www.tszw.net/files/article/html/79/79194/2417331.html", UriKind.Absolute);
            chapter.ChapterName = "敌手的面目";
            chapter.LastUpdateTime = DateTime.Now;
            chapter.IsImageContent = true;
            //db.Chapters.InsertOnSubmit(chapter);
            return chapter;
        }

        //private ArticleImage CreateFakeArticleImage(Chapter chapter)
        //{
        //    var image = new ArticleImage();
        //    //  image.Chapter = chapter;

        //    var bitMapImage = new BitmapImage();

        //    GifDecoder gd = new GifDecoder();
        //    var img = new ImageTools.ExtendedImage();

        //    gd.Decode(img, Application.GetResourceStream(
        //        new Uri("/SmartReader.Library;component/Resource/8004887.gif", UriKind.Relative)
        //        ).Stream);

        //    var png = new PngEncoder();
        //    BitmapImage bitmap = new BitmapImage();
        //    bitmap.CreateOptions = BitmapCreateOptions.None;
        //    using (MemoryStream stream = new MemoryStream())
        //    {
        //        png.Encode(img, stream);
        //        bitmap.SetSource(stream);
        //    }

        //    image.ImageBytes = ConvertToBytes(bitmap);
        //    Storage.SaveArticleImages(new[] { image });

        //    return image;
        //}
    }
}
