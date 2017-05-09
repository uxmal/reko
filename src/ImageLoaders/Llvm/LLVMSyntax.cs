#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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

using Reko.Core.Output;
using System;
using System.Collections.Generic;
using System.IO;

namespace Reko.ImageLoaders.LLVM
{
    public abstract class LLVMSyntax
    {
        public abstract void Write(Formatter w);

        public override string ToString()
        {
            var sw = new StringWriter();
            var f = new TextFormatter(sw);
            f.UseTabs = false;
            this.Write(f);
            return sw.ToString();
        }
    }

    public class Module : LLVMSyntax
    {
        // Each module consists of functions, global variables, and symbol 
        // table entries
        public List<ModuleEntry> Entries;

        public override void Write(Formatter w)
        {
            bool sep = false;
            foreach (var entry in Entries)
            {
                if (sep)
                    w.WriteLine();
                entry.Write(w);
                sep = true;
            }
        }
    }

    public abstract class ModuleEntry : LLVMSyntax
    {

    }

    public class GlobalDefinition : ModuleEntry
    {
        public string Linkage;
        public string Visibility;
        public string DLLStorageClass;
        public bool ThreadLocal;
        public bool unnamed_addr;
        public bool local_unnamed_addr;
        public string AddrSpace;
        public string ExternallyInitialized;
        public bool constant;
        public string Name;
        public LLVMType Type;
        public Value Initializer;
        public string section;
        public string comdat;
        public int Alignment;

        public override void Write(Formatter w)
        {
            w.Write("@" + Name);
            w.Write(" = ");
            if (Visibility != null)
            {
                w.WriteKeyword(Visibility);
                w.Write(' ');
            }
            if (unnamed_addr)
            {
                w.WriteKeyword("unnamed_addr");
                w.Write(' ');
            }
            if (constant)
            {
                w.WriteKeyword("constant");
                w.Write(' ');
            }
            Type.Write(w);
            if (Initializer != null)
            {
                w.Write(' ');
                Initializer.Write(w);
            }
            w.WriteLine();
        }
    }

    public class Declaration : ModuleEntry
    {
        public string Name;
        public LLVMFunctionType Type;

        public override void Write(Formatter w)
        {
            w.WriteKeyword("declare");
            w.Write(" ");
            Type.ReturnType.Write(w);
            w.Write(" ");
            w.Write("@" + Name);
            w.Write('(');
            var sep = "";
            foreach (var arg in Type.Arguments)
            {
                w.Write(sep);
                sep = ", ";
                arg.Type.Write(w);
                if (arg.name != null)
                {
                    w.Write(" ");
                    w.Write(arg.name);
                }
            }
            w.Write(')');
            w.WriteLine();
        }
    }

    public class FunctionDefinition : ModuleEntry
    {
        public string linkage;
        public string visibility;
        public string DLLStorageClass;
        public string cconv;
        public string ret_attrs;
        public LLVMType ResultType;
        public string FunctionName;
        public List<LLVMArgument> Parameters;
        public List<Instruction> Instructions;

        public bool unnamed_addr;
        public bool local_unnamed_addr;
        //[fn Attrs]
        //[section "name"]
        //[comdat [($name)]] [align N]
        //[gc]
        //[prefix Constant]
        //[prologue Constant]
        //[personality Constant] (!name !N)* { ... }

        public override void Write(Formatter w)
        {
            w.WriteKeyword("define");
            w.Write(' ');
            ResultType.Write(w);
            w.Write(' ');
            w.Write("@" + FunctionName);
            w.Write('(');
            var sep = "";
            foreach (var param in Parameters)
            {
                w.Write(sep);
                sep = ", ";
                param.Type.Write(w);
                if (param.name != null)
                {
                    w.Write(' ');
                    w.Write('%' + param.name);
                }
            }
            w.Write(')');
            w.Write(' ');
            w.WriteLine("{");
            foreach (var instr in Instructions)
            {
                w.Indent();
                instr.Write(w);
                w.WriteLine();
            }
            w.Indentation -= 4;
            w.WriteLine("}");
        }
    }

    public abstract class Instruction : LLVMSyntax
    {
    }

    public abstract class Terminator : Instruction
    {

    }

    public class BrInstr : Terminator
    {
        public Value Cond;
        public Value IfTrue;
        public Value IfFalse;

        public override void Write(Formatter w)
        {
            w.WriteKeyword("br");
            w.Write(' ');
            if (Cond == null)
            {
                w.WriteKeyword("label");
                w.Write(' ');
                IfTrue.Write(w);
            }
        }
    }

    public class RetInstr : Terminator
    {
        public LLVMType Type;
        public Value Value;

        public override void Write(Formatter w)
        {
            w.WriteKeyword("ret");
            w.Write(" ");
            if (this.Type == LLVMType.Void)
            {
                w.WriteKeyword("void");
            }
            else
            {
                Type.Write(w);
                w.Write(' ');
                Value.Write(w);
            }
        }
    }

    public abstract class Binary : Instruction
    {
    }

    public abstract class BitwiseBinary :Instruction
    {

    }

    public abstract class MemoryInstruction : Instruction
    {

    }

    public abstract class OtherInstruction : Instruction
    {

    }

    public abstract class Value : LLVMSyntax
    {

    }

    public class Constant : Value
    {
        public readonly object Value;

        public Constant(object value)
        {
            this.Value = value;
        }

        public override void Write(Formatter w)
        {
            if (Value is Int32)
            {
                w.Write("{0}", Value);
                return;
            }
            var bytes = Value as byte[];
            if (bytes != null)
            {
                w.Write("c\"");
                foreach (var b in bytes)
                {
                    if (0x20 <= b && b < 0x7F)
                    {
                        w.Write((char)b);
                    }
                    else
                    {
                        w.Write("\\{0:X2}", (int)b);
                    }
                }
                w.Write("\"");
                return;
            }
            throw new NotImplementedException();
        }
    }

    public class LocalId : Value
    {
        public string Name;

        public LocalId(string name)
        {
            this.Name = name;
        }

        public override void Write(Formatter w)
        {
            w.Write("%{0}", Name);
        }
    }

    public class GlobalId : Value
    {
        public string Name;

        public GlobalId(string name)
        {
            this.Name = name;
        }

        public override void Write(Formatter w)
        {
            w.Write("@{0}", Name);
        }
    }

    public abstract class InitializerConstant : Value
    {
    }
}