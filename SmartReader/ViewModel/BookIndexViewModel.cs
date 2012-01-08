using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class BookIndexViewModel : ViewModelBase  
    {
        private Book _book;
        public Book Book
        {
            set 
            { 
                _book = value;
                if (_displayingChapters == null)
                {
                    int displayStartChapterIndex = 0;
                    if (_book.LastReadChapterId > 0)
                    {
                        for (var i = 0; i < _book.Chapters.Length; i++)
                        {
                            if (_book.Chapters[i].Id == _book.LastReadChapterId)
                            {
                                displayStartChapterIndex = i;
                            }
                        }
                    }

                    if (_book.Chapters != null) 
                    {
                        DisplayingChapters = (from c in _book.Chapters
                                              select c).Skip(displayStartChapterIndex)
                                                       .Take(Constants.ChapterShowInOnePage).ToArray();
                    }
                   currentDisplayingChapterIndex = 0;
                }
                RaiseProperyChanged("Book");
            }
            get
            {
                return _book;
            }
        }

        private int currentDisplayingChapterIndex;
        private Chapter [] _displayingChapters;
        public Chapter [] DisplayingChapters
        {
            set
            {
                _displayingChapters = value;
                RaiseProperyChanged("DisplayingChapters");
            }
            get { return _displayingChapters; }
        }
        
        private readonly Object _thisLock = new Object();
        private int _batchCompleteCount;
        private const int ThreadCount = 2;
        public int DownloadStartIndex { set; get; }
        public int ChapterToBeDownloadedCount { set; get; }
        
        private readonly PhoneStorage _storage = PhoneStorage.GetPhoneStorageInstance();
        
        private readonly List<HttpContentDownloader> DownloaderList = new List<HttpContentDownloader>();

        public BookIndexViewModel (Book targetBook)
        {
            if (targetBook != null ) Book = targetBook;
            DownloadStartIndex = 1;

            int chapterToBeDownloadedCount;
            ChapterToBeDownloadedCount = !AppSetting.TryGetSetting("DefaultDownloadBatchSize", out chapterToBeDownloadedCount) ?
                                         10 : chapterToBeDownloadedCount;
        }

        public void DownloadBookContents()
        {
            _storage.SaveWebSite(Book.WebSite);
            _storage.SaveBook(Book);
            _storage.SaveChapters(Book.Chapters);
            ThreadPool.QueueUserWorkItem(DownloadChapters);
        }

        public void DownloadChapters(object s)
        {
            Debug.Assert(DownloadStartIndex > 0);

            ProgressIndicatorHelper.CrossThreadStartProgressIndicator(false, "批量下载中");
            ProgressIndicatorHelper.SetIndicatorValue(0);
            var downloadEndIndex = DownloadStartIndex + ChapterToBeDownloadedCount - 1 > Book.Chapters.Length
                                       ? Book.Chapters.Length
                                       : DownloadStartIndex + ChapterToBeDownloadedCount - 1;

            var i = DownloadStartIndex - 1;
            var updatedChapters = new List<Chapter>();

            do
            {
                WaitHandle[] resetEvents = i + ThreadCount <= downloadEndIndex
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
            DownloaderList.Add(downloader);
            downloader.Download(cha.TaskChapter.ChapterUri, ar =>
            {
                try
                {
                    //At this step, we can get the index page in the search engine 
                    var state = (RequestState)ar.AsyncState;
                    state.stopTimer = true;
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
                        ((ManualResetEvent) cha.ResetEvent).Set();
                    }

                }catch (WebException e)
                {
                    ProgressIndicatorHelper.StopProgressIndicator();
                    if (e.Status == WebExceptionStatus.RequestCanceled)
                    {
                        throw new TimeoutException(String.Format("连接{0}超时", cha.TaskChapter.Book.WebSite.WebSiteName));
                    }
                    //TODO need to recover from exception
                    ExceptionHandler.HandleException(e);
                }
            });
        }
      
        public void NextPage()
        {
            DisplayingChapters = Book.Chapters.Skip(currentDisplayingChapterIndex + Constants.ChapterShowInOnePage)
                                 .Take(Constants.ChapterShowInOnePage)
                                 .ToArray();
            currentDisplayingChapterIndex += Constants.ChapterShowInOnePage;
        }

        public void PreviousPage()
        {
            DisplayingChapters = Book.Chapters.Skip(currentDisplayingChapterIndex - Constants.ChapterShowInOnePage)
                                 .Take(Constants.ChapterShowInOnePage)
                                 .ToArray();
            currentDisplayingChapterIndex -= Constants.ChapterShowInOnePage;
        }

        public void FirstPage()
        {
            DisplayingChapters = Book.Chapters.Take(Constants.ChapterShowInOnePage).ToArray();
            currentDisplayingChapterIndex = 0;
        }

        public void LastPage()
        {
            DisplayingChapters = Book.Chapters.Skip(_book.Chapters.Count() - Constants.ChapterShowInOnePage).ToArray();
            currentDisplayingChapterIndex = _book.Chapters.Count() - Constants.ChapterShowInOnePage;
        }
        
        public void Refresh()
        {
            ProgressIndicatorHelper.StartProgressIndicator(true , "更新本书目录");
            var downloader = new HttpContentDownloader();
            DownloaderList.Add(downloader);
            downloader.Download(Book.IndexPage,
                ar =>
                {
                    //At this step, we can get the index page in the search engine 
                    var state = (RequestState)ar.AsyncState;
                    state.stopTimer = true;
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
                        _storage.SaveChapters(newChapters);
                        Book.Chapters = totalChapters.ToArray();
                        LastPage();
                    }
                    else
                    {
                        CrossThreadHelper.CrossThreadMethodCall(() => MessageBox.Show("本书尚未有新的章节"));
                    }

                    ProgressIndicatorHelper.StopProgressIndicator();
                });
        }

        public void CancelRunningConnections()
        {
            foreach (var downloader in DownloaderList)
            {
                downloader.CancelConnection();
            }
        }

        public class DownloadTask
        {
            public Chapter TaskChapter;
            public WaitHandle ResetEvent;
            public int BatchCount;
        }
    }
}
