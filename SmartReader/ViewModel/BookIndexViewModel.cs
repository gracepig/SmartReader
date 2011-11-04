using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Threading;
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

            var chapterToBeDownloadedCount = ChapterToBeDownloadedCount;
            if (!AppSetting.TryGetSetting<int>("DefaultDownloadBatchSize", out chapterToBeDownloadedCount))
            {
                ChapterToBeDownloadedCount = 10;
            }
            else
            {
                ChapterToBeDownloadedCount = chapterToBeDownloadedCount;
            }
        }

        private readonly PhoneStorage _storage = PhoneStorage.GetPhoneStorageInstance();

        public void DownloadBookContents()
        {
            _storage.SaveWebSite(Book.WebSite);
            _storage.SaveBook(Book);
            ThreadPool.QueueUserWorkItem(DownloadChapters);
        }

        private Object thisLock = new Object();
        private int BatchCompleteCount = 0;
        private int ThreadCount = 2;

        public void DownloadChapters(object s)
        {
            Debug.Assert(DownloadStartIndex > 0);

            var downloadEndIndex = DownloadStartIndex + ChapterToBeDownloadedCount > Book.Chapters.Length
                                       ? Book.Chapters.Length
                                       : DownloadStartIndex + ChapterToBeDownloadedCount;

            var i = 0;

            do
            {
                var resetEvents = i + ThreadCount < downloadEndIndex
                                      ? new ManualResetEvent[ThreadCount]
                                      : new ManualResetEvent[1];
                for (var j = DownloadStartIndex - 1; j < ThreadCount && i + j < downloadEndIndex; j++)
                {
                    var chapter = Book.Chapters[i + j];
                    resetEvents[j] = new ManualResetEvent(false);
                    ThreadPool.QueueUserWorkItem(DownloadChapter,
                                                 new DownloadTask {TaskChapter = chapter, ResetEvent = resetEvents[j]});
                }
                
                WaitHandle.WaitAny(resetEvents);

                i += ThreadCount;

                ProgressIndicatorHelper.SetIndicatorValue(i/(double) ChapterToBeDownloadedCount);
                
                if (i == ChapterToBeDownloadedCount)
                {
                    ProgressIndicatorHelper.StopProgressIndicator();
                    _storage.SaveChapters(Book.Chapters);
                }

            } while (i < downloadEndIndex);
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

                    lock (thisLock)
                    {
                        BatchCompleteCount++;
                    }

                    if (BatchCompleteCount == ThreadCount)
                    {
                        BatchCompleteCount = 0;
                        cha.ResetEvent.Set();
                    }

                }catch (WebException e)
                {
                    //TODO need to recover from exception
                    ExceptionHandler.HandleException(e);
                }
            });
        }

        /// <summary>
        /// Download single chapter, should be active from the ChapterView Page
        /// </summary>
        /// <param name="chapter"></param>
        /// <param name="callback"></param>
        public void DownloadSingleChapter (Chapter chapter, Action callback)
        {
            var downloader = new HttpContentDownloader();
            downloader.Download(chapter.ChapterUri, ar =>
            {
                try
                {
                    //At this step, we can get the index page in the search engine 
                    var state = (RequestState) ar.AsyncState;
                    var response = (HttpWebResponse) state.Request.EndGetResponse(ar);
                    response.GetResponseStream();

                    var parser = new WebSiteBookContentPageParser();
                    chapter.Downloaded = true;
                    parser.Parse(response.GetResponseStream(), chapter);

                    if (callback != null )
                    {
                        callback();
                    }
                }
                catch (WebException e)
                {
                    //TODO need to recover from exception
                    ExceptionHandler.HandleException(e);
                }
            });
        }

        public class DownloadTask
        {
            public Chapter TaskChapter;
            public ManualResetEvent ResetEvent;
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
            {
                SmartDispatcher.BeginInvoke(() => PropertyChanged(this, e));

            }
        }

        private void RaiseProperyChanged(string name)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(name));
        }
    }
}
