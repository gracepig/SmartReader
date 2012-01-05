using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Threading;
using SmartReader.Library.Interface;

namespace SmartReader.Library.Network
{
    public class HttpContentDownloader : IDownloader 
    {
        private readonly Dictionary< Object, IParser> uriParserPairs = new Dictionary<Object, IParser>();
        private RequestState state;
        private Timer timer;

        public void Download(Uri targetUri, AsyncCallback callback)
        {
            var request = (HttpWebRequest)WebRequest.Create(targetUri);
            state = new RequestState { Request = request, stopTimer = false };

            request.BeginGetResponse(callback, state);

            timer = new Timer(x =>  {
                                        if (!state.stopTimer)
                                        {
                                            ((RequestState)x).Request.Abort();
                                        }
                                        timer.Dispose(); }, 
                                    state, 
                                    TimeSpan.FromSeconds(Settings.DefaultTimeOutSeconds), 
                                    TimeSpan.FromTicks(0));
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

        public void CancelConnection()
        {
            if (!state.stopTimer)
            {
                state.Request.Abort();
            }
        }

        #region only used by test code 
        public void Download(Uri uri, object metaData, IParser parser)
        {
            uriParserPairs.Add(metaData, parser);
            var request = (HttpWebRequest)WebRequest.Create(uri);
            var state = new RequestState { Request = request, Metadata = metaData };
            request.BeginGetResponse(GetData, state);
        }

        private void GetData(IAsyncResult ar)
        {
            var state = (RequestState)ar.AsyncState;
            var response = (HttpWebResponse)state.Request.EndGetResponse(ar);
            response.GetResponseStream();
            var parser = uriParserPairs[state.Metadata];
            parser.Parse(response.GetResponseStream(), state.Metadata);
        }
        #endregion 
    }

    public class RequestState
    {
        public HttpWebRequest Request { set; get; }
        public object Metadata { set; get; }

        public bool stopTimer = false;
    }
}
