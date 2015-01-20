using System;
using System.Collections.Generic;

namespace Decompiler.ImageLoaders.OdbgScript
{
    using System.Linq;
    using rulong = System.UInt64;
    public partial class var
    {
        public

            enum etype { EMP, DW, STR, FLT };

        public rulong dw;
        public string str;
        public double flt;

        public etype type;
        public int size;
        public bool isbuf;

        public var() { type = etype.EMP; }
        //var(const var   rhs);
        //var(const string& rhs); 
        //var(const char* rhs);
        public var(rulong rhs) { type = etype.DW; dw = (rhs); size = (4); }
        public var(int rhs) { type = etype.DW; dw = (uint)rhs; size = (4); } // needed for var = 0
        public var(uint rhs) { type = etype.DW; dw = (rhs); size = (4); }
#if _WIN64
	var(const ulong&  rhs) : type(DW),  dw(rhs),  size(sizeof(rhs)) {}
#endif
        public var(double rhs) { type = (etype.FLT); flt = (rhs); size = (8); }

        //int compare(var& rhs) const; 

        //string to_bytes() const;
        //string to_string() const;

        //var operator+(const var& rhs) { return var(*this).operator+=(rhs); }

        //var& operator+=(const var& rhs);
        //var& operator+=(const string& rhs);
        //var& operator+=(const rulong& rhs);
        //var& operator+=(const double& rhs);
        //var& operator+=(const int& rhs) { return operator+=((const rulong))rhs; }

        //operator rulong();
        //operator string();
        //operator double();

        //void resize(size_t newsize);
        //void reverse();


        //public var( var rhs)
        //{
        //    *this = rhs;
        //}

        public var(string rhs)
        {
            type = etype.STR; isbuf = (false);

            size = rhs.Length;
            str = rhs;
            if (Helper.is_bytestring(str))
            {
                str = str.ToUpperInvariant();
                size = (size / 2) - 1; // num of bytes
                isbuf = true;
            }
        }

        public static var operator +(var lhs, var rhs)
        {
            switch (rhs.type)
            {
            case etype.DW: return new var(lhs.dw + rhs.dw); break;
            case etype.STR: return new var(lhs.str + rhs.str); break;
            case etype.FLT: return new var(lhs.flt + rhs.flt); break;
            }
            return lhs;
        }

        public static var operator +(var lhs, string rhs)
{
	var v = new var(rhs);

	if(lhs.type == etype.STR)
	{
		if(!lhs.isbuf) // str + buf/str -> str
		{
            return new var(lhs.str + v.to_string());
		}
		else // buf + buf/str -> buf
		{
			return new var("#" + lhs.to_bytes() + v.to_bytes() + '#');
		}
	}
	else if(lhs.type ==etype.DW)
	{
		if(v.isbuf) // rulong + buf -> buf
		{
			return new var("#" + Helper.rul2hexstr(Helper.reverse(lhs.dw), sizeof(rulong)*2) + v.to_bytes() + '#');
		}
		else // rulong + str -> str
		{
			return new var(Helper.toupper(Helper.rul2hexstr(lhs.dw)) + v.str);
		}
	}
	return lhs;
}

        public static var operator +(var lhs, rulong rhs)
{
	switch (lhs.type)
	{
	case etype.DW:  lhs.dw  += rhs; break;
	case etype.FLT: lhs.flt += rhs; break;
	case etype.STR:
		if(lhs.isbuf) // buf + rulong -> buf
		{
			return new var("#"+lhs.to_bytes() + Helper.rul2hexstr(Helper.reverse(rhs), sizeof(rulong)*2) + '#');
		}
		else // str + rulong -> str
		{
            return new var(lhs.str + Helper.toupper(Helper.rul2hexstr(rhs)));
		}
		break;
	}
	return lhs;
}

        public static var operator +(var lhs, double rhs)
        {
            if (lhs.type == etype.FLT)
                return new var(lhs.flt + rhs);
            return lhs;
        }

        /*
        var::operator rulong()
        {
            if(type ==etype.DW)
                return dw;
            else return 0;
        }

        var::operator string()
        {
            if(type == etype.STR)
                return str;
            else return string();
        }

        var::operator double()
        {
            if(type == etype.FLT)
                return flt;
            else return 0.0;
        }
        */

        public int compare(var rhs)
        {
            // less than zero this < rhs
            // zero this == rhs 
            // greater than zero this > rhs 

            if (type != rhs.type || type == etype.EMP)
                return -2;

            switch (type)
            {
            case etype.DW:
                if (dw < rhs.dw) return -1;
                if (dw == rhs.dw) return 0;
                if (dw > rhs.dw) return 1;
                break;

            case etype.FLT:
                if (flt < rhs.flt) return -1;
                if (flt == rhs.flt) return 0;
                if (flt > rhs.flt) return 1;
                break;

            case etype.STR:
                if (isbuf == rhs.isbuf)
                    return str.CompareTo(rhs.str);
                else
                    return to_bytes().CompareTo(rhs.to_bytes());
                break;
            }
            return -2;
        }

        public string to_bytes() 
{
	if(type != etype.STR)
		return "";

    if (isbuf) // #001122# to "001122"
        return str.Substring(1, str.Length - 2);
    else      // "001122" to "303031313232"
        throw new NotImplementedException("return toupper(bytes2hexstr((const byte*)str.data(), size));");
}

        public string to_string() 
{
	if(type != etype.STR)
		return "";

	if(isbuf) // #303132# to "012"
	{
		byte[] bytes = new byte[size];
		throw new NotImplementedException("Helper.hexstr2bytes(to_bytes(), (byte)bytes, size);");
        //string tmp(bytes, size);
        //return tmp;
	}
	else return str;
}

        public void resize(int newsize)
        {
            switch (type)
            {
            case etype.DW:
                dw = Helper.resize(dw, newsize);
                size = newsize;
                break;
            case etype.STR:
                if (newsize < size)
                {
                    if (isbuf)
                        str = '#' + to_bytes().Substring(0, newsize * 2) + '#';
                    else
                        str.Remove(newsize);
                    size = newsize;
                }
                break;
            }
        }

        void reverse()
        {
            switch (type)
            {
            case etype.DW:
                dw = Helper.reverse(dw);
                break;
            case etype.STR:
                if (isbuf)
                {
                    throw new NotImplementedException();
#if LATER
			for(size_t i = 0; i < size/2; i++)
			{
				size_t offs = (i*2)+1;

				char& c1 = str[offs], & c2 = str[offs+1];
				char& c3 = str[str.size()-offs-1-1], & c4 = str[str.size()-offs-1];

				char tmp1 = c1;
				char tmp2 = c2;
				c1 = c3;
				c2 = c4;
				c3 = tmp1;
				c4 = tmp2;
			}
#endif
                }
                else
                    str = new string(str.Reverse().ToArray());
                break;
            }
        }
    }
}
