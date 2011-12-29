using System.IO.IsolatedStorage;
using SmartReader.Library.Helper;

namespace SmartReader.ViewModel
{
    public class Settings : ViewModelBase
    {
        
#if DEBUG 
    public static bool IsDebugMode=true;
#else
        public static bool IsDebugMode=false;
#endif


public static int ChapterItemCountInSinglePage { set; get; }

        private static int _defaultDownloadItemCount = 10;
        public static int DefaultDownloadItemCount
        {
            set { _defaultDownloadItemCount = value; } 
            get { return _defaultDownloadItemCount; }
        }

        public static SearchEngineType DefaultSearchEngineType{ set; get;}

        private static int _defaultTimeOutSeconds = 30;
        public static int DefaultTimeOutSeconds 
        { 
            set { _defaultTimeOutSeconds = value; }
            get { return _defaultTimeOutSeconds; }
        
        }

        public const string ChapterItemCountInSinglePageKey = "ChapterItemCountInSinglePage";
        public const string DefaultDownloadItemCountKey = "DefaultDownloadItemCount";
        public const string DefaultSearchEngineKey = "DefaultSearchEngine";
        public const string DefaultNetworkTimeOutKey = "DefaultTimeOut";

        public static void Load()
        {
            ChapterItemCountInSinglePage = LoadSettingByKey<int>(ChapterItemCountInSinglePageKey);
            DefaultDownloadItemCount = LoadSettingByKey<int>(DefaultDownloadItemCountKey);
            DefaultSearchEngineType = LoadSettingByKey<SearchEngineType>(DefaultSearchEngineKey);
            DefaultTimeOutSeconds = LoadSettingByKey<int>(DefaultNetworkTimeOutKey);
        }

        public static void Save()
        {
            SaveSettingByKey(DefaultDownloadItemCountKey, DefaultDownloadItemCount);
            SaveSettingByKey(DefaultSearchEngineKey, DefaultSearchEngineType);
            SaveSettingByKey(DefaultNetworkTimeOutKey, DefaultTimeOutSeconds);
        }

        public static T LoadSettingByKey<T> (string key)
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
            {
                var value = (T)IsolatedStorageSettings.ApplicationSettings[key];
                return value;
            }
            return default(T);
        }

        public static void SaveSettingByKey(string key, object value)
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
            {
                IsolatedStorageSettings.ApplicationSettings[key] = value;
            }
            else
            {
                IsolatedStorageSettings.ApplicationSettings.Add(key,value);
            }

            IsolatedStorageSettings.ApplicationSettings.Save();
        }
    }
}
