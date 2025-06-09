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
using System.Text;

namespace Reko.Environments.SysV
{
    // https://mentorembedded.github.io/cxx-abi/abi.html#mangling-type
    public class GccMangledNameParser
    {
        private readonly string str;
        private readonly int ptrSize;
        private readonly Dictionary<string, object> substitutions;
        private int i;

        public GccMangledNameParser(string parse, int ptrSize)
        {
            this.str = parse;
            this.ptrSize = ptrSize;
            this.substitutions = new Dictionary<string, object>();
        }

        public string? Modifier { get; set; }
        public string? Scope { get; set; }

        public StructField_v1? Parse()
        {
            this.i = 0;
            if (!PeekAndDiscard('_'))
                return null;
            if (!PeekAndDiscard('Z'))
                return null;

            if (PeekAndDiscard('L'))
            {
                //$TODO: local name => in C, this is 'static'
            }
            else if (Peek('N'))
            {
                var qname = NestedName();
                SerializedType? type = null;
                if (i < str.Length)
                {
                    type = new SerializedSignature
                    {
                        Arguments = Arguments()
                    };
                }
                return new StructField_v1
                {
                    Name = string.Join("::", qname),
                    Type = type,
                };
            }
            var name = UnscopedName();
            var args = Arguments();
            return new StructField_v1
            {
                Name = name,
                Type = new SerializedSignature
                {
                    Arguments = args
                }
            };
        }

        private List<object> NestedName()
        {
            Expect('N');
            CvQualifier();
            var items = Prefix();
            var name = UnqualifiedName();
            name = string.Format(name, items.Count > 0 ? items.LastOrDefault() : "");
            items!.Add(name);
            Expect('E');
            return items;
        }

        private List<object> Prefix()
        {
            var prefixes = new List<object>();
            if (Peek('S'))
            {
                prefixes.AddRange(Substitution());
                return prefixes;
            }

            while (true)
            {
                char c = str[i];
                if (Char.IsDigit(str[i]))
                {
                    var n = SourceName();
                    if (Peek('I'))
                    {
                        // n is the name of a template.
                        AddSubstitution(n);
                        var args = TemplateArgs();
                        var tmplate = new SerializedTemplate(Array.Empty<string>(), n, args);
                        AddSubstitution(tmplate);
                        prefixes.Add(tmplate);
                        return prefixes;
                    }
                    else if (Peek('D') || Peek('C'))
                    {
                        prefixes.Add(n);
                        return prefixes;
                    }
                    else
                    {
                        AddSubstitution(new TypeReference_v1 { TypeName = n });
                    }
                }
                else
                    break;
            }
            return prefixes;
        }

        private void AddSubstitution(object n)
        {
            int c = substitutions.Count;
            if (c == 0)
            {
                substitutions.Add("_", n);
            }
            else if (c <= 10)
            {
                substitutions.Add(string.Format("{0}_", (char)('0' + c - 1)), n);
            }
            else if (c <= 36)
            {
                substitutions.Add(string.Format("{0}_", (char)('A' + c - 11)), n);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        private string SourceName()
        {
            int len = 0;
            while (char.IsDigit(str[i]))
            {
                len = len * 10 + (str[i] - '0');
                ++i;
            }
            var n = str.Substring(i, len);
            i += len;
            return n;
        }

        private SerializedType[] TemplateArgs()
        {
            Expect('I');
            var args = new List<SerializedType>();
            do
            {
                var type = Type();
                args.Add(type);
            } while (!PeekAndDiscard('E'));
            return args.ToArray();
        }

        private List<string> Substitution()
        {
            Expect('S');
            switch (str[i])
            {
            case 'o':
                ++i;
                return new List<string>
                {
                    "std", "ostream"
                };
            case 's':
                ++i;
                return new List<string>
                {
                    "std", "string"
                };
            default:
                int iStart = i;
                while (str[i] != '_')
                    ++i;
                ++i;
                var sub = str.Substring(iStart, i - iStart);
                return new List<string> { substitutions[sub].ToString()! };

            }
            throw new NotImplementedException();
        }

        private string UnscopedName()
        {
            if (i < str.Length - 1 && str[i] == 'S' && str[i + 1] == 't')
            {
                i += 2;
                return "std::" + UnqualifiedName();
            }
            return UnqualifiedName();
        }

        private string UnqualifiedName()
        {
            switch (str[i])
            {
            case 'l':
                if (str[i + 1] == 's')
                {
                    i += 2;
                    return "operator<<";
                }
                throw new NotImplementedException();
            case 'C':
                // C1	# complete object constructor
                // C2	# base object constructor
                // C3	# complete object allocating constructor
                i += 2;
                return "{0}";
            case 'D':
                // D0	# deleting destructor
                // D1	# complete object destructor
                // D2	# base object destructor
                i += 2;
                return "~{0}";
            }
            int n = NameLength();
            var name = str.Substring(i, n);
            i += n;
            return name;
        }

        private int NameLength()
        {
            int n = 0;
            for (;;)
            {
                var ch = str[i++];
                if ('0' <= ch && ch <= '9')
                {
                    n = n * 10 + (ch - '0');
                }
                else
                {
                    --i;
                    return n;
                }
            }
        }

        private void Error(string format, params object[] args)
        {
            throw new FormatException(string.Format(format, args));
        }

        private void Expect(char ch)
        {
            if (str[i++] != ch)
                Error("Expected '{0}' but found '{1}' ({2}).", ch, str[i - 1], str.Substring(i-1));
        }

        private bool Peek(char ch)
        {
            return str[i] == ch;
        }

        private bool PeekAndDiscard(char ch)
        {
            if (i < str.Length && str[i] == ch)
            {
                ++i;
                return true;
            }
            else
            {
                return false;
            }
        }

        private Argument_v1[] Arguments()
        {
            var args = new List<Argument_v1>();
            while (i < str.Length)
            {
                var arg = Argument();
                if (arg is null)
                    break;
                args.Add(arg);
            }
            return args.ToArray();
        }

        private Argument_v1? Argument()
        {
            switch (str[i])
            {
            case 'z': ++i; return new Argument_v1 { Name = "...", Type = new VoidType_v1() };
            case 'v': ++i; return null;
            default: return new Argument_v1 { Type = Type() };
            }
        }

        private SerializedType Type()
        {
            var qual = CvQualifier();
            switch (str[i++])
            {
            case 'v': return new VoidType_v1();
            case 'b': return Qualify(PrimitiveType_v1.Bool(), qual);
            case 'c': return Qualify(PrimitiveType_v1.Char8(), qual);
            case 'h': return Qualify(PrimitiveType_v1.UChar8(), qual);
            case 's': return Qualify(PrimitiveType_v1.Int16(), qual);
            case 't': return Qualify(PrimitiveType_v1.UInt16(), qual);
            case 'i': return Qualify(PrimitiveType_v1.Int32(), qual);
            case 'j': return Qualify(PrimitiveType_v1.UInt32(), qual);
            case 'l': return Qualify(PrimitiveType_v1.Int64(), qual);
            case 'm': return Qualify(PrimitiveType_v1.UInt64(), qual);
            case 'w': return Qualify(PrimitiveType_v1.WChar16(), qual);
            case 'f': return Qualify(PrimitiveType_v1.Real32(), qual);
            case 'd': return Qualify(PrimitiveType_v1.Real64(), qual);
            case 'F': --i; return FunctionType(); 
            case 'N': --i; return CreateTypeReference(NestedName()); 
            case 'P':
                var ptr = new PointerType_v1 { DataType = Type(), PointerSize = ptrSize };
                AddSubstitution(ptr);
                return Qualify(ptr, qual);
            case 'R':
                var referent = Type();
                var r = new ReferenceType_v1 { Referent = referent, Size = ptrSize };
                return Qualify(r, qual);
            case 'S':
                switch (str[i++])
                {
                case 't':
                    return Qualify(new TypeReference_v1
                    {
                        Scope = new[] { "std" },
                        TypeName = Type().ToString(),
                    }, qual);
                case 's':
                    return Qualify(new TypeReference_v1
                    {
                        Scope = new[] { "std" },
                        TypeName = "string"
                    }, qual);
                default:
                    int iStart = --i;
                    while (str[i] != '_')
                        ++i;
                    ++i;
                    var sub = str.Substring(iStart, i - iStart);
                    Debug.Print(sub);
                    return Qualify((SerializedType)substitutions[sub], qual);
                }
                throw new NotImplementedException();
            default:
                --i;
                if (char.IsDigit(str[i]))
                {
                    var tref = new TypeReference_v1
                    {
                        TypeName = UnqualifiedName()
                    };
                    AddSubstitution(tref);
                    return Qualify(tref, qual);
                }
                throw new NotImplementedException(string.Format("Unknown GCC type code '{0}' ({1}).", str[i], str.Substring(i)));
            }
        }

        private SerializedType Qualify(SerializedType t, Qualifier q)
        {
            t.Qualifier = q;
            return t;
        }

        /*
* <function-type> ::= [<CV-qualifiers>] [Dx] F [Y] <bare-function-type> [<ref-qualifier>] E
* <bare-function-type> ::= <signature type>+
*/
        private SerializedSignature FunctionType()
        {
            Expect('F');
            var t = BareFunctionType();
            Expect('E');
            return t;
        }

        private SerializedSignature BareFunctionType()
        {
            var list = new List<Argument_v1>();
            do
            {
                var tt = Type();
                list.Add(new Argument_v1 { Type = tt });
            } while (str[i] != 'E');
            return new SerializedSignature
            {
                Arguments = list
                    .Skip(1)
                    .ToArray(),
                ReturnValue = list.First()
            };
        }

        private SerializedType CreateTypeReference(List<object> list)
        {
            return new TypeReference_v1
            {
                TypeName = string.Join("::", list.Select(n => n.ToString()))
            };
        }

        private Qualifier CvQualifier()
        {
            char c = str[i];
            switch (str[i])
            {
            case 'K': ++i; return Qualifier.Const;
            case 'V': ++i; return Qualifier.Volatile;
            case 'r': ++i; return Qualifier.Restricted;
            default: return Qualifier.None;
            }
        }
    }

#if FUTURE

    public class GccGrammar
    {
        private string str;
        private int i;

        //<mangled-name> ::= _Z<encoding>
        //<encoding> ::= <function name> <bare-function-type>
        //    ::= <data name>
        //    ::= <special-name>
        public void MangledName()
        {
            Expect("_Z");
            if (FIRST_function_name())
            {
                function_name();
                bare_function_type();
            } else if (FIRST_data_name())
            {
                data_name();
            }
            else if (FIRST_Sspecial_name())
            {
                special_name();
            }
        }


        // <name> ::= <nested-name>
	    //        ::= <unscoped-name>
	    //        ::= <unscoped-template-name> <template-args>
	    //        ::= <local-name>	
        public void name()
        {
            if (FIRST_nested_name())
            {
                nested_name();
            } else if (FIRST_unscoped_name())
            {
                unscoped_name();
            } else if (FIRST_unscoped_template_name())
            {
                unscoped_template_name();
                template_args();
            }else
            {
                local_name();
            }
        }

    // <nested-name> ::= N[< CV-qualifiers >][<ref-qualifier>] <prefix> <unqualified-name> E
    //               ::= N[< CV-qualifiers >][<ref-qualifier>] <template-prefix> <template-args> E

    // <prefix> ::= <unqualified-name>                 # global class or namespace
    //          ::= <prefix> <unqualified-name>        # nested class or namespace
    //          ::= <template-prefix> <template-args>  # class template specialization
    //          ::= <template-param>                   # template type parameter
    //          ::= <decltype>                         # decltype qualifier
    //          ::= <prefix> <data-member-prefix>      # initializer of a data member
    //          ::= <substitution>

    //<template-prefix> ::= <template unqualified-name>           # global template
    //                  ::= <prefix> <template unqualified-name>  # nested template
    //                  ::= <template-param>                      # template template parameter
    //                  ::= <substitution>

    //<unqualified-name> ::= <operator-name>
    //                   ::= <ctor-dtor-name>  
    //                   ::= <source-name>   
    //                   ::= <unnamed-type-name>   
    //                   ::= DC<source-name>+ E      # structured binding declaration

    // <source-name> ::= <positive length number> <identifier>
    // <identifier> ::= <unqualified source code identifier>


        public void nested_name()
        {
            Expect("N");
            if (CV_qualifiers_FIRST())
            {
                CV_qualifiers();
            }
            if (ref_qualifier_FIRST())
            {
                ref_qualifier();
            }
            if (template_prefix_FIRST())
            {
                teplate_prefix();
                template_args();
            }
            prefix();
            unqualified_name();
            Expect("E");
        }


        public void prefix()
        {
            for (;;)
            {
                if (unqualified_name_FIRST())
                {
                    unqualified_name();
                }
                else if (template_prefix_FIRST())
                {
                    template_prefix();
                    template_args();
                }
                else if (template_param_FIRST())
                {
                    template_param();
                }
                else if (decltype_FIRST())
                {
                    decltype();
                }
                else if (data_member_prefix_FIRST())
                {
                    data_member_prefix();
                }
                else if (substitution_FIRST())
                {
                    substitution();
                }
                else
                    return;
            }
        }

        public void unqualified_name()
        {
            if (operator_name_FIRST())
            {
                operator_name();
            }
            else if (ctor_dtor_name_FIRST())
            {
                ctor_dtor_name();
            }
            else if (char.IsDigit(str[i]))
            {
                source_name();
            } else if (Peek("Ut"))
            {
                unnamed_type_name();
            } else if (Peek("DC"))
            {
                do
                {
                    source_name();

                } while (!Peek("E"));
            }
        }

        private void unnamed_type_name()
        {
            Expect("Ut");
            number();
        }

        private bool Peek(string s)
        {
            throw new NotImplementedException();
        }

        private void source_name()
        {
            var n = number();
            identifier(n);
        }
    }

#endif
}

