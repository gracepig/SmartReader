using System;
using System.Windows;
using Microsoft.Phone.Controls;
using SmartReader.Helper;
using SmartReader.ViewModel;
using GestureEventArgs = Microsoft.Phone.Controls.GestureEventArgs;

namespace SmartReader.Views
{
    public partial class ChapterViewPage
    {
        private ChapterViewModel _model;
        public ChapterViewModel Model
        {
            set 
            {
                _model = value;
                DataContext = _model;
                LoadMoreBtn.DataContext = value;
            }
            get { return _model; }
        }

        public ChapterViewPage()
        {
            InitializeComponent();
            Model = ModelManager.GetChapterViewModel();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ProgressIndicatorHelper.StartProgressIndicator(true, "连接中");
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            ProgressIndicatorHelper.StopProgressIndicator();
        }

        private void PageOrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            if (e.Orientation == PageOrientation.LandscapeLeft || e.Orientation == PageOrientation.LandscapeRight)
            {
                var temp = ChapterTextContent.Height;
                ChapterTextContent.Height = ChapterTextContent.Width;
                ChapterTextContent.Width = temp;
            }
        }

        private void ShowApplicationBar(object sender, GestureEventArgs e)
        {
            //ApplicationBar.IsVisible = !ApplicationBar.IsVisible;
        }

        private void SwipeEventCompleted(object sender, DragCompletedGestureEventArgs e)
        {
            if (e.Direction == System.Windows.Controls.Orientation.Horizontal)
            {
                if (Math.Abs(e.HorizontalChange) > 50)
                {

                    if (e.HorizontalChange > 0)
                    {
                        ShowPreviousChapter();
                    }
                    else
                    {
                        ShowNextChapter();
                    }
                }
            }
        }

        private void ShowNextChapter()
        {
            ProgressIndicatorHelper.StartProgressIndicator(true, "读取下一章节内容");
            Model.NextChapter();
        }

        private void ShowPreviousChapter()
        {
            ProgressIndicatorHelper.StartProgressIndicator(true, "读取上一章节内容");
            Model.PreviousChapter();
        }

        private void NextChapter(object sender, EventArgs e)
        {
            Model.NextChapter();
            ChapterTextContent.BackToTop();
        }

        private void PreviousChapter(object sender, EventArgs e)
        {
            Model.PreviousChapter();
            ChapterTextContent.BackToTop();
        }

        private new void LayoutUpdated(object sender, EventArgs e)
        {
            ProgressIndicatorHelper.StopProgressIndicator();
        }

        private void BackToBookIndex(object sender, EventArgs e)
        {
            PageManager.Navigate(PageManager.BookIndexPage);
        }

        private void BackToBookListPage(object sender, EventArgs e)
        {
            PageManager.Navigate(PageManager.BookListPage);
        }

        private void LoadNextImage(object sender, RoutedEventArgs e)
        {
            Model.LoadNextImage();
            ImageContainer.ScrollToVerticalOffset(0);
        }
    }
}