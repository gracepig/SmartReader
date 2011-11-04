using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Windows.Threading;
using SmartReader.Library.DataContract;
using SmartReader.Library.Helper;
using SmartReader.Library.Network;
using SmartReader.Library.Parser.BookSite;
using SmartReader.Library.Parser.Sodu;

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

            var searchEngine = GetSearchEngines()[0];

            var searchUri = new Uri(String.Format(searchEngine.SearchUri, HttpUtility.UrlEncode(keyword)), UriKind.Absolute);

            //var searchResults = new List<SearchResult>();

            HttpContentDownloader downloader = new HttpContentDownloader();
            var searchResult = new SearchResult();
            //downloader.Download(searchUri, searchResult, GetData);
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

                    //if (searchEngine.Name == "Sodu")
                    //{
                    //    if (searchResults.Count > 0)
                    //    {
                    //        GetBookSiteLink(searchResults[0].IndexPageUri, searchResults[0]);
                    //    }
                    //}

                    //Application.Current.RootVisual.Dispatcher.BeginInvoke(() => {Downloading = false;});
                    Downloading = false;
                });
        }

        private List<Book> _bookList;
        public List<Book> BookList
        {
            set { _bookList = value; RaiseProperyChanged("BookList"); }
            get { return _bookList; }
        }

        public void GetBookSiteLink(SearchResult searchResult)
        {
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
                    var parser = new SoduBookLastestUpdatePageParser();
                    BookList = parser.Parse(response.GetResponseStream(), searchResult.Book) as List<Book>;
                });
        }

        public event EventHandler GetBookIndexPageCompleted;

        public void GetBookSiteBookIndexPageLink(Book book)
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
                    var parser = new WebSiteBookContentPageParser();
                    parser.Parse(response.GetResponseStream(), book);

                    ParseWebSiteBookIndexPage(book);

                    if (GetBookIndexPageCompleted != null )
                    {
                        GetBookIndexPageCompleted(this, null);
                    }
                });
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
                    parser.Parse(response.GetResponseStream(), book);

                    SelectedBook = book;

                    ModelManager.GetBookIndexModel().Book = SelectedBook;
                });
        }

        private SearchEngine[] GetSearchEngines()
        {
            var ret = new SearchEngine();
            ret.Name = "Sodu";
            //var uri = new Uri(String.Format("http://search.sodu.org/searchname.aspx?wd={0}", HttpUtility.UrlEncode(keyword)), UriKind.Absolute);
            ret.SearchUri = "http://search.sodu.org/searchname.aspx?wd={0}";
            return new SearchEngine[] { ret };
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
