using System;
using System.Collections.Generic;
using SmartReader.Helper;
using SmartReader.Library.DataContract;

namespace SmartReader.ViewModel
{
    public class ChapterViewModel : ViewModelBase
    {
        public Book CurrentBook { set; get; }

        private int CurrentChapterIndex { set; get; }

        private Chapter _currentChapter;
        public Chapter CurrentChapter
        {
            set 
            {
                _currentChapter = value;
                var l = new List<Chapter>(CurrentBook.Chapters);
                CurrentChapterIndex = l.IndexOf(_currentChapter);

                if (_currentChapter.Downloaded)
                {
                    RaiseProperyChanged("CurrentChapter");
                }
                else
                {
                    CurrentChapter.Content = String.Empty;

                    var tempChapter = new Chapter();
                    tempChapter.ChapterUri = CurrentChapter.ChapterUri;
                    tempChapter.ChapterName = CurrentChapter.ChapterName;
                    ModelManager.GetBookIndexModel().DownloadSingleChapter(tempChapter, 
                         () =>
                         {
                             CrossThreadHelper.CrossThreadMethodCall(() => { 
                                 _currentChapter.Content = tempChapter.Content;
                                 _currentChapter.Downloaded = tempChapter.Downloaded;
                                 RaiseProperyChanged("CurrentChapter");
                          });
                         });
                }
            }

            get
            {
                if (_currentChapter == null )
                {
                    CurrentChapterIndex = 0;
                    _currentChapter = CurrentBook.Chapters[CurrentChapterIndex];
                }
                return _currentChapter;
            }
        }

        public ChapterViewModel()
        {
        }

        public ChapterViewModel(Book book)
        {
            CurrentBook = book;
        }

        public void NextChapter()
        {
            if (CurrentChapterIndex + 1 < CurrentBook.Chapters.Length)
            {
                CurrentChapterIndex++;

                if (CurrentBook.Chapters[CurrentChapterIndex].Downloaded)
                {
                    CurrentChapter = CurrentBook.Chapters[CurrentChapterIndex];
                }
                else
                {
                    var tempChapter = new Chapter();
                    tempChapter.ChapterName = CurrentBook.Chapters[CurrentChapterIndex].ChapterName;
                    tempChapter.ChapterUri= CurrentBook.Chapters[CurrentChapterIndex].ChapterUri;

                    ModelManager.GetBookIndexModel().
                        DownloadSingleChapter(tempChapter, () =>
                        {
                            CrossThreadHelper.CrossThreadMethodCall(() =>
                            {
                                _currentChapter.Content = tempChapter.Content;
                                _currentChapter.Downloaded = tempChapter.Downloaded;
                                RaiseProperyChanged("CurrentChapter");
                            });
                        });
                }
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
    }
}
