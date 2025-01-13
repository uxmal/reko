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

// https://github.com/migueletto/PumpkinOS/blob/master/src/libpumpkin/emulation/trapnames.c
using Reko.Arch.M68k.Machine;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Environments.PalmOS
{
    public class Traps
    {
        public struct TrapInfo
        {
            public ushort trap;
            public string name;
            public string rType;
            public string[] args;

            public TrapInfo(ushort trap, string name, string rType, int nArgs, params string?[] args)
            {
                this.trap = trap;
                this.name = name;
                this.rType = rType;
                this.args = args!;
            }
        }

        private static readonly StructureType Form = new StructureType("Form", 106, true);
        private static readonly StructureType FormObj = new StructureType("FormObj", 0, true);
        private static readonly StructureType Control = new StructureType("Control", 0, true);
        private static readonly StructureType List = new StructureType("List", 0, true);
        private static readonly StructureType Field = new StructureType("Field", 0, true);
        private static readonly StructureType Table = new StructureType("Table", 0, true);
        private static readonly StructureType Scrollbar = new StructureType("Scrollbar", 0, true);
        private static readonly StructureType Gadget = new StructureType("Gadget", 0, true);
        private static readonly StructureType Event = new StructureType("Event", 0, true)
        {
            Fields =
            {
                new StructureField(0, PrimitiveType.UInt16, "type")
            }
        };
        private static readonly StructureType Rect = new StructureType("Rect", 8, true)
        {
            Fields =
            {
                new StructureField(0, PrimitiveType.Int16, "x"),
                new StructureField(2, PrimitiveType.Int16, "y"),
                new StructureField(4, PrimitiveType.Int16, "width"),
                new StructureField(6, PrimitiveType.Int16, "height"),
            }
        };
        private static readonly StructureType DateTime = new StructureType("DateTime", 12, true)
        {
            Fields =
            {
                new StructureField(0, PrimitiveType.Int16, "second"),
                new StructureField(2, PrimitiveType.Int16, "minute"),
                new StructureField(4, PrimitiveType.Int16, "hour"),
                new StructureField(6, PrimitiveType.Int16, "day"),
                new StructureField(8, PrimitiveType.Int16, "month"),
                new StructureField(10, PrimitiveType.Int16, "year"),
            }
        };
        private static readonly StructureType Bitmap = new StructureType("Bitmap", 0, true)
        {
            Fields =
            {
                new StructureField(0, PrimitiveType.Int16, "width"),
                new StructureField(2, PrimitiveType.Int16, "height"),
                new StructureField(8, PrimitiveType.Int16, "version"),
            }
        };
        private static readonly StructureType Window = new StructureType("Window", 0, true)
        {
            Fields =
            {
                new StructureField(0, PrimitiveType.Int16, "width"),
                new StructureField(2, PrimitiveType.Int16, "height"),
            }
        };
        private static readonly TypeReference LocalID = new TypeReference("LocalID", PrimitiveType.Int16);
        private static readonly Pointer PLocalID = new Pointer(LocalID, 32);


        private static readonly TrapInfo[] traps = new TrapInfo[]
        {
            new TrapInfo( 0xA000, "MemInit", "W", 0),
            new TrapInfo( 0xA001, "MemInitHeapTable", "W", 1, "W"),
            new TrapInfo( 0xA002, "MemStoreInit", "?", 0),
            new TrapInfo( 0xA003, "MemCardFormat", "?", 0),
            new TrapInfo( 0xA004, "MemCardInfo", "W", 8, "W", "cp", "cp", "Wp", "Lp", "Lp", "Lp", "Lp"),
            new TrapInfo( 0xA005, "MemStoreInfo", "?", 0),
            new TrapInfo( 0xA006, "MemStoreSetInfo", "?", 0),
            new TrapInfo( 0xA007, "MemNumHeaps", "W", 1, "W"),
            new TrapInfo( 0xA008, "MemNumRAMHeaps", "W", 1, "W"),
            new TrapInfo( 0xA009, "MemHeapID", "W", 2, "W", "W"),
            new TrapInfo( 0xA00A, "MemHeapPtr", "?", 0),
            new TrapInfo( 0xA00B, "MemHeapFreeBytes", "W", 3, "W", "Lp", "Lp"),
            new TrapInfo( 0xA00C, "MemHeapSize", "L", 1, "W"),
            new TrapInfo( 0xA00D, "MemHeapFlags", "W", 1, "W"),
            new TrapInfo( 0xA00E, "MemHeapCompact", "W", 1, "W"),
            new TrapInfo( 0xA00F, "MemHeapInit", "W", 3, "W", "w", "B"),
            new TrapInfo( 0xA010, "MemHeapFreeByOwnerID", "W", 2, "W", "W"),
            new TrapInfo( 0xA011, "MemChunkNew", "vp", 3, "W", "L", "W"),
            new TrapInfo( 0xA012, "MemChunkFree", "W", 1, "vp"),
            new TrapInfo( 0xA013, "MemPtrNew", "vp", 1, "L"),
            new TrapInfo( 0xA014, "MemPtrRecoverHandle", "p", 1, "vp"),
            new TrapInfo( 0xA015, "MemPtrFlags", "W", 1, "vp"),
            new TrapInfo( 0xA016, "MemPtrSize", "L", 1, "vp"),
            new TrapInfo( 0xA017, "MemPtrOwner", "W", 1, "vp"),
            new TrapInfo( 0xA018, "MemPtrHeapID", "W", 1, "vp"),
            new TrapInfo( 0xA019, "MemPtrCardNo", "W", 1, "vp"),
            new TrapInfo( 0xA01A, "MemPtrToLocalID", "lid", 1, "vp"),
            new TrapInfo( 0xA01B, "MemPtrSetOwner", "W", 2, "vp", "W"),
            new TrapInfo( 0xA01C, "MemPtrResize", "W", 2, "vp", "L"),
            new TrapInfo( 0xA01D, "MemPtrResetLock", "W", 1, "vp"),
            new TrapInfo( 0xA01E, "MemHandleNew", "p", 1, "L"),
            new TrapInfo( 0xA01F, "MemHandleLockCount", "W", 1, "p"),
            new TrapInfo( 0xA020, "MemHandleToLocalID", "lid", 1, "p"),
            new TrapInfo( 0xA021, "MemHandleLock", "vp", 1, "p"),
            new TrapInfo( 0xA022, "MemHandleUnlock", "W", 1, "p"),
            new TrapInfo( 0xA023, "MemLocalIDToGlobal", "vp", 2, "lid", "W"),
            new TrapInfo( 0xA024, "MemLocalIDKind", "L", 1, "lid"),
            new TrapInfo( 0xA025, "MemLocalIDToPtr", "vp", 2, "lid", "W"),
            new TrapInfo( 0xA026, "MemMove", "W", 3, "vp", "vp", "l"),
            new TrapInfo( 0xA027, "MemSet", "W", 3, "vp", "l", "B"),
            new TrapInfo( 0xA028, "MemStoreSearch", "?", 0),
            new TrapInfo( 0xA029, "SysReserved10Trap1", "?", 0),
            new TrapInfo( 0xA02A, "MemKernelInit", "W", 0),
            new TrapInfo( 0xA02B, "MemHandleFree", "W", 1, "p"),
            new TrapInfo( 0xA02C, "MemHandleFlags", "W", 1, "p"),
            new TrapInfo( 0xA02D, "MemHandleSize", "L", 1, "p"),
            new TrapInfo( 0xA02E, "MemHandleOwner", "W", 1, "p"),
            new TrapInfo( 0xA02F, "MemHandleHeapID", "W", 1, "p"),
            new TrapInfo( 0xA030, "MemHandleDataStorage", "B", 1, "p"),
            new TrapInfo( 0xA031, "MemHandleCardNo", "W", 1, "p"),
            new TrapInfo( 0xA032, "MemHandleSetOwner", "W", 2, "p", "W"),
            new TrapInfo( 0xA033, "MemHandleResize", "W", 2, "p", "L"),
            new TrapInfo( 0xA034, "MemHandleResetLock", "W", 1, "p"),
            new TrapInfo( 0xA035, "MemPtrUnlock", "W", 1, "vp"),
            new TrapInfo( 0xA036, "MemLocalIDToLockedPtr", "vp", 2, "lid", "W"),
            new TrapInfo( 0xA037, "MemSetDebugMode", "W", 1, "W"),
            new TrapInfo( 0xA038, "MemHeapScramble", "W", 1, "W"),
            new TrapInfo( 0xA039, "MemHeapCheck", "W", 1, "W"),
            new TrapInfo( 0xA03A, "MemNumCards", "W", 0),
            new TrapInfo( 0xA03B, "MemDebugMode", "W", 0),
            new TrapInfo( 0xA03C, "MemSemaphoreReserve", "W", 1, "B"),
            new TrapInfo( 0xA03D, "MemSemaphoreRelease", "W", 1, "B"),
            new TrapInfo( 0xA03E, "MemHeapDynamic", "B", 1, "W"),
            new TrapInfo( 0xA03F, "MemNVParams", "?", 0),
            new TrapInfo( 0xA040, "DmInit", "W", 0),
            new TrapInfo( 0xA041, "DmCreateDatabase", "W", 5, "W", "S", "4", "4", "B"),
            new TrapInfo( 0xA042, "DmDeleteDatabase", "W", 2, "W", "L"),
            new TrapInfo( 0xA043, "DmNumDatabases", "W", 1, "W"),
            new TrapInfo( 0xA044, "DmGetDatabase", "L", 2, "W", "W"),
            new TrapInfo( 0xA045, "DmFindDatabase", "L", 2, "W", "S"),
            new TrapInfo( 0xA046, "DmDatabaseInfo", "W", 13, "W", "lid", "ocp", "oWp", "oWp", "oLp", "oLp", "oLp", "oLp", "olidp", "olidp", "o4p", "o4p"),
            new TrapInfo( 0xA047, "DmSetDatabaseInfo", "W", 13, "W", "lid", "cp", "Wp", "Wp", "Lp", "Lp", "Lp", "Lp", "lidp", "lidp", "4p", "4p"),
            new TrapInfo( 0xA048, "DmDatabaseSize", "W", 5, "W", "L", "Lp", "Lp", "Lp"),
            new TrapInfo( 0xA049, "DmOpenDatabase", "p", 3, "W", "L", "W"),
            new TrapInfo( 0xA04A, "DmCloseDatabase", "W", 1, "p"),
            new TrapInfo( 0xA04B, "DmNextOpenDatabase", "p", 1, "p"),
            new TrapInfo( 0xA04C, "DmOpenDatabaseInfo", "W", 6, "p", "olidp", "oWp", "oWp", "oWp", "oBp"),
            new TrapInfo( 0xA04D, "DmResetRecordStates", "W", 1, "p"),
            new TrapInfo( 0xA04E, "DmGetLastErr", "W", 0),
            new TrapInfo( 0xA04F, "DmNumRecords", "W", 1, "p"),
            new TrapInfo( 0xA050, "DmRecordInfo", "W", 5, "p", "W", "oWp", "oLp", "oLp"),
            new TrapInfo( 0xA051, "DmSetRecordInfo", "W", 4, "p", "W", "Wp", "Lp"),
            new TrapInfo( 0xA052, "DmAttachRecord", "W", 4, "p", "Wp", "p", "pp"),
            new TrapInfo( 0xA053, "DmDetachRecord", "W", 3, "p", "W", "pp"),
            new TrapInfo( 0xA054, "DmMoveRecord", "W", 3, "p", "W", "W"),
            new TrapInfo( 0xA055, "DmNewRecord", "p", 3, "p", "Wp", "L"),
            new TrapInfo( 0xA056, "DmRemoveRecord", "W", 2, "p", "W"),
            new TrapInfo( 0xA057, "DmDeleteRecord", "W", 2, "p", "W"),
            new TrapInfo( 0xA058, "DmArchiveRecord", "W", 2, "p", "W"),
            new TrapInfo( 0xA059, "DmNewHandle", "p", 2, "p", "L"),
            new TrapInfo( 0xA05A, "DmRemoveSecretRecords", "W", 1, "p"),
            new TrapInfo( 0xA05B, "DmQueryRecord", "p", 2, "p", "W"),
            new TrapInfo( 0xA05C, "DmGetRecord", "p", 2, "p", "W"),
            new TrapInfo( 0xA05D, "DmResizeRecord", "p", 3, "p", "W", "L"),
            new TrapInfo( 0xA05E, "DmReleaseRecord", "W", 3, "p", "W", "B"),
            new TrapInfo( 0xA05F, "DmGetResource", "p", 2, "4", "W"),
            new TrapInfo( 0xA060, "DmGet1Resource", "p", 2, "4", "W"),
            new TrapInfo( 0xA061, "DmReleaseResource", "W", 1, "p"),
            new TrapInfo( 0xA062, "DmResizeResource", "p", 2, "p", "L"),
            new TrapInfo( 0xA063, "DmNextOpenResDatabase", "p", 1, "p"),
            new TrapInfo( 0xA064, "DmFindResourceType", "W", 3, "p", "4", "W"),
            new TrapInfo( 0xA065, "DmFindResource", "W", 4, "p", "4", "W", "p"),
            new TrapInfo( 0xA066, "DmSearchResource", "W", 4, "4", "W", "p", "pp"),
            new TrapInfo( 0xA067, "DmNumResources", "W", 1, "p"),
            new TrapInfo( 0xA068, "DmResourceInfo", "W", 5, "p", "W", "Lp", "Wp", "Lp"),
            new TrapInfo( 0xA069, "DmSetResourceInfo", "W", 4, "p", "W", "Lp", "Wp"),
            new TrapInfo( 0xA06A, "DmAttachResource", "?", 0),
            new TrapInfo( 0xA06B, "DmDetachResource", "W", 3, "p", "W", "pp"),
            new TrapInfo( 0xA06C, "DmNewResource", "p", 4, "p", "4", "W", "L"),
            new TrapInfo( 0xA06D, "DmRemoveResource", "W", 2, "p", "W"),
            new TrapInfo( 0xA06E, "DmGetResourceIndex", "p", 2, "p", "W"),
            new TrapInfo( 0xA06F, "DmQuickSort", "W", 3, "p", "pp", "w"),
            new TrapInfo( 0xA070, "DmQueryNextInCategory", "p", 3, "p", "Wp", "W"),
            new TrapInfo( 0xA071, "DmNumRecordsInCategory", "W", 2, "p", "W"),
            new TrapInfo( 0xA072, "DmPositionInCategory", "W", 3, "p", "W", "W"),
            new TrapInfo( 0xA073, "DmSeekRecordInCategory", "W", 5, "p", "Wp", "W", "w", "W"),
            new TrapInfo( 0xA074, "DmMoveCategory", "W", 4, "p", "W", "W", "B"),
            new TrapInfo( 0xA075, "DmOpenDatabaseByTypeCreator", "p", 3, "4", "4", "W"),
            new TrapInfo( 0xA076, "DmWrite", "W", 4, "vp", "L", "vp", "L"),
            new TrapInfo( 0xA077, "DmStrCopy", "W", 3, "vp", "L", "S"),
            new TrapInfo( 0xA078, "DmGetNextDatabaseByTypeCreator", "W", 7, "B", "p", "4", "4", "B", "Wp", "Lp"),
            new TrapInfo( 0xA079, "DmWriteCheck", "W", 3, "vp", "L", "L"),
            new TrapInfo( 0xA07A, "DmMoveOpenDBContext", "?", 0),
            new TrapInfo( 0xA07B, "DmFindRecordByID", "W", 3, "p", "L", "Wp"),
            new TrapInfo( 0xA07C, "DmGetAppInfoID", "L", 1, "p"),
            new TrapInfo( 0xA07D, "DmFindSortPositionV10", "?", 0),
            new TrapInfo( 0xA07E, "DmSet", "W", 4, "vp", "L", "L", "B"),
            new TrapInfo( 0xA07F, "DmCreateDatabaseFromImage", "W", 1, "vp"),
            new TrapInfo( 0xA080, "DbgSrcMessage", "?", 0),
            new TrapInfo( 0xA081, "DbgMessage", "?", 0),
            new TrapInfo( 0xA082, "DbgGetMessage", "?", 0),
            new TrapInfo( 0xA083, "DbgCommSettings", "?", 0),
            new TrapInfo( 0xA084, "ErrDisplayFileLineMsg", "v", 3, "S", "W", "S"),
            new TrapInfo( 0xA085, "ErrSetJump", "w", 1, "p"),
            new TrapInfo( 0xA086, "ErrLongJump", "v", 2, "p", "w"),
            new TrapInfo( 0xA087, "ErrThrow", "v", 1, "l"),
            new TrapInfo( 0xA088, "ErrExceptionList", "p", 0),
            new TrapInfo( 0xA089, "SysBroadcastActionCode", "?", 0),
            new TrapInfo( 0xA08A, "SysUnimplemented", "?", 0),
            new TrapInfo( 0xA08B, "SysColdBoot", "?", 0),
            new TrapInfo( 0xA08C, "SysReset", "?", 0),
            new TrapInfo( 0xA08D, "SysDoze", "?", 0),
            new TrapInfo( 0xA08E, "SysAppLaunch", "W", 6, "W", "L", "W", "W", "vp", "Lp"),
            new TrapInfo( 0xA08F, "SysAppStartup", "W", 3, "p", "p", "p"),
            new TrapInfo( 0xA090, "SysAppExit", "?", 0),
            new TrapInfo( 0xA091, "SysSetA5", "?", 0),
            new TrapInfo( 0xA092, "SysSetTrapAddress", "W", 2, "W", "vp"),
            new TrapInfo( 0xA093, "SysGetTrapAddress", "vp", 1, "W"),
            new TrapInfo( 0xA094, "SysTranslateKernelErr", "?", 0),
            new TrapInfo( 0xA095, "SysSemaphoreCreate", "?", 0),
            new TrapInfo( 0xA096, "SysSemaphoreDelete", "?", 0),
            new TrapInfo( 0xA097, "SysSemaphoreWait", "?", 0),
            new TrapInfo( 0xA098, "SysSemaphoreSignal", "?", 0),
            new TrapInfo( 0xA099, "SysTimerCreate", "?", 0),
            new TrapInfo( 0xA09A, "SysTimerWrite", "?", 0),
            new TrapInfo( 0xA09B, "SysTaskCreate", "?", 0),
            new TrapInfo( 0xA09C, "SysTaskDelete", "?", 0),
            new TrapInfo( 0xA09D, "SysTaskTrigger", "?", 0),
            new TrapInfo( 0xA09E, "SysTaskID", "?", 0),
            new TrapInfo( 0xA09F, "SysTaskUserInfoPtr", "?", 0),
            new TrapInfo( 0xA0A0, "SysTaskDelay", "W", 1, "l"),
            new TrapInfo( 0xA0A1, "SysTaskSetTermProc", "?", 0),
            new TrapInfo( 0xA0A2, "SysUILaunch", "?", 0),
            new TrapInfo( 0xA0A3, "SysNewOwnerID", "?", 0),
            new TrapInfo( 0xA0A4, "SysSemaphoreSet", "?", 0),
            new TrapInfo( 0xA0A5, "SysDisableInts", "?", 0),
            new TrapInfo( 0xA0A6, "SysRestoreStatus", "?", 0),
            new TrapInfo( 0xA0A7, "SysUIAppSwitch", "W", 4, "W", "L", "W", "vp"),
            new TrapInfo( 0xA0A8, "SysCurAppInfoPV20", "?", 0),
            new TrapInfo( 0xA0A9, "SysHandleEvent", "B", 1, "evtp"),
            new TrapInfo( 0xA0AA, "SysInit", "?", 0),
            new TrapInfo( 0xA0AB, "SysQSort", "v", 5, "vp", "W", "w", "p", "l"),
            new TrapInfo( 0xA0AC, "SysCurAppDatabase", "W", 2, "Wp", "Lp"),
            new TrapInfo( 0xA0AD, "SysFatalAlert", "?", 0),
            new TrapInfo( 0xA0AE, "SysResSemaphoreCreate", "?", 0),
            new TrapInfo( 0xA0AF, "SysResSemaphoreDelete", "?", 0),
            new TrapInfo( 0xA0B0, "SysResSemaphoreReserve", "?", 0),
            new TrapInfo( 0xA0B1, "SysResSemaphoreRelease", "?", 0),
            new TrapInfo( 0xA0B2, "SysSleep", "?", 0),
            new TrapInfo( 0xA0B3, "SysKeyboardDialogV10", "v", 0),
            new TrapInfo( 0xA0B4, "SysAppLauncherDialog", "?", 0),
            new TrapInfo( 0xA0B5, "SysSetPerformance", "?", 0),
            new TrapInfo( 0xA0B6, "SysBatteryInfoV20", "?", 0),
            new TrapInfo( 0xA0B7, "SysLibInstall", "?", 0),
            new TrapInfo( 0xA0B8, "SysLibRemove", "W", 1, "W"),
            new TrapInfo( 0xA0B9, "SysLibTblEntry", "p", 1, "W"),
            new TrapInfo( 0xA0BA, "SysLibFind", "W", 2, "S", "Wp"),
            new TrapInfo( 0xA0BB, "SysBatteryDialog", "?", 0),
            new TrapInfo( 0xA0BC, "SysCopyStringResource", "v", 2, "cp", "w"),
            new TrapInfo( 0xA0BD, "SysKernelInfo", "?", 0),
            new TrapInfo( 0xA0BE, "SysLaunchConsole", "?", 0),
            new TrapInfo( 0xA0BF, "SysTimerDelete", "?", 0),
            new TrapInfo( 0xA0C0, "SysSetAutoOffTime", "?", 0),
            new TrapInfo( 0xA0C1, "SysFormPointerArrayToStrings", "p", 2, "cp", "w"),
            new TrapInfo( 0xA0C2, "SysRandom", "w", 1, "l"),
            new TrapInfo( 0xA0C3, "SysTaskSwitching", "?", 0),
            new TrapInfo( 0xA0C4, "SysTimerRead", "?", 0),
            new TrapInfo( 0xA0C5, "StrCopy", "cp", 2, "cp", "S"),
            new TrapInfo( 0xA0C6, "StrCat", "cp", 2, "S", "S"),
            new TrapInfo( 0xA0C7, "StrLen", "W", 1, "S"),
            new TrapInfo( 0xA0C8, "StrCompare", "w", 2, "S", "S"),
            new TrapInfo( 0xA0C9, "StrIToA", "cp", 2, "cp", "l"),
            new TrapInfo( 0xA0CA, "StrCaselessCompare", "w", 2, "S", "S"),
            new TrapInfo( 0xA0CB, "StrIToH", "cp", 2, "cp", "L"),
            new TrapInfo( 0xA0CC, "StrChr", "cp", 2, "S", "W"),
            new TrapInfo( 0xA0CD, "StrStr", "cp", 2, "S", "S"),
            new TrapInfo( 0xA0CE, "StrAToI", "l", 1, "S"),
            new TrapInfo( 0xA0CF, "StrToLower", "cp", 2, "cp", "S"),
            new TrapInfo( 0xA0D0, "SerReceiveISP", "?", 0),
            new TrapInfo( 0xA0D1, "SlkOpen", "?", 0),
            new TrapInfo( 0xA0D2, "SlkClose", "?", 0),
            new TrapInfo( 0xA0D3, "SlkOpenSocket", "?", 0),
            new TrapInfo( 0xA0D4, "SlkCloseSocket", "?", 0),
            new TrapInfo( 0xA0D5, "SlkSocketRefNum", "?", 0),
            new TrapInfo( 0xA0D6, "SlkSocketSetTimeout", "?", 0),
            new TrapInfo( 0xA0D7, "SlkFlushSocket", "?", 0),
            new TrapInfo( 0xA0D8, "SlkSetSocketListener", "?", 0),
            new TrapInfo( 0xA0D9, "SlkSendPacket", "?", 0),
            new TrapInfo( 0xA0DA, "SlkReceivePacket", "?", 0),
            new TrapInfo( 0xA0DB, "SlkSysPktDefaultResponse", "?", 0),
            new TrapInfo( 0xA0DC, "SlkProcessRPC", "?", 0),
            new TrapInfo( 0xA0DD, "ConPutS", "?", 0),
            new TrapInfo( 0xA0DE, "ConGetS", "?", 0),
            new TrapInfo( 0xA0DF, "FplInit", "?", 0),
            new TrapInfo( 0xA0E0, "FplFree", "?", 0),
            new TrapInfo( 0xA0E1, "FplFToA", "?", 0),
            new TrapInfo( 0xA0E2, "FplAToF", "?", 0),
            new TrapInfo( 0xA0E3, "FplBase10Info", "?", 0),
            new TrapInfo( 0xA0E4, "FplLongToFloat", "?", 0),
            new TrapInfo( 0xA0E5, "FplFloatToLong", "?", 0),
            new TrapInfo( 0xA0E6, "FplFloatToULong", "?", 0),
            new TrapInfo( 0xA0E7, "FplMul", "?", 0),
            new TrapInfo( 0xA0E8, "FplAdd", "?", 0),
            new TrapInfo( 0xA0E9, "FplSub", "?", 0),
            new TrapInfo( 0xA0EA, "FplDiv", "?", 0),
            new TrapInfo( 0xA0EB, "WinScreenInit", "v", 0),
            new TrapInfo( 0xA0EC, "ScrCopyRectangle", "?", 0),
            new TrapInfo( 0xA0ED, "ScrDrawChars", "?", 0),
            new TrapInfo( 0xA0EE, "ScrLineRoutine", "?", 0),
            new TrapInfo( 0xA0EF, "ScrRectangleRoutine", "?", 0),
            new TrapInfo( 0xA0F0, "ScrScreenInfo", "?", 0),
            new TrapInfo( 0xA0F1, "ScrDrawNotify", "?", 0),
            new TrapInfo( 0xA0F2, "ScrSendUpdateArea", "?", 0),
            new TrapInfo( 0xA0F3, "ScrCompressScanLine", "?", 0),
            new TrapInfo( 0xA0F4, "ScrDeCompressScanLine", "?", 0),
            new TrapInfo( 0xA0F5, "TimGetSeconds", "L", 0),
            new TrapInfo( 0xA0F6, "TimSetSeconds", "?", 0),
            new TrapInfo( 0xA0F7, "TimGetTicks", "L", 0),
            new TrapInfo( 0xA0F8, "TimInit", "?", 0),
            new TrapInfo( 0xA0F9, "TimSetAlarm", "?", 0),
            new TrapInfo( 0xA0FA, "TimGetAlarm", "?", 0),
            new TrapInfo( 0xA0FB, "TimHandleInterrupt", "?", 0),
            new TrapInfo( 0xA0FC, "TimSecondsToDateTime", "v", 2, "L", "dttp"),
            new TrapInfo( 0xA0FD, "TimDateTimeToSeconds", "L", 1, "dttp"),
            new TrapInfo( 0xA0FE, "TimAdjust", "v", 2, "dttp", "l"),
            new TrapInfo( 0xA0FF, "TimSleep", "?", 0),
            new TrapInfo( 0xA100, "TimWake", "?", 0),
            new TrapInfo( 0xA101, "CategoryCreateListV10", "v", 4, "p", "lstp", "W", "B"),
            new TrapInfo( 0xA102, "CategoryFreeListV10", "v", 2, "p", "lstp"),
            new TrapInfo( 0xA103, "CategoryFind", "W", 2, "p", "S"),
            new TrapInfo( 0xA104, "CategoryGetName", "v", 3, "p", "W", "cp"),
            new TrapInfo( 0xA105, "CategoryEditV10", "B", 2, "p", "Wp"),
            new TrapInfo( 0xA106, "CategorySelectV10", "B", 7, "p", "frmp", "W", "W", "B", "Wp", "S"),
            new TrapInfo( 0xA107, "CategoryGetNext", "W", 2, "p", "W"),
            new TrapInfo( 0xA108, "CategorySetTriggerLabel", "v", 2, "ctlp", "S"),
            new TrapInfo( 0xA109, "CategoryTruncateName", "v", 2, "S", "W"),
            new TrapInfo( 0xA10A, "ClipboardAddItem", "?", 0),
            new TrapInfo( 0xA10B, "ClipboardCheckIfItemExist", "?", 0),
            new TrapInfo( 0xA10C, "ClipboardGetItem", "p", 2, "B", "Wp"),
            new TrapInfo( 0xA10D, "CtlDrawControl", "v", 1, "ctlp"),
            new TrapInfo( 0xA10E, "CtlEraseControl", "v", 1, "ctlp"),
            new TrapInfo( 0xA10F, "CtlHideControl", "v", 1, "ctlp"),
            new TrapInfo( 0xA110, "CtlShowControl", "v", 1, "ctlp"),
            new TrapInfo( 0xA111, "CtlGetValue", "w", 1, "ctlp"),
            new TrapInfo( 0xA112, "CtlSetValue", "v", 2, "ctlp", "w"),
            new TrapInfo( 0xA113, "CtlGetLabel", "cp", 1, "ctlp"),
            new TrapInfo( 0xA114, "CtlSetLabel", "v", 2, "ctlp", "S"),
            new TrapInfo( 0xA115, "CtlHandleEvent", "B", 2, "ctlp", "evtp"),
            new TrapInfo( 0xA116, "CtlHitControl", "v", 1, "ctlp"),
            new TrapInfo( 0xA117, "CtlSetEnabled", "v", 2, "ctlp", "B"),
            new TrapInfo( 0xA118, "CtlSetUsable", "v", 2, "ctlp", "B"),
            new TrapInfo( 0xA119, "CtlEnabled", "B", 1, "ctlp"),
            new TrapInfo( 0xA11A, "EvtInitialize", "?", 0),
            new TrapInfo( 0xA11B, "EvtAddEventToQueue", "v", 1, "evtp"),
            new TrapInfo( 0xA11C, "EvtCopyEvent", "v", 2, "evtp", "oevtp"),
            new TrapInfo( 0xA11D, "EvtGetEvent", "v", 2, "oevtp", "l"),
            new TrapInfo( 0xA11E, "EvtGetPen", "v", 3, "wp", "wp", "Bp"),
            new TrapInfo( 0xA11F, "EvtSysInit", "?", 0),
            new TrapInfo( 0xA120, "EvtGetSysEvent", "?", 0),
            new TrapInfo( 0xA121, "EvtProcessSoftKeyStroke", "?", 0),
            new TrapInfo( 0xA122, "EvtGetPenBtnList", "?", 0),
            new TrapInfo( 0xA123, "EvtSetPenQueuePtr", "?", 0),
            new TrapInfo( 0xA124, "EvtPenQueueSize", "?", 0),
            new TrapInfo( 0xA125, "EvtFlushPenQueue", "?", 0),
            new TrapInfo( 0xA126, "EvtEnqueuePenPoint", "?", 0),
            new TrapInfo( 0xA127, "EvtDequeuePenStrokeInfo", "?", 0),
            new TrapInfo( 0xA128, "EvtDequeuePenPoint", "?", 0),
            new TrapInfo( 0xA129, "EvtFlushNextPenStroke", "?", 0),
            new TrapInfo( 0xA12A, "EvtSetKeyQueuePtr", "?", 0),
            new TrapInfo( 0xA12B, "EvtKeyQueueSize", "?", 0),
            new TrapInfo( 0xA12C, "EvtFlushKeyQueue", "?", 0),
            new TrapInfo( 0xA12D, "EvtEnqueueKey", "?", 0),
            new TrapInfo( 0xA12E, "EvtDequeueKeyEvent", "?", 0),
            new TrapInfo( 0xA12F, "EvtWakeup", "?", 0),
            new TrapInfo( 0xA130, "EvtResetAutoOffTimer", "W", 0),
            new TrapInfo( 0xA131, "EvtKeyQueueEmpty", "?", 0),
            new TrapInfo( 0xA132, "EvtEnableGraffiti", "v", 1, "B"),
            new TrapInfo( 0xA133, "FldCopy", "v", 1, "fldp"),
            new TrapInfo( 0xA134, "FldCut", "v", 1, "fldp"),
            new TrapInfo( 0xA135, "FldDrawField", "v", 1, "fldp"),
            new TrapInfo( 0xA136, "FldEraseField", "v", 1, "fldp"),
            new TrapInfo( 0xA137, "FldFreeMemory", "v", 1, "fldp"),
            new TrapInfo( 0xA138, "FldGetBounds", "v", 2, "fldp", "rctp"),
            new TrapInfo( 0xA139, "FldGetTextPtr", "cp", 1, "fldp"),
            new TrapInfo( 0xA13A, "FldGetSelection", "v", 3, "fldp", "Wp", "Wp"),
            new TrapInfo( 0xA13B, "FldHandleEvent", "B", 2, "fldp", "evtp"),
            new TrapInfo( 0xA13C, "FldPaste", "v", 1, "fldp"),
            new TrapInfo( 0xA13D, "FldRecalculateField", "v", 2, "fldp", "B"),
            new TrapInfo( 0xA13E, "FldSetBounds", "v", 2, "fldp", "rctp"),
            new TrapInfo( 0xA13F, "FldSetText", "v", 4, "fldp", "p", "W", "W"),
            new TrapInfo( 0xA140, "FldGetFont", "B", 1, "fldp"),
            new TrapInfo( 0xA141, "FldSetFont", "v", 2, "fldp", "B"),
            new TrapInfo( 0xA142, "FldSetSelection", "v", 3, "fldp", "W", "W"),
            new TrapInfo( 0xA143, "FldGrabFocus", "v", 1, "fldp"),
            new TrapInfo( 0xA144, "FldReleaseFocus", "v", 1, "fldp"),
            new TrapInfo( 0xA145, "FldGetInsPtPosition", "W", 1, "fldp"),
            new TrapInfo( 0xA146, "FldSetInsPtPosition", "v", 2, "fldp", "W"),
            new TrapInfo( 0xA147, "FldSetScrollPosition", "v", 2, "fldp", "W"),
            new TrapInfo( 0xA148, "FldGetScrollPosition", "W", 1, "fldp"),
            new TrapInfo( 0xA149, "FldGetTextHeight", "W", 1, "fldp"),
            new TrapInfo( 0xA14A, "FldGetTextAllocatedSize", "W", 1, "fldp"),
            new TrapInfo( 0xA14B, "FldGetTextLength", "W", 1, "fldp"),
            new TrapInfo( 0xA14C, "FldScrollField", "v", 3, "fldp", "W", "B"),
            new TrapInfo( 0xA14D, "FldScrollable", "B", 2, "fldp", "B"),
            new TrapInfo( 0xA14E, "FldGetVisibleLines", "W", 1, "fldp"),
            new TrapInfo( 0xA14F, "FldGetAttributes", "v", 2, "fldp", "p"),
            new TrapInfo( 0xA150, "FldSetAttributes", "v", 2, "fldp", "p"),
            new TrapInfo( 0xA151, "FldSendChangeNotification", "v", 1, "fldp"),
            new TrapInfo( 0xA152, "FldCalcFieldHeight", "W", 2, "S", "W"),
            new TrapInfo( 0xA153, "FldGetTextHandle", "p", 1, "fldp"),
            new TrapInfo( 0xA154, "FldCompactText", "v", 1, "fldp"),
            new TrapInfo( 0xA155, "FldDirty", "B", 1, "fldp"),
            new TrapInfo( 0xA156, "FldWordWrap", "W", 2, "S", "w"),
            new TrapInfo( 0xA157, "FldSetTextAllocatedSize", "v", 2, "fldp", "W"),
            new TrapInfo( 0xA158, "FldSetTextHandle", "v", 2, "fldp", "p"),
            new TrapInfo( 0xA159, "FldSetTextPtr", "v", 2, "fldp", "S"),
            new TrapInfo( 0xA15A, "FldGetMaxChars", "W", 1, "fldp"),
            new TrapInfo( 0xA15B, "FldSetMaxChars", "v", 2, "fldp", "W"),
            new TrapInfo( 0xA15C, "FldSetUsable", "v", 2, "fldp", "B"),
            new TrapInfo( 0xA15D, "FldInsert", "B", 3, "fldp", "S", "W"),
            new TrapInfo( 0xA15E, "FldDelete", "v", 3, "fldp", "W", "W"),
            new TrapInfo( 0xA15F, "FldUndo", "v", 1, "fldp"),
            new TrapInfo( 0xA160, "FldSetDirty", "v", 2, "fldp", "B"),
            new TrapInfo( 0xA161, "FldSendHeightChangeNotification", "v", 3, "fldp", "W", "w"),
            new TrapInfo( 0xA162, "FldMakeFullyVisible", "B", 1, "fldp"),
            new TrapInfo( 0xA163, "FntGetFont", "B", 0),
            new TrapInfo( 0xA164, "FntSetFont", "B", 1, "B"),
            new TrapInfo( 0xA165, "FntGetFontPtr", "p", 0),
            new TrapInfo( 0xA166, "FntBaseLine", "w", 0),
            new TrapInfo( 0xA167, "FntCharHeight", "w", 0),
            new TrapInfo( 0xA168, "FntLineHeight", "w", 0),
            new TrapInfo( 0xA169, "FntAverageCharWidth", "w", 0),
            new TrapInfo( 0xA16A, "FntCharWidth", "w", 1, "c"),
            new TrapInfo( 0xA16B, "FntCharsWidth", "w", 2, "cp", "w"),
            new TrapInfo( 0xA16C, "FntDescenderHeight", "w", 0),
            new TrapInfo( 0xA16D, "FntCharsInWidth", "v", 4, "cp", "wp", "wp", "Bp"),
            new TrapInfo( 0xA16E, "FntLineWidth", "w", 2, "cp", "W"),
            new TrapInfo( 0xA16F, "FrmInitForm", "frmp", 1, "W"),
            new TrapInfo( 0xA170, "FrmDeleteForm", "v", 1, "frmp"),
            new TrapInfo( 0xA171, "FrmDrawForm", "v", 1, "frmp"),
            new TrapInfo( 0xA172, "FrmEraseForm", "v", 1, "frmp"),
            new TrapInfo( 0xA173, "FrmGetActiveForm", "frmp", 0),
            new TrapInfo( 0xA174, "FrmSetActiveForm", "v", 1, "frmp"),
            new TrapInfo( 0xA175, "FrmGetActiveFormID", "W", 0),
            new TrapInfo( 0xA176, "FrmGetUserModifiedState", "?", 0),
            new TrapInfo( 0xA177, "FrmSetNotUserModified", "?", 0),
            new TrapInfo( 0xA178, "FrmGetFocus", "W", 1, "frmp"),
            new TrapInfo( 0xA179, "FrmSetFocus", "v", 2, "frmp", "W"),
            new TrapInfo( 0xA17A, "FrmHandleEvent", "B", 2, "frmp", "evtp"),
            new TrapInfo( 0xA17B, "FrmGetFormBounds", "v", 2, "frmp", "rctp"),
            new TrapInfo( 0xA17C, "FrmGetWindowHandle", "winp", 1, "frmp"),
            new TrapInfo( 0xA17D, "FrmGetFormId", "W", 1, "frmp"),
            new TrapInfo( 0xA17E, "FrmGetFormPtr", "frmp", 1, "W"),
            new TrapInfo( 0xA17F, "FrmGetNumberOfObjects", "W", 1, "frmp"),
            new TrapInfo( 0xA180, "FrmGetObjectIndex", "W", 2, "frmp", "W"),
            new TrapInfo( 0xA181, "FrmGetObjectId", "W", 2, "frmp", "W"),
            new TrapInfo( 0xA182, "FrmGetObjectType", "B", 2, "frmp", "W"),
            new TrapInfo( 0xA183, "FrmGetObjectPtr", "fobjp", 2, "frmp", "W"),
            new TrapInfo( 0xA184, "FrmHideObject", "v", 2, "frmp", "W"),
            new TrapInfo( 0xA185, "FrmShowObject", "v", 2, "frmp", "W"),
            new TrapInfo( 0xA186, "FrmGetObjectPosition", "v", 4, "frmp", "W", "wp", "wp"),
            new TrapInfo( 0xA187, "FrmSetObjectPosition", "v", 4, "frmp", "W", "w", "w"),
            new TrapInfo( 0xA188, "FrmGetControlValue", "w", 2, "frmp", "W"),
            new TrapInfo( 0xA189, "FrmSetControlValue", "v", 3, "frmp", "W", "w"),
            new TrapInfo( 0xA18A, "FrmGetControlGroupSelection", "W", 2, "frmp", "B"),
            new TrapInfo( 0xA18B, "FrmSetControlGroupSelection", "v", 3, "frmp", "B", "W"),
            new TrapInfo( 0xA18C, "FrmCopyLabel", "v", 3, "frmp", "W", "S"),
            new TrapInfo( 0xA18D, "FrmSetLabel", "?", 0),
            new TrapInfo( 0xA18E, "FrmGetLabel", "cp", 2, "frmp", "W"),
            new TrapInfo( 0xA18F, "FrmSetCategoryLabel", "?", 0),
            new TrapInfo( 0xA190, "FrmGetTitle", "cp", 1, "frmp"),
            new TrapInfo( 0xA191, "FrmSetTitle", "v", 2, "frmp", "S"),
            new TrapInfo( 0xA192, "FrmAlert", "W", 1, "W"),
            new TrapInfo( 0xA193, "FrmDoDialog", "W", 1, "frmp"),
            new TrapInfo( 0xA194, "FrmCustomAlert", "W", 4, "W", "S", "S", "S"),
            new TrapInfo( 0xA195, "FrmHelp", "v", 1, "W"),
            new TrapInfo( 0xA196, "FrmUpdateScrollers", "v", 5, "frmp", "W", "W", "B", "B"),
            new TrapInfo( 0xA197, "FrmGetFirstForm", "frmp", 0),
            new TrapInfo( 0xA198, "FrmVisible", "B", 1, "frmp"),
            new TrapInfo( 0xA199, "FrmGetObjectBounds", "v", 3, "frmp", "W", "rctp"),
            new TrapInfo( 0xA19A, "FrmCopyTitle", "v", 2, "frmp", "S"),
            new TrapInfo( 0xA19B, "FrmGotoForm", "v", 1, "W"),
            new TrapInfo( 0xA19C, "FrmPopupForm", "v", 1, "W"),
            new TrapInfo( 0xA19D, "FrmUpdateForm", "v", 2, "W", "W"),
            new TrapInfo( 0xA19E, "FrmReturnToForm", "v", 1, "W"),
            new TrapInfo( 0xA19F, "FrmSetEventHandler", "v", 2, "frmp", "p"),
            new TrapInfo( 0xA1A0, "FrmDispatchEvent", "B", 1, "evtp"),
            new TrapInfo( 0xA1A1, "FrmCloseAllForms", "?", 0),
            new TrapInfo( 0xA1A2, "FrmSaveAllForms", "?", 0),
            new TrapInfo( 0xA1A3, "FrmGetGadgetData", "vp", 2, "frmp", "W"),
            new TrapInfo( 0xA1A4, "FrmSetGadgetData", "v", 3, "frmp", "W", "vp"),
            new TrapInfo( 0xA1A5, "FrmSetCategoryTrigger", "?", 0),
            new TrapInfo( 0xA1A6, "UIInitialize", "?", 0),
            new TrapInfo( 0xA1A7, "UIReset", "?", 0),
            new TrapInfo( 0xA1A8, "InsPtInitialize", "v", 0),
            new TrapInfo( 0xA1A9, "InsPtSetLocation", "v", 2, "w", "w"),
            new TrapInfo( 0xA1AA, "InsPtGetLocation", "v", 2, "wp", "wp"),
            new TrapInfo( 0xA1AB, "InsPtEnable", "v", 1, "B"),
            new TrapInfo( 0xA1AC, "InsPtEnabled", "B", 0),
            new TrapInfo( 0xA1AD, "InsPtSetHeight", "v", 1, "w"),
            new TrapInfo( 0xA1AE, "InsPtGetHeight", "w", 0),
            new TrapInfo( 0xA1AF, "InsPtCheckBlink", "v", 0),
            new TrapInfo( 0xA1B0, "LstSetDrawFunction", "v", 2, "lstp", "p"),
            new TrapInfo( 0xA1B1, "LstDrawList", "v", 1, "lstp"),
            new TrapInfo( 0xA1B2, "LstEraseList", "v", 1, "lstp"),
            new TrapInfo( 0xA1B3, "LstGetSelection", "w", 1, "lstp"),
            new TrapInfo( 0xA1B4, "LstGetSelectionText", "cp", 2, "lstp", "w"),
            new TrapInfo( 0xA1B5, "LstHandleEvent", "B", 2, "lstp", "evtp"),
            new TrapInfo( 0xA1B6, "LstSetHeight", "v", 2, "lstp", "w"),
            new TrapInfo( 0xA1B7, "LstSetSelection", "v", 2, "lstp", "w"),
            new TrapInfo( 0xA1B8, "LstSetListChoices", "v", 3, "lstp", "p", "w"),
            new TrapInfo( 0xA1B9, "LstMakeItemVisible", "v", 2, "lstp", "w"),
            new TrapInfo( 0xA1BA, "LstGetNumberOfItems", "w", 1, "lstp"),
            new TrapInfo( 0xA1BB, "LstPopupList", "w", 1, "lstp"),
            new TrapInfo( 0xA1BC, "LstSetPosition", "v", 3, "lstp", "w", "w"),
            new TrapInfo( 0xA1BD, "MenuInit", "p", 1, "W"),
            new TrapInfo( 0xA1BE, "MenuDispose", "v", 1, "p"),
            new TrapInfo( 0xA1BF, "MenuHandleEvent", "B", 3, "p", "evtp", "oWp"),
            new TrapInfo( 0xA1C0, "MenuDrawMenu", "v", 1, "p"),
            new TrapInfo( 0xA1C1, "MenuEraseStatus", "v", 1, "p"),
            new TrapInfo( 0xA1C2, "MenuGetActiveMenu", "p", 0),
            new TrapInfo( 0xA1C3, "MenuSetActiveMenu", "p", 1, "p"),
            new TrapInfo( 0xA1C4, "RctSetRectangle", "v", 5, "rctp", "w", "w", "w", "w"),
            new TrapInfo( 0xA1C5, "RctCopyRectangle", "v", 2, "rctp", "rctp"),
            new TrapInfo( 0xA1C6, "RctInsetRectangle", "v", 2, "rctp", "w"),
            new TrapInfo( 0xA1C7, "RctOffsetRectangle", "?", 0),
            new TrapInfo( 0xA1C8, "RctPtInRectangle", "B", 3, "w", "w", "rctp"),
            new TrapInfo( 0xA1C9, "RctGetIntersection", "v", 3, "rctp", "rctp", "rctp"),
            new TrapInfo( 0xA1CA, "TblDrawTable", "v", 1, "tblp"),
            new TrapInfo( 0xA1CB, "TblEraseTable", "v", 1, "tblp"),
            new TrapInfo( 0xA1CC, "TblHandleEvent", "B", 2, "tblp", "evtp"),
            new TrapInfo( 0xA1CD, "TblGetItemBounds", "v", 4, "tblp", "w", "w", "rctp"),
            new TrapInfo( 0xA1CE, "TblSelectItem", "v", 3, "tblp", "w", "w"),
            new TrapInfo( 0xA1CF, "TblGetItemInt", "w", 3, "tblp", "w", "w"),
            new TrapInfo( 0xA1D0, "TblSetItemInt", "v", 4, "tblp", "w", "w", "w"),
            new TrapInfo( 0xA1D1, "TblSetItemStyle", "v", 4, "tblp", "w", "w", "B"),
            new TrapInfo( 0xA1D2, "TblUnhighlightSelection", "v", 1, "tblp"),
            new TrapInfo( 0xA1D3, "TblSetRowUsable", "v", 3, "tblp", "w", "B"),
            new TrapInfo( 0xA1D4, "TblGetNumberOfRows", "w", 1, "tblp"),
            new TrapInfo( 0xA1D5, "TblSetCustomDrawProcedure", "v", 3, "tblp", "w", "p"),
            new TrapInfo( 0xA1D6, "TblSetRowSelectable", "v", 3, "tblp", "w", "B"),
            new TrapInfo( 0xA1D7, "TblRowSelectable", "B", 2, "tblp", "w"),
            new TrapInfo( 0xA1D8, "TblSetLoadDataProcedure", "v", 3, "tblp", "w", "p"),
            new TrapInfo( 0xA1D9, "TblSetSaveDataProcedure", "v", 3, "tblp", "w", "p"),
            new TrapInfo( 0xA1DA, "TblGetBounds", "v", 2, "tblp", "orctp"),
            new TrapInfo( 0xA1DB, "TblSetRowHeight", "v", 3, "tblp", "w", "w"),
            new TrapInfo( 0xA1DC, "TblGetColumnWidth", "w", 2, "tblp", "w"),
            new TrapInfo( 0xA1DD, "TblGetRowID", "W", 2, "tblp", "w"),
            new TrapInfo( 0xA1DE, "TblSetRowID", "v", 3, "tblp", "w", "W"),
            new TrapInfo( 0xA1DF, "TblMarkRowInvalid", "v", 2, "tblp", "w"),
            new TrapInfo( 0xA1E0, "TblMarkTableInvalid", "v", 1, "tblp"),
            new TrapInfo( 0xA1E1, "TblGetSelection", "B", 3, "tblp", "wp", "wp"),
            new TrapInfo( 0xA1E2, "TblInsertRow", "v", 2, "tblp", "w"),
            new TrapInfo( 0xA1E3, "TblRemoveRow", "v", 2, "tblp", "w"),
            new TrapInfo( 0xA1E4, "TblRowInvalid", "B", 2, "tblp", "w"),
            new TrapInfo( 0xA1E5, "TblRedrawTable", "v", 1, "tblp"),
            new TrapInfo( 0xA1E6, "TblRowUsable", "B", 2, "tblp", "w"),
            new TrapInfo( 0xA1E7, "TblReleaseFocus", "v", 1, "tblp"),
            new TrapInfo( 0xA1E8, "TblEditing", "B", 1, "tblp"),
            new TrapInfo( 0xA1E9, "TblGetCurrentField", "fldp", 1, "tblp"),
            new TrapInfo( 0xA1EA, "TblSetColumnUsable", "v", 3, "tblp", "w", "B"),
            new TrapInfo( 0xA1EB, "TblGetRowHeight", "w", 2, "tblp", "w"),
            new TrapInfo( 0xA1EC, "TblSetColumnWidth", "v", 3, "tblp", "w", "w"),
            new TrapInfo( 0xA1ED, "TblGrabFocus", "v", 3, "tblp", "w", "w"),
            new TrapInfo( 0xA1EE, "TblSetItemPtr", "v", 4, "tblp", "w", "w", "vp"),
            new TrapInfo( 0xA1EF, "TblFindRowID", "B", 3, "tblp", "W", "owp"),
            new TrapInfo( 0xA1F0, "TblGetLastUsableRow", "w", 1, "tblp"),
            new TrapInfo( 0xA1F1, "TblGetColumnSpacing", "w", 2, "tblp", "w"),
            new TrapInfo( 0xA1F2, "TblFindRowData", "B", 3, "tblp", "L", "wp"),
            new TrapInfo( 0xA1F3, "TblGetRowData", "p", 2, "tblp", "w"),
            new TrapInfo( 0xA1F4, "TblSetRowData", "v", 3, "tblp", "w", "p"),
            new TrapInfo( 0xA1F5, "TblSetColumnSpacing", "v", 3, "tblp", "w", "w"),
            new TrapInfo( 0xA1F6, "WinCreateWindow", "winp", 5, "rctp", "W", "B", "B", "Wp"),
            new TrapInfo( 0xA1F7, "WinCreateOffscreenWindow", "winp", 4, "w", "w", "B", "Wp"),
            new TrapInfo( 0xA1F8, "WinDeleteWindow", "v", 2, "winp", "B"),
            new TrapInfo( 0xA1F9, "WinInitializeWindow", "v", 1, "winp"),
            new TrapInfo( 0xA1FA, "WinAddWindow", "v", 1, "winp"),
            new TrapInfo( 0xA1FB, "WinRemoveWindow", "v", 1, "winp"),
            new TrapInfo( 0xA1FC, "WinSetActiveWindow", "v", 1, "winp"),
            new TrapInfo( 0xA1FD, "WinSetDrawWindow", "winp", 1, "winp"),
            new TrapInfo( 0xA1FE, "WinGetDrawWindow", "winp", 0),
            new TrapInfo( 0xA1FF, "WinGetActiveWindow", "winp", 0),
            new TrapInfo( 0xA200, "WinGetDisplayWindow", "winp", 0),
            new TrapInfo( 0xA201, "WinGetFirstWindow", "winp", 0),
            new TrapInfo( 0xA202, "WinEnableWindow", "v", 1, "winp"),
            new TrapInfo( 0xA203, "WinDisableWindow", "v", 1, "winp"),
            new TrapInfo( 0xA204, "WinGetWindowFrameRect", "v", 2, "winp", "rctp"),
            new TrapInfo( 0xA205, "WinDrawWindowFrame", "v", 0),
            new TrapInfo( 0xA206, "WinEraseWindow", "v", 0),
            new TrapInfo( 0xA207, "WinSaveBits", "winp", 2, "rctp", "Wp"),
            new TrapInfo( 0xA208, "WinRestoreBits", "v", 3, "winp", "w", "w"),
            new TrapInfo( 0xA209, "WinCopyRectangle", "v", 6, "winp", "winp", "rctp", "w", "w", "B"),
            new TrapInfo( 0xA20A, "WinScrollRectangle", "v", 4, "rctp", "B", "w", "rctp"),
            new TrapInfo( 0xA20B, "WinGetDisplayExtent", "v", 2, "wp", "wp"),
            new TrapInfo( 0xA20C, "WinGetWindowExtent", "v", 2, "wp", "wp"),
            new TrapInfo( 0xA20D, "WinDisplayToWindowPt", "v", 2, "wp", "wp"),
            new TrapInfo( 0xA20E, "WinWindowToDisplayPt", "v", 2, "wp", "wp"),
            new TrapInfo( 0xA20F, "WinGetClip", "v", 1, "rctp"),
            new TrapInfo( 0xA210, "WinSetClip", "v", 1, "rctp"),
            new TrapInfo( 0xA211, "WinResetClip", "v", 0),
            new TrapInfo( 0xA212, "WinClipRectangle", "v", 1, "rctp"),
            new TrapInfo( 0xA213, "WinDrawLine", "v", 4, "w", "w", "w", "w"),
            new TrapInfo( 0xA214, "WinDrawGrayLine", "v", 4, "w", "w", "w", "w"),
            new TrapInfo( 0xA215, "WinEraseLine", "v", 4, "w", "w", "w", "w"),
            new TrapInfo( 0xA216, "WinInvertLine", "v", 4, "w", "w", "w", "w"),
            new TrapInfo( 0xA217, "WinFillLine", "v", 4, "w", "w", "w", "w"),
            new TrapInfo( 0xA218, "WinDrawRectangle", "v", 2, "rctp", "W"),
            new TrapInfo( 0xA219, "WinEraseRectangle", "v", 2, "rctp", "W"),
            new TrapInfo( 0xA21A, "WinInvertRectangle", "v", 2, "rctp", "W"),
            new TrapInfo( 0xA21B, "WinDrawRectangleFrame", "v", 2, "W", "rctp"),
            new TrapInfo( 0xA21C, "WinDrawGrayRectangleFrame", "v", 2, "W", "rctp"),
            new TrapInfo( 0xA21D, "WinEraseRectangleFrame", "v", 2, "W", "rctp"),
            new TrapInfo( 0xA21E, "WinInvertRectangleFrame", "v", 2, "W", "rctp"),
            new TrapInfo( 0xA21F, "WinGetFramesRectangle", "?", 0),
            new TrapInfo( 0xA220, "WinDrawChars", "v", 4, "S", "w", "w", "w"),
            new TrapInfo( 0xA221, "WinEraseChars", "v", 4, "S", "w", "w", "w"),
            new TrapInfo( 0xA222, "WinInvertChars", "v", 4, "S", "w", "w", "w"),
            new TrapInfo( 0xA223, "WinGetPattern", "v", 1, "p"),
            new TrapInfo( 0xA224, "WinSetPattern", "v", 1, "p"),
            new TrapInfo( 0xA225, "WinSetUnderlineMode", "B", 1, "B"),
            new TrapInfo( 0xA226, "WinDrawBitmap", "v", 3, "bmpp", "w", "w"),
            new TrapInfo( 0xA227, "WinModal", "B", 1, "winp"),
            new TrapInfo( 0xA228, "WinGetDrawWindowBounds", "v", 1, "rctp"),
            new TrapInfo( 0xA229, "WinFillRectangle", "v", 2, "rctp", "W"),
            new TrapInfo( 0xA22A, "WinDrawInvertedChars", "v", 4, "S", "w", "w", "w"),
            new TrapInfo( 0xA22B, "PrefOpenPreferenceDBV10", "p", 0),
            new TrapInfo( 0xA22C, "PrefGetPreferences", "v", 1, "p"),
            new TrapInfo( 0xA22D, "PrefSetPreferences", "?", 0),
            new TrapInfo( 0xA22E, "PrefGetAppPreferencesV10", "B", 4, "L", "w", "vp", "W"),
            new TrapInfo( 0xA22F, "PrefSetAppPreferencesV10", "v", 4, "4", "w", "vp", "W"),
            new TrapInfo( 0xA230, "SndInit", "?", 0),
            new TrapInfo( 0xA231, "SndSetDefaultVolume", "v", 3, "Wp", "Wp", "Wp"),
            new TrapInfo( 0xA232, "SndGetDefaultVolume", "v", 3, "Wp", "Wp", "Wp"),
            new TrapInfo( 0xA233, "SndDoCmd", "W", 3, "vp", "p", "B"),
            new TrapInfo( 0xA234, "SndPlaySystemSound", "v", 1, "B"),
            new TrapInfo( 0xA235, "AlmInit", "?", 0),
            new TrapInfo( 0xA236, "AlmCancelAll", "?", 0),
            new TrapInfo( 0xA237, "AlmAlarmCallback", "?", 0),
            new TrapInfo( 0xA238, "AlmSetAlarm", "?", 0),
            new TrapInfo( 0xA239, "AlmGetAlarm", "?", 0),
            new TrapInfo( 0xA23A, "AlmDisplayAlarm", "?", 0),
            new TrapInfo( 0xA23B, "AlmEnableNotification", "?", 0),
            new TrapInfo( 0xA23C, "HwrGetRAMMapping", "?", 0),
            new TrapInfo( 0xA23D, "HwrMemWritable", "?", 0),
            new TrapInfo( 0xA23E, "HwrMemReadable", "?", 0),
            new TrapInfo( 0xA23F, "HwrDoze", "?", 0),
            new TrapInfo( 0xA240, "HwrSleep", "?", 0),
            new TrapInfo( 0xA241, "HwrWake", "?", 0),
            new TrapInfo( 0xA242, "HwrSetSystemClock", "?", 0),
            new TrapInfo( 0xA243, "HwrSetCPUDutyCycle", "?", 0),
            new TrapInfo( 0xA244, "HwrDisplayInit", "?", 0),
            new TrapInfo( 0xA245, "HwrDisplaySleep", "?", 0),
            new TrapInfo( 0xA246, "HwrTimerInit", "?", 0),
            new TrapInfo( 0xA247, "HwrCursorV33", "?", 0),
            new TrapInfo( 0xA248, "HwrBatteryLevel", "?", 0),
            new TrapInfo( 0xA249, "HwrDelay", "?", 0),
            new TrapInfo( 0xA24A, "HwrEnableDataWrites", "?", 0),
            new TrapInfo( 0xA24B, "HwrDisableDataWrites", "?", 0),
            new TrapInfo( 0xA24C, "HwrLCDBaseAddrV33", "?", 0),
            new TrapInfo( 0xA24D, "HwrDisplayDrawBootScreen", "?", 0),
            new TrapInfo( 0xA24E, "HwrTimerSleep", "?", 0),
            new TrapInfo( 0xA24F, "HwrTimerWake", "?", 0),
            new TrapInfo( 0xA250, "HwrDisplayWake", "?", 0),
            new TrapInfo( 0xA251, "HwrIRQ1Handler", "?", 0),
            new TrapInfo( 0xA252, "HwrIRQ2Handler", "?", 0),
            new TrapInfo( 0xA253, "HwrIRQ3Handler", "?", 0),
            new TrapInfo( 0xA254, "HwrIRQ4Handler", "?", 0),
            new TrapInfo( 0xA255, "HwrIRQ5Handler", "?", 0),
            new TrapInfo( 0xA256, "HwrIRQ6Handler", "?", 0),
            new TrapInfo( 0xA257, "HwrDockSignals", "?", 0),
            new TrapInfo( 0xA258, "HwrPluggedIn", "?", 0),
            new TrapInfo( 0xA259, "Crc16CalcBlock", "W", 3, "vp", "W", "W"),
            new TrapInfo( 0xA25A, "SelectDayV10", "?", 0),
            new TrapInfo( 0xA25B, "SelectTimeV33", "?", 0),
            new TrapInfo( 0xA25C, "DayDrawDaySelector", "?", 0),
            new TrapInfo( 0xA25D, "DayHandleEvent", "?", 0),
            new TrapInfo( 0xA25E, "DayDrawDays", "?", 0),
            new TrapInfo( 0xA25F, "DayOfWeek", "w", 3, "w", "w", "w"),
            new TrapInfo( 0xA260, "DaysInMonth", "w", 2, "w", "w"),
            new TrapInfo( 0xA261, "DayOfMonth", "?", 0),
            new TrapInfo( 0xA262, "DateDaysToDate", "?", 0),
            new TrapInfo( 0xA263, "DateToDays", "L", 1, "W"),
            new TrapInfo( 0xA264, "DateAdjust", "v", 2, "Wp", "l"),
            new TrapInfo( 0xA265, "DateSecondsToDate", "v", 2, "L", "Wp"),
            new TrapInfo( 0xA266, "DateToAscii", "v", 5, "B", "B", "W", "B", "cp"),
            new TrapInfo( 0xA267, "DateToDOWDMFormat", "v", 5, "B", "B", "W", "B", "cp"),
            new TrapInfo( 0xA268, "TimeToAscii", "v", 4, "B", "B", "W", "cp"),
            new TrapInfo( 0xA269, "Find", "?", 0),
            new TrapInfo( 0xA26A, "FindStrInStr", "?", 0),
            new TrapInfo( 0xA26B, "FindSaveMatch", "?", 0),
            new TrapInfo( 0xA26C, "FindGetLineBounds", "?", 0),
            new TrapInfo( 0xA26D, "FindDrawHeader", "?", 0),
            new TrapInfo( 0xA26E, "PenOpen", "?", 0),
            new TrapInfo( 0xA26F, "PenClose", "?", 0),
            new TrapInfo( 0xA270, "PenGetRawPen", "?", 0),
            new TrapInfo( 0xA271, "PenCalibrate", "?", 0),
            new TrapInfo( 0xA272, "PenRawToScreen", "?", 0),
            new TrapInfo( 0xA273, "PenScreenToRaw", "?", 0),
            new TrapInfo( 0xA274, "PenResetCalibration", "?", 0),
            new TrapInfo( 0xA275, "PenSleep", "W", 0),
            new TrapInfo( 0xA276, "PenWake", "W", 0),
            new TrapInfo( 0xA277, "ResLoadForm", "?", 0),
            new TrapInfo( 0xA278, "ResLoadMenu", "?", 0),
            new TrapInfo( 0xA279, "FtrInit", "?", 0),
            new TrapInfo( 0xA27A, "FtrUnregister", "W", 2, "L", "W"),
            new TrapInfo( 0xA27B, "FtrGet", "W", 3, "4", "W", "Lp"),
            new TrapInfo( 0xA27C, "FtrSet", "W", 3, "4", "W", "L"),
            new TrapInfo( 0xA27D, "FtrGetByIndex", "?", 0),
            new TrapInfo( 0xA27E, "GrfInit", "?", 0),
            new TrapInfo( 0xA27F, "GrfFree", "?", 0),
            new TrapInfo( 0xA280, "GrfGetState", "?", 0),
            new TrapInfo( 0xA281, "GrfSetState", "W", 3, "B", "B", "B"),
            new TrapInfo( 0xA282, "GrfFlushPoints", "?", 0),
            new TrapInfo( 0xA283, "GrfAddPoint", "?", 0),
            new TrapInfo( 0xA284, "GrfInitState", "?", 0),
            new TrapInfo( 0xA285, "GrfCleanState", "?", 0),
            new TrapInfo( 0xA286, "GrfMatch", "?", 0),
            new TrapInfo( 0xA287, "GrfGetMacro", "?", 0),
            new TrapInfo( 0xA288, "GrfFilterPoints", "?", 0),
            new TrapInfo( 0xA289, "GrfGetNumPoints", "?", 0),
            new TrapInfo( 0xA28A, "GrfGetPoint", "?", 0),
            new TrapInfo( 0xA28B, "GrfFindBranch", "?", 0),
            new TrapInfo( 0xA28C, "GrfMatchGlyph", "?", 0),
            new TrapInfo( 0xA28D, "GrfGetGlyphMapping", "?", 0),
            new TrapInfo( 0xA28E, "GrfGetMacroName", "?", 0),
            new TrapInfo( 0xA28F, "GrfDeleteMacro", "?", 0),
            new TrapInfo( 0xA290, "GrfAddMacro", "?", 0),
            new TrapInfo( 0xA291, "GrfGetAndExpandMacro", "?", 0),
            new TrapInfo( 0xA292, "GrfProcessStroke", "?", 0),
            new TrapInfo( 0xA293, "GrfFieldChange", "?", 0),
            new TrapInfo( 0xA294, "GetCharSortValue", "?", 0),
            new TrapInfo( 0xA295, "GetCharAttr", "?", 0),
            new TrapInfo( 0xA296, "GetCharCaselessValue", "?", 0),
            new TrapInfo( 0xA297, "PwdExists", "?", 0),
            new TrapInfo( 0xA298, "PwdVerify", "?", 0),
            new TrapInfo( 0xA299, "PwdSet", "?", 0),
            new TrapInfo( 0xA29A, "PwdRemove", "?", 0),
            new TrapInfo( 0xA29B, "GsiInitialize", "?", 0),
            new TrapInfo( 0xA29C, "GsiSetLocation", "?", 0),
            new TrapInfo( 0xA29D, "GsiEnable", "?", 0),
            new TrapInfo( 0xA29E, "GsiEnabled", "?", 0),
            new TrapInfo( 0xA29F, "GsiSetShiftState", "?", 0),
            new TrapInfo( 0xA2A0, "KeyInit", "?", 0),
            new TrapInfo( 0xA2A1, "KeyHandleInterrupt", "?", 0),
            new TrapInfo( 0xA2A2, "KeyCurrentState", "L", 0),
            new TrapInfo( 0xA2A3, "KeyResetDoubleTap", "?", 0),
            new TrapInfo( 0xA2A4, "KeyRates", "W", 5, "B", "Wp", "Wp", "Wp", "Bp"),
            new TrapInfo( 0xA2A5, "KeySleep", "?", 0),
            new TrapInfo( 0xA2A6, "KeyWake", "?", 0),
            new TrapInfo( 0xA2A7, "DlkControl", "?", 0),
            new TrapInfo( 0xA2A8, "DlkStartServer", "?", 0),
            new TrapInfo( 0xA2A9, "DlkGetSyncInfo", "W", 6, "Lp", "Lp", "p", "cp", "cp", "lp"),
            new TrapInfo( 0xA2AA, "DlkSetLogEntry", "?", 0),
            new TrapInfo( 0xA2AB, "IntlDispatch", "?", 0),
            new TrapInfo( 0xA2AC, "SysLibLoad", "W", 3, "4", "4", "Wp"),
            new TrapInfo( 0xA2AD, "SndPlaySmf", "W", 7, "vp", "B", "Bp", "p", "p", "p", "B"),
            new TrapInfo( 0xA2AE, "SndCreateMidiList", "B", 4, "L", "B", "Wp", "pp"),
            new TrapInfo( 0xA2AF, "AbtShowAbout", "v", 1, "4"),
            new TrapInfo( 0xA2B0, "MdmDial", "?", 0),
            new TrapInfo( 0xA2B1, "MdmHangUp", "?", 0),
            new TrapInfo( 0xA2B2, "DmSearchRecord", "?", 0),
            new TrapInfo( 0xA2B3, "SysInsertionSort", "?", 0),
            new TrapInfo( 0xA2B4, "DmInsertionSort", "W", 3, "p", "pp", "w"),
            new TrapInfo( 0xA2B5, "LstSetTopItem", "v", 2, "lstp", "w"),
            new TrapInfo( 0xA2B6, "SclSetScrollBar", "v", 5, "sclp", "w", "w", "w", "w"),
            new TrapInfo( 0xA2B7, "SclDrawScrollBar", "v", 1, "sclp"),
            new TrapInfo( 0xA2B8, "SclHandleEvent", "B", 2, "sclp", "evtp"),
            new TrapInfo( 0xA2B9, "SysMailboxCreate", "?", 0),
            new TrapInfo( 0xA2BA, "SysMailboxDelete", "?", 0),
            new TrapInfo( 0xA2BB, "SysMailboxFlush", "?", 0),
            new TrapInfo( 0xA2BC, "SysMailboxSend", "?", 0),
            new TrapInfo( 0xA2BD, "SysMailboxWait", "?", 0),
            new TrapInfo( 0xA2BE, "SysTaskWait", "?", 0),
            new TrapInfo( 0xA2BF, "SysTaskWake", "?", 0),
            new TrapInfo( 0xA2C0, "SysTaskWaitClr", "?", 0),
            new TrapInfo( 0xA2C1, "SysTaskSuspend", "?", 0),
            new TrapInfo( 0xA2C2, "SysTaskResume", "?", 0),
            new TrapInfo( 0xA2C3, "CategoryCreateList", "v", 8, "p", "lstp", "W", "B", "B", "B", "L", "B"),
            new TrapInfo( 0xA2C4, "CategoryFreeList", "v", 4, "p", "lstp", "B", "L"),
            new TrapInfo( 0xA2C5, "CategoryEditV20", "B", 3, "p", "Wp", "L"),
            new TrapInfo( 0xA2C6, "CategorySelect", "B", 9, "p", "frmp", "W", "W", "B", "Wp", "S", "B", "L"),
            new TrapInfo( 0xA2C7, "DmDeleteCategory", "W", 2, "p", "W"),
            new TrapInfo( 0xA2C8, "SysEvGroupCreate", "?", 0),
            new TrapInfo( 0xA2C9, "SysEvGroupSignal", "?", 0),
            new TrapInfo( 0xA2CA, "SysEvGroupRead", "?", 0),
            new TrapInfo( 0xA2CB, "SysEvGroupWait", "?", 0),
            new TrapInfo( 0xA2CC, "EvtEventAvail", "B", 0),
            new TrapInfo( 0xA2CD, "EvtSysEventAvail", "B", 1, "B"),
            new TrapInfo( 0xA2CE, "StrNCopy", "cp", 3, "cp", "cp", "w"),
            new TrapInfo( 0xA2CF, "KeySetMask", "L", 1, "L"),
            new TrapInfo( 0xA2D0, "SelectDay", "?", 0),
            new TrapInfo( 0xA2D1, "PrefGetPreference", "L", 1, "B"),
            new TrapInfo( 0xA2D2, "PrefSetPreference", "?", 0),
            new TrapInfo( 0xA2D3, "PrefGetAppPreferences", "w", 5, "4", "W", "vp", "Wp", "B"),
            new TrapInfo( 0xA2D4, "PrefSetAppPreferences", "v", 6, "4", "W", "w", "vp", "W", "B"),
            new TrapInfo( 0xA2D5, "FrmPointInTitle", "?", 0),
            new TrapInfo( 0xA2D6, "StrNCat", "cp", 3, "S", "cp", "w"),
            new TrapInfo( 0xA2D7, "MemCmp", "w", 3, "vp", "vp", "l"),
            new TrapInfo( 0xA2D8, "TblSetColumnEditIndicator", "v", 3, "tblp", "w", "B"),
            new TrapInfo( 0xA2D9, "FntWordWrap", "W", 2, "cp", "W"),
            new TrapInfo( 0xA2DA, "FldGetScrollValues", "v", 4, "fldp", "Wp", "Wp", "Wp"),
            new TrapInfo( 0xA2DB, "SysCreateDataBaseList", "B", 5, "L", "L", "Wp", "pp", "B"),
            new TrapInfo( 0xA2DC, "SysCreatePanelList", "?", 0),
            new TrapInfo( 0xA2DD, "DlkDispatchRequest", "?", 0),
            new TrapInfo( 0xA2DE, "StrPrintF", "?", 0),
            new TrapInfo( 0xA2DF, "StrVPrintF", "w", 2, "cp", "S"),
            new TrapInfo( 0xA2E0, "PrefOpenPreferenceDB", "p", 1, "B"),
            new TrapInfo( 0xA2E1, "SysGraffitiReferenceDialog", "?", 0),
            new TrapInfo( 0xA2E2, "SysKeyboardDialog", "v", 1, "B"),
            new TrapInfo( 0xA2E3, "FntWordWrapReverseNLines", "v", 4, "cp", "W", "Wp", "Wp"),
            new TrapInfo( 0xA2E4, "FntGetScrollValues", "v", 5, "cp", "W", "W", "Wp", "Wp"),
            new TrapInfo( 0xA2E5, "TblSetRowStaticHeight", "v", 3, "tblp", "w", "B"),
            new TrapInfo( 0xA2E6, "TblHasScrollBar", "v", 2, "tblp", "B"),
            new TrapInfo( 0xA2E7, "SclGetScrollBar", "v", 5, "sclp", "wp", "wp", "wp", "wp"),
            new TrapInfo( 0xA2E8, "FldGetNumberOfBlankLines", "W", 1, "fldp"),
            new TrapInfo( 0xA2E9, "SysTicksPerSecond", "W", 0),
            new TrapInfo( 0xA2EA, "HwrBacklightV33", "?", 0),
            new TrapInfo( 0xA2EB, "DmDatabaseProtect", "W", 3, "W", "L", "B"),
            new TrapInfo( 0xA2EC, "TblSetBounds", "v", 2, "tblp", "rctp"),
            new TrapInfo( 0xA2ED, "StrNCompare", "w", 3, "cp", "cp", "l"),
            new TrapInfo( 0xA2EE, "StrNCaselessCompare", "w", 3, "cp", "cp", "l"),
            new TrapInfo( 0xA2EF, "PhoneNumberLookup", "?", 0),
            new TrapInfo( 0xA2F0, "FrmSetMenu", "v", 2, "frmp", "W"),
            new TrapInfo( 0xA2F1, "EncDigestMD5", "?", 0),
            new TrapInfo( 0xA2F2, "DmFindSortPosition", "W", 5, "p", "vp", "p", "pp", "w"),
            new TrapInfo( 0xA2F3, "SysBinarySearch", "?", 0),
            new TrapInfo( 0xA2F4, "SysErrString", "?", 0),
            new TrapInfo( 0xA2F5, "SysStringByIndex", "cp", 4, "W", "W", "S", "W"),
            new TrapInfo( 0xA2F6, "EvtAddUniqueEventToQueue", "v", 3, "evtp", "L", "B"),
            new TrapInfo( 0xA2F7, "StrLocalizeNumber", "cp", 3, "cp", "c", "c"),
            new TrapInfo( 0xA2F8, "StrDelocalizeNumber", "cp", 3, "cp", "c", "c"),
            new TrapInfo( 0xA2F9, "LocGetNumberSeparators", "v", 3, "B", "cp", "cp"),
            new TrapInfo( 0xA2FA, "MenuSetActiveMenuRscID", "v", 1, "W"),
            new TrapInfo( 0xA2FB, "LstScrollList", "B", 3, "lstp", "B", "w"),
            new TrapInfo( 0xA2FC, "CategoryInitialize", "v", 2, "p", "W"),
            new TrapInfo( 0xA2FD, "EncDigestMD4", "?", 0),
            new TrapInfo( 0xA2FE, "EncDES", "?", 0),
            new TrapInfo( 0xA2FF, "LstGetVisibleItems", "w", 1, "lstp"),
            new TrapInfo( 0xA300, "WinSetBounds", "v", 2, "winp", "rctp"),
            new TrapInfo( 0xA301, "CategorySetName", "v", 3, "p", "W", "S"),
            new TrapInfo( 0xA302, "FldSetInsertionPoint", "v", 2, "fldp", "W"),
            new TrapInfo( 0xA303, "FrmSetObjectBounds", "v", 3, "frmp", "W", "rctp"),
            new TrapInfo( 0xA304, "WinSetColors", "v", 4, "p", "p", "p", "p"),
            new TrapInfo( 0xA305, "FlpDispatch", "?", 0),
            new TrapInfo( 0xA306, "FlpEmDispatch", "?", 0),
            new TrapInfo( 0xA307, "ExgInit", "?", 0),
            new TrapInfo( 0xA308, "ExgConnect", "?", 0),
            new TrapInfo( 0xA309, "ExgPut", "?", 0),
            new TrapInfo( 0xA30A, "ExgGet", "?", 0),
            new TrapInfo( 0xA30B, "ExgAccept", "?", 0),
            new TrapInfo( 0xA30C, "ExgDisconnect", "?", 0),
            new TrapInfo( 0xA30D, "ExgSend", "?", 0),
            new TrapInfo( 0xA30E, "ExgReceive", "?", 0),
            new TrapInfo( 0xA30F, "ExgRegisterData", "?", 0),
            new TrapInfo( 0xA310, "ExgNotifyReceiveV35", "?", 0),
            new TrapInfo( 0xA311, "SysReserved30Trap2", "?", 0),
            new TrapInfo( 0xA312, "PrgStartDialogV31", "?", 0),
            new TrapInfo( 0xA313, "PrgStopDialog", "?", 0),
            new TrapInfo( 0xA314, "PrgUpdateDialog", "?", 0),
            new TrapInfo( 0xA315, "PrgHandleEvent", "?", 0),
            new TrapInfo( 0xA316, "ImcReadFieldNoSemicolon", "?", 0),
            new TrapInfo( 0xA317, "ImcReadFieldQuotablePrintable", "?", 0),
            new TrapInfo( 0xA318, "ImcReadPropertyParameter", "?", 0),
            new TrapInfo( 0xA319, "ImcSkipAllPropertyParameters", "?", 0),
            new TrapInfo( 0xA31A, "ImcReadWhiteSpace", "?", 0),
            new TrapInfo( 0xA31B, "ImcWriteQuotedPrintable", "?", 0),
            new TrapInfo( 0xA31C, "ImcWriteNoSemicolon", "?", 0),
            new TrapInfo( 0xA31D, "ImcStringIsAscii", "?", 0),
            new TrapInfo( 0xA31E, "TblGetItemFont", "B", 3, "tblp", "w", "w"),
            new TrapInfo( 0xA31F, "TblSetItemFont", "v", 4, "tblp", "w", "w", "B"),
            new TrapInfo( 0xA320, "FontSelect", "B", 1, "B"),
            new TrapInfo( 0xA321, "FntDefineFont", "W", 2, "B", "p"),
            new TrapInfo( 0xA322, "CategoryEdit", "B", 4, "p", "Wp", "L", "B"),
            new TrapInfo( 0xA323, "SysGetOSVersionString", "cp", 0),
            new TrapInfo( 0xA324, "SysBatteryInfo", "W", 7, "B", "Wp", "Wp", "wp", "p", "Bp", "Bp"),
            new TrapInfo( 0xA325, "SysUIBusy", "?", 0),
            new TrapInfo( 0xA326, "WinValidateHandle", "B", 1, "winp"),
            new TrapInfo( 0xA327, "FrmValidatePtr", "?", 0),
            new TrapInfo( 0xA328, "CtlValidatePointer", "B", 1, "ctlp"),
            new TrapInfo( 0xA329, "WinMoveWindowAddr", "?", 0),
            new TrapInfo( 0xA32A, "FrmAddSpaceForObject", "?", 0),
            new TrapInfo( 0xA32B, "FrmNewForm", "frmp", 10, "W", "S", "w", "w", "w", "w", "B", "W", "W", "W"),
            new TrapInfo( 0xA32C, "CtlNewControl", "?", 0),
            new TrapInfo( 0xA32D, "FldNewField", "?", 0),
            new TrapInfo( 0xA32E, "LstNewList", "?", 0),
            new TrapInfo( 0xA32F, "FrmNewLabel", "?", 0),
            new TrapInfo( 0xA330, "FrmNewBitmap", "?", 0),
            new TrapInfo( 0xA331, "FrmNewGadget", "gadp", 6, "p", "W", "w", "w", "w", "w"),
            new TrapInfo( 0xA332, "FileOpen", "p", 6, "W", "S", "L", "L", "L", "Wp"),
            new TrapInfo( 0xA333, "FileClose", "W", 1, "p"),
            new TrapInfo( 0xA334, "FileDelete", "W", 2, "W", "S"),
            new TrapInfo( 0xA335, "FileReadLow", "l", 7, "p", "vp", "l", "B", "l", "l", "Wp"),
            new TrapInfo( 0xA336, "FileWrite", "l", 5, "p", "vp", "l", "l", "Wp"),
            new TrapInfo( 0xA337, "FileSeek", "W", 3, "p", "l", "B"),
            new TrapInfo( 0xA338, "FileTell", "l", 3, "p", "lp", "Wp"),
            new TrapInfo( 0xA339, "FileTruncate", "W", 2, "p", "l"),
            new TrapInfo( 0xA33A, "FileControl", "W", 4, "B", "p", "vp", "lp"),
            new TrapInfo( 0xA33B, "FrmActiveState", "W", 2, "p", "B"),
            new TrapInfo( 0xA33C, "SysGetAppInfo", "p", 2, "p", "p"),
            new TrapInfo( 0xA33D, "SysGetStackInfo", "B", 2, "p", "p"),
            new TrapInfo( 0xA33E, "WinScreenMode", "W", 5, "B", "Lp", "Lp", "Lp", "Bp"),
            new TrapInfo( 0xA33F, "HwrLCDGetDepthV33", "?", 0),
            new TrapInfo( 0xA340, "HwrGetROMToken", "W", 4, "W", "L", "p", "Wp"),
            new TrapInfo( 0xA341, "DbgControl", "?", 0),
            new TrapInfo( 0xA342, "ExgDBRead", "?", 0),
            new TrapInfo( 0xA343, "ExgDBWrite", "?", 0),
            new TrapInfo( 0xA344, "HostControl", "?", 0),
            new TrapInfo( 0xA345, "FrmRemoveObject", "?", 0),
            new TrapInfo( 0xA346, "SysReserved30Trap1", "?", 0),
            new TrapInfo( 0xA347, "ExpansionDispatch", "?", 0),
            new TrapInfo( 0xA348, "FileSystemDispatch", "?", 0),
            new TrapInfo( 0xA349, "OEMDispatch", "?", 0),
            new TrapInfo( 0xA34A, "HwrLCDContrastV33", "?", 0),
            new TrapInfo( 0xA34B, "SysLCDContrast", "B", 2, "B", "B"),
            new TrapInfo( 0xA34C, "UIContrastAdjust", "?", 0),
            new TrapInfo( 0xA34D, "HwrDockStatus", "?", 0),
            new TrapInfo( 0xA34E, "FntWidthToOffset", "w", 5, "cp", "W", "w", "Bp", "wp"),
            new TrapInfo( 0xA34F, "SelectOneTime", "?", 0),
            new TrapInfo( 0xA350, "WinDrawChar", "v", 3, "W", "w", "w"),
            new TrapInfo( 0xA351, "WinDrawTruncChars", "v", 5, "S", "w", "w", "w", "w"),
            new TrapInfo( 0xA352, "SysNotifyInit", "?", 0),
            new TrapInfo( 0xA353, "SysNotifyRegister", "W", 6, "W", "L", "L", "p", "b", "vp"),
            new TrapInfo( 0xA354, "SysNotifyUnregister", "W", 4, "W", "L", "L", "b"),
            new TrapInfo( 0xA355, "SysNotifyBroadcast", "W", 1, "p"),
            new TrapInfo( 0xA356, "SysNotifyBroadcastDeferred", "W", 2, "p", "w"),
            new TrapInfo( 0xA357, "SysNotifyDatabaseAdded", "?", 0),
            new TrapInfo( 0xA358, "SysNotifyDatabaseRemoved", "?", 0),
            new TrapInfo( 0xA359, "SysWantEvent", "?", 0),
            new TrapInfo( 0xA35A, "FtrPtrNew", "W", 4, "L", "W", "L", "p"),
            new TrapInfo( 0xA35B, "FtrPtrFree", "W", 2, "L", "W"),
            new TrapInfo( 0xA35C, "FtrPtrResize", "?", 0),
            new TrapInfo( 0xA35D, "SysReserved31Trap1", "?", 0),
            new TrapInfo( 0xA35E, "HwrNVPrefSet", "?", 0),
            new TrapInfo( 0xA35F, "HwrNVPrefGet", "?", 0),
            new TrapInfo( 0xA360, "FlashInit", "?", 0),
            new TrapInfo( 0xA361, "FlashCompress", "?", 0),
            new TrapInfo( 0xA362, "FlashErase", "?", 0),
            new TrapInfo( 0xA363, "FlashProgram", "?", 0),
            new TrapInfo( 0xA364, "AlmTimeChange", "?", 0),
            new TrapInfo( 0xA365, "ErrAlertCustom", "?", 0),
            new TrapInfo( 0xA366, "PrgStartDialog", "?", 0),
            new TrapInfo( 0xA367, "SerialDispatch", "?", 0),
            new TrapInfo( 0xA368, "HwrBattery", "?", 0),
            new TrapInfo( 0xA369, "DmGetDatabaseLockState", "v", 4, "p", "Bp", "Lp", "Lp"),
            new TrapInfo( 0xA36A, "CncGetProfileList", "?", 0),
            new TrapInfo( 0xA36B, "CncGetProfileInfo", "?", 0),
            new TrapInfo( 0xA36C, "CncAddProfile", "?", 0),
            new TrapInfo( 0xA36D, "CncDeleteProfile", "?", 0),
            new TrapInfo( 0xA36E, "SndPlaySmfResource", "?", 0),
            new TrapInfo( 0xA36F, "MemPtrDataStorage", "B", 1, "vp"),
            new TrapInfo( 0xA370, "ClipboardAppendItem", "?", 0),
            new TrapInfo( 0xA371, "WiCmdV32", "?", 0),
            new TrapInfo( 0xA372, "HwrDisplayAttributes", "?", 0),
            new TrapInfo( 0xA373, "HwrDisplayDoze", "?", 0),
            new TrapInfo( 0xA374, "HwrDisplayPalette", "?", 0),
            new TrapInfo( 0xA375, "BltFindIndexes", "?", 0),
            new TrapInfo( 0xA376, "BmpGetBits", "vp", 1, "bmpp"),
            new TrapInfo( 0xA377, "BltCopyRectangle", "?", 0),
            new TrapInfo( 0xA378, "BltDrawChars", "?", 0),
            new TrapInfo( 0xA379, "BltLineRoutine", "?", 0),
            new TrapInfo( 0xA37A, "BltRectangleRoutine", "?", 0),
            new TrapInfo( 0xA37B, "ScrCompress", "?", 0),
            new TrapInfo( 0xA37C, "ScrDecompress", "?", 0),
            new TrapInfo( 0xA37D, "SysLCDBrightness", "B", 2, "B", "B"),
            new TrapInfo( 0xA37E, "WinPaintChar", "v", 3, "W", "w", "w"),
            new TrapInfo( 0xA37F, "WinPaintChars", "v", 4, "S", "w", "w", "w"),
            new TrapInfo( 0xA380, "WinPaintBitmap", "v", 3, "bmpp", "w", "w"),
            new TrapInfo( 0xA381, "WinGetPixel", "B", 2, "w", "w"),
            new TrapInfo( 0xA382, "WinPaintPixel", "v", 2, "w", "w"),
            new TrapInfo( 0xA383, "WinDrawPixel", "v", 2, "w", "w"),
            new TrapInfo( 0xA384, "WinErasePixel", "v", 2, "w", "w"),
            new TrapInfo( 0xA385, "WinInvertPixel", "v", 2, "w", "w"),
            new TrapInfo( 0xA386, "WinPaintPixels", "?", 0),
            new TrapInfo( 0xA387, "WinPaintLines", "?", 0),
            new TrapInfo( 0xA388, "WinPaintLine", "v", 4, "w", "w", "w", "w"),
            new TrapInfo( 0xA389, "WinPaintRectangle", "v", 2, "rctp", "W"),
            new TrapInfo( 0xA38A, "WinPaintRectangleFrame", "v", 2, "W", "rctp"),
            new TrapInfo( 0xA38B, "WinPaintPolygon", "?", 0),
            new TrapInfo( 0xA38C, "WinDrawPolygon", "?", 0),
            new TrapInfo( 0xA38D, "WinErasePolygon", "?", 0),
            new TrapInfo( 0xA38E, "WinInvertPolygon", "?", 0),
            new TrapInfo( 0xA38F, "WinFillPolygon", "?", 0),
            new TrapInfo( 0xA390, "WinPaintArc", "?", 0),
            new TrapInfo( 0xA391, "WinDrawArc", "?", 0),
            new TrapInfo( 0xA392, "WinEraseArc", "?", 0),
            new TrapInfo( 0xA393, "WinInvertArc", "?", 0),
            new TrapInfo( 0xA394, "WinFillArc", "?", 0),
            new TrapInfo( 0xA395, "WinPushDrawState", "v", 0),
            new TrapInfo( 0xA396, "WinPopDrawState", "v", 0),
            new TrapInfo( 0xA397, "WinSetDrawMode", "B", 1, "B"),
            new TrapInfo( 0xA398, "WinSetForeColor", "B", 1, "B"),
            new TrapInfo( 0xA399, "WinSetBackColor", "B", 1, "B"),
            new TrapInfo( 0xA39A, "WinSetTextColor", "B", 1, "B"),
            new TrapInfo( 0xA39B, "WinGetPatternType", "B", 0),
            new TrapInfo( 0xA39C, "WinSetPatternType", "v", 1, "B"),
            new TrapInfo( 0xA39D, "WinPalette", "W", 4, "B", "w", "W", "p"),
            new TrapInfo( 0xA39E, "WinRGBToIndex", "B", 1, "p"),
            new TrapInfo( 0xA39F, "WinIndexToRGB", "v", 2, "B", "p"),
            new TrapInfo( 0xA3A0, "WinScreenLock", "Bp", 1, "B"),
            new TrapInfo( 0xA3A1, "WinScreenUnlock", "v", 0),
            new TrapInfo( 0xA3A2, "WinGetBitmap", "bmpp", 1, "winp"),
            new TrapInfo( 0xA3A3, "UIColorInit", "?", 0),
            new TrapInfo( 0xA3A4, "UIColorGetTableEntryIndex", "B", 1, "B"),
            new TrapInfo( 0xA3A5, "UIColorGetTableEntryRGB", "v", 2, "B", "p"),
            new TrapInfo( 0xA3A6, "UIColorSetTableEntry", "W", 2, "B", "p"),
            new TrapInfo( 0xA3A7, "UIColorPushTable", "W", 0),
            new TrapInfo( 0xA3A8, "UIColorPopTable", "W", 0),
            new TrapInfo( 0xA3A9, "CtlNewGraphicControl", "?", 0),
            new TrapInfo( 0xA3AA, "TblGetItemPtr", "vp", 3, "tblp", "w", "w"),
            new TrapInfo( 0xA3AB, "UIBrightnessAdjust", "?", 0),
            new TrapInfo( 0xA3AC, "UIPickColor", "B", 5, "Bp", "p", "W", "S", "S"),
            new TrapInfo( 0xA3AD, "EvtSetAutoOffTimer", "?", 0),
            new TrapInfo( 0xA3AE, "TsmDispatch", "?", 0),
            new TrapInfo( 0xA3AF, "OmDispatch", "?", 0),
            new TrapInfo( 0xA3B0, "DmOpenDBNoOverlay", "p", 3, "W", "L", "W"),
            new TrapInfo( 0xA3B1, "DmOpenDBWithLocale", "?", 0),
            new TrapInfo( 0xA3B2, "ResLoadConstant", "L", 1, "W"),
            new TrapInfo( 0xA3B3, "HwrPreDebugInit", "?", 0),
            new TrapInfo( 0xA3B4, "HwrResetNMI", "?", 0),
            new TrapInfo( 0xA3B5, "HwrResetPWM", "?", 0),
            new TrapInfo( 0xA3B6, "KeyBootKeys", "?", 0),
            new TrapInfo( 0xA3B7, "DbgSerDrvOpen", "?", 0),
            new TrapInfo( 0xA3B8, "DbgSerDrvClose", "?", 0),
            new TrapInfo( 0xA3B9, "DbgSerDrvControl", "?", 0),
            new TrapInfo( 0xA3BA, "DbgSerDrvStatus", "?", 0),
            new TrapInfo( 0xA3BB, "DbgSerDrvWriteChar", "?", 0),
            new TrapInfo( 0xA3BC, "DbgSerDrvReadChar", "?", 0),
            new TrapInfo( 0xA3BD, "HwrPostDebugInit", "?", 0),
            new TrapInfo( 0xA3BE, "HwrIdentifyFeatures", "?", 0),
            new TrapInfo( 0xA3BF, "HwrModelSpecificInit", "?", 0),
            new TrapInfo( 0xA3C0, "HwrModelInitStage2", "?", 0),
            new TrapInfo( 0xA3C1, "HwrInterruptsInit", "?", 0),
            new TrapInfo( 0xA3C2, "HwrSoundOn", "?", 0),
            new TrapInfo( 0xA3C3, "HwrSoundOff", "?", 0),
            new TrapInfo( 0xA3C4, "SysKernelClockTick", "?", 0),
            new TrapInfo( 0xA3C5, "MenuEraseMenu", "?", 0),
            new TrapInfo( 0xA3C6, "SelectTime", "?", 0),
            new TrapInfo( 0xA3C7, "MenuCmdBarAddButton", "W", 5, "B", "W", "B", "L", "S"),
            new TrapInfo( 0xA3C8, "MenuCmdBarGetButtonData", "B", 5, "w", "Wp", "Bp", "Lp", "cp"),
            new TrapInfo( 0xA3C9, "MenuCmdBarDisplay", "v", 0),
            new TrapInfo( 0xA3CA, "HwrGetSilkscreenID", "?", 0),
            new TrapInfo( 0xA3CB, "EvtGetSilkscreenAreaList", "p", 1, "Wp"),
            new TrapInfo( 0xA3CC, "SysFatalAlertInit", "?", 0),
            new TrapInfo( 0xA3CD, "DateTemplateToAscii", "W", 6, "S", "B", "B", "W", "cp", "w"),
            new TrapInfo( 0xA3CE, "SecVerifyPW", "?", 0),
            new TrapInfo( 0xA3CF, "SecSelectViewStatus", "B", 0),
            new TrapInfo( 0xA3D0, "TblSetColumnMasked", "v", 3, "tblp", "w", "B"),
            new TrapInfo( 0xA3D1, "TblSetRowMasked", "v", 3, "tblp", "w", "B"),
            new TrapInfo( 0xA3D2, "TblRowMasked", "B", 2, "tblp", "w"),
            new TrapInfo( 0xA3D3, "FrmCustomResponseAlert", "?", 0),
            new TrapInfo( 0xA3D4, "FrmNewGsi", "?", 0),
            new TrapInfo( 0xA3D5, "MenuShowItem", "B", 1, "W"),
            new TrapInfo( 0xA3D6, "MenuHideItem", "B", 1, "W"),
            new TrapInfo( 0xA3D7, "MenuAddItem", "W", 4, "W", "W", "c", "S"),
            new TrapInfo( 0xA3D8, "FrmSetGadgetHandler", "v", 3, "frmp", "W", "p"),
            new TrapInfo( 0xA3D9, "CtlSetGraphics", "v", 3, "ctlp", "W", "W"),
            new TrapInfo( 0xA3DA, "CtlGetSliderValues", "v", 5, "ctlp", "Wp", "Wp", "Wp", "Wp"),
            new TrapInfo( 0xA3DB, "CtlSetSliderValues", "v", 5, "ctlp", "Wp", "Wp", "Wp", "Wp"),
            new TrapInfo( 0xA3DC, "CtlNewSliderControl", "?", 0),
            new TrapInfo( 0xA3DD, "BmpCreate", "bmpp", 5, "w", "w", "B", "p", "Wp"),
            new TrapInfo( 0xA3DE, "BmpDelete", "W", 1, "bmpp"),
            new TrapInfo( 0xA3DF, "BmpCompress", "W", 2, "bmpp", "B"),
            new TrapInfo( 0xA3E0, "BmpGetColortable", "p", 1, "bmpp"),
            new TrapInfo( 0xA3E1, "BmpSize", "W", 1, "bmpp"),
            new TrapInfo( 0xA3E2, "BmpBitsSize", "W", 1, "bmpp"),
            new TrapInfo( 0xA3E3, "BmpColortableSize", "W", 1, "bmpp"),
            new TrapInfo( 0xA3E4, "WinCreateBitmapWindow", "winp", 2, "bmpp", "Wp"),
            new TrapInfo( 0xA3E5, "EvtSetNullEventTick", "?", 0),
            new TrapInfo( 0xA3E6, "ExgDoDialog", "?", 0),
            new TrapInfo( 0xA3E7, "SysUICleanup", "?", 0),
            new TrapInfo( 0xA3E8, "WinSetForeColorRGB", "v", 2, "p", "p"),
            new TrapInfo( 0xA3E9, "WinSetBackColorRGB", "v", 2, "p", "p"),
            new TrapInfo( 0xA3EA, "WinSetTextColorRGB", "v", 2, "p", "p"),
            new TrapInfo( 0xA3EB, "WinGetPixelRGB", "W", 3, "w", "w", "p"),
            new TrapInfo( 0xA3EC, "HighDensityDispatch", "?", 0),
            new TrapInfo( 0xA3ED, "SysReserved40Trap2", "?", 0),
            new TrapInfo( 0xA3EE, "SysReserved40Trap3", "?", 0),
            new TrapInfo( 0xA3EF, "SysReserved40Trap4", "?", 0),
            new TrapInfo( 0xA3F0, "CncMgrDispatch", "?", 0),
            new TrapInfo( 0xA3F1, "SysNotifyBroadcastFromInterrupt", "?", 0),
            new TrapInfo( 0xA3F2, "EvtWakeupWithoutNilEvent", "?", 0),
            new TrapInfo( 0xA3F3, "StrCompareAscii", "w", 2, "S", "S"),
            new TrapInfo( 0xA3F4, "AccessorDispatch", "?", 0),
            new TrapInfo( 0xA3F5, "BltGetPixel", "?", 0),
            new TrapInfo( 0xA3F6, "BltPaintPixel", "?", 0),
            new TrapInfo( 0xA3F7, "ScrScreenInit", "?", 0),
            new TrapInfo( 0xA3F8, "ScrUpdateScreenBitmap", "?", 0),
            new TrapInfo( 0xA3F9, "ScrPalette", "?", 0),
            new TrapInfo( 0xA3FA, "ScrGetColortable", "?", 0),
            new TrapInfo( 0xA3FB, "ScrGetGrayPat", "?", 0),
            new TrapInfo( 0xA3FC, "ScrScreenLock", "?", 0),
            new TrapInfo( 0xA3FD, "ScrScreenUnlock", "?", 0),
            new TrapInfo( 0xA3FE, "FntPrvGetFontList", "?", 0),
            new TrapInfo( 0xA3FF, "ExgRegisterDatatype", "?", 0),
            new TrapInfo( 0xA400, "ExgNotifyReceive", "?", 0),
            new TrapInfo( 0xA401, "ExgNotifyGoto", "?", 0),
            new TrapInfo( 0xA402, "ExgRequest", "?", 0),
            new TrapInfo( 0xA403, "ExgSetDefaultApplication", "?", 0),
            new TrapInfo( 0xA404, "ExgGetDefaultApplication", "?", 0),
            new TrapInfo( 0xA405, "ExgGetTargetApplication", "?", 0),
            new TrapInfo( 0xA406, "ExgGetRegisteredApplications", "?", 0),
            new TrapInfo( 0xA407, "ExgGetRegisteredTypes", "?", 0),
            new TrapInfo( 0xA408, "ExgNotifyPreview", "?", 0),
            new TrapInfo( 0xA409, "ExgControl", "?", 0),
            new TrapInfo( 0xA40A, "LmDispatch", "?", 0),
            new TrapInfo( 0xA40B, "MemGetRomNVParams", "?", 0),
            new TrapInfo( 0xA40C, "FntWCharWidth", "w", 1, "W"),
            new TrapInfo( 0xA40D, "DmFindDatabaseWithTypeCreator", "?", 0),
            new TrapInfo( 0xA40E, "SelectTimeZone", "?", 0),
            new TrapInfo( 0xA40F, "TimeZoneToAscii", "?", 0),
            new TrapInfo( 0xA410, "StrNCompareAscii", "w", 3, "S", "S", "l"),
            new TrapInfo( 0xA411, "TimTimeZoneToUTC", "?", 0),
            new TrapInfo( 0xA412, "TimUTCToTimeZone", "?", 0),
            new TrapInfo( 0xA413, "PhoneNumberLookupCustom", "?", 0),
            new TrapInfo( 0xA414, "HwrDebugSelect", "?", 0),
            new TrapInfo( 0xA415, "BltRoundedRectangle", "?", 0),
            new TrapInfo( 0xA416, "BltRoundedRectangleFill", "?", 0),
            new TrapInfo( 0xA417, "WinPrvInitCanvas", "?", 0),
            new TrapInfo( 0xA418, "HwrCalcDynamicHeapSize", "?", 0),
            new TrapInfo( 0xA419, "HwrDebuggerEnter", "?", 0),
            new TrapInfo( 0xA41A, "HwrDebuggerExit", "?", 0),
            new TrapInfo( 0xA41B, "LstGetTopItem", "w", 1, "lstp"),
            new TrapInfo( 0xA41C, "HwrModelInitStage3", "?", 0),
            new TrapInfo( 0xA41D, "AttnIndicatorAllow", "?", 0),
            new TrapInfo( 0xA41E, "AttnIndicatorAllowed", "?", 0),
            new TrapInfo( 0xA41F, "AttnIndicatorEnable", "v", 1, "B"),
            new TrapInfo( 0xA420, "AttnIndicatorEnabled", "?", 0),
            new TrapInfo( 0xA421, "AttnIndicatorSetBlinkPattern", "?", 0),
            new TrapInfo( 0xA422, "AttnIndicatorGetBlinkPattern", "?", 0),
            new TrapInfo( 0xA423, "AttnIndicatorTicksTillNextBlink", "?", 0),
            new TrapInfo( 0xA424, "AttnIndicatorCheckBlink", "?", 0),
            new TrapInfo( 0xA425, "AttnInitialize", "?", 0),
            new TrapInfo( 0xA426, "AttnGetAttention", "?", 0),
            new TrapInfo( 0xA427, "AttnUpdate", "?", 0),
            new TrapInfo( 0xA428, "AttnForgetIt", "?", 0),
            new TrapInfo( 0xA429, "AttnGetCounts", "?", 0),
            new TrapInfo( 0xA42A, "AttnListOpen", "?", 0),
            new TrapInfo( 0xA42B, "AttnHandleEvent", "?", 0),
            new TrapInfo( 0xA42C, "AttnEffectOfEvent", "?", 0),
            new TrapInfo( 0xA42D, "AttnIterate", "?", 0),
            new TrapInfo( 0xA42E, "AttnDoSpecialEffects", "?", 0),
            new TrapInfo( 0xA42F, "AttnDoEmergencySpecialEffects", "?", 0),
            new TrapInfo( 0xA430, "AttnAllowClose", "?", 0),
            new TrapInfo( 0xA431, "AttnReopen", "?", 0),
            new TrapInfo( 0xA432, "AttnEnableNotification", "?", 0),
            new TrapInfo( 0xA433, "HwrLEDAttributes", "?", 0),
            new TrapInfo( 0xA434, "HwrVibrateAttributes", "?", 0),
            new TrapInfo( 0xA435, "SecGetPwdHint", "?", 0),
            new TrapInfo( 0xA436, "SecSetPwdHint", "?", 0),
            new TrapInfo( 0xA437, "HwrFlashWrite", "?", 0),
            new TrapInfo( 0xA438, "KeyboardStatusNew", "?", 0),
            new TrapInfo( 0xA439, "KeyboardStatusFree", "?", 0),
            new TrapInfo( 0xA43A, "KbdSetLayout", "?", 0),
            new TrapInfo( 0xA43B, "KbdGetLayout", "?", 0),
            new TrapInfo( 0xA43C, "KbdSetPosition", "?", 0),
            new TrapInfo( 0xA43D, "KbdGetPosition", "?", 0),
            new TrapInfo( 0xA43E, "KbdSetShiftState", "?", 0),
            new TrapInfo( 0xA43F, "KbdGetShiftState", "?", 0),
            new TrapInfo( 0xA440, "KbdDraw", "?", 0),
            new TrapInfo( 0xA441, "KbdErase", "?", 0),
            new TrapInfo( 0xA442, "KbdHandleEvent", "?", 0),
            new TrapInfo( 0xA443, "OEMDispatch2", "?", 0),
            new TrapInfo( 0xA444, "HwrCustom", "?", 0),
            new TrapInfo( 0xA445, "FrmGetActiveField", "fldp", 1, "frmp"),
            new TrapInfo( 0xA446, "SndPlaySmfIrregardless", "?", 0),
            new TrapInfo( 0xA447, "SndPlaySmfResourceIrregardless", "?", 0),
            new TrapInfo( 0xA448, "SndInterruptSmfIrregardless", "?", 0),
            new TrapInfo( 0xA449, "UdaMgrDispatch", "?", 0),
            new TrapInfo( 0xA44A, "PalmPrivate1", "?", 0),
            new TrapInfo( 0xA44B, "PalmPrivate2", "?", 0),
            new TrapInfo( 0xA44C, "PalmPrivate3", "?", 0),
            new TrapInfo( 0xA44D, "PalmPrivate4", "?", 0),
            new TrapInfo( 0xA44E, "BmpGetDimensions", "v", 4, "bmpp", "wp", "wp", "Wp"),
            new TrapInfo( 0xA44F, "BmpGetBitDepth", "B", 1, "bmpp"),
            new TrapInfo( 0xA450, "BmpGetNextBitmap", "bmpp", 1, "bmpp"),
            new TrapInfo( 0xA451, "TblGetNumberOfColumns", "w", 1, "tblp"),
            new TrapInfo( 0xA452, "TblGetTopRow", "w", 1, "tblp"),
            new TrapInfo( 0xA453, "TblSetSelection", "v", 3, "tblp", "w", "w"),
            new TrapInfo( 0xA454, "FrmGetObjectIndexFromPtr", "W", 2, "frmp", "vp"),
            new TrapInfo( 0xA455, "BmpGetSizes", "v", 3, "bmpp", "Lp", "Lp"),
            new TrapInfo( 0xA456, "WinGetBounds", "v", 2, "winp", "rctp"),
            new TrapInfo( 0xA457, "BltPaintPixels", "?", 0),
            new TrapInfo( 0xA458, "FldSetMaxVisibleLines", "v", 2, "fldp", "B"),
            new TrapInfo( 0xA459, "ScrDefaultPaletteState", "?", 0),
            new TrapInfo( 0xA45A, "PceNativeCall", "L", 2, "p", "vp"),
            new TrapInfo( 0xA45B, "SndStreamCreate", "W", 9, "pp", "B", "L", "w", "B", "p", "vp", "L", "B"),
            new TrapInfo( 0xA45C, "SndStreamDelete", "W", 1, "p"),
            new TrapInfo( 0xA45D, "SndStreamStart", "W", 1, "p"),
            new TrapInfo( 0xA45E, "SndStreamPause", "?", 0),
            new TrapInfo( 0xA45F, "SndStreamStop", "?", 0),
            new TrapInfo( 0xA460, "SndStreamSetVolume", "W", 2, "p", "l"),
            new TrapInfo( 0xA461, "SndStreamGetVolume", "?", 0),
            new TrapInfo( 0xA462, "SndPlayResource", "?", 0),
            new TrapInfo( 0xA463, "SndStreamSetPan", "?", 0),
            new TrapInfo( 0xA464, "SndStreamGetPan", "?", 0),
            new TrapInfo( 0xA465, "MultimediaDispatch", "?", 0),
            new TrapInfo( 0xa466, "SndStreamCreateExtended", "?", 0),
            new TrapInfo( 0xa467, "SndStreamDeviceControl", "?", 0),
            new TrapInfo( 0xA468, "BmpCreateVersion3", "?", 0),
            new TrapInfo( 0xA469, "ECFixedMul", "?", 0),
            new TrapInfo( 0xA46A, "ECFixedDiv", "?", 0),
            new TrapInfo( 0xA46B, "HALDrawGetSupportedDensity", "?", 0),
            new TrapInfo( 0xA46C, "HALRedrawInputArea", "?", 0),
            new TrapInfo( 0xA46D, "GrfBeginStroke", "?", 0),
            new TrapInfo( 0xA46E, "BmpPrvConvertBitmap", "?", 0),
            new TrapInfo( 0xA46F, "SysReservedTrap5", "?", 0),
            new TrapInfo( 0xA470, "PinsDispatch", "?", 0),
            new TrapInfo( 0xA471, "SysReservedTrap1", "?", 0),
            new TrapInfo( 0xA472, "SysReservedTrap2", "?", 0),
            new TrapInfo( 0xA473, "SysReservedTrap3", "?", 0),
            new TrapInfo( 0xA474, "SysReservedTrap4", "?", 0),
            new TrapInfo( 0xA475, "LastTrapNumber", "?", 0),

            new TrapInfo( 0xA800, "LibTrapName", "?", 0),
            new TrapInfo( 0xA801, "LibTrapOpen", "?", 0),
            new TrapInfo( 0xA802, "LibTrapClose", "?", 0),
            new TrapInfo( 0xA803, "LibTrapSleep", "?", 0),
            new TrapInfo( 0xA804, "LibTrapWake", "?", 0),
};

        private static Dictionary<ushort, SystemService> allTraps =
            traps.ToDictionary(t => t.trap, t => BuildService(t));

        public static SystemService? GetTrapSignature(ushort uTrap)
        {
            return allTraps.TryGetValue(uTrap, out var sig)
                ? sig
                : null;
        }

        private static SystemService BuildService(in TrapInfo trap)
        {
            var s = new SystemService
            {
                 Name = trap.name,
                 Signature = BuildSignature(trap),
                 SyscallInfo = new SyscallInfo
                 {
                      Vector = trap.trap,
                 },
                 Characteristics = new ProcedureCharacteristics
                 {
                     // Skip past the trap and the trap #
                     ReturnAddressAdjustment = 4,
                 }
            };
            return s;
        }

        private static FunctionType BuildSignature(in TrapInfo trap)
        {
            var cce = new CallingConventionEmitter();
            cce.LowLevelDetails(2, 4);
            var ret = CreateReturnIdentifier(trap.rType);
            if (trap.args is not null)
            {
                foreach (var arg in trap.args)
                {
                    var dt = CreateParameterType(arg);
                    cce.StackParam(dt);
                }
            }
            var parameters = cce.Parameters
                .Select(p => new Identifier(
                    NamingPolicy.Instance.StackArgumentName(
                        p.DataType,
                        ((StackStorage) p).StackOffset,
                        null),
                    p.DataType,
                    p));
            return new FunctionType(ret, parameters.ToArray());
        }

        private static DataType GetPointerParameter(ReadOnlySpan<char> a)
        {
            if (a == "Wp")
            {
                return PrimitiveType.UInt16;
            }
            else if (a == "wp")
            {
                return PrimitiveType.Int16;
            }
            else if (a == "Bp")
            {
                return PrimitiveType.Byte;
            }
            else if (a == "bp")
            {
                return PrimitiveType.Int8;
            }
            else if (a == "Lp")
            {
                return PrimitiveType.UInt32;
            }
            else if (a == "lp")
            {
                return PrimitiveType.Int32;
            }
            else if (a == "4p")
            {
                //$TODO: no way to represent 32-bit chars yet.
                // Model as uint32
                return PrimitiveType.UInt32;
            }
            else if (a == "frmp")
            {
                return Form;
            }
            else if (a == "fobjp")
            {
                return FormObj;
            }
            else if (a == "ctlp")
            {
                return Control;
            }
            else if (a == "lstp")
            {
                return List;
            }
            else if (a == "fldp")
            {
                return Field;
            }
            else if (a == "tblp")
            {
                return Table;
            }
            else if (a == "sclp")
            {
                return Scrollbar;
            }
            else if (a == "gadp")
            {
                return Gadget;
            }
            else if (a == "evtp")
            {
                return Event;
            }
            else if (a == "rctp")
            {
                return Rect;
            }
            else if (a == "dttp")
            {
                return DateTime;
            }
            else if (a == "bmpp")
            {
                return Bitmap;
            }
            else if (a == "winp")
            {
                return Window;
            }
            else if (a == "lid")
            {
                return LocalID;
            }
            else if (a == "lidp")
            {
                return PLocalID;
            }
            else
            {
                return new UnknownType();
            }
        }

        private static DataType CreateParameterType(string arg)
        {
            int a = 0;
            if (arg[0] == 'o')
            {
                a++;
            }

            DataType dt;
            if (arg.Length == a + 1)
            {
                switch (arg[a])
                {
                case 'c':
                    dt = PrimitiveType.Char;
                    break;
                case 'b':
                    dt = PrimitiveType.Int8;
                    break;
                case 'B':
                    dt = PrimitiveType.Byte;
                    break;
                case 'w':
                    dt = PrimitiveType.Int16;
                    break;
                case 'W':
                    dt = PrimitiveType.UInt16;
                    break;
                case 'l':
                    dt = PrimitiveType.Int32;
                    break;
                case 'L':
                    dt = PrimitiveType.UInt32;
                    break;
                case 'p':
                    dt = PrimitiveType.Ptr32;
                    break;
                case '4':
                    // We need a 'char32', model as a uint32
                    dt = PrimitiveType.UInt32;
                    break;
                case 'S':
                    dt = new Pointer(PrimitiveType.Char, 32);
                    break;
                default:
                    throw new ArgumentException();
                }
            }
            else
            {
                DataType? dtOut = GetPointerParameter(arg.AsSpan(a));
                if (dtOut is null)
                    dt = PrimitiveType.Ptr32;
                else
                    dt = new Pointer(dtOut, 32);
            }
            return dt;
        }


        private static Identifier CreateReturnIdentifier(string rType)
        {
            Storage? stg = null;
            DataType dt;
            switch (rType[0])
            {
            case '?':
                dt = new UnknownType();
                break;
            case 'v':
                dt = VoidType.Instance;
                break;
            case 'p':
                dt = PrimitiveType.Ptr32;
                stg = Registers.a0;
                break;
            case 'c':
                dt = PrimitiveType.Char;
                stg = Registers.d0;
                break;
            case 'b':
                dt = PrimitiveType.Int8;
                stg = Registers.d0;
                break;
            case 'w':
                dt = PrimitiveType.Int16;
                stg = Registers.d0;
                break;
            case 'l':
                dt = PrimitiveType.Int32;
                stg = Registers.d0;
                break;
            case 'B':
                dt = PrimitiveType.Byte;
                stg = Registers.d0;
                break;
            case 'W':
                dt = PrimitiveType.UInt16;
                stg = Registers.d0;
                break;
            case 'L':
                dt = PrimitiveType.UInt32;
                stg = Registers.d0;
                break;
            default:
                dt = GetPointerParameter(rType.AsSpan());
                dt = new Pointer(dt, 32);
                stg = Registers.a0;
                break;
            }
            return new Identifier("", dt, stg!);
        }
    }
}
