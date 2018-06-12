﻿#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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

using System;
using Reko.Core;
using System.Collections.Generic;
using Reko.Core.Types;
using Expression = Reko.Core.Expressions.Expression;
using Identifier = Reko.Core.Expressions.Identifier;
using IrConstant = Reko.Core.Expressions.Constant;
using IrDomain = Reko.Core.Types.Domain;
using Reko.Core.Configuration;

namespace Reko.ImageLoaders.LLVM
{
    public class ProgramBuilder
    {
        private IServiceProvider services;
        private Program program;
        private IConfigurationService cfgSvc;

        public ProgramBuilder(IServiceProvider services, Program program)
        {
            this.services = services;
            this.program = program;
            this.Functions = new Dictionary<FunctionDefinition, ProcedureBuilder>();
            this.Globals = new Dictionary<string, Expression>();
            this.cfgSvc = services.RequireService<IConfigurationService>();
        }

        public Dictionary<FunctionDefinition, ProcedureBuilder> Functions { get; private set; }
        public Dictionary<string, Expression> Globals { get; private set; }
        public Dictionary<LocalId, DataType> Types { get; private set;}

        public Program BuildProgram(Module module)
        {
            DetermineTargetInformation(module.Targets);
            var address = Address.Create(program.Platform.PointerType, 0x1000);
            foreach (var entry in module.Entries)
            {
                address = RegisterEntry(entry, address);
            }

            foreach (var entry in module.Entries)
            {
                TranslateEntry(entry);
            }
            return program;
        }

        public void DetermineTargetInformation(List<TargetSpecification> targets)
        {
            IProcessorArchitecture arch = null;
            IPlatform platform = null;
            foreach (var target in targets)
            {
                var segs = target.Specification.Split('-');
                switch (target.Type)
                {
                case TokenType.datalayout:
                    break;
                case TokenType.triple:
                    arch = LoadArchitecture(segs[0]);
                    // vendor = segs[1];
                    platform = LoadPlatform(arch, segs[2]);
                    break;
                }
            }
            if (platform == null)
                platform = new DefaultPlatform(services, arch);
            this.program = new Program
            {
                Architecture = arch,
                Platform = platform,
                // No segment map.
            };
        }

        private IProcessorArchitecture LoadArchitecture(string llvmArchName)
        {
            string archName = null;
            switch (llvmArchName)
            {
            case "x86_64": archName = "x86-protected-64"; break;
            default:
                throw new NotImplementedException(string.Format(
                    "Support for LLVM type {0} not supported yet.",
                    llvmArchName));
            }
            return cfgSvc.GetArchitecture(archName);
        }

        private IPlatform LoadPlatform(IProcessorArchitecture arch, string llvmPlatformName)
        {
            string platformName = null;
            switch (llvmPlatformName)
            {
            case "linux": platformName = "elf-neutral"; break;
            default:
                return new DefaultPlatform(services, arch);
            }
            var el = cfgSvc.GetEnvironment(platformName);
            var platform = el.Load(services, arch);
            return platform;
        }
        

        public Address RegisterEntry(ModuleEntry entry, Address addr)
        {
            var global = entry as GlobalDefinition;
            if (global != null)
            {
                var glob = RegisterGlobal(global);
                program.GlobalFields.Fields.Add((int)addr.ToLinear(), glob.DataType, glob.Name);
                return addr + 1;
            }
            var fn = entry as FunctionDefinition;
            if (fn != null)
            {
                var proc = RegisterFunction(fn);
                program.Procedures.Add(addr, proc);
                this.Globals[fn.FunctionName] = new Core.Expressions.ProcedureConstant(
                    new Pointer(proc.Signature, program.Platform.PointerType.BitSize),
                    proc);
                return addr + 1;
            }
            var tydec = entry as TypeDefinition;
            if (fn != null)
            {
                RegisterTypeDefinition(tydec);
                return addr;
            }
            var decl = entry as Declaration;
            if (decl != null)
            {
                RegisterDeclaration(decl);
                return addr;
            }
            throw new NotImplementedException(entry.GetType().Name);
        }

        public Identifier RegisterGlobal(GlobalDefinition global)
        {
            var dt = TranslateType(global.Type);
            var id = new Identifier(global.Name, dt, new MemoryStorage());
            this.Globals.Add(global.Name, id);
            return id;
        }

        public void RegisterDeclaration(Declaration decl)
        {
            var dt = TranslateType(decl.Type);
            var id = new Identifier(decl.Name, dt, new MemoryStorage());
            this.Globals.Add(id.Name, id);
        }
        
        public Procedure RegisterFunction(FunctionDefinition fn)
        {
            var proc = new Procedure(program.Architecture, fn.FunctionName, new Frame(program.Platform.PointerType));
            var builder = new ProcedureBuilder(proc);
            Functions.Add(fn, builder);
            return proc;
        }

        public void RegisterTypeDefinition(TypeDefinition tydef)
        {
            Types.Add(tydef.Name, tydef.Type.Accept(new TypeTranslator(program.Platform.PointerType.BitSize)));
        }

        public void TranslateEntry(ModuleEntry entry)
        {
            if (entry is GlobalDefinition)
                return;
            var fn = entry as FunctionDefinition;
            if (fn != null)
            {
                TranslateFunction(fn);
                return;
            }
            var decl = entry as Declaration;
            if (decl != null)
            {
                //$TODO
                return;
            }
            throw new NotImplementedException(string.Format("TranslateEntry({0})", entry.GetType()));
        }


        public Reko.Core.Types.FunctionType TranslateSignature(
            LLVMType retType, 
            List<LLVMParameter> parameters, 
            ProcedureBuilder m)
        {
            var rt = TranslateType(retType);
            var sigRet = m.Procedure.Frame.CreateTemporary(rt);
            var sigParameters = new List<Identifier>();
            foreach (var param in parameters)
            {
                if (param.name == "...")
                {
                    throw new NotImplementedException("Varargs");
                }
                else
                {
                    var prefix = "arg";
                    var pt = TranslateType(param.Type);
                    var id = m.CreateLocalId(prefix, pt);
                    sigParameters.Add(id);
                }
            }
            return new FunctionType(sigRet, sigParameters.ToArray());
        }

        public DataType TranslateType(LLVMType type)
        {
            var xlat = new TypeTranslator(program.Platform.PointerType.BitSize);
            return type.Accept(xlat);
        }

        public void TranslateFunction(FunctionDefinition fn)
        {
            var m = Functions[fn];
            var proc = m.Procedure;
            var sig = TranslateSignature(fn.ResultType, fn.Parameters, m);
            proc.Signature = sig;

            m.EnsureBlock(null);
            var xlat = new LLVMInstructionTranslator(this, m);
            foreach (var instr in fn.Instructions)
            {
                instr.Accept(xlat);
            }
            xlat.ResolvePhis();

            //$TODO: this is hacky; after all procedures have been identified
            // there is no need for addresses anymore. Here, we synthesize 
            // a fake address for each procedure.
            var addr = Address.Create(
                program.Platform.PointerType,
                0x0010000 + (ulong)(program.Procedures.Count + 1) * 0x100);
            program.Procedures.Add(addr, m.Procedure);
        }
    }
}