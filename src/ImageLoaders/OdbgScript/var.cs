using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.ImageLoaders.OdbgScript
{
    using System.Text;
    using rulong = System.UInt64;

    public partial class Var
    {
        public enum etype { EMP, DW, STR, FLT, ADR, };

        private rulong dw;
        public string str;
        public double flt;

        public etype type;
        public int size;
        public bool IsBuf;

        public Reko.Core.Address Address;

        public static Var Create() { return new Var { type = etype.EMP }; }
        public static Var Create(rulong rhs) { return new Var { type = etype.DW, dw = (rhs), size = 4 }; }
        public static Var Create(int rhs) { return new Var { type = etype.DW, dw = (uint)rhs, size = (4) }; } // needed for var = 0
        public static Var Create(uint rhs) { return new Var { type = etype.DW, dw = (rhs), size = (4) }; }
        public static Var Create(double rhs) { return new Var { type = etype.FLT, flt = (rhs), size = (8) }; }
        public static Var Create(Core.Address addr) { return new Var { type = etype.ADR, Address = addr, size = addr.DataType.Size };  }
        public static Var Empty() { return new Var { type = etype.EMP }; }
        
        protected Var() {}

        public bool IsInteger() { return type == etype.DW; }

        public static Var Create(string rhs)
        {
            return new StringVar(rhs);
        }

        public bool IsString() { return type == etype.STR; }

        public static Var operator +(Var lhs, Var rhs)
        {
            switch (rhs.type)
            {
            case etype.DW: return Var.Create(lhs.dw + rhs.dw);
            case etype.STR: return Var.Create(lhs.str + rhs.str);
            case etype.FLT: return Var.Create(lhs.flt + rhs.flt);
            }
            return lhs;
        }

        public static Var operator +(Var lhs, string rhs)
        {
            return lhs.Add(rhs);
        }

        protected virtual Var Add(string rhs)
        {
            Var v = Var.Create(rhs);

            if (this.type == etype.DW)
            {
                if (v.IsBuf) // rulong + buf -> buf
                {
                    return Var.Create("#" + Helper.rul2hexstr(Helper.reverse(this.dw), sizeof(rulong) * 2) + v.ToHexString() + '#');
                }
                else // rulong + str -> str
                {
                    return Var.Create(Helper.toupper(Helper.rul2hexstr(this.dw)) + v.str);
                }
            }
            return this;
        }

        private class StringVar : Var
        {
            public StringVar(string value)
            {
                type = etype.STR;
                IsBuf = false;
                size = value.Length;
                str = value;
                if (Helper.IsHexLiteral(value))
                {
                    str = str.ToUpperInvariant();
                    size = (size / 2) - 1; // num of bytes
                    IsBuf = true;
                }
            }

            protected override Var Add(string rhs)
            {
                var v = Create(rhs);
                if (!this.IsBuf) // str + buf/str -> str
                {
                    return Var.Create(this.str + v.to_string());
                }
                else // buf + buf/str -> buf
                {
                    return Var.Create("#" + this.ToHexString() + v.ToHexString() + '#');
                }
            }

            public override Var Add(rulong rhs)
            {
                if (this.IsBuf) // buf + rulong -> buf
                {
                    return Var.Create("#" + this.ToHexString() + Helper.rul2hexstr(Helper.reverse(rhs), sizeof(rulong) * 2) + '#');
                }
                else // str + rulong -> str
                {
                    return Var.Create(this.str + Helper.rul2hexstr(rhs).ToUpperInvariant());
                }
            }

            public override Var reverse()
            {
                if (IsBuf)
                {
                    char[] revChars = new char[str.Length];
                    int iMax = str.Length - 1;
                    revChars[0] = '#';
                    revChars[iMax] = '#';
                    for (int i = 1, j = iMax - 1; i < j; i += 2, j -= 2)
                    {
                        char c1 = str[i];
                        char c2 = str[i + 1];
                        char c3 = str[j - 1];
                        char c4 = str[j];

                        revChars[i] = c3;
                        revChars[i + 1] = c4;

                        revChars[j - 1] = c1;
                        revChars[j] = c2;
                    }
                    return Var.Create(new string(revChars));
                }
                else
                    return Var.Create(new string(str.Reverse().ToArray()));
            }

            public override string ToHexString()
            {
                if (IsBuf) // #001122# to "001122"
                    return str.Substring(1, str.Length - 2);
                else      // "001122" to "303031313232"
                    return Helper.bytes2hexstr(
                        Encoding.ASCII.GetBytes(str),
                        size)
                        .ToUpperInvariant();
            }

            public override string to_string()
            {
                if (IsBuf) // #303132# to "012"
                {
                    byte[] bytes = new byte[size];
                    throw new NotImplementedException("Helper.hexstr2bytes(to_bytes(), (byte)bytes, size);");
                    //string tmp(bytes, size);
                    //return tmp;
                }
                else return str;
            }
        }

        public static Var operator +(Var lhs, rulong rhs)
        {
            return lhs.Add(rhs);
        }

        public virtual Var Add(rulong rhs)
        {
            switch (type)
            {
            case etype.DW: return Create(this.dw + rhs);  
            case etype.FLT: return Create(this.flt + rhs);
            }
            return this;
        }

        public static Var operator +(Var lhs, double rhs)
        {
            return lhs.Add(rhs);
        }

        public virtual Var Add(double rhs)
        {
            if (this.type == etype.FLT)
                return Var.Create(this.flt + rhs);
            return this;
        }

        public int Compare(Var rhs)
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
                if (IsBuf == rhs.IsBuf)
                    return str.CompareTo(rhs.str);
                else
                    return ToHexString().CompareTo(rhs.ToHexString());
            }
            return -2;
        }

        public virtual string ToHexString()
        {
            return "";
        }

        public virtual string to_string()
        {
            return "";
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
                    if (IsBuf)
                        str = '#' + ToHexString().Substring(0, newsize * 2) + '#';
                    else
                        str.Remove(newsize);
                    size = newsize;
                }
                break;
            }
        }

        public virtual Var reverse()
        {
            switch (type)
            {
            case etype.DW:
                dw = Helper.reverse(dw);
                break;
            }
            return this;
        }

        public ulong ToUInt64()
        {
            if (etype.DW == this.type)
                return dw;
            else if (etype.ADR == this.type)
                return Address.ToLinear();
            throw new NotSupportedException();
        }
    }
}
