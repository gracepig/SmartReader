using SmartReader.Library.Helper;

namespace SmartReader.Controls
{
    public partial class DownloadSetting
    {

        public DownloadSetting()
        {
            InitializeComponent();

            this.DownloadChapterCountSelect.ItemsSource = Constants.DownloadItemCountList;
        }
    }
}
