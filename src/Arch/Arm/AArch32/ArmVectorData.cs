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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Arm.AArch32
{
    public enum ArmVectorData
    {
        INVALID = 0,
        S8,
        S16,
        S32,
        S64,

        I8,
        I16,
        I32,
        I64,

        U8,
        U16,
        U32,
        U64,

        F16,
        F32,
        F64,

        S16F16,
        S32F16,
        S32F32,
        S32F64,
     
        U16F16,
        U32F16,
        U32F32,
        U32F64,

        F16F32,
        F16F64,
        F16S16,
        F16S32,
        F16U16,
        F16U32,

        F32S32,
        F32F16,
        F32F64,
        F32U32,

        F64S32,
        F64U32,
        F64F16,
        F64F32,
    }
}
