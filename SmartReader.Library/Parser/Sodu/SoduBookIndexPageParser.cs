using System.IO;
using HtmlAgilityPack;
using SmartReader.Library.Interface;

namespace SmartReader.Library.Parser
{
    public class SoduBookIndexPageParser : IParser 
    {
        public object Parse ( Stream input, object state)
        {
            var doc = new HtmlDocument();
            doc.Load(input);

            return null;
        }


        public event System.EventHandler ParsingCompleted;
    }
}
