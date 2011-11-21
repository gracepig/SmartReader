using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using SmartReader.Library.Helper;

namespace SmartReader.Library.DataContract
{
    [Table]
    public class Book : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public Book()
        {
        }

        private int _id;
        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int Id
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    NotifyPropertyChanging("Id");
                    _id = value;
                    NotifyPropertyChanged("Id");
                }
            }
        }

        [Column(IsVersion = true)]
        private Binary version;

        private Chapter[] _chapters;
        public Chapter[] Chapters
        {
            set 
            {
                _chapters = value;

                if (_displayingChapters == null)
                {

                    int displayStartChapterIndex = 0;
                    if (LastReadChapterId > 0)
                    {
                        for(var i =0 ; i< Chapters.Length; i++)
                        {
                            if (Chapters[i].Id == LastReadChapterId)
                            {
                                displayStartChapterIndex = i;
                            }
                        }
                    }

                    _displayingChapters = (from c in Chapters
                                           select c).Skip(displayStartChapterIndex).Take(Constants.ChapterShowInOnePage).ToArray();
                    currentDisplayingChapterIndex = 0;
                }
            }
            get { return _chapters; }
        }

        private int currentDisplayingChapterIndex;
        private Chapter[] _displayingChapters;
        public Chapter[] DisplayingChapters
        {
            set
            {
                _displayingChapters = value;
                NotifyPropertyChanged("DisplayingChapters"); 
            }
            get { return _displayingChapters; }
        }

        public Chapter[] DownloadChapters { get; set; }

        private string _name;
        [Column]
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    NotifyPropertyChanging("Name");
                    _name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        private string _author;
        [Column]
        public string Author
        {
            get { return _author; }
            set
            {
                if (_author != value)
                {
                    NotifyPropertyChanging("Author");
                    _author = value;
                    NotifyPropertyChanged("Author");
                }
            }
        }

        [Column]
        private int _webSiteId;

        // Entity reference, to identify the ToDoCategory "storage" table
        private EntityRef<WebSite> _website;


        [Association(Storage = "_website", ThisKey = "_webSiteId", OtherKey = "Id", IsForeignKey = true)]
        public WebSite WebSite
        {
            get { return _website.Entity; }
            set
            {
                NotifyPropertyChanging("WebSite");
                _website.Entity = value;

                if (value != null)
                {
                    _webSiteId = value.Id;
                }

                NotifyPropertyChanging("WebSite");
            }
        }

        [Column(Name = "IndexPage", DbType = "nvarchar(255)")]
        private string _indexPage; 

        public Uri IndexPage
        {
            get
            {
                return new Uri(_indexPage);
            }
            set
            {
                _indexPage = value.AbsoluteUri;
            }
        }


        private DateTime _lastUpdateTime = DateTime.Now;

        [Column(CanBeNull = true)]
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

        private int _lastReadChapterId;
        [Column]
        public int LastReadChapterId
        {
            set
            {
                NotifyPropertyChanging("LastReadChapterId");
                _lastReadChapterId = value;
                NotifyPropertyChanged("LastReadChapterId");
            }
            get { return _lastReadChapterId;  }
        }

        private string _lastestUpdateChapterName;
        [Column]
        public string LastestUpdateChapterName
        {
            set
            {
                NotifyPropertyChanging("LastestUpdateChapterName");
                _lastestUpdateChapterName = value;
                NotifyPropertyChanged("LastestUpdateChapterName");
            }
            get { return _lastestUpdateChapterName; }
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

        public string RootUrl;

        public void NextPageChapters()
        {
            DisplayingChapters = Chapters.Skip(currentDisplayingChapterIndex + Constants.ChapterShowInOnePage).Take(Constants.ChapterShowInOnePage).ToArray();
            currentDisplayingChapterIndex += Constants.ChapterShowInOnePage;
        }

        public void PreviousPageChapters()
        {
            DisplayingChapters = Chapters.Skip(currentDisplayingChapterIndex - Constants.ChapterShowInOnePage).Take(Constants.ChapterShowInOnePage).ToArray();
            currentDisplayingChapterIndex -= Constants.ChapterShowInOnePage;
        }

        public void FirstPageChapters()
        {
            DisplayingChapters = Chapters.Take(Constants.ChapterShowInOnePage).ToArray();
            currentDisplayingChapterIndex = 0;
        }

        public void LastPageChapters()
        {
            DisplayingChapters = Chapters.Skip(Chapters.Count() - Constants.ChapterShowInOnePage).ToArray();
            currentDisplayingChapterIndex = Chapters.Count() - Constants.ChapterShowInOnePage;
        }
    }
}
