using SmartReader.Library.Helper;

namespace SmartReader.Library.DataContract
{
    public class SearchEngine
    {
        public  SearchEngineType  Type { set; get; }
        public string SearchUri { set; get; }
        public HttpMethod Method { set; get; }
    }
}
