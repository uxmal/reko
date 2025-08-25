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

using System.Diagnostics.CodeAnalysis;
using System.Text;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Types;

namespace Reko.Arch.NatSemi;

public class Ns32kArchitecture : ProcessorArchitecture
{
    public Ns32kArchitecture(IServiceProvider services, string archId, Dictionary<string, object> options)
        : base(services, archId, options, Registers.RegistersByName, Registers.RegistersByDomain)
    {
        this.CarryFlag = Registers.C;
        this.Endianness = EndianServices.Little;
        this.CodeMemoryGranularity = 8;
        this.FramePointerType = PrimitiveType.Ptr32;
        this.InstructionBitSize = 8;
        this.StackRegister = Registers.SP0;
        this.PointerType = PrimitiveType.Ptr32;
        this.WordWidth = PrimitiveType.Word32;
    }

    public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader)
    {
        return new Ns32kDisassembler(this, imageReader);
    }

    public override IEqualityComparer<MachineInstruction>? CreateInstructionComparer(Normalize norm)
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
    {
        throw new NotImplementedException();
    }

    public override ProcessorState CreateProcessorState()
    {
        return new DefaultProcessorState(this);
    }

    public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
    {
        return new Ns32kRewriter(this, rdr, state, binder, host);
    }

    public override FlagGroupStorage? GetFlagGroup(RegisterStorage flagRegister, uint grf)
    {
        var fl = new FlagGroupStorage(flagRegister, grf, this.GrfToString(flagRegister, "", grf));
        return fl;
    }

    public override FlagGroupStorage? GetFlagGroup(string name)
    {
        throw new NotImplementedException();
    }

    public override SortedList<string, int> GetMnemonicNames()
    {
        throw new NotImplementedException();
    }

    public override int? GetMnemonicNumber(string name)
    {
        throw new NotImplementedException();
    }

    public override RegisterStorage[] GetRegisters()
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<FlagGroupStorage> GetSubFlags(FlagGroupStorage flags)
    {
        uint grf = flags.FlagGroupBits;
        if ((grf & Registers.C.FlagGroupBits) != 0) yield return Registers.C;
        if ((grf & Registers.F.FlagGroupBits) != 0) yield return Registers.F;
        if ((grf & Registers.I.FlagGroupBits) != 0) yield return Registers.I;
        if ((grf & Registers.L.FlagGroupBits) != 0) yield return Registers.L;
        if ((grf & Registers.N.FlagGroupBits) != 0) yield return Registers.N;
        if ((grf & Registers.P.FlagGroupBits) != 0) yield return Registers.P;
        if ((grf & Registers.S.FlagGroupBits) != 0) yield return Registers.S;
        if ((grf & Registers.T.FlagGroupBits) != 0) yield return Registers.T;
        if ((grf & Registers.U.FlagGroupBits) != 0) yield return Registers.U;
        if ((grf & Registers.V.FlagGroupBits) != 0) yield return Registers.V;
        if ((grf & Registers.Z.FlagGroupBits) != 0) yield return Registers.Z;
    }

    public override string GrfToString(RegisterStorage flagRegister, string prefix, uint grf)
    {
        StringBuilder sb = new StringBuilder();
        if ((grf & Registers.C.FlagGroupBits) != 0) sb.Append(Registers.S.Name);
        if ((grf & Registers.F.FlagGroupBits) != 0) sb.Append(Registers.S.Name);
        if ((grf & Registers.I.FlagGroupBits) != 0) sb.Append(Registers.S.Name);
        if ((grf & Registers.L.FlagGroupBits) != 0) sb.Append(Registers.S.Name);
        if ((grf & Registers.N.FlagGroupBits) != 0) sb.Append(Registers.S.Name);
        if ((grf & Registers.P.FlagGroupBits) != 0) sb.Append(Registers.S.Name);
        if ((grf & Registers.S.FlagGroupBits) != 0) sb.Append(Registers.S.Name);
        if ((grf & Registers.T.FlagGroupBits) != 0) sb.Append(Registers.S.Name);
        if ((grf & Registers.U.FlagGroupBits) != 0) sb.Append(Registers.S.Name);
        if ((grf & Registers.V.FlagGroupBits) != 0) sb.Append(Registers.S.Name);
        if ((grf & Registers.Z.FlagGroupBits) != 0) sb.Append(Registers.S.Name);
        return sb.ToString();
    }

    public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
    {
        return Address.Ptr32(c.ToUInt32());
    }

    public override Address? ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState? state)
    {
        throw new NotImplementedException();
    }

    public override bool TryParseAddress(string? txtAddr, [MaybeNullWhen(false)] out Address addr)
    {
        return Address.TryParse32(txtAddr, out addr);
    }
}
