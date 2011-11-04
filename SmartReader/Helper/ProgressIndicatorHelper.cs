using Microsoft.Phone.Shell;

namespace SmartReader.Helper
{
    public class ProgressIndicatorHelper
    {
        private static ProgressIndicator _progressIndicator;

        public static void StartProgressIndicator(bool isIndeterminate)
        {
            if  ( _progressIndicator == null)
            {
                _progressIndicator = new ProgressIndicator();   
            }
                
            _progressIndicator.IsVisible = true;
            _progressIndicator.IsIndeterminate = isIndeterminate;
            _progressIndicator.Text = "Loading";
            SystemTray.ProgressIndicator = _progressIndicator;
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
                                 _progressIndicator.Text = string.Format("Loading " + d.ToString("#0%"));
                              });
            //SystemTray.ProgressIndicator = _progressIndicator;
        }
    }
}
