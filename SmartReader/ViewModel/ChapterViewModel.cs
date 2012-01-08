using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Media.Imaging;
using SmartReader.Helper;
using SmartReader.Library.DataContract;
using SmartReader.Library.Helper;
using SmartReader.Library.Network;
using SmartReader.Library.Parser.BookSite;
using SmartReader.Library.Storage;

namespace SmartReader.ViewModel
{
    public class ChapterViewModel : ViewModelBase
    {
        public Book CurrentBook { set; get; }

        private int CurrentChapterIndex { set; get; }

        public string ChapterInfo
        {
            get { return String.Format("{0}/{1} {2} {3}", CurrentChapterIndex + 1, 
                                                      CurrentBook.Chapters.Length, 
                                                      CurrentBook.Name,
                                                      CurrentChapter.ChapterName); 
                }
        }

        private Uri _currentImage;
        public Uri CurrentImage
        {
            set 
            { 
                _currentImage = value;
                RaiseProperyChanged("CurrentImage");
            }
            get { return _currentImage; }
        }

        private bool _showLoadMoreBtn = true;
        public bool ShowLoadMoreBtn
        {
            get
            {
                return _showLoadMoreBtn;
            }
            set
            {
                _showLoadMoreBtn = value;
                RaiseProperyChanged("ShowLoadMoreBtn");
            }
        }

        private Chapter _currentChapter;
        public Chapter CurrentChapter
        {
            set 
            {
                _currentChapter = value;

                var l = new List<Chapter>(CurrentBook.Chapters);
                CurrentChapterIndex = l.IndexOf(_currentChapter);

                CurrentBook.LastReadChapterId = _currentChapter.Id;
                UpdateLastReadChapter();

                if (_currentChapter.Downloaded)
                {
                    RaiseProperyChanged("CurrentChapter");
                }
                else
                {
                    SaveCurrentBook(CurrentBook);

                    CurrentChapter.SaveContent1 = String.Empty;
                    //Use tempChapter to deal with the cross thread access problem
                    var tempChapter = new Chapter();
                    tempChapter.ChapterUri = CurrentChapter.ChapterUri;
                    tempChapter.ChapterName = CurrentChapter.ChapterName;
                    tempChapter.Book = CurrentChapter.Book;
                    tempChapter.Id = CurrentChapter.Id;

                    DownloadSingleChapter(tempChapter, 
                         () => CrossThreadHelper.CrossThreadMethodCall(() => {
                             _currentChapter.SaveContent1 = tempChapter.SaveContent1;
                             _currentChapter.SaveContent2 = tempChapter.SaveContent2;
                             _currentChapter.Downloaded = tempChapter.Downloaded;
                             _currentChapter.IsImageContent = tempChapter.IsImageContent;
                             PhoneStorage.GetPhoneStorageInstance().UpdateDB();
                             RaiseProperyChanged("CurrentChapter");
                             ProgressIndicatorHelper.StopProgressIndicator();
                         }));
                }

                RaiseProperyChanged("ChapterInfo");
            }

            get
            {
                if (_currentChapter == null )
                {
                    CurrentChapterIndex = 0;
                    _currentChapter = CurrentBook.Chapters[CurrentChapterIndex];
                    CurrentBook.LastReadChapterId = CurrentChapter.Id;
                }
                
                if (CurrentImage == null && _currentChapter.ImageUris != null)
                {
                    CurrentImage = _currentChapter.ImageUris[0];
                }

                return _currentChapter;
            }
        }

        public ChapterViewModel() { }
        
        public ChapterViewModel(Book book)
        {
            CurrentBook = book;
        }

        public void SaveCurrentBook(Book book)
        {
            var storage = PhoneStorage.GetPhoneStorageInstance();

            if (!storage.IsWebSiteExist(book.WebSite))
            {
                storage.SaveWebSite(book.WebSite);
            }

            if (!storage.IsBookExist(book))
            {
                storage.SaveBook(book);
                storage.SaveChapters(book.Chapters);
            }
        }

        public void UpdateLastReadChapter()
        {
            PhoneStorage.GetPhoneStorageInstance().UpdateDB();
        }

        public void NextChapter()
        {
            if (CurrentChapterIndex + 1 < CurrentBook.Chapters.Length)
            {
                CurrentChapterIndex++;
                CurrentChapter = CurrentBook.Chapters[CurrentChapterIndex];
            }
        }

        public void PreviousChapter()
        {
            if (CurrentChapterIndex - 1 > 0)
            {
                CurrentChapterIndex--;
                CurrentChapter = CurrentBook.Chapters[CurrentChapterIndex];
            }
        }

        public BitmapImage[] GetImageSource()
        {
           var images =  PhoneStorage.GetPhoneStorageInstance().GetArticleImageByChapter(CurrentChapter);
           if (images != null && images.Count() > 0)
           {
               return images.Select(image => ConvertToBitmapImage(image.ImageBytes)).ToArray();
           }
            return null;
        }

        public static BitmapImage ConvertToBitmapImage(byte[] bytes)
        {
            var stream = new MemoryStream(bytes);
            var image = new BitmapImage();
            image.SetSource(stream);
            return image;
        }

        /// <summary>
        /// Download single chapter, should be active from the ChapterView Page
        /// </summary>
        /// <param name="chapter"></param>
        /// <param name="callback"></param>
        public void DownloadSingleChapter(Chapter chapter, Action callback)
        {
            var downloader = new HttpContentDownloader();
            downloader.Download(chapter.ChapterUri, ar =>
            {
                try
                {
                    //At this step, we can get the index page in the search engine 
                    var state = (RequestState)ar.AsyncState;
                    state.stopTimer = true;
                    var response = (HttpWebResponse)state.Request.EndGetResponse(ar);
                    response.GetResponseStream();

                    var parser = new WebSiteBookContentPageParser();
                    parser.ParsingCompleted += ChapterContentParsingCompleted;

                    chapter.Downloaded = true;
                    parser.Parse(response.GetResponseStream(), chapter);

                    if (callback != null)
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

        private void ChapterContentParsingCompleted(object sender, EventArgs e)
        {
            RaiseProperyChanged("CurrentChapter");
        }

        public void LoadNextImage()
        {
            var index = CurrentChapter.ImageUris.ToList().IndexOf(CurrentImage);
            index++;
            if (index < CurrentChapter.ImageUris.Length)
            {
                CurrentImage = CurrentChapter.ImageUris[index];
            }
            
            if (index >= CurrentChapter.ImageUris.Length  - 1)
            {
                ShowLoadMoreBtn = false;
            }
        }
    }
}
