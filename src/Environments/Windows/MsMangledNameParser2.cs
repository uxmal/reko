/*
 *  Demangle VC++ symbols into C function prototypes
 *
 *  Copyright 2000 Jon Griffiths
 *            2004 Eric Pouech
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301, USA
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.Environments.Windows
{
    class MsMangledNameParser2
    {
        /* TODO:
         * - document a bit (grammar + functions)
         * - back-port this new code into tools/winedump/msmangle.c
         */

        /* How data types modifiers are stored:
         * M (in the following definitions) is defined for 
         * 'A', 'B', 'C' and 'D' as follows
         *      {<A>}:  ""
         *      {<B>}:  "const "
         *      {<C>}:  "volatile "
         *      {<D>}:  "const volatile "
         *
         *      in arguments:
         *              P<M>x   {<M>}x*
         *              Q<M>x   {<M>}x* const
         *              A<M>x   {<M>}x&
         *      in data fields:
         *              same as for arguments and also the following
         *              ?<M>x   {<M>}x
         *              
         */

        public class array
        {
            public uint start;          /* first valid reference in array */
            public uint num;            /* total number of used elts */
            public uint max;
            public uint alloc;
            public List<string> elts;
        };

        /* Structure holding a parsed symbol */
        public partial class parsed_symbol
        {

            uint flags;          /* the UNDNAME_ flags used for demangling */
            //malloc_func_t mem_alloc_ptr;  /* internal allocator */
            //free_func_t mem_free_ptr;   /* internal deallocator */

            public string mangled;
            public int current;        /* pointer in input (mangled) string */
            public string result;         /* demangled string */

            List<string> names;          /* array of names for back reference */
            List<string> stack;          /* stack of parsed strings */

            //object alloc_list;      /* linked list of allocated blocks */
            //uint avail_in_first;    /* number of available bytes in head block */
        };

        /* Type for parsing mangled types */
        public class datatype_t
        {
            public string left;
            public string right;
        };


        public partial class parsed_symbol
        {
            /******************************************************************
             *		und_alloc
             *
             * Internal allocator. Uses a simple linked list of large blocks
             * where we use a poor-man allocator. It's fast, and since all
             * allocation is pool, memory management is easy (esp. freeing).
             */
            static void und_alloc(parsed_symbol sym, uint len)
            {
                //    void* ptr;

                //            const int BLOCK_SIZE = 1024;
                ////#define AVAIL_SIZE      (1024 - sizeof(void*))

                //    if (len > AVAIL_SIZE)
                //    {
                //        /* allocate a specific block */
                //        ptr = sym.mem_alloc_ptr(sizeof(void*) + len);
                //        if (!ptr) return NULL;
                //        *(void**)ptr = sym.alloc_list;
                //        sym.alloc_list = ptr;
                //        sym.avail_in_first = 0;
                //        ptr = (char*)sym.alloc_list + sizeof(void*);
                //    }
                //    else 
                //    {
                //        if (len > sym.avail_in_first)
                //        {
                //            /* add a new block */
                //            ptr = sym.mem_alloc_ptr(BLOCK_SIZE);
                //            if (!ptr) return NULL;
                //            *(void**)ptr = sym.alloc_list;
                //            sym.alloc_list = ptr;
                //            sym.avail_in_first = AVAIL_SIZE;
                //        }
                ///* grab memory from head block */
                //ptr = (char*)sym.alloc_list + BLOCK_SIZE - sym.avail_in_first;
                //        sym.avail_in_first -= len;
                //    }
                //    return ptr;
                //#undef BLOCK_SIZE
                //#undef AVAIL_SIZE
            }

            /******************************************************************
             *		und_free
             * Frees all the blocks in the list of large blocks allocated by
             * und_alloc.
             */
            static void und_free_all(parsed_symbol sym)
            {
                //    void* next;

                //    while (sym.alloc_list)
                //    {
                //        next = *(void**)sym.alloc_list;
                //        if(sym.mem_free_ptr) sym.mem_free_ptr(sym.alloc_list);
                //sym.alloc_list = next;
                //    }
                //    sym.avail_in_first = 0;
            }

            /******************************************************************
             *		str_array_init
             * Initialises an array of strings
             */
            static void str_array_init(array a)
            {
                a.start = a.num = a.max = a.alloc = 0;
                a.elts = null;
            }

            /******************************************************************
             *		str_array_push
             * Adding a new string to an array
             */
            static bool str_array_push(parsed_symbol sym, string ptr, int len,
                                       array a)
            {
                //    char** _new;

                //    assert(ptr);
                //    assert(a);

                //    if (!a.alloc)
                //    {
                //        new = und_alloc(sym, (a.alloc = 32) * sizeof(a.elts[0]));
                //        if (!new) return FALSE;
                //        a.elts = new;
                //    }
                //    else if (a.max >= a.alloc)
                //    {
                //        new = und_alloc(sym, (a.alloc* 2) * sizeof(a.elts[0]));
                //        if (!new) return FALSE;
                //        memcpy(new, a.elts, a.alloc* sizeof(a.elts[0]));
                //        a.alloc *= 2;
                //        a.elts = new;
                //    }
                //    if (len == -1) len = strlen(ptr);
                //a.elts[a.num] = und_alloc(sym, len + 1);
                //    assert(a.elts[a.num]);
                //    memcpy(a.elts[a.num], ptr, len);
                //a.elts[a.num][len] = '\0'; 
                //    if (++a.num >= a.max) a.max = a.num;
                //    {
                //        int i;
                //char c;

                //        for (i = a.max - 1; i >= 0; i--)
                //        {
                //            c = '>';
                //            if (i<a.start) c = '-';
                //            else if (i >= a.num) c = '}';
                //            TRACE("%p\t%d%c %s\n", a, i, c, a.elts[i]);
                //        }
                //    }

                //    return TRUE;
                throw new NotImplementedException();
            }

            /******************************************************************
             *		str_array_get_ref
             * Extracts a reference from an existing array (doing proper type
             * checking)
             */
            static string str_array_get_ref(array cref, uint idx)
            {
                //assert(cref);
                //if (cref.start + idx >= cref.max)
                //{
                //    WARN("Out of bounds: %p %d + %d >= %d\n",
                //          cref, cref.start, idx, cref.max);
                //    return NULL;
                //}
                //TRACE("Returning %p[%d] => %s\n",
                //      cref, idx, cref.elts[cref.start + idx]);
                //return cref.elts[cref.start + idx];
                throw new NotImplementedException();
            }

            /******************************************************************
             *		str_printf
             * Helper for printf type of command (only %s and %c are implemented) 
             * while dynamically allocating the buffer
             */
            static string str_printf(parsed_symbol sym, string format, params object[] args)
            {
                //    va_list args;
                //unsigned int len = 1, i, sz;
                //char* tmp;
                //char* p;
                //char* t;

                //    va_start(args, format);
                //    for (i = 0; format[i]; i++)
                //    {
                //        if (format[i] == '%')
                //        {
                //            switch (format[++i])
                //            {
                //            case 's': t = va_arg(args, char*); if (t) len += strlen(t); break;
                //            case 'c': (void)va_arg(args, int); len++; break;
                //            default: i--; /* fall through */
                //            case '%': len++; break;
                //            }
                //        }
                //        else len++;
                //    }
                //    va_end(args);
                //    if (!(tmp = und_alloc(sym, len))) return NULL;
                //    va_start(args, format);
                //    for (p = tmp, i = 0; format[i]; i++)
                //    {
                //        if (format[i] == '%')
                //        {
                //            switch (format[++i])
                //            {
                //            case 's':
                //                t = va_arg(args, char*);
                //                if (t)
                //                {
                //                    sz = strlen(t);
                //                    memcpy(p, t, sz);
                //p += sz;
                //                }
                //                break;
                //            case 'c':
                //                * p++ = (char)va_arg(args, int);
                //                break;
                //            default: i--; /* fall through */
                //            case '%': * p++ = '%'; break;
                //            }
                //        }
                //        else * p++ = format[i];
                //    }
                //    va_end(args);
                //    * p = '\0';
                //    return tmp;
                throw new NotImplementedException();
            }

            /* forward declaration */
            //static bool demangle_datatype(parsed_symbol sym,  datatype_t* ct,
            //                              struct array* pmt, BOOL in_args);

            static string get_number(parsed_symbol sym)
            {
                string ptr;
                bool sgn = false;

                if (sym.mangled[sym.current] == '?')
                {
                    sgn = true;
                    sym.current++;
                }
                if (sym.mangled[sym.current] >= '0' && sym.mangled[sym.current] <= '8')
                {
                    ptr = string.Format("{0}{1}", sgn ? "-" : "", (char)(sym.current + 1));
                    sym.current++;
                }
                else if (sym.mangled[sym.current] == '9')
                {
                    ptr = string.Format("{0}10", sgn ? "-" : "");
                    sym.current++;
                }
                else if (sym.mangled[sym.current] >= 'A' && sym.mangled[sym.current] <= 'P')
                {
                    int ret = 0;

                    while (sym.mangled[sym.current] >= 'A' && sym.mangled[sym.current] <= 'P')
                    {
                        ret *= 16;
                        ret += sym.mangled[sym.current++] - 'A';
                    }
                    if (sym.mangled[sym.current] != '@')
                        return null;
                    ptr = string.Format("{0}{1}", sgn ? "-" : "", ret);
                    sym.current++;
                }
                else return null;
                return ptr;
            }

            /******************************************************************
             *		get_args
             * Parses a list of function/method arguments, creates a string corresponding
             * to the arguments' list.
             */
            static string get_args(parsed_symbol sym, List<string> pmt_ref, bool z_term,
                                  char open_char, char close_char)

            {
                datatype_t ct;
                List<string> arg_collect;
                string args_str = null;
                string last;

                arg_collect = new List<string>();

                /* Now come the function arguments */
                while (sym.current < sym.mangled.Length)
                {
                    /* Decode each data type and append it to the argument list */
                    if (sym.mangled[sym.current] == '@')
                    {
                        sym.current++;
                        break;
                    }
                    ct = new datatype_t();
                    if (!demangle_datatype(sym, ct, pmt_ref, true))
                        return null;
                    /* 'void' terminates an argument list in a function */
                    if (z_term && ct.left == "void") break;
                    arg_collect.Add(string.Format("{0}{1}", ct.left, ct.right));
                    if (ct.left == "...") break;
                }
                /* Functions are always terminated by 'Z'. If we made it this far and
                 * don't find it, we have incorrectly identified a data type.
                 */
                if (z_term && sym.mangled[sym.current++] != 'Z') return null;

                if (arg_collect.Count == 0 ||
                    (arg_collect.Count == 1 && arg_collect[0] == "void"))
                    return string.Format("{0}void{1}", open_char, close_char);
                for (int i = 1; i < arg_collect.Count; i++)
                {
                    args_str = str_printf(sym, "{0},{1}", args_str, arg_collect[i]);
                }

                last = args_str != null ? args_str : arg_collect[0];
                if (close_char == '>' && last[last.Length - 1] == '>')
                    args_str = str_printf(sym, "{0}{1}{2} {3}",
                                          open_char, arg_collect[0], args_str, close_char);
                else
                    args_str = str_printf(sym, "{0}{1}{2}{3}",
                                          open_char, arg_collect[0], args_str, close_char);

                return args_str;
            }

            /******************************************************************
             *		get_modifier
             * Parses the type modifier. Always returns static strings.
             */
            static bool get_modifier(parsed_symbol sym, out string ret, out string ptr_modif)
            {
                ptr_modif = null;
                if (sym.mangled[sym.current] == 'E')
                {
                    ptr_modif = "__ptr64";
                    sym.current++;
                }
                switch (sym.mangled[sym.current++])
                {
                case 'A': ret = null; break;
                case 'B': ret = "const"; break;
                case 'C': ret = "volatile"; break;
                case 'D': ret = "const volatile"; break;
                default: ret = null; return false;
                }
                return true;
            }

            static bool get_modified_type(datatype_t ct, parsed_symbol sym,
                                          List<string> pmt_ref, char modif, bool in_args)
            {
                string modifier;
                string str_modif;
                string ptr_modif = "";

                if (sym.mangled[sym.current] == 'E')
                {
                    ptr_modif = " __ptr64";
                    sym.current++;
                }

                switch (modif)
                {
                case 'A': str_modif = str_printf(sym, " &%s", ptr_modif); break;
                case 'B': str_modif = str_printf(sym, " &%s volatile", ptr_modif); break;
                case 'P': str_modif = str_printf(sym, " *%s", ptr_modif); break;
                case 'Q': str_modif = str_printf(sym, " *%s const", ptr_modif); break;
                case 'R': str_modif = str_printf(sym, " *%s volatile", ptr_modif); break;
                case 'S': str_modif = str_printf(sym, " *%s const volatile", ptr_modif); break;
                case '?': str_modif = ""; break;
                default: return false;
                }

                if (get_modifier(sym, out modifier, out ptr_modif))
                {
                    int mark = sym.stack.Count;
                    datatype_t sub_ct = new datatype_t();

                    /* multidimensional arrays */
                    if (sym.mangled[sym.current] == 'Y')
                    {
                        string n1;
                        int num;
                        sym.current++;
                        n1 = get_number(sym);
                        if (n1 == null) return false;
                        num = Convert.ToInt32(n1);

                        if (str_modif[0] == ' ' && modifier == null)
                            str_modif = str_modif.Substring(1);

                        if (modifier != null)
                        {
                            str_modif = str_printf(sym, " ({0}{1})", modifier, str_modif);
                            modifier = null;
                        }
                        else
                            str_modif = str_printf(sym, " ({0})", str_modif);

                        while (num-- > 0)
                            str_modif = str_printf(sym, "{0}[{1}]", str_modif, get_number(sym));
                    }

                    /* Recurse to get the referred-to type */
                    if (!demangle_datatype(sym, sub_ct, pmt_ref, false))
                        return false;
                    if (modifier != null)
                        ct.left = str_printf(sym, "{0} {1}{2}", sub_ct.left, modifier, str_modif);
                    else
                    {
                        /* don't insert a space between duplicate '*' */
                        if (!in_args && str_modif.Length > 0 && str_modif[1] == '*' && sub_ct.left[sub_ct.left.Length - 1] == '*')
                            str_modif = str_modif.Substring(1);
                        ct.left = string.Format("{0}{1}", sub_ct.left, str_modif);
                    }
                    ct.right = sub_ct.right;
                    sym.stack.RemoveRange(mark, sym.stack.Count - mark);
                }
                return true;
            }

            /******************************************************************
             *             get_literal_string
             * Gets the literal name from the current position in the mangled
             * symbol to the first '@' character. It pushes the parsed name to
             * the symbol names stack and returns a pointer to it or NULL in
             * case of an error.
             */
            static string get_literal_string(parsed_symbol sym)
            {
                int ptr = sym.current;

                do {
                    var ch = sym.mangled[sym.current];
                    if (!((ch >= 'A' && ch <= 'Z') ||
                          (ch >= 'a' && ch <= 'z') ||
                          (ch >= '0' && ch <= '9') ||
                          (ch == '_' || ch == '$'))) {
                        Debug.Print("Failed at '{0}' in {1}\n", sym.mangled[sym.current], ptr);
                        return null;
                    }
                } while (sym.mangled[++sym.current] != '@');
                sym.current++;
                sym.names.Add(sym.mangled.Substring(sym.current - 1 - ptr));
                return sym.names[sym.names.Count() - 1];
            }

            /******************************************************************
             *		get_template_name
             * Parses a name with a template argument list and returns it as
             * a string.
             * In a template argument list the back reference to the names
             * table is separately created. '0' points to the class component
             * name with the template arguments.  We use the same stack array
             * to hold the names but save/restore the stack state before/after
             * parsing the template argument list.
             */
            static string get_template_name(parsed_symbol sym)
            {
                string name, args;
                uint num_mark = sym.names.Count;
                uint start_mark = sym.names.start;
                uint stack_mark = sym.stack.num;
                List<string> array_pmt = new List<string>();

                sym.names.start = sym.names.num;
                if (!(name = get_literal_string(sym))) {
                    sym.names.start = start_mark;
                    return null;
                }
                //array_pmt);
                args = get_args(sym, array_pmt, false, '<', '>');
                if (args != null)
                    name = str_printf(sym, "%s%s", name, args);
                sym.names.num = num_mark;
                sym.names.start = start_mark;
                sym.stack.num = stack_mark;
                return name;
            }

            /******************************************************************
             *		get_class
             * Parses class as a list of parent-classes, terminated by '@' and stores the
             * result in 'a' array. Each parent-classes, as well as the inner element
             * (either field/method name or class name), are represented in the mangled
             * name by a literal name ([a-zA-Z0-9_]+ terminated by '@') or a back reference
             * ([0-9]) or a name with template arguments ('?$' literal name followed by the
             * template argument list). The class name components appear in the reverse
             * order in the mangled name, e.g aaa@bbb@ccc@@ will be demangled to
             * ccc::bbb::aaa
             * For each of these class name components a string will be allocated in the
             * array.
             */
            static bool get_class(parsed_symbol sym)
            {
                string name = null;

                while (sym.mangled[sym.current] != '@')
                {
                    if (sym.mangled.Length == sym.current)
                        return false;
                    switch (sym.mangled[sym.current])
                    {
                    case '0': case '1': case '2': case '3':
                    case '4': case '5': case '6': case '7':
                    case '8': case '9':
                        name = sym.names[sym.mangled[sym.current++] - '0'];
                        break;
                    case '?':
                        switch (sym.mangled[++sym.current])
                        {
                        case '$':
                            sym.current++;
                            name = get_template_name(sym);
                            if (name != null)
                            {
                                sym.names.Add(name);
                            }
                            break;
                        case '?':
                            {
                                var stack = new List<string>( sym.stack);
                                uint start = sym.names.start;
                                uint num = sym.names.num;

                                sym.stack = new List<string>();
                                if (symbol_demangle(sym))
                                    name = string.Format("`{0}'", sym.result);
                                sym.names.start = start;
                                sym.names.num = num;
                                sym.stack = stack;
                            }
                            break;
                        default:
                            if (!(name = get_number(sym))) return false;
                            name = str_printf(sym, "`%s'", name);
                            break;
                        }
                        break;
                    default:
                        name = get_literal_string(sym);
                        break;
                    }
                    if (name == null)
                        return false;
                    sym.stack.Add(name);
                }
                sym.current++;
                return true;
            }

            /******************************************************************
             *		get_class_string
             * From an array collected by get_class in sym.stack, constructs the
             * corresponding (allocated) string
             */
            static string get_class_string(parsed_symbol sym, int start)
            {
                return string.Join("::", sym.stack.Reverse());
            }

            /******************************************************************
             *            get_class_name
             * Wrapper around get_class and get_class_string.
             */
            static string get_class_name(parsed_symbol sym)
            {
                int mark = sym.stack.num;
                string s = null;

                if (get_class(sym))
                    s = get_class_string(sym, mark);
                sym.stack.num = mark;
                return s;
            }

            /******************************************************************
             *		get_calling_convention
             * Returns a static string corresponding to the calling convention described
             * by char 'ch'. Sets export to TRUE iff the calling convention is exported.
             */
            static bool get_calling_convention(char ch, out string call_conv,
                                               out string exported, uint flags)
            {
                call_conv = exported = null;
                if (((ch - 'A') % 2) == 1) exported = "__dll_export ";
                switch (ch)
                {
                case 'A': case 'B': call_conv = "__cdecl"; break;
                case 'C': case 'D': call_conv = "__pascal"; break;
                case 'E': case 'F': call_conv = "__thiscall"; break;
                case 'G': case 'H': call_conv = "__stdcall"; break;
                case 'I': case 'J': call_conv = "__fastcall"; break;
                case 'K': case 'L': break;
                case 'M': call_conv = "__clrcall"; break;
                default: ERR("Unknown calling convention{0}\n", ch); return false;
                }
                return true;
            }

            /*******************************************************************
             *         get_simple_type
             * Return a string containing an allocated string for a simple data type
             */
            static string get_simple_type(char c)
            {
                string type_string;

                switch (c)
                {
                case 'C': type_string = "signed char"; break;
                case 'D': type_string = "char"; break;
                case 'E': type_string = "unsigned char"; break;
                case 'F': type_string = "short"; break;
                case 'G': type_string = "unsigned short"; break;
                case 'H': type_string = "int"; break;
                case 'I': type_string = "unsigned int"; break;
                case 'J': type_string = "long"; break;
                case 'K': type_string = "unsigned long"; break;
                case 'M': type_string = "float"; break;
                case 'N': type_string = "double"; break;
                case 'O': type_string = "long double"; break;
                case 'X': type_string = "void"; break;
                case 'Z': type_string = "..."; break;
                default: type_string = null; break;
                }
                return type_string;
            }

            /*******************************************************************
             *         get_extended_type
             * Return a string containing an allocated string for a simple data type
             */
            static string get_extended_type(char c)
            {
                string type_string;

                switch (c)
                {
                case 'D': type_string = "__int8"; break;
                case 'E': type_string = "unsigned __int8"; break;
                case 'F': type_string = "__int16"; break;
                case 'G': type_string = "unsigned __int16"; break;
                case 'H': type_string = "__int32"; break;
                case 'I': type_string = "unsigned __int32"; break;
                case 'J': type_string = "__int64"; break;
                case 'K': type_string = "unsigned __int64"; break;
                case 'L': type_string = "__int128"; break;
                case 'M': type_string = "unsigned __int128"; break;
                case 'N': type_string = "bool"; break;
                case 'W': type_string = "wchar_t"; break;
                default: type_string = null; break;
                }
                return type_string;
            }

            /*******************************************************************
             *         demangle_datatype
             *
             * Attempt to demangle a C++ data type, which may be datatype.
             * a datatype type is made up of a number of simple types. e.g:
             * char** = (pointer to (pointer to (char)))
             */
            static bool demangle_datatype(parsed_symbol sym, datatype_t ct,
                                          List<string> pmt_ref, bool in_args)
            {
                char dt;
                bool add_pmt = true;

                Debug.Assert(ct != null);
                ct.left = ct.right = null;

                switch (dt = sym.mangled[sym.current++])
                {
                case '_':
                    /* MS type: __int8,__int16 etc */
                    ct.left = get_extended_type(sym.mangled[sym.current++]);
                    break;
                case 'C': case 'D': case 'E': case 'F': case 'G':
                case 'H': case 'I': case 'J': case 'K': case 'M':
                case 'N': case 'O': case 'X': case 'Z':
                    /* Simple data types */
                    ct.left = get_simple_type(dt);
                    add_pmt = false;
                    break;
                case 'T': /* union */
                case 'U': /* struct */
                case 'V': /* class */
                case 'Y': /* cointerface */
                          /* Class/struct/union/cointerface */
                    {
                        string struct_name = null;
                        string type_name = null;

                        struct_name = get_class_name(sym);
                        if (struct_name == null)
                            goto done;
                        switch (dt)
                        {
                        case 'T': type_name = "union "; break;
                        case 'U': type_name = "struct "; break;
                        case 'V': type_name = "class "; break;
                        case 'Y': type_name = "cointerface "; break;
                        }
                        ct.left = string.Format("{0}{1}", type_name, struct_name);
                    }
                    break;
                case '?':
                    /* not all the time is seems */
                    if (in_args)
                    {
                        string ptr = get_number(sym);
                        if (ptr == null)
                            goto done;
                        ct.left = string.Format("`template-parameter-{0}'", ptr);
                    }
                    else
                    {
                        if (!get_modified_type(ct, sym, pmt_ref, '?', in_args)) goto done;
                    }
                    break;
                case 'A': /* reference */
                case 'B': /* volatile reference */
                    if (!get_modified_type(ct, sym, pmt_ref, dt, in_args)) goto done;
                    break;
                case 'Q': /* const pointer */
                case 'R': /* volatile pointer */
                case 'S': /* const volatile pointer */
                    if (!get_modified_type(ct, sym, pmt_ref, in_args ? dt : 'P', in_args)) goto done;
                    break;
                case 'P': /* Pointer */
                    if (Char.IsDigit(sym.mangled[sym.current]))
                    {
                        /* FIXME:
                         *   P6 = Function pointer
                         *   P8 = Member function pointer
                         *   others who knows.. */
                        if (sym.mangled[sym.current] == '8')
                        {
                            string args = null;
                            string call_conv;
                            string exported;
                            datatype_t sub_ct = new datatype_t();
                            uint mark = sym.stack.num;
                            string _class;
                            string modifier;
                            string ptr_modif;

                            sym.current++;
                            _class = get_class_name(sym);
                            if (_class == null)
                                goto done;
                            if (!get_modifier(sym, out modifier, out ptr_modif))
                                goto done;
                            if (modifier != null)
                                modifier = str_printf(sym, "%s %s", modifier, ptr_modif);
                            else if (ptr_modif.Length > 0)
                                modifier = str_printf(sym, " %s", ptr_modif);
                            if (!get_calling_convention(sym.mangled[sym.current++],
                                        out call_conv, out exported,
                                        sym.flags))
                                goto done;
                            if (!demangle_datatype(sym, sub_ct, pmt_ref, false))
                                goto done;

                            args = get_args(sym, pmt_ref, true, '(', ')');
                            if (args == null) goto done;
                            sym.stack.num = mark;

                            ct.left = str_printf(sym, "%s%s (%s %s::*",
                                    sub_ct.left, sub_ct.right, call_conv, _class);
                            ct.right = string.Format("){0}{1}", args, modifier);
                        }
                        else if (sym.mangled[sym.current] == '6')
                        {
                            string args = null;
                            string call_conv;
                            string exported;
                            datatype_t sub_ct;
                            uint mark = sym.stack.num;
                            sym.current++;
                            if (!get_calling_convention(sym.mangled[sym.current++],
                                                        out call_conv, out exported,
                                                        sym.flags) ||
                                !demangle_datatype(sym, out sub_ct, pmt_ref, false))
                                goto done;

                            args = get_args(sym, pmt_ref, true, '(', ')');
                            if (args == null) goto done;
                            sym.stack.num = mark;

                            ct.left = string.Format(sym, "{0}{1} ({2}*",
                                                   sub_ct.left, sub_ct.right, call_conv);
                            ct.right = str_printf(sym, "){0}", args);
                        }
                        else
                            goto done;
                    }
                    else if (!get_modified_type(ct, sym, pmt_ref, 'P', in_args)) goto done;
                    break;
                case 'W':
                    if (sym.mangled[sym.current] == '4')
                    {
                        string enum_name;
                        sym.current++;
                        if (!(enum_name = get_class_name(sym)))
                            goto done;
                        if (sym.flags & UNDNAME_NO_COMPLEX_TYPE)
                            ct.left = enum_name;
                        else
                            ct.left = str_printf(sym, "enum %s", enum_name);
                    }
                    else goto done;
                    break;
                case '0': case '1': case '2': case '3': case '4':
                case '5': case '6': case '7': case '8': case '9':
                    /* Referring back to previously parsed type */
                    /* left and right are pushed as two separate strings */
                    ct.left = str_array_get_ref(pmt_ref, (dt - '0') * 2);
                    ct.right = str_array_get_ref(pmt_ref, (dt - '0') * 2 + 1);
                    if (!ct.left) goto done;
                    add_pmt = false;
                    break;
                case '$':
                    switch (sym.mangled[sym.current++])
                    {
                    case '0':
                        if (!(ct.left = get_number(sym))) goto done;
                        break;
                    case 'D':
                        {
                            string ptr = get_number(sym);
                            if (ptr == null) goto done;
                            ct.left = str_printf(sym, "`template-parameter{0}'", ptr);
                        }
                        break;
                    case 'F':
                        {
                            string p1 = get_number(sym);
                            if (p1 == null)
                                goto done;
                            string p2 = get_number(sym);
                            if (p2 == null)
                                goto done;
                            ct.left = string.Format("{0}{1},{2}{3}", "{", p1, p2, "}");
                        }
                        break;
                    case 'G':
                        {
                            string p1;
                            string p2;
                            string p3;
                            if (!(p1 = get_number(sym))) goto done;
                            if (!(p2 = get_number(sym))) goto done;
                            if (!(p3 = get_number(sym))) goto done;
                            ct.left = str_printf(sym, "{%s,%s,%s}", p1, p2, p3);
                        }
                        break;
                    case 'Q':
                        {
                            string ptr;
                            if (!(ptr = get_number(sym))) goto done;
                            ct.left = str_printf(sym, "`non-type-template-parameter{0}'", ptr);
                        }
                        break;
                    case '$':
                        if (sym.mangled[sym.current] == 'B')
                        {
                            uint mark = sym.stack.num;
                            datatype_t sub_ct;
                            string arr = null;
                            sym.current++;

                            /* multidimensional arrays */
                            if (sym.mangled[sym.current] == 'Y')
                            {
                                string n1;
                                int num;

                                sym.current++;
                                if (!(n1 = get_number(sym))) goto done;
                                num = atoi(n1);

                                while (num-- > 0)
                                    arr = string.Format("{0}[{1}]", arr, get_number(sym));
                            }

                            if (!demangle_datatype(sym, out sub_ct, pmt_ref, false)) goto done;

                            if (arr != null)
                                ct.left = string.Format("{0} {1}", sub_ct.left, arr);
                            else
                                ct.left = sub_ct.left;
                            ct.right = sub_ct.right;
                            sym.stack.num = mark;
                        }
                        else if (sym.mangled[sym.current] == 'C')
                        {
                            string ptr, ptr_modif;
                            sym.current++;
                            if (!get_modifier(sym, out ptr, out ptr_modif)) goto done;
                            if (!demangle_datatype(sym, ct, pmt_ref, in_args)) goto done;
                            ct.left = string.Format("{0} {1}", ct.left, ptr);
                        }
                        break;
                    }
                    break;
                default:
                    ERR("Unknown type {0}\n", dt);
                    break;
                }
                if (add_pmt && pmt_ref != null && in_args)
                {
                    /* left and right are pushed as two separate strings */
                    if (!str_array_push(sym, ct.left != null? ct.left : "", -1, pmt_ref) ||
                        !str_array_push(sym, ct.right != null? ct.right : "", -1, pmt_ref))
                        return false;
                }
                done:
                return ct.left != null;
            }

            /******************************************************************
             *		handle_data
             * Does the final parsing and handling for a variable or a field in
             * a class.
             */
            static bool handle_data(parsed_symbol sym)
            {
                string access = null;
                string member_type = null;
                string modifier = null;
                string ptr_modif;
                datatype_t ct;
                string name = null;
                bool ret = false;

                /* 0 private static
                 * 1 protected static
                 * 2 public static
                 * 3 private non-static
                 * 4 protected non-static
                 * 5 public non-static
                 * 6 ?? static
                 * 7 ?? static
                 */

                /* we only print the access for static members */
                switch (sym.mangled[sym.current])
                {
                case '0': access = "private: "; break;
                case '1': access = "protected: "; break;
                case '2': access = "public: "; break;
                }

                if (sym.mangled[sym.current] >= '0' && sym.mangled[sym.current] <= '2')
                    member_type = "static ";

                name = get_class_string(sym, 0);

                switch (sym.mangled[sym.current++])
                {
                case '0': case '1': case '2':
                case '3': case '4': case '5':
                    {
                        uint mark = sym.stack.num;
                        List<string> pmt = new List<string>();

                        if (!demangle_datatype(sym, ct, pmt, false)) goto done;
                        if (!get_modifier(sym, out modifier, out ptr_modif)) goto done;
                        if (modifier != null && ptr_modif != null) modifier = string.Format("{0} {1}", modifier, ptr_modif);
                        else if (modifier == null) modifier = ptr_modif;
                        sym.stack.num = mark;
                    }
                    break;
                case '6': /* compiler generated static */
                case '7': /* compiler generated static */
                    ct.left = ct.right = null;
                    if (!get_modifier(sym, out modifier, out ptr_modif)) goto done;
                    if (sym.mangled[sym.current] != '@')
                    {
                        string cls = get_class_name(sym);
                        if (cls == null)
                            goto done;
                        ct.right = str_printf(sym, "{1}for `{0}'{2}", cls, "{", "}");
                    }
                    break;
                case '8':
                case '9':
                    modifier = ct.left = ct.right = null;
                    break;
                default: goto done;
                }

                sym.result = str_printf(sym, "{0}{1}{2}{3}{4}{5}{6}{7}", 
                                        access,
                                         member_type, ct.left,
                                         modifier!=null && ct.left != null? " " : null, modifier,
                                         modifier!=null || ct.left !=null? " " : null, name, ct.right);
                ret = true;
                done:
                return ret;
            }

            /******************************************************************
             *		handle_method
             * Does the final parsing and handling for a function or a method in
             * a class.
             */
            static bool handle_method(parsed_symbol sym, bool cast_op)
            {
                char accmem;
                string access = null;
                int access_id = -1;
                string member_type = null;
                datatype_t ct_ret = new datatype_t();
                string call_conv;
                string modifier = null;
                string exported;
                string args_str = null;
                string name = null;
                bool ret = false, has_args = true, has_ret = true;
                uint mark;
                List<string> array_pmt = new List<string>();

                /* FIXME: why 2 possible letters for each option?
                 * 'A' private:
                 * 'B' private:
                 * 'C' private: static
                 * 'D' private: static
                 * 'E' private: virtual
                 * 'F' private: virtual
                 * 'G' private: thunk
                 * 'H' private: thunk
                 * 'I' protected:
                 * 'J' protected:
                 * 'K' protected: static
                 * 'L' protected: static
                 * 'M' protected: virtual
                 * 'N' protected: virtual
                 * 'O' protected: thunk
                 * 'P' protected: thunk
                 * 'Q' public:
                 * 'R' public:
                 * 'S' public: static
                 * 'T' public: static
                 * 'U' public: virtual
                 * 'V' public: virtual
                 * 'W' public: thunk
                 * 'X' public: thunk
                 * 'Y'
                 * 'Z'
                 * "$0" private: thunk vtordisp
                 * "$1" private: thunk vtordisp
                 * "$2" protected: thunk vtordisp
                 * "$3" protected: thunk vtordisp
                 * "$4" public: thunk vtordisp
                 * "$5" public: thunk vtordisp
                 * "$B" vcall thunk
                 * "$R" thunk vtordispex
                 */
                accmem = sym.mangled[sym.current++];
                if (accmem == '$')
                {
                    if (sym.mangled[sym.current] >= '0' && sym.mangled[sym.current] <= '5')
                        access_id = (sym.mangled[sym.current] - '0') / 2;
                    else if (sym.mangled[sym.current] == 'R')
                        access_id = (sym.current[1] - '0') / 2;
                    else if (sym.mangled[sym.current] != 'B')
                        goto done;
                }
                else if (accmem >= 'A' && accmem <= 'Z')
                    access_id = (accmem - 'A') / 8;
                else
                    goto done;

                switch (access_id)
                {
                case 0: access = "private: "; break;
                case 1: access = "protected: "; break;
                case 2: access = "public: "; break;
                }
                if (accmem == '$' || (accmem - 'A') % 8 == 6 || (accmem - 'A') % 8 == 7)
                    access = str_printf(sym, "[thunk]:%s", access != null ? access : " ");

                if (accmem == '$' && sym.mangled[sym.current] != 'B')
                    member_type = "virtual ";
                else if (accmem <= 'X')
                {
                    switch ((accmem - 'A') % 8)
                    {
                    case 2: case 3: member_type = "static "; break;
                    case 4: case 5: case 6: case 7: member_type = "virtual "; break;
                    }
                }

                if (sym.flags & UNDNAME_NO_ACCESS_SPECIFIERS)
                    access = null;
                if (sym.flags & UNDNAME_NO_MEMBER_TYPE)
                    member_type = null;

                name = get_class_string(sym, 0);

                if (accmem == '$' && sym.mangled[sym.current] == 'B') /* vcall thunk */
                {
                    string n;

                    sym.current++;
                    n = get_number(sym);

                    if (!n || sym.mangled[sym.current++] != 'A') goto done;
                    name = str_printf(sym, "%s{%s,{flat}}' }'", name, n);
                    has_args = FALSE;
                    has_ret = FALSE;
                }
                else if (accmem == '$' && sym.mangled[sym.current] == 'R') /* vtordispex thunk */
                {
                    string n1, n2, n3, n4;

                    sym.current += 2;
                    n1 = get_number(sym);
                    n2 = get_number(sym);
                    n3 = get_number(sym);
                    n4 = get_number(sym);

                    if (n1 == null || n2 == null || n3 == null || n4 == null) goto done;
                    name = str_printf(sym, "%s`vtordispex{%s,%s,%s,%s}' ", name, n1, n2, n3, n4);
                }
                else if (accmem == '$') /* vtordisp thunk */
                {
                    string n1, n2;

                    sym.current++;
                    n1 = get_number(sym);
                    n2 = get_number(sym);

                    if (n1 ==null|| n2==null) goto done;
                    name = str_printf(sym, "%s`vtordisp{%s,%s}' ", name, n1, n2);
                }
                else if ((accmem - 'A') % 8 == 6 || (accmem - 'A') % 8 == 7) /* a thunk */
                    name = str_printf(sym, "%s`adjustor{%s}' ", name, get_number(sym));

                if (has_args && (accmem == '$' ||
                            (accmem <= 'X' && (accmem - 'A') % 8 != 2 && (accmem - 'A') % 8 != 3)))
                {
                    int ptr_modif;
                    /* Implicit 'this' pointer */
                    /* If there is an implicit this pointer, const modifier follows */
                    if (!get_modifier(sym, out modifier, out ptr_modif)) goto done;
                    if (modifier || ptr_modif) modifier = str_printf(sym, "%s %s", modifier, ptr_modif);
                }

                if (!get_calling_convention(*sym.current++, &call_conv, &exported,
                                            sym.flags))
                    goto done;

                str_array_init(&array_pmt);

                /* Return type, or @ if 'void' */
                if (has_ret && sym.mangled[sym.current] == '@')
                {
                    ct_ret.left = "void";
                    ct_ret.right = null;
                    sym.current++;
                }
                else if (has_ret)
                {
                    if (!demangle_datatype(sym, ct_ret, &array_pmt, false))
                        goto done;
                }
                if (!has_ret || sym.flags & UNDNAME_NO_FUNCTION_RETURNS)
                    ct_ret.left = ct_ret.right = NULL;
                if (cast_op)
                {
                    name = str_printf(sym, "%s%s%s", name, ct_ret.left, ct_ret.right);
                    ct_ret.left = ct_ret.right = NULL;
                }

                mark = sym.stack.num;
                if (has_args && !(args_str = get_args(sym, array_pmt, true, '(', ')'))) goto done;
                sym.stack.RemoveRange(mark, sym.stack.Count - mark);

                /* Note: '()' after 'Z' means 'throws', but we don't care here
                 * Yet!!! FIXME
                 */
                sym.result = str_printf(sym, "%s%s%s%s%s%s%s%s%s%s%s",
                                         access, member_type, ct_ret.left,
                                         (ct_ret.left !=null && ct_ret.right == null) ? " " : null,
                                         call_conv, call_conv!=null ? " " : null, exported,
                                         name, args_str, modifier, ct_ret.right);
                ret = true;
                done:
                return ret;
            }

            /*******************************************************************
             *         symbol_demangle
             * Demangle a C++ linker symbol
             */
            static bool symbol_demangle(parsed_symbol sym)
            {
                bool ret = false;
                uint do_after = 0;
                string dashed_null = "--null--";

                /* FIXME seems wrong as name, as it demangles a simple data type */
                if (false)
                {
                    datatype_t ct = new datatype_t();

                    if (demangle_datatype(sym, ct, null, false))
                    {
                        sym.result = string.Format("{0}{1}", ct.left, ct.right);
                        ret = true;
                    }
                    goto done;
                }

                /* MS mangled names always begin with '?' */
                if (sym.mangled[sym.current] != '?') return false;
                sym.current++;

                /* Then function name or operator code */
                if (sym.mangled[sym.current] == '?' &&
                    (sym.mangled[sym.current+1] != '$' || sym.mangled[sym.current+2] == '?'))
                {
                    string function_name = null;

                    if (sym.mangled[sym.current+1] == '$')
                    {
                        do_after = 6;
                        sym.current += 2;
                    }

                    /* C++ operator code (one character, or two if the first is '_') */
                    switch (sym.mangled[++sym.current])
                    {
                    case '0': do_after = 1; break;      //ctor
                    case '1': do_after = 2; break;      //dtor
                    case '2': function_name = "operator new"; break;
                    case '3': function_name = "operator delete"; break;
                    case '4': function_name = "operator="; break;
                    case '5': function_name = "operator>>"; break;
                    case '6': function_name = "operator<<"; break;
                    case '7': function_name = "operator!"; break;
                    case '8': function_name = "operator=="; break;
                    case '9': function_name = "operator!="; break;
                    case 'A': function_name = "operator[]"; break;
                    case 'B': function_name = "operator "; do_after = 3; break;
                    case 'C': function_name = "operator."; break;
                    case 'D': function_name = "operator*"; break;
                    case 'E': function_name = "operator++"; break;
                    case 'F': function_name = "operator--"; break;
                    case 'G': function_name = "operator-"; break;
                    case 'H': function_name = "operator+"; break;
                    case 'I': function_name = "operator&"; break;
                    case 'J': function_name = "operator.*"; break;
                    case 'K': function_name = "operator/"; break;
                    case 'L': function_name = "operator%"; break;
                    case 'M': function_name = "operator<"; break;
                    case 'N': function_name = "operator<="; break;
                    case 'O': function_name = "operator>"; break;
                    case 'P': function_name = "operator>="; break;
                    case 'Q': function_name = "operator,"; break;
                    case 'R': function_name = "operator()"; break;
                    case 'S': function_name = "operator~"; break;
                    case 'T': function_name = "operator^"; break;
                    case 'U': function_name = "operator|"; break;
                    case 'V': function_name = "operator&&"; break;
                    case 'W': function_name = "operator||"; break;
                    case 'X': function_name = "operator*="; break;
                    case 'Y': function_name = "operator+="; break;
                    case 'Z': function_name = "operator-="; break;
                    case '_':
                        switch (sym.mangled[++sym.current])
                        {
                        case '0': function_name = "operator/="; break;
                        case '1': function_name = "operator%="; break;
                        case '2': function_name = "operator>>="; break;
                        case '3': function_name = "operator<<="; break;
                        case '4': function_name = "operator&="; break;
                        case '5': function_name = "operator|="; break;
                        case '6': function_name = "operator^="; break;
                        case '7': function_name = "`vftable'"; break;
                        case '8': function_name = "`vbtable'"; break;
                        case '9': function_name = "`vcall'"; break;
                        case 'A': function_name = "`typeof'"; break;
                        case 'B': function_name = "`local static guard'"; break;
                        case 'C': function_name = "`string'"; do_after = 4; break;
                        case 'D': function_name = "`vbase destructor'"; break;
                        case 'E': function_name = "`vector deleting destructor'"; break;
                        case 'F': function_name = "`default constructor closure'"; break;
                        case 'G': function_name = "`scalar deleting destructor'"; break;
                        case 'H': function_name = "`vector constructor iterator'"; break;
                        case 'I': function_name = "`vector destructor iterator'"; break;
                        case 'J': function_name = "`vector vbase constructor iterator'"; break;
                        case 'K': function_name = "`virtual displacement map'"; break;
                        case 'L': function_name = "`eh vector constructor iterator'"; break;
                        case 'M': function_name = "`eh vector destructor iterator'"; break;
                        case 'N': function_name = "`eh vector vbase constructor iterator'"; break;
                        case 'O': function_name = "`copy constructor closure'"; break;
                        case 'R':
                            switch (sym.mangled[++sym.current])
                            {
                            case '0':
                                {
                                    datatype_t ct = new datatype_t();
                                    List<string> pmt = new List<string>();

                                    sym.current++;
                                    demangle_datatype(sym, ct, pmt, false);
                                    if (!demangle_datatype(sym, ct, null, false))
                                        goto done;
                                    function_name = str_printf(sym, "{0}{1} `RTTI Type Descriptor'",
                                                               ct.left, ct.right);
                                    sym.current--;
                                }
                                break;
                            case '1':
                                {
                                    string n1, n2, n3, n4;
                                    sym.current++;
                                    n1 = get_number(sym);
                                    n2 = get_number(sym);
                                    n3 = get_number(sym);
                                    n4 = get_number(sym);
                                    sym.current--;
                                    function_name = str_printf(sym, "`RTTI Base Class Descriptor at ({0},{1},{2},{3})'",
                                                               n1, n2, n3, n4);
                                }
                                break;
                            case '2': function_name = "`RTTI Base Class Array'"; break;
                            case '3': function_name = "`RTTI Class Hierarchy Descriptor'"; break;
                            case '4': function_name = "`RTTI Complete Object Locator'"; break;
                            default:
                                ERR("Unknown RTTI operator: _R%c\n", sym.mangled[sym.current]);
                                break;
                            }
                            break;
                        case 'S': function_name = "`local vftable'"; break;
                        case 'T': function_name = "`local vftable constructor closure'"; break;
                        case 'U': function_name = "operator new[]"; break;
                        case 'V': function_name = "operator delete[]"; break;
                        case 'X': function_name = "`placement delete closure'"; break;
                        case 'Y': function_name = "`placement delete[] closure'"; break;
                        default:
                            ERR("Unknown operator: _%c\n", sym.mangled[sym.current]);
                            return false;
                        }
                        break;
                    default:
                        /* FIXME: Other operators */
                        ERR("Unknown operator: {0}\n", sym.mangled[sym.current]);
                        return false;
                    }
                    sym.current++;
                    switch (do_after)
                    {
                    case 1: case 2:
                        sym.stack.Add(dashed_null);
                        break;
                    case 4:
                        sym.result = function_name;
                        ret = true;
                        goto done;
                    case 6:
                        {
                            var array_pmt = new List<string>();
                            string args = get_args(sym, array_pmt, false, '<', '>');
                            if (args != null) function_name = str_printf(sym, "{0}{1}", function_name, args);
                            sym.names.Clear();
                        }
                        /* fall through */
                        goto default;
                    default:
                        sym.stack.Add(function_name);
                        break;
                    }
                }
                else if (sym.mangled[sym.current] == '$')
                {
                    /* Strange construct, it's a name with a template argument list
                       and that's all. */
                    sym.current++;
                    ret = (sym.result = get_template_name(sym)) != null;
                    goto done;
                }
                else if (sym.mangled[sym.current] == '?' && sym.mangled[sym.current + 1] == '$')
                    do_after = 5;

                /* Either a class name, or '@' if the symbol is not a class member */
                switch (sym.mangled[sym.current])
                {
                case '@': sym.current++; break;
                case '$': break;
                default:
                    /* Class the function is associated with, terminated by '@@' */
                    if (!get_class(sym)) goto done;
                    break;
                }

                switch (do_after)
                {
                case 0: default: break;
                case 1: case 2:
                    /* it's time to set the member name for ctor & dtor */
                    if (sym.stack.Count <= 1) goto done;
                    if (do_after == 1)
                        sym.stack[0] = sym.stack[1];
                    else
                        sym.stack[0] = string.Format("~{0}", sym.stack[1]);
                    /* ctors and dtors don't have return type */
                    sym.flags |= UNDNAME_NO_FUNCTION_RETURNS;
                    break;
                case 3:
                    sym.flags &= ~UNDNAME_NO_FUNCTION_RETURNS;
                    break;
                case 5:
                    sym.names.start++;
                    break;
                }

                /* Function/Data type and access level */
                if (sym.mangled[sym.current] >= '0' && sym.mangled[sym.current] <= '9')
                    ret = handle_data(sym);
                else if ((sym.mangled[sym.current] >= 'A' && sym.mangled[sym.current] <= 'Z') || sym.mangled[sym.current] == '$')
                    ret = handle_method(sym, do_after == 3);
                else ret = false;
                done:
                if (ret) Debug.Assert(sym.result != null);
                else WARN("Failed at %s\n", sym.current);

                return ret;
            }

            private static void ERR(string v1, char v2)
            {
                throw new NotImplementedException();
            }

            /*********************************************************************
             *		__unDNameEx (MSVCRT.@)
             *
             * Demangle a C++ identifier.
             *
             * PARAMS
             *  buffer   [O] If not NULL, the place to put the demangled string
             *  mangled  [I] Mangled name of the function
             *  buflen   [I] Length of buffer
             *  memget   [I] Function to allocate memory with
             *  memfree  [I] Function to free memory with
             *  unknown  [?] Unknown, possibly a call back
             *  flags    [I] Flags determining demangled format
             *
             * RETURNS
             *  Success: A string pointing to the unmangled name, allocated with memget.
             *  Failure: NULL.
             */
            string __unDNameEx(string buffer, string mangled, int buflen,
                                    object unknown, uint flags)
            {
                parsed_symbol sym;

                TRACE("({0},{1},{2},{5},{6:X})\n",
                      buffer, mangled, buflen, unknown, flags);

                /* The flags details is not documented by MS. However, it looks exactly
                 * like the UNDNAME_ manifest constants from imagehlp.h and dbghelp.h
                 * So, we copied those (on top of the file)
                 */
                sym = new parsed_symbol();
                sym.flags = flags;
                sym.mangled = mangled;
                sym.current = 0;
                sym.names = new List<string>();
                sym.stack = new List<string>();

                return symbol_demangle(sym) ? sym.result : mangled;
            }
        }
    }
}
