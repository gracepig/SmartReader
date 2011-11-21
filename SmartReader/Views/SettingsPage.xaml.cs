using System;
using SmartReader.Library.Helper;
using SmartReader.ViewModel;

namespace SmartReader.Views
{
    public partial class SettingsPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            Settings.Load();
            DownloadChapterCountSelect.ItemsSource = Constants.DownloadItemCountList;
            DownloadChapterCountSelect.SelectedItem = Settings.DefaultDownloadItemCount;

            SearchEngineSelection.ItemsSource = new[] { SearchEngineType.Sodu, SearchEngineType.Xiaoelang};
            SearchEngineSelection.SelectedItem = Settings.DefaultSearchEngineType;
        }

        private void Save(object sender, EventArgs e)
        {
            Settings.SaveSettingByKey(Settings.DefaultDownloadItemCountKey, DownloadChapterCountSelect.SelectedItem);
            Settings.SaveSettingByKey(Settings.DefaultSearchEngineKey, SearchEngineSelection.SelectedItem);
            NavigationService.GoBack();
        }

        private void Cancel(object sender, EventArgs e)
        {
           NavigationService.GoBack();
        }
    }
}