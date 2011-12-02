<<<<<<< HEAD
﻿using System;
using System.Collections.Generic;
using System.Linq;
using SmartReader.Library.DataContract;
using SmartReader.Library.Interface;

namespace SmartReader.Library.Storage
{
    public class PhoneStorage : IStorage
    {

        private static SmartReaderDataContext _db;

        private static PhoneStorage _instance;
        private PhoneStorage(SmartReaderDataContext db)
        {
            _db = db;
        }

        public static PhoneStorage GetPhoneStorageInstance()
        {
            if (_instance == null)
            {
                if (_db == null)
                    _db = new SmartReaderDataContext(SmartReaderDataContext.DBConnectionString);
                _instance = new PhoneStorage(_db);
            }
            return _instance;
        }

        public void UpdateBook(Book _book)
        {
            if (_book == null) throw new ArgumentNullException("_book");
            var target = from book in _db.Books
                         where book.Name == _book.Name
                         select book;
            if (target.Count() > 0)
            {
                var book = target.First();
                book.Name = _book.Name;
                book.IndexPage = _book.IndexPage;
                book.LastReadChapterId = _book.LastReadChapterId;
                book.LastUpdateTime = _book.LastUpdateTime;
                book.WebSite = _book.WebSite;
                book.Author = _book.Author;
            }

            _db.SubmitChanges();
        }

        public Chapter[] GetChaptersByBook(Book book)
        {
            var chapters = from c in _db.Chapters
                           where c.Book.Name == book.Name
                           select c;

            return chapters.ToArray();
        }

        public void SaveChapters(IEnumerable<Chapter> chapters)
        {
            var temp = new List<Chapter>();
            foreach (var chapter in chapters)
            {
                if (!_db.Chapters.Contains(chapter))
                {
                    temp.Add(chapter);
                }
            }

            _db.Chapters.InsertAllOnSubmit(temp);
            _db.SubmitChanges(); 
        }

        public void SaveChapter(Chapter chapter)
        {
            _db.Chapters.Attach(chapter);
            _db.SubmitChanges();
        }

        public void UpdateDB()
        {
            //var target = from chapter in _db.Chapters
            //             where chapter.Id == _chapter.Id
            //             select chapter;

            //if (target.Count() > 0)
            //{
            //    var chapter = target.First();
            //    chapter.ChapterName = _chapter.ChapterName;
            //    chapter.ChapterUri = _chapter.ChapterUri;
            //    chapter.Content = _chapter.Content;
            //    chapter.LastUpdateTime = _chapter.LastUpdateTime;
            //    chapter.Book = _chapter.Book;
            //}

            _db.SubmitChanges();
        }

        public void SaveBook(Book book)
        {
            if (IsBookExist(book))
            {
                UpdateBook(book);
            }
            else
            {
                _db.Books.InsertOnSubmit(book);
            }
            _db.SubmitChanges();
        }

        public void DeleteBook(Book _book)
        {
            var target = from book in _db.Books
                         where book.Id == _book.Id
                         select book;

            if (target.Count() > 0)
            {
                _db.Books.DeleteOnSubmit(target.First());
            }

            _db.SubmitChanges();
        }

        public IEnumerable<Book> GetAllBooks()
        {
            var books = from book in _db.Books
                        select book;
            return books;
        }

        public void SaveWebSite(WebSite website)
        {
            if (!IsWebSiteExist(website))
            {
                _db.WebSites.InsertOnSubmit(website);
                _db.SubmitChanges();
            }
        }

        public void SaveArticleImages(IEnumerable<ArticleImage> images)
        {
            _db.ArticleImages.InsertAllOnSubmit(images);
            _db.SubmitChanges();
        }

        public void SaveArticleImages(List<ArticleImage> images)
        {
            _db.ArticleImages.InsertAllOnSubmit(images);
            _db.SubmitChanges();
        }

        public IEnumerable<ArticleImage> GetAllArticleImages ()
        {
            var result = from articleImage in _db.ArticleImages
                         select articleImage;
            return result;
        }

        public bool IsBookExist (Book book)
        {
            var books = from b in _db.Books
                        where b.Id == book.Id
                        select b;

            if (books.Any()) { return true;}
            return false;
        }

        public bool IsWebSiteExist(WebSite website)
        {
            var ws = from w in _db.WebSites
                     where w.WebSiteName == website.WebSiteName
                     select w;

            if (ws.Any())
            {
                return true;
            }

            return false;
        }

        public IEnumerable<ArticleImage> GetArticleImageByChapter(Chapter currentChapter)
        {
            var result = from articleImage in _db.ArticleImages
                         where articleImage.ChapterId == currentChapter.Id orderby articleImage.SequenceId
                         select articleImage ;
            return result;
        }
    }
}
=======
﻿using System;
using System.Collections.Generic;
using System.Linq;
using SmartReader.Library.DataContract;
using SmartReader.Library.Interface;

namespace SmartReader.Library.Storage
{
    public class PhoneStorage : IStorage
    {

        private static SmartReaderDataContext _db;

        private static PhoneStorage _instance;
        private PhoneStorage(SmartReaderDataContext db)
        {
            _db = db;
        }

        public static PhoneStorage GetPhoneStorageInstance()
        {
            if (_instance == null)
            {
                if (_db == null)
                    _db = new SmartReaderDataContext(SmartReaderDataContext.DBConnectionString);
                _instance = new PhoneStorage(_db);
            }
            return _instance;
        }

        public void UpdateBook(Book _book)
        {
            if (_book == null) throw new ArgumentNullException("_book");
            var target = from book in _db.Books
                         where book.Name == _book.Name
                         select book;
            if (target.Count() > 0)
            {
                var book = target.First();
                book.Name = _book.Name;
                book.IndexPage = _book.IndexPage;
                book.LastReadChapterId = _book.LastReadChapterId;
                book.LastUpdateTime = _book.LastUpdateTime;
                book.WebSite = _book.WebSite;
                book.Author = _book.Author;
            }

            _db.SubmitChanges();
        }

        public Chapter[] GetChaptersByBook(Book book)
        {
            var chapters = from c in _db.Chapters
                           where c.Book.Name == book.Name
                           select c;

            return chapters.ToArray();
        }

        public void SaveChapters(IEnumerable<Chapter> chapters)
        {
            var temp = new List<Chapter>();
            foreach (var chapter in chapters)
            {
                if (!_db.Chapters.Contains(chapter))
                {
                    temp.Add(chapter);
                }
            }

            _db.Chapters.InsertAllOnSubmit(temp);
            _db.SubmitChanges(); 
        }

        public void SaveChapter(Chapter chapter)
        {
            _db.Chapters.Attach(chapter);
            _db.SubmitChanges();
        }

        public void UpdateDB()
        {
            //var target = from chapter in _db.Chapters
            //             where chapter.Id == _chapter.Id
            //             select chapter;

            //if (target.Count() > 0)
            //{
            //    var chapter = target.First();
            //    chapter.ChapterName = _chapter.ChapterName;
            //    chapter.ChapterUri = _chapter.ChapterUri;
            //    chapter.Content = _chapter.Content;
            //    chapter.LastUpdateTime = _chapter.LastUpdateTime;
            //    chapter.Book = _chapter.Book;
            //}

            _db.SubmitChanges();
        }

        public void SaveBook(Book book)
        {
            if (IsBookExist(book))
            {
                UpdateBook(book);
            }
            else
            {
                _db.Books.InsertOnSubmit(book);
            }
            _db.SubmitChanges();
        }

        public void DeleteBook(Book _book)
        {
            var target = from book in _db.Books
                         where book.Id == _book.Id
                         select book;

            if (target.Count() > 0)
            {
                _db.Books.DeleteOnSubmit(target.First());
            }

            _db.SubmitChanges();
        }

        public IEnumerable<Book> GetAllBooks()
        {
            var books = from book in _db.Books
                        select book;
            return books;
        }

        public void SaveWebSite(WebSite website)
        {
            if (!IsWebSiteExist(website))
            {
                _db.WebSites.InsertOnSubmit(website);
                _db.SubmitChanges();
            }
        }

        public void SaveArticleImages(IEnumerable<ArticleImage> images)
        {
            _db.ArticleImages.InsertAllOnSubmit(images);
            _db.SubmitChanges();
        }

        public void SaveArticleImages(List<ArticleImage> images)
        {
            _db.ArticleImages.InsertAllOnSubmit(images);
            _db.SubmitChanges();
        }

        public IEnumerable<ArticleImage> GetAllArticleImages ()
        {
            var result = from articleImage in _db.ArticleImages
                         select articleImage;
            return result;
        }

        public bool IsBookExist (Book book)
        {
            var books = from b in _db.Books
                        where b.Id == book.Id
                        select b;

            if (books.Any()) { return true;}
            return false;
        }

        public bool IsWebSiteExist(WebSite website)
        {
            var ws = from w in _db.WebSites
                     where w.WebSiteName == website.WebSiteName
                     select w;

            if (ws.Any())
            {
                return true;
            }

            return false;
        }

        public IEnumerable<ArticleImage> GetArticleImageByChapter(Chapter currentChapter)
        {
            var result = from articleImage in _db.ArticleImages
                         where articleImage.ChapterId == currentChapter.Id orderby articleImage.SequenceId
                         select articleImage ;
            return result;
        }
    }
}
>>>>>>> upstream/master
