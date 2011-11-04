using System;
using System.Windows;

namespace SmartReader.Helper
{
    public class CrossThreadHelper
    {
        public static void CrossThreadMethodCall (Action action)
        {
            Deployment.Current.Dispatcher.BeginInvoke(action);
        }
    }
}
