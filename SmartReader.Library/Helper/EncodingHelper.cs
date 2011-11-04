using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Linq;

namespace SmartReader.Library.Helper
{
    public class EncodingHelper
    {
        public static Dictionary<string , byte []> Mapping = new Dictionary<string,byte[]>();

        public static string FromGBKToUnicode (Stream inputStream)
        {
            var reader = new BinaryReader(inputStream);
            var list = new List<byte>();
            do
            {
                try
                {
                    var x = reader.ReadByte();
                    if (x < 0x81)
                    {
                        list.AddRange(new[] { x, (byte)0 });
                    }
                    else
                    {
                        try
                        {
                            var y = reader.ReadByte();
                            var unicode = Mapping[BitConverter.ToString(new[] { x, y })];
                            list.AddRange(unicode.Reverse().ToArray());    
                        }
                        catch (KeyNotFoundException e)
                        {
                            //Swallow it
                        }

                    }
                }
                catch (EndOfStreamException)
                {
                    reader.Close();
                    break;
                }
            } while (true);

            var result = Encoding.Unicode.GetString(list.ToArray(),0, list.Count);
            return result;
        }

        public static void BuildGBKToUnicodeMapping ()
        {
            var mappingData = Application.GetResourceStream(new Uri("/SmartReader.Library;component/Resource/GBKToUnicode.txt", UriKind.Relative));

            var reader = new StreamReader(mappingData.Stream);
            while (reader.Peek() > 0)
            {
                var line = reader.ReadLine();

                var parts = line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2 || String.IsNullOrEmpty(parts[1].Trim()))
                    continue;

                var gbk = StringToBytes2(parts[0].Substring(2, parts[0].Length - 2));
                var unicode = StringToBytes2(parts[1].Substring(2, parts[1].Length - 2));


                Mapping.Add(BitConverter.ToString(gbk), unicode);
            }
        }

        private static byte[] StringToBytes2(string strInput)
        {
            // allocate byte array based on half of string length
            int numBytes = (strInput.Length) / 2;
            byte[] bytes = new byte[numBytes];

            // loop through the string - 2 bytes at a time converting it to decimal equivalent and store in byte array
            // x variable used to hold byte array element position
            for (int x = 0; x < numBytes; ++x)
            {
                bytes[x] = Convert.ToByte(strInput.Substring(x * 2, 2), 16);
            }

            // return the finished byte array of decimal values
            return bytes;
        }
    }
}
