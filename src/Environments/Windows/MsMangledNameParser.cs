#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.Environments.Windows
{
    /// <summary>
    /// Unmangles methods and data objects whose names have been mangled using Microsoft's
    /// mangling algorithm.
    /// </summary>
    /// <remarks>
    /// Microsoft doesn't document the mangling format, but reverse engineering has been done, resulting in:
    /// http://www.kegel.com/mangle.html
    /// http://www.agner.org/optimize/calling_conventions.pdf
    /// https://github.com/wine-mirror/wine/dlls/msvcrt/undname.c
    /// https://mearie.org/documents/mscmangle/
    /// </remarks>
    public class MsMangledNameParser
    {
        private string str;
        private int i;
        private List<string> namesSeen;
        private int pointerSize;
        private List<string> templateNamesSeen;
        private List<Argument_v1> compoundArgs;
        private bool isInstanceMethod;

        public MsMangledNameParser(string str)
        {
            this.str = str;
            this.i = 0;
            this.namesSeen = new List<string>();
            this.pointerSize = 4;  //$TODO: should follow platform pointer size, really.
            this.compoundArgs = null!;
            this.templateNamesSeen = null!;
        }

        public string? Modifier;
        public string? ClassName;
        public string? Scope;
        private bool isConstuctor;

        public (string?,SerializedType?,SerializedType?) Parse()
        {
            Expect('?');
            string? basicName = ParseBasicName();
            if (basicName is null)
                return (null, null, null);
            (string, SerializedType?, SerializedType?) typeCode;
            var compoundArgs =     new List<Argument_v1>();
            if (PeekAndDiscard('@'))
            {
                typeCode = ParseUnqualifiedTypeCode(basicName);
            }
            else
            {
                string[] qualification = ParseQualification();
                basicName = string.Format(basicName, qualification.Last());
                typeCode = ParseQualifiedTypeCode(basicName, qualification, compoundArgs);
            }
            return typeCode;
        }

        private void Error(string format, params object[] args)
        {
            throw new FormatException(string.Format(format, args));
        }

        [Conditional("DEBUG")]
        private void Dump(int i)
        {
            Debug.WriteLine(str);
            Debug.Print("{0}^", new string(' ', i));
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

        public string? ParseBasicName()
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
                return new SerializedTemplate(null!, name, types.ToArray());
            }
        }

        public List<SerializedType> ParseTemplateArguments()
        {
            var old = this.compoundArgs;
            this.compoundArgs = new List<Argument_v1>();
            var types = new List<SerializedType>();
            while (!PeekAndDiscard('@'))
            {
                if (PeekAndDiscard('$'))
                {
                    ParseNonTypeTemplateArgument();
                }
                else
                {
                    var t = ParseDataTypeCode(this.compoundArgs);
                    if (t is not null)
                        types.Add(t);
                }
            }
            this.compoundArgs = old;
            return types;
        }

        private void ParseNonTypeTemplateArgument()
        {
            switch (str[i++])
            {
            case '0':
                if ('0' <= str[i] && str[i] <= '9')
                {
                    ++i;
                }
                else
                {
                    while (str[i++] != '@')
                        ;
                }
                break;  // Integer value.
            case '2': throw new NotSupportedException();    // real value
            case 'D': throw new NotSupportedException();    // Anonymous
            default: Dump(i); Error("Unknown template argument {0}.", str[i - 1]); break;
            }
        }

        /// <summary>
        /// Reads a qualification followed by '@'.
        /// </summary>
        /// <returns></returns>
        public string[] ParseQualification()
        {
            var qualifiers = new List<string>();
            SerializedType[]? typeArgs = null;
            while (i < str.Length && !PeekAndDiscard('@'))
            {
                string name = ParseAtName();
                if (name.StartsWith("?$"))
                {
                    name = name.Substring(2);
                    namesSeen.Add(name);
                    var oldNames = namesSeen;
                    if (templateNamesSeen is null)
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
            var tr = new TypeReference_v1
            {
                TypeName = qualifiers.Last(),
                Scope = qualifiers.Take(qualifiers.Count - 1).ToArray(),
                TypeArguments = typeArgs
            };
            return qualifiers.ToArray();
        }

        public (string, SerializedType?, SerializedType?) ParseQualifiedTypeCode(string basicName, string[] qualification, List<Argument_v1> compoundArgs)
        {
            this.compoundArgs = new List<Argument_v1>();
            this.Scope = string.Join("::", qualification);
            SerializedSignature? sig = null;
            switch (str[i++])
            {
            case '0':
            case '1':
            case '2':
            case '3':
                return (
                    basicName,
                    ParseDataTypeCode(compoundArgs),
                    CreateEnclosingType(Scope));
            case '6':   // Compiler-generated static
                //$TODO: deal with const/volatile modifier
                ParseStorageClass();
                break;
            case 'A': sig = ParseInstanceMethod("private"); break;
            case 'B': sig = ParseInstanceMethod("private far"); break;
            case 'C': sig = ParseStaticMethod("private static"); break;
            case 'D': sig = ParseStaticMethod("private static far"); break;
            case 'E': sig = ParseInstanceMethod("private virtual"); break;
            case 'F': sig = ParseInstanceMethod("private virtual far"); break;

            case 'I': sig = ParseInstanceMethod("protected"); break;
            case 'J': sig = ParseInstanceMethod("protected far"); break;
            case 'K': sig = ParseStaticMethod("protected static"); break;
            case 'L': sig = ParseStaticMethod("protected static far"); break;
            case 'M': sig = ParseInstanceMethod("protected virtual"); break;
            case 'N': sig = ParseInstanceMethod("protected virtual far"); break;

            case 'Q': sig = ParseInstanceMethod("public"); break;
            case 'R': sig = ParseInstanceMethod("public far"); break;
            case 'S': sig = ParseStaticMethod("public static"); break;
            case 'T': sig = ParseStaticMethod("public static far"); break;
            case 'U': sig = ParseInstanceMethod("public virtual"); break;
            case 'V': sig = ParseInstanceMethod("public virtual far"); break;

            case 'Y': sig = ParseGlobalFunction( ""); break;
            case 'Z': sig = ParseGlobalFunction( "far"); break;
            default: throw new NotImplementedException(string.Format("Character '{0}' not supported", str[i - 1]));

            }
            return (
                basicName,
                sig,
                sig is not null 
                    ? sig.EnclosingType
                    : CreateEnclosingType(Scope));
        }

        public (string name, SerializedType?, SerializedType?) ParseUnqualifiedTypeCode(string basicName)
        {
            this.compoundArgs = new List<Argument_v1>();
            if (PeekAndDiscard('Y'))
            {
                return (
                    basicName,
                    ParseFunctionTypeCode(),
                    null);
            }
            else
            {
                Expect('3');
                return (
                    basicName,
                    ParseDataTypeCode(new List<Argument_v1>()),
                    null);
            }
        }

        public SerializedSignature ParseInstanceMethod(string modifier)
        {
            this.Modifier = modifier;
            this.isInstanceMethod = true;
            ParseThisStorageClass();
            return ParseFunctionTypeCode();
        }

        public SerializedSignature ParseGlobalFunction(string modifier)
        {
            this.Modifier = modifier;
            var convention = ParseCallingConvention();
            var returnStorageClass = ParseReturnQualifier();
            var retType = ParseDataTypeCode(new List<Argument_v1>());
            var args = ParseArgumentList();
            return new SerializedSignature
            {
                Convention = convention,
                IsInstanceMethod = this.isInstanceMethod,
                EnclosingType = null,
                Arguments = args,
                ReturnValue = new Argument_v1 { Type = retType ?? new VoidType_v1() }
            };
        }

        public Qualifier ParseReturnQualifier()
        {
            if (PeekAndDiscard('?'))
            {
                switch (str[i++])
                {
                case 'A': return Qualifier.None;
                case 'B': return Qualifier.Const;
                case 'C': return Qualifier.Volatile;
                case 'D': return Qualifier.Const | Qualifier.Volatile;
                default: throw new FormatException($"Unexpected return qualifier {str[i - 1]}.");
                }
            }
            return Qualifier.None;
        }

        public SerializedSignature ParseStaticMethod(string modifier)
        {
            this.isInstanceMethod = false;
            return ParseFunctionTypeCode();
        }

        public string? ParseStorageClass()
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

        public string? ParseThisStorageClass()
        {
            if (PeekAndDiscard('E'))    // 64-bit ptr
            {
                // do nothing?
            }
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
            string? convention = ParseCallingConvention();
            SerializedType? retType;
            if (PeekAndDiscard('@'))
            {
                // C++ ctors have no explicit return type!
                if (isConstuctor)
                {
                    // C++ constructor implicitly returns pointer to
                    // initialised object
                    retType = new PointerType_v1
                    {
                        DataType = CreateEnclosingType(Scope),
                        PointerSize = pointerSize,
                    };
                }
                else
                {
                    retType = null;
                }
            }
            else
            {
                retType = ParseDataTypeCode(new List<Argument_v1>());
            }
            Argument_v1[] args = ParseArgumentList();
            return new SerializedSignature
            {
                Convention = convention,
                Arguments = args,
                EnclosingType = CreateEnclosingType(Scope),
                IsInstanceMethod = this.isInstanceMethod,
                ReturnValue = new Argument_v1 { Type = retType ?? new VoidType_v1() }
            };
        }

        private StructType_v1? CreateEnclosingType(string? scope)
        {
            return !string.IsNullOrEmpty(Scope)
                ? new StructType_v1 { Name = scope, ForceStructure = true }
                : null;
        }

        public MemberPointer_v1 ParseMemberFunctionPointerCode(int byteSize, List<Argument_v1> compoundArgs)
        {
            var className = ParseStructure(compoundArgs);
            var storageClass = ParseThisStorageClass();
            var callConv = ParseCallingConvention();
            var retType = ParseDataTypeCode(compoundArgs);
            var args = ParseArgumentList();
            return new MemberPointer_v1
            {
                DeclaringClass = className,
                Size = byteSize,
                MemberType = new SerializedSignature
                {
                    Convention = callConv,
                    Arguments = args,
                    ReturnValue = new Argument_v1 { Type = retType ?? new VoidType_v1() }
                }
            };
        }

        public string? ParseOperatorCode()
        {
            switch (str[i++])
            {
            case '0':
                this.isConstuctor = true;
                return "{0}";
            case '1': return "~{0}";
            case '2': return "operator new";
            case '3': return "operator delete";
            case '4': return "operator =";
            case '5': return "operator >>";
            case '6': return "operator <<";
            case '8': return "operator ==";
            case '9': return "operator !=";
            case 'A': return "operator []";
            case 'B': return "operator returntype";
            case 'C': return "operator ->";
            case 'D': return "operator *";
            case 'E': return "operator ++";
            case 'F': return "operator --";
            case 'G': return "operator -";
            case 'H': return "operator +";
            case 'I': return "operator &";
            case 'J': return "operator ->*";
            case 'K': return "operator /";
            case 'L': return "operator %";
            case 'M': return "operator <";
            case 'N': return "operator <=";
            case 'O': return "operator >";
            case 'P': return "operator >=";
            case 'Q': return "operator,";
            case 'R': return "operator ()";
            case 'S': return "operator ~";
            case 'T': return "operator ^";
            case 'U': return "operator |";
            case 'V': return "operator &&";
            case 'W': return "operator ||";
            case 'X': return "operator *=";
            case 'Y': return "operator +=";
            case 'Z': return "operator -=";
            case '_':
                switch (str[i++])
                {
                case '0': return "operator /=";
                case '1': return "operator %="; 
                case '2': return "operator >>=";
                case '3': return "operator <<=";
                case '4': return "operator &=";
                case '5': return "operator |=";
                case '6': return "operator ^=";
                case '7': return "`vftable'"; 
                case '8': return "`vbtable'";
                case '9': return "`vcall'";
                case 'A': return "typeof";
                case 'B': return "`local static guard'";
                case 'D': return "`vbase destructor'";
                case 'E': return "`vector deleting destructor'";
                case 'F': return "`default constructor closure'";
                case 'G': return "`scalar deleting destructor'";
                case 'H': return "`vector constructor iterator'";
                case 'I': return "`vector destructor iterator'";
                case 'J': return "`vector vbase constructor iterator'";
                case 'K': return "`virtual displacement map'";
                case 'L': return "`eh vector constructor iterator'";
                case 'M': return "`eh vector destructor iterator'";
                case 'N': return "`eh vector vbase constructor iterator'";
                case 'O': return "`copy constructor closure'";
                case 'P': return "`udt returning'";
                case 'R': throw new NotSupportedException("RTTI Codes not supported yet");
                case 'S': return "`local vftable'";
                case 'T': return "`local vftable constructor closure'";
                case 'U': return "operator new[]";
                case 'V': return "operator delete[]";
                default: Error("Unknown operator code '_{0}'.", str[i - 1]);
                    return null;
                }
            default: Error("Unknown operator code '{0}'.", str[i - 1]);
                return null;
            }
        }

        public string? ParseCallingConvention()
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
        
        public Argument_v1[] ParseArgumentList()
        {
            var args = new List<Argument_v1>();
            if (!PeekAndDiscard('X'))    // Empty arg list
            {
                while (!PeekAndDiscard('@'))
                {
                    if (PeekAndDiscard('Z'))    // Ellipses ('...')
                    {
                        args.Add(new Argument_v1 { Name="...", Type=new VoidType_v1() });
                        break;      // Ellipses can only be the last arg, so arglist is done!
                    }
                    var arg = ParseDataTypeCode(this.compoundArgs);
                    args.Add(new Argument_v1 { Type = arg });
                }
            }
            Expect('Z');
            return args.ToArray();
        }

        public SerializedType? ParseDataTypeCode(List<Argument_v1> compoundArgs)
        {
            if (PeekAndDiscard('?'))
            {
                switch (str[i++])
                {
                case 'A': break;
                case 'B': /* const */ break;
                default: Error("Expected 'A' or 'B', but saw '{0}'.", str[i - 1]); break;
                }
            }
            switch (str[i++])
            {
            case '0': return compoundArgs[0].Type;
            case '1': return compoundArgs[1].Type;
            case '2': return compoundArgs[2].Type;
            case '3': return compoundArgs[3].Type;
            case '4': return compoundArgs[4].Type;
            case '5': return compoundArgs[5].Type;
            case '6': return compoundArgs[6].Type;
            case '7': return compoundArgs[7].Type;
            case '8': return compoundArgs[8].Type;
            case '9': return compoundArgs[9].Type;
            case 'A': return ParsePointer(compoundArgs, Qualifier.None);        //$TODO: really is a lvalue reference but is implemented as a pointer on Win32...
            case 'B': return ParsePointer(compoundArgs, Qualifier.Volatile);    //$TODO: really is a volatile lvalue reference but is implemented as a pointer on Win32...
            case 'C': return new PrimitiveType_v1(Domain.Character | Domain.SignedInt, 1);
            case 'D': return new PrimitiveType_v1(Domain.Character, 1);
            case 'E': return new PrimitiveType_v1(Domain.Character | Domain.UnsignedInt, 1);
            case 'F': return new PrimitiveType_v1(Domain.SignedInt, 2);
            case 'G': return new PrimitiveType_v1(Domain.UnsignedInt, 2);
            case 'H': return new PrimitiveType_v1(Domain.SignedInt, 4);
            case 'I': return new PrimitiveType_v1(Domain.UnsignedInt, 4);
            case 'J': return new PrimitiveType_v1(Domain.SignedInt, 4);      // 'long' on Win32 is actually 4 bytes
            case 'K': return new PrimitiveType_v1(Domain.UnsignedInt, 4);  // 'long' on Win32 is actually 4 bytes
            case 'M': return new PrimitiveType_v1(Domain.Real, 4);
            case 'N': return new PrimitiveType_v1(Domain.Real, 8);
            case 'O': return new PrimitiveType_v1(Domain.Real, 10);
            case 'P': return ParsePointer(compoundArgs, Qualifier.None);    // pointer
            case 'Q': return ParsePointer(compoundArgs, Qualifier.Const);    // const pointer
            case 'R': return ParsePointer(compoundArgs, Qualifier.Volatile);    // volatile pointer
            case 'T': return ParseStructure(compoundArgs);  // union 
            case 'U': return ParseStructure(compoundArgs); // struct (see below)
            case 'V': return ParseStructure(compoundArgs); // class (see below)
            case 'W': return ParseEnum(compoundArgs);
            case 'X': return new VoidType_v1();      // void (as in 'void return value', 'X' terminates argument list)
            case 'Y': return ParseStructure(compoundArgs); // cointerface (see below)
            case '_':
                PrimitiveType_v1 prim;
                switch (str[i++])
                {
                case 'J': prim = new PrimitiveType_v1(Domain.SignedInt, 8); break;   // __int64
                case 'K': prim = new PrimitiveType_v1(Domain.UnsignedInt, 8); break; // unsigned __int64
                case 'N': prim = new PrimitiveType_v1(Domain.Boolean, 1); break;     // bool
                case 'W': prim = new PrimitiveType_v1(Domain.Character, 2); break;   // wchar_t
                default: Error("Unsupported type code '_{0}'.", str[i - 1]); return null;
                }
                compoundArgs.Add(new Argument_v1 { Type = prim });
                return prim;
            case '$':
                switch (str[i++])
                {
                case '$':
                    switch (str[i++])
                    {
                    case 'Q': return ParsePointer(compoundArgs, Qualifier.None); //$ rvalue reference
                    }
                    Error("Unsupported type code '$${0}'.", str[i - 1]); return null;
                default:
                    Error("Unsupported type code '$${0}'.", str[i - 1]); return null;
                }
            default:
                Error("Unsupported type code '{0}'.", str[i - 1]);
                return null;
            }
        }

        public SerializedType? ParsePointer(List<Argument_v1> compoundArgs, Qualifier q)
        {
            int size = pointerSize;
            SerializedType? type;
            if (PeekAndDiscard('E')) // 64-bit pointer
            {
                size = 8;
            }
            switch (str[i++])
            {
            case 'A': type = ParseDataTypeCode(new List<Argument_v1>()); break;       //$BUG: assumes 32-bitness
            case 'B': type = Qualify(ParseDataTypeCode(new List<Argument_v1>()), Qualifier.Const); break;       // const ptr
            case 'C': type = Qualify(ParseDataTypeCode(new List<Argument_v1>()), Qualifier.Volatile); break;       // volatile ptr
            case 'D': type = Qualify(ParseDataTypeCode(new List<Argument_v1>()), Qualifier.Const|Qualifier.Volatile); break;       // const volatile ptr
            case '6': type = ParseFunctionTypeCode(); break;     // fn ptr
            case '8': return ParseMemberFunctionPointerCode(size, compoundArgs);
            default: Error("Unsupported pointer code 'P{0}'.", str[i - 1]); return null;
            }
            SerializedType pType = new PointerType_v1
            {
                DataType = type,
                PointerSize = size,
                Qualifier = q,
            };
            compoundArgs.Add(new Argument_v1 { Type = pType });
            return pType;
        }

        private SerializedType? Qualify(SerializedType? t, Qualifier q)
        {
            if (t is null)
                return null;
            t.Qualifier = q;
            return t;
        }

        public TypeReference_v1 ParseStructure(List<Argument_v1> compoundArgs)
        {
            var q = ParseQualification();
            var tr = new TypeReference_v1
            {
                TypeName = q.Last(),
                Scope = q.Take(q.Length - 1).ToArray()
            };
            compoundArgs.Add(new Argument_v1 { Type = tr });
            return tr;
        }

        public SerializedType? ParseEnum(List<Argument_v1> compoundArgs)
        {
            int size;
            Domain domain;
            switch (str[i++])
            {
            case '0': size = 1; domain = Domain.Character; break;
            case '1': size = 1; domain = Domain.Character|Domain.UnsignedInt; break;
            case '2': size = 2; domain = Domain.SignedInt; break;
            case '3': size = 2; domain = Domain.UnsignedInt; break;
            case '4': size = 4; domain = Domain.SignedInt; break;
            case '5': size = 4; domain = Domain.UnsignedInt; break;
            case '6': size = 4; domain = Domain.SignedInt; break;
            case '7': size = 4; domain = Domain.UnsignedInt; break;
            default: Error("Unknown enum code {0}.", str[i - 1]); return null;
            }
            var n = ParseQualification();
            var e = new SerializedEnumType(size, domain, n.Last());
            compoundArgs.Add(new Argument_v1 { Type = e });
            return e;
        }

        internal string ParseAtName()
        {
            int iStart = i;
            while (i < str.Length && str[i] != '@')
            {
                if (i == iStart && Char.IsDigit(str[i]))
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
