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

using Reko.Core.NativeInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Tools.HdrGen
{
    public class HeaderGenerator
    {
        private static Dictionary<Type, string> blittable = new Dictionary<Type, string>
        {
            { typeof(byte), "uint8_t" },
            { typeof(short), "int16_t" },
            { typeof(ushort), "uint16_t" },
            { typeof(int), "int32_t" },
            { typeof(uint), "uint32_t" },
            { typeof(long), "int64_t" },
            { typeof(ulong), "uint64_t" },
            { typeof(char), "wchar_t" },
            { typeof(IntPtr), "void *" },
        };

        private Assembly asm;
        private TextWriter w;

        public static void Main(string[] args)
        {
            if (args.Length <= 0)
            {
                Console.WriteLine("usage: hdrgen <.NET assembly file name> [output file name]");
                return;
            }
            var assemblyName = args[0];
            var asm = typeof(NativeInteropAttribute).Assembly;
            if (args.Length == 1)
            {
                var hdrgen = new HeaderGenerator(asm, System.Console.Out);
                hdrgen.Execute();
            }
            else
            {
                TextWriter w = null;
                try
                {
                    w = File.CreateText(args[1]);
                    var hdrgen = new HeaderGenerator(asm, w);
                    hdrgen.Execute();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"hdrgen: an error occurred while generating headers. {ex.Message}");
                }
                finally
                {
                    if (w is not null)
                        w.Dispose();
                }
            }
        }

        public HeaderGenerator(Assembly asm, TextWriter w)
        {
            this.asm = asm;
            this.w = w;
        }

        public void Execute()
        { 
            var types = CollectInteropTypes(asm);
            GenerateOutput(types, w);
        }


        private IEnumerable<Type> CollectInteropTypes(Assembly asm)
        {
            try
            {
                var list = asm.GetTypes()
                    .Where(TypeHasInteropAttribute)
                    .OrderBy(t => t.Name)
                    .ToList();
                Console.Error.WriteLine("hdrgen: there are {0} classes", list.Count);
                return list;
            } catch (ReflectionTypeLoadException ex)
            {
                Console.Write(string.Join("\r\n\r\n", ex.LoaderExceptions.Select(e => e.Message)));
                throw;
            }
        }

        private bool TypeHasInteropAttribute(Type type)
        {
            var attrs = type.GetCustomAttributes(typeof(NativeInteropAttribute), true);
            return attrs.Length > 0;
        }

        public void WriteInterfaceDefinition(Type type)
        {
            var guid = type.GetCustomAttribute<GuidAttribute>();
            if (guid is not null)
            {
                WriteGuidDefinition(type.Name, guid.Value);
            }

            w.WriteLine("class {0} : public IUnknown", type.Name);
            w.WriteLine("{");
            w.WriteLine("public:");
            foreach (var method in type.GetMethods())
            {
                WriteInterfaceMethod(method);
            }
            w.WriteLine("};");
        }

        public void WriteInterfaceMethod(MethodInfo method)
        {
            w.Write("    virtual ");
            if (method.ReturnType == typeof(void))
            {
                w.Write("HRESULT STDAPICALLTYPE");
            }
            else if (blittable.TryGetValue(method.ReturnType, out string cppEquivalent))
            {
                w.Write("{0} STDAPICALLTYPE", cppEquivalent);
            }
            else if (method.ReturnType.IsEnum)
            {
                w.Write("{0} STDAPICALLTYPE", method.ReturnType.Name);
            }
            else if (method.ReturnType.IsInterface)
            {
                w.Write("{0} * STDAPICALLTYPE", method.ReturnType.Name);
            }
            else
                throw new NotImplementedException($"{method.DeclaringType.Name}.{method.Name}(): return type {method.ReturnType.FullName}");
            w.Write(" {0}(", method.Name);
            var sep = "";
            foreach (var parameter in method.GetParameters())
            {
                w.Write(sep);
                sep = ", ";
                WriteParameter(parameter);
            }
            w.WriteLine(") = 0;");
        }

        private void WriteParameter(ParameterInfo parameter)
        {
            var pType = parameter.ParameterType;
            if (parameter.IsOut)
            {
                pType = pType.GetElementType();
            }
            if (blittable.TryGetValue(pType, out string cppEquivalent))
            {
                w.Write(cppEquivalent);
            }
            else if (pType == typeof(string))
            {
                var marshalAs = parameter.GetCustomAttribute<MarshalAsAttribute>();
                if (marshalAs is null || marshalAs.Value != UnmanagedType.LPStr)
                    throw new InvalidOperationException($"Expected [MarshalAs(LPStr)] attribute on parameter {parameter.Name} of type {pType.FullName} in {parameter.Member.DeclaringType.Name}.{parameter.Member.Name}.");
                w.Write("const char *");
            }
            else if (pType.IsEnum || pType.IsValueType)
            {
                w.Write(pType.Name);
            }
            else if (pType.IsInterface)
            {
                w.Write("{0} *", pType.Name);
            }
            else throw new NotImplementedException($"Parameter {parameter.Name} of type {pType.FullName} in {parameter.Member.DeclaringType.Name}.{parameter.Member.Name}.");
            if (parameter.IsOut)
            {
                w.Write("*");
            }
            w.Write(" {0}", parameter.Name);
        }

        public void WriteGuidDefinition(string name, string value)
        {
            var guid = new Guid(value);
            var ab = guid.ToByteArray();
            w.WriteLine($"// {guid:B}".ToUpper());
            
            var guid_args = $"{guid:B}".ToUpper().Replace("{", "").Replace("}", "");
            var guid_parts = guid_args.Split('-');

            var define_args = new List<string>();

            for(int i=0; i<3; i++){
                define_args.Add("0x" + guid_parts[i]);
            }
            
            for(int i=3; i<guid_parts.Length; i++){
                var piece = guid_parts[i];
                for(int c=0; c<piece.Length; c+=2){
                    define_args.Add("0x" + piece.Substring(c, 2));
                }
            }

            var guid_define = $"DEFINE_GUID(IID_{name}, {string.Join(",", define_args.ToArray())});";
            w.WriteLine(guid_define);
        }

        private void GenerateOutput(IEnumerable<Type> types, TextWriter w)
        {
            WriteHeader(w);
            WriteForwardDeclarations(types);
            WriteDefinitions(types);
            WriteFooter(w);
        }

        private void WriteHeader(TextWriter w)
        {
            w.WriteLine("#ifndef _reko_h_");
            w.WriteLine("#define _reko_h_");
            w.WriteLine();
            w.WriteLine("// reko.h");
            w.WriteLine("// Note: this file is automatically generated by the hdrgen tool.");
            w.WriteLine("// Do not make changes in this file as they will be discarded.");
            w.WriteLine();
        }

        private void WriteForwardDeclarations(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                if (type.IsEnum)
                {
                    w.WriteLine("enum class {0};", type.Name);
                }
                else if (type.IsValueType)
                {
                    w.WriteLine("struct {0};", type.Name);
                }
                else if (type.IsInterface)
                {
                    w.WriteLine("class {0};", type.Name);
                }
            }
            w.WriteLine();
        }

        private void WriteDefinitions(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                if (type.IsEnum)
                {
                    WriteEnumDefinition(type);
                }
                else if (type.IsValueType)
                {
                    WriteStructDefinition(type);
                }
                else if (type.IsInterface)
                {
                    WriteInterfaceDefinition(type);
                }
                else
                    throw new NotImplementedException(type.FullName);
                w.WriteLine();
            }
        }

        public void WriteEnumDefinition(Type type)
        {
            w.WriteLine("enum class {0}", type.Name);
            w.WriteLine("{");

            var names = Enum.GetNames(type);
            foreach (var name in names)
            {
                var value = Convert.ToInt32(Enum.Parse(type, name));
                w.WriteLine("    {0} = {1},", name, value);
            }
            w.WriteLine("};");
        }

        public void WriteStructDefinition(Type type)
        {
            w.WriteLine("struct {0}", type.Name);
            w.WriteLine("{");
            foreach (var field in type.GetFields())
            {
                WriteField(field);
            }
            w.WriteLine("};");
        }

        public void WriteField(FieldInfo field)
        {
            w.Write("    ");
            if (blittable.TryGetValue(field.FieldType, out string cppEquivalent))
            {
                w.Write(cppEquivalent);
            }
            else if (field.FieldType == typeof(string))
            {
                w.Write("const char *");
            }
            else
                throw new NotImplementedException($"{field.DeclaringType.FullName}.{field.Name} not implemented.");
            w.WriteLine(" {0};", field.Name);
        }

        private void WriteFooter(TextWriter w)
        {
            w.WriteLine("#endif /*_reko_h_*/");
        }
    }
}
