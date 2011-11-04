using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Windows.Media.Imaging;

namespace SmartReader.Library.DataContract
{
    [Table]
    public class Chapter : INotifyPropertyChanged , INotifyPropertyChanging
    {
        public int _id;
        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int Id
        {
            get { return _id; }
            set
            {
                NotifyPropertyChanging("Id");
                _id = value;
                NotifyPropertyChanged("Id");
            }
        }

        private bool _downloaded = false;
        [Column]
        public bool Downloaded
        {
            set
            {
                _downloaded = value;
                NotifyPropertyChanged("Downloaded");
            }

            get
            {
                return _downloaded;
            }
        }

        [Column(IsVersion = true)]
        private Binary version;

        [Column]
        public bool IsImageContent{ set; get; }

        private string _chapterName;
        [Column]
        public String ChapterName
        {
            set
            {
                NotifyPropertyChanging("ChapterName");
                _chapterName = value;
                NotifyPropertyChanged("ChapterName");
            }
            get { return _chapterName; }
        }



        [Column(Name = "ChapterUri", DbType = "nvarchar(255)")]
        private string _chapterUri;

        public Uri ChapterUri
        {
            get
            {
                return new Uri(_chapterUri);
            }
            set
            {
                _chapterUri = value.AbsoluteUri;
            }
        }

        private DateTime _lastUpdateTime;
        [Column]
        public DateTime LastUpdateTime
        {
            set
            {
                NotifyPropertyChanging("LastUpdateTime");
                _lastUpdateTime = value;
                NotifyPropertyChanged("LastUpdateTime");
            }
            get { return _lastUpdateTime; }
        }


        [Column]
        private int _bookId;

        // Entity reference, to identify the ToDoCategory "storage" table
        private EntityRef<Book> _book;


        [Association(Storage = "_book", ThisKey = "_bookId", OtherKey = "Id", IsForeignKey = true)]
        public Book Book
        {
            get { return _book.Entity; }
            set
            {
                NotifyPropertyChanging("Book");
                _book.Entity = value;

                if (value != null)
                {
                    _bookId = value.Id;
                }

                NotifyPropertyChanging("Book");
            }
        }

        private string _content;

        [Column]
        public string Content
        {
            set
            {
                NotifyPropertyChanging("Content");
                _content = value;
                NotifyPropertyChanged("Content");
            }
            get { return _content; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangingEventHandler PropertyChanging;

        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        // Used to notify the page that a data context property changed
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
