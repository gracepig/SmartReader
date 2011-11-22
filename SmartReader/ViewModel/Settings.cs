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

        public static int DefaultDownloadItemCount { set; get; }

        public static SearchEngineType DefaultSearchEngineType{ set; get;}

        public const string ChapterItemCountInSinglePageKey = "ChapterItemCountInSinglePage";
        public const string DefaultDownloadItemCountKey = "DefaultDownloadItemCount";
        public const string DefaultSearchEngineKey = "DefaultSearchEngine";


        public static void Load()
        {
            ChapterItemCountInSinglePage = LoadSettingByKey<int>(ChapterItemCountInSinglePageKey);
            DefaultDownloadItemCount = LoadSettingByKey<int>(DefaultDownloadItemCountKey);
            DefaultSearchEngineType = LoadSettingByKey<SearchEngineType>(DefaultSearchEngineKey);
        }

        public static void Save()
        {
            SaveSettingByKey("DefaultDownloadItemCount", DefaultDownloadItemCount);
            SaveSettingByKey("DefaultSearchEngine", DefaultSearchEngineType);
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
