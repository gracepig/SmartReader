using Microsoft.Phone.Shell;

namespace SmartReader.Helper
{
    public class ProgressIndicatorHelper
    {
        private static ProgressIndicator _progressIndicator;

        public static void StartProgressIndicator(bool isIndeterminate, string message)
        {
            if (_progressIndicator == null)
            {
                _progressIndicator = new ProgressIndicator();
            }

            _progressIndicator.IsVisible = true;
            _progressIndicator.IsIndeterminate = isIndeterminate;
            _progressIndicator.Text = string.IsNullOrEmpty(message) ? "Loading" : message;
            SystemTray.ProgressIndicator = _progressIndicator;
        }

        public static void CrossThreadStartProgressIndicator(bool isIndeterminate, string message)
        {
            CrossThreadHelper.CrossThreadMethodCall(() =>
                                                         {
                             _progressIndicator.IsVisible = true;
                             _progressIndicator.IsIndeterminate = isIndeterminate;
                             _progressIndicator.Text = string.IsNullOrEmpty(message) ? "Loading" : message;
                             SystemTray.ProgressIndicator = _progressIndicator;
                                                         });
        }

        public static void StopProgressIndicator ()
        {
            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() => {
                                    _progressIndicator.IsIndeterminate = false;
                                    _progressIndicator.IsVisible = false;
                                 });
        }

        public static void SetIndicatorValue(double d)
        {
            CrossThreadHelper.CrossThreadMethodCall(() =>
                              {
                                 _progressIndicator.Value = d;
                                 _progressIndicator.Text = string.Format("进度 " + d.ToString("#0%"));
                              });
        }
    }
}
