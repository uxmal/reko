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

using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Emulation;
using Reko.Core.Expressions;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Operators;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.OdbgScript
{
    // This is the table for Script Execution
    public class t_dbgmemblock
    {
        public Address? address;    //Memory Adress
        public uint size;     //Block Size
        public int script_pos; //Registred at script pos
        public bool autoclean; //On script restart/change

        public Address? free_at_ip; //To free memory block used in ASM commands

        //Optional actions to do
        public bool restore_registers;

        //Delayed Result Origin
        public bool result_register;
        public eContextData reg_to_return;
    }

    public class t_export
    {
        public ulong addr;
        public string? label; // ;label[256];
    }

    public partial class OllyLangInterpreter : IScriptInterpreter
    {
        private const byte OS_VERSION_HI = 1;  // High plugin version
        private const byte OS_VERSION_LO = 77; // Low plugin version
        private const byte TE_VERSION_HI = 2;
        private const byte TE_VERSION_LO = 03;
        private const byte TS_VERSION_HI = 0;
        private const byte TS_VERSION_LO = 7;
        private const int STRING_READSIZE = 256;

        private readonly IServiceProvider services;
        private IProcessorArchitecture arch;
        private IProcessorEmulator? emu;
        private readonly List<int> calls = new List<int>();         // Call/Ret stack in script
        public readonly Dictionary<string, Var> variables = new Dictionary<string, Var>(); // Variables that exist
        private readonly Dictionary<Address, int> bpjumps = new Dictionary<Address, int>();  // Breakpoint Auto Jumps 
        public bool debuggee_running;
        public bool script_running;
        public bool run_till_return;
        public bool return_to_usercode;
        private int script_pos, script_pos_next;
        private readonly UnknownType unk = new UnknownType();

        // Debugger state
        private bool resumeDebuggee;
        private bool ignore_exceptions;
        private int stepcount;

        //allocated memory blocks to free at end of script
        private readonly List<t_dbgmemblock> tMemBlocks = new List<t_dbgmemblock>();

        //last breakpoint reason
        private ulong break_reason;
        private ulong break_memaddr;
        private Address? pmemforexec;
        private Address? membpaddr;
        private ulong membpsize;
        //private bool require_addonaction;
        //private bool back_to_debugloop;
        private string errorstr;

        public OllyLangInterpreter(IServiceProvider services, Program program, IProcessorArchitecture arch)
            : this(services, arch)
        {
            var envEmu = program.Platform.CreateEmulator(program.SegmentMap, program.ImportReferences);
            this.emu = arch.CreateEmulator(program.SegmentMap, envEmu);
            this.Host = new OdbgScriptHost(services, null, program);
            this.Debugger = new Debugger(arch, emu);
            emu.BeforeStart += delegate
            {
                this.Reset();
                this.debuggee_running = false;
                this.InitGlobalVariables();
                script_running = true;
                Step();
            };
        }

        public OllyLangInterpreter(IServiceProvider services, IProcessorArchitecture arch)
        {
            this.services = services;
            this.arch = arch;
            this.Script = new OllyScript();
            this.Debugger = null!;
            this.Host = null!;
            this.errorstr = "";
            this.callback_return = null!;

            #region Initialize command array
            commands["add"] = DoADD;
            commands["ai"] = DoAI;
            commands["alloc"] = DoALLOC;
            commands["an"] = DoAN;
            commands["and"] = DoAND;
            commands["ao"] = DoAO;
            commands["ask"] = DoASK;
            commands["asm"] = DoASM;
            commands["asmtxt"] = DoASMTXT;
            commands["atoi"] = DoATOI;
            commands["backup"] = DoOPENDUMP;
            commands["bc"] = DoBC;
            commands["bd"] = DoBD;
            commands["beginsearch"] = DoBEGINSEARCH;
            commands["bp"] = DoBP;
            commands["bpcnd"] = DoBPCND;
            commands["bpd"] = DoBPD;
            commands["bpgoto"] = DoBPGOTO;
            commands["bphwcall"] = DoBPHWCA;
            commands["bphwc"] = DoBPHWC;
            commands["bphws"] = DoBPHWS;
            commands["bpl"] = DoBPL;
            commands["bplcnd"] = DoBPLCND;
            commands["bpmc"] = DoBPMC;
            commands["bprm"] = DoBPRM;
            commands["bpwm"] = DoBPWM;
            commands["bpx"] = DoBPX;
            commands["buf"] = DoBUF;
            commands["call"] = DoCALL;
            commands["close"] = DoCLOSE;
            commands["cmp"] = DoCMP;
            commands["cmt"] = DoCMT;
            commands["cob"] = DoCOB;
            commands["coe"] = DoCOE;
            commands["dbh"] = DoDBH;
            commands["dbs"] = DoDBS;
            commands["dec"] = DoDEC;
            commands["div"] = DoDIV;
            commands["dm"] = DoDM;
            commands["dma"] = DoDMA;
            commands["dpe"] = DoDPE;
            commands["ende"] = DoENDE;
            commands["endsearch"] = DoENDSEARCH;
            commands["erun"] = DoERUN;
            commands["esti"] = DoESTI;
            commands["esto"] = DoERUN;
            commands["estep"] = DoESTEP;
            commands["eob"] = DoEOB;
            commands["eoe"] = DoEOE;
            commands["eval"] = DoEVAL;
            commands["exec"] = DoEXEC;
            commands["fill"] = DoFILL;
            commands["find"] = DoFIND;
            commands["findcalls"] = DoFINDCALLS;
            commands["findcmd"] = DoFINDCMD;
            commands["findcmds"] = DoFINDCMD;
            commands["findop"] = DoFINDOP;
            commands["findmem"] = DoFINDMEM;
            commands["free"] = DoFREE;
            commands["gapi"] = DoGAPI;
            commands["gbpm"] = DoGBPM;
            commands["gbpr"] = DoGBPR;
            commands["gci"] = DoGCI;
            commands["gcmt"] = DoGCMT;
            commands["gfo"] = DoGFO;
            commands["glbl"] = DoGLBL;
            commands["gmemi"] = DoGMEMI;
            commands["gmexp"] = DoGMEXP;
            commands["gma"] = DoGMA;
            commands["gmi"] = DoGMI;
            commands["gmimp"] = DoGMIMP;
            commands["gn"] = DoGN;
            commands["go"] = DoGO;
            commands["gopi"] = DoGOPI;
            commands["gpa"] = DoGPA;
            commands["gpi"] = DoGPI;
            commands["gpp"] = DoGPP;
            commands["gro"] = DoGRO;
            commands["gref"] = DoGREF;
            commands["gsl"] = DoGSL;
            commands["gstr"] = DoGSTR;
            commands["handle"] = DoHANDLE;
            commands["history"] = DoHISTORY;
            commands["inc"] = DoINC;
            commands["itoa"] = DoITOA;

            commands["jmp"] = DoJMP;

            commands["ja"] = DoJA;
            commands["jg"] = DoJA;
            commands["jnbe"] = DoJA;
            commands["jnle"] = DoJA;

            commands["jae"] = DoJAE;
            commands["jge"] = DoJAE;
            commands["jnb"] = DoJAE;
            commands["jnl"] = DoJAE;

            commands["jnae"] = DoJB;
            commands["jnge"] = DoJB;
            commands["jb"] = DoJB;
            commands["jl"] = DoJB;

            commands["jna"] = DoJBE;
            commands["jng"] = DoJBE;
            commands["jbe"] = DoJBE;
            commands["jle"] = DoJBE;

            commands["je"] = DoJE;
            commands["jz"] = DoJE;

            commands["jne"] = DoJNE;
            commands["jnz"] = DoJNE;

            commands["key"] = DoKEY;
            commands["lbl"] = DoLBL;
            commands["lc"] = DoLC;
            commands["lclr"] = DoLCLR;
            commands["len"] = DoLEN;
            commands["loadlib"] = DoLOADLIB;
            commands["lm"] = DoLM;
            commands["log"] = DoLOG;
            commands["logbuf"] = DoLOGBUF;
            commands["memcpy"] = DoMEMCPY;
            commands["mov"] = DoMOV;
            commands["msg"] = DoMSG;
            commands["msgyn"] = DoMSGYN;
            commands["mul"] = DoMUL;
            commands["names"] = DoNAMES;
            commands["neg"] = DoNEG;
            commands["not"] = DoNOT;
            commands["or"] = DoOR;
            commands["olly"] = DoOLLY;
            commands["opcode"] = DoOPCODE;
            commands["opendump"] = DoOPENDUMP;
            commands["opentrace"] = DoOPENTRACE;
            commands["pause"] = DoPAUSE;
            commands["pop"] = DoPOP;
            commands["popa"] = DoPOPA;
            commands["preop"] = DoPREOP;
            commands["push"] = DoPUSH;
            commands["pusha"] = DoPUSHA;
            commands["rbp"] = DoRBP;
            commands["readstr"] = DoREADSTR;
            commands["refresh"] = DoREFRESH;
            commands["ref"] = DoREF;
            commands["repl"] = DoREPL;
            commands["reset"] = DoRESET;
            commands["ret"] = DoRET;
            commands["rev"] = DoREV;
            commands["rol"] = DoROL;
            commands["ror"] = DoROR;
            commands["rtr"] = DoRTR;
            commands["rtu"] = DoRTU;
            commands["run"] = DoRUN;
            commands["sbp"] = DoSBP;
            commands["scmp"] = DoSCMP;
            commands["scmpi"] = DoSCMPI;
            commands["setoption"] = DoSETOPTION;
            commands["shl"] = DoSHL;
            commands["shr"] = DoSHR;
            commands["step"] = DoSTO;
            commands["sti"] = DoSTI;
            commands["sto"] = DoSTO;
            commands["str"] = DoSTR;
            commands["sub"] = DoSUB;
            commands["tc"] = DoTC;
            commands["test"] = DoTEST;
            commands["ti"] = DoTI;
            commands["tick"] = DoTICK;
            commands["ticnd"] = DoTICND;
            commands["to"] = DoTO;
            commands["tocnd"] = DoTOCND;
            commands["ubp"] = DoBP;
            commands["unicode"] = DoUNICODE;
            commands["var"] = DoVAR;
            commands["xor"] = DoXOR;
            commands["xchg"] = DoXCHG;
            commands["wrt"] = DoWRT;
            commands["wrta"] = DoWRTA;

            commands["reko.addseg"] = RekoAddSegmentReference;
            commands["reko.db"] = RekoDumpBytes;
            commands["reko.dasm"] = RekoDisassemble;
            commands["reko.setarch"] = RekoArch;
            commands["reko.clreps"] = RekoClearEntryPoints;
            commands["reko.addep"] = RekoAddEntryPoint;


            #endregion
#if LATER
            commands["error"] = DoError;
            commands["dnf"] = DoDumpAndFix;
            commands["stopdebug"] = DoStopDebug;
            commands["dumpprocess"] = DoDumpProcess;
            commands["dumpregions"] = DoDumpRegions;
            commands["dumpmodule"] = DoDumpModule;
            commands["pastepeheader"] = DoPastePEHeader;
            commands["extractoverlay"] = DoExtractOverlay;
            commands["addoverlay"] = DoAddOverlay;
            commands["copyoverlay"] = DoCopyOverlay;
            commands["removeoverlay"] = DoRemoveOverlay;
            commands["resortfilesections"] = DoResortFileSections;
            commands["makeallsectionsrwe"] = DoMakeAllSectionsRWE;
            commands["addnewsection"] = DoAddNewSection;
            commands["resizelastsection"] = DoResizeLastSection;
            commands["getpe32data"] = DoGetPE32Data;
            commands["setpe32data"] = DoSetPE32Data;
            commands["getpe32sectionnumberfromva"] = DoGetPE32SectionNumberFromVA;
            commands["convertvatofileoffset"] = DoConvertVAtoFileOffset;
            commands["convertfileoffsettova"] = DoConvertFileOffsetToVA;
            commands["isfiledll"] = DoIsFileDLL;
            commands["realignpe"] = DoRealignPE;
            commands["relocatercleanup"] = DoRelocaterCleanup;
            commands["relocaterinit"] = DoRelocaterInit;
            commands["relocateraddnewrelocation"] = DoRelocaterAddNewRelocation;
            commands["relocaterestimatedsize"] = DoRelocaterEstimatedSize;
            commands["relocaterexportrelocation"] = DoRelocaterExportRelocation;
            commands["relocaterexportrelocationex"] = DoRelocaterExportRelocationEx;
            commands["relocatermakesnapshot"] = DoRelocaterMakeSnapshot;
            commands["relocatercomparetwosnapshots"] = DoRelocaterCompareTwoSnapshots;
            commands["relocaterchangefilebase"] = DoRelocaterChangeFileBase;
            commands["threaderpausethread"] = DoThreaderPauseThread;
            commands["threaderresumethread"] = DoThreaderResumeThread;
            commands["threaderterminatethread"] = DoThreaderTerminateThread;
            commands["threaderpauseallthreads"] = DoThreaderPauseAllThreads;
            commands["threaderresumeallthreads"] = DoThreaderResumeAllThreads;
            commands["getdebuggeddllbaseaddress"] = DoGetDebuggedDLLBaseAddress;
            commands["getdebuggedfilebaseaddress"] = DoGetDebuggedFileBaseAddress;
            commands["getjumpdestination"] = DoGetJumpDestination;
            commands["isjumpgoingtoexecute"] = DoIsJumpGoingToExecute;
            commands["getpeblocation"] = DoGetPEBLocation;
            commands["detachdebuggerex"] = DoDetachDebuggerEx;
            commands["setcustomhandler"] = DoSetCustomHandler;
            commands["importercleanup"] = DoImporterCleanup;
            commands["importersetimagebase"] = DoImporterSetImageBase;
            commands["importerinit"] = DoImporterInit;
            commands["importeraddnewdll"] = DoImporterAddNewDll;
            commands["importeraddnewapi"] = DoImporterAddNewAPI;
            commands["importeraddnewordinalapi"] = DoImporterAddNewOrdinalAPI;
            commands["importergetaddeddllcount"] = DoImporterGetAddedDllCount;
            commands["importergetaddedapicount"] = DoImporterGetAddedAPICount;
            commands["importermoveiat"] = DoImporterMoveIAT;
            commands["importerrelocatewritelocation"] = DoImporterRelocateWriteLocation;
            commands["importerexportiat"] = DoImporterExportIAT;
            commands["importerestimatedsize"] = DoImporterEstimatedSize;
            commands["importerexportiatex"] = DoImporterExportIATEx;
            commands["importergetnearestapiaddress"] = DoImporterGetNearestAPIAddress;
            commands["importerautosearchiat"] = DoImporterAutoSearchIAT;
            commands["importerautosearchiatex"] = DoImporterAutoSearchIATEx;
            commands["importerautofixiatex"] = DoImporterAutoFixIATEx;
            commands["importerautofixiat"] = DoImporterAutoFixIAT;
            commands["tracerlevel1"] = DoTracerLevel1;
            commands["hashtracerlevel1"] = DoHashTracerLevel1;
            commands["tracerdetectredirection"] = DoTracerDetectRedirection;
            commands["tracerfixknownredirection"] = DoTracerFixKnownRedirection;
            commands["tracerfixredirectionviaimprecplugin"] = DoTracerFixRedirectionViaImpRecPlugin;
            commands["exportercleanup"] = DoExporterCleanup;
            commands["exportersetimagebase"] = DoExporterSetImageBase;
            commands["exporterinit"] = DoExporterInit;
            commands["exporteraddnewexport"] = DoExporterAddNewExport;
            commands["exporteraddnewordinalexport"] = DoExporterAddNewOrdinalExport;
            commands["exportergetaddedexportcount"] = DoExporterGetAddedExportCount;
            commands["exporterestimatedsize"] = DoExporterEstimatedSize;
            commands["exporterbuildexporttable"] = DoExporterBuildExportTable;
            commands["exporterbuildexporttableex"] = DoExporterBuildExportTableEx;
            commands["librariansetbreakpoint"] = DoLibrarianSetBreakPoint;
            commands["librarianremovebreakpoint"] = DoLibrarianRemoveBreakPoint;
            commands["tlsremovecallback"] = DoTLSRemoveCallback;
            commands["tlsremovetable"] = DoTLSRemoveTable;
            commands["tlsbackupdata"] = DoTLSBackupData;
            commands["tlsrestoredata"] = DoTLSRestoreData;
            commands["handlerishandleopen"] = DoHandlerIsHandleOpen;
            commands["handlercloseremotehandle"] = DoHandlerCloseRemoteHandle;
            commands["staticfileload"] = DoStaticFileLoad;
            commands["staticfileunload"] = DoStaticFileUnload;

            CustomHandlerCallbacks[UE_CH_BREAKPOINT] = CHC_BREAKPOINT;
            CustomHandlerCallbacks[UE_CH_SINGLESTEP] = CHC_SINGLESTEP;
            CustomHandlerCallbacks[UE_CH_ACCESSVIOLATION] = CHC_ACCESSVIOLATION;
            CustomHandlerCallbacks[UE_CH_ILLEGALINSTRUCTION] = CHC_ILLEGALINSTRUCTION;
            CustomHandlerCallbacks[UE_CH_NONCONTINUABLEEXCEPTION] = CHC_NONCONTINUABLEEXCEPTION;
            CustomHandlerCallbacks[UE_CH_ARRAYBOUNDSEXCEPTION] = CHC_ARRAYBOUNDSEXCEPTION;
            CustomHandlerCallbacks[UE_CH_FLOATDENORMALOPERAND] = CHC_FLOATDENORMALOPERAND;
            CustomHandlerCallbacks[UE_CH_FLOATDEVIDEBYZERO] = CHC_FLOATDEVIDEBYZERO;
            CustomHandlerCallbacks[UE_CH_INTEGERDEVIDEBYZERO] = CHC_INTEGERDEVIDEBYZERO;
            CustomHandlerCallbacks[UE_CH_INTEGEROVERFLOW] = CHC_INTEGEROVERFLOW;
            CustomHandlerCallbacks[UE_CH_PRIVILEGEDINSTRUCTION] = CHC_PRIVILEGEDINSTRUCTION;
            CustomHandlerCallbacks[UE_CH_PAGEGUARD] = CHC_PAGEGUARD;
            CustomHandlerCallbacks[UE_CH_EVERYTHINGELSE] = CHC_EVERYTHINGELSE;
            CustomHandlerCallbacks[UE_CH_CREATETHREAD] = CHC_CREATETHREAD;
            CustomHandlerCallbacks[UE_CH_EXITTHREAD] = CHC_EXITTHREAD;
            CustomHandlerCallbacks[UE_CH_CREATEPROCESS] = CHC_CREATEPROCESS;
            CustomHandlerCallbacks[UE_CH_EXITPROCESS] = CHC_EXITPROCESS;
            CustomHandlerCallbacks[UE_CH_LOADDLL] = CHC_LOADDLL;
            CustomHandlerCallbacks[UE_CH_UNLOADDLL] = CHC_UNLOADDLL;
            CustomHandlerCallbacks[UE_CH_OUTPUTDEBUGSTRING] = CHC_OUTPUTDEBUGSTRING;

            LibraryBreakpointCallbacks[UE_ON_LIB_LOAD] = LBPC_LOAD;
            LibraryBreakpointCallbacks[UE_ON_LIB_UNLOAD] = LBPC_UNLOAD;
            LibraryBreakpointCallbacks[UE_ON_LIB_ALL] = LBPC_ALL;
#endif
        }

        void SoftwareCallback() { OnBreakpoint(BreakpointType.PP_INT3BREAK); }
        void HardwareCallback() { OnBreakpoint(BreakpointType.PP_HWBREAK); }
        void MemoryCallback() { OnBreakpoint(BreakpointType.PP_MEMBREAK); }
        void EXECJMPCallback() { DoSTI(); }

        public struct callback_t
        {
            public uint call;
            public bool returns_value;
            public Var.EType return_type;
        }

        private readonly List<callback_t> callbacks = new List<callback_t>();
        private Var callback_return;

        private readonly Dictionary<eCustomException, Expression> CustomHandlerLabels = new Dictionary<eCustomException, Expression>();
        private readonly Dictionary<eCustomException, Debugger.fCustomHandlerCallback> CustomHandlerCallbacks = new Dictionary<eCustomException, Debugger.fCustomHandlerCallback>();

        private string Label_AutoFixIATEx = "";

        private Dictionary<eLibraryEvent, Dictionary<string, string>> LibraryBreakpointLabels = //<library path, label name>
            new Dictionary<eLibraryEvent, Dictionary<string, string>>();
        private Dictionary<eLibraryEvent, Librarian.fLibraryBreakPointCallback> LibraryBreakpointCallbacks =
            new Dictionary<eLibraryEvent, Librarian.fLibraryBreakPointCallback>();

        private enum BreakpointType { PP_INT3BREAK = 0x10, PP_MEMBREAK = 0x20, PP_HWBREAK = 0x40 };

        public class register_t
        {
            public register_t(string n, eContextData i, byte s, byte o)
            {
                this.name = n; this.id = i; this.size = s; this.offset = o;
            }
            public readonly string name;
            public readonly eContextData id;
            public readonly byte size;
            public readonly byte offset;
        }

        public class eflags_t
        {
            public ulong dw;

            public class Flagomizer
            {
                private eflags_t eflags_t;

                public Flagomizer(eflags_t eflags_t)
                {
                    this.eflags_t = eflags_t;
                }

                public bool CF { get { return ((eflags_t.dw & 1) != 0); } set { if (value) eflags_t.dw |= 1u; else eflags_t.dw &= ~1u; } }
                public bool PF { get { return ((eflags_t.dw & 4) != 0); } set { if (value) eflags_t.dw |= 4u; else eflags_t.dw &= ~4u; } }
                public bool AF { get { return ((eflags_t.dw & 16) != 0); } set { if (value) eflags_t.dw |= 16u; else eflags_t.dw &= ~16u; } }
                public bool ZF { get { return ((eflags_t.dw & 32) != 0); } set { if (value) eflags_t.dw |= 32u; else eflags_t.dw &= ~32u; } }
                public bool SF { get { return ((eflags_t.dw & 64) != 0); } set { if (value) eflags_t.dw |= 64u; else eflags_t.dw &= ~64u; } }
                public bool TF { get { return ((eflags_t.dw & 128) != 0); } set { if (value) eflags_t.dw |= 128u; else eflags_t.dw &= ~128u; } }
                public bool IF { get { return ((eflags_t.dw & 256) != 0); } set { if (value) eflags_t.dw |= 256u; else eflags_t.dw &= ~256u; } }
                public bool DF { get { return ((eflags_t.dw & 512) != 0); } set { if (value) eflags_t.dw |= 512u; else eflags_t.dw &= ~512u; } }
                public bool OF { get { return ((eflags_t.dw & 1024) != 0); } set { if (value) eflags_t.dw |= 1024u; else eflags_t.dw &= ~1024u; } }
            }
            public readonly Flagomizer bits;

            public eflags_t()
            {
                bits = new Flagomizer(this);
            }
        }


        // Commands that can be executed
        Dictionary<string, Func<Expression[], bool>> commands = new(StringComparer.InvariantCultureIgnoreCase);

        private int EOB_row, EOE_row;
        private bool bInternalBP;
        private ulong tickcount_startup;
        private byte[]? search_buffer;

        // Pseudo-flags to emulate CMP
        private bool zf;
        private bool cf;

        // Cursor for REF / (NEXT)REF function
        //int adrREF;
        //int curREF;

        void SetCMPFlags(int diff)
        {
            zf = (diff == 0);
            cf = (diff < 0);
        }

        // Commands
        bool GetByte(Expression op, out byte value)
        {
            if (GetUlong(op, out ulong temp) && temp <= Byte.MaxValue)
            {
                value = (byte) temp;
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }

        // Save / Restore Breakpoints
        /*
        t_hardbpoint hwbp_t[4];
        t_sorted sortedsoftbp_t;
        t_bpoint* softbp_t;
        */

        //uint saved_bp;
        //uint alloc_bp;
        //bool AllocSwbpMem(uint tmpSizet);
        //void FreeBpMem();

        // Save / Restore Registers
        public class t_reg_backup
        {
            public bool loaded;
            public Dictionary<RegisterStorage, ulong> regs = new();
            public ulong eflags;
            public uint threadid;
            public int script_pos;
        }
        t_reg_backup reg_backup = new t_reg_backup();

        //cache for GMEXP
        List<t_export> tExportsCache = new List<t_export>();
        //ulong exportsCacheAddr;

        //cache for GMIMP
        List<t_export> tImportsCache = new List<t_export>();
        //ulong importsCacheAddr;

        readonly static string[] fpu_registers = { "st(0)", "st(1)", "st(2)", "st(3)", "st(4)", "st(5)", "st(6)", "st(7)" };

        readonly static string[] e_flags = { "!cf", "!pf", "!af", "!zf", "!sf", "!df", "!of" };

        /// <summary>
        /// Constant values.
        /// </summary>
        readonly static Dictionary<string, ulong> constants = new Dictionary<string, ulong>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "true",  1},
            { "false", 0 },
            { "null",  0 },
    
#if LATER
	new constant_t("ue_access_read",  UE_ACCESS_READ),
	new constant_t("ue_access_write", UE_ACCESS_WRITE),
	new constant_t("ue_access_all",   UE_ACCESS_ALL),
    
	new constant_t("ue_pe_offset",              UE_PE_OFFSET),
	new constant_t("ue_imagebase",              UE_IMAGEBASE),
	new constant_t("ue_oep",                    UE_OEP),
	new constant_t("ue_sizeofimage",            UE_SIZEOFIMAGE),
	new constant_t("ue_sizeofheaders",          UE_SIZEOFHEADERS),
	new constant_t("ue_sizeofoptionalheader",   UE_SIZEOFOPTIONALHEADER),
	new constant_t("ue_sectionalignment",       UE_SECTIONALIGNMENT),
	new constant_t("ue_importtableaddress",     UE_IMPORTTABLEADDRESS),
	new constant_t("ue_importtablesize",        UE_IMPORTTABLESIZE),
	new constant_t("ue_resourcetableaddress",   UE_RESOURCETABLEADDRESS),
	new constant_t("ue_resourcetablesize",      UE_RESOURCETABLESIZE),
	new constant_t("ue_exporttableaddress",     UE_EXPORTTABLEADDRESS),
	new constant_t("ue_exporttablesize",        UE_EXPORTTABLESIZE),
	new constant_t("ue_tlstableaddress",        UE_TLSTABLEADDRESS),
	new constant_t("ue_tlstablesize",           UE_TLSTABLESIZE),
	new constant_t("ue_relocationtableaddress", UE_RELOCATIONTABLEADDRESS),
	new constant_t("ue_relocationtablesize",    UE_RELOCATIONTABLESIZE),
	new constant_t("ue_timedatestamp",          UE_TIMEDATESTAMP),
	new constant_t("ue_sectionnumber",          UE_SECTIONNUMBER),
	new constant_t("ue_checksum",               UE_CHECKSUM),
	new constant_t("ue_subsystem",              UE_SUBSYSTEM),
	new constant_t("ue_characteristics",        UE_CHARACTERISTICS),
	new constant_t("ue_numberofrvaandsizes",    UE_NUMBEROFRVAANDSIZES),
	new constant_t("ue_sectionname",            UE_SECTIONNAME),
	new constant_t("ue_sectionvirtualoffset",   UE_SECTIONVIRTUALOFFSET),
	new constant_t("ue_sectionvirtualsize",     UE_SECTIONVIRTUALSIZE),
	new constant_t("ue_sectionrawoffset",       UE_SECTIONRAWOFFSET),
	new constant_t("ue_sectionrawsize",         UE_SECTIONRAWSIZE),
	new constant_t("ue_sectionflags",           UE_SECTIONFLAGS),
    
	new constant_t("ue_ch_breakpoint",              UE_CH_BREAKPOINT),
	new constant_t("ue_ch_singlestep",              UE_CH_SINGLESTEP),
	new constant_t("ue_ch_accessviolation",         UE_CH_ACCESSVIOLATION),
	new constant_t("ue_ch_illegalinstruction",      UE_CH_ILLEGALINSTRUCTION),
	new constant_t("ue_ch_noncontinuableexception", UE_CH_NONCONTINUABLEEXCEPTION),
	new constant_t("ue_ch_arrayboundsexception",    UE_CH_ARRAYBOUNDSEXCEPTION),
	new constant_t("ue_ch_floatdenormaloperand",    UE_CH_FLOATDENORMALOPERAND),
	new constant_t("ue_ch_floatdevidebyzero",       UE_CH_FLOATDEVIDEBYZERO),
	new constant_t("ue_ch_integerdevidebyzero",     UE_CH_INTEGERDEVIDEBYZERO),
	new constant_t("ue_ch_integeroverflow",         UE_CH_INTEGEROVERFLOW),
	new constant_t("ue_ch_privilegedinstruction",   UE_CH_PRIVILEGEDINSTRUCTION),
	new constant_t("ue_ch_pageguard",               UE_CH_PAGEGUARD),
	new constant_t("ue_ch_everythingelse",          UE_CH_EVERYTHINGELSE),
	new constant_t("ue_ch_createthread",            UE_CH_CREATETHREAD),
	new constant_t("ue_ch_exitthread",              UE_CH_EXITTHREAD),
	new constant_t("ue_ch_createprocess",           UE_CH_CREATEPROCESS),
	new constant_t("ue_ch_exitprocess",             UE_CH_EXITPROCESS),
	new constant_t("ue_ch_loaddll",                 UE_CH_LOADDLL),
	new constant_t("ue_ch_unloaddll",               UE_CH_UNLOADDLL),
	new constant_t("ue_ch_outputdebugstring",       UE_CH_OUTPUTDEBUGSTRING),
    
	new constant_t("ue_on_lib_load",   UE_ON_LIB_LOAD),
	new constant_t("ue_on_lib_unload", UE_ON_LIB_UNLOAD),
	new constant_t("ue_on_lib_all",    UE_ON_LIB_ALL)
#endif
        };

        public Debugger Debugger { get; set; }
        public IOdbgScriptHost Host { get; set; }
        public OllyScript Script { get; set; }

        public void Dispose()
        {
            if (search_buffer is not null)
                DoENDSEARCH();
            FreeMemBlocks();
        }

        public void InitGlobalVariables()
        {
            // Global variables
            var targetPath = Host.TE_GetTargetPath() ?? "";
            variables["$INPUTFILE"] = Var.Create(targetPath);

            string? name = Host.TE_GetOutputPath();
            if (string.IsNullOrEmpty(name))
            {
                string ext;
                int offs;
                name = targetPath;
                if ((offs = name.LastIndexOf('.')) >= 0)
                {
                    ext = ".unpacked" + name.Substring(offs);
                    name = name + ext;
                }
            }
            variables["$OUTPUTFILE"] = Var.Create(name);
        }

        public void Reset()
        {
            FreeMemBlocks();
            variables.Clear();
            bpjumps.Clear();
            calls.Clear();

            variables["$OS_VERSION"] = Var.Create(Helper.rul2decstr(OS_VERSION_HI) + '.' + Helper.rul2decstr(OS_VERSION_LO));
            variables["$TE_VERSION"] = Var.Create(Helper.rul2decstr(TE_VERSION_HI) + '.' + Helper.rul2decstr(TE_VERSION_LO));
            variables["$TS_VERSION"] = Var.Create(Helper.rul2decstr(TS_VERSION_HI) + '.' + Helper.rul2decstr(TS_VERSION_LO));
            variables["$VERSION"] = variables["$OS_VERSION"];
            variables["$WIN_VERSION"] = Var.Create(Environment.OSVersion.VersionString);

#if _WIN64
	variables["$PLATFORM"] = "x86-64";
#else
            variables["$PLATFORM"] = Var.Create("x86-32");
#endif

            EOB_row = EOE_row = -1;

            zf = cf = false;
            search_buffer = null;

            //saved_bp = 0;
            //alloc_bp = 0;

            script_running = false;

            script_pos_next = 0;
            tickcount_startup = 0;
            break_memaddr = 0;
            break_reason = 0;

            pmemforexec = null;
            membpaddr = null;
            membpsize = 0;

            reg_backup.loaded = false;

            variables["$RESULT"] = Var.Create(0);

            callbacks.Clear();
            debuggee_running = false;
            //require_addonaction = false;

            run_till_return = false;
            return_to_usercode = false;

            tExportsCache.Clear();
            //exportsCacheAddr = 0;
            tImportsCache.Clear();
            //importsCacheAddr = 0;
        }

        public void LoadFromFile(string scriptFilename, Program program, string curDir)
        {
            var host = new OdbgScriptHost(services, null, program);
            var fsSvc = services.RequireService<IFileSystemService>();
            using (var parser = OllyScriptParser.FromFile(host, fsSvc, scriptFilename, curDir))
            {
                this.Script = parser.ParseScript();
            }
        }

        public void LoadFromString(string scriptString, Program program, string curDir)
        {
            var host = new OdbgScriptHost(services, null, program);
            var fsSvc = services.RequireService<IFileSystemService>();
            using var parser = OllyScriptParser.FromString(host, fsSvc, scriptString, curDir);
            this.Script = parser.ParseScript();
        }

        public void Run()
        {
            emu?.Start();
        }

        public void RunInner()
        {
            script_running = true;
            Step();
        }

        public bool Pause()
        {
            script_running = false;
            return true;
        }

        public bool Step()
        {
            resumeDebuggee = false;
            ignore_exceptions = false;
            stepcount = 0;

            while (!resumeDebuggee && script_running)
            {
                if (tickcount_startup == 0)
                    tickcount_startup = Helper.MyTickCount();

                script_pos = Script.NextCommandIndex(script_pos_next);

                // Check if script out of bounds
                if (script_pos >= Script.Lines.Count)
                    return false;

                var line = Script.Lines[script_pos];

                script_pos_next = script_pos + 1;

                // Log line of code if  enabled
                if (Script.Log)
                {
                    Host.TE_Log("--> " + line.ToString());
                }

                // Find command and execute it
                Func<Expression[], bool>? cmd = line.CommandPtr;
                if (cmd is null &&
                    line.Command is not null &&
                    commands.TryGetValue(line.Command, out var it))
                {
                    line.CommandPtr = cmd = it;
                }

                bool result = false;
                if (cmd is not null)
                {
                    result = cmd(line.Args); // Call command
                }
                else
                {
                    errorstr = "Unknown command: " + line.Command;
                }

                if (callbacks.Count != 0 && resumeDebuggee)
                {
                    result = false;
                    errorstr = "Unallowed command during callback: " + line.Command;
                }

                // Error in processing, show error and die
                if (!result)
                {
                    Pause();
                    string message = $"Error on line {line.LineNumber + 1}: {line.RawLine}\r\n{errorstr}";
                    Host.MsgError(message);
                    errorstr = "";
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Executed after some commands to clean memory or to get something 
        /// after ollydbg processing.
        /// </summary>
        /// <returns></returns>
        bool ProcessAddonAction()
        {
            bool restore_registers = false;
            Address ip = Debugger.InstructionPointer;
            for (int i = 0; i < tMemBlocks.Count;)
            {
                if (tMemBlocks[i].free_at_ip == ip)
                {
                    Host.FreeMemory(tMemBlocks[i].address!.Value, tMemBlocks[i].size);
                    if (tMemBlocks[i].result_register)
                        variables["$RESULT"] = Var.Create((ulong) Debugger.GetContextData(tMemBlocks[i].reg_to_return));
                    if (tMemBlocks[i].restore_registers)
                        restore_registers = true;
                    tMemBlocks.RemoveAt(i);
                }
                else i++;
            }

            if (restore_registers)
                RestoreRegisters(true);

            return true;
        }

        /// <summary>
        /// This method is called when the debugger hits a breakpoint.
        /// </summary>
        /// <param name="reason"></param>
        void OnBreakpoint(BreakpointType reason)
        {
            if (bInternalBP) //dont process temporary bp (exec/ende/go)
            {
                bInternalBP = false;
            }
            else
            {
                break_reason = (ulong) reason;

                if (EOB_row > -1)
                {
                    script_pos_next = EOB_row;
                }
                else
                {
                    Address ip = Debugger.InstructionPointer;
                    if (bpjumps.TryGetValue(ip, out int it))
                    {
                        script_pos_next = it;
                    }
                }
                debuggee_running = false;
            }
            StepChecked();
        }

        public void OnException()
        {
            if (EOE_row > -1)
            {
                script_pos_next = EOE_row;
            }
            else if (ignore_exceptions)
            {
                return;
            }
            StepChecked();
        }

        int GetFloatOperatorPos(string ops)
        {
            string operators = "+-*/";
            int b = 0, e = 0, p;

            //Search for operator(s) outside [pointers]
            while ((b = ops.IndexOf('[', b)) >= 0)
            {
                //Check Before
                p = ops.IndexOfAny(operators.ToCharArray());
                if (e >= 0 && p < b)
                {
                    return p;
                }

                e = ops.IndexOf(']', b + 1);
                if (e >= 0)
                {
                    //Check between
                    if ((p = ops.IndexOfAny(operators.ToCharArray(), e + 1)) >= 0 && p < ops.IndexOf('[', e + 1))
                    {
                        return p;
                    }
                    b = e;
                }
                b++;
            }

            //Check after
            return ops.IndexOfAny(operators.ToCharArray(), e);
        }

#if MIXING_PARSING_AND_EVALUATION

        bool ParseFloat(string arg, out string result)
        {
            throw new NotImplementedException();
            /*
            int start = 0, offs;
            char oper = '+';
            double val = 0, curval;
            result = "";
            if ((offs = GetFloatOperatorPos(arg)) >= 0)
            {
                do
                {
                    if (start==0 && offs==0) // allow leading +/-
                    {
                        curval = 0.0;
                    }
                    else
                    {
                        string token = Helper.trim(arg.Substring(start, offs));
                        if (!GetFloat(token, out curval))
                        {
                            //Convert integer to float (but not for first operand)
                            if (start != 0 && GetRulong(token, out rulong  dw))
                                curval = dw;
                            else
                                return false;
                        }
                    }

                    if (oper == '/' && curval == 0.0)
                    {
                        errorstr = "Division by 0";
                        return false;
                    }

                    switch (oper)
                    {
                    case '+': val += curval; break;
                    case '-': val -= curval; break;
                    case '*': val *= curval; break;
                    case '/': val /= curval; break;
                    }

                    if (offs < 0)
                        break;

                    oper = arg[start + offs];

                    start += offs + 1;
                    offs = GetFloatOperatorPos(arg.Substring(start));
                }
                while (start < arg.Length);

                result = Helper.dbl2str(val);

                // Remove trailing zeroes (keep 1 digit after '.')
                int p;
                if ((p = result.IndexOf('.')) >= 0)
                {
                    int psize = result.Length - p;

                    do psize--;
                    while (psize > 1 && result[p + psize] == '0');

                    result = result.Remove(p + psize + 1);
                }
                return true;
            }

            return false;
            */
        }
#endif

        bool GetAnyValue(Expression op, out string? value, bool hex8forExec = false)
        {
            value = null;
            if (op is Identifier id && IsVariable(id.Name))
            {
                Var v = variables[id.Name];
                if (v.IsString())
                {
                    value = v.str;
                    return true;
                }
                else if (v.Address is Address addr)
                {
                    value = addr.ToString();
                    return true;
                }
                else if (v.IsInteger())
                {
                    if (hex8forExec) //For Assemble Command (EXEC/ENDE) ie. "0DEADBEEF"
                        value = '0' + Helper.rul2hexstr(v.ToUInt64()).ToUpperInvariant();
                    else
                        value = Helper.rul2hexstr(v.ToUInt64()).ToUpperInvariant();
                    return true;
                }
            }
            else if (op is MkSequence seq && GetAddress(op, out Address addr))
            {
                value = addr.ToString();
                //$TODO: hex8forexec?
                return true;
            }
            else if (op is Constant c)
            {
                //$TODO: should be hex.
                value = c.ToString();
                if (hex8forExec && !char.IsDigit(value[0]))
                    value = '0' + value;
                return true;
            }
            else if (op is StringConstant str)
            {
                value = str.ToString();
                return true;
            }
            else if (op is Application app && app.Procedure is ProcedureConstant pc)
            {
                if (pc.Procedure.Name == "Interpolate")
                {
                    value = InterpolateVariables(((StringConstant) app.Arguments[0]).ToString(), false);
                    return true;
                }
                value = null;
                return false;
            }
            //$TODO: more values.
            /*
            else if (Helper.IsInterpolatedString(op))
            {
                value = Helper.UnquoteInterpolatedString(op);
                value = InterpolateVariables(value, hex8forExec);
                return true;
            }
            else if (Helper.IsStringLiteral(op))
            {
                value = Helper.UnquoteString(op, '"');
                return true;
            }
            else if (Helper.IsHexLiteral(op))
            {
                value = op.ToString();
                return true;
            }
            */
            else if (op is MemoryAccess mem)
            {
                return GetString(mem, out value);
            }
            else if (GetUlong(op, out ulong dw))
            {
                if (hex8forExec)
                    value = '0' + Helper.rul2hexstr(dw).ToUpperInvariant();
                else
                    value = Helper.rul2hexstr(dw).ToUpperInvariant();
                return true;
            }
            return false;
        }

        bool GetString(Expression op, [MaybeNullWhen(false)] out string value)
        {
            return GetString(op, 0, out value);
        }

        bool GetString(Expression op, int size, [MaybeNullWhen(false)] out string? value)
        {
            if (op is Identifier id && IsVariable(id.Name))
            {
                if (variables[id.Name].IsString())
                {
                    if (size != 0 && size < variables[id.Name].size)
                    {
                        Var tmp = variables[id.Name];
                        tmp.resize(size);
                        value = tmp.str!;
                    }
                    else
                    {
                        value = variables[id.Name].str!;
                    }
                    return true;
                }
            }
            else if (op is StringConstant str)
            {
                value = str.ToString();
                if (size != 0 && size < value.Length)
                    value = value.Remove(size);
                return true;
            }
            else if (Helper.TryGetHexLiteral(op, out value))
            {
                if (size != 0 && (size * 2) < (value.Length - 2))
                    value = value.Substring(0, (size * 2) + 1) + '#';
                return true;
            }
            else if (op is MemoryAccess mem)
            {
                if (GetAddress(mem.EffectiveAddress, out Address src))
                {
                    value = "";
                    if (size != 0)
                    {
                        byte[] buffer = new byte[size];
                        if (Host.TryReadBytes(src, size, buffer))
                        {
                            value = '#' + Helper.bytes2hexstr(buffer, size) + '#';
                            return true;
                        }
                    }
                    else
                    {
                        var ea = src;
                        if (!Host.SegmentMap.TryFindSegment(ea, out ImageSegment? segment))
                            throw new AccessViolationException();
                        byte[] buffer = new byte[STRING_READSIZE];
                        if (segment.MemoryArea is ByteMemoryArea bmem &&
                            bmem.TryReadBytes(ea, buffer.Length, buffer))
                        {
                            buffer[buffer.Length - 1] = 0;
                            value = Encoding.UTF8.GetString(buffer);
                            return true;
                        }
                    }
                }
            }
            else if (op is BinaryExpression bin && bin.Operator.Type == OperatorType.IAdd)
            {
                if (GetString(bin.Left, out var left) &&
                    GetString(bin.Right, out var right))
                {
                    value = left + right;
                    return true;
                }
            }
            value = "";
            return false;
        }

        bool GetBool(Expression op, out bool value)
        {
            if (GetUlong(op, out ulong temp))
            {
                value = temp != 0;
                return true;
            }
            value = false;
            return false;
        }

        public bool GetAddress(Expression op, out Address value)
        {
            value = default;
            if (op is MkSequence seq && seq.Expressions.Length == 2)
            {
                // Possible segmented address. Evaluate part before
                // and after colon.
                if (GetUlong(seq.Expressions[0], out var seg) &&
                    GetUlong(seq.Expressions[1], out var off))
                {
                    var cSeg = Constant.UInt16((ushort) seg);
                    var cOff = Constant.UInt32((uint) off);
                    value = arch.MakeSegmentedAddress(cSeg, cOff);
                    return true;
                }
            }
            if (op is Identifier id && IsVariable(id.Name) && variables[id.Name].Address is not null)
            {
                value = variables[id.Name].Address!.Value;
                return true;
            }
            if (!GetUlong(op, out ulong uAddr))
                return false;
            value = arch.MakeAddressFromConstant(Constant.UInt64(uAddr), false);
            return true;
        }

        private bool GetArchitecture(Expression? eArch, [MaybeNullWhen(false)] out IProcessorArchitecture arch)
        {
            if (eArch is not StringConstant sArch)
            {
                arch = this.arch;
                return true;
            }
            var cfgSvc = services.RequireService<IConfigurationService>();
            arch = cfgSvc.GetArchitecture(sArch.Literal);
            return arch is not null;
        }

        bool GetUlong(Expression op, out ulong value)
        {
            value = 0;
            if (op is Identifier id)
            {
                if (arch.TryGetRegister(id.Name, out var reg))
                {
                    value = Debugger.GetRegisterValue(reg);
                    return true;
                }
                else if (IsFlag(id.Name))
                {
                    eflags_t flags = new eflags_t();
                    flags.dw = Debugger.GetContextData(eContextData.UE_EFLAGS);
                    switch (id.Name[1])
                    {
                    case 'a': value = (flags.bits.AF ? 1u : 0u); break;
                    case 'c': value = (flags.bits.CF ? 1u : 0u); break;
                    case 'd': value = (flags.bits.DF ? 1u : 0u); break;
                    case 'o': value = (flags.bits.OF ? 1u : 0u); break;
                    case 'p': value = (flags.bits.PF ? 1u : 0u); break;
                    case 's': value = (flags.bits.SF ? 1u : 0u); break;
                    case 'z': value = (flags.bits.ZF ? 1u : 0u); break;
                    }
                    return true;
                }
                else if (IsVariable(id.Name))
                {
                    if (variables[id.Name].IsInteger())
                    {
                        value = variables[id.Name].ToUInt64();
                        return true;
                    }
                }
                else if (TryFindConstant(id.Name, out value))
                {
                    return true;
                }
            }
            else if (op is Constant c)
            {
                value = c.ToUInt64();
                return true;
            }
            else if (op is MemoryAccess mem)
            {
                if (GetAddress(mem.EffectiveAddress, out Address ea))
                {
                    //$TODO: add a method IHost.TryReadLeUint32.
                    if (!Host.SegmentMap.TryFindSegment(ea, out ImageSegment? segment))
                        throw new AccessViolationException();
                    bool ret = segment.MemoryArea.TryReadLeUInt32(ea, out uint dw);
                    value = dw;
                    return ret;
                }
            }
            else if (op is BinaryExpression bin)
            {
                if (!GetUlong(bin.Left, out value) ||
                    !GetUlong(bin.Right, out var right))
                    return false;

                if (bin.Operator is UDivOperator && right == 0)
                {
                    errorstr = "Division by 0";
                    return false;
                }

                switch (bin.Operator)
                {
                case IAddOperator _: value += right; break;
                case ISubOperator _: value -= right; break;
                case IMulOperator _: value *= right; break;
                case UDivOperator _: value /= right; break;
                case AndOperator _: value &= right; break;
                case OrOperator _: value |= right; break;
                case XorOperator _: value ^= right; break;
                case ShlOperator _: value >>= (int) right; break;
                case ShrOperator _: value <<= (int) right; break;
                }
                return true;
            }
            value = 0;
            return false;
        }

        bool GetFloat(Expression op, out double value)
        {
            value = 0.0;
            if (op is Constant c)
            {
                value = c.ToDouble();
                return true;
            }
            else if (op is Identifier id)
            {
                if (IsFloatRegister(id.Name))
                {
                    int index = id.Name[3] - '0';
#if LATER
                double reg;
#if _WIN64
			XMM_SAVE_AREA32 fltctx;
			//reg = (double)fltctx.FloatRegisters[index];
#else
                FLOATING_SAVE_AREA fltctx;
                reg = ((double*)&fltctx.RegisterArea[0])[index];
#endif
                if (Debugger.GetContextFPUDataEx(Host.TE_GetCurrentThreadHandle(), &fltctx))
                {
                    value = reg;
                    return true;
                }
#else
                    value = 0;
                    throw new NotImplementedException();
#endif
                }
                else if (IsVariable(id.Name))
                {
                    if (variables[id.Name].type == Var.EType.FLT)
                    {
                        value = variables[id.Name].flt;
                        return true;
                    }
                }
            }
            else if (op is MemoryAccess mem)
            {
                if (GetAddress(mem.EffectiveAddress, out Address ea))
                {
                    if (!Host.SegmentMap.TryFindSegment(ea, out ImageSegment? segment))
                        throw new AccessViolationException();
                    var offset = ea - segment.MemoryArea.BaseAddress;
                    return segment.MemoryArea.TryReadLeDouble(offset, out value);
                }
            }
            else
            {
                //$TODO: evaluate binary exp
                throw new NotImplementedException("//$ return (ParseFloat(op, out string parsed) && GetFloat(parsed, out value));");
            }
            return false;
        }

        bool SetULong(Expression op, ulong value, int size = 0)
        {
            if (size > sizeof(ulong))
                size = sizeof(ulong);

            if (op is Identifier id)
            {
                if (IsVariable(id.Name))
                {
                    variables[id.Name] = Var.Create(value);
                    variables[id.Name].resize(size);
                    return true;
                }
                else if (arch.TryGetRegister(id.Name, out var reg))
                {
                    Debugger.SetRegisterValue(reg, value);
                    return true;
                }
                else if (IsFlag(id.Name))
                {
                    throw new NotImplementedException();
#if NYI
                    bool flagval = value != 0;

                    eflags_t flags;
                    flags.dw = Debugger.GetContextData(UE_EFLAGS);

                    switch (op[1])
                    {
                    case 'a': flags.bits.AF = flagval; break;
                    case 'c': flags.bits.CF = flagval; break;
                    case 'd': flags.bits.DF = flagval; break;
                    case 'o': flags.bits.OF = flagval; break;
                    case 'p': flags.bits.PF = flagval; break;
                    case 's': flags.bits.SF = flagval; break;
                    case 'z': flags.bits.ZF = flagval; break;
                    }

                    return Debugger.SetContextData(eContextData.UE_EFLAGS, flags.dw);
#endif
                }
            }
            else if (op is MemoryAccess)
            {
                throw new NotImplementedException();
#if NYI
                string tmp = Helper.UnquoteString(op, '[', ']');

                rulong target;
                if (GetRulong(tmp, out target))
                {
                    Debug.Assert(target != 0);
                    return Host.TE_WriteMemory(target, size, value);
                }
#endif
            }
            return false;
        }

        bool SetAddress(Expression op, Address addr)
        {
            if (op is Identifier id && variables.TryGetValue(id.Name, out var variable))
            {
                variables[id.Name] = Var.Create(addr);
                return true;
            }
            throw new NotImplementedException();
        }

        bool SetFloat(Expression op, double value)
        {
            if (op is Identifier id)
            {
                if (IsVariable(id.Name))
                {
                    variables[id.Name] = Var.Create(value);
                    return true;
                }
                else if (IsFloatRegister(id.Name))
                {
                    int index = id.Name[3] - '0';
                    double preg = 0.0;
#if _WIN64
			XMM_SAVE_AREA32 fltctx;
			preg = (double*)&fltctx.FloatRegisters + index;
#else
                    FLOATING_SAVE_AREA? fltctx = null;
                    //preg = (double*)&fltctx.RegisterArea[0] + index;
#endif
                    if (Debugger.GetContextFPUDataEx(Host.TE_GetCurrentThreadHandle(), fltctx!))
                    {
                        preg = value;
                        return Debugger.SetContextFPUDataEx(Host.TE_GetCurrentThreadHandle(), fltctx!);
                    }
                }
            }
            else if (op is MemoryAccess mem)
            {
                if (GetAddress(mem.EffectiveAddress, out Address target))
                {
                    return Host.WriteMemory(target, value);
                }
            }
            return false;
        }

        bool SetString(Expression op, string value, int size = 0)
        {
            if (op is Identifier id && IsVariable(id.Name))
            {
                variables[id.Name] = Var.Create(value);
                if (size!=0 && size < variables[id.Name].size)
                    variables[id.Name].resize(size);
                return true;
            }
            else if (op is MemoryAccess mem)
            {
                if (GetAddress(mem, out Address target))
                {
                    var bytes = Encoding.ASCII.GetBytes(value);
                    return Host.WriteMemory(target, Math.Min(size, bytes.Length), bytes);
                }
            }
            return false;
        }

        bool TryFindConstant(string name, out ulong value)
        {
            return constants.TryGetValue(name, out value);
        }

        private bool IsRegister(string s)
        {
            return arch.GetRegister(s) is not null;
        }

        private bool IsFloatRegister(string s)
        {
            return fpu_registers.Any<string>(x => StringComparer.InvariantCultureIgnoreCase.Compare(x, s) == 0);
        }

        private bool IsFlag(string s)
        {
            return e_flags.Any(x => StringComparer.InvariantCultureIgnoreCase.Compare(x, s) == 0);
        }

        bool IsVariable(string s)
        {
            return variables.ContainsKey(s);
        }

        bool IsConstant(string s)
        {
            return constants.ContainsKey(s);
        }

        bool IsValidVariableName(string s)
        {
            return (s.Length != 0 && char.IsLetter(s[0]) && !IsRegister(s) && !IsFloatRegister(s) && !IsConstant(s));
        }

        bool IsWriteable(Expression e)
        {
            if (e is Identifier id)
            {
                var s = id.Name;
                return IsVariable(s) || IsRegister(s) || IsFlag(s) || IsFloatRegister(s);
            }
            return e is MemoryAccess;
        }


        /// <summary>
        /// Given a string literal, interpolates values.
        /// </summary>
        /// <param name="intrString"></param>
        /// <param name="hex8forExec"></param>
        /// <returns></returns>
        string InterpolateVariables(string intrString, bool hex8forExec)
        {
            string ti = intrString.Trim();
            bool insideVar = false;

            var sb = new StringBuilder();
            var varname = new StringBuilder();
            for (int i = 0; i < ti.Length; i++)
            {
                if (ti[i] == '{')
                {
                    insideVar = true;
                }
                else if (ti[i] == '}')
                {
                    insideVar = false;
                    GetAnyValue(MkId(varname.ToString()), out string? value, hex8forExec);
                    sb.Append(value);
                    varname.Clear();
                }
                else
                {
                    char ch = ti[i];
                    if (insideVar)
                        varname.Append(ch);
                    else
                        sb.Append(ch);
                }
            }
            return sb.ToString();
        }

        //Add zero char before dw values, ex: 0DEADBEEF (to be assembled) useful if first char is letter
        public string FormatAsmDwords(string asmLine)
        {
            // Create command and arguments
            string args;
            int pos = asmLine.IndexOfAny(Helper.whitespaces.ToCharArray());

            if (pos < 0)
                return asmLine; //no args

            var command = new StringBuilder(asmLine.Substring(0, pos));
            command.Append(' ');
            args = asmLine.Substring(pos + 1).Trim();
            int iStart = 0;
            while (iStart < args.Length)
            {
                int iEnd = args.IndexOfAny(new [] {'+', '-', ',', '[', ']'}, iStart+1);
                if (iEnd > iStart && args[iStart] == '[' && args[iEnd] == ']')
                { 
                    var arg = args.Substring(iStart+1, iEnd - iStart - 1).Trim();
                    if (Char.IsLetter(arg[0]))
                        command.AppendFormat("[0x{0}]", arg);
                    else 
                        command.AppendFormat("[0x{0}]", arg);
                }
                else
                {
                    if (iEnd < 0)
                        iEnd = args.Length;
                    command.Append(args[iStart]);
                    ++iStart;
                    var arg = args.Substring(iStart, iEnd-iStart);
                    if (Helper.is_hex(arg) && Char.IsLetter(arg[0]))
                        command.Append("0x");
                    command.Append(arg);
                }
                iStart = iEnd + 1;
            }
            return command.ToString().Trim();
        }

        void regBlockToFree(t_dbgmemblock block)
        {
            tMemBlocks.Add(block);
        }

        void regBlockToFree(Address address, ulong size, bool autoclean)
        {
            t_dbgmemblock block = new t_dbgmemblock();

            block.address = address;
            block.size = (uint)size;
            block.autoclean = autoclean;
            block.script_pos = script_pos;
            block.restore_registers = false;

            regBlockToFree(block);
        }

        private bool UnregMemBlock(Address address)
        {
            for (int i = 0; i < tMemBlocks.Count; i++)
            {
                if (tMemBlocks[i].address == address)
                {
                    tMemBlocks.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        private bool FreeMemBlocks()
        {
            for (int i = 0; i < tMemBlocks.Count; i++)
            {
                if (tMemBlocks[i].autoclean)
                    Host.FreeMemory(tMemBlocks[i].address!.Value, tMemBlocks[i].size);
            }
            tMemBlocks.Clear();
            return true;
        }

        private bool SaveRegisters(bool stackToo)
        {
            foreach (var register in arch.GetRegisters())
            {
                if ((int)register.BitSize >= arch.WordWidth.BitSize)
                {
                    if (stackToo || register != arch.StackRegister)
                    {
                        reg_backup.regs[register] = Debugger.GetContextData(register);
                    }
                }
            }
            reg_backup.eflags = Debugger.GetContextData(eContextData.UE_EFLAGS);

            reg_backup.threadid = Host.TE_GetCurrentThreadId();
            reg_backup.script_pos = script_pos;
            reg_backup.loaded = true;
            return true;
        }

        bool RestoreRegisters(bool stackToo)
        {
            if (!reg_backup.loaded)
                return false;

            if (Host.TE_GetCurrentThreadId() != reg_backup.threadid)
                return false;

            foreach (var register in arch.GetRegisters())
            {
                if ((int)register.BitSize >= arch.WordWidth.BitSize)
                {
                    if (stackToo || (register != arch.StackRegister))
                    {
                        Debugger.SetContextData(register, reg_backup.regs[register]);
                    }
                }
            }

            Debugger.SetContextData(eContextData.UE_EFLAGS, reg_backup.eflags);
            return true;
        }

        bool AllocSwbpMem(uint tmpSizet)
        {
            /*
            if(!tmpSizet)
            {
                FreeBpMem();
            }
            else if (!softbp_t || tmpSizet > alloc_bp)
            {
                try
                {
                    if(softbp_t)
                        delete[] softbp_t;
                    softbp_t = new t_bpoint[tmpSizet]; // new tmt_bpointpSizet* ???
                    alloc_bp = tmpSizet;
                }
                catch(...)
                {
                    return false;
                }
            }*/
            return true;
        }

        void FreeBpMem()
        {
            /*
            if(softbp_t)
            {
                delete[] softbp_t;
                softbp_t = null;
            }*/
            //saved_bp = 0;
            //alloc_bp = 0;
        }

        void StepIntoCallback()
        {
            switch (stepcount)
            {
            default: // continue stepping, count > 0
                stepcount--;
                goto case -1;
            case -1: // endless stepping, only enter script command loop on BP/exception
                Debugger.StepInto(StepIntoCallback);
                break;
            case 0: // stop stepping, enter script command loop
                debuggee_running = false;
                StepChecked();
                break;
            }
        }

        void StepOverCallback()
        {
            switch (stepcount)
            {
            default:
                stepcount--;
                goto case -1;
            case -1:
                if (return_to_usercode)
                {
                    if (true/*is_this_user_code(EIP)*/)
                    {
                        return_to_usercode = false;
                        stepcount = 0;
                        StepChecked();
                        break;
                    }
                }
                else if (run_till_return)
                {
                    var instr = Host.Disassemble(Debugger.InstructionPointer);
                    if (instr is not null && (instr.InstructionClass.HasFlag(InstrClass.Return)))
                    {
                        run_till_return = false;
                        stepcount = 0;
                        StepChecked();
                        break;
                    }
                }
                {
                    /*
                    only step over calls and string operations
                    StepOver effectively sets a BP after the current instruction
                    that's not gonna do us any good for jumps
                    so we'll stepinto except for a few exceptions
                    */
                    var instr = (X86Instruction?) Host.Disassemble(Debugger.InstructionPointer);
                    if (instr is not null && (instr.Mnemonic == Mnemonic.call || instr.RepPrefix != 0))
                        Debugger.StepOver(StepOverCallback);
                    else
                        Debugger.StepInto(StepIntoCallback);
                    break;
                }
            case 0:
                StepChecked();
                break;
            }
        }

        bool StepChecked()
        {
            if (!debuggee_running)
            {
                debuggee_running = true;
                Step();
            }
            return true;
        }

        bool StepCallback(int pos, bool returns_value, Var.EType return_type, ref Var result)
        {
            callback_t callback;
            callback.call = (uint) calls.Count;
            callback.returns_value = returns_value;
            callback.return_type = return_type;

            callbacks.Add(callback);

            calls.Add(script_pos + 1);
            script_pos_next = pos;

            bool ret = Step();
            if (ret && returns_value && result is not null)
                result = callback_return;

            return ret;
        }

        void CHC_TRAMPOLINE(object ExceptionData, eCustomException ExceptionId)
        {
            if (CustomHandlerLabels.TryGetValue(ExceptionId, out Expression? it))
            {
                //variables["$TE_ARG_1"] = (rulong)ExceptionData;
                DoCALL(it);
            }
        }

        object Callback_AutoFixIATEx(object fIATPointer)
        {
            Var ret = Var.Empty();

            int label = Script.Labels[Label_AutoFixIATEx];
            variables["$TE_ARG_1"] = Var.Create((ulong)fIATPointer);
            if (StepCallback(label, true, Var.EType.DW, ref ret))
                return (object)ret.ToUInt64();
            else
                return 0;
        }

        void LBPC_TRAMPOLINE(LOAD_DLL_DEBUG_INFO SpecialDBG, eLibraryEvent bpxType)
        {
            Librarian.LIBRARY_ITEM_DATA Lib = Librarian.GetLibraryInfoEx(SpecialDBG.lpBaseOfDll!);
            if (Lib is not null)
            {
                Dictionary<string, string> labels = LibraryBreakpointLabels[bpxType];
                if (labels.TryGetValue(Lib.szLibraryPath!, out string? it))
                {
                    DoCALL(new[] { MkString(it) });
                }
            }
        }

        private Identifier MkId(string id)
        {
            return new Identifier(id, unk, MemoryStorage.Instance);
        }

        private StringConstant MkString(string s)
        {
            return Constant.String(s, StringType.NullTerminated(PrimitiveType.Char));
        }
    }
}