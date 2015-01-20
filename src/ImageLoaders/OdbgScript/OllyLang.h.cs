using System;
using System.Collections.Generic;

////#include <windows.h>
//#include <string>
//#include <vector>
//#include <map>
//#include <limits>
//#include "var.h"
//#include "SDK.hpp"
//#include "types.h"

//// Removes "Script finished" message
////#define TITANSCRIPT_STANDALONE_BUILD

//using namespace TE;
//using std::string;
//using std::wstring;
//using std::vector;
//using std::map;
//using std::numeric_limits;


namespace Decompiler.ImageLoaders.OdbgScript
{
    using System.Runtime.InteropServices;
    using rulong = System.UInt64;

    // This is the table for Script Execution
    public struct t_dbgmemblock
    {
        public rulong address;    //Memory Adress
        public uint size;     //Block Size
        public uint script_pos; //Registred at script pos
        public bool autoclean; //On script restart/change

        public rulong free_at_ip; //To free memory block used in ASM commands

        //Optional actions to do
        public bool restore_registers;

        //Delayed Result Origin
        public bool result_register;
        public eContextData reg_to_return;
    }

    struct t_export
    {
        rulong addr;
        string label; // ;label[256];
    }

    partial class OllyLang
    {
        private
            enum eBreakpointType { PP_INT3BREAK = 0x10, PP_MEMBREAK = 0x20, PP_HWBREAK = 0x40 };

        public

       const byte OS_VERSION_HI = 1;  // High plugin version
       const byte OS_VERSION_LO = 77; // Low plugin version
        //static const byte OS_VERSION_ST = 3;  // plugin state (0 hacked, 1 svn, 2 beta, 3 official release)

        const byte TE_VERSION_HI = 2;
        const byte TE_VERSION_LO = 03;

        const byte TS_VERSION_HI = 0;
        const byte TS_VERSION_LO = 7;

        static OllyLang()
        {
            instance = new OllyLang();
        }
        static OllyLang Instance() { return instance; }
        static OllyLang instance;

        //bool Pause();
        //bool Run();
        //void Reset();
        //void InitGlobalVariables();

        // "Events"
        //void OnBreakpoint(eBreakpointType reason);
        //void OnException();

        bool debuggee_running;
        bool script_running;

        bool run_till_return;
        bool return_to_usercode;

        //private

        //    // Constructor & destructor
        //    OllyLang();
        //    OllyLang(const OllyLang&);
        //    ~OllyLang();

        //OllyLang& operator=(const OllyLang&);

        //typedef bool (OllyLang::*PFCOMMAND)(const string*, size_t);

        public

            OllyScript script = new OllyScript();

        private

            uint script_pos, script_pos_next;

        private

            const int STRING_READSIZE = 256;

        Dictionary<string, var> variables; // Variables that exist
        Dictionary<rulong, uint> bpjumps;  // Breakpoint Auto Jumps 
        List<uint> calls;         // Call/Ret in script

        // Debugger state
        bool back_to_debugloop;
        bool ignore_exceptions;
        int stepcount;

        //allocated memory blocks to free at end of script
        List<t_dbgmemblock> tMemBlocks;

        //last breakpoint reason
        rulong break_reason;
        rulong break_memaddr;

        rulong pmemforexec;
        rulong membpaddr, membpsize;

        // Free Allocated Virtual Memory
        ////bool freeMemBlocks();
        //void regBlockToFree(t_dbgmemblock block);
        //void regBlockToFree(rulong address, int size, bool autoclean);
        //bool unregMemBlock(rulong address);

        bool require_addonaction;

        string errorstr;

	static void SoftwareCallback() { Instance().OnBreakpoint(eBreakpointType.PP_INT3BREAK); }
	static void HardwareCallback() { Instance().OnBreakpoint(eBreakpointType.PP_HWBREAK);   }
	static void   MemoryCallback() { Instance().OnBreakpoint(eBreakpointType.PP_MEMBREAK);  }

#if LATER
	static void StepIntoCallback();
	static void StepOverCallback();

	bool StepChecked();
#endif

    static void EXECJMPCallback() { Instance().DoSTI(); }

        struct callback_t
        {
            uint call;
            bool returns_value;
            var.etype return_type;
        };

        List<callback_t> callbacks;
        var callback_return;

        //bool StepCallback(uint pos, bool returns_value, var.etype return_type, ref var result);

        Dictionary<eCustomException, string> CustomHandlerLabels;
        Dictionary<eCustomException, Debugger.fCustomHandlerCallback> CustomHandlerCallbacks;

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

        string Label_AutoFixIATEx;
        //static void __stdcall Callback_AutoFixIATEx(object fIATPointer);

        Dictionary<eLibraryEvent, Dictionary<string, string>> LibraryBreakpointLabels; //<library path, label name>
        Dictionary<eLibraryEvent, Librarian.fLibraryBreakPointCallback> LibraryBreakpointCallbacks;

        //void LBPC_TRAMPOLINE(const LOAD_DLL_DEBUG_INFO* SpecialDBG, eLibraryEvent bpxType);

        //static void __stdcall LBPC_LOAD(const LOAD_DLL_DEBUG_INFO* SpecialDBG)   { Instance().LBPC_TRAMPOLINE(SpecialDBG, UE_ON_LIB_LOAD); }
        //static void __stdcall LBPC_UNLOAD(const LOAD_DLL_DEBUG_INFO* SpecialDBG) { Instance().LBPC_TRAMPOLINE(SpecialDBG, UE_ON_LIB_UNLOAD); }
        //static void __stdcall LBPC_ALL(const LOAD_DLL_DEBUG_INFO* SpecialDBG)    { Instance().LBPC_TRAMPOLINE(SpecialDBG, UE_ON_LIB_ALL); }

        // ---

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

        struct constant_t
        {
            public constant_t(string name, byte value) { this.name = name; this.value = value; }
            public readonly string name;
            public readonly byte value;
        }

        //static register_t [] registers;
        //static string fpu_registers[];
        //static string e_flags[];
        //static constant_t constants[];

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
            //bool dummy1 : 1;
            //bool PF : 1;
            //bool dummy2 : 1;
            //bool AF : 1;
            //bool dummy3 : 1;
            //bool ZF : 1;
            //bool SF : 1;
            //bool TF : 1;
            //bool IF : 1;
            //bool DF : 1;
            //bool OF : 1
            }
            public readonly  Flagomizer bits;

            public eflags_t()
            {
                bits = new Flagomizer(this);
            }

            //struct
            //{
            //    bool CF : 1;
            //    bool dummy1 : 1;
            //    bool PF : 1;
            //    bool dummy2 : 1;
            //    bool AF : 1;
            //    bool dummy3 : 1;
            //    bool ZF : 1;
            //    bool SF : 1;
            //    bool TF : 1;
            //    bool IF : 1;
            //    bool DF : 1;
            //    bool OF : 1;
            //} bits;
        };

        // Commands that can be executed
        Dictionary<string, Func<string[], bool>> commands;

        int EOB_row, EOE_row;

        bool bInternalBP;

        ulong tickcount_startup;

        byte[] search_buffer;

        // Pseudo-flags to emulate CMP
        bool zf, cf;

        // Cursor for REF / (NEXT)REF function
        int adrREF, curREF;

        //bool ProcessAddonAction();

        void SetCMPFlags(int diff)
        {
            zf = (diff == 0);
            cf = (diff < 0);
        }

        // Commands
        /*
        bool DoADD(const string*, size_t);
        bool DoAI(const string*, size_t);
        bool DoALLOC(const string*, size_t);
        bool DoAN(const string*, size_t);
        bool DoAND(const string*, size_t);
        bool DoAO(const string*, size_t);
        bool DoASK(const string*, size_t);
        bool DoASM(const string*, size_t);
        bool DoASMTXT(const string*, size_t);
        bool DoATOI(const string*, size_t);
        bool DoBC(const string*, size_t);
        bool DoBCA(const string*, size_t);
        bool DoBD(const string*, size_t);
        bool DoBDA(const string*, size_t);
        bool DoBEGINSEARCH(const string*, size_t);
        bool DoBP(const string*, size_t);
        bool DoBPCND(const string*, size_t);
        bool DoBPD(const string*, size_t);
        bool DoBPGOTO(const string*, size_t);
        bool DoBPHWCA(const string*, size_t);
        bool DoBPHWC(const string*, size_t);
        bool DoBPHWS(const string*, size_t);
        bool DoBPL(const string*, size_t);
        bool DoBPLCND(const string*, size_t);
        bool DoBPMC(const string*, size_t);
        bool DoBPRM(const string*, size_t);
        bool DoBPWM(const string*, size_t);
        bool DoBPX(const string*, size_t);
        bool DoBUF(const string*, size_t);
        bool DoCALL(const string*, size_t);
        bool DoCLOSE(const string*, size_t);
        bool DoCMP(const string*, size_t);
        bool DoCMT(const string*, size_t);
        bool DoCOB(const string*, size_t);
        bool DoCOE(const string*, size_t);
        bool DoDBH(const string*, size_t);
        bool DoDBS(const string*, size_t);
        bool DoDEC(const string*, size_t);
        bool DoDIV(const string*, size_t);
        bool DoDM(const string*, size_t);
        bool DoDMA(const string*, size_t);
        bool DoDPE(const string*, size_t);
        bool DoENDE(const string*, size_t);
        bool DoENDSEARCH(const string*, size_t);
        bool DoEOB(const string*, size_t);
        bool DoEOE(const string*, size_t);
        bool DoERUN(const string*, size_t);
        bool DoESTEP(const string*, size_t);
        bool DoESTI(const string*, size_t);
        bool DoEVAL(const string*, size_t);
        bool DoEXEC(const string*, size_t);
        bool DoFILL(const string*, size_t);
        bool DoFIND(const string*, size_t);
        bool DoFINDCALLS(const string*, size_t);
        bool DoFINDCMD(const string*, size_t);
        bool DoFINDOP(const string*, size_t);
        bool DoFINDMEM(const string*, size_t);
        bool DoFREE(const string*, size_t);
        bool DoGAPI(const string*, size_t);
        bool DoGBPM(const string*, size_t);
        bool DoGBPR(const string*, size_t);
        bool DoGCI(const string*, size_t);
        bool DoGCMT(const string*, size_t);
        bool DoGFO(const string*, size_t);
        bool DoGLBL(const string*, size_t);
        bool DoGMA(const string*, size_t);
        bool DoGMEMI(const string*, size_t);
        bool DoGMEXP(const string*, size_t);
        bool DoGMI(const string*, size_t);
        bool DoGMIMP(const string*, size_t);
        bool DoGN(const string*, size_t);
        bool DoGO(const string*, size_t);
        bool DoGOPI(const string*, size_t);
        bool DoGPA(const string*, size_t);
        bool DoGPP(const string*, size_t);
        bool DoGPI(const string*, size_t);
        bool DoGREF(const string*, size_t);
        bool DoGRO(const string*, size_t);
        bool DoGSL(const string*, size_t);
        bool DoGSTR(const string*, size_t);
        bool DoHANDLE(const string*, size_t);
        bool DoHISTORY(const string*, size_t);
        bool DoINC(const string*, size_t);
        bool DoITOA(const string*, size_t);
        bool DoJA(const string*, size_t);
        bool DoJAE(const string*, size_t);
        bool DoJB(const string*, size_t);
        bool DoJBE(const string*, size_t);
        bool DoJE(const string*, size_t);
        bool DoJMP(const string*, size_t);
        bool DoJNE(const string*, size_t);
        bool DoKEY(const string*, size_t);
        bool DoLBL(const string*, size_t);
        bool DoLC(const string*, size_t);	
        bool DoLCLR(const string*, size_t);
        bool DoLEN(const string*, size_t);
        bool DoLOADLIB(const string*, size_t);
        bool DoLOG(const string*, size_t);
        bool DoLOGBUF(const string*, size_t);
        bool DoLM(const string*, size_t);
        bool DoMEMCPY(const string*, size_t);
        bool DoMOV(const string*, size_t);
        bool DoMSG(const string*, size_t);
        bool DoMSGYN(const string*, size_t);
        bool DoMUL(const string*, size_t);
        bool DoNAMES(const string*, size_t);
        bool DoNEG(const string*, size_t);
        bool DoNOT(const string*, size_t);
        bool DoOLLY(const string*, size_t);
        bool DoOR(const string*, size_t);
        bool DoOPCODE(const string*, size_t);
        bool DoOPENDUMP(const string*, size_t);
        bool DoOPENTRACE(const string*, size_t);
        bool DoPAUSE(const string*, size_t);
        bool DoPOP(const string*, size_t);
        bool DoPOPA(const string*, size_t);
        bool DoPREOP(const string*, size_t);
        bool DoPUSH(const string*, size_t);
        bool DoPUSHA(const string*, size_t);
        bool DoRBP(const string*, size_t);
        bool DoREADSTR(const string*, size_t);
        bool DoREFRESH(const string*, size_t);
        bool DoREPL(const string*, size_t);
        bool DoRESET(const string*, size_t);
        bool DoREF(const string*, size_t);
        bool DoRET(const string*, size_t);
        bool DoREV(const string*, size_t);
        bool DoROL(const string*, size_t);
        bool DoROR(const string*, size_t);
        bool DoRTR(const string*, size_t);
        bool DoRTU(const string*, size_t);
        bool DoRUN(const string*, size_t);
        bool DoSBP(const string*, size_t);
        bool DoSCMP(const string*, size_t);
        bool DoSCMPI(const string*, size_t);
        bool DoSETOPTION(const string*, size_t);
        bool DoSHL(const string*, size_t);
        bool DoSHR(const string*, size_t);
        bool DoSTI(const string*, size_t);
        bool DoSTO(const string*, size_t);
        bool DoSTR(const string*, size_t);
        bool DoSUB(const string*, size_t);
        bool DoTC(const string*, size_t);
        bool DoTEST(const string*, size_t);
        bool DoTI(const string*, size_t);
        bool DoTICK(const string*, size_t);
        bool DoTICND(const string*, size_t);
        bool DoTO(const string*, size_t);
        bool DoTOCND(const string*, size_t);
        bool DoUNICODE(const string*, size_t);
        bool DoVAR(const string*, size_t);
        bool DoXOR(const string*, size_t);
        bool DoXCHG(const string*, size_t);
        bool DoWRT(const string*, size_t);
        bool DoWRTA(const string*, size_t);

        // TE commands
        bool DoError(const string*, size_t);
        bool DoDumpAndFix(const string*, size_t);
        bool DoStopDebug(const string*, size_t);
        bool DoDumpProcess(const string*, size_t);
        bool DoDumpRegions(const string*, size_t);
        bool DoDumpModule(const string*, size_t);
        bool DoPastePEHeader(const string*, size_t);
        bool DoExtractOverlay(const string*, size_t);
        bool DoAddOverlay(const string*, size_t);
        bool DoCopyOverlay(const string*, size_t);
        bool DoRemoveOverlay(const string*, size_t);
        bool DoResortFileSections(const string*, size_t);
        bool DoMakeAllSectionsRWE(const string*, size_t);
        bool DoAddNewSection(const string*, size_t);
        bool DoResizeLastSection(const string*, size_t);
        bool DoGetPE32Data(const string*, size_t);
        bool DoSetPE32Data(const string*, size_t);
        bool DoGetPE32SectionNumberFromVA(const string*, size_t);
        bool DoConvertVAtoFileOffset(const string*, size_t);
        bool DoConvertFileOffsetToVA(const string*, size_t);
        bool DoIsFileDLL(const string*, size_t);
        bool DoRealignPE(const string*, size_t);
        bool DoRelocaterCleanup(const string*, size_t);
        bool DoRelocaterInit(const string*, size_t);
        bool DoRelocaterAddNewRelocation(const string*, size_t);
        bool DoRelocaterEstimatedSize(const string*, size_t);
        bool DoRelocaterExportRelocation(const string*, size_t);
        bool DoRelocaterExportRelocationEx(const string*, size_t);
        bool DoRelocaterMakeSnapshot(const string*, size_t);
        bool DoRelocaterCompareTwoSnapshots(const string*, size_t);
        bool DoRelocaterChangeFileBase(const string*, size_t);
        bool DoThreaderPauseThread(const string*, size_t);
        bool DoThreaderResumeThread(const string*, size_t);
        bool DoThreaderTerminateThread(const string*, size_t);
        bool DoThreaderPauseAllThreads(const string*, size_t);
        bool DoThreaderResumeAllThreads(const string*, size_t);
        bool DoGetDebuggedDLLBaseAddress(const string*, size_t);
        bool DoGetDebuggedFileBaseAddress(const string*, size_t);
        bool DoGetJumpDestination(const string*, size_t);
        bool DoIsJumpGoingToExecute(const string*, size_t);
        bool DoGetPEBLocation(const string*, size_t);
        bool DoDetachDebuggerEx(const string*, size_t);
        bool DoSetCustomHandler(const string*, size_t);
        bool DoImporterCleanup(const string*, size_t);
        bool DoImporterSetImageBase(const string*, size_t);
        bool DoImporterInit(const string*, size_t);
        bool DoImporterAddNewDll(const string*, size_t);
        bool DoImporterAddNewAPI(const string*, size_t);
        bool DoImporterAddNewOrdinalAPI(const string*, size_t);
        bool DoImporterGetAddedDllCount(const string*, size_t);
        bool DoImporterGetAddedAPICount(const string*, size_t);
        bool DoImporterMoveIAT(const string*, size_t);
        bool DoImporterRelocateWriteLocation(const string*, size_t);
        bool DoImporterExportIAT(const string*, size_t);
        bool DoImporterEstimatedSize(const string*, size_t);
        bool DoImporterExportIATEx(const string*, size_t);
        bool DoImporterGetNearestAPIAddress(const string*, size_t);
        bool DoImporterAutoSearchIAT(const string*, size_t);
        bool DoImporterAutoSearchIATEx(const string*, size_t);
        bool DoImporterAutoFixIATEx(const string*, size_t);
        bool DoImporterAutoFixIAT(const string*, size_t);
        bool DoTracerLevel1(const string*, size_t);
        bool DoHashTracerLevel1(const string*, size_t);
        bool DoTracerDetectRedirection(const string*, size_t);
        bool DoTracerFixKnownRedirection(const string*, size_t);
        bool DoTracerFixRedirectionViaImpRecPlugin(const string*, size_t);
        bool DoExporterCleanup(const string*, size_t);
        bool DoExporterSetImageBase(const string*, size_t);
        bool DoExporterInit(const string*, size_t);
        bool DoExporterAddNewExport(const string*, size_t);
        bool DoExporterAddNewOrdinalExport(const string*, size_t);
        bool DoExporterGetAddedExportCount(const string*, size_t);
        bool DoExporterEstimatedSize(const string*, size_t);
        bool DoExporterBuildExportTable(const string*, size_t);
        bool DoExporterBuildExportTableEx(const string*, size_t);
        bool DoLibrarianSetBreakPoint(const string*, size_t);
        bool DoLibrarianRemoveBreakPoint(const string*, size_t);
        bool DoTLSRemoveCallback(const string*, size_t);
        bool DoTLSRemoveTable(const string*, size_t);
        bool DoTLSBackupData(const string*, size_t);
        bool DoTLSRestoreData(const string*, size_t);
        bool DoHandlerIsHandleOpen(const string*, size_t);
        bool DoHandlerCloseRemoteHandle(const string*, size_t);
        bool DoStaticFileLoad(const string*, size_t);
        bool DoStaticFileUnload(const string*, size_t);

        bool callCommand(PFCOMMAND command, int count, ...);

        bool Step();

        size_t GetStringOperatorPos(const string& ops);
        size_t GetRulongOperatorPos(const string& ops);
        size_t GetFloatOperatorPos(const string& ops);

        bool ParseString(const string& arg, string& result);
        bool ParseRulong(const string& arg, string& result);
        bool ParseFloat (const string& arg, string& result);

        //bool ParseOperands(const string* args, string* results, size_t count, bool preferstr = false);

        bool GetRulong(const string& op, rulong& value);
        */
        bool GetNum<T>(string op, ref T value)
        {
            rulong temp;

            if (GetRulong(op, out temp)/* && temp <= numeric_limits<T>::max()*/)
            {
                value = temp;
                return true;
            }
            return false;
        }

        //bool GetFloat(const string& op, double& value);
        //bool GetString(const string& op, string& value, size_t size = 0);
        //bool GetStringLiteral(const string& op, string& value);
        //bool GetBytestring(const string& op, string& value, size_t size = 0);
        //bool GetBool(const string& op, bool& value);

        //bool GetAnyValue(const string& op, string& value, bool hex8forExec = false);

        //bool SetRulong(const string& op, const rulong& value, size_t size = sizeof(rulong));

        bool SetNum<T>(string op, T value, int size = -1)
        {
            if (size < 0)
                size = Marshal.SizeOf(value);
            return SetRulong(op, (ulong)value, size);
        }

        //bool SetFloat(const string& op, const double& value);
        //bool SetString(const string& op, const string& value, size_t size = 0);
        ////bool SetBytestring(const string& op, string& value, size_t size = 0);
        //bool SetBool(const string& op, const bool& value);

        //const register_t* find_register(const string& name);
        //const constant_t* find_constant(const string& name);

        //bool is_register(const string& s);
        //bool is_floatreg(const string& s);
        //bool is_flag(const string& s);
        //bool is_variable(const string& s);
        //bool is_constant(const string& s);
        //bool is_valid_variable_name(const string& s);
        //bool is_writable(const string&s);

        //string ResolveVarsForExec(const string& in, bool hex8forExec);

        //string FormatAsmDwords(const string& asmLine);

        // Save / Restore Breakpoints
        /*
        t_hardbpoint hwbp_t[4];
        t_sorted sortedsoftbp_t;
        t_bpoint* softbp_t;
        */

        uint saved_bp;
        uint alloc_bp;
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
        t_reg_backup reg_backup;

        //bool SaveRegisters(bool stackToo);
        //bool RestoreRegisters(bool stackToo);

        //cache for GMEXP
        List<t_export> tExportsCache;
        ulong exportsCacheAddr;

        //cache for GMIMP
        List<t_export> tImportsCache;
        ulong importsCacheAddr;
    }
}