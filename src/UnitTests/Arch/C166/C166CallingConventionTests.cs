#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using System.Text;

namespace Reko.UnitTests.Arch.C166
{
    class C166CallingConventionTests
    {
        //$TODO: implement this!
        //        C functions pass up to five parameters in registers(R8-R12). Bit parameters are passed in R15.Parameters that do not fit into R8-R12 are passed on the user stack and are accessed using [R0+#disp] instructions.


        //      void func2(
        //int b,        /* 1st parameter passed in R8  */
        //int c,        /* 2nd parameter passed in R9  */
        //int near *d,  /* 3rd parameter passed in R10 */
        //char e,       /* 4th parameter passed in R11 */
        //char f)       /* 5th parameter passed in R12 */

        // void func4 (
        //      double k,     /* 1st parameter passed in R8/R9/R10/R11 */
        //long j)       /* 2nd parameter LSW passed in R12 */
        //              /* MSW passed on the user stack */
        //The above function has two arguments.The first argument uses four of the five registers. The LSW of the second argument is passed in R12 and the MSW is passed on the user stack.


        //        void func6(
        //          char o,       /* 1st parameter passed in R8    */
        //          bit p,               /* 2nd parameter passed in R15.0 */
        //          char q,       /* 3rd parameter passed in R9    */
        //          bit r)               /* 4th parameter passed in R15.1 */
        //The above function has four arguments(two are bits) that are passed in registers and in R15(for the bits).

//        bit R4.0	Single bit returned in R4.0.
//char,
//unsigned char RL4 Single byte type returned in RL4.
//int,
//unsigned int,
//near pointer    R4 Two byte (16-bit) type returned in R4.
//long,
//unsigned long,
//far pointer,
//huge pointer R4 & R5 LSB in R4, MSB in R5.
//float R4 & R5	32-Bit IEEE format.
//double R4-R7	64-Bit IEEE format.
    }
}
