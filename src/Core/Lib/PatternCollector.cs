using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

#pragma warning disable CS1591

namespace Reko.Core.Lib
{


    //const int SYMLEN = 16 ;         /* Number of chars in the symbol name, incl null */
    //        const int PATLEN = 23 ;         /* Number of bytes in the pattern part */

    public struct HASHENTRY
    {
        const int SYMLEN = 16;         /* Number of chars in the symbol name, incl null */
        const int PATLEN = 23;         /* Number of bytes in the pattern part */

        public string? name;         // char name[SYMLEN];      /* The symbol name */
        public byte[]? pat; // [PATLEN];   /* The pattern */
        public ushort? offset;        /* Offset (needed temporarily) */
    };

    public abstract class PatternCollector
    {
        const int SYMLEN = 16;         /* Number of chars in the symbol name, incl null */
        const int PATLEN = 23;         /* Number of bytes in the pattern part */

#if NO
        byte buf[100], bufSave[7];   /* Temp buffer for reading the file */

            uint16_t readShort(FILE* f)
            {
                uint8_t b1, b2;

                if (fread(&b1, 1, 1, f) != 1)
                {
                    printf("Could not read\n");
                    exit(11);
                }
                if (fread(&b2, 1, 1, f) != 1)
                {
                    printf("Could not read\n");
                    exit(11);
                }
                return (b2 << 8) + b1;
            }

            void grab(FILE* f, int n)
            {
                if (fread(buf, 1, n, f) != (size_t)n)
                {
                    printf("Could not read\n");
                    exit(11);
                }
            }

            uint8_t readByte(FILE* f)
            {
                uint8_t b;

                if (fread(&b, 1, 1, f) != 1)
                {
                    printf("Could not read\n");
                    exit(11);
                }
                return b;
            }

            uint16_t readWord(FILE* fl)
            {
                uint8_t b1, b2;

                b1 = readByte(fl);
                b2 = readByte(fl);

                return b1 + (b2 << 8);
            }
#endif
        /* Called by map(). Return the i+1th key in *pKeys */
        public byte[]? getKey(int i)
        {
            return keys[i].pat;
        }

        /* Display key i */
        public void dispKey(int i)
        {
            Debug.Print("{0}", keys[i].name);
        }
        List<HASHENTRY> keys = new List<HASHENTRY>(); /* array of keys */

        public abstract int readSyms(Stream f);
    }

}