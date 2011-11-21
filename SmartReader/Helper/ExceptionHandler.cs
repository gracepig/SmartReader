using System;
using System.Windows.Navigation;
using SmartReader.Helper;
using SmartReader.Views;

namespace SmartReader.Library.Helper
{
    public class ExceptionHandler
    {
        public static void HandleException (Exception e)
        {

            if (CrossThreadHelper.IsUIThread)
            {
                Error.Exception = e;
                PageManager.Navigate(PageManager.ErrorPage);
            }
            else
            {
                CrossThreadHelper.CrossThreadMethodCall(()=>
                                                            {
                                                                Error.Exception = e;
                                                                PageManager.Navigate(PageManager.ErrorPage);
                                                            });
            }
        }
    }
}
