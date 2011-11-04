using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartReader.Library.DataContract;

namespace SmartReader.Library.Interface
{
    interface IStorage
    {
        void SaveBook(Book book);
        
        void UpdateBook(Book book);
        
        void DeleteBook(Book book);
        
        IEnumerable<Book> GetAllBooks();

        void SaveChapters(IEnumerable<Chapter> chapter);

        void UpdateChapter(Chapter chapter);

        void SaveWebSite(WebSite website);
    }
}
