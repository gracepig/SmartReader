namespace SmartReader.Library.Helper
{
    public class Constants
    {
        public const int ChapterShowInOnePage = 12;

        public static readonly int[]  DownloadItemCountList = new int[] {0, 5, 10, 25, 50 };
    }

    public enum SearchEngineType
    {
        Sodu = 0,
        Xiaoelang 
    }

    public enum HttpMethod
    { 
        Get = 0,
        Post 
    }
}
