using System;

namespace SmartReader.Library.DataContract
{
    public class SearchResult
    {
        public string KeyWord { set; get; }
        public string BookName { set; get; }
        public string LastUpdateChapterName { set; get; }
        public string Authoring { set; get; }
        public Uri IndexPageUri { set; get; }
        public Uri LastUpdateChapterLink { set; get; }
        public DateTime LastUpdateDate { set; get; }

        public Book Book { set; get; }
        public WebSite WebSite { set; get; }
    }
}
