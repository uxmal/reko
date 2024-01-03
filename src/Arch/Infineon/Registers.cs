#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Arch.Infineon
{
    public static class Registers
    {
        public static readonly RegisterStorage[] AddrRegisters;
        public static readonly RegisterStorage[] DataRegisters;
        public static readonly SequenceStorage[] ExtendedARegisters;
        public static readonly SequenceStorage[] ExtendedDRegisters;
        public static readonly FlagGroupStorage[] PswFlags;

        public static readonly Dictionary<uint, RegisterStorage> CoreRegisters;

        public static readonly RegisterStorage a11;
        public static readonly RegisterStorage psw;

        public static readonly FlagGroupStorage C;
        public static readonly FlagGroupStorage V;
        public static readonly FlagGroupStorage SV;
        public static readonly FlagGroupStorage AV;
        public static readonly FlagGroupStorage SAV;
        public static readonly FlagGroupStorage V_SV;
        public static readonly FlagGroupStorage V_SV_AV_SAV;
        public static readonly FlagGroupStorage C_V_SV_AV_SAV;

        public static Dictionary<string, RegisterStorage> ByName { get; }
        public static Dictionary<StorageDomain, RegisterStorage> ByDomain { get; }


        static Registers()
        {
            var factory = new StorageFactory();
            DataRegisters = factory.RangeOfReg32(16, "d{0}");
            AddrRegisters = factory.RangeOfReg32(16, "a{0}");
            ExtendedARegisters = Enumerable.Range(0, 8)
                .Select(n => new SequenceStorage(
                    $"p{n * 2}",
                    PrimitiveType.Word64,
                    AddrRegisters[n * 2 + 1],
                    AddrRegisters[n * 2]))
                .ToArray();
            ExtendedDRegisters = Enumerable.Range(0, 8)
                .Select(n => new SequenceStorage(
                    $"e{n*2}",
                    PrimitiveType.Word64,
                    DataRegisters[n*2+1],
                    DataRegisters[n*2]))
                .ToArray();
            factory = new StorageFactory(StorageDomain.SystemRegister);
            CoreRegisters = new Dictionary<uint, RegisterStorage>
            {
{ 0xFE00, factory.Reg32("PCXI") },          // Previous Context Information Register.
{ 0xFE04, factory.Reg32("PSW") },           // Program Status Word.
{ 0xFE08, factory.Reg32("PC") },            // Program Counter Register.
{ 0xFE14, factory.Reg32("SYSCON") },        // System Configuration Register.
{ 0xFE18, factory.Reg32("CPU_ID") },        // CPU Identification Register (Read Only).
{ 0xFE1C, factory.Reg32("CORE_ID") },       // Core Identification Register
{ 0xFE20, factory.Reg32("BIV") },           // Base Address of Interrupt Vector Table Register.
{ 0xFE24, factory.Reg32("BTV") },           // Base Address of Trap Vector Table Register.
{ 0xFE28, factory.Reg32("ISP") },           // Interrupt Stack Pointer Register.
{ 0xFE2C, factory.Reg32("ICR") },           // ICU Interrupt Control Register.
{ 0xFE38, factory.Reg32("FCX") },           // Free Context List Head Pointer Register.
{ 0xFE3C, factory.Reg32("LCX") },           // Free Context List Limit Pointer Register.
{ 0x9400, factory.Reg32("COMPAT") },        // Compatibility Mode Register.
{ 0xC000, factory.Reg32("DPR0_L") },        // Data Segment Protection Range 0, Lower.
{ 0xC004, factory.Reg32("DPR0_U") },        // Data Segment Protection Range 0, Upper.
{ 0xC008, factory.Reg32("DPR1_L") },        // Data Segment Protection Range 1, Lower.
{ 0xC00C, factory.Reg32("DPR1_U") },        // Data Segment Protection Range 1, Upper.
{ 0xC010, factory.Reg32("DPR2_L") },        // Data Segment Protection Range 2, Lower.
{ 0xC014, factory.Reg32("DPR2_U") },        // Data Segment Protection Range 2, Upper.
{ 0xC018, factory.Reg32("DPR3_L") },        // Data Segment Protection Range 3, Lower.
{ 0xC01C, factory.Reg32("DPR3_U") },        // Data Segment Protection Range 3, Upper.
{ 0xC020, factory.Reg32("DPR4_L") },        // Data Segment Protection Range 4, Lower.
{ 0xC024, factory.Reg32("DPR4_U") },        // Data Segment Protection Range 4, Upper.
{ 0xC028, factory.Reg32("DPR5_L") },        // Data Segment Protection Range 5, Lower.
{ 0xC02C, factory.Reg32("DPR5_U") },        // Data Segment Protection Range 5, Upper.
{ 0xC030, factory.Reg32("DPR6_L") },        // Data Segment Protection Range 6, Lower.
{ 0xC034, factory.Reg32("DPR6_U") },        // Data Segment Protection Range 6, Upper.
{ 0xC038, factory.Reg32("DPR7_L") },        // Data Segment Protection Range 7, Lower.
{ 0xC03C, factory.Reg32("DPR7_U") },        // Data Segment Protection Range 7, Upper.
{ 0xC040, factory.Reg32("DPR8_L") },        // Data Segment Protection Range 8, Lower.
{ 0xC044, factory.Reg32("DPR8_U") },        // Data Segment Protection Range 8, Upper.
{ 0xC048, factory.Reg32("DPR9_L") },        // Data Segment Protection Range 9, Lower.
{ 0xC04C, factory.Reg32("DPR9_U") },        // Data Segment Protection Range 9, Upper.
{ 0xC050, factory.Reg32("DPR10_L") },       // Data Segment Protection Range 10, Lower.
{ 0xC054, factory.Reg32("DPR10_U") },       // Data Segment Protection Range 10, Upper.
{ 0xC058, factory.Reg32("DPR11_L") },       // Data Segment Protection Range 11, Lower.
{ 0xC05C, factory.Reg32("DPR11_U") },       // Data Segment Protection Range 11, Upper.

{ 0xC060, factory.Reg32("DPR12_L") },       //Data Segment Protection Range 12, Lower.
{ 0xC064, factory.Reg32("DPR12_U") },       //Data Segment Protection Range 12, Upper.
{ 0xC068, factory.Reg32("DPR13_L") },       //Data Segment Protection Range 13, Lower.
{ 0xC06C, factory.Reg32("DPR13_U") },       //Data Segment Protection Range 13, Upper.
{ 0xC070, factory.Reg32("DPR14_L") },       //Data Segment Protection Range 14, Lower.
{ 0xC074, factory.Reg32("DPR14_U") },       //Data Segment Protection Range 14, Upper.
{ 0xC078, factory.Reg32("DPR15_L") },
{ 0xC07C, factory.Reg32("DPR15_U") },
{ 0xD000, factory.Reg32("CPR0_L") },
{ 0xD004, factory.Reg32("CPR0_U") },
{ 0xD008, factory.Reg32("CPR1_L") },
{ 0xD00C, factory.Reg32("CPR1_U") },
{ 0xD010, factory.Reg32("CPR2_L") },
{ 0xD014, factory.Reg32("CPR2_U") },
{ 0xD018, factory.Reg32("CPR3_L") },
{ 0xD01C, factory.Reg32("CPR3_U") },
{ 0xD020, factory.Reg32("CPR4_L") },
{ 0xD024, factory.Reg32("CPR4_U") },
{ 0xD028, factory.Reg32("CPR5_L") },
{ 0xD02C, factory.Reg32("CPR5_U") },
{ 0xD030, factory.Reg32("CPR6_L") },
{ 0xD034, factory.Reg32("CPR6_U") },
{ 0xD038, factory.Reg32("CPR7_L") },
{ 0xD03C, factory.Reg32("CPR7_U") },
{ 0xD040, factory.Reg32("CPR8_L") },
{ 0xD044, factory.Reg32("CPR8_U") },
{ 0xD048, factory.Reg32("CPR9_L") },
{ 0xD04C, factory.Reg32("CPR9_U") },
{ 0xD050, factory.Reg32("CPR10_L") },
{ 0xD054, factory.Reg32("CPR10_U") },
{ 0xD058, factory.Reg32("CPR11_L") },
{ 0xD05C, factory.Reg32("CPR11_U") },
{ 0xD060, factory.Reg32("CPR12_L") },
{ 0xD064, factory.Reg32("CPR12_U") },
{ 0xD068, factory.Reg32("CPR13_L") },
{ 0xD06C, factory.Reg32("CPR13_U") },
{ 0xD070, factory.Reg32("CPR14_L") },
{ 0xD074, factory.Reg32("CPR14_U") },
{ 0xD078, factory.Reg32("CPR15_L") },
{ 0xD07C, factory.Reg32("CPR15_U") },
{ 0xE000, factory.Reg32("CPXE_0") },
{ 0xE004, factory.Reg32("CPXE_1") },
{ 0xE008, factory.Reg32("CPXE_2") },
{ 0xE00C, factory.Reg32("CPXE_3") },

//Data Segment Protection Range 15, Lower.
//Data Segment Protection Range 15, Upper.
//Code Segment Protection Range 0, Lower.
//Code Segment Protection Range 0, Upper.
//Code Segment Protection Range 1, Lower.
//Code Segment Protection Range 1, Upper.
//Code Segment Protection Range 2, Lower.
//Code Segment Protection Range 2, Upper.
//Code Segment Protection Range 3, Lower.
//Code Segment Protection Range 3, Upper.
//Code Segment Protection Range 4, Lower.
//Code Segment Protection Range 4, Upper.
//Code Segment Protection Range 5, Lower.
//Code Segment Protection Range 5, Upper.
//Code Segment Protection Range 6, Lower.
//Code Segment Protection Range 6, Upper.
//Code Segment Protection Range 7, Lower.
//Code Segment Protection Range 7, Upper.
//Code Segment Protection Range 8, Lower.
//Code Segment Protection Range 8, Upper.
//Code Segment Protection Range 9, Lower.
//Code Segment Protection Range 9, Upper.
//Code Segment Protection Range 10, Lower.
//Code Segment Protection Range 10, Upper.
//Code Segment Protection Range 11, Lower.
//Code Segment Protection Range 11, Upper.
//Code Segment Protection Range 12, Lower.
//Code Segment Protection Range 12, Upper.
//Code Segment Protection Range 13, Lower.
//Code Segment Protection Range 13, Upper.
//Code Segment Protection Range 14, Lower.
//Code Segment Protection Range 14, Upper.
//Code Segment Protection Range 15, Lower.
//Code Segment Protection Range 15, Upper.
//Code Protection Execute Enable Set-0.
//Code Protection Execute Enable Set-1.
//Code Protection Execute Enable Set-2.
//Code Protection Execute Enable Set-3.
{ 0xE040, factory.Reg32("CPXE_4") },
{ 0xE044, factory.Reg32("CPXE_5") },
{ 0xE048, factory.Reg32("CPXE_6") },
{ 0xE04C, factory.Reg32("CPXE_7") },
{ 0xE010, factory.Reg32("DPRE_0") },
{ 0xE014, factory.Reg32("DPRE_1") },
{ 0xE018, factory.Reg32("DPRE_2") },
{ 0xE01C, factory.Reg32("DPRE_3") },
{ 0xE050, factory.Reg32("DPRE_4") },
{ 0xE054, factory.Reg32("DPRE_5") },
{ 0xE058, factory.Reg32("DPRE_6") },
{ 0xE05C, factory.Reg32("DPRE_7") },
{ 0xE020, factory.Reg32("DPWE_0") },
{ 0xE024, factory.Reg32("DPWE_1") },
{ 0xE028, factory.Reg32("DPWE_2") },
{ 0xE02C, factory.Reg32("DPWE_3") },
{ 0xE060, factory.Reg32("DPWE_4") },
{ 0xE064, factory.Reg32("DPWE_5") },
{ 0xE068, factory.Reg32("DPWE_6") },
{ 0xE06C, factory.Reg32("DPWE_7") },
{ 0xE400, factory.Reg32("TPS_CON") },
{ 0xE404, factory.Reg32("TPS_TIMER0") },
{ 0xE408, factory.Reg32("TPS_TIMER1") },
{ 0xE40C, factory.Reg32("TPS_TIMER2") },
{ 0xE440, factory.Reg32("TPS_EXTIM_ENTRY_CVAL") },
{ 0xE444, factory.Reg32("TPS_EXTIM_ENTRY_LVAL") },
{ 0xE448, factory.Reg32("TPS_EXTIM_EXIT_CVAL") },
{ 0xE44C, factory.Reg32("TPS_EXTIM_EXIT_LVAL") },
{ 0xE450, factory.Reg32("TPS_EXTIM_CLASS_EN") },
{ 0xE454, factory.Reg32("TPS_EXTIM_STAT") },
{ 0xE458, factory.Reg32("TPS_EXTIM_FCX") },


//Memory Management Registers (If implemented)
{ 0x8000, factory.Reg32("MMU_CON") },   //Memory Management Unit Configuration Register.
{ 0x8004, factory.Reg32("MMU_ASI") },   //MMU Address Space Identifier Register.

{ 0x800C, factory.Reg32("MMU_TVA") },   //MMU Translation Virtual Address Register.
{ 0x8010, factory.Reg32("MMU_TPA") },   //MMU Translation Physical Address Register.
//Code Protection Execute Enable Set-4.
//Code Protection Execute Enable Set-5.
//Code Protection Execute Enable Set-6.
//Code Protection Execute Enable Set-7.
//Data Protection Read Enable Set-0.
//Data Protection Read Enable Set-1.
//Data Protection Read Enable Set-2.
//Data Protection Read Enable Set-3.
//Data Protection Read Enable Set-4.
//Data Protection Read Enable Set-5.
//Data Protection Read Enable Set-6.
//Data Protection Read Enable Set-7.
//Data Protection Write Enable Set-0.
//Data Protection Write Enable Set-1.
//Data Protection Write Enable Set-2.
//Data Protection Write Enable Set-3.
//Data Protection Write Enable Set-4.
//Data Protection Write Enable Set-5.
//Data Protection Write Enable Set-6.
//Data Protection Write Enable Set-7.
//Timer Protection Configuration Register
//Temporal Protection Timer 0
//Temporal Protection Timer 1
//Temporal Protection Timer 2
//Temporal Protection Exception Timer Register
//Temporal Protection Exception Timer Register
//Temporal Protection Exception Timer Register
//Temporal Protection Exception Timer Register
//Temporal Protection Exception Timer Register
//Temporal Protection Exception Timer Register
//Temporal Protection Exception Timer Register




{ 0x8014, factory.Reg32("MMU_TPX") },
{ 0x8018, factory.Reg32("MMU_TFA") },
{ 0x8020, factory.Reg32("MMU_TFAS") },
//
//{ 0x801C, factory.Reg32("PMA0'") },
{ 0x8100, factory.Reg32("PMA0") },
{ 0x8104, factory.Reg32("PMA1") },
{ 0x8108, factory.Reg32("PMA2") },
{ 0x9000, factory.Reg32("DCON2") },
{ 0x9008, factory.Reg32("DCON1") },
{ 0x900C, factory.Reg32("SMACON") },
{ 0x9010, factory.Reg32("DSTR") },
{ 0x9018, factory.Reg32("DATR") },
{ 0x901C, factory.Reg32("DEADD") },
{ 0x9020, factory.Reg32("DIEAR") },
{ 0x9024, factory.Reg32("DIETR") },
{ 0x9040, factory.Reg32("DCON0") },
{ 0x9200, factory.Reg32("PSTR") },
{ 0x9204, factory.Reg32("PCON1") },
{ 0x9208, factory.Reg32("PCON2") },
{ 0x920C, factory.Reg32("PCON0") },
{ 0x9210, factory.Reg32("PIEAR") },
{ 0x9214, factory.Reg32("PIETR") },

//Debug Registers
{ 0xFD00, factory.Reg32("DBGSR") },
{ 0xFD08, factory.Reg32("EXEVT") },
{ 0xFD0C, factory.Reg32("CREVT") },
{ 0xFD10, factory.Reg32("SWEVT") },
{ 0xF000, factory.Reg32("TR0EVT") },
{ 0xF004, factory.Reg32("TR0ADR") },
{ 0xF008, factory.Reg32("TR1EVT") },
{ 0xF00C, factory.Reg32("TR1ADR") },
{ 0xF010, factory.Reg32("TR2EVT") },
{ 0xF014, factory.Reg32("TR2ADR") },
{ 0xF018, factory.Reg32("TR3EVT") },
{ 0xF01C, factory.Reg32("TR3ADR") },
{ 0xF020, factory.Reg32("TR4EVT") },
{ 0xF024, factory.Reg32("TR4ADR") },
//MMU Translation Physical Index Register.
//MMU Translation Fault Address Register.
//MMU Translation Fault Address Status Register.
//Physical Memory Attributes Register 0.
//Physical Memory Attributes Register 0.
//Physical Memory Attributes Register 1.
//Physical Memory Attributes Register 2.
//Data Memory Configuration Register-2.
//Data memory Configuration Register-1.
//SIST mode Control Register.
//Data Synchronous Error Trap Register.
//Data Asynchronous Error Trap Register.
//Data Error Address Register.
//Data Integrity Error Address Register.
//Data Integrity Error Trap Register.
//Data Memory Configuration Register-0.
//Program Synchronous Error Trap Register.
//Program Memory Configuration Register-1.
//Program Memory Configuration Register-2.
//Program Memory Configuration Register-0.
//Program Integrity Error Address Register.
//Program Integrity Error Trap Register.
//Debug Status Register.
//External Event Register.
//Core Register Event Register.
//Software Event Register.
//Trigger Event 0 Register.
//Trigger Address 0 Register.
//Trigger Event 1 Register
//Trigger Address 1 Register.
//Trigger Event 2 Register
//Trigger Address 2 Register.
//Trigger Event 3 Register
//Trigger Address 3 Register.
//Trigger Event 4 Register
//Trigger Address 4 Register.
{ 0xF028, factory.Reg32("TR5EVT") },
{ 0xF02C, factory.Reg32("TR5ADR") },
{ 0xF030, factory.Reg32("TR6EVT") },
{ 0xF034, factory.Reg32("TR6ADR") },
{ 0xF038, factory.Reg32("TR7EVT") },
{ 0xF03C, factory.Reg32("TR7ADR") },
{ 0xFD30, factory.Reg32("TRIG_ACC") },
{ 0xFD40, factory.Reg32("DMS") },
{ 0xFD44, factory.Reg32("DCX") },
//{ 0x8004, factory.Reg32("TASK_ASI") },
{ 0xFD48, factory.Reg32("DBGTCR") },
{ 0xFC00, factory.Reg32("CCTRL") },
{ 0xFC04, factory.Reg32("CCNT") },
{ 0xFC08, factory.Reg32("ICNT") },
{ 0xFC0C, factory.Reg32("M1CNT") },
{ 0xFC10, factory.Reg32("M2CNT") },
{ 0xFC14, factory.Reg32("M3CNT") },
//Floating Point Registers
{ 0xA000, factory.Reg32("FPU_TRAP_CON") },
{ 0xA004, factory.Reg32("FPU_TRAP_PC") },
{ 0xA008, factory.Reg32("FPU_TRAP_OPC") },
{ 0xA010, factory.Reg32("FPU_TRAP_SRC1") },
{ 0xA014, factory.Reg32("FPU_TRAP_SRC2") },
{ 0xA018, factory.Reg32("FPU_TRAP_SRC3") },

            };
            psw = CoreRegisters[0xFE04];
            a11 = AddrRegisters[11];

            C = new FlagGroupStorage(psw, (uint)FlagM.CF, "C", PrimitiveType.Bool);
            V = new FlagGroupStorage(psw, (uint) FlagM.CF, "V", PrimitiveType.Bool);
            SV = new FlagGroupStorage(psw, (uint) FlagM.CF, "SV", PrimitiveType.Bool);
            AV = new FlagGroupStorage(psw, (uint) FlagM.CF, "AV", PrimitiveType.Bool);
            SAV = new FlagGroupStorage(psw, (uint) FlagM.CF, "AV", PrimitiveType.Bool);
            PswFlags = new[] { C, V, SV, AV, SAV };

            V_SV = new FlagGroupStorage(psw, (uint) (FlagM.VF|FlagM.SVF), "V_SV", PrimitiveType.Byte);
            V_SV_AV_SAV = new FlagGroupStorage(psw, (uint) (FlagM.VF|FlagM.SVF|FlagM.AVF|FlagM.SAVF), "V_SV_AV_SAV", PrimitiveType.Byte);
            C_V_SV_AV_SAV = new FlagGroupStorage(psw, (uint) (FlagM.CF|FlagM.VF|FlagM.SVF|FlagM.AVF|FlagM.SAVF), "C_V_SV_AV_SAV", PrimitiveType.Byte);

            ByName = factory.NamesToRegisters;
            ByDomain = factory.DomainsToRegisters;
        }
    }

    [Flags]
    public enum FlagM : uint
    {
        CF = 1u << 31,
        VF = 1u << 30,
        SVF = 1u << 29,
        AVF = 1u << 28,
        SAVF = 1u << 27,
    }
}
