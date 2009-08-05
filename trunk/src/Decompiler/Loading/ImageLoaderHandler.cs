using Decompiler.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Text;

namespace Decompiler.Loading
{
    /// <summary>
    /// Custom configuration section that 
    /// </summary>
    public class ImageLoaderHandler : ConfigurationSection
    {
        [ConfigurationProperty("MagicNumber", IsRequired = true)]
        public string MagicNumber
        {
            get { return (string) this["MagicNumber"]; }
            set { this["MagicNumber"] = value; }
        }

        [ConfigurationProperty("LoaderType", IsRequired = true)]
        public string LoaderType
        {
            get { return (string) this["LoaderType"]; }
            set { this["LoaderType"] = value; }
        }

        public bool ImageBeginsWithMagicNumber(byte [] image)
        {
            byte[] magic = ConvertHexStringToBytes(MagicNumber);
            if (image.Length < magic.Length)
                return false;
            for (int i = 0; i < magic.Length; ++i)
            {
                if (magic[i] != image[i])
                    return false;
            }
            return true;

        }

        private byte[] ConvertHexStringToBytes(string hexString)
        {
            List<byte> bytes = new List<byte>();
            for (int i = 0; i < hexString.Length; i += 2)
            {
                uint hi = HexDigit(hexString[i]);
                uint lo = HexDigit(hexString[i + 1]);
                bytes.Add((byte) ((hi << 4) | lo));
            }
            return bytes.ToArray();
        }

        private uint HexDigit(char digit)
        {
            switch (digit)
            {
            case '0': case '1': case '2': case '3': case '4': 
            case '5': case '6': case '7': case '8': case '9':
                return (uint) (digit - '0');
            case 'A': case 'B': case 'C': case 'D': case 'E': case 'F':
                return (uint) ((digit - 'A') + 10);
            case 'a': case 'b': case 'c': case 'd': case 'e': case 'f':
                return (uint) ((digit - 'a') + 10);
            default:
                throw new ArgumentException(string.Format("Invalid hexadecimal digit '{0}'", digit));
            }
        }


        public ImageLoader CreateLoaderInstance(Program prog, byte [] bytes)
        {
            Type t = Type.GetType(this.LoaderType);
            ConstructorInfo ci = t.GetConstructor(new Type[] { typeof (Program), typeof(byte[]) });
            return (ImageLoader) ci.Invoke(new object[] { prog, bytes });
        }
    }
}
