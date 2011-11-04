using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Windows;

namespace SmartReader.Library.Helper
{
    public class GBKEncoding
    {

        private void PrepareMappingTable()
        {

            
            var reader = new StreamReader(@"C:\Temp\EncodeMapping\Result1.txt");

            while (reader.Peek() > 0)
            {
                var line = reader.ReadLine();

                var parts = line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2 || String.IsNullOrEmpty(parts[1].Trim()))
                    continue;

                var gbk = String_To_Bytes2(parts[0].Substring(2, parts[0].Length - 2));
                var unicode = String_To_Bytes2(parts[1].Substring(2, parts[1].Length - 2));


                Mapping.Add(BitConverter.ToString(gbk), unicode);
            }
        }

        private byte[] String_To_Bytes2(string strInput)
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


        Dictionary<string, byte[]> Mapping = new Dictionary<string, byte[]>();
    }
}
