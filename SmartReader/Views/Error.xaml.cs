using System;
using System.Net;

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

            if (Exception is WebException )
            {
                UserFriendlyMessage.Text = Exception.Message;
            }
            ErrorText.Text = Exception.ToString();
        }
    }
}