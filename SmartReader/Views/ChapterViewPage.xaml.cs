using System;
using Microsoft.Phone.Controls;
using SmartReader.ViewModel;
using GestureEventArgs = Microsoft.Phone.Controls.GestureEventArgs;

namespace SmartReader.Views
{
    public partial class ChapterViewPage
    {
        private ChapterViewModel _model;
        public ChapterViewModel Model
        {
            set { _model = value;
                DataContext = _model;
            }
            get { return _model; }
        }

        public ChapterViewPage()
        {
            InitializeComponent();
            Model = ModelManager.GetChapterViewModel();
        }

        private void PageOrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            if (e.Orientation == PageOrientation.LandscapeLeft || e.Orientation == PageOrientation.LandscapeRight)
            {
                var temp = this.ChapterTextContent.Height;
                this.ChapterTextContent.Height = this.ChapterTextContent.Width;
                this.ChapterTextContent.Width = temp;
            }
        }

        private void ShowApplicationBar(object sender, GestureEventArgs e)
        {
            ApplicationBar.IsVisible = !ApplicationBar.IsVisible;
        }

        private void SwipeEventCompleted(object sender, DragCompletedGestureEventArgs e)
        {
            if (e.Direction == System.Windows.Controls.Orientation.Horizontal)
            {
                if (Math.Abs(e.HorizontalChange) > 200)
                {

                    if (e.HorizontalChange > 0)
                    {
                        ShowNextChapter();
                    }
                    else
                    {
                        ShowPreviousChapter();
                    }
                }
            }

        }

        private void ShowNextChapter()
        {
            Model.NextChapter();
        }

        private void ShowPreviousChapter()
        {
            Model.PreviousChapter();
        }
    }
}