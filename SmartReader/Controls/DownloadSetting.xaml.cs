using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ImageTools;
using SmartReader.ViewModel;

namespace SmartReader.Controls
{
    public partial class DownloadSetting : UserControl
    {

        public DownloadSetting()
        {
            InitializeComponent();

            this.DownloadChapterCountSelect.ItemsSource = new[] {5, 10, 50};
        }

        private void StartDownload(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
