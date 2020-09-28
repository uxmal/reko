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

namespace Reko.UnitTests.Fragments.Regressions
{
    public static class Reg00001
    {
        public static string Text =
                @"
mane proc
        call testproc
        mov [0x02000000],eax
        ret
    endp

testproc proc

    mov edi,edi
    push ebp	      
    mov ebp,esp	      
    mov eax,DWORD PTR [ebp+8]	      
    mov ecx,DWORD PTR [eax+0x3c]      
    add ecx,eax	      
    movzx eax,WORD PTR [ecx+0x14]	;char* dst = arg[0]
    push ebx	      
    push esi	      
    movzx esi,WORD PTR [ecx+0x6]	      
    xor edx,edx	      
    push edi	      
    lea eax,[eax+ecx*1+18]
    test esi,esi	      
    jbe loc_0000003d 
	      
    mov edi,DWORD PTR [ebp+0xc]	      

loc_00000025:
        mov ecx,DWORD PTR [eax+0xc]	      
        cmp edi,ecx	      
        jb loc_00000035 
	      
        mov ebx,DWORD PTR [eax+0x8]	      
        add ebx,ecx	      
        cmp edi,ebx	      
        jb loc_0000003f

loc_00000035:
        inc edx	      
        add eax,0x28	      
        cmp edx,esi	      
        jb loc_00000025 
	      
loc_0000003d:
xor eax,eax	      

loc_0000003f:
pop edi	      
pop esi	      
pop ebx	      
pop ebp
ret
endp
";
    }
}
