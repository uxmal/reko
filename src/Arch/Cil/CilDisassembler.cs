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

using Reko.Core;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace Reko.Arch.Cil
{
    public class CilDisassembler : DisassemblerBase<CilInstruction, OpCode>
    {
        private EndianImageReader rdr;
        private CilInstruction instr;

        public CilDisassembler(EndianImageReader rdr)
        {
            this.rdr = rdr;
        }

        public override CilInstruction DisassembleInstruction()
        {
            if (!rdr.IsValid)
                return null;
            var addr = rdr.Address;
            var opcode = rdr.ReadByte();
            instr = new CilInstruction
            {
                Address = addr,
                Operands = MachineInstruction.NoOperands,
            };
            switch (opcode)
            {
            #region OPCODES
            case 0x00: instr.Opcode = OpCodes.Nop; break;
            case 0x01: instr.Opcode = OpCodes.Break; break;
            case 0x02: instr.Opcode = OpCodes.Ldarg_0; break;
            case 0x03: instr.Opcode = OpCodes.Ldarg_1; break;
            case 0x04: instr.Opcode = OpCodes.Ldarg_2; break;
            case 0x05: instr.Opcode = OpCodes.Ldarg_3; break;
            case 0x06: instr.Opcode = OpCodes.Ldloc_0; break;
            case 0x07: instr.Opcode = OpCodes.Ldloc_1; break;
            case 0x08: instr.Opcode = OpCodes.Ldloc_2; break;
            case 0x09: instr.Opcode = OpCodes.Ldloc_3; break;
            case 0x0A: instr.Opcode = OpCodes.Stloc_0; break; //  Pop a value from stack into local variable 0. 
            case 0x0B: instr.Opcode = OpCodes.Stloc_1; break; //Pop a value from stack into local variable 1. 
            case 0x0C: instr.Opcode = OpCodes.Stloc_2; break;   //Pop a value from stack into local variable 2. 
            case 0x0D: instr.Opcode = OpCodes.Stloc_3; break;   //Pop a value from stack into local variable 3. 
            case 0x0E:                                          //Load argument numbered num onto the stack, short form. 
                instr.Opcode = OpCodes.Ldarg_S; // <uint8 (num)>
                instr.Operand = rdr.ReadByte();
                break;
            case 0x0F:
                instr.Opcode = OpCodes.Ldarga_S;  // <uint8 (argNum)>
                instr.Operand = rdr.ReadByte();
                break;

            //Fetch the address of argument argNum, short form. 
            case 0x10:

                instr.Opcode = OpCodes.Starg_S; // .s <uint8 (num)>
                instr.Operand = rdr.ReadByte();
                break;

            //Store value to the argument numbered num, short form. 
            case 0x11:

                instr.Opcode = OpCodes.Ldloc_S; // <uint8 (indx)>
                instr.Operand = rdr.ReadByte();
                break;

            //Load local variable of index indx onto stack, short form. 
            case 0x12:
                instr.Opcode = OpCodes.Ldloca_S;    // .s <uint8 (indx)>
                instr.Operand = rdr.ReadByte();
                break;

            //Load address of local variable with index indx, short form. 
            case 0x13:

                instr.Opcode = OpCodes.Stloc_S; // <uint8 (indx)>
                instr.Operand = rdr.ReadByte();
                break;

            //Pop a value from stack into local variable indx, short form. 
            case 0x14: instr.Opcode = OpCodes.Ldnull; break;    //Push a null reference on the stack. 
            case 0x15: instr.Opcode = OpCodes.Ldc_I4_M1; break; //Push -1 onto the stack as int32. 
            case 0x16: instr.Opcode = OpCodes.Ldc_I4_0; break;
            case 0x17: instr.Opcode = OpCodes.Ldc_I4_1; break;  //Push 1 onto the stack as int32. 
            case 0x18: instr.Opcode = OpCodes.Ldc_I4_2; break;   //Push 2 onto the stack as int32. 
            case 0x19: instr.Opcode = OpCodes.Ldc_I4_3; break;   //Push 3 onto the stack as int32. 
            case 0x1A:
                instr.Opcode = OpCodes.Ldc_I4_4;
                break;

            //Push 4 onto the stack as int32. 
            case 0x1B:
                instr.Opcode = OpCodes.Ldc_I4_5;
                break;

            //Push 5 onto the stack as int32. 
            case 0x1C:
                instr.Opcode = OpCodes.Ldc_I4_6;
                break;

            //Push 6 onto the stack as int32. 
            case 0x1D:
                instr.Opcode = OpCodes.Ldc_I4_7;
                break;

            //Push 7 onto the stack as int32. 
            case 0x1E:
                instr.Opcode = OpCodes.Ldc_I4_8;
                break;

            //Push 8 onto the stack as int32. 
            case 0x1F:
                instr.Opcode = OpCodes.Ldc_I4_S;    // <int8 (num)>
                instr.Operand = rdr.ReadSByte();
                break;

            //Push num onto the stack as int32, short form. 
            case 0x20:
                instr.Opcode = OpCodes.Ldc_I4;// <int32 (num)>
                instr.Operand = rdr.ReadLeInt32();
                break;

            //Push num of type int32 onto the stack as int32. 
            case 0x21:
                instr.Opcode = OpCodes.Ldc_I8;// <int64 (num)>
                instr.Operand = rdr.ReadLeInt64();
                break;

            //Push num of type int64 onto the stack as int64. 
            case 0x22:
                instr.Opcode = OpCodes.Ldc_R4;// <float32 (num)>
                instr.Operand = rdr.ReadLeUInt64();
                break;

            //Push num of type float32 onto the stack as F. 
            case 0x23:
                instr.Opcode = OpCodes.Ldc_R8; // <float64 (num)>
                instr.Operand = rdr.ReadLeUInt64();
                break;

            //Push num of type float64 onto the stack as F. 
            case 0x25: instr.Opcode = OpCodes.Dup; break;

            //Duplicate the value on the top of the stack. 
            case 0x26: instr.Opcode = OpCodes.Pop; break; //Pop value from the stack. 
            case 0x27:
                instr.Opcode = OpCodes.Jmp;// <method>
                break;

            //Exit current method and jump to the specified method. 
            case 0x28:
                instr.Opcode = OpCodes.Call;// <method>
                break;

            //Call method described by method. 
            case 0x29:
                instr.Opcode = OpCodes.Calli;// <callsitedescr>
                break;

            //Call method indicated on the stack with arguments described by callsitedescr. 
            case 0x2A: instr.Opcode = OpCodes.Ret; break; //Return from method, possibly with a value. 
            case 0x2B:
                instr.Opcode = OpCodes.Br_S; // <int8 (target)>
                break;

            //Branch to target, short form. 
            case 0x2C:
                instr.Opcode = OpCodes.Brfalse_S;// <int8 (target)>
                break;

            //Branch to target if value is zero (false), short form. 
            case 0x2D:
                instr.Opcode = OpCodes.Brtrue_S; // <int8 (target)>
                break;

            //Branch to target if value is non-zero (true), short form. 
            case 0x2E:
                instr.Opcode = OpCodes.Beq_S;  //.s <int8 (target)>
                break;

            //Branch to target if equal, short form. 
            case 0x2F:
                instr.Opcode = OpCodes.Bge_S; //.s <int8 (target)>
                break;

            //Branch to target if greater than or equal to, short form. 
            case 0x30:
                instr.Opcode = OpCodes.Bgt_S;   // .s <int8 (target)>
                break;

            //Branch to target if greater than, short form. 
            case 0x31:
                instr.Opcode = OpCodes.Ble_S; // .s <int8 (target)>
                break;

            //Branch to target if less than or equal to, short form. 
            case 0x32:
                instr.Opcode = OpCodes.Blt_S; //.s <int8 (target)>
                break;

            //Branch to target if less than, short form. 
            case 0x33:
                instr.Opcode = OpCodes.Bne_Un_S;//.un.s <int8 (target)>
                break;

            //Branch to target if unequal or unordered, short form. 
            case 0x34:
                instr.Opcode = OpCodes.Bge_Un_S;// <int8 (target)>
                break;

            //Branch to target if greater than or equal to (unsigned or unordered), short form 
            case 0x35:
                instr.Opcode = OpCodes.Bgt_Un_S;// <int8 (target)>
                break;

            //Branch to target if greater than (unsigned or unordered), short form. 
            case 0x36:
                instr.Opcode = OpCodes.Ble_Un_S;// <int8 (target)>
                break;

            //Branch to target if less than or equal to (unsigned or unordered), short form 
            case 0x37:
                instr.Opcode = OpCodes.Blt_Un_S;//.un.s <int8 (target)>
                break;

            //Branch to target if less than (unsigned or unordered), short form. 
            case 0x38:
                instr.Opcode = OpCodes.Br;// <int32 (target)>
                break;

            //Branch to target. 
            case 0x39:
                instr.Opcode = OpCodes.Brfalse;// <int32 (target)>
                break;

            //Branch to target if value is zero (alias for brfalse). 
            case 0x3A:
                instr.Opcode = OpCodes.Brtrue;// <int32 (target)>
                break;

            //Branch to target if value is non-zero (true). 
            case 0x3B:
                instr.Opcode = OpCodes.Beq;// <int32 (target)>
                break;

            //Branch to target if equal. 
            case 0x3C:
                instr.Opcode = OpCodes.Bge;// <int32 (target)>
                break;

            //Branch to target if greater than or equal to. 
            case 0x3D:
                instr.Opcode = OpCodes.Bgt; // <int32 (target)>
                break;

            //Branch to target if greater than. 
            case 0x3E:
                instr.Opcode = OpCodes.Ble; // <int32 (target)>
                break;

            //Branch to target if less than or equal to. 
            case 0x3F:
                instr.Opcode = OpCodes.Blt;// <int32 (target)>
                break;

            //Branch to target if less than. 
            case 0x40:
                instr.Opcode = OpCodes.Bne_Un;// <int32 (target)>
                break;

            //Branch to target if unequal or unordered. 
            case 0x41:
                instr.Opcode = OpCodes.Bge_Un;// <int32 (target)>
                break;

            //Branch to target if greater than or equal to (unsigned or unordered). 
            case 0x42:
                instr.Opcode = OpCodes.Bgt_Un;// <int32 (target)>
                break;

            //Branch to target if greater than (unsigned or unordered). 
            case 0x43:
                instr.Opcode = OpCodes.Ble_Un;// <int32 (target)>
                break;

            //Branch to target if less than or equal to (unsigned or unordered). 
            case 0x44:
                instr.Opcode = OpCodes.Blt_Un;// <int32 (target)>
                break;

            //Branch to target if less than (unsigned or unordered). 
            case 0x45:
                instr.Opcode = OpCodes.Switch;// <uint32, int32,int32 (t1..tN)>
                break;

            //Jump to one of n values. 
            case 0x46: instr.Opcode = OpCodes.Ldind_I1; break; //Indirect load value of type int8 as int32 on the stack. 
            case 0x47: instr.Opcode = OpCodes.Ldind_U1; break; //Indirect load value of type unsigned int8 as int32 on the stack 
            case 0x48: instr.Opcode = OpCodes.Ldind_I2; break; //Indirect load value of type int16 as int32 on the stack. 
            case 0x49: instr.Opcode = OpCodes.Ldind_U2; break; //Indirect load value of type unsigned int16 as int32 on the stack 
            case 0x4A: instr.Opcode = OpCodes.Ldind_I4; break;  //Indirect load value of type int32 as int32 on the stack. 
            case 0x4B: instr.Opcode = OpCodes.Ldind_U4; break;

            //Indirect load value of type unsigned int32 as int32 on the stack 
            case 0x4C:
                instr.Opcode = OpCodes.Ldind_I8;
                break;

            //Indirect load value of type int64 as int64 on the stack. 
            case 0x4D:

                instr.Opcode = OpCodes.Ldind_I;
                break;

            //Indirect load value of type native int as native int on the stack 
            case 0x4E:

                instr.Opcode = OpCodes.Ldind_R4;
                break;

            //Indirect load value of type float32 as F on the stack. 
            case 0x4F:

                instr.Opcode = OpCodes.Ldind_R8;
                break;

            //Indirect load value of type float64 as F on the stack. 
            case 0x50:

                instr.Opcode = OpCodes.Ldind_Ref;
                break;

            //Indirect load value of type object ref as O on the stack. 
            case 0x51:

                instr.Opcode = OpCodes.Stind_Ref;
                break;

            //Store value of type object ref (type O) into memory at address 
            case 0x52:

                instr.Opcode = OpCodes.Stind_I1;
                break;

            //Store value of type int8 into memory at address 
            case 0x53:

                instr.Opcode = OpCodes.Stind_I2;
                break;

            //Store value of type int16 into memory at address 
            case 0x54:

                instr.Opcode = OpCodes.Stind_I4;
                break;

            //Store value of type int32 into memory at address 
            case 0x55: instr.Opcode = OpCodes.Stind_I8; break;  // Store value of type int64 into memory at address 
            case 0x56:

                instr.Opcode = OpCodes.Stind_R4;
                break;

            //Store value of type float32 into memory at address 
            case 0x57:

                instr.Opcode = OpCodes.Stind_R8;
                break;

            //Store value of type float64 into memory at address 
            case 0x58:

                instr.Opcode = OpCodes.Add;
                break;

            //Add two values, returning a new value. 
            case 0x59:

                instr.Opcode = OpCodes.Sub;
                break;

            //Subtract value2 from value1, returning a new value. 
            case 0x5A: instr.Opcode = OpCodes.Mul; break; //Multiply values. 
            case 0x5B: instr.Opcode = OpCodes.Div; break;//Divide two values to return a quotient or floating-point result. 
            case 0x5C: instr.Opcode = OpCodes.Div_Un; break; //Divide two values, unsigned, returning a quotient. 
            case 0x5D: instr.Opcode = OpCodes.Rem; break; //Remainder when dividing one value by another. 
            case 0x5E:

                instr.Opcode = OpCodes.Rem_Un;
                break;

            //Remainder when dividing one unsigned value by another. 
            case 0x5F:
                instr.Opcode = OpCodes.And;
                break;

            //Bitwise AND of two integral values, returns an integral value. 
            case 0x60:

                instr.Opcode = OpCodes.Or;
                break;

            //Bitwise OR of two integer values, returns an integer. 
            case 0x61:

                instr.Opcode = OpCodes.Xor;
                break;

            //Bitwise XOR of integer values, returns an integer. 
            case 0x62:

                instr.Opcode = OpCodes.Shl;
                break;

            //Shift an integer left (shifting in zeros), return an integer. 
            case 0x63: instr.Opcode = OpCodes.Shr; break;  //Shift an integer right (shift in sign), return an integer. 
            case 0x64: instr.Opcode = OpCodes.Shr_Un; break;    //Shift an integer right (shift in zero), return an integer. 
            case 0x65: instr.Opcode = OpCodes.Neg; break;       //Negate value. 
            case 0x66: instr.Opcode = OpCodes.Not; break;       //Bitwise complement (logical not). 
            case 0x67: instr.Opcode = OpCodes.Conv_I1; break;   //Convert to int8, pushing int32 on stack. 
            case 0x68: instr.Opcode = OpCodes.Conv_I2; break;   //Convert to int16, pushing int32 on stack. 
            case 0x69: instr.Opcode = OpCodes.Conv_I4; break;   //Convert to int32, pushing int32 on stack. 
            case 0x6A: instr.Opcode = OpCodes.Conv_I8; break;   //Convert to int64, pushing int64 on stack. 
            case 0x6B: instr.Opcode = OpCodes.Conv_R4; break; //Convert to float32, pushing F on stack. 
            case 0x6C: instr.Opcode = OpCodes.Conv_R8; break; //Convert to float64, pushing F on stack. 
            case 0x6D: instr.Opcode = OpCodes.Conv_U4; break; //Convert to unsigned int32, pushing int32 on stack. 
            case 0x6E: instr.Opcode = OpCodes.Conv_U8; break; //Convert to unsigned int64, pushing int64 on stack. 
            case 0x6F: instr.Opcode = OpCodes.Callvirt; // <method>
                break;

            //Call a method associated with an object. Object model instruction 
            case 0x70: instr.Opcode = OpCodes.Cpobj; // <typeTok>
                break;
            //Copy a value type from src to dest. Object model instruction 
            case 0x71:
                instr.Opcode = OpCodes.Ldobj;// <typeTok>
                break;
            //Copy the value stored at address src to the stack. Object model instruction 
            case 0x72:

                instr.Opcode = OpCodes.Ldstr;// <string>
                break;

            //Push a string object for the literal string. Object model instruction 
            case 0x73:

                instr.Opcode = OpCodes.Newobj;// <ctor>
                break;

            //Allocate an uninitialized object or value type and call ctor. Object model instruction 
            case 0x74:

                instr.Opcode = OpCodes.Castclass; // <class>
                break;

            //Cast obj to class. Object model instruction 
            case 0x75:

                instr.Opcode = OpCodes.Isinst; //<class>
                break;

            //Test if obj is an instance of class, returning null or an instance of that class or interface. Object model instruction 
            case 0x76: instr.Opcode = OpCodes.Conv_R_Un; break; //Convert unsigned integer to floating-point, pushing F on stack. 
            case 0x79: instr.Opcode = OpCodes.Unbox; //<valuetype>
                break;                              //Extract a value-type from obj, its boxed representation. Object model instruction 
            case 0x7A:
                instr.Opcode = OpCodes.Throw;
                break;

            //Throw an exception. Object model instruction 
            case 0x7B:
                instr.Opcode = OpCodes.Ldfld; // <field>
                break;

            //Push the value of field of object (or value type) obj, onto the stack. Object model instruction 
            case 0x7C:
                instr.Opcode = OpCodes.Ldflda; // <field>
                break;

            //Push the address of field of object obj on the stack. Object model instruction 
            case 0x7D:
                instr.Opcode = OpCodes.Stfld;  //<field>
                break;

            //Replace the value of field of the object obj with value. Object model instruction 
            case 0x7E:
                instr.Opcode = OpCodes.Ldsfld;// <field>
                break;

            //Push the value of field on the stack. Object model instruction 
            case 0x7F:
                instr.Opcode = OpCodes.Ldsflda;// <field>
                break;

            //Push the address of the static field, field, on the stack. Object model instruction 
            case 0x80:
                instr.Opcode = OpCodes.Stsfld; // <field>
                break;

            //Replace the value of field with val. Object model instruction 
            case 0x81:
                instr.Opcode = OpCodes.Stobj; //<typeTok>
                break;

            //Store a value of type typeTok at an address. Object model instruction 
            case 0x82: instr.Opcode = OpCodes.Conv_Ovf_I1_Un; break;  //Convert unsigned to an int8 (on the stack as int32) and throw an exception on overflow. 
            case 0x83:
                instr.Opcode = OpCodes.Conv_Ovf_I2_Un;
                break;

            //Convert unsigned to an int16 (on the stack as int32) and throw an exception on overflow. 
            case 0x84: instr.Opcode = OpCodes.Conv_Ovf_I4_Un; break; //Convert unsigned to an int32 (on the stack as int32) and throw an exception on overflow. 
            case 0x85: instr.Opcode = OpCodes.Conv_Ovf_I8_Un; break; //Convert unsigned to an int64 (on the stack as int64) and throw an exception on overflow. 
            case 0x86: instr.Opcode = OpCodes.Conv_Ovf_U1_Un; break; //Convert unsigned to an unsigned int8 (on the stack as int32) and throw an exception on overflow. 
            case 0x87:

                instr.Opcode = OpCodes.Conv_Ovf_I2_Un;
                break;

            //Convert unsigned to an unsigned int16 (on the stack as int32) and throw an exception on overflow. 
            case 0x88:

                instr.Opcode = OpCodes.Conv_Ovf_U4_Un;
                break;

            //Convert unsigned to an unsigned int32 (on the stack as int32) and throw an exception on overflow. 
            case 0x89:

                instr.Opcode = OpCodes.Conv_Ovf_U8_Un;
                break;

            //Convert unsigned to an unsigned int64 (on the stack as int64) and throw an exception on overflow. 
            case 0x8A:

                instr.Opcode = OpCodes.Conv_Ovf_I_Un;
                break;

            //Convert unsigned to a native int (on the stack as native int) and throw an exception on overflow. 
            case 0x8B:

                instr.Opcode = OpCodes.Conv_Ovf_U_Un;
                break;

            //Convert unsigned to a native unsigned int (on the stack as native int) and throw an exception on overflow. 
            case 0x8C:

                instr.Opcode = OpCodes.Box;// <typeTok>
                break;

            //Convert a boxable value to its boxed form Object model instruction 
            case 0x8D:

                instr.Opcode = OpCodes.Newarr;// <etype>
                break;

            //Create a new array with elements of type etype. Object model instruction 
            case 0x8E:

                instr.Opcode = OpCodes.Ldlen;
                break;

            //Push the length (of type native unsigned int) of array on the stack. Object model instruction 
            case 0x8F:

                instr.Opcode = OpCodes.Ldelema;// <class>
                break;

            //Load the address of element at index onto the top of the stack. Object model instruction 
            case 0x90:

                instr.Opcode = OpCodes.Ldelem_I1;
                break;

            //Load the element with type int8 at index onto the top of the stack as an int32. Object model instruction 
            case 0x91:

                instr.Opcode = OpCodes.Ldelem_U1;
                break;

            //Load the element with type unsigned int8 at index onto the top of the stack as an int32. Object model instruction 
            case 0x92:

                instr.Opcode = OpCodes.Ldelem_I2;
                break;

            //Load the element with type int16 at index onto the top of the stack as an int32. Object model instruction 
            case 0x93:

                instr.Opcode = OpCodes.Ldelem_U2;
                break;

            //Load the element with type unsigned int16 at index onto the top of the stack as an int32. Object model instruction 
            case 0x94:

                instr.Opcode = OpCodes.Ldelem_I4;
                break;

            //Load the element with type int32 at index onto the top of the stack as an int32. Object model instruction 
            case 0x95:

                instr.Opcode = OpCodes.Ldelem_U4;
                break;

            //Load the element with type unsigned int32 at index onto the top of the stack as an int32. Object model instruction 
            case 0x96:

                instr.Opcode = OpCodes.Ldelem_I8;
                break;

            //Load the element with type unsigned int64 at index onto the top of the stack as an int64 (alias for ldelem.i8). Object model instruction 
            case 0x97:

                instr.Opcode = OpCodes.Ldelem_I;
                break;

            //Load the element with type native int at index onto the top of the stack as a native int. Object model instruction 
            case 0x98:

                instr.Opcode = OpCodes.Ldelem_R4;
                break;

            //Load the element with type float32 at index onto the top of the stack as an F Object model instruction 
            case 0x99:

                instr.Opcode = OpCodes.Ldelem_R8;
                break;

            //Load the element with type float64 at index onto the top of the stack as an F. Object model instruction 
            case 0x9A:

                instr.Opcode = OpCodes.Ldelem_Ref;
                break;

            //Load the element at index onto the top of the stack as an O. The type of the O is the same as the element type of the array pushed on the CIL stack. Object model instruction 
            case 0x9B:

                instr.Opcode = OpCodes.Stelem_I;
                break;

            //Replace array element at index with the i value on the stack. Object model instruction 
            case 0x9C:

                instr.Opcode = OpCodes.Stelem_I1;
                break;

            //Replace array element at index with the int8 value on the stack. Object model instruction 
            case 0x9D:

                instr.Opcode = OpCodes.Stelem_I2;
                break;

            //Replace array element at index with the int16 value on the stack. Object model instruction 
            case 0x9E:

                instr.Opcode = OpCodes.Stelem_I4;
                break;

            //Replace array element at index with the int32 value on the stack. Object model instruction 
            case 0x9F: instr.Opcode = OpCodes.Stelem_I8; break;  //Replace array element at index with the int64 value on the stack. Object model instruction 
            case 0xA0: instr.Opcode = OpCodes.Stelem_R4; break;  //Replace array element at index with the float32 value on the stack. Object model instruction 
            case 0xA1: instr.Opcode = OpCodes.Stelem_R8; break; //Replace array element at index with the float64 value on the stack. Object model instruction 
            case 0xA2: instr.Opcode = OpCodes.Stelem_Ref; break; //Replace array element at index with the ref value on the stack. Object model instruction 
            case 0xA3:
                instr.Opcode = OpCodes.Ldelem;// <typeTok>
                break;

            //Load the element at index onto the top of the stack. Object model instruction 
            case 0xA4:
                instr.Opcode = OpCodes.Stelem;// <typeTok>
                break;

            //Replace array element at index with the value on the stack Object model instruction 
            case 0xA5: instr.Opcode = OpCodes.Unbox_Any;//<typeTok>
                break;

            //Extract a value-type from obj, its boxed representation Object model instruction 
            case 0xB3: instr.Opcode = OpCodes.Conv_Ovf_I1; break;

            //Convert to an int8 (on the stack as int32) and throw an exception on overflow. 
            case 0xB4: instr.Opcode = OpCodes.Conv_Ovf_U1; break; //Convert to an unsigned int8 (on the stack as int32) and throw an exception on overflow. 
            case 0xB5: instr.Opcode = OpCodes.Conv_Ovf_I2; break; //Convert to an int16 (on the stack as int32) and throw an exception on overflow. 
            case 0xB6: instr.Opcode = OpCodes.Conv_Ovf_U2; break; //Convert to an unsigned int16 (on the stack as int32) and throw an exception on overflow. 
            case 0xB7: instr.Opcode = OpCodes.Conv_Ovf_I4; break; //Convert to an int32 (on the stack as int32) and throw an exception on overflow. 
            case 0xB8: instr.Opcode = OpCodes.Conv_Ovf_U4; break; //Convert to an unsigned int32 (on the stack as int32) and throw an exception on overflow. 
            case 0xB9: instr.Opcode = OpCodes.Conv_Ovf_I8; break; //Convert to an int64 (on the stack as int64) and throw an exception on overflow. 
            case 0xBA: instr.Opcode = OpCodes.Conv_Ovf_U8; break; //Convert to an unsigned int64 (on the stack as int64) and throw an exception on overflow. 
            case 0xC2:
                instr.Opcode = OpCodes.Refanyval;// <type>
                break;

            //Push the address stored in a typed reference. Object model instruction 
            case 0xC3:
                instr.Opcode = OpCodes.Ckfinite;
                break;

            //Throw ArithmeticException if value is not a finite number. 
            case 0xC6:
                instr.Opcode = OpCodes.Mkrefany; // <class>
                break;

            //Push a typed reference to ptr of type class onto the stack. Object model instruction 
            case 0xD0:
                instr.Opcode = OpCodes.Ldtoken;// <token>
                break;

            //Convert metadata token to its runtime representation. Object model instruction 
            case 0xD1: instr.Opcode = OpCodes.Conv_U2; break;   // Convert to unsigned int16, pushing int32 on stack. 
            case 0xD2: instr.Opcode = OpCodes.Conv_U1; break;   // Convert to unsigned int8, pushing int32 on stack. 
            case 0xD3: instr.Opcode = OpCodes.Conv_I;  break;   // Convert to native int, pushing native int on stack. 
            case 0xD4: instr.Opcode = OpCodes.Conv_Ovf_I; break; // Convert to a native int (on the stack as native int) and throw an exception on overflow. 
            case 0xD5: instr.Opcode = OpCodes.Conv_Ovf_U; break; // Convert to a native unsigned int (on the stack as native int) and throw an exception on overflow. 
            case 0xD6: instr.Opcode = OpCodes.Add_Ovf; break;   // Add signed integer values with overflow check. 
            case 0xD7: instr.Opcode = OpCodes.Add_Ovf_Un; break;    // Add unsigned integer values with overflow check. 
            case 0xD8:
                instr.Opcode = OpCodes.Mul_Ovf;
                break;
                // Multiply signed integer values. Signed result shall fit in same size 
            case 0xD9:
                instr.Opcode = OpCodes.Mul_Ovf_Un;
                break;
                // Multiply unsigned integer values. Unsigned result shall fit in same size 
            case 0xDA:
                instr.Opcode = OpCodes.Sub_Ovf;
                break;
                // Subtract native int from a native int. Signed result shall fit in same size 
            case 0xDB:
                instr.Opcode = OpCodes.Sub_Ovf_Un;
                break;
                // Subtract native unsigned int from a native unsigned int. Unsigned result shall fit in same size. 
            case 0xDC:
                instr.Opcode = OpCodes.Endfinally;
                break;
                // End finally clause of an exception block. 
            case 0xDD:
                instr.Opcode = OpCodes.Leave;   // <int32 (target)>
                break;
                //Exit a protected region of code. 
            case 0xDE:
                instr.Opcode = OpCodes.Leave_S; // <int8 (target)>
                break;
                //Exit a protected region of code, short form. 
            case 0xDF:
                instr.Opcode = OpCodes.Stind_I;
                break;
                // Store value of type native int into memory at address 
            case 0xE0:
                instr.Opcode = OpCodes.Conv_U;
                break;
                //Convert to native unsigned int, pushing native int on stack. 
            case 0xFE:
                switch (rdr.ReadByte())
                {
                case 0x00: instr.Opcode = OpCodes.Arglist; break;       // Return argument list handle for the current method. 
                case 0x01: instr.Opcode = OpCodes.Ceq; break;           // Push 1 (of type int32) if value1 equals value2, else push 0. 
                case 0x02:
                    instr.Opcode = OpCodes.Cgt;
                    break;
                    // Push 1 (of type int32) if value1 > value2, else push 0. 
                case 0x03:
                    instr.Opcode = OpCodes.Cgt_Un;
                    break;
                    // Push 1 (of type int32) if value1 > value2, unsigned or unordered, else push 0. 
                case 0x04:
                    instr.Opcode = OpCodes.Clt;
                    break;
                    // Push 1 (of type int32) if value1 < value2, else push 0. 
                case 0x05:
                    instr.Opcode = OpCodes.Clt_Un;
                    break;
                    // Push 1 (of type int32) if value1 < value2, unsigned or unordered, else push 0. 
                case 0x06:
                    instr.Opcode = OpCodes.Ldftn;// <method>
                    break;
                    // Push a pointer to a method referenced by method, on the stack. 
                case 0x07:
                    instr.Opcode = OpCodes.Ldvirtftn;// <method>
                    break;
                    // Push address of virtual method on the stack. Object model instruction 
                case 0x09:
                    instr.Opcode = OpCodes.Ldarg;//<uint16 (num)>
                    break;
                    //Load argument numbered num onto the stack. 
                case 0x0A:
                    instr.Opcode = OpCodes.Ldarga;// <uint16 (argNum)>
                    break;
                    //Fetch the address of argument argNum. 
                case 0x0B:
                    instr.Opcode = OpCodes.Starg; // <uint16 (num)>
                    break;
                    //Store value to the argument numbered num. 
                case 0x0C:
                    instr.Opcode = OpCodes.Ldloc; //<uint16 (indx)>
                    break;

                //Load local variable of index indx onto stack. 
                case 0x0D: instr.Opcode = OpCodes.Ldloca; //<uint16 (indx)>
                    break;

                //Load address of local variable with index indx. 
                case 0x0E: instr.Opcode = OpCodes.Stloc; // <uint16 (indx)>
                    break;
                    // Pop a value from stack into local variable indx. 
                case 0x0F:
                    instr.Opcode = OpCodes.Localloc;
                    break;
                    // Allocate space from the local memory pool. 
                case 0x11:
                    instr.Opcode = OpCodes.Endfilter;
                    break;
                    // End an exception handling filter clause. 
                case 0x12:
                    instr.Opcode = OpCodes.Unaligned;   //. (alignment)
                    break;

                //Subsequent pointer instruction might be unaligned. Prefix to instruction 
                case 0x13:
                    instr.Opcode = OpCodes.Volatile;
                    break;

                //Subsequent pointer reference is volatile. Prefix to instruction 
                case 0x14: instr.Opcode = OpCodes.Tailcall; break; // Subsequent call terminates current method Prefix to instruction 
                case 0x15:
                    instr.Opcode = OpCodes.Initobj; // <typeTok>
                    break;
                    // Initialize the value at address dest. Object model instruction 
                case 0x16:
                    instr.Opcode = OpCodes.Constrained; //. <thisType>
                    break;
                    // Call a virtual method on a type constrained to be type T Prefix to instruction 
                case 0x17:
                    instr.Opcode = OpCodes.Cpblk;
                    break;
                    // Copy data from memory to memory. 
                case 0x18:
                    instr.Opcode = OpCodes.Initblk;
                    break;

                //Set all bytes in a block of memory to a given byte value. 
                case 0x19:

                    throw new NotImplementedException();
                //instr.Opcode = OpCodes.no {
                //break;
                //typecheck,
                //rangecheck,
                //nullcheck
                    //The specified fault check(s) normally performed as part of the execution of the subsequent instruction can/shall be skipped. Prefix to instruction 
                case 0x1A:
                    instr.Opcode = OpCodes.Rethrow;
                    break;
                    // Rethrow the current exception. Object model instruction 
                case 0x1C:
                    instr.Opcode = OpCodes.Sizeof; //<typeTok>
                    break;
                    //Push the size, in bytes, of a type as an unsigned int32. Object model instruction 
                case 0x1D:
                    instr.Opcode = OpCodes.Refanytype;
                    break;
                    //Push the type token stored in a typed reference. Object model instruction 
                case 0x1E:
                    instr.Opcode = OpCodes.Readonly;
                    break;
                    //Specify that the subsequent array address operation performs no type check at runtime, and that it returns a controlled-mutability managed pointer Prefix to instruction 
                }
                break;
            #endregion
            }
            instr.Length = (int)(rdr.Address - addr);
            return instr;
        }

        public override CilInstruction CreateInvalidInstruction()
        {
            return new CilInstruction
            {
                InstructionClass = InstrClass.Invalid,
                Opcode = default(OpCode),
                Operands = MachineInstruction.NoOperands,
            };
        }
    }
}
