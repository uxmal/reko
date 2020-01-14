using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Reflection;

namespace Reko.ImageLoaders.OdbgScript
{
    using rulong = System.UInt64;
    using System.Text;
    using System.Linq;
    using Reko.Core.Lib;

    public static class Helper
    {
        // Number conversion

        const string digits = "0123456789ABCDEFGHIJKLMNOPQRSTUV";

        public static rulong char2rul(char c)
        {
            c = Char.ToUpper(c);
            for (int i = 0; i < digits.Length; i++)
            {
                if (c == digits[i])
                    return (uint)i;
            }
            return ~(rulong)0;
        }

        public static rulong str2rul(string str, uint @base)
        {
            rulong num = 0;
            rulong charnum;

            if (@base < 2 || @base > 32)
                return 0;

            for (int i = 0; i < str.Length; i++)
            {
                charnum = char2rul(str[i]);
                if (charnum < 0 || charnum >= @base)
                    break;

                num *= @base;
                num += charnum;
            }
            return num;

            /*
            #ifdef _WIN64
                return _strtoui64(string.c_str(), NULL, Base);
            #else
                return strtoul(string.c_str(), NULL, Base);
            #endif
            */
        }

        public static rulong hexstr2rul(string s)
        {
            return str2rul(s, 16);
        }

        public static rulong decstr2rul(string s)
        {
            return str2rul(s, 10);
        }

        public static double str2dbl(string s)
        {
            return Convert.ToDouble(s);
        }

        public static string rul2str(rulong x, uint @base, uint fill = 0)
        {
            StringBuilder @out = new StringBuilder();
            uint i = 0;

            if (@base < 2 || @base > 32)
                return "";
            do
            {
                @out.Insert(0, digits[(int)(x % @base)]);
                x /= @base;
                i++;
            }
            while (x != 0 || i < fill);

            return @out.ToString();

            /*
        #ifdef _WIN64
            _ui64toa(x, buffer, Base);
        #else
            _ultoa(x, buffer, Base);
        #endif
            string @out = buffer;
            if(fill > @out.size())
                @out.insert(0, fill-@out.size(), '0');
            return @out;
            */
        }

        public static string rul2hexstr(rulong x, uint fill = 0)
        {
            return rul2str(x, 16, fill);
        }

        public static string rul2decstr(rulong x, uint fill = 0)
        {
            return rul2str(x, 10, fill);
        }

        public static string dbl2str(double x)
        {
            return x.ToString(CultureInfo.InvariantCulture);
        }

        // Number manipulation

        public static rulong reverse(rulong dw)
        {
            throw new NotImplementedException();
#if LATER
            byte [] pdw = (byte*)&dw;
            reverse(pdw, pdw + sizeof(dw));
            return dw;
#endif
        }

        /// <summary>
        /// Masks off the high part of the given value.
        /// </summary>
        public static rulong resize(rulong dw, int size)
        {
            if (0 <= size && size < sizeof(ulong))
            {
                dw &= Bits.Mask(0, size * 8);
            }
            return dw;
        }

        public static rulong round_up(rulong dw, rulong val)
        {
            rulong mod = dw % val;
            if (mod != 0)
                dw += (val - mod);
            return dw;
        }

        public static rulong round_down(rulong dw, rulong val)
        {
            return (dw - (dw % val));
        }

        // Memory functions

        public static string bytes2hexstr(byte[] bytes, int size)
        {
            StringBuilder @out = new StringBuilder(size * 2);

            for (int i = 0; i < size; i++)
            {
                @out.Append(rul2hexstr(bytes[i], 2));
            }
            return @out.ToString();
        }

        public static int hexstr2bytes(string s, byte [] bytes, int size)
        {
            size = Math.Min(size, s.Length / 2);
            for (int i = 0; i < size; i++)
            {
                char[] sub = s.Substring(i * 2, 2).ToCharArray();
                if (sub[0] == '?') sub[0] = '0';
                if (sub[1] == '?') sub[1] = '0';
                bytes[i] = (byte) hexstr2rul(new string(sub));
            }
            return size;
        }

        public static int hexstr2bytemask(string s, byte[] mask, int size)
        {
            size = Math.Min(size, s.Length / 2);
            for (int i = 0; i < size; ++i)
                mask[i] = 0;
            for (int i = 0, e = s.Length; i < e; i++)
            {
                if (s[i] != '?')
                    mask[i / 2] |= (byte)(0xF0 >> ((i % 2) * 4));
            }
            return size;
        }

        public static bool MaskedCompare(byte[] b1, int offset, byte[] b2, byte[] mask, int size)
        {
            for (int i = 0; i < size; i++)
            {
                if ((b1[i+offset] & mask[i]) != (b2[i] & mask[i]))
                    return false;
            }
            return true;
        }

        public static bool memcpy_mask(byte[] b1, int offset, byte[] b2, byte[] mask, int size)
        {
            for (int i = 0; i < size; i++)
            {
                b1[i+offset] = (byte)((b1[i+offset] & ~mask[i]) | (b2[i] & mask[i]));
            }
            return true;
        }

        // Script stuff

        public static int FindFirstNotOf(this string source, string chars)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (chars == null) throw new ArgumentNullException("chars");
            if (source.Length == 0) return -1;
            if (chars.Length == 0) return 0;

            for (int i = 0; i < source.Length; i++)
            {
                if (chars.IndexOf(source[i]) == -1) return i;
            }
            return -1;
        }

        public static int FindLastNotOf(this string source, string chars)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (chars == null) throw new ArgumentNullException("chars");
            if (source.Length == 0) return -1;
            if (chars.Length == 0) return source.Length - 1;

            for (int i = source.Length - 1; i >= 0; i--)
            {
                if (chars.IndexOf(source[i]) == -1) return i;
            }
            return -1;
        }

        public static bool is_hex(string s)
        {
            return (s.Length!=0 && s.FindFirstNotOf("0123456789abcdefABCDEF") < 0); // '-' ?
        }

        public static bool IsHexWild(string s)
        {
            return (s.Length!=0 && s.FindFirstNotOf("?0123456789abcdefABCDEF") < 0); // '-' ?
        }

        public static bool is_dec(string s)
        {
            int len = s.Length;
            return (len >= 2 && s[len - 1] == '.' && s.FindFirstNotOf("0123456789") == len - 1);
        }

        public static bool is_float(string s)
        {
            int p = s.IndexOf('.');
            if (p < 0 || p + 1 == s.Length)
                return false;

            return (s.Substring(0, p).FindFirstNotOf("0123456789") < 0 &&
                    s.Substring(p + 1).FindFirstNotOf("0123456789") < 0);
        }


        // A hex literal matches:
        // #(..)*#
        // where . is a hex wild character.
        public static bool IsHexLiteral(string s)
        {
            int len = s.Length;
            return (len >= 2 && (len % 2) == 0 && s[0] == '#' && s[len - 1] == '#' && IsHexWild(s.Substring(1, len - 2)));
        }

        public static bool IsStringLiteral(string s)
        {
            int len = s.Length;
            return (len > 2 && s[0] == '"' && s.IndexOf('"', 1) == len - 1);
        }

        public static bool IsInterpolatedString(string s)
        {
            return s.Length >= 3 &&
                s.StartsWith("$\"") &&
                s.EndsWith("\"");
        }

        public static bool IsMemoryAccess(string s)
        {
            int len = s.Length;
            return (len > 2 && s[0] == '[' && s[len - 1] == ']');
        }

        // String manipulation

        public static string toupper(string @in)
        {
            return @in.ToUpperInvariant();
        }

        public static string reverse(string @in)
        {
            return new string(@in.Reverse().ToArray());
        }

        internal const string whitespaces = " \t\r\n\f";

        public static string trim(string s) // Thanks to A. Focht for this one
        {
            int left, right;

            if ((left = s.FindFirstNotOf(whitespaces)) >= 0)
            {
                right = s.FindLastNotOf(whitespaces);
                return s.Substring(left, right - left + 1);
            }
            return "";
        }

        public static IEnumerable<string> split(char delim, string str)
        {
            List<int> pos = new List<int>();
            bool inQuotes = false;

            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '"')
                    inQuotes = !inQuotes;

                if (str[i] == delim && !inQuotes)
                    pos.Add(i);
            }

            int start = 0, end;

            for (int i = 0; i < pos.Count; i++)
            {
                end = pos[i];
                yield return str.Substring(start, end - start);
                start = end + 1;
            }

            if (start < str.Length)
                yield return str.Substring(start);
        }

        public static bool IsQuotedString(string s, char cstart, char cend = '\0')
        {
            if (cend == 0)
                cend = cstart;

            return (s.Length >= 2 && s[0] == cstart && s[s.Length - 1] == cend);
        }

        public static string UnquoteInterpolatedString(string s)
        {
            return s.Substring(2, s.Length - 3);
        }

        public static string UnquoteString(string s, char cstart, char cend = '\0')
        {
            if (cend == 0)
                cend = cstart;

            if (!IsQuotedString(s, cstart, cend))
                return s;

            var result = s.Substring(1, s.Length - 2);
            if (cstart == '"')
            {
                result = result.Replace("\\r\\n", "\r\n");
            }
            return result;
        }

        public static void ReplaceString(ref string s, string what, string with)
        {
            throw new NotImplementedException();
#if NOTYET
            int p = 0;
            int la = what.Length;
            int li = with.Length;

            while ((p = s.IndexOf(what, p)) >= 0)
            {
                s.replace(p, la, with, li);
                p += li;
            }
#endif
        }

        //Remove characters in string for display
        public static string CleanString(string s)
        {
            StringBuilder str = new StringBuilder();

            for (int p = 0; p < s.Length; p++)
            {
                char ch = str[p];
                str.Append(ch != '\0' ? ch : ' ');
            }
            return str.ToString();
        }

        // File functions

        public static string pathfixup(string path, bool isfolder)
        {
            string @out = path;

            if (!string.IsNullOrEmpty(@out))
            {
				if (isfolder && @out[@out.Length-1] != Path.DirectorySeparatorChar)
					@out += Path.DirectorySeparatorChar;
            }
            return @out;
        }

        public static string filefrompath(string path)
        {
            string @out = path;
            int p;

            if ((p = @out.LastIndexOf('\\')) >= 0)
                @out = @out.Substring(p + 1);

            return @out;
        }

        public static List<string> ReadLinesFromFile(string file)
        {
            using (StreamReader hFile = new StreamReader(file, Encoding.UTF8))
            {
                return ReadLines(hFile);
            }
        }

        public static List<string> ReadLines(TextReader content)
        {
            List<string> script = new List<string>();
            string line = content.ReadLine();
            while (line != null)
            {
                script.Add(line);
                line = content.ReadLine();
            }
            return script;
        }

        // Misc functions
#if LATER
BOOL CALLBACK EnumThreadWndProc(HWND hwnd, LPARAM lParam) 
{
	*(HWND*)lParam = hwnd;
	return !IsWindowVisible(hwnd);
}

HWND FindHandle(DWORD dwThreadId, string  wdwClass, long x, long y) 
{
char buffer[256] = {0};
HWND handle, desktop, parent;

	EnumThreadWindows(dwThreadId, &EnumThreadWndProc, (LPARAM)&parent);

	desktop = GetDesktopWindow();

	do
	{
		handle = parent;
		parent = GetParent(handle);
	}
	while(parent && parent != desktop);

	POINT pt = { x, y };

	handle = ChildWindowFromPoint(handle, pt);
	if(handle)
	{
 		GetClassName(handle, buffer, _countof(buffer)); 
		if(!_strnicmp(buffer, wdwClass.c_str(), wdwClass.Length))
			return handle;
	}	
	return 0;
}
#endif

        public static ulong MyTickCount()
        {
            return (uint) System.Environment.TickCount * 1000uL;
#if LATER
            ulong PerformanceCount = { 0 }, Frequency = { 0 };
            ulong result;

            if (!QueryPerformanceFrequency(&Frequency) || !Frequency.QuadPart || !QueryPerformanceCounter(&PerformanceCount))
            {
                // millseconds * 1,000 = microseconds
                result = GetTickCount() * 1000;
            }
            else
            {
                // ticks * 1,000,000 / ticks per second = seconds * 1,000,000 = microseconds
                result = (PerformanceCount.QuadPart * 1000000) / Frequency.QuadPart;
            }
            return result;
#endif
        }

#if LATER

        bool GetAppVersionString(string LibName, string Value, out string Output)
{
DWORD dwHandle, dwLen;
UINT BufLen;
BYTE * lpData;
DWORD * lpTranslate;
const char * lpPropertyBuffer;

	dwLen = GetFileVersionInfoSize(LibName, &dwHandle);
	if(!dwLen) 
		return false;

	lpData = new BYTE[dwLen];

	if(GetFileVersionInfo(LibName, dwHandle, dwLen, lpData))
	{
		// Read the list of languages and code pages.
		if(VerQueryValue(lpData, "\\VarFileInfo\\Translation", (void **)&lpTranslate, &BufLen))
		{
			// Use first language
			string ValueString = "\\StringFileInfo\\" + rul2hexstr(LOWORD(*lpTranslate), 4) + rul2hexstr(HIWORD(*lpTranslate), 4) + "\\" + Value;
			if(VerQueryValue(lpData, ValueString.c_str(), (void **)&lpPropertyBuffer, &BufLen))
			{
				Output = lpPropertyBuffer;
				delete[] lpData;
				return true;
			}
		}
	}

	delete[] lpData;
	return false;
}
#endif

        /*
        bool GetAppVersion(const char * LibName, WORD * MajorVersion, WORD * MinorVersion, WORD * BuildNumber, WORD * RevisionNumber)
        {
        DWORD dwHandle, dwLen;
        UINT BufLen;
        BYTE * lpData;
        VS_FIXEDFILEINFO * pFileInfo;

            dwLen = GetFileVersionInfoSize(LibName, &dwHandle);
            if(!dwLen) 
                return false;

            lpData = new BYTE[dwLen];

            if(GetFileVersionInfo(LibName, dwHandle, dwLen, lpData))
            {
                if(VerQueryValue(lpData, "\\", (void**)&pFileInfo, &BufLen)) 
                {
                    *MajorVersion   = HIWORD(pFileInfo->dwFileVersionMS);
                    *MinorVersion   = LOWORD(pFileInfo->dwFileVersionMS);
                    *BuildNumber    = HIWORD(pFileInfo->dwFileVersionLS);
                    *RevisionNumber = LOWORD(pFileInfo->dwFileVersionLS);
                    delete[] lpData;
                    return true;
                }
            }

            delete[] lpData;
            return false;
        }
        */
    }
}