using System;
using System.IO;

namespace SmartReader.Library.Interface
{
    public interface IParser
    {
        object Parse(Stream inputStream, object state);

        event EventHandler ParsingCompleted; 
    }
}
