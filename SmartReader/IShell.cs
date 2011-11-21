using System;

namespace SmartReader
{
    public interface IShell
    {
        void ShowExceptionError(Exception ex);
        void Navigate(Uri uri);
    }
}
