#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Arch.M68k;
using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Lib;
using Decompiler.Core.Operators;
using Decompiler.Core.Rtl;
using Decompiler.Core.Serialization;
using Decompiler.Core.Services;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Decompiler.Environments.AmigaOS
{
    /// <summary>
    /// Represents the AmigaOS platform.
    /// </summary>
    public class AmigaOSPlatform : Platform
    {
        private RtlInstructionMatcher a6Pattern;
        private Dictionary<int, SystemService> funcs;

        public AmigaOSPlatform(IServiceProvider services, IProcessorArchitecture arch)
            : base(services, arch)
        {
            this.a6Pattern = new RtlInstructionMatcher(
                new RtlCall(
                    new MemoryAccess(
                        new BinaryExpression(
                            Operator.IAdd,
                            PrimitiveType.Word32,
                            ExpressionMatcher.AnyId("addrReg"),
                            ExpressionMatcher.AnyConstant("offset")),
                        PrimitiveType.Word32),
                    4,
                    RtlClass.Transfer));
        }

        public override BitSet CreateImplicitArgumentRegisters()
        {
            var bitset = Architecture.CreateRegisterBitset();
            Registers.a7.SetAliases(bitset, true);
            return bitset;
        }

        public override SystemService FindService(int vector, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        public override SystemService FindService(RtlInstruction rtl, ProcessorState state)
        {
            if (!a6Pattern.Match(rtl))
                return null;
            var reg = ((Identifier) a6Pattern.CapturedExpressions("addrReg")).Storage as RegisterStorage;
            var offset = ((Constant) a6Pattern.CapturedExpressions("offset")).ToInt32();
            if (reg != Registers.a6)
                return null;
            if (funcs == null)
                funcs = LoadFuncs();
            SystemService svc;
            return funcs.TryGetValue(offset, out svc) ? svc : null;
        }

        public override string DefaultCallingConvention
        {
            get { return ""; }
        }


        public override ProcedureBase GetTrampolineDestination(ImageReader imageReader, IRewriterHost host)
        {
            return null;
        }

        public override ExternalProcedure LookupProcedureByName(string moduleName, string procName)
        {
            throw new NotImplementedException();
        }

        public override Address MakeAddressFromConstant(Constant c)
        {
            return Address.Ptr32(c.ToUInt32());
        }

        private Dictionary<int, SystemService> LoadFuncs()
        {
            var fsSvc = Services.RequireService<IFileSystemService>();
            var sser = Architecture.CreateProcedureSerializer(
                new TypeLibraryLoader(Architecture,true),
                DefaultCallingConvention);

            using (var rdr = new StreamReader(fsSvc.CreateFileStream("exec.funcs", FileMode.Open, FileAccess.Read)))
            {
                var fpp = new FuncsFileParser((M68kArchitecture) this.Architecture, rdr);
                fpp.Parse();
                return fpp.FunctionsByA6Offset.Values
                    .Select(amiSvc => new SystemService
                    {
                        Name = amiSvc.Name,
                        SyscallInfo = new SyscallInfo
                        {
                            Vector = amiSvc.Offset,
                        },
                        Signature = sser.Deserialize(amiSvc.Signature, Architecture.CreateFrame()),   //$BUGBUG: catch dupes?   
                        Characteristics = new ProcedureCharacteristics { }
                    })
                    .ToDictionary(de => de.SyscallInfo.Vector, de => de);
            };
        }
    }
}
