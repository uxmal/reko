#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using Decompiler.Core;
using Decompiler.Core.Serialization;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Environments.Win32
{
    /// <summary>
    /// Unmangles methods and data objects whose names have been mangled using Microsoft's 
    /// mangling algorithm.
    /// </summary>
    /// <remarks>
    /// Microsoft doesn't document the mangling format, but reverse engineering has been done, resulting in:
    /// http://www.kegel.com/mangle.html
    /// http://www.agner.org/optimize/calling_conventions.pdf
    /// </remarks>
    public class MsMangledNameParser
    {
        private string str;
        private int i;
        private List<string> namesSeen;
        private List<string> templateNamesSeen;

        public MsMangledNameParser(string str)
        {
            this.str = str;
            this.i = 0;
            this.namesSeen = new List<string>();
        }

        public string Modifier;
        public string ClassName;
        public string Scope;

        public SerializedProcedure Parse()
        {
            Expect('?');
            string basicName = ParseBasicName();
            SerializedType typeCode;
            if (PeekAndDiscard('@'))
            {
                typeCode = ParseUnqualifiedTypeCode();
            }
            else
            {
                string[] qualification = ParseQualification();
                basicName = string.Format(basicName, qualification.Last());
                typeCode = ParseQualifiedTypeCode(qualification);
            }
            string storageClass = ParseStorageClass();
            return new SerializedProcedure
            {
                Name = basicName,
                Signature = (SerializedSignature) typeCode,
            };
        }

        private void Error(string format, params object[] args)
        {
            throw new FormatException(string.Format(format, args));
        }

        private void Expect(char ch)
        {
            if (str[i++] != ch)
                Error("Expected '{0}' but found '{1}'.", ch, str[i-1]);
        }

        private bool PeekAndDiscard(char ch)
        {
            if (str[i] == ch)
            {
                ++i;
                return true;
            }
            else
            {
                return false;
            }
        }

        public string ParseBasicName()
        {
            if (PeekAndDiscard('?'))
            {
                if (!PeekAndDiscard('$'))
                    return ParseOperatorCode();
                // Template!
                var t = new TemplateParser(this).Parse();
                return t.Name;
            }
            int iStart = i;
            while (i < str.Length && str[i] != '@')
                ++i;
            string name = str.Substring(iStart, i - iStart);
            ++i;

            namesSeen.Add(name);
            return name;
        }

        private class TemplateParser
        {
            private  MsMangledNameParser outer;

            public TemplateParser(MsMangledNameParser outer)
            {
                this.outer = outer;
            }

            public SerializedTemplate Parse()
            {
                string name = outer.ParseAtName();
                List<SerializedType> types = outer.ParseTemplateArguments();
                return new SerializedTemplate(null, name, types.ToArray());
            }
        }

        public List<SerializedType> ParseTemplateArguments()
        {
            var types = new List<SerializedType>();
            while (!PeekAndDiscard('@'))
            {
                var t = ParseDataTypeCode();
                types.Add(t);
            }
            return types;
        }

        /// <summary>
        /// Reads a qualification followed by '@'.
        /// </summary>
        /// <returns></returns>
        public string[] ParseQualification()
        {
            var qualifiers = new List<string>();
            SerializedType[] typeArgs = null;
            while (i < str.Length && !PeekAndDiscard('@'))
            {
                string name = ParseAtName();
                if (name.StartsWith("?$"))
                {
                    name = name.Substring(2);
                    var oldNames = namesSeen;
                    if (templateNamesSeen == null)
                    {
                        templateNamesSeen = new List<string> { name };
                    }
                    else
                        templateNamesSeen.Add(name);
                    namesSeen = templateNamesSeen;
                    typeArgs = ParseTemplateArguments().ToArray(); ///$TODO: what to do about these if they're nested?
                    namesSeen = oldNames;
                }
                else
                {
                    namesSeen.Add(name);
                }
                qualifiers.Insert(0, name);
            }
            var tr = new SerializedTypeReference
            {
                TypeName = qualifiers.Last(),
                Scope = qualifiers.Take(qualifiers.Count - 1).ToArray(),
                TypeArguments = typeArgs
            };
            return qualifiers.ToArray();
        }

        public SerializedType ParseQualifiedTypeCode(string [] qualification)
        {
            this.Scope = string.Join("::", qualification);
            switch (str[i++])
            {
            case 'A': return ParseInstanceMethod("private");
            case 'B': return ParseInstanceMethod("private far");
            case 'C': return ParseStaticMethod("private static");
            case 'D': return ParseStaticMethod("private static far");
            case 'E': return ParseInstanceMethod("private virtual");
            case 'F': return ParseInstanceMethod("private virtual far");

            case 'I': return ParseInstanceMethod("protected");
            case 'J': return ParseInstanceMethod("protected far");
            case 'K': return ParseStaticMethod("protected static");
            case 'L': return ParseStaticMethod("protected static far");
            case 'M': return ParseInstanceMethod("protected virtual");
            case 'N': return ParseInstanceMethod("protected virtual far");

            case 'Q': return ParseInstanceMethod("public");
            case 'R': return ParseInstanceMethod("public far");
            case 'S': return ParseStaticMethod("public static");
            case 'T': return ParseStaticMethod("public static far");
            case 'U': return ParseInstanceMethod("public virtual");
            case 'V': return ParseInstanceMethod("public virtual far");
            default:
                Expect('2');
                return ParseDataTypeCode();
            }
        }


        public SerializedType ParseUnqualifiedTypeCode()
        {
            if (PeekAndDiscard('Y'))
            {
                return ParseFunctionTypeCode();
            }
            Expect('3');
            return ParseDataTypeCode();
        }

        public SerializedSignature ParseInstanceMethod(string modifier)
        {
            this.Modifier = modifier;
            ParseThisStorageClass();
            return ParseFunctionTypeCode();
        }

        public SerializedSignature ParseStaticMethod(string modifier)
        {
            return ParseFunctionTypeCode();
        }

        public string ParseStorageClass()
        {
            switch (str[i++])
            {
            case 'A': return "";
            case 'B': return "volatile";
            case 'C': return "const";
            case 'Z': return "__executable";
            default:
                Error("Unknown storage class code '{0}'.", str[i - 1]);
                return null;
            }
        }

        public string ParseThisStorageClass()
        {
            switch (str[i++])
            {
            case 'A': return "";
            case 'B': return "volatile";
            case 'C': return "const";
            case 'E': return "__ptr64";
            case 'F': return "__unaligned";
            default:
                Error("Unknown 'this' storage class code '{0}'.", str[i - 1]);
                return null;
            }
        }

        public SerializedSignature ParseFunctionTypeCode()
        {
            string convention = ParseCallingConvention();
            SerializedType retType;
            if (PeekAndDiscard('@'))
            {
                // C++ ctors have no return type!
                retType = null;
            }
            else
            {
                retType = ParseDataTypeCode();
            }
            SerializedArgument[] args = ParseArgumentList();
            return new SerializedSignature
            {
                Convention = convention,
                Arguments = args,
                ReturnValue = new SerializedArgument { Type=retType!= null ? retType : new SerializedVoidType() }
            };
        }

        public string ParseOperatorCode()
        {
            switch (str[i++])
            {
            case '0': return "{0}";
            case '1': return "~{0}";
            case '2': return "operator new";
            default: Error("Unknown operator code '{0}'.", str[i - 1]);
                return null;
            }
        }

        public string ParseCallingConvention()
        {
            switch (str[i++])
            {
            case 'A': return "__cdecl";
            case 'C': return "__pascal";
            case 'E': return "__thiscall";
            case 'G': return "__stdcall";
            case 'I': return "__fastcall";
            case 'K': return "";
            case 'M': return "__clrcall";
            case 'O': return "__eabi";
            default: Error("Unknown calling convention code '{0}'.", str[i - 1]);
                return null;
            }
        }
        
        public SerializedArgument[] ParseArgumentList()
        {
            var args = new List<SerializedArgument>();
            var arg = ParseDataTypeCode();
            if (arg != null)
            {
                args.Add(new SerializedArgument { Type = arg });
                while (!PeekAndDiscard('@'))
                {
                    arg = ParseDataTypeCode();
                    if (arg == null)
                        break;
                    args.Add(new SerializedArgument { Type = arg });
                }
            }
            return args.ToArray();
        }

        public SerializedType ParseDataTypeCode()
        {
            switch (str[i++])
            {
            case 'A': return ParsePointer();        //$TODO: really is a reference but is implemented as a pointer on Win32...
            case 'C': return new SerializedPrimitiveType(Domain.Character | Domain.SignedInt, 1);
            case 'D': return new SerializedPrimitiveType(Domain.Character, 1);
            case 'E': return new SerializedPrimitiveType(Domain.Character | Domain.UnsignedInt, 1);
            case 'F': return new SerializedPrimitiveType(Domain.SignedInt, 2);
            case 'G': return new SerializedPrimitiveType(Domain.UnsignedInt, 2);
            case 'H': return new SerializedPrimitiveType(Domain.SignedInt, 4);
            case 'I': return new SerializedPrimitiveType(Domain.UnsignedInt, 4);
            case 'J': return new SerializedPrimitiveType(Domain.SignedInt, 4);      // 'long' on Win32 is actually 4 bytes
            case 'K': return new SerializedPrimitiveType(Domain.UnsignedInt, 4);  // 'long' on Win32 is actually 4 bytes
            case 'M': return new SerializedPrimitiveType(Domain.Real, 4);
            case 'N': return new SerializedPrimitiveType(Domain.Real, 8);
            case 'O': return new SerializedPrimitiveType(Domain.Real, 10);
            case 'P': return ParsePointer();
            //case 'P': pointer        (see below)
            //case 'Q': array          (see below)
            case 'U': return ParseStructure(); // struct (see below)
            case 'V': return ParseStructure(); // class (see below)
            case 'X': return null;      // void           (terminates argument list)
            case 'Z': return null;      // elipsis        (terminates argument list)
            default: Error("Unsupported type code '{0}'.", str[i - 1]); return null;
            }
        }

        public SerializedPointerType ParsePointer()
        {
            int size = 0;       //$REVIEW how to deal with 64-bitness
            switch (str[i++])
            {
            case 'A': size = 4; break;      //$BUG: assumes 32-bitness
            case 'B': size = 4; break;      // const ptr
            case 'C': size = 4; break;      // volatile ptr
            case 'D': size = 4; break;      // const volatile ptr
            default: Error("Unsupported pointer code 'P{0}'.", str[i - 1]); break;
            }
            var type = ParseDataTypeCode();
            return new SerializedPointerType
            {
                DataType = type,
                PointerSize = size,
            };
        }

        public SerializedTypeReference ParseStructure()
        {
            var q = ParseQualification();
            var tr = new SerializedTypeReference
            {
                TypeName = q.Last(),
                Scope = q.Take(q.Length - 1).ToArray()
            };
            return tr;
        }

        internal string ParseAtName()
        {
            int iStart = i;
            while (i < str.Length && str[i] != '@')
            {
                if (Char.IsDigit(str[i]))
                {
                    return namesSeen[str[i++] - '0'];
                }
                ++i;
            }
            string name = str.Substring(iStart, i - iStart);
            Expect('@');
            return name;
        }
    }
}
