using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using SmartReader.Library.DataContract;
using SmartReader.Library.Interface;

namespace SmartReader.Library.Network
{
    public class HttpContentDownloader : IDownloader 
    {
        private Dictionary< Object, IParser> UriParserPairs = new Dictionary<Object, IParser>();

        public void Download (Uri uri , SearchResult  keyWord)
        {
            //var client = new WebClient();
            //client.OpenReadCompleted += get;
            //client.OpenReadAsync(uri,keyWord);
        }

        public void Download(Uri uri, object metaData ,IParser parser )
        {
            UriParserPairs.Add(metaData, parser);
            var request = (HttpWebRequest)WebRequest.Create(uri);
            var state = new RequestState {  Request = request, Metadata = metaData};
            request.BeginGetResponse(GetData, state);
        }

        private void GetData(IAsyncResult ar)
        {
            var state = (RequestState) ar.AsyncState;
            var response = (HttpWebResponse)state.Request.EndGetResponse(ar);
            var url1 = response.ResponseUri.ToString();
            var metaData = state.Metadata as Chapter;
            //if (metaData != null )
                //metaData.PageRootUri = GetRootUrl(url1);
            response.GetResponseStream();
            var parser = UriParserPairs[state.Metadata];
            parser.Parse(response.GetResponseStream(), state.Metadata);
        }
        
        private string GetRootUrl(string urlString)
        {
            //http://www.opny.net/Html/Book/23/18992/4274362.html
            if (urlString.EndsWith("/")) return urlString;
            else
            {
                return (urlString.Substring(0, urlString.LastIndexOf('/') + 1));
            }
        }

        public void Download(Uri targetUri, AsyncCallback callback)
        {
            //UriParserPairs.Add(metaData, parser);
            var request = (HttpWebRequest)WebRequest.Create(targetUri);
            var state = new RequestState { Request = request };
            request.BeginGetResponse(callback,state );
        }


        public void DownloadPost(Uri targetUri, string key,  string data, AsyncCallback callback)
        {
            var request = (HttpWebRequest)WebRequest.Create(targetUri);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            var state = new RequestState { Request = request };

            request.BeginGetRequestStream( ar =>
                                               {
                                                   var requestStream = request.EndGetRequestStream(ar);
                                                   using (var sw = new StreamWriter(requestStream))
                                                   {
                                                       sw.Write(String.Format("_method=POST&"));
                                                       sw.Write(String.Format("{0}={1}", key, data));
                                                   }

                                                   request.BeginGetResponse(callback, state);

                                               }, null);
        }

    }

    public class RequestState
    {
        public HttpWebRequest Request { set; get; }
        public object Metadata { set; get; }
    }
}
