<<<<<<< HEAD
﻿using System;
using System.Windows;
using System.Windows.Controls;
using SmartReader.Library.DataContract;
using SmartReader.Library.Storage;
using SmartReader.ViewModel;

namespace SmartReader.Views
{
    public partial class BookIndexPage
    {
        private readonly PhoneStorage _storage;

        private BookIndexViewModel _model;

        public BookIndexViewModel Model
        {
            set { _model = value;
                DataContext = _model;
            }
            get { return _model; }
        }

        public BookIndexPage()
        {
            InitializeComponent();

            var temp = ModelManager.GetBookIndexModel();
            _storage = PhoneStorage.GetPhoneStorageInstance();
            
            temp.Book.Chapters = _storage.GetChaptersByBook(temp.Book);
            Model = temp;
        }

        private void ShowChapter(object sender, RoutedEventArgs e)
        {
            var x = ModelManager.GetChapterViewModel();
            x.CurrentBook = Model.Book;
            x.CurrentChapter = ((Button) sender).DataContext as Chapter;
            //Navigate to new page 
            NavigationService.Navigate(new Uri("/Views/ChapterViewPage.xaml", UriKind.Relative));
        }

        private void NextPage(object sender, EventArgs e)
        {
            Model.NextPageChapters();
        }

        private void PreviousPage(object sender, EventArgs e)
        {
            Model.PreviousPageChapters();
        }

        private void FirstPage(object sender, EventArgs e)
        {
            Model.FirstPage();
        }

        private void LastPage(object sender, EventArgs e)
        {
            Model.LastPage();
        }

        private void Refresh(object sender, EventArgs e)
        {
            Model.Refresh();
        }

        private void BackToBookListPage(object sender, EventArgs e)
        {
            PageManager.Navigate(PageManager.BookListPage);
        }
    }
=======
﻿using System;
using System.Windows;
using System.Windows.Controls;
using SmartReader.Library.DataContract;
using SmartReader.Library.Storage;
using SmartReader.ViewModel;

namespace SmartReader.Views
{
    public partial class BookIndexPage
    {
        private readonly PhoneStorage _storage;

        private BookIndexViewModel _model;

        public BookIndexViewModel Model
        {
            set { _model = value;
                DataContext = _model;
            }
            get { return _model; }
        }

        public BookIndexPage()
        {
            InitializeComponent();

            var temp = ModelManager.GetBookIndexModel();
            _storage = PhoneStorage.GetPhoneStorageInstance();
            
            temp.Book.Chapters = _storage.GetChaptersByBook(temp.Book);
            Model = temp;
        }

        private void ShowChapter(object sender, RoutedEventArgs e)
        {
            var x = ModelManager.GetChapterViewModel();
            x.CurrentBook = Model.Book;
            x.CurrentChapter = ((Button) sender).DataContext as Chapter;
            //Navigate to new page 
            NavigationService.Navigate(new Uri("/Views/ChapterViewPage.xaml", UriKind.Relative));
        }

        private void NextPage(object sender, EventArgs e)
        {
            Model.NextPageChapters();
        }

        private void PreviousPage(object sender, EventArgs e)
        {
            Model.PreviousPageChapters();
        }

        private void FirstPage(object sender, EventArgs e)
        {
            Model.FirstPage();
        }

        private void LastPage(object sender, EventArgs e)
        {
            Model.LastPage();
        }

        private void Refresh(object sender, EventArgs e)
        {
            Model.Refresh();
        }

        private void BackToBookListPage(object sender, EventArgs e)
        {
            PageManager.Navigate(PageManager.BookListPage);
        }
    }
>>>>>>> upstream/master
}