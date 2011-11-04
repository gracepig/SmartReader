using System.Collections.Generic;
using SmartReader.Library.DataContract;
using SmartReader.Library.Storage;

namespace SmartReader.ViewModel
{
    public class BookListModel : ViewModelBase
    {
        private PhoneStorage _storage;

        private IEnumerable<Book> _bookList;
        public IEnumerable<Book> BookList
        {
            set { _bookList = value; RaiseProperyChanged("BookList"); }
            get { return _bookList; }
        }


        public BookListModel()
        {
            _storage = PhoneStorage.GetPhoneStorageInstance();
            _bookList = _storage.GetAllBooks();
        }
    }
}
