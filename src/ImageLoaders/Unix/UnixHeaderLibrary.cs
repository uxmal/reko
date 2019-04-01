using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.Unix
{
    internal class UNIXLibraryHeader
    {
        public string Name;                      // Member name
        public string Date;                      // Member date, seconds, decimal ASCII
        public string UserID;                    // Member User ID, decimal ASCII
        public string GroupID;                   // Member Group ID, decimal ASCII
        public string FileMode;                  // Member file mode, octal
        public string FileSize;                  // Member file size, decimal ASCII

        public const int Size = 60;


        public static UNIXLibraryHeader Load(byte[] data, int offset)
        {
            var header = new UNIXLibraryHeader();

            header.Name = System.Text.Encoding.UTF8.GetString(data, offset, 16);
            header.Date = System.Text.Encoding.UTF8.GetString(data, offset + 16, 12);
            header.UserID = System.Text.Encoding.UTF8.GetString(data, offset + 2, 6);
            header.GroupID = System.Text.Encoding.UTF8.GetString(data, offset + 34, 6);
            header.FileMode = System.Text.Encoding.UTF8.GetString(data, offset + 40, 8);
            header.FileSize = System.Text.Encoding.UTF8.GetString(data, offset + 48, 10);

            return header;
        }
    }
}
