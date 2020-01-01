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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.OdbgScript
{
    using Reko.Arch.X86;
    using Reko.Core.Expressions;
    using rulong = System.UInt64;

    // This is the table for Script Execution
    public class t_dbgmemblock
    {
        public rulong address;    //Memory Adress
        public uint size;     //Block Size
        public uint script_pos; //Registred at script pos
        public bool autoclean; //On script restart/change

        public Address free_at_ip; //To free memory block used in ASM commands

        //Optional actions to do
        public bool restore_registers;

        //Delayed Result Origin
        public bool result_register;
        public eContextData reg_to_return;
    }

    public class t_export
    {
        public rulong addr;
        public string label; // ;label[256];
    }

    public partial class OllyLang : IScriptInterpreter
    {
        private const byte OS_VERSION_HI = 1;  // High plugin version
        private const byte OS_VERSION_LO = 77; // Low plugin version
        private const byte TE_VERSION_HI = 2;
        private const byte TE_VERSION_LO = 03;
        private const byte TS_VERSION_HI = 0;
        private const byte TS_VERSION_LO = 7;
        private const int STRING_READSIZE = 256;

        private readonly IServiceProvider services;
        private readonly IProcessorArchitecture arch;
        private readonly List<uint> calls = new List<uint>();         // Call/Ret in script
        public readonly Dictionary<string, Var> variables = new Dictionary<string, Var>(); // Variables that exist
        private readonly Dictionary<rulong, uint> bpjumps = new Dictionary<rulong, uint>();  // Breakpoint Auto Jumps 
        public  bool debuggee_running;
        public  bool script_running;
        public  bool run_till_return;
        public  bool return_to_usercode;
        private  uint script_pos, script_pos_next;

        // Debugger state
        private bool resumeDebuggee;
        private bool ignore_exceptions;
        private int stepcount;

        //allocated memory blocks to free at end of script
        private readonly List<t_dbgmemblock> tMemBlocks = new List<t_dbgmemblock>();

        //last breakpoint reason
        private rulong break_reason;
        private rulong break_memaddr;
        private rulong pmemforexec;
        private Address membpaddr;
        private ulong membpsize;
        //private bool require_addonaction;
        //private bool back_to_debugloop;
        private string errorstr;

        public OllyLang(IServiceProvider services, IProcessorArchitecture arch)
        {
            this.services = services;
            this.arch = arch;
            this.Script = new OllyScript(this);

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

        void SoftwareCallback() { OnBreakpoint(eBreakpointType.PP_INT3BREAK); }
        void HardwareCallback() { OnBreakpoint(eBreakpointType.PP_HWBREAK); }
        void MemoryCallback()   { OnBreakpoint(eBreakpointType.PP_MEMBREAK); }
        void EXECJMPCallback()  { DoSTI(); }

        public struct callback_t
        {
            public uint call;
            public bool returns_value;
            public Var.etype return_type;
        }

        private readonly List<callback_t> callbacks = new List<callback_t>();
        private Var callback_return;

        //bool StepCallback(uint pos, bool returns_value, var.etype return_type, ref var result);

        private readonly Dictionary<eCustomException, string> CustomHandlerLabels = new Dictionary<eCustomException, string>();
        private readonly Dictionary<eCustomException, Debugger.fCustomHandlerCallback> CustomHandlerCallbacks = new Dictionary<eCustomException, Debugger.fCustomHandlerCallback>();

        //void CHC_TRAMPOLINE(object ExceptionData, eCustomException ExceptionId);

        //static void __stdcall CHC_BREAKPOINT(object  ExceptionData)              { Instance().CHC_TRAMPOLINE(ExceptionData, UE_CH_BREAKPOINT); }
        //static void __stdcall CHC_SINGLESTEP(object  ExceptionData)              { Instance().CHC_TRAMPOLINE(ExceptionData, UE_CH_SINGLESTEP); }
        //static void __stdcall CHC_ACCESSVIOLATION(object  ExceptionData)         { Instance().CHC_TRAMPOLINE(ExceptionData, UE_CH_ACCESSVIOLATION); }
        //static void __stdcall CHC_ILLEGALINSTRUCTION(object  ExceptionData)      { Instance().CHC_TRAMPOLINE(ExceptionData, UE_CH_ILLEGALINSTRUCTION); }
        //static void __stdcall CHC_NONCONTINUABLEEXCEPTION(object  ExceptionData) { Instance().CHC_TRAMPOLINE(ExceptionData, UE_CH_NONCONTINUABLEEXCEPTION); }
        //static void __stdcall CHC_ARRAYBOUNDSEXCEPTION(object  ExceptionData)    { Instance().CHC_TRAMPOLINE(ExceptionData, UE_CH_ARRAYBOUNDSEXCEPTION); }
        //static void __stdcall CHC_FLOATDENORMALOPERAND(object  ExceptionData)    { Instance().CHC_TRAMPOLINE(ExceptionData, UE_CH_FLOATDENORMALOPERAND); }
        //static void __stdcall CHC_FLOATDEVIDEBYZERO(object  ExceptionData)       { Instance().CHC_TRAMPOLINE(ExceptionData, UE_CH_FLOATDEVIDEBYZERO); }
        //static void __stdcall CHC_INTEGERDEVIDEBYZERO(object  ExceptionData)     { Instance().CHC_TRAMPOLINE(ExceptionData, UE_CH_INTEGERDEVIDEBYZERO); }
        //static void __stdcall CHC_INTEGEROVERFLOW(object  ExceptionData)         { Instance().CHC_TRAMPOLINE(ExceptionData, UE_CH_INTEGEROVERFLOW); }
        //static void __stdcall CHC_PRIVILEGEDINSTRUCTION(object  ExceptionData)   { Instance().CHC_TRAMPOLINE(ExceptionData, UE_CH_PRIVILEGEDINSTRUCTION); }
        //static void __stdcall CHC_PAGEGUARD(object  ExceptionData)               { Instance().CHC_TRAMPOLINE(ExceptionData, UE_CH_PAGEGUARD); }
        //static void __stdcall CHC_EVERYTHINGELSE(object  ExceptionData)          { Instance().CHC_TRAMPOLINE(ExceptionData, UE_CH_EVERYTHINGELSE); }
        //static void __stdcall CHC_CREATETHREAD(object  ExceptionData)            { Instance().CHC_TRAMPOLINE(ExceptionData, UE_CH_CREATETHREAD); }
        //static void __stdcall CHC_EXITTHREAD(object  ExceptionData)              { Instance().CHC_TRAMPOLINE(ExceptionData, UE_CH_EXITTHREAD); }
        //static void __stdcall CHC_CREATEPROCESS(object  ExceptionData)           { Instance().CHC_TRAMPOLINE(ExceptionData, UE_CH_CREATEPROCESS); }
        //static void __stdcall CHC_EXITPROCESS(object  ExceptionData)             { Instance().CHC_TRAMPOLINE(ExceptionData, UE_CH_EXITPROCESS); }
        //static void __stdcall CHC_LOADDLL(object  ExceptionData)                 { Instance().CHC_TRAMPOLINE(ExceptionData, UE_CH_LOADDLL); }
        //static void __stdcall CHC_UNLOADDLL(object  ExceptionData)               { Instance().CHC_TRAMPOLINE(ExceptionData, UE_CH_UNLOADDLL); }
        //static void __stdcall CHC_OUTPUTDEBUGSTRING(object  ExceptionData)       { Instance().CHC_TRAMPOLINE(ExceptionData, UE_CH_OUTPUTDEBUGSTRING); }

        private string Label_AutoFixIATEx = "";
        //static void __stdcall Callback_AutoFixIATEx(object fIATPointer);

        private Dictionary<eLibraryEvent, Dictionary<string, string>> LibraryBreakpointLabels = //<library path, label name>
            new Dictionary<eLibraryEvent, Dictionary<string, string>>();
        private Dictionary<eLibraryEvent, Librarian.fLibraryBreakPointCallback> LibraryBreakpointCallbacks =
            new Dictionary<eLibraryEvent, Librarian.fLibraryBreakPointCallback>();

        //void LBPC_TRAMPOLINE(const LOAD_DLL_DEBUG_INFO* SpecialDBG, eLibraryEvent bpxType);

        //static void __stdcall LBPC_LOAD(const LOAD_DLL_DEBUG_INFO* SpecialDBG)   { Instance().LBPC_TRAMPOLINE(SpecialDBG, UE_ON_LIB_LOAD); }
        //static void __stdcall LBPC_UNLOAD(const LOAD_DLL_DEBUG_INFO* SpecialDBG) { Instance().LBPC_TRAMPOLINE(SpecialDBG, UE_ON_LIB_UNLOAD); }
        //static void __stdcall LBPC_ALL(const LOAD_DLL_DEBUG_INFO* SpecialDBG)    { Instance().LBPC_TRAMPOLINE(SpecialDBG, UE_ON_LIB_ALL); }

        private enum eBreakpointType { PP_INT3BREAK = 0x10, PP_MEMBREAK = 0x20, PP_HWBREAK = 0x40 };

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

        public class constant_t
        {
            public constant_t(string name, byte value) { this.name = name; this.value = value; }
            public readonly string name;
            public readonly byte value;
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
        Dictionary<string, Func<string[], bool>> commands = new Dictionary<string, Func<string[], bool>>();

        private int EOB_row, EOE_row;
        private bool bInternalBP;
        private ulong tickcount_startup;
        private byte[] search_buffer;

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
        bool GetByte(string op, ref byte value)
        {
            if (GetRulong(op, out rulong temp) && temp <= Byte.MaxValue)
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
            public rulong[] regs = new rulong[17];
            public ulong eflags;
            public uint threadid;
            public uint script_pos;
        }
        t_reg_backup reg_backup = new t_reg_backup();

        //cache for GMEXP
        List<t_export> tExportsCache = new List<t_export>();
        //ulong exportsCacheAddr;

        //cache for GMIMP
        List<t_export> tImportsCache = new List<t_export>();
        //ulong importsCacheAddr;

#if _WIN64

    register_t [] registers = 
    {
	new register_t("rax",  eContextData.UE_RAX, 8, 0), new register_t("rbx",  eContextData.UE_RBX, 8, 0), new register_t("rcx",  eContextData.UE_RCX, 8, 0),
	new register_t("rdx",  eContextData.UE_RDX, 8, 0), new register_t("rsi",  eContextData.UE_RSI, 8, 0), new register_t("rdi",  eContextData.UE_RDI, 8, 0),
	new register_t("rbp",  eContextData.UE_RBP, 8, 0), new register_t("rsp",  eContextData.UE_RSP, 8, 0), new register_t("rip",  eContextData.UE_RIP, 8, 0),

	new register_t("r8",   eContextData.UE_R8,  8, 0), new register_t("r9",   eContextData.UE_R9,  8, 0), new register_t("r10",  eContextData.UE_R10, 8, 0),
	new register_t("r11",  eContextData.UE_R11, 8, 0), new register_t("r12",  eContextData.UE_R12, 8, 0), new register_t("r13",  eContextData.UE_R13, 8, 0),
	new register_t("r14",  eContextData.UE_R14, 8, 0), new register_t("r15",  eContextData.UE_R15, 8, 0),

	new register_t("dr0",  eContextData.UE_DR0, 8, 0), new register_t("dr1",  eContextData.UE_DR1, 8, 0), new register_t("dr2",  eContextData.UE_DR2, 8, 0),
	new register_t("dr3",  eContextData.UE_DR3, 8, 0), new register_t("dr6",  eContextData.UE_DR6, 8, 0), new register_t("dr7",  eContextData.UE_DR7, 8, 0),

	new register_t("eax",  eContextData.UE_RAX, 4, 0), new register_t("ebx",  eContextData.UE_RBX, 4, 0), new register_t("ecx",  eContextData.UE_RCX, 4, 0),
	new register_t("edx",  eContextData.UE_RDX, 4, 0), new register_t("esi",  eContextData.UE_RSI, 4, 0), new register_t("edi",  eContextData.UE_RDI, 4, 0),
	new register_t("ebp",  eContextData.UE_RBP, 4, 0), new register_t("esp",  eContextData.UE_RSP, 4, 0),

	new register_t("r8d",  eContextData.UE_R8,  4, 0), new register_t("r9d",  eContextData.UE_R9,  4, 0), new register_t("r10d", eContextData.UE_R10, 4, 0),
	new register_t("r11d", eContextData.UE_R11, 4, 0), new register_t("r12d", eContextData.UE_R12, 4, 0), new register_t("r13d", eContextData.UE_R13, 4, 0),
	new register_t("r14d", eContextData.UE_R14, 4, 0), new register_t("r15d", eContextData.UE_R15, 4, 0),

	new register_t("ax",   eContextData.UE_RAX, 2, 0), new register_t("bx",   eContextData.UE_RBX, 2, 0), new register_t("cx",   eContextData.UE_RCX, 2, 0),
	new register_t("dx",   eContextData.UE_RDX, 2, 0), new register_t("si",   eContextData.UE_RSI, 2, 0), new register_t("di",   eContextData.UE_RDI, 2, 0),
	new register_t("bp",   eContextData.UE_RBP, 2, 0), new register_t("sp",   eContextData.UE_RSP, 2, 0),

	new register_t("r8w",  eContextData.UE_R8,  2, 0), new register_t("r9w",  eContextData.UE_R9,  2, 0), new register_t("r10w", eContextData.UE_R10, 2, 0),
	new register_t("r11w", eContextData.UE_R11, 2, 0), new register_t("r12w", eContextData.UE_R12, 2, 0), new register_t("r13w", eContextData.UE_R13, 2, 0),
	new register_t("r14w", eContextData.UE_R14, 2, 0), new register_t("r15w", eContextData.UE_R15, 2, 0),

	new register_t("ah",   eContextData.UE_RAX, 1, 1), new register_t("bh",   eContextData.UE_RBX, 1, 1), new register_t("ch",   eContextData.UE_RCX, 1, 1),
	new register_t("dh",   eContextData.UE_RDX, 1, 1),

	new register_t("al",   eContextData.UE_RAX, 1, 0), new register_t("bl",   eContextData.UE_RBX, 1, 0), new register_t("cl",   eContextData.UE_RCX, 1, 0),
	new register_t("dl",   eContextData.UE_RDX, 1, 0), new register_t("sil",  eContextData.UE_RSI, 1, 0), new register_t("dil",  eContextData.UE_RDI, 1, 0),
	new register_t("bpl",  eContextData.UE_RBP, 1, 0), new register_t("spl",  eContextData.UE_RSP, 1, 0),

	new register_t("r8b",  eContextData.UE_R8,  1, 0), new register_t("r9b",  eContextData.UE_R9,  1, 0), new register_t("r10b", eContextData.UE_R10, 1, 0),
	new register_t("r11b", eContextData.UE_R11, 1, 0), new register_t("r12b", eContextData.UE_R12, 1, 0), new register_t("r13b", eContextData.UE_R13, 1, 0),
	new register_t("r14b", eContextData.UE_R14, 1, 0), new register_t("r15b", eContextData.UE_R15, 1, 0),
};

#else

        readonly register_t[] registers = new register_t[]
        {
            new register_t("eax", eContextData.UE_EAX, 4, 0), new register_t("ebx", eContextData.UE_EBX, 4, 0), new register_t("ecx", eContextData.UE_ECX, 4, 0),
            new register_t("edx", eContextData.UE_EDX, 4, 0), new register_t("esi", eContextData.UE_ESI, 4, 0), new register_t("edi", eContextData.UE_EDI, 4, 0),
            new register_t("ebp", eContextData.UE_EBP, 4, 0), new register_t("esp", eContextData.UE_ESP, 4, 0), new register_t("eip", eContextData.UE_EIP, 4, 0),

            new register_t("dr0", eContextData.UE_DR0, 4, 0), new register_t("dr1", eContextData.UE_DR1, 4, 0), new register_t("dr2", eContextData.UE_DR2, 4, 0),
            new register_t("dr3", eContextData.UE_DR3, 4, 0), new register_t("dr6", eContextData.UE_DR6, 4, 0), new register_t("dr7", eContextData.UE_DR7, 4, 0),

            new register_t("ax", eContextData.UE_EAX, 2, 0), new register_t("bx", eContextData. UE_EBX, 2, 0), new register_t("cx", eContextData. UE_ECX, 2, 0),
            new register_t("dx", eContextData.UE_EDX, 2, 0), new register_t("si", eContextData. UE_ESI, 2, 0), new register_t("di", eContextData. UE_EDI, 2, 0),
            new register_t("bp", eContextData. UE_EBP, 2, 0), new register_t("sp", eContextData. UE_ESP, 2, 0),

            new register_t("ah", eContextData. UE_EAX, 1, 1), new register_t("bh", eContextData. UE_EBX, 1, 1), new register_t("ch", eContextData. UE_ECX, 1, 1),
            new register_t("dh", eContextData. UE_EDX, 1, 1),

            new register_t("al", eContextData. UE_EAX, 1, 0), new register_t("bl", eContextData. UE_EBX, 1, 0), new register_t("cl", eContextData. UE_ECX, 1, 0),
            new register_t("dl", eContextData. UE_EDX, 1, 0)
        };

#endif

        readonly static string[] fpu_registers = { "st(0)", "st(1)", "st(2)", "st(3)", "st(4)", "st(5)", "st(6)", "st(7)" };

        readonly static string[] e_flags = { "!cf", "!pf", "!af", "!zf", "!sf", "!df", "!of" };

        readonly static constant_t[] constants =
{
	new constant_t("true",  1),
	new constant_t("false", 0),
	new constant_t("null",  0),
    
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
        public IHost Host { get; set; }
        public OllyScript Script { get; set; }

        public void Dispose()
        {
            if (search_buffer != null)
                DoENDSEARCH();
            FreeMemBlocks();
        }

        public void InitGlobalVariables()
        {
            // Global variables
            variables["$INPUTFILE"] = Var.Create(Host.TE_GetTargetPath());

            string name = Host.TE_GetOutputPath();
            if (string.IsNullOrEmpty(name))
            {
                string ext;
                int offs;
                name = Host.TE_GetTargetPath();
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

            pmemforexec = 0;
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

        public void LoadFromFile(string scriptFilename, string curDir)
        {
            Script.LoadFile(scriptFilename, curDir);
        }

        public void LoadFromString(string scriptString, string curDir)
        {
            Script.LoadScriptFromString(scriptString, curDir);
        }

        public void Run()
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

            while (!resumeDebuggee && Script.IsLoaded && script_running)
            {
                if (tickcount_startup == 0)
                    tickcount_startup = Helper.MyTickCount();

                script_pos = (uint) Script.NextCommandIndex((int)script_pos_next);

                // Check if script out of bounds
                if (script_pos >= Script.Lines.Count)
                    return false;

                var line = Script.Lines[(int)script_pos];

                script_pos_next = script_pos + 1;

                // Log line of code if  enabled
                if (Script.Log)
                {
                    Host.TE_Log("--> " + line.RawLine, Host.TS_LOG_COMMAND);
                }

                // Find command and execute it
                Func<string[], bool> cmd = line.CommandPtr;
                if (cmd == null && commands.TryGetValue(line.Command, out var it))
                {
                        line.CommandPtr = cmd = it;
                    }

                bool result = false;
                if (cmd != null)
                {
                    result = cmd(line.args); // Call command
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
                    string message = "Error on line " + Helper.rul2decstr(line.LineNumber) + ": " + line.RawLine + "\r\n" + errorstr;
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
            for (int i = 0; i < tMemBlocks.Count; )
            {
                if (tMemBlocks[i].free_at_ip == ip)
                {
                    Host.TE_FreeMemory(tMemBlocks[i].address, tMemBlocks[i].size);
                    if (tMemBlocks[i].result_register)
                        variables["$RESULT"] = Var.Create((rulong)Debugger.GetContextData(tMemBlocks[i].reg_to_return));
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

        void OnBreakpoint(eBreakpointType reason)
        {
            if (bInternalBP) //dont process temporary bp (exec/ende/go)
            {
                bInternalBP = false;
            }
            else
            {
                break_reason = (rulong)reason;

                if (EOB_row > -1)
                {
                    script_pos_next = (uint)EOB_row;
                }
                else
                {
                    rulong ip = Debugger.InstructionPointer.ToLinear();
                    if (bpjumps.TryGetValue(ip, out uint it))
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
                script_pos_next = (uint)EOE_row;
            }
            else if (ignore_exceptions)
            {
                return;
            }
            StepChecked();
        }

        int GetStringOperatorPos(string ops)
        {
            string cache = ops;
            int b = 0, e = 0, p = 0;

            //hide [pointer operations]
            while ((p = cache.IndexOf('[', p)) >= 0 && (e = cache.IndexOf(']', p)) >= 0)
            {
                cache = cache.Remove(p) + cache.Substring(e - p + 1);
            }
            e = p = 0;

            //Search for operator(s) outside "quotes" 
            while ((b = cache.IndexOf('\"', b)) >= 0)
            {
                //Check Before
                p = cache.IndexOf('+');
                if (e >= 0 && p < b)
                {
                    return p;
                }

                if ((e = cache.IndexOf('\"', b + 1)) >= 0)
                {
                    //Check between
                    if ((p = cache.IndexOf('+', e + 1)) >= 0 && p < cache.IndexOf('\"', e + 1))
                    {
                        return p;
                    }
                    b = e;
                }
                b++;
            }

            //Check after
            return cache.IndexOf('+', e);
        }

        int GetRulongOperatorPos(string ops)
        {
            var operators = "+-*/&|^<>".ToCharArray();
            int b = 0, e = 0, p;

            //Search for operator(s) outside [pointers]
            while ((b = ops.IndexOf('[', b)) >= 0)
            {
                //Check Before
                p = ops.IndexOfAny(operators);
                if (e >= 0 && p < b)
                {
                    return p;
                }
                e = ops.IndexOf(']', b + 1);
                if (e >= 0)
                {
                    //Check between
                    if ((p = ops.IndexOfAny(operators, e + 1)) >= 0 && p < ops.IndexOf('[', e + 1))
                    {
                        return p;
                    }
                    b = e;
                }
                b++;
            }

            // look for operators after
            return ops.IndexOfAny(operators, e);
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

        /*
        bool ParseOperands(const string* args, string* results, int count, bool preferstr)
        {
            for(int i = 0; i < count; i++) 
            {
                results[i] = args[i];

                continue;

                if(preferstr || args[i].IndexOf('\"') >= 0)
                {
                    if(!ParseString(args[i], results[i]))
                    {
                        if(!ParseRulong(args[i], results[i]))
                        {
                            ParseFloat(args[i], results[i]);
                        }
                    }
                }
                else
                {
                    if(!ParseRulong(args[i], results[i]))
                    {
                        if(!ParseFloat(args[i], results[i]))
                        {
                            ParseString(args[i], results[i]);
                        }
                    }
                }
            }
            return true;
        }
        */

        bool ParseString(string arg, out string result)
        {
            int start = 0, offs;
            char oper = '+';
            Var val = Var.Create("");
            result = "";

            if ((offs = GetStringOperatorPos(arg)) >= 0)
            {
                do
                {
                    string token = Helper.trim(arg.Substring(start, offs));

                    if (!GetString(token, out string curval))
                        return false;

                    switch (oper)
                    {
                    case '+': val += Var.Create(curval); break;
                    }

                    if (offs < 0)
                        break;

                    oper = arg[start + offs];

                    start += offs + 1;
                    offs = GetRulongOperatorPos(arg.Substring(start));
                }
                while (start < arg.Length);

                if (!val.IsBuf)
                    result = '\"' + val.str + '\"';
                else
                    result = val.str;
                return true;
            }

            return false;
        }

        bool ParseRulong(string arg, out string result)
        {
            int start = 0, offs;
            char oper = '+';
            rulong val = 0, curval;

            result = "";
            if ((offs = GetRulongOperatorPos(arg)) >= 0)
            {
                do
                {
                    if (start == 0 && offs == 0) // allow leading +/-
                    {
                        curval = 0;
                    }
                    else
                    {
                        string token = Helper.trim(arg.Substring(start, offs));

                        if (!GetRulong(token, out curval))
                            return false;
                    }

                    if (oper == '/' && curval == 0)
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
                    case '&': val &= curval; break;
                    case '|': val |= curval; break;
                    case '^': val ^= curval; break;
                    case '>': val >>= (int)curval; break;
                    case '<': val <<= (int)curval; break;
                    }

                    if (offs < 0)
                        break;

                    oper = arg[start + offs];

                    start += offs + 1;
                    offs = GetRulongOperatorPos(arg.Substring(start));
                }
                while (start < arg.Length);

                result = Helper.rul2hexstr(val);
                return true;
            }

            return false;
        }

        bool ParseFloat(string arg, out string result)
        {
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
        }

        bool GetAnyValue(string op, out string value, bool hex8forExec = false)
        {
            value = null;

            if (IsVariable(op))
            {
                Var  v = variables[op];
                if (v.IsString())
                {
                    value = v.str;
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
            else if (op.Contains(':') && GetAddress(op, out Address addr))
            {
                value = addr.ToString();
                //$TODO: hex8forexec?
                return true;
            }
            else if (Helper.is_float(op))
            {
                value = op;
                return true;
            }
            else if (Helper.is_hex(op))
            {
                if (hex8forExec)
                    value = '0' + op;
                else
                    value = op;
                return true;
            }
            else if (Helper.is_dec(op))
            {
                value = Helper.rul2hexstr(Helper.decstr2rul(op.Substring(0, op.Length - 1))).ToUpperInvariant();
                return true;
            }
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
                value = op;
                return true;
            }
            else if (Helper.IsMemoryAccess(op))
            {
                return GetString(op, out value);
            }
            else if (GetRulong(op, out ulong dw))
            {
                if (hex8forExec)
                    value = '0' + Helper.rul2hexstr(dw).ToUpperInvariant();
                else
                    value = Helper.rul2hexstr(dw).ToUpperInvariant();
                return true;
            }
            return false;
        }

        bool GetString(string op, out string value)
        {
            return GetString(op, 0, out value);
        }

        bool GetString(string op, int size, out string value)
        {
            value = "";
            if (IsVariable(op))
            {
                if (variables[op].IsString())
                {
                    if (size != 0 && size < variables[op].size)
                    {
                        Var tmp = variables[op];
                        tmp.resize(size);
                        value = tmp.str;
                    }
                    else
                    {
                        value = variables[op].str;
                    }
                    return true;
                }
            }
            else if (Helper.IsStringLiteral(op))
            {
                value = Helper.UnquoteString(op, '"');
                if (size!=0 && size < value.Length)
                    value = value.Remove(size);
                return true;
            }
            else if (Helper.IsHexLiteral(op))
            {
                if (size!= 0 && (size * 2) < (op.Length - 2))
                    value = op.Substring(0, (size * 2) + 1) + '#';
                else
                    value = op;
                return true;
            }
            else if (Helper.IsMemoryAccess(op))
            {
                string tmp = Helper.UnquoteString(op, '[', ']');
                if (GetAddress(tmp, out Address src))
                {
                    Debug.Assert(src != null);

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
                        if (!Host.SegmentMap.TryFindSegment(ea, out ImageSegment segment))
                            throw new AccessViolationException();
                        byte[] buffer = new byte[STRING_READSIZE];

                        if (segment.MemoryArea.TryReadBytes(ea, buffer.Length, buffer))
                        {
                            buffer[buffer.Length - 1] = 0;
                            value = Encoding.UTF8.GetString(buffer);
                            return true;
                        }
                    }
                }
            }
            else
            {
                return (ParseString(op, out string parsed) && GetString(parsed, size, out value));
            }
            value = "";
            return false;
        }

        bool GetBool(string op, out bool value)
        {
            if (GetRulong(op, out rulong temp))
            {
                value = temp != 0;
                return true;
            }
            value = false;
            return false;
        }

        public bool GetAddress(string op, out Address value)
        {
            value = null;
            if (op.StartsWith("["))
                return false;
            int iColon = op.IndexOf(':');
            if (iColon > 0)
            {
                // Possible segmented address. Evaluate part before
                // and after colon.
                if (GetRulong(op.Remove(iColon), out var seg) &&
                    GetRulong(op.Substring(iColon+1), out var off))
                {
                    var cSeg = Constant.UInt16((ushort) seg);
                    var cOff = Constant.UInt32((uint) off);
                    value = arch.MakeSegmentedAddress(cSeg, cOff);
                    return true;
                }
            }
            if (IsVariable(op) && variables[op].Address != null)
            {
                value = variables[op].Address;
                return true;
            }
            if (!GetRulong(op, out rulong uAddr))
                return false;
            value = arch.MakeAddressFromConstant(Constant.UInt64(uAddr), false);
            return true;
        }

        bool GetRulong(string op, out rulong value)
        {
            value = 0;
            if (arch.TryGetRegister(op, out var reg))
            {
                value = Debugger.GetRegisterValue(reg);
                    return true;
                }
            else if (is_flag(op))
            {
                eflags_t flags = new eflags_t();
                flags.dw = Debugger.GetContextData(eContextData.UE_EFLAGS);
                switch (op[1])
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
            else if (IsVariable(op))
            {
                if (variables[op].IsInteger())
                {
                    value = variables[op].ToUInt64();
                    return true;
                }
            }
            else if (is_constant(op))
            {
                value = find_constant(op).value;
                return true;
            }
            else if (Helper.is_hex(op))
            {
                value = Helper.hexstr2rul(op);
                return true;
            }
            else if (Helper.is_dec(op))
            {
                value = Helper.decstr2rul(op.Substring(0, op.Length - 1));
                return true;
            }
            else if (Helper.IsQuotedString(op, '[', ']'))
            {
                string tmp = Helper.UnquoteString(op, '[', ']');

                if (GetAddress(tmp, out Address ea))
                {
                    if (!Host.SegmentMap.TryFindSegment(ea, out ImageSegment segment))
                        throw new AccessViolationException();
                    bool ret = segment.MemoryArea.TryReadLeUInt32(ea, out uint dw);
                    value = dw;
                    return ret;
                }
            }
            else
            {
                return (ParseRulong(op, out string parsed) &&
                        GetRulong(parsed, out value));
            }
            value = 0;
            return false;
        }

        bool GetFloat(string op, out double value)
        {
            value = 0.0;
            if (Helper.is_float(op))
            {
                value = Helper.str2dbl(op);
                return true;
            }
            else if (is_floatreg(op))
            {
                int index = op[3] - '0';
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
            else if (IsVariable(op))
            {
                if (variables[op].type == Var.etype.FLT)
                {
                    value = variables[op].flt;
                    return true;
                }
            }
            else if (Helper.IsMemoryAccess(op))
            {
                string tmp = Helper.UnquoteString(op, '[', ']');

                if (GetRulong(tmp, out rulong src))
                {
                    Debug.Assert(src != 0);
                    var ea = Address.Ptr32((uint) src);
                    if (!Host.SegmentMap.TryFindSegment(ea, out ImageSegment segment))
                        throw new AccessViolationException();
                    value = segment.MemoryArea.ReadLeDouble(ea).ToDouble();
                    return true;
                }
            }
            else
            {
                return (ParseFloat(op, out string parsed) && GetFloat(parsed, out value));
            }
            return false;
        }

        bool SetRulong(string op, rulong value, int size = 0)
        {
            if (size > sizeof(rulong))
                size = sizeof(rulong);

            if (IsVariable(op))
            {
                variables[op] = Var.Create(value);
                variables[op].resize(size);
                return true;
            }
            else if (arch.TryGetRegister(op, out var reg))
            {
                Debugger.SetRegisterValue(reg, value);
                return true;
                }
            else if (is_flag(op))
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
            else if (Helper.IsMemoryAccess(op))
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

        bool SetAddress(string op, Address addr)
        {
            if (IsVariable(op))
            {
                variables[op] = Var.Create(addr);
                return true;
            }
            throw new NotImplementedException();
        }

        bool SetFloat(string op, double value)
        {
            if (IsVariable(op))
            {
                variables[op] = Var.Create(value);
                return true;
            }
            else if (is_floatreg(op))
            {
                int index = op[3] - '0';
                double preg = 0.0;
#if _WIN64
			XMM_SAVE_AREA32 fltctx;
			preg = (double*)&fltctx.FloatRegisters + index;
#else
                FLOATING_SAVE_AREA fltctx = null;
                //preg = (double*)&fltctx.RegisterArea[0] + index;
#endif
                if (Debugger.GetContextFPUDataEx(Host.TE_GetCurrentThreadHandle(), fltctx))
                {
                    preg = value;
                    return Debugger.SetContextFPUDataEx(Host.TE_GetCurrentThreadHandle(), fltctx);
                }
            }
            else if (Helper.IsMemoryAccess(op))
            {
                string tmp = Helper.UnquoteString(op, '[', ']');

                if (GetAddress(tmp, out Address target))
                {
                    Debug.Assert(target != null);

                    return Host.WriteMemory(target, value);
                }
            }
            return false;
        }

        bool SetString(string op, string value, int size = 0)
        {
            if (IsVariable(op))
            {
                variables[op] = Var.Create(value);
                if (size!=0 && size < variables[op].size)
                    variables[op].resize(size);
                return true;
            }
            else if (Helper.IsMemoryAccess(op))
            {
                string tmp = Helper.UnquoteString(op, '[', ']');
                if (GetAddress(tmp, out Address target))
                {
                    Debug.Assert(target != null);
                    var bytes = Encoding.ASCII.GetBytes(value);
                    return Host.WriteMemory(target, Math.Min(size, bytes.Length), bytes);
                }
            }
            return false;
        }

        bool SetBool(string op, bool value)
        {
            return SetRulong(op, value?1u:0u, 1);
        }

        register_t find_register(string name)
        {
            string lower = name.ToLowerInvariant();
            for (int i = 0; i < registers.Length; i++)
            {
                if (registers[i].name == lower)
                    return registers[i];
            }
            return null;
        }

        constant_t find_constant(string name)
        {
            string lower = name.ToLowerInvariant();
            for (int i = 0; i < constants.Length; i++)
            {
                if (constants[i].name == lower)
                    return constants[i];
            }
            return default(constant_t);
        }

        bool is_register(string s)
        {
            return (find_register(s) != null);
        }

        bool is_floatreg(string s)
        {
            return fpu_registers.Any<string>(x => StringComparer.InvariantCultureIgnoreCase.Compare(x, s) == 0);
        }

        bool is_flag(string s)
        {
            return e_flags.Any(x => StringComparer.InvariantCultureIgnoreCase.Compare(x, s) == 0);
        }

        bool IsVariable(string s)
        {
            return (variables.ContainsKey(s));
        }

        bool is_constant(string s)
        {
            return (find_constant(s) != null);
        }

        bool is_valid_variable_name(string s)
        {
            return (s.Length != 0 && char.IsLetter(s[0]) && !is_register(s) && !is_floatreg(s) && !is_constant(s));
        }

        bool is_writable(string s)
        {
            return (IsVariable(s) || Helper.IsMemoryAccess(s) || is_register(s) || is_flag(s) || is_floatreg(s));
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
                    GetAnyValue(varname.ToString(), out string value, hex8forExec);
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
            return Helper.trim(command.ToString());
        }

        private bool CallCommand(Func<string[], bool> command, params string[] args)
        {
            return command(args);
        }

        void regBlockToFree(t_dbgmemblock block)
        {
            tMemBlocks.Add(block);
        }

        void regBlockToFree(rulong address, rulong size, bool autoclean)
        {
            t_dbgmemblock block = new t_dbgmemblock();

            block.address = address;
            block.size = (uint)size;
            block.autoclean = autoclean;
            block.script_pos = script_pos;
            block.restore_registers = false;

            regBlockToFree(block);
        }

        private bool UnregMemBlock(rulong address)
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
                    Host.TE_FreeMemory(tMemBlocks[i].address, tMemBlocks[i].size);
            }
            tMemBlocks.Clear();
            return true;
        }

        private bool SaveRegisters(bool stackToo)
        {
            for (int i = 0; i < registers.Length; i++)
            {
                if (registers[i].size == sizeof(rulong))
                {
                    eContextData reg = registers[i].id;
                    if (stackToo || (reg != eContextData.UE_ESP && reg != eContextData.UE_RSP && reg != eContextData.UE_EBP && reg != eContextData.UE_RBP))
                    {
                        reg_backup.regs[i] = Debugger.GetContextData(reg);
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

            for (int i = 0; i <registers.Length; i++)
            {
                if (registers[i].size == sizeof(rulong))
                {
                    eContextData reg = registers[i].id;
                    if (stackToo || (reg != eContextData.UE_ESP && reg != eContextData.UE_RSP && reg != eContextData.UE_EBP && reg != eContextData.UE_RBP))
                    {
                        Debugger.SetContextData(reg, reg_backup.regs[i]);
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
                    var instr = (X86Instruction) Host.DisassembleEx(Debugger.InstructionPointer);
                    if (instr.code == Arch.X86.Mnemonic.ret ||
                       instr.code == Arch.X86.Mnemonic.retf)
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
                    var instr = (X86Instruction) Host.DisassembleEx(Debugger.InstructionPointer);
                    if (instr.code == Arch.X86.Mnemonic.call || instr.repPrefix != 0)
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

        bool StepCallback(uint pos, bool returns_value, Var.etype return_type, ref Var result)
        {
            callback_t callback;
            callback.call = (uint) calls.Count;
            callback.returns_value = returns_value;
            callback.return_type = return_type;

            callbacks.Add(callback);

            calls.Add(script_pos + 1);
            script_pos_next = pos;

            bool ret = Step();
            if (ret && returns_value && result != null)
                result = callback_return;

            return ret;
        }

        void CHC_TRAMPOLINE(object ExceptionData, eCustomException ExceptionId)
        {
            if (CustomHandlerLabels.TryGetValue(ExceptionId, out string it))
            {
                //variables["$TE_ARG_1"] = (rulong)ExceptionData;
                DoCALL(it);
            }
        }

        object Callback_AutoFixIATEx(object fIATPointer)
        {
            Var ret = Var.Empty();

            uint label = Script.Labels[Label_AutoFixIATEx];
            variables["$TE_ARG_1"] = Var.Create((rulong)fIATPointer);
            if (StepCallback(label, true, Var.etype.DW, ref ret))
                return (object)ret.ToUInt64();
            else
                return 0;
        }

        void LBPC_TRAMPOLINE(LOAD_DLL_DEBUG_INFO SpecialDBG, eLibraryEvent bpxType)
        {
            Librarian.LIBRARY_ITEM_DATA Lib = Librarian.GetLibraryInfoEx(SpecialDBG.lpBaseOfDll);
            if (Lib != null)
            {
                Dictionary<string, string> labels = LibraryBreakpointLabels[bpxType];
                if (labels.TryGetValue(Lib.szLibraryPath, out string it))
                {
                    DoCALL(new[] { it });
                }
            }
        }
    }
}