using System;
using System.Threading;
using System.Windows;

namespace SmartReader.Helper
{
    public class CrossThreadHelper
    {
        public static int UIThreadId;
        
        public static bool IsUIThread
        {
            get { return UIThreadId == Thread.CurrentThread.ManagedThreadId; }
        } 

        public static void CrossThreadMethodCall (Action action)
        {
            Deployment.Current.Dispatcher.BeginInvoke(action);
        }
    }
}
