<<<<<<< HEAD
﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Windows.Threading;
using SmartReader.Library.DataContract;
using SmartReader.Library.Helper;
using SmartReader.Library.Network;
using SmartReader.Library.Parser.BookSite;
using SmartReader.Library.Parser.Sodu;
using SmartReader.Library.Parser.Xiaoelang;
using SmartReader.Library.Storage;
using SearchEngine = SmartReader.Library.DataContract.SearchEngine;

namespace SmartReader.ViewModel
{
    public class SearchViewModel : INotifyPropertyChanged 
    {
        private bool _downloading;
        public bool Downloading
        {
            set 
            { 
                _downloading = value;
                RaiseProperyChanged("Downloading");
            }
            get
            {
                return _downloading;
            }
        }

        private List<SearchResult> _searchBookResult = null;
        public List<SearchResult> SearchBookResult
        {
            set 
            {
                _searchBookResult = value;
                RaiseProperyChanged("SearchBookResult");
            }

            get { return _searchBookResult; }
        }


        private Book _selectedBook;
        public Book SelectedBook
        {
            set
            {
                _selectedBook = value; 
                RaiseProperyChanged("SelectedBook");
            }
            get { return _selectedBook; }
        }


        public void Search(string keyword)
        {
            Downloading = true;

            var searchEngine = GetSearchEngine();

            Uri searchUri = new Uri(searchEngine.SearchUri, UriKind.Absolute);
            if (searchEngine.Type == SearchEngineType.Sodu)
            {
                searchUri = new Uri(String.Format(searchEngine.SearchUri, HttpUtility.UrlEncode(keyword)), UriKind.Absolute);
            }
            
            //var searchResults = new List<SearchResult>();

            HttpContentDownloader downloader = new HttpContentDownloader();
            var searchResult = new SearchResult();

            if (searchEngine.Type == SearchEngineType.Sodu)
            {
                downloader.Download(searchUri,
                ar =>
                {
                    //At this step, we can get the index page in the search engine 
                    var state = (RequestState)ar.AsyncState;
                    var response = (HttpWebResponse)state.Request.EndGetResponse(ar);
                    var metaData = state.Metadata as SearchResult;
                    if (metaData != null)
                    {

                    }
                    response.GetResponseStream();
                    var parser = new SoduSearchResultPageParser();
                    SearchBookResult = parser.Parse(response.GetResponseStream(), searchResult) as List<SearchResult>;
                    Downloading = false;
                });
            }

            if (searchEngine.Type == SearchEngineType.Xiaoelang)
            {
                downloader.DownloadPost(searchUri, "keyword" , keyword, 
                ar =>
                {
                    //At this step, we can get the index page in the search engine 
                    var state = (RequestState)ar.AsyncState;
                    var response = (HttpWebResponse)state.Request.EndGetResponse(ar);
                    var metaData = state.Metadata as SearchResult;
                    if (metaData != null) { }
                    response.GetResponseStream();
                    var parser = new XiaoelangSearchResultPageParser();
                    SearchBookResult = parser.Parse(response.GetResponseStream(), searchResult) as List<SearchResult>;
                    
                    Downloading = false;
                });
            }
        }

        private List<Book> _bookList;
        public List<Book> BookList
        {
            set { _bookList = value; RaiseProperyChanged("BookList"); }
            get { return _bookList; }
        }

        public void GetBookSiteLink(SearchResult searchResult)
        {
            var searchEngine = GetSearchEngine();
            
            Uri soduBookLatestUpdatePage = searchResult.IndexPageUri;
            var downloader = new HttpContentDownloader();
            //downloader.Download(searchUri, searchResult, GetData);
            downloader.Download(soduBookLatestUpdatePage,
                ar =>
                {
                    //At this step, we can get the index page in the search engine 
                    var state = (RequestState)ar.AsyncState;
                    var response = (HttpWebResponse)state.Request.EndGetResponse(ar);
                    response.GetResponseStream();

                    if (searchEngine.Type == SearchEngineType.Sodu)
                    {
                        var parser = new SoduBookLastestUpdatePageParser();
                        BookList = parser.Parse(response.GetResponseStream(), searchResult.Book) as List<Book>;    
                    }
                    
                    if (searchEngine.Type == SearchEngineType.Xiaoelang)
                    {
                        var parser = new XiaoelangBookLastestUpdatePageParser();
                        BookList = parser.Parse(response.GetResponseStream(), searchResult.Book) as List<Book>;    
                    }

                });
        }

        public event EventHandler GetBookIndexPageCompleted;

        public void GetBookSiteBookIndexPageLink(Book book)
        {
            var downloader = new HttpContentDownloader();
            try
            {
                downloader.Download(book.IndexPage,
                    ar =>
                    {
                        //At this step, we can get the index page in the search engine 
                        var state = (RequestState)ar.AsyncState;
                        var response = (HttpWebResponse)state.Request.EndGetResponse(ar);
                        response.GetResponseStream();

                        book.RootUrl = UrlHelper.GetRootUrlString(response.ResponseUri);
                        var parser = new WebSiteBookContentPageParser();
                        parser.Parse(response.GetResponseStream(), book);

                        ParseWebSiteBookIndexPage(book);

                        if (GetBookIndexPageCompleted != null)
                        {
                            GetBookIndexPageCompleted(this, null);
                        }
                    });
            }
            catch (WebException we)
            {
                ExceptionHandler.HandleException(we);
            }
        }

        public void ParseWebSiteBookIndexPage(Book book)
        {
            var downloader = new HttpContentDownloader();

            downloader.Download(book.IndexPage,
                ar =>
                {
                    //At this step, we can get the index page in the search engine 
                    var state = (RequestState)ar.AsyncState;
                    var response = (HttpWebResponse)state.Request.EndGetResponse(ar);
                    response.GetResponseStream();

                    book.RootUrl = UrlHelper.GetRootUrlString(response.ResponseUri);
                    var parser = new WebsiteBookIndexPageParser();

                    try {
                        parser.Parse(response.GetResponseStream(), book);
                    }
                    catch (Exception e)
                    {
                        ExceptionHandler.HandleException(e);
                    }

                    SelectedBook = book;

                    ModelManager.GetBookIndexModel().Book = SelectedBook;
                });
        }

        private SearchEngine GetSearchEngine()
        {
            if (Settings.DefaultSearchEngineType == SearchEngineType.Sodu)
            {
                var ret = new SearchEngine();
                ret.Type = SearchEngineType.Sodu;
                //var uri = new Uri(String.Format("http://search.sodu.org/searchname.aspx?wd={0}", HttpUtility.UrlEncode(keyword)), UriKind.Absolute);
                ret.SearchUri = "http://search.sodu.org/searchname.aspx?wd={0}";
                ret.Method = HttpMethod.Get;
                return ret ;
            }

            if (Settings.DefaultSearchEngineType == SearchEngineType.Xiaoelang)
            {
                var ret = new SearchEngine();
                ret.Type = SearchEngineType.Xiaoelang;
                //var uri = new Uri(String.Format("http://www.xiaoelang.com/books/search", HttpUtility.UrlEncode(keyword)), UriKind.Absolute);
                ret.SearchUri = "http://www.xiaoelang.com/books/search";
                ret.Method = HttpMethod.Post;
                return  ret ;
            }

            return null;
            
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


        public Book CheckBookExists(Book book)
        {
            var storage = PhoneStorage.GetPhoneStorageInstance();

            foreach (var b in storage.GetAllBooks())
            {
                if (b.Name == book.Name && b.WebSite.WebSiteName == book.WebSite.WebSiteName)
                {
                    return b;
                }
            }

            return book;
        }
    }
}
=======
﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Windows.Threading;
using SmartReader.Library.DataContract;
using SmartReader.Library.Helper;
using SmartReader.Library.Network;
using SmartReader.Library.Parser.BookSite;
using SmartReader.Library.Parser.Sodu;
using SmartReader.Library.Parser.Xiaoelang;
using SmartReader.Library.Storage;
using SearchEngine = SmartReader.Library.DataContract.SearchEngine;

namespace SmartReader.ViewModel
{
    public class SearchViewModel : INotifyPropertyChanged 
    {
        private bool _downloading;
        public bool Downloading
        {
            set 
            { 
                _downloading = value;
                RaiseProperyChanged("Downloading");
            }
            get
            {
                return _downloading;
            }
        }

        private List<SearchResult> _searchBookResult = null;
        public List<SearchResult> SearchBookResult
        {
            set 
            {
                _searchBookResult = value;
                RaiseProperyChanged("SearchBookResult");
            }

            get { return _searchBookResult; }
        }


        private Book _selectedBook;
        public Book SelectedBook
        {
            set
            {
                _selectedBook = value; 
                RaiseProperyChanged("SelectedBook");
            }
            get { return _selectedBook; }
        }


        public void Search(string keyword)
        {
            Downloading = true;

            var searchEngine = GetSearchEngine();

            Uri searchUri = new Uri(searchEngine.SearchUri, UriKind.Absolute);
            if (searchEngine.Type == SearchEngineType.Sodu)
            {
                searchUri = new Uri(String.Format(searchEngine.SearchUri, HttpUtility.UrlEncode(keyword)), UriKind.Absolute);
            }
            
            //var searchResults = new List<SearchResult>();

            HttpContentDownloader downloader = new HttpContentDownloader();
            var searchResult = new SearchResult();

            if (searchEngine.Type == SearchEngineType.Sodu)
            {
                downloader.Download(searchUri,
                ar =>
                {
                    //At this step, we can get the index page in the search engine 
                    var state = (RequestState)ar.AsyncState;
                    var response = (HttpWebResponse)state.Request.EndGetResponse(ar);
                    var metaData = state.Metadata as SearchResult;
                    if (metaData != null)
                    {

                    }
                    response.GetResponseStream();
                    var parser = new SoduSearchResultPageParser();
                    SearchBookResult = parser.Parse(response.GetResponseStream(), searchResult) as List<SearchResult>;
                    Downloading = false;
                });
            }

            if (searchEngine.Type == SearchEngineType.Xiaoelang)
            {
                downloader.DownloadPost(searchUri, "keyword" , keyword, 
                ar =>
                {
                    //At this step, we can get the index page in the search engine 
                    var state = (RequestState)ar.AsyncState;
                    var response = (HttpWebResponse)state.Request.EndGetResponse(ar);
                    var metaData = state.Metadata as SearchResult;
                    if (metaData != null) { }
                    response.GetResponseStream();
                    var parser = new XiaoelangSearchResultPageParser();
                    SearchBookResult = parser.Parse(response.GetResponseStream(), searchResult) as List<SearchResult>;
                    
                    Downloading = false;
                });
            }
        }

        private List<Book> _bookList;
        public List<Book> BookList
        {
            set { _bookList = value; RaiseProperyChanged("BookList"); }
            get { return _bookList; }
        }

        public void GetBookSiteLink(SearchResult searchResult)
        {
            var searchEngine = GetSearchEngine();
            
            Uri soduBookLatestUpdatePage = searchResult.IndexPageUri;
            var downloader = new HttpContentDownloader();
            //downloader.Download(searchUri, searchResult, GetData);
            downloader.Download(soduBookLatestUpdatePage,
                ar =>
                {
                    //At this step, we can get the index page in the search engine 
                    var state = (RequestState)ar.AsyncState;
                    var response = (HttpWebResponse)state.Request.EndGetResponse(ar);
                    response.GetResponseStream();

                    if (searchEngine.Type == SearchEngineType.Sodu)
                    {
                        var parser = new SoduBookLastestUpdatePageParser();
                        BookList = parser.Parse(response.GetResponseStream(), searchResult.Book) as List<Book>;    
                    }
                    
                    if (searchEngine.Type == SearchEngineType.Xiaoelang)
                    {
                        var parser = new XiaoelangBookLastestUpdatePageParser();
                        BookList = parser.Parse(response.GetResponseStream(), searchResult.Book) as List<Book>;    
                    }

                });
        }

        public event EventHandler GetBookIndexPageCompleted;

        public void GetBookSiteBookIndexPageLink(Book book)
        {
            var downloader = new HttpContentDownloader();
            try
            {
                downloader.Download(book.IndexPage,
                    ar =>
                    {
                        //At this step, we can get the index page in the search engine 
                        var state = (RequestState)ar.AsyncState;
                        var response = (HttpWebResponse)state.Request.EndGetResponse(ar);
                        response.GetResponseStream();

                        book.RootUrl = UrlHelper.GetRootUrlString(response.ResponseUri);
                        var parser = new WebSiteBookContentPageParser();
                        parser.Parse(response.GetResponseStream(), book);

                        ParseWebSiteBookIndexPage(book);

                        if (GetBookIndexPageCompleted != null)
                        {
                            GetBookIndexPageCompleted(this, null);
                        }
                    });
            }
            catch (WebException we)
            {
                ExceptionHandler.HandleException(we);
            }
        }

        public void ParseWebSiteBookIndexPage(Book book)
        {
            var downloader = new HttpContentDownloader();

            downloader.Download(book.IndexPage,
                ar =>
                {
                    //At this step, we can get the index page in the search engine 
                    var state = (RequestState)ar.AsyncState;
                    var response = (HttpWebResponse)state.Request.EndGetResponse(ar);
                    response.GetResponseStream();

                    book.RootUrl = UrlHelper.GetRootUrlString(response.ResponseUri);
                    var parser = new WebsiteBookIndexPageParser();

                    try {
                        parser.Parse(response.GetResponseStream(), book);
                    }
                    catch (Exception e)
                    {
                        ExceptionHandler.HandleException(e);
                    }

                    SelectedBook = book;

                    ModelManager.GetBookIndexModel().Book = SelectedBook;
                });
        }

        private SearchEngine GetSearchEngine()
        {
            if (Settings.DefaultSearchEngineType == SearchEngineType.Sodu)
            {
                var ret = new SearchEngine();
                ret.Type = SearchEngineType.Sodu;
                //var uri = new Uri(String.Format("http://search.sodu.org/searchname.aspx?wd={0}", HttpUtility.UrlEncode(keyword)), UriKind.Absolute);
                ret.SearchUri = "http://search.sodu.org/searchname.aspx?wd={0}";
                ret.Method = HttpMethod.Get;
                return ret ;
            }

            if (Settings.DefaultSearchEngineType == SearchEngineType.Xiaoelang)
            {
                var ret = new SearchEngine();
                ret.Type = SearchEngineType.Xiaoelang;
                //var uri = new Uri(String.Format("http://www.xiaoelang.com/books/search", HttpUtility.UrlEncode(keyword)), UriKind.Absolute);
                ret.SearchUri = "http://www.xiaoelang.com/books/search";
                ret.Method = HttpMethod.Post;
                return  ret ;
            }

            return null;
            
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


        public Book CheckBookExists(Book book)
        {
            var storage = PhoneStorage.GetPhoneStorageInstance();

            foreach (var b in storage.GetAllBooks())
            {
                if (b.Name == book.Name && b.WebSite.WebSiteName == book.WebSite.WebSiteName)
                {
                    return b;
                }
            }

            return book;
        }
    }
}
>>>>>>> upstream/master
