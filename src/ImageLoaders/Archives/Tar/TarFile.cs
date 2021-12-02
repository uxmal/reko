using Reko.Core;
using Reko.Core.Loading;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Reko.ImageLoaders.Archives.Tar
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct tar_header
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public byte[] filename;    // Encoded in UTF-8
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] octFilemode;       // The following fields are encoded in octal.
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] octUserID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] octGroupID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public byte[] octFileSize;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public byte[] octLastModified;  // Unix time format(octal)
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] octChecksum;
        public byte fileType;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public byte[] linkedFilename;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ustar_header
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] magic;     // UStar indicator "ustar" then NUL
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] version;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] ownerUserName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] ownerGroupName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] deviceMajorNumber;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] deviceMinorNumber;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 155)]
        public byte[] filenamePrefix;
    }

    public class TarFile : ArchivedFile
    {
        private IArchive archive;
        private readonly ByteImageReader rdr;
        private readonly long offset;

        private TarFile(IArchive archive, ArchiveDirectoryEntry? parent, ByteImageReader rdr)
        {
            this.archive = archive;
            this.Parent = parent;
            this.rdr = rdr;
            this.offset = rdr.Offset;
            this.Name = default!;
            this.Filename = default!;
        }

        public static TarFile Load(IArchive archive, ArchiveDirectoryEntry? parent, string name, in tar_header tarHdr, in ustar_header? ustarHdr, ByteImageReader rdr)
        {
            var filename = GetString(tarHdr.filename);
            var length = GetOctal(tarHdr.octFileSize);
            return new TarFile(archive, parent, rdr)
            {
                Filename = filename,
                Name = name,
                Length = length
            };
        }

        public string Filename { get; private set; }
        public long Length { get; private set; }
        public string Name { get; private set; }
        public ArchiveDirectoryEntry? Parent { get; }

        public byte[] GetBytes()
        {
            this.rdr.Offset = this.offset;
            return rdr.ReadBytes((int)Length);
        }

        public ILoadedImage LoadImage(IServiceProvider services, Address? addrLoad)
        {
            var path = archive.GetRootPath(this);
            var imageLocation = archive.Location.AppendFragment(path);
            return new Blob(imageLocation, GetBytes());
        }

        private static long GetOctal(byte[] aOctalString)
        {
            long result = 0;
            foreach (var b in aOctalString)
            {
                if ((byte) '0' <= b && b <= '7')
                {
                    result = result * 8 + (b - (byte) '0');
                }
                else
                    return result;
            }
            return result;
        }
        public static string GetString(byte[] aString)
        {
            int i = Array.IndexOf(aString, (byte) 0);
            var encoding = Encoding.UTF8;
            if (i < 0)
                return encoding.GetString(aString);
            else
                return encoding.GetString(aString, 0, i);
        }
    }
}
