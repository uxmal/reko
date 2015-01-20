namespace Decompiler.ImageLoaders.OdbgScript
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using rulong = System.UInt64;
    partial class OllyLang
    {
#if _WIN64

    register_t [] registers = 
{
	{"rax",  UE_RAX, 8, 0}, {"rbx",  UE_RBX, 8, 0}, {"rcx",  UE_RCX, 8, 0},
	{"rdx",  UE_RDX, 8, 0}, {"rsi",  UE_RSI, 8, 0}, {"rdi",  UE_RDI, 8, 0},
	{"rbp",  UE_RBP, 8, 0}, {"rsp",  UE_RSP, 8, 0}, {"rip",  UE_RIP, 8, 0},

	{"r8",   UE_R8,  8, 0}, {"r9",   UE_R9,  8, 0}, {"r10",  UE_R10, 8, 0},
	{"r11",  UE_R11, 8, 0}, {"r12",  UE_R12, 8, 0}, {"r13",  UE_R13, 8, 0},
	{"r14",  UE_R14, 8, 0}, {"r15",  UE_R15, 8, 0},

	{"dr0",  UE_DR0, 8, 0}, {"dr1",  UE_DR1, 8, 0}, {"dr2",  UE_DR2, 8, 0},
	{"dr3",  UE_DR3, 8, 0}, {"dr6",  UE_DR6, 8, 0}, {"dr7",  UE_DR7, 8, 0},

	{"eax",  UE_RAX, 4, 0}, {"ebx",  UE_RBX, 4, 0}, {"ecx",  UE_RCX, 4, 0},
	{"edx",  UE_RDX, 4, 0}, {"esi",  UE_RSI, 4, 0}, {"edi",  UE_RDI, 4, 0},
	{"ebp",  UE_RBP, 4, 0}, {"esp",  UE_RSP, 4, 0},

	{"r8d",  UE_R8,  4, 0}, {"r9d",  UE_R9,  4, 0}, {"r10d", UE_R10, 4, 0},
	{"r11d", UE_R11, 4, 0}, {"r12d", UE_R12, 4, 0}, {"r13d", UE_R13, 4, 0},
	{"r14d", UE_R14, 4, 0}, {"r15d", UE_R15, 4, 0},

	{"ax",   UE_RAX, 2, 0}, {"bx",   UE_RBX, 2, 0}, {"cx",   UE_RCX, 2, 0},
	{"dx",   UE_RDX, 2, 0}, {"si",   UE_RSI, 2, 0}, {"di",   UE_RDI, 2, 0},
	{"bp",   UE_RBP, 2, 0}, {"sp",   UE_RSP, 2, 0},

	{"r8w",  UE_R8,  2, 0}, {"r9w",  UE_R9,  2, 0}, {"r10w", UE_R10, 2, 0},
	{"r11w", UE_R11, 2, 0}, {"r12w", UE_R12, 2, 0}, {"r13w", UE_R13, 2, 0},
	{"r14w", UE_R14, 2, 0}, {"r15w", UE_R15, 2, 0},

	{"ah",   UE_RAX, 1, 1}, {"bh",   UE_RBX, 1, 1}, {"ch",   UE_RCX, 1, 1},
	{"dh",   UE_RDX, 1, 1},

	{"al",   UE_RAX, 1, 0}, {"bl",   UE_RBX, 1, 0}, {"cl",   UE_RCX, 1, 0},
	{"dl",   UE_RDX, 1, 0}, {"sil",  UE_RSI, 1, 0}, {"dil",  UE_RDI, 1, 0},
	{"bpl",  UE_RBP, 1, 0}, {"spl",  UE_RSP, 1, 0},

	{"r8b",  UE_R8,  1, 0}, {"r9b",  UE_R9,  1, 0}, {"r10b", UE_R10, 1, 0},
	{"r11b", UE_R11, 1, 0}, {"r12b", UE_R12, 1, 0}, {"r13b", UE_R13, 1, 0},
	{"r14b", UE_R14, 1, 0}, {"r15b", UE_R15, 1, 0},
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

        public OllyLang()
        {
            // Init command array
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
            /*
            variables["$RESULT"] = 0;

            script_running = false;

            script_pos_next = 0;
            EOB_row = EOE_row = -1;
            zf = cf = 0;
            log_commands = false;
            search_buffer = null;

            back_to_debugloop = false;
            require_addonaction = false;

            saved_bp = 0;
            alloc_bp = 0;
            //softbp_t = null;

            //for(int i = 0; i < 4; i++)
            //	hwbp_t[i].addr = 0;
            */
        }

        public void Dispose()
        {
            if (search_buffer != null)
                DoENDSEARCH();
            //FreeBpMem();
            freeMemBlocks();
        }

        // Loaded script state
        public class OllyScript
        {
            public class scriptline_t
            {
                public uint linenum;
                public string line;
                public bool is_command;
                public string command;
                public Func<string[], bool> commandptr;
                public List<string> args;
            }

            public bool log;
            public string path;
            public List<scriptline_t> lines;
            public Dictionary<string, uint> labels;

            public OllyScript() { loaded = (false); log = (false); }

            //bool load_file(const wchar_t* file, const wchar_t* dir = null);
            //bool load_buff(const char* buff, int size, const wchar_t* dir = null);
            public bool isloaded() { return loaded; }
            //uint next_command(uint from = 0);
            public void clear() { loaded = false; path = ""; ; lines.Clear(); labels.Clear(); }

            //bool is_label(string  s);

            private

                bool loaded;

            //void parse_insert(const List<string>& toInsert, const string& currentdir);

            void parse_insert(List<string> toInsert, string currentdir)
            {
                uint curline = 1;
                bool in_comment = false, in_asm = false;

                loaded = true;

                for (int i = 0; i < toInsert.Count; i++, curline++)
                {
                    string scriptline = Helper.trim(toInsert[i]);
                    bool nextline = false;
                    int curpos = 0; // for skipping string literals

                    while (!nextline)
                    {
                        // Handle comments and string literals
                        int linecmt = -1, spancmt = -1, strdel = -1;

                        if (curpos < scriptline.Length)
                            if (in_comment)
                            {
                                spancmt = 0;
                            }
                            else
                            {
                                int min = -1, tmp;

                                tmp = scriptline.IndexOf("//", curpos);
                                if (tmp < min)
                                    min = linecmt = tmp;
                                tmp = scriptline.IndexOf(';', curpos);
                                if (tmp < min)
                                    min = linecmt = tmp;
                                tmp = scriptline.IndexOf("/*", curpos);
                                if (tmp < min)
                                    min = spancmt = tmp;
                                tmp = scriptline.IndexOf('\"', curpos);
                                if (tmp < min)
                                    min = strdel = tmp;

                                curpos = min;

                                if (linecmt != min)
                                    linecmt = -1;
                                if (spancmt != min)
                                    spancmt = -1;
                                if (strdel != min)
                                    strdel = -1;
                            }

                        if (strdel >= 0)
                        {
                            curpos = scriptline.IndexOf('\"', strdel + 1); // find end of string
                            if (curpos >= 0)
                                curpos++;
                        }
                        else if (linecmt >= 0)
                        {
                            scriptline = scriptline.Remove(linecmt);
                        }
                        else if (spancmt >= 0)
                        {
                            int start = in_comment ? spancmt : spancmt + 2;
                            int end = scriptline.IndexOf("*/", start);
                            in_comment = (end < 0);
                            if (in_comment)
                                scriptline = scriptline.Remove(spancmt);
                            else
                                scriptline = scriptline.Remove(spancmt) + scriptline.Substring(end - spancmt + 2);
                        }
                        else
                        {
                            scriptline = Helper.trim(scriptline);
                            int len = scriptline.Length;

                            if (len != 0)
                            {
                                string lcline = Helper.tolower(scriptline);

                                // Check for label
                                if (!in_asm && len > 1 && scriptline[len - 1] == ':')
                                {
                                    scriptline = scriptline.Remove(len - 1);
                                    labels[Helper.trim(scriptline)] = (uint)(lines.Count);
                                }
                                // Check for #inc and include file if it exists
                                else if (0 == lcline.IndexOf("#inc"))
                                {
                                    if (len > 5 && Char.IsWhiteSpace(lcline[4]))
                                    {
                                        string args = Helper.trim(scriptline.Substring(5));
                                        string filename;
                                        if (args.Length > 2 && args[0] == '\"' && args.EndsWith("\""))
                                        {
                                            string dir;
                                            string philename = Helper.pathfixup(args.Substring(1, args.Length - 2), false);
                                            if (!Helper.isfullpath(philename))
                                            {
                                                philename = currentdir + philename;
                                                dir = currentdir;
                                            }
                                            else
                                                dir = Helper.folderfrompath(philename);

                                            parse_insert(Helper.getlines_file(philename), dir);
                                        }
                                        else Host.MsgError("Bad #inc directive!");
                                    }
                                    else Host.MsgError("Bad #inc directive!");
                                }
                                // Logging
                                else if (!in_asm && lcline == "#log")
                                {
                                    log = true;
                                }
                                // Add line
                                else
                                {
                                    scriptline_t cur = new scriptline_t();

                                    if (in_asm && lcline == "ende")
                                        in_asm = false;

                                    cur.line = scriptline;
                                    cur.linenum = curline;
                                    cur.is_command = !in_asm;

                                    if (!in_asm && lcline == "exec")
                                        in_asm = true;

                                    int pos = scriptline.IndexOfAny(Helper.whitespaces.ToCharArray());
                                    if (pos >= 0)
                                    {
                                        cur.command = Helper.tolower(scriptline.Substring(0, pos));
                                        Helper.split(cur.args, scriptline.Substring(pos + 1), ',');
                                    }
                                    else
                                    {
                                        cur.command = Helper.tolower(scriptline);
                                    }

                                    lines.Add(cur);
                                }
                            }
                            nextline = true;
                        }
                    }
                }
            }

            public int next_command(int from)
            {
                while (from < lines.Count && !lines[from].is_command)
                {
                    from++;
                }
                return from;
            }

            /*
            bool load_file(const char* file, const char* dir)
            {
                clear();

                char cdir[MAX_PATH];
                GetCurrentDirectory(_countof(cdir), cdir);
                string curdir = Helper.pathfixup(cdir, true);
                string sdir;

                path = Helper.pathfixup(file, false);
                if(!Helper.isfullpath(path))
                {
                    path = curdir + path;
                }
                if(!dir)
                    sdir = Helper.folderfrompath(path);
                else
                    sdir = dir;

                List<string> unparsedScript = getlines_file(path);
                parse_insert(unparsedScript, sdir);

                return true;
            }
            */

            bool load_file(string file, string dir)
            {
                clear();

                string cdir = Environment.CurrentDirectory;
                string curdir = Helper.pathfixup(cdir, true);
                string sdir;

                path = Helper.pathfixup(file, false);
                if (!Helper.isfullpath(path))
                {
                    path = curdir + path;
                }
                if (string.IsNullOrEmpty(dir))
                    sdir = Helper.folderfrompath(path);
                else
                    sdir = dir;

                List<string> unparsedScript = Helper.getlines_file(path);
                parse_insert(unparsedScript, sdir);

                //TSErrorExit = false;
                return true;
            }

            bool load_buff(string buff, string dir)
            {
                clear();

                string curdir = Helper.pathfixup(Environment.CurrentDirectory, true);
                string sdir;

                path = "";
                if (dir == null)
                {
                    sdir = curdir;
                }
                else
                    sdir = dir;

                List<string> unparsedScript = Helper.getlines_buff(new StringReader(buff), buff.Length);
                parse_insert(unparsedScript, sdir);

                //$LATER TSErrorExit = false;
                return true;
            }

            public bool is_label(string s)
            {
                return (labels.ContainsKey(s));
            }
        }

        void InitGlobalVariables()
        {

            // Global variables
            variables["$INPUTFILE"] = Host.TE_GetTargetPath();

            string name = Host.TE_GetOutputPath();
            if (name.Length == null)
            {
                string ext;
                int offs;
                name = Host.TE_GetTargetPath();
                if ((offs = name.rfind('.')) >= 0)
                {
                    ext = name.substr(offs);
                    ext.insert(0, ".unpacked");
                    name.erase(offs);
                    name.append(ext);
                }
            }
            variables["$OUTPUTFILE"] = new var(name);
        }

        void Reset()
        {
            freeMemBlocks();
            variables.Clear();
            bpjumps.Clear();
            calls.Clear();

            variables["$OS_VERSION"] = new var(Helper.rul2decstr(OS_VERSION_HI) + '.' + Helper.rul2decstr(OS_VERSION_LO));
            variables["$TE_VERSION"] = new var(Helper.rul2decstr(TE_VERSION_HI) + '.' + Helper.rul2decstr(TE_VERSION_LO));
            variables["$TS_VERSION"] = new var(Helper.rul2decstr(TS_VERSION_HI) + '.' + Helper.rul2decstr(TS_VERSION_LO));
            variables["$VERSION"] = variables["$OS_VERSION"];                   
            variables["$WIN_VERSION"] = new var(Environment.OSVersion.VersionString);

#if _WIN64
	variables["$PLATFORM"] = "x86-64";
#else
            variables["$PLATFORM"] = new var("x86-32");
#endif

            EOB_row = EOE_row = -1;

            zf = cf = 0;
            search_buffer = null;

            saved_bp = 0;
            alloc_bp = 0;

            script_running = false;

            script_pos_next = 0;
            tickcount_startup = 0;
            break_memaddr = 0;
            break_reason = 0;

            pmemforexec = 0;
            membpaddr = 0;
            membpsize = 0;

            reg_backup.loaded = false;

            variables["$RESULT"] = 0;

            callbacks.Clear();
            debuggee_running = false;
            require_addonaction = false;

            run_till_return = false;
            return_to_usercode = false;

            tExportsCache.Clear();
            exportsCacheAddr = 0;
            tImportsCache.Clear();
            importsCacheAddr = 0;
        }

        bool Run()
        {
            script_running = true;
            Step();
            return true;
        }

        bool Pause()
        {
            script_running = false;
            return true;
        }

        bool Step()
        {
            back_to_debugloop = false;
            ignore_exceptions = false;
            stepcount = 0;

            while (!back_to_debugloop && script.isloaded() && script_running)
            {
                if (tickcount_startup == 0)
                    tickcount_startup = MyTickCount();

                script_pos = script.next_command(script_pos_next);

                // Check if script out of bounds
                if (script_pos >= script.lines.Count)
                    return false;

                OllyScript.scriptline_t line = script.lines[script_pos];

                script_pos_next = script_pos + 1;

                // Log line of code if  enabled
                if (script.log)
                {
                    string logstr = "-. " + line.line;
                    Host.TE_Log(logstr, Host.TS_LOG_COMMAND);
                }

                bool result = false;

                // Find command and execute it
                Func<string[], bool> cmd = line.commandptr;
                if (cmd == null)
                {
                    Func<string[], bool> it;
                    if (commands.TryGetValue(line.command, out it))
                    {
                        line.commandptr = cmd = it;
                    }
                }

                if (cmd != null)
                {
                    result = cmd(line.args); // Call command
                }
                else errorstr = "Unknown command: " + line.command;

                if (callbacks.Count && back_to_debugloop)
                {
                    result = false;
                    errorstr = "Unallowed command during callback: " + line.command;
                }

                // Error in processing, show error and die
                if (!result)
                {
                    Pause();
                    string message = "Error on line " + Helper.rul2decstr(line.linenum) + ": " + line.line + "\r\n" + errorstr;
                    Host.MsgError(message);
                    errorstr = "";
                    return false;
                }
            }
            return true;
        }

        //Executed after some commands to clean memory or to get something after ollydbg processing
        bool ProcessAddonAction()
        {
            bool restore_registers = false;

            rulong ip = Debugger.GetContextData(eGetContextData.UE_CIP);

            for (int i = 0; i < tMemBlocks.Count; )
            {
                if (tMemBlocks[i].free_at_ip == ip)
                {
                    Host.TE_FreeMemory(tMemBlocks[i].address, tMemBlocks[i].size);
                    if (tMemBlocks[i].result_register)
                        variables["$RESULT"] = new var((rulong)Debugger.GetContextData(tMemBlocks[i].reg_to_return));
                    if (tMemBlocks[i].restore_registers)
                        restore_registers = true;
                    require_addonaction = false;
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
                    rulong ip = Debugger.GetContextData(eContextData.UE_CIP);
                    uint it;
                    if (bpjumps.TryGetValue(op, out it))
                    {
                        script_pos_next = it;
                    }
                }
            }

            StepChecked();
        }

        void OnException()
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
                cache.erase(p, e - p + 1);
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
            string operators = "+-*/&|^<>";
            int b = 0, e = 0, p;

            // []]
            // [[]
            // [[]]

            //Search for operator(s) outside [pointers]
            while ((b = ops.IndexOf('[', b)) >= 0)
            {
                //Check Before
                p = ops.IndexOfAny(operators);
                if (r >= 0 && p < b)
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

            //Check after
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

        bool ParseString(string arg, ref string result)
        {
            int start = 0, offs;
            char oper = '+';
            var val = new var("");
            string curval;

            if ((offs = GetStringOperatorPos(arg)) >= 0)
            {
                do
                {
                    string token = Helper.trim(arg.Substring(start, offs));

                    if (!GetString(token, curval))
                        return false;

                    switch (oper)
                    {
                    case '+': val += new var(curval); break;
                    }

                    if (offs < 0)
                        break;

                    oper = arg[start + offs];

                    start += offs + 1;
                    offs = GetRulongOperatorPos(arg.Substring(start));
                }
                while (start < arg.Count);

                if (!val.isbuf)
                    result = '\"' + val.str + '\"';
                else
                    result = val.str;
                return true;
            }

            return false;
        }

        bool ParseRulong(string arg, ref string result)
        {
            int start = 0, offs;
            char oper = '+';
            rulong val = 0, curval;

            if ((offs = GetRulongOperatorPos(arg)) >= 0)
            {
                do
                {
                    if (!start && !offs) // allow leading +/-
                    {
                        curval = 0;
                    }
                    else
                    {
                        string token = trim(arg.substr(start, offs));

                        if (!GetRulong(token, curval))
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
                    case '>': val >>= curval; break;
                    case '<': val <<= curval; break;
                    }

                    if (offs < 0)
                        break;

                    oper = arg[start + offs];

                    start += offs + 1;
                    offs = GetRulongOperatorPos(arg.substr(start));
                }
                while (start < arg.Count);

                result = rul2hexstr(val);
                return true;
            }

            return false;
        }

        bool ParseFloat(string arg, out string result)
        {
            int start = 0, offs;
            char oper = '+';
            double val = 0, curval;

            if ((offs = GetFloatOperatorPos(arg)) >= 0)
            {
                do
                {
                    if (!start && !offs) // allow leading +/-
                    {
                        curval = 0.0;
                    }
                    else
                    {
                        string token = trim(arg.substr(start, offs));

                        if (!GetFloat(token, curval))
                        {
                            //Convert integer to float (but not for first operand)
                            rulong dw;
                            if (start && GetRulong(token, out dw))
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

                    result.resize(p + psize + 1);
                }
                return true;
            }

            return false;
        }

        bool GetAnyValue(string op, out string value, bool hex8forExec = false)
        {
            rulong dw;

            if (is_variable(op))
            {
                var  v = variables[op];
                if (v.type == var.etype.STR)
                {
                    value = v.str;
                    return true;
                }
                else if (v.type == var.DW)
                {
                    if (hex8forExec) //For Assemble Command (EXEC/ENDE) ie. "0DEADBEEF"
                        value = '0' + toupper(rul2hexstr(v.dw));
                    else
                        value = toupper(rul2hexstr(v.dw));
                    return true;
                }
            }
            else if (is_float(op))
            {
                value = op;
                return true;
            }
            else if (is_hex(op))
            {
                if (hex8forExec)
                    value = '0' + op;
                else
                    value = op;
                return true;
            }
            else if (is_dec(op))
            {
                value = toupper(rul2hexstr(decstr2rul(op.substr(0, op.Length - 1))));
                return true;
            }
            else if (is_string(op))
            {
                value = Helper.UnquoteString(op, '"');
                return true;
            }
            else if (is_bytestring(op))
            {
                value = op;
                return true;
            }
            else if (Helper.is_memory(op))
            {
                return GetString(op, value);
            }
            else if (GetRulong(op, dw))
            {
                if (hex8forExec)
                    value = '0' + toupper(rul2hexstr(dw));
                else
                    value = toupper(rul2hexstr(dw));
                return true;
            }
            return false;
        }

        bool GetString(string op, out string value, int size = 0)
        {
            if (is_variable(op))
            {
                if (variables[op].type == var.etype.STR)
                {
                    if (size != 0 && size < variables[op].size)
                    {
                        var tmp = variables[op];
                        tmp.resize(size);
                        value = tmp.str;
                    }
                    else
                    {
                        value = variables[op].str;
                    }
                    return true;
                    /*
                    // It's a string var, return value
                    if(size && size < v.size)
                        value = v.to_string().substr(0, size);
                    else
                        value = v.str;
                    return true;
                    */
                }
            }
            else if (is_string(op))
            {
                value = Helper.UnquoteString(op, '"');

                if (size && size < value.Length)
                    value.resize(size);
                return true;
            }
            else if (is_bytestring(op))
            {
                if (size && (size * 2) < (op.Length - 2))
                    value = op.substr(0, (size * 2) + 1) + '#';
                else
                    value = op;
                return true;
            }
            else if (Helper.is_memory(op))
            {
                string tmp = Helper.UnquoteString(op, '[', ']');

                rulong src;
                if (GetRulong(tmp, out src))
                {
                    Debug.Assert(src != 0);

                    value = "";
                    if (size != 0)
                    {
                        byte[] buffer;

                            buffer = new byte[size];

                        if (Host.TE_ReadMemory(src, (uint)size, buffer))
                        {
                            value = '#' + Helper.bytes2hexstr(buffer, size) + '#';
                            return true;
                        }
                        

                        /*
                        char* buffer;

                        try
                        {
                            buffer = new char[size+1];
                        }
                        catch(std::bad_alloc)
                        {
                            return false;	
                        }

                        if(Host.TE_ReadMemory(src, size, buffer))
                        {
                            buffer[size] = '\0';
                            value = buffer;
                            if(value.Length != size)
                            {
                                var v = value;
                                value = '#' + v.to_bytes() + '#';
                            }
                            delete[] buffer;
                            return true;
                        }
                        delete[] buffer;
                        */
                    }
                    else
                    {
                        byte[] buffer = new byte[STRING_READSIZE];
                        if (Host.TE_ReadMemory(src, sizeof(buffer), buffer))
                        {
                            buffer[_countof(buffer) - 1] = '\0';
                            value = buffer;
                            return true;
                        }
                    }
                }
            }
            else
            {
                string parsed;
                return (ParseString(op, parsed) && GetString(parsed, value, size));
            }
            return false;
        }
        /*
        bool GetStringLiteral(string  op, string &value)
        {
            if(is_variable(op))
            {
                const var& v = variables[op];
                if(v.type == var::STR && !v.isbuf)
                {
                    value = v.str;
                    return true;
                }
            }
            else if(is_string(op))
            {
                value = Helper.UnquoteString(op, '"');
                return true; 
            }
            else
            {
                string parsed;
                return (ParseString(op, parsed) && GetStringLiteral(parsed, value));
            }
            return false;
        }

        bool GetBytestring(string  op, string &value, int size)
        {
            if(is_variable(op))
            {
                const var& v = variables[op];
                if(v.type == var::STR && v.isbuf)
                {
                    if(size && size < v.size)
                    {
                        var tmp = v;
                        tmp.resize(size);
                        value = tmp.str;
                    }
                    else
                        value = v.str;
                    return true;
                }
            }
            else if(is_bytestring(op))
            {
                if(size && (size*2) < (op.Length-2))
                    value = op.substr(0, (size*2)+1) + '#';
                else
                    value = op;
                return true;
            }
            else if(Helper.is_memory(op))
            {
                string tmp = Helper.UnquoteString(op, '[', ']');

                rulong src;
                if(GetRulong(tmp, src))
                {
                    ASSERT(src != 0);

                    if(size)
                    {
                        byte* buffer;

                        try
                        {
                            buffer = new byte[size];
                        }
                        catch(std::bad_alloc)
                        {
                            return false;	
                        }

                        if(Host.TE_ReadMemory(src, size, buffer))
                        {
                            value = '#' + bytes2hexstr(buffer, size) + '#';
                            delete[] buffer;
                            return true;
                        }
                        delete[] buffer;
                    }
                }
            }
            else
            {
                string parsed;
                return (ParseString(op, parsed) && GetBytestring(parsed, value, size));
            }
            return false;
        }
        */

        bool GetBool(string op, out bool value)
        {
            rulong temp;

            if (GetRulong(op, temp))
            {
                value = temp != 0;
                return true;
            }
            return false;
        }

        bool GetRulong(string op, out rulong value)
        {
            if (is_register(op))
            {
                const register_t* reg = find_register(op);
                value = Debugger.GetContextData(reg.id);
                value = resize(value >> (reg.offset * 8), reg.size);
                return true;
            }
            else if (is_flag(op))
            {
                eflags_t flags;
                flags.dw = Debugger.GetContextData(UE_EFLAGS);
                switch (op[1])
                {
                case 'a': value =(int)flags.bits.AF; break;
                case 'c': value =(int)flags.bits.CF; break;
                case 'd': value =(int)flags.bits.DF; break;
                case 'o': value =(int)flags.bits.OF; break;
                case 'p': value =(int)flags.bits.PF; break;
                case 's': value =(int)flags.bits.SF; break;
                case 'z': value =(int)flags.bits.ZF; break;
                }
                return true;
            }
            else if (is_variable(op))
            {
                if (variables[op].type == var.etype.DW)
                {
                    value = variables[op].dw;
                    return true;
                }
            }
            else if (is_constant(op))
            {
                value = find_constant(op).value;
                return true;
            }
            else if (is_hex(op))
            {
                value = hexstr2rul(op);
                return true;
            }
            else if (is_dec(op))
            {
                value = decstr2rul(op.substr(0, op.Length - 1));
                return true;
            }
            else if (IsQuotedString(op, '[', ']'))
            {
                string tmp = Helper.UnquoteString(op, '[', ']');

                rulong src;
                if (GetRulong(tmp, src))
                {
                    ASSERT(src != 0);
                    return Host.TE_ReadMemory(src, sizeof(value), out value);
                }
            }
            else
            {
                string parsed;
                return (ParseRulong(op, ref parsed) && GetRulong(parsed, out value));
            }
            return false;
        }

        bool GetFloat(string op, out double value)
        {
            throw new NotImplementedException();
#if LATER
            if (Helper.is_float(op))
            {
                value =Helper.str2dbl(op);
                return true;
            }
            else if (is_floatreg(op))
            {
                int index = op[3] - '0';
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
            }
            else if (is_variable(op))
            {
                if (variables[op].type == var.etype.FLT)
                {
                    value = variables[op].flt;
                    return true;
                }
            }
            else if (Helper.is_memory(op))
            {
                string tmp = Helper.UnquoteString(op, '[', ']');

                rulong src;
                if (GetRulong(tmp, src))
                {
                    ASSERT(src != 0);
                    return Host.TE_ReadMemory(src, sizeof(value), &value);
                }
            }
            else
            {
                string parsed;
                return (ParseFloat(op, parsed) && GetFloat(parsed, value));
            }
            return false;
#endif
        }

        bool SetRulong(string op, rulong value, int size = 0)
        {
            throw new NotImplementedException();
#if LATER
            if (size > sizeof(value))
                size = sizeof(value);

            if (is_variable(op))
            {
                variables[op] = value;
                variables[op].resize(size);
                return true;
            }
            else if (is_register(op))
            {
                register_t reg = find_register(op);
                rulong tmp = resize(value, min(size, (int)reg.size));
                if (reg.size < sizeof(rulong))
                {
                    rulong oldval, newval;
                    oldval = Debugger.GetContextData(reg.id);
                    oldval &= ~(((1 << (reg.size * 8)) - 1) << (reg.offset * 8));
                    newval = resize(value, reg.size) << (reg.offset * 8);
                    tmp = oldval | newval;
                }
                return Debugger.SetContextData(reg.id, tmp);
            }
            else if (is_flag(op))
            {
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
            }
            else if (Helper.is_memory(op))
            {
                string tmp = Helper.UnquoteString(op, '[', ']');

                rulong target;
                if (GetRulong(tmp, out target))
                {
                    Debug.Assert(target != 0);
                    return Host.TE_WriteMemory(target, size, value);
                }
            }

            return false;
#endif
        }

        bool SetFloat(string op, double value)
        {
            if (is_variable(op))
            {
                variables[op] = value;
                return true;
            }
            else if (is_floatreg(op))
            {
                int index = op[3] - '0';
                double* preg;
#if _WIN64
			XMM_SAVE_AREA32 fltctx;
			preg = (double*)&fltctx.FloatRegisters + index;
#else
                FLOATING_SAVE_AREA fltctx;
                preg = (double*)&fltctx.RegisterArea[0] + index;
#endif
                if (Debugger.GetContextFPUDataEx(TE_GetCurrentThreadHandle(), &fltctx))
                {
                    *preg = value;
                    return Debugger.SetContextFPUDataEx(Host.TE_GetCurrentThreadHandle(), &fltctx);
                }
            }
            else if (Helper.is_memory(op))
            {
                string tmp = Helper.UnquoteString(op, '[', ']');

                rulong target;
                if (GetRulong(tmp, target))
                {
                    ASSERT(target != 0);

                    return TE_WriteMemory(target, sizeof(value), &value);
                }
            }

            return false;
        }

        bool SetString(string op, string value, int size = 0)
        {
            if (is_variable(op))
            {
                variables[op] = value;
                if (size && size < variables[op].size)
                    variables[op].resize(size);
                return true;
            }
            else if (Helper.is_memory(op))
            {
                string tmp = Helper.UnquoteString(op, '[', ']');

                rulong target;
                if (GetRulong(tmp, target))
                {
                    ASSERT(target != 0);
                    return Host.TE_WriteMemory(target, min(size, value.Count), &value);
                }
            }
            return false;
        }

        bool SetBool(string op, bool value)
        {
            return SetRulong(op, value, sizeof(value));
        }

        register_t find_register(string name)
        {
            string lower = Helper.tolower(name);

            for (int i = 0; i < registers.Length; i++)
            {
                if (registers[i].name == lower)
                    return registers[i];
            }
            return null;
        }

        constant_t find_constant(string name)
        {
            string lower = Helper.tolower(name);

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
            return Array.Find(fpu_registers, x => StringComparer.InvariantCultureIgnoreCase.Compare(x, s) == 0);
        }

        bool is_flag(string s)
        {
            return Array.Find(e_flags, x => StringComparer.InvariantCultureIgnoreCase.Compare(x, s) == 0);
        }

        bool is_variable(string s)
        {
            return (variables.ContainsKey(s));
        }

        bool is_constant(string s)
        {
            return (find_constant(s) != null);
        }

        bool is_valid_variable_name(string s)
        {
            return (s.Length && isalpha(s[0]) && !is_register(s) && !is_floatreg(s) && !is_constant(s));
        }

        bool is_writable(string s)
        {
            return (is_variable(s) || Helper.is_memory(s) || is_register(s) || is_flag(s) || is_floatreg(s));
        }

        string ResolveVarsForExec(string @in, bool hex8forExec)
        {
            string @out, varname;
            const string ti = trim(@in);
            bool in_var = false;

            for (int i = 0; i < ti.Length; i++)
            {
                if (ti[i] == '{')
                {
                    in_var = true;
                }
                else if (ti[i] == '}')
                {
                    in_var = false;
                    GetAnyValue(varname, out varname, hex8forExec);
                    @out += varname;
                    varname = "";
                }
                else
                {
                    if (in_var)
                        varname += ti[i];
                    else
                        @out += ti[i];
                }
            }
            return @out;
        }

        //Add zero char before dw values, ex: 0DEADBEEF (to be assembled) usefull if first char is letter
        string FormatAsmDwords(string asmLine)
        {
            // Create command and arguments
            string command, arg, args;
            string cSep = "";
            int pos;

            pos = asmLine.find_first_of(whitespaces);

            if (pos < 0)
                return asmLine; //no args

            command = asmLine.substr(0, pos) + ' ';
            args = asmLine.substr(pos + 1);

            while ((pos = args.find_first_of("+,[")) >= 0)
            {
                arg = trim(args.substr(0, pos));
            ForLastArg:
                if (cSep == "[")
                {
                    if (arg.Count && arg[arg.Count - 1] == ']')
                    {
                        if (is_hex(arg.substr(0, arg.Count - 1)) && isalpha(arg[0]))
                        {
                            arg.insert(0, 1, '0');
                        }
                    }
                }
                else
                {
                    if (Helper.is_hex(arg) && Char.IsLetter(arg[0]))
                    {
                        arg.insert(0, 1, '0');
                    }
                }

                command += cSep + arg;

                if (args != "")
                {
                    cSep = "" + args[pos];
                    args.erase(0, pos + 1);
                }
            }

            args = trim(args);
            if (args != "")
            {
                arg = args;
                args = "";
                goto ForLastArg;
            }

            return trim(command);
        }

        bool callCommand(Func<string[], bool> command, params string[] args)
        {
            return command(args);
        }

        void regBlockToFree(t_dbgmemblock block)
        {
            tMemBlocks.Add(block);
        }

        void regBlockToFree(rulong address, rulong size, bool autoclean)
        {
            t_dbgmemblock block = { 0 };

            block.address = address;
            block.size = size;
            block.autoclean = autoclean;
            block.script_pos = script_pos;
            block.restore_registers = false;

            regBlockToFree(block);
        }

        bool unregMemBlock(rulong address)
        {
            for (int i = 0; i < tMemBlocks.Count; i++)
            {
                if (tMemBlocks[i].address == address)
                {
                    tMemBlocks.erase(tMemBlocks.begin() + i);
                    return true;
                }
            }
            return false;
        }

        bool freeMemBlocks()
        {
            for (int i = 0; i < tMemBlocks.Count; i++)
            {
                if (tMemBlocks[i].autoclean)
                    Host.TE_FreeMemory(tMemBlocks[i].address, tMemBlocks[i].size);
            }
            tMemBlocks.Clear();
            return true;
        }

        bool SaveRegisters(bool stackToo)
        {
            for (int i = 0; i < _countof(registers); i++)
            {
                if (registers[i].size == sizeof(rulong))
                {
                    eContextData reg = registers[i].id;
                    if (stackToo || (reg != UE_ESP && reg != UE_RSP && reg != UE_EBP && reg != UE_RBP))
                    {
                        reg_backup.regs[i] = Debugger.GetContextData(reg);
                    }
                }
            }
            reg_backup.eflags = Debugger.GetContextData(UE_EFLAGS);

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

            for (int i = 0; i < _countof(registers); i++)
            {
                if (registers[i].size == sizeof(rulong))
                {
                    eContextData reg = registers[i].id;
                    if (stackToo || (reg != UE_ESP && reg != UE_RSP && reg != UE_EBP && reg != UE_RBP))
                    {
                        Debugger.SetContextData(reg, reg_backup.regs[i]);
                    }
                }
            }

            Debugger.SetContextData(UE_EFLAGS, reg_backup.eflags);

            return true;
        }

        bool AllocSwbpMem(uint tmpSizet)
        {
            /*
            if(!tmpSizet)
            {
                FreeBpMem();
            }
            else if(!softbp_t || tmpSizet > alloc_bp)
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
            saved_bp = 0;
            alloc_bp = 0;
        }

        void StepIntoCallback()
        {
            switch (Instance().stepcount)
            {
            default: // continue stepping, count > 0
                Instance().stepcount--;
            case -1: // endless stepping, only enter script command loop on BP/exception
                Debugger.StepInto(&StepIntoCallback);
                break;
            case 0: // stop stepping, enter script command loop
                Instance().StepChecked();
                break;
            }
        }

        void StepOverCallback()
        {
            switch (Instance().stepcount)
            {
            default:
                Instance().stepcount--;
            case -1:
                if (Instance().return_to_usercode)
                {
                    if (true/*is_this_user_code(EIP)*/)
                    {
                        Instance().return_to_usercode = false;
                        Instance().stepcount = 0;
                        Instance().StepChecked();
                        break;
                    }
                }
                else if (Instance().run_till_return)
                {
                    string cmd = DisassembleEx(Debugger.GetContextData(UE_CIP));
                    if (cmd.IndexOf("RETN") == 0)
                    {
                        Instance().run_till_return = false;
                        Instance().stepcount = 0;
                        Instance().StepChecked();
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
                    string cmd = DisassembleEx(Debugger.GetContextData(UE_CIP));
                    if (cmd.IndexOf("CALL") == 0 || cmd.IndexOf("REP") == 0)
                        Debugger.StepOver(&StepOverCallback);
                    else
                        Debugger.StepInto(&StepIntoCallback);
                    break;
                }
            case 0:
                Instance().StepChecked();
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

        bool StepCallback(uint pos, bool returns_value, var.etype return_type, out var result)
        {
            callback_t callback;
            callback.call = calls.Count;
            callback.returns_value = returns_value;
            callback.return_type = return_type;

            callbacks.Add(callback);

            calls.Add(script_pos + 1);
            script_pos_next = pos;

            bool ret = Step();
            if (ret!=0 && returns_value && result)
                result = callback_return;

            return ret;
        }

        void CHC_TRAMPOLINE(object ExceptionData, eCustomException ExceptionId)
        {
            var it = Instance().CustomHandlerLabels.IndexOf(ExceptionId);
            if (it != Instance().CustomHandlerLabels.end())
            {
                //variables["$TE_ARG_1"] = (rulong)ExceptionData;
                Instance().DoCALL(&it.second, 1);
            }
        }

        object Callback_AutoFixIATEx(object fIATPointer)
        {
            var ret;

            uint label = Instance().script.labels[Instance().Label_AutoFixIATEx];
            Instance().variables["$TE_ARG_1"] = (rulong)fIATPointer;
            if (Instance().StepCallback(label, true, var.etype.DW, &ret))
                return (void*)ret.dw;
            else
                return 0;
        }

        void LBPC_TRAMPOLINE(LOAD_DLL_DEBUG_INFO SpecialDBG, eLibraryEvent bpxType)
        {
            Librarian.LIBRARY_ITEM_DATA Lib = Librarian.GetLibraryInfoEx(SpecialDBG.lpBaseOfDll);
            if (Lib != null)
            {
                Dictionary<string, string> labels = LibraryBreakpointLabels[bpxType];
                string it;
                if (labels.TryGetValue(Lib.szLibraryPath, out it))
                {
                    Instance().DoCALL(new[] { it });
                }
            }
        }
    }
}