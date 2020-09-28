#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.LLVM
{
    public interface InstructionVisitor<T>
    {
        T VisitAlloca(Alloca alloca);
        T VisitBinary(Binary bin);
        T VisitBitwiseBinary(BitwiseBinary bit);
        T VisitBr(BrInstr br);
        T VisitCall(LLVMCall call);
        T VisitCmp(CmpInstruction cmp);
        T VisitConversion(Conversion conv);
        T VisitExtractvalue(Extractvalue ext);
        T VisitFence(Fence fence);
        T VisitGetelementptr(GetElementPtr get);
        T VisitLoad(Load load);
        T VisitPhi(PhiInstruction phi);
        T VisitRet(RetInstr ret);
        T VisitSelect(Select select);
        T VisitStore(Store store);
        T VisitSwitch(Switch sw);
        T VisitUnreachable(Unreachable unreachable);
    }
}
