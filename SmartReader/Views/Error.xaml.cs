using System;
using System.Net;
using SmartReader.Library;

namespace SmartReader.Views
{
    public partial class Error
    {

        public static Exception Exception;

        public Error()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (Settings.IsDebugMode)
            {
                if (Exception is WebException)
                {
                    UserFriendlyMessage.Text = Exception.Message;
                }

                ErrorText.Text = Exception.ToString();    
            }
            else
            {
                if (Exception is WebException)
                {
                   
                    UserFriendlyMessage.Text = "当前页面无法访问，请尝试其他网站。";
                }

                if (Exception is TimeoutException )
                {
                    UserFriendlyMessage.Text = Exception.Message;
                }
            }
        }
    }
}