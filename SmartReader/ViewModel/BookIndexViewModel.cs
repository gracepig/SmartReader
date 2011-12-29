using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using SmartReader.Helper;
using SmartReader.Library.DataContract;
using SmartReader.Library.Helper;
using SmartReader.Library.Network;
using SmartReader.Library.Parser.BookSite;
using SmartReader.Library.Storage;

namespace SmartReader.ViewModel
{
    public class BookIndexViewModel : INotifyPropertyChanged 
    {
        private Book _book;
        public Book Book
        {
            set 
            { 
                _book = value; 
                RaiseProperyChanged("Book");
            }
            get
            {
                return _book;
            }
        }

        public int DownloadStartIndex { set; get; }
        public int ChapterToBeDownloadedCount { set; get; }

        public List<Chapter> Chapters { set; get; }

        public BookIndexViewModel (Book targetBook)
        {
            Book = targetBook;
            DownloadStartIndex = 1;

            int chapterToBeDownloadedCount;
            ChapterToBeDownloadedCount = !AppSetting.TryGetSetting("DefaultDownloadBatchSize", out chapterToBeDownloadedCount) ? 10 : chapterToBeDownloadedCount;
        }

        private readonly PhoneStorage _storage = PhoneStorage.GetPhoneStorageInstance();

        public void DownloadBookContents()
        {
            _storage.SaveWebSite(Book.WebSite);
            _storage.SaveBook(Book);
            _storage.SaveChapters(Book.Chapters);
            ThreadPool.QueueUserWorkItem(DownloadChapters);
        }

        private readonly Object _thisLock = new Object();
        private int _batchCompleteCount;
        private const int ThreadCount = 2;

        public void DownloadChapters(object s)
        {
            Debug.Assert(DownloadStartIndex > 0);

            var downloadEndIndex = DownloadStartIndex + ChapterToBeDownloadedCount - 1 > Book.Chapters.Length
                                       ? Book.Chapters.Length
                                       : DownloadStartIndex + ChapterToBeDownloadedCount - 1;

            var i = DownloadStartIndex - 1;
            var updatedChapters = new List<Chapter>();

            do
            {
                var resetEvents = i + ThreadCount <= downloadEndIndex
                                      ? new ManualResetEvent[ThreadCount]
                                      : new ManualResetEvent[1];
                for (var j = 0; i + j < downloadEndIndex && j < resetEvents.Length; j++)
                {
                    var chapter = Book.Chapters[i + j];
                    updatedChapters.Add(chapter);
                    resetEvents[j] = new ManualResetEvent(false);
                    ThreadPool.QueueUserWorkItem(DownloadChapter,
                                                 new DownloadTask {TaskChapter = chapter, 
                                                     ResetEvent = resetEvents[j],
                                                     BatchCount = resetEvents.Length});
                }
                
                WaitHandle.WaitAny(resetEvents);

                if ( i + ThreadCount <= downloadEndIndex)
                {
                    i += ThreadCount;
                }
                else
                {
                    i++;
                }

                ProgressIndicatorHelper.SetIndicatorValue((i-DownloadStartIndex + 1)/(double) ChapterToBeDownloadedCount);
                
                if ((i - DownloadStartIndex + 1) >= ChapterToBeDownloadedCount)
                {
                    ProgressIndicatorHelper.StopProgressIndicator();
                }

            } while (i < downloadEndIndex);
            
            _storage.UpdateDB();
        }

        public void DownloadChapter(object s)
        {
            var cha = (DownloadTask)s;
            var downloader = new HttpContentDownloader();
            downloader.Download(cha.TaskChapter.ChapterUri, ar =>
            {
                try
                {
                    //At this step, we can get the index page in the search engine 
                    var state = (RequestState)ar.AsyncState;
                    var response = (HttpWebResponse)state.Request.EndGetResponse(ar);
                    response.GetResponseStream();

                    var parser = new WebSiteBookContentPageParser();
                    parser.Parse(response.GetResponseStream(), cha.TaskChapter);

                    lock (_thisLock)
                    {
                        _batchCompleteCount++;
                    }

                    if (_batchCompleteCount == cha.BatchCount)
                    {
                        _batchCompleteCount = 0;
                        cha.ResetEvent.Set();
                    }

                }catch (WebException e)
                {
                    //TODO need to recover from exception
                    ExceptionHandler.HandleException(e);
                }
            });
        }

        public void NextPageChapters()
        {
            Book.NextPageChapters();
        }

        public void PreviousPageChapters()
        {
            Book.PreviousPageChapters();
        }

        public void FirstPage()
        {
            Book.FirstPageChapters();
        }

        public void LastPage()
        {
            Book.LastPageChapters();
        }
        
        public void Refresh()
        {

            ProgressIndicatorHelper.StartProgressIndicator(true);
            var downloader = new HttpContentDownloader();
            downloader.Download(Book.IndexPage,
                ar =>
                {
                    //At this step, we can get the index page in the search engine 
                    var state = (RequestState)ar.AsyncState;
                    var response = (HttpWebResponse)state.Request.EndGetResponse(ar);
                    response.GetResponseStream();

                    Book.RootUrl = UrlHelper.GetRootUrlString(response.ResponseUri);
                    var parser = new WebsiteBookIndexPageParser();

                    var temp = new Book {RootUrl = Book.RootUrl};
                    parser.Parse(response.GetResponseStream(), temp );

                    var newChapters = new List<Chapter>();
                    newChapters.AddRange(
                            temp.Chapters.Where
                            (
                                chapter => !Book.Chapters.Any(c => c.ChapterName == chapter.ChapterName)
                            ));

                    if (newChapters.Count > 0)
                    {
                        var totalChapters = new List<Chapter>();
                        totalChapters.AddRange(Book.Chapters);
                        totalChapters.AddRange(newChapters);

                        Book.Chapters = totalChapters.ToArray();
                        RaiseProperyChanged("Book");
                    }
                    else
                    {
                        CrossThreadHelper.CrossThreadMethodCall(() => MessageBox.Show("本书尚未有新的章节"));
                    }

                    ProgressIndicatorHelper.StopProgressIndicator();
                });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                SmartDispatcher.BeginInvoke(() => PropertyChanged(this, e));
            }
        }

        private void RaiseProperyChanged(string name)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(name));
        }

        public class DownloadTask
        {
            public Chapter TaskChapter;
            public ManualResetEvent ResetEvent;
            public int BatchCount;
        }
    }
}
