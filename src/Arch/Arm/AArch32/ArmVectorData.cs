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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Arm
{
    public enum ArmVectorData
    {
        INVALID = 0,
        S16,
        S8,
        I16,
        I8,
        U8,
        U16,
        F32,
        S64,
        S32,
        I32,
        F64,
        U32,
        F64S32,
        F32S32,
        F16U16,
        S16F16,
        U16F16,
        F16S16,
        S32F32,
        F32U32,
        U32F32,
    }
}
