using System;

namespace SmartReader.Library.Helper
{
    public class UrlHelper
    {
        public static string GetRootUrlString ( Uri uri)
        {
            //http://shuzong.com/5200/0/26/1341054.shtml
            string s = uri.ToString();

            return s.Substring(0, s.LastIndexOf("/")+1);
        }
    }
}
