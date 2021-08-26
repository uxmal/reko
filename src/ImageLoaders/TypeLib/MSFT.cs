// ****************************************
// 	"MSFT.bas"

// This code is based on code from the ReactOS
// project at: http://www.reactos.org/en/index.html
// The primary source file used was:
// 	...reactos\dll\win32\oleaut32\typelib.c

// So far, it only disassembles data with the "MSFT"
// magic value (not the "SLTG" type yet).

// It is intended to be integrated into bep (Bin
// Edit Plus) and perhaps other sub-projects

// This should have a TypeLib "diff" feature
// to help learn more about its format.

// This file is GPL 2008, by TheirCorp
// ****************************************

using Reko.Core.Memory;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Reko.ImageLoaders.TypeLib
{
    public class MSFT
    {
        private DecompilerEventListener listener;

        public MSFT(DecompilerEventListener listener)
        {
            this.listener = listener;
        }

        // ****************************************
        // 	"Study-Mode" Code
        // ****************************************
#if Study

$StudyMark		= "@@@: " // to help find items of interest
Global CurFile		As String

// you can set a string// s priority by prefixing
// it with a digit from 0 through 9. The minimum
// priority is determined by the value of %Priority
Sub Note(ps As String)
Long n;
Static ff	As Long
Static ct	As Long

	If Len(ps) = 0 Then
		// close it each time to make sure it// s accessible to a text viewer
		Close# ff
		ff = 0 : ct = 0
	Else

		If ff = 0 Then
			ff = FreeFile
			Open "Study.txt" For Append Lock Write As #ff
			Print# ff,
			Print# ff, "****************************************"
			Print# ff, "	New File: "; CurFile
		End If

		n = Val(Left$(ps, 1)) // get priority level
		ps = LTrim$(ps, Any " 0123456789") // remove priority digits

		If ct = 0 Then // alert the user, if the priority is high enough
			If n >= %Priority Then
				// MsgBox "Found something of interest" & $CrLf & ps
				Incr ct // disable any further alerts
			End If
		End If

		// print marker in output file to help find items of interest
		// (this assumes that the last file opened was the output file)
		fo.WriteLine();
		fo.WriteLine( $StudyMark; ps);

		// maintain a log of findings
		Print# ff, ps

	End If

End Sub // Note

#endif




        // ********************************************************************************
        // 					TypeLib-Specific Equates and Data Types
        // ********************************************************************************

        // Note:	OAIDL.h says hRefType is a DWORD

        public const int HELPDLLFLAG = 0x0100;


        // ****************************************
        // 	"Magic" Values
        // ****************************************
        public const int MSFT_SIGNATURE = 0x05446534D; //   "MSFT"
        public const int SLTG_SIGNATURE = 0x047544C53; //   "SLTG"


        // ****************************************
        // 	Equates for Translating Flags
        // 		and Codes to Text
        // ****************************************
        public enum SysKind { Win16, Win32, Macintosh }
        public enum VarFlags { ReadOnly, Source, Bindable, RequestEdit, DisplayBind, DefaultBind, Hidden, Restricted, DefaultCollelem, UiDefault, NonBrowsable, Replaceable, ImmediateBind }
        public enum TKind { Enum, Record, Module, Interface, Dispatch, Coclass, Alias, Union, Max }
        public enum TypeFlags { AppObject, CanCreate, Licensed, PredeclId, Hidden, Control, Dual, Nonextensible, Oleautomation, Restricted, Aggregatable, Replaceable, Dispatchable, ReverseBind }
        public enum ParamFlags { In, Out, LCID, RetVal, Opt, HasDefault, HasCustData }
        public enum CallConv { FastCall, CDecl, Pascal, MacPascal, StdCall, FPFastCall, SysCall, MPWCDecl, MPWPascal, Max }
        public enum InvoKind { Func, PropertyGet, PropertyPut, PropertyPutRef }
        public enum FuncKind { Virtual, PureVirtual, NonVirtual, Static, Dispatch }


        // ****************************************
        // 	Variable-Type Codes, Masks and Flags
        // ****************************************

        public const int VT_Empty = 0;
        public const int VT_Null = 1;
        public const int VT_I2 = 2;
        public const int VT_I4 = 3;
        public const int VT_R4 = 4;
        public const int VT_R8 = 5;
        public const int VT_Cy = 6;
        public const int VT_Date = 7;
        public const int VT_BStr = 8;
        public const int VT_Dispatch = 9;
        public const int VT_Error = 10;
        public const int VT_Bool = 11;
        public const int VT_Variant = 12;
        public const int VT_Unknown = 13;
        public const int VT_Decimal = 14;
        public const int VT_I1 = 16;
        public const int VT_UI1 = 17;
        public const int VT_UI2 = 18;
        public const int VT_UI4 = 19;
        public const int VT_I8 = 20;
        public const int VT_UI8 = 21;
        public const int VT_Int = 22;
        public const int VT_UInt = 23;
        public const int VT_Void = 24;
        public const int VT_HResult = 25;
        public const int VT_Ptr = 26;
        public const int VT_SafeArray = 27;
        public const int VT_CArray = 28;
        public const int VT_UserDefined = 29;
        public const int VT_LPStr = 30;
        public const int VT_LPWStr = 31;
        public const int VT_Record = 36;
        public const int VT_FileTime = 64;
        public const int VT_Blob = 65;
        public const int VT_Stream = 66;
        public const int VT_Storage = 67;
        public const int VT_Streamed_Object = 68;
        public const int VT_Stored_Object = 69;
        public const int VT_Blob_Object = 70;
        public const int VT_CF = 71;
        public const int VT_ClsID = 72;

        // 	flags
        public const int VT_Bstr_Blob = 0x00FFF;
        public const int VT_Vector = 0x01000;
        public const int VT_Array = 0x02000;
        public const int VT_ByRef = 0x04000;
        public const int VT_Reserved = 0x08000;

        // 	masks
        public const int VT_Illegal = 0x0FFFF;
        public const int VT_IllegalMasked = 0x00FFF;
        public const int VT_TypeMask = 0x00FFF;


        // ****************************************
        // 	Calling Conventions
        // ****************************************
        public const int CC_FASTCALL = 0;
        public const int CC_CDECL = 1;
        public const int CC_MSCPASCAL = 2;
        public const int CC_PASCAL = 2;
        public const int CC_MACPASCAL = 3;
        public const int CC_STDCALL = 4;
        public const int CC_FPFASTCALL = 5;
        public const int CC_SYSCALL = 6;
        public const int CC_MPWCDECL = 7;
        public const int CC_MPWPASCAL = 8;
        public const int CC_MAX = 9;


        // ****************************************
        // 	Function Types
        // ****************************************
        public const int FUNC_VIRTUAL = 0;
        public const int FUNC_PUREVIRTUAL = 1;
        public const int FUNC_NONVIRTUAL = 2;
        public const int FUNC_STATIC = 3;
        public const int FUNC_DISPATCH = 4;


        // ****************************************
        // 	Function Flags
        // ****************************************
        public const int FUNCFLAG_FRESTRICTED = 0x00001;
        public const int FUNCFLAG_FSOURCE = 0x00002;
        public const int FUNCFLAG_FBINDABLE = 0x00004;
        public const int FUNCFLAG_FREQUESTEDIT = 0x00008;
        public const int FUNCFLAG_FDISPLAYBIND = 0x00010;
        public const int FUNCFLAG_FDEFAULTBIND = 0x00020;
        public const int FUNCFLAG_FHIDDEN = 0x00040;
        public const int FUNCFLAG_FUSESGETLASTERROR = 0x00080;
        public const int FUNCFLAG_FDEFAULTCOLLELEM = 0x00100;
        public const int FUNCFLAG_FUIDEFAULT = 0x00200;
        public const int FUNCFLAG_FNONBROWSABLE = 0x00400;
        public const int FUNCFLAG_FREPLACEABLE = 0x00800;
        public const int FUNCFLAG_FIMMEDIATEBIND = 0x01000;
#if MAC
public const int FUNCFLAG_FORCELONG			= 2147483647;
#endif


        // ****************************************
        // 	Invocation Kinds
        // **************************************** 
        public const int INVOKE_FUNC = 1;
        public const int INVOKE_PROPERTYGET = 2;
        public const int INVOKE_PROPERTYPUT = 4;
        public const int INVOKE_PROPERTYPUTREF = 8;


        // ****************************************
        // 	Parameter Flags
        // ****************************************
        public const int PARAMFLAG_NONE = 0x000;
        public const int PARAMFLAG_FIN = 0x001;
        public const int PARAMFLAG_FOUT = 0x002;
        public const int PARAMFLAG_FLCID = 0x004;
        public const int PARAMFLAG_FRETVAL = 0x008;
        public const int PARAMFLAG_FOPT = 0x010;
        public const int PARAMFLAG_FHASDEFAULT = 0x020;
        public const int PARAMFLAG_FHASCUSTDATA = 0x040;


        // ****************************************
        // 	System Kind
        // ****************************************
        public const int SYS_WIN16 = 0;
        public const int SYS_WIN32 = 1;
        public const int SYS_MAC = 2;

        // SYS_WIN16 --- The target operating system for the type library is 16-bit Windows systems.
        // By default, data members are packed.

        // SYS_WIN32 --- The target operating system for the type library is 32-bit Windows systems.
        // By default, data members are naturally aligned (for example, 2-byte integers are aligned
        // on even-byte boundaries; 4-byte integers are aligned on quad-word boundaries, and so on).

        // SYS_MAC --- The target operating system for the type library is Apple Macintosh. By default,
        // all data members are aligned on even-byte boundaries.


        // ****************************************
        // 	Type-Kinds
        // ****************************************
        public const int TKIND_ENUM = 0;
        public const int TKIND_RECORD = 1;
        public const int TKIND_MODULE = 2;
        public const int TKIND_INTERFACE = 3;
        public const int TKIND_DISPATCH = 4;
        public const int TKIND_COCLASS = 5;
        public const int TKIND_ALIAS = 6;
        public const int TKIND_UNION = 7;
        public const int TKIND_MAX = 8;


        // ****************************************
        // 	Type Flags
        // ****************************************
        public const int TYPEFLAG_FAPPOBJECT = 0x00001;
        public const int TYPEFLAG_FCANCREATE = 0x00002;
        public const int TYPEFLAG_FLICENSED = 0x00004;
        public const int TYPEFLAG_FPREDECLID = 0x00008;
        public const int TYPEFLAG_FHIDDEN = 0x00010;
        public const int TYPEFLAG_FCONTROL = 0x00020;
        public const int TYPEFLAG_FDUAL = 0x00040;
        public const int TYPEFLAG_FNONEXTENSIBLE = 0x00080;
        public const int TYPEFLAG_FOLEAUTOMATION = 0x00100;
        public const int TYPEFLAG_FRESTRICTED = 0x00200;
        public const int TYPEFLAG_FAGGREGATABLE = 0x00400;
        public const int TYPEFLAG_FREPLACEABLE = 0x00800;
        public const int TYPEFLAG_FDISPATCHABLE = 0x01000;
        public const int TYPEFLAG_FREVERSEBIND = 0x02000;
        public const int TYPEFLAG_MASK = TYPEFLAG_FREVERSEBIND - 1;

        // ****************************************
        // 	Variable Kinds
        // ****************************************
        // not sure if these are ever used in MSFT format data
        // %VAR_PERINSTANCE	= 0
        // %VAR_STATIC		= %VAR_PERINSTANCE + 1
        // %VAR_CONST			= %VAR_STATIC + 1
        // %VAR_DISPATCH		= %VAR_CONST + 1
        public const int VAR_PERINSTANCE = 0;
        public const int VAR_STATIC = 1;
        public const int VAR_CONST = 2;
        public const int VAR_DISPATCH = 3;


        // ****************************************
        // 	Variable Flags
        // ****************************************
        public const int VARFLAG_FREADONLY = 0x00001;
        public const int VARFLAG_FSOURCE = 0x00002;
        public const int VARFLAG_FBINDABLE = 0x00004;
        public const int VARFLAG_FREQUESTEDIT = 0x00008;
        public const int VARFLAG_FDISPLAYBIND = 0x00010;
        public const int VARFLAG_FDEFAULTBIND = 0x00020;
        public const int VARFLAG_FHIDDEN = 0x00040;
        public const int VARFLAG_FRESTRICTED = 0x00080;
        public const int VARFLAG_FDEFAULTCOLLELEM = 0x00100;
        public const int VARFLAG_FUIDEFAULT = 0x00200;
        public const int VARFLAG_FNONBROWSABLE = 0x00400;
        public const int VARFLAG_FREPLACEABLE = 0x00800;
        public const int VARFLAG_FIMMEDIATEBIND = 0x01000;




        // ****************************************
        // 	TypeLib UDTs
        // ****************************************


        public struct TYPEDESCUNION
        {
            public uint lptdesc; // TYPEDESC Ptr
            public uint lpadesc; // ARRAYDESC Ptr
            public int hRefType; // hRefType
        } // TYPEDESCUNION


        public unsafe struct tagTYPEDESC
        {
            public TYPEDESCUNION u;
            public int vt; // VARTYPE
        } // TYPEDESC


        public struct tagPARAMDESCEX
        {
            public uint cBytes; // ULONG
                                // varDefaultValue		As Variant // VARIANTARG
        } // tagPARAMDESCEX


        public class tagPARAMDESC
        {
            public tagPARAMDESCEX pParamDescEx; // or a Ptr ?
            public ushort fParam; // (USHORT)
        } // tagPARAMDESC


        public class tagIDLDESC
        {
            public uint Res; // Reserved
            public ushort fIDL; // USHORT
        } //End Type tagIDLDESC


        public struct ELEMDESCUNION
        {
            public tagIDLDESC idldesc;  // info for remoting the element
            public tagPARAMDESC ParamDesc;  // info about the parameter
        }


        public class tagELEMDESC
        {
            public tagTYPEDESC tdesc; // the type of the element
            public ELEMDESCUNION u;
        } // tagELEMDESC



        // 	MSFT typelibs
        // These are TypeLibs created with ICreateTypeLib2 structure of the typelib type2 header
        // it is at the beginning of a type lib file
        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public unsafe struct TlbHeader
        {
            public uint Magic1; // 0x5446534D "MSFT"								00
            public uint Magic2; // 0x00010002 version number?
            public uint oGUID; // position of libid in GUID table (should be,  else -1)
            public uint LCID; // locale id
            public uint LCID2; // 												10
            public uint fVar; // (largely) unknown flags
                              // * the lower nibble is SysKind
                              // * bit 5 is set if a helpfile is defined
                              // * bit 8 is set if a help dll is defined

            public uint Version;         // set with SetVersion()
            public uint Flags;       // set with SetFlags()
            public int nTypeInfo;       // number of TypeInfo's							20
            public uint HelpStr;         // offset to help string in string table
            public uint HelpStrCnt;
            public uint HelpCntxt;
            public uint nName;       // number of names in name table					30
            public uint nChars;      // characters in name table
            public uint oName;       // offset of name in string table
            public uint HelpFile;        // offset of helpfile in string table
            public uint CustDat;         // if -1 no custom data, else it is offset		40
                                         // in custom data/GUID offset table
            public uint Res1;        // unknown always: 0x20 (GUID hash size?)
            public uint Res2;        // unknown always: 0x80 (name hash size?)
            public uint oDispatch;       // hRefType to IDispatch, or -1 if no IDispatch
            public uint nImpInfos;       // number of ImpInfos								50
                                         // oFileName			As Long // offset to typelib file name in string table
        } // TlbHeader


        // segments in the type lib file have a structure like this:
        public struct SegDesc
        {
            public int Offs; // absolute offset in file
            public int nLen; // length of segment
            public int Res01; // unknown always -1
            public int Res02; // unknown always 0x0F in the header
                               // 0x03 in the TypeInfo_data
        }

        // 	segment directory
        public struct MSFT_SegDir
        {
            public SegDesc pTypInfo;    // 1 - TypeInfo table
            public SegDesc pImpInfo;    // 2 - table with info for imported types
            public SegDesc pImpFiles;   // 3 - import libaries
            public SegDesc pRefer;      // 4 - References table
            public SegDesc pLibs;       // 5 - always exists, alway same size (0x80)
                                        //   - hash table with offsets to GUID;;?
            public SegDesc pGUID;       // 6 - all GUIDs are stored here together with
                                        //   - offset in some table;;
            public SegDesc Unk01;       // 7 - always created, always same size (0x0200)
                                        //   - contains offsets into the name table
            public SegDesc pNames;      // 8 - name table
            public SegDesc pStrings;    // 9 - string table
            public SegDesc pTypDesc;    // A - type description table
            public SegDesc pArryDesc;   // B - array descriptions
            public SegDesc pCustData;   // C - data table, used for custom data and default
                                        //   - parameter values
            public SegDesc pCDGuids;    // D - table with offsets for the GUIDs and into
                                        //   - the custom data table
            public SegDesc Unk02;       // E - unknown
            public SegDesc Unk03;       // F - unknown
        } // MSFT_SegDir


        // type info data
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public unsafe struct TypeInfo {
            public Byte TypeKind;       // TKIND_xxx
            public Byte Align;      // alignment
            public short Unk;       // unknown
            public int oFunRec;     // 	- points past the file, if no elements
            public int Alloc;       // 	Recommended (or required?) amount of memory to allocate for...?
            public int Reconst;     // 	size of reconstituted TypeInfo data
            public int Res01;       // 10 - always? 3
            public int Res02;       // 	- always? zero
            public short nFuncs;         //  - count of functions
            public short nProps;        //  - count of properties
            public int Res03;       //    - always? zero
            public int Res04;       // 20 - always? zero
            public int Res05;       //    - always? zero
            public int Res06;       //    - always? zero
            public int oGUID;       //    - position in GUID table
            public int fType;       // 30 - Typeflags
            public int oName;       //    - offset in name table
            public int Version;     //    - element version
            public int DocStr;      //    - offset of docstring in string tab
            public int HelpStrCnt;      // 40
            public int HelpCntxt;
            public int oCustData;       //    - offset in custom data table
#if WORDS_BIGENDIAN        // 
	public short cVft; // virtual table size, not including inherits
	public short nImplTypes; // number of implemented interfaces
#else
            public short nImplTypes;        // number of implemented interfaces
            public short cVft;              // virtual table size, not including inherits
#endif
            public int Unk03; // 50 - size in bytes, at least for structures

            public int Type1;  // 	- position in type description table
                               // 	- or in base interfaces
                               // 	- if coclass: offset in reftable
                               // 	- if interface: reference to inherited if
                               // 	- if module: offset to dllname in name table
            public int Type2;  // 	- if 0x8000, entry above is valid, else it is zero?
            public int Res07;  // 	- always? 0
            public int Res08;  // 60 - always? -1
        } // TypeInfo



        // information on imported types
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public unsafe struct ImportInfo
        {
            public ushort Count; // count
            public Byte Flags; // if <> 0 then oGUID is an offset to GUID, else it// s a TypeInfo index in the specified typelib
            public Byte TypeKind;   //  TKIND of reference
            public uint oImpFile; // offset in the Import File table
            public uint oGuid; // offset in GUID table or TypeInfo index (see bit 16 of Res0)
        } // ImportInfo



        // information on imported files
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct TlbImpLib
        {
            public int oGUID;
            public int LCID;
            public ushort MajVer;
            public ushort MinVer;
            public ushort cSize; // divide by 4 to get the length of the file name
        } // TlbImpLib



        // Structure of the reference data
        public struct RefRecord
        {
            // either offset in TypeInfo table, then it// s a multiple of 4...
            // or offset in the external reference table with an offset of 1
            public int RefType;
            public int Flags;                // ?
            public int oCustData; // custom data
            public int oNext; // next offset, -1 if last
        } // RefRecord



        // this is how a GUID is stored
        public unsafe struct GuidEntry
        {
            public Guid oGUID;
            //  = -2 for a TypeLib GUID
            // TypeInfo offset for TypeInfo GUID,
            // Otherwise, the low two bits:
            // 	= 01 for an imported TypeInfo
            // 	= 10 for an imported TypeLib (used by imported TypeInfos)
            public int hRefType;
            public int NextHash;    // offset to next GUID in the hash bucket
        }// GuidEntry



        // some data preceding entries in the name table
        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct NameIntro {
            // is -1 if name is for neither a TypeInfo,
            // a variable, or a function (that is, name
            // is for a typelib or a function parameter).
            // otherwise is the offset of the first
            // TypeInfo that this name refers to (either
            // to the TypeInfo itself or to a member of
            // the TypeInfo
            public int hRefType;

            public int NextHash;    // offset to next name in the hash bucket

            // only lower 8 bits are valid,
            // lower-middle 8 Bits are unknown (flags?),
            // upper 16 Bits are hash code
            public int cName;
        } // NameIntro


        // this is only here to illustrate the storage format for strings
        // Type TlbString
        // 	nLen	As Word // length of string
        // 	zStr	As String * nLen // text of string
        // 	zPad	As String$(?, "W") // pad to Dword alignment
        // End Type TlbString



        public unsafe struct TYPEDESC // a substitute for a tagTYPEDESC to simplify the code
        {
            public short v1;
            public short v2;
            public short v3;
            public short v4;
        } // TYPEDESC



        // type for arrays
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public unsafe struct SafeArrayBound
        {
            public uint nElements;
            public int lLBound;
        }// SafeArrayBound

        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public unsafe struct ARRAYDESC {
            public TYPEDESCUNION u;
            public ushort nDims;
            public ushort tVar; // VARTYPE
            public SafeArrayBound Bounds;   // array.
        } // ARRAYDESC

        public unsafe struct CArrayDesc {
            public ARRAYDESC Desc;
            public SafeArrayBound Bnd;
        }



        // Custom data table entries are Dword aligned by padding with (usually) "W"
        // Type CustomData
        // 	nLen				As Word // length of nLen plus bData in Words
        // 	cData(nLen - 1)		As Word	// 
        // End Type // CustomData



        // the custom data/GUID table directory has entries like this
        public class CDGuid
        {
            public int oGUID;
            public int oData;
            public int oNext; // next offset in the table, -1 if it// s the last
        } // CDGuid



        // 	Function description data
        // SizeOf(FuncRecord) = 24 (required fields only)
        // These exist in arrays along with zero or more "PropRecord"
        // elements. Each array is preceded by a Dword stating the total
        // size of the array.
        // ...ArraySize		As Long
        public struct FuncRecord
        {
            public ushort RecSize;      // record size, including optional fields and ParamInfo// s
            public ushort Unk1;         //  zero-based function number ?
            public ushort DataType;          // data type returned by the function

            // If the .Flags MSB = 1, then the low byte is valid. So far it seems
            // to always be valid, except for pointers. When MSB = 1, the low byte
            // is the code for a data type that's equivalent to or compatible with
            // that in .DataType.
            public short Flags;
            public int Res1; // always(?) zero

#if WORDS_BIGENDIAN
	public short cFuncDesc; // size of reconstituted FUNCDESC and related structs
	public short oVtable; // offset in vtable
#else
            public short oVtable;               // offset in vtable
            public short cFuncDesc;// size of reconstituted FUNCDESC and related structs
#endif

            // The bits in FKCCIC have the following meanings:
            // 0 - 2 = function kind (eg virtual)
            // 3 - 6 = Invocation kind
            // 7 means custom data is present
            // 8 - 11 = calling convention
            // 12 means one or more parameters have a default value
            // +---+---+---+---+---+---+---+---+---+---+---+---+---+---+---+---+
            // |               |               |               |               |
            // | 15  14  13  12| 11  10  9   8 | 7   6   5   4 | 3   2   1   0 |
            // +---+---+---+---+---+---+---+---+---+---+---+---+---+---+---+---+
            // |   |   |   | D |   |   |   |   | C |   |   |   |   |   |   |   |
            // |   |   |   | e |    Calling    | u |  Invocation   | Function  |
            // |   |   |   | f |  Convention   | s |    Kind       |   Kind    |
            // |   |   |   | V |               | t |               |           |
            // |   |   |   | a |               | D |               |           |
            // |   |   |   | l |               | a |               |           |
            // |   |   |   | u |               | t |               |           |
            // |   |   |   | e |   |   |   |   | a |   |   |   |   |   |   |   |
            // +---+---+---+---+---+---+---+---+---+---+---+---+---+---+---+---+
            public uint FKCCIC;

            public short nParams; // parameter count
            public short Unk2;

            // ****** Optional attribute fields, the number of them is variable ******
            // HelpCntxt			As Long // 0
            // oHelpStr			As Long // 1
            // oEntry				As Long // 2 either offset in string table or numeric as it is
            // res2				As Long // 3 unknown (-1)
            // res3				As Long // 4 unknown (-1)
            // HelpStrCnt			As Long // 5
            // ****** these are present if bit 12 of FKCCIC is set ******
            // oCustData			As Long // 6 custom data for function
            // ArgCustData(0)		As Long // 7 custom data per argument
        }// FuncRecord



        // Parameter info: one per argument
        public struct ParamInfo
        {
            public ushort DataType;
            public ushort Flags;
            public uint oName;
            public uint fParam;
        } // ParamInfo



        // 	Property description data
        //  The size of the required fields of the PropRecord structure
        // is 20 (= 0x14)
        // These exist in arrays along with zero or more "FuncRecord"
        // elements. Each array is preceded by a Dword stating the total
        // size of the array.
        public struct PropRecord
        {
            public ushort RecSize; // size of PropRecord
            public ushort PropNum; // Property number?
            public short DataType; // data type of the variable
            public short Flags; // VarFlags
#if WORDS_BIGENDIAN
	public short cVarDesc; // size of reconstituted VARDESC and related structs
	public short VarKind; // VarKind
#else
            public short VarKind; // VarKind --- %VAR and %VarFlags
            public short cVarDesc; // size of reconstituted VARDESC and related structs
#endif
            public int OffsValue; // value of the variable or the offset in the data structure
                                  // ***** End of required fields *****

            // Optional attribute fields, the number of them is variable
            // and are determined from the record size (if there// s room
            // for it, then it// s there...)
            public int Unk;
            public int HelpCntxt;
            public int oHelpStr;
            public int Res; // unknown (-1)
            public int oCustData; // custom data for variable
            public int HelpStrCnt;
        }// PropRecord



        // ****************************************

        // Declare Function Locale(lcid As Long) As String
        // Declare Function VarType(ByVal pn As Long) As String
        // Declare Function tlName(cs As String, SegDir As MSFT_SegDir, ByVal offs As Long) As String
        // Declare Function tlString(cs As String, SegDir As MSFT_SegDir, ByVal offs As Long) As String
        // Declare Function DisSltg(cs As String) As Dword
        // Declare Function DisFunction(cs As String, SegDir As MSFT_SegDir, ByVal pBase As Dword, ByVal nFunc As Long, ByVal nProp As Long) As Long
        // Declare Function DisTypeLib(cs As String, ps As String) As Dword

        // ****************************************

        public string Locale(int lcid) {

            var locales = new[] {
                new { Lcid=0x0401, Code="ARA", Desc="Arabic Saudi Arabia" },
                new { Lcid=0x0801, Code="ARI", Desc="Arabic Iraq" },
                new { Lcid=0x0C01, Code="ARE", Desc="Arabic Egypt" },
                new { Lcid=0x1001, Code="ARL", Desc="Arabic Libya" },
                new { Lcid=0x1401, Code="ARG", Desc="Arabic Algeria" },
                new { Lcid=0x1801, Code="ARM", Desc="Arabic Morocco" },
                new { Lcid=0x1C01, Code="ART", Desc="Arabic Tunisia" },
                new { Lcid=0x2001, Code="ARO", Desc="Arabic Oman" },
                new { Lcid=0x2401, Code="ARY", Desc="Arabic Yemen" },
                new { Lcid=0x2801, Code="ARS", Desc="Arabic Syria" },
                new { Lcid=0x2C01, Code="ARJ", Desc="Arabic Jordan" },
                new { Lcid=0x3001, Code="ARB", Desc="Arabic Lebanon" },
                new { Lcid=0x3401, Code="ARK", Desc="Arabic Kuwait" },
                new { Lcid=0x3801, Code="ARU", Desc="Arabic U.A.E." },
                new { Lcid=0x3C01, Code="ARH", Desc="Arabic Bahrain" },
                new { Lcid=0x4001, Code="ARQ", Desc="Arabic Qatar" },
                new { Lcid=0x0402, Code= "BGR", Desc="Bulgarian Bulgaria" },
                new { Lcid=0x0403, Code="CAT", Desc="Catalan Spain" },
                new { Lcid=0x0404, Code="CHT", Desc="Chinese Taiwan" },
                new { Lcid=0x0804, Code="CHS", Desc="Chinese PRC" },
                new { Lcid=0x0C04, Code="ZHH", Desc="Chinese Hong Kong" },
                new { Lcid=0x1004, Code="ZHI", Desc="Chinese Singapore" },
                new { Lcid=0x1404, Code="ZHM", Desc="Chinese Macau" },
                new { Lcid=0x0405, Code="CSY", Desc="Czech Czech Republic" },
                new { Lcid=0x0406, Code="DAN", Desc="Danish Denmark" },
                new { Lcid=0x0407, Code="GERMANY", Desc="German Germany" },
                new { Lcid=0x0807, Code="DES", Desc="German Switzerland" },
                new { Lcid=0x0C07, Code="DEA", Desc="German Austria" },
                new { Lcid=0x1007, Code="DEL", Desc="German Luxembourg" },
                new { Lcid=0x1407, Code="DEC", Desc="German Liechtenstein" },
                new { Lcid=0x0408, Code="ELL", Desc="Greek Greece" },
                new { Lcid=0x0409, Code="USA", Desc="English United States" },
                new { Lcid=0x0809, Code="ENG", Desc="English United Kingdom" },
                new { Lcid=0x0C09, Code="ENA", Desc="English Australia" },
                new { Lcid=0x1009, Code="ENC", Desc="English Canada" },
                new { Lcid=0x1409, Code="ENZ", Desc="English New Zealand" },
                new { Lcid=0x1809, Code="ENI", Desc="English Ireland" },
                new { Lcid=0x1C09, Code="ENS", Desc="English South Africa" },
                new { Lcid=0x2009, Code="ENJ", Desc="English Jamaica" },
                new { Lcid=0x2409, Code="ENB", Desc="English Caribbean" },
                new { Lcid=0x2809, Code="ENL", Desc="English Belize" },
                new { Lcid=0x2C09, Code="ENT", Desc="English Trinidad" },
                new { Lcid=0x3009, Code="ENW", Desc="English Zimbabwe" },
                new { Lcid=0x3409, Code="ENP", Desc="English Philippines" },
                new { Lcid=0x040A, Code="SPAIN", Desc="Spanish Spain" },
                new { Lcid=0x080A, Code="ESM", Desc="Spanish Mexico" },
                new { Lcid=0x0C0A, Code="ESN", Desc="Spanish Spain (International Sort)" },
                new { Lcid=0x100A, Code="ESG", Desc="Spanish Guatemala" },
                new { Lcid=0x140A, Code="ESC", Desc="Spanish Costa Rica" },
                new { Lcid=0x180A, Code="ESA", Desc="Spanish Panama" },
                new { Lcid=0x1C0A, Code="ESD", Desc="Spanish Dominican Republic" },
                new { Lcid=0x200A, Code="ESV", Desc="Spanish Venezuela" },
                new { Lcid=0x240A, Code="ESO", Desc="Spanish Colombia" },
                new { Lcid=0x280A, Code="ESR", Desc="Spanish Peru" },
                new { Lcid=0x2C0A, Code="ESS", Desc="Spanish Argentina" },
                new { Lcid=0x300A, Code="ESF", Desc="Spanish Ecuador" },
                new { Lcid=0x340A, Code="ESL", Desc="Spanish Chile" },
                new { Lcid=0x380A, Code="ESY", Desc="Spanish Uruguay" },
                new { Lcid=0x3C0A, Code="ESZ", Desc="Spanish Paraguay" },
                new { Lcid=0x400A, Code="ESB", Desc="Spanish Bolivia" },
                new { Lcid=0x440A, Code="ESE", Desc="Spanish El Salvador" },
                new { Lcid=0x480A, Code="ESH", Desc="Spanish Honduras" },
                new { Lcid=0x4C0A, Code="ESI", Desc="Spanish Nicaragua" },
                new { Lcid=0x500A, Code="ESU", Desc="Spanish Puerto Rico" },
                new { Lcid=0x040B, Code="FIN", Desc="Finnish Finland" },
                new { Lcid=0x040C, Code="FRANCE", Desc="French France" },
                new { Lcid=0x080C, Code="FRB", Desc="French Belgium" },
                new { Lcid=0x0C0C, Code="FRC", Desc="French Canada" },
                new { Lcid=0x100C, Code="FRS", Desc="French Switzerland" },
                new { Lcid=0x140C, Code="FRL", Desc="French Luxembourg" },
                new { Lcid=0x180C, Code="FRM", Desc="French Monaco" },
                new { Lcid=0x040D, Code="HEB", Desc="Hebrew Israel" },
                new { Lcid=0x040E, Code="HUN", Desc="Hungarian Hungary" },
                new { Lcid=0x040F, Code="ISL", Desc="Icelandic Iceland" },
                new { Lcid=0x0410, Code="ITALY", Desc="Italian Italy" },
                new { Lcid=0x0810, Code="ITS", Desc="Italian Switzerland" },
                new { Lcid=0x0411, Code="JAPAN", Desc="Japanese Japan" },
                new { Lcid=0x0412, Code="KOREA", Desc="Korean Korea" },
                new { Lcid=0x0413, Code="NLD", Desc="Dutch Netherlands" },
                new { Lcid=0x0813, Code="NLB", Desc="Dutch Belgium" },
                new { Lcid=0x0414, Code="NOR", Desc="Norwegian Norway (Bokmål)" },
                new { Lcid=0x0814, Code="NON", Desc="Norwegian Norway (Nynorsk)" },
                new { Lcid=0x0415, Code="PLK", Desc="Polish Poland" },
                new { Lcid=0x0416, Code="BRAZIL", Desc="Portuguese Brazil" },
                new { Lcid=0x0816, Code="PTG", Desc="Portuguese Portugal" },
                new { Lcid=0x0418, Code="ROM", Desc="Romanian Romania" },
                new { Lcid=0x0419, Code="RUS", Desc="Russian Russia" },
                new { Lcid=0x041A, Code="HRV", Desc="Croatian Croatia" },
                new { Lcid=0x081A, Code="SRL", Desc="Serbian Serbia (Latin)" },
                new { Lcid=0x0C1A, Code="SRB", Desc="Serbian Serbia (Cyrillic)" },
                new { Lcid=0x041B, Code="SKY", Desc="Slovak Slovakia" },
                new { Lcid=0x041C, Code="SQI", Desc="Albanian Albania" },
                new { Lcid=0x041D, Code="SVE", Desc="Swedish Sweden" },
                new { Lcid=0x081D, Code="SVF", Desc="Swedish Finland" },
                new { Lcid=0x041E, Code="THA", Desc="Thai Thailand" },
                new { Lcid=0x041F, Code="TRK", Desc="Turkish Turkey" },
                new { Lcid=0x0420, Code="URP", Desc="Urdu Pakistan" },
                new { Lcid=0x0421, Code="IND", Desc="Indonesian Indonesia" },
                new { Lcid=0x0422, Code="UKR", Desc="Ukrainian Ukraine" },
                new { Lcid=0x0423, Code="BEL", Desc="Belarusian Belarus" },
                new { Lcid=0x0424, Code="SLV", Desc="Slovene Slovenia" },
                new { Lcid=0x0425, Code="ETI", Desc="Estonian Estonia" },
                new { Lcid=0x0426, Code="LVI", Desc="Latvian Latvia" },
                new { Lcid=0x0427, Code="LTH", Desc="Lithuanian Lithuania" },
                new { Lcid=0x0827, Code="LTC", Desc="Classic Lithuanian Lithuania" },
                new { Lcid=0x0429, Code="FAR", Desc="Farsi Iran" },
                new { Lcid=0x042A, Code="VIT", Desc="Vietnamese Viet Nam" },
                new { Lcid=0x042B, Code="HYE", Desc="Armenian Armenia" },
                new { Lcid=0x042C, Code="AZE", Desc="Azeri Azerbaijan (Latin)" },
                new { Lcid=0x082C, Code="AZE", Desc="Azeri Azerbaijan (Cyrillic)" },
                new { Lcid=0x042D, Code="EUQ", Desc="Basque Spain" },
                new { Lcid=0x042F, Code="MKI", Desc="Macedonian Macedonia" },
                new { Lcid=0x0436, Code="AFK", Desc="Afrikaans South Africa" },
                new { Lcid=0x0437, Code="KAT", Desc="Georgian Georgia" },
                new { Lcid=0x0438, Code="FOS", Desc="Faeroese Faeroe Islands" },
                new { Lcid=0x0439, Code="HIN", Desc="Hindi India" },
                new { Lcid=0x043E, Code="MSL", Desc="Malay Malaysia" },
                new { Lcid=0x083E, Code="MSB", Desc="Malay Brunei Darussalam" },
                new { Lcid=0x043F, Code="KAZ", Desc="Kazak Kazakstan" },
                new { Lcid=0x0441, Code="SWK", Desc="Swahili Kenya" },
                new { Lcid=0x0443, Code="UZB", Desc="Uzbek Uzbekistan (Latin)" },
                new { Lcid=0x0843, Code="UZB", Desc="Uzbek Uzbekistan (Cyrillic)" },
                new { Lcid=0x0444, Code="TAT", Desc="Tatar Tatarstan" },
                new { Lcid=0x0445, Code="BEN", Desc="Bengali India" },
                new { Lcid=0x0446, Code="PAN", Desc="Punjabi India" },
                new { Lcid=0x0447, Code="GUJ", Desc="Gujarati India" },
                new { Lcid=0x0448, Code="ORI", Desc="Oriya India" },
                new { Lcid=0x0449, Code="TAM", Desc="Tamil India" },
                new { Lcid=0x044A, Code="TEL", Desc="Telugu India" },
                new { Lcid=0x044B, Code="KAN", Desc="Kannada India" },
                new { Lcid=0x044C, Code="MAL", Desc="Malayalam India" },
                new { Lcid=0x044D, Code="ASM", Desc="Assamese India" },
                new { Lcid=0x044E, Code="MAR", Desc="Marathi India" },
                new { Lcid=0x044F, Code="SAN", Desc="Sanskrit India" },
                new { Lcid=0x0457, Code="KOK", Desc="Konkani India" },
                new { Lcid=0x0000, Code="Language-Neutral", Desc="Language-Neutral" },
                new { Lcid=0x0400, Code="Process Default Language", Desc="Process Default Language" }
                };

            return
                locales.Where(l => l.Lcid == lcid)
                    .Select(l => l.Desc)
                    .FirstOrDefault();

        } // Locale

        // ****************************************

        private void LSet<T>(out T d, byte[] cs, int offset)
            where T : struct
        {
            var rdr = new LeImageReader(cs, offset);
            d = rdr.ReadStruct<T>();
        }

        private readonly static Dictionary<int, string> varTypes = new Dictionary<int, string>{
            { 0, "Empty" },
            { 1, "Null" },
            { 2, "I2" },
            { 3, "I4" },
            { 4, "R4" },
            { 5, "R8" },
            { 6, "Cy" },
            { 7, "Date" },
            { 8, "BStr" },
            { 9, "Dispatch" },
            { 10, "Error" },
            { 11, "Bool" },
            { 12, "Variant" },
            { 13, "Unknown" },
            { 14, "Decimal" },
            //Data
            { 16, "I1" },
            { 17, "UI1" },
            { 18, "UI2" },
            { 19, "UI4" },
            { 20, "I8" },
            { 21, "UI8" },
            { 22, "Int" },
            { 23, "UInt" },
            { 24, "Void" },
            { 25, "HResult" },
            { 26, "Ptr" },
            { 27, "SafeArray" },
            { 28, "CArray" },
            { 29, "UserDefined" },
            { 30, "LPStr" },
            { 31, "LPWStr" },
            //Data , , ,
            { 36, "Record" },
            // 	end of continuous sequence
            { 64, "FileTime" },
            { 65, "Blob" },
            { 66, "Stream" },
            { 67, "Storage" },
            { 68, "Streamed_Object" },
            { 69, "Stored_Object" },
            { 70, "Blob_Object" },
            { 71, "CF" },
            { 72, "ClsID" },
            // Data "Bstr_Blob=4095"
        };

        string VarType(int pn) {
            pn = pn & VT_TypeMask;
            if (varTypes.TryGetValue(pn, out string ls))
                return "VT_" + ls;
            return "(Unknown)";

        } // VarType

#if NOT_YET
        // ****************************************
        // offs = zero-based offset into the GUID table
        public String tlGuid(byte[] cs, MSFT_SegDir SegDir, int offs) {
            if (offs >= 0)
            {
                LSet(out Guid g, cs, SegDir.pGUID!.Offs + offs);
                return g.ToString();
            }
            else
                return "";

        } // tlGuid

        public unsafe string tlGuid(byte[] cs, MSFT_SegDir SegDir, uint offs) =>
            tlGuid(cs, SegDir, (int) offs);

        // ****************************************
        // offs = zero-based offset into the name table
        public string? tlName(byte[] cs, MSFT_SegDir SegDir, int offs) {
            if (offs >= 0) {
                offs = SegDir.pNames.Offs + offs + 1;
                LSet(out NameIntro NameInt, cs, offs);
                offs += sizeof(MSFT.NameIntro);
                return $"\"{Encoding.ASCII.GetString(cs, offs, NameInt.cName & 0x0FF)}\"";
            }
            return null;
        } // tlName

        public string? tlName(byte[] cs, MSFT_SegDir SegDir, uint offs) =>
            tlName(cs, SegDir, (int) offs);

        // ****************************************
        // offs = zero-based offset into the string table
        public string? tlString(byte[] cs, MSFT_SegDir SegDir, int offs) {
            if (offs >= 0 && offs < cs.Length) {
                offs = SegDir.pStrings.Offs + offs;
                return "\"" + Encoding.ASCII.GetString(cs, offs + 2, CvWrd(cs, offs)) + "\"";
            }
            else return "";

        } // tlString

        public string? tlString(byte[] cs, MSFT_SegDir SegDir, uint offs) =>
            tlString(cs, SegDir, (int) offs);


        /// <summary>
        /// <param name="cs">    TypeLib data                         </param>
        /// <param name="SegDir">	the segment directory                </param>
        /// <param name="pBase">	the zero-based offset to the FuncRec </param>
        /// <param name="nFunc">	number of functions                  </param>
        /// <param name="nProp">	number of properties                 </param>
        /// <param name="fo">	file handle                          </param>
        /// </summary>
        public unsafe void DisFunction(
            byte[] cs,
            MSFT_SegDir SegDir,
            int pBase,
            int nFunc,
            int nProp,
            TextWriter fo)
        {
            fixed (byte* pcs = cs)
            {
                int d;
                int i;
                int j;
                int n;
                int ub = 0;
                int p;
                int iElem = 0;
                int nAttr;
                int ArraySize;
                int oParamInfo;
                int oDefValue;

                FuncRecord FuncRec;
                ParamInfo ParmInfo;
                PropRecord PropRec;

                p = pBase;
                ArraySize = (int) CvDwd(cs, pBase);
                fo.WriteLine();
                fo.WriteLine("	Function record array size:	{0:X8}", ArraySize);
                p = p + 4; // advance past the "ArraySize" value
                int* Refer = null;
                int* IdElem = null;
                int* oName = null;

                // ----------------------------------------
                // 	Other function and property data
                n = nFunc + nProp;
                if (n > 0) {
                    ub = n - 1;
                    byte* pTmp = pcs + pBase + ArraySize + 4;
                    IdElem = (int*) pTmp;
                    pTmp = pTmp + (4 * n);
                    oName = (int*) pTmp;
                    pTmp = pTmp + (4 * n);
                    Refer = (int*) pTmp;
                }


                if (nFunc != 0)
                {
                    fo.WriteLine();
                    fo.WriteLine("	----------------------------------------");
                    fo.WriteLine("				Functions:");
                    for (i = 1; i <= nFunc; ++i)
                    {

                        LSet(out FuncRec, cs, p);

                        fo.WriteLine("	----------------------------------------");
                        fo.WriteLine("	ID:			{0:X4} ==> {1}", IdElem[iElem], IdElem[iElem]);
                        fo.WriteLine("	Name:		{0:X8} ==> {1}", oName[iElem], tlName(cs, SegDir, oName[iElem]));
                        fo.WriteLine("	Reference:	{0:X8}", Refer[iElem]); // offset to the corresponding function record
                        ++iElem;
                        fo.WriteLine("	Record size:	{0:X4}", FuncRec.RecSize);
                        fo.WriteLine("	Unknown 1:		{0:X4}", FuncRec.Unk1);
                        fo.WriteLine("	Flags:			{0:X4}{1}", FuncRec.Flags, FuncRec.Flags < 0 ? " = " + VarType(FuncRec.Flags) : "");
                        fo.WriteLine("	DataType:		{0:X4} = {1}", FuncRec.DataType, VarType(FuncRec.DataType));

#if Study
			fo.WriteLine(
			fo.WriteLine( "	Reserved 1:		", FuncRec.Res1, 8)
#endif

                        fo.WriteLine("	Vtable offset:	{0:X4}", FuncRec.oVtable);
                        fo.WriteLine("	Func Desc Size:	{0:X4}", FuncRec.cFuncDesc);

                        // 	FKCCIC
                        // The bits in FKCCIC have the following meanings:
                        // 0 - 2 = function kind (eg virtual)
                        // 3 - 6 = Invocation kind
                        // 7 means custom data is present
                        // 8 - 11 = calling convention
                        // 12 means one or more parameters have a default value
                        fo.WriteLine();
                        fo.WriteLine("	FKCCIC (raw):	{0:X8}", FuncRec.FKCCIC);
                        if ((FuncRec.FKCCIC & 0x01000) != 0) fo.WriteLine("		Default value(s) present");
                        if ((FuncRec.FKCCIC & 0x040000) != 0) fo.WriteLine("		oEntry is numeric");
                        if ((FuncRec.FKCCIC & 0x080) != 0) fo.WriteLine("		Custom data present");

                        d = (int) ((FuncRec.FKCCIC & 0x0F00) >> 8);
                        fo.WriteLine("		Calling convention:	{0:X2} = {1}", d, (CallConv) d);

                        d = (int) ((FuncRec.FKCCIC & 0x078) >> 3); // this is a bit field
                        fo.Write("		Invocation kind:	{0:X2} = ", d);
                        for (n = 4; n >= 1; --n) {
                            if ((d & INVOKE_PROPERTYPUTREF) != 0) {
                                fo.WriteLine((InvoKind) n);
                                if ((d & 0x07) != 0) fo.Write(", ");
                            }
                            d <<= 1;
                        }
                        fo.WriteLine();

                        d = (int) (FuncRec.FKCCIC & 7);
                        fo.WriteLine("		Function kind:		{0:X2} = ", d, (FuncKind) d);


                        // 	Algorithm
                        // 1) Dim the ParamInfo array at the end of the available space
                        // 2) If (FKCCIC And 0x1000) then Dim an array of default values just before the ParamInfo array
                        // 3) Assume anything preceding the above arrays is the function// s optional data
                        fo.WriteLine();
                        n = FuncRec.nParams;
                        fo.WriteLine("	Number of parameters:	{0:X4} == {0}", n);

                        //#If %Def(%Study)
                        fo.WriteLine("	Unknown 2:		{0:X4}", FuncRec.Unk2);
                        //#EndIf

                        oParamInfo = p + FuncRec.RecSize - (n * sizeof(ParamInfo)); // must be one-based

                        oDefValue = oParamInfo;
                        int* DefVal = null;
                        // If (FuncRec.FKCCIC And 0x01000) Then // there might be default values present
                        if ((FuncRec.FKCCIC & 0x01000) != 0 && (n > 0)) { // there might be default values present
                            oDefValue = oDefValue - (n * 4);
                            DefVal = (int*) (pcs + oDefValue); // must be zero-based
                        }


                        // Dim array for the function// s optional data, if any
                        ub = (((oDefValue - sizeof(FuncRecord)) - p) / 4) - 1;
                        if (ub >= 0) {

                            fo.WriteLine("		----------------------------------------");
                            fo.WriteLine("		Optional Data:");
                            int* OptData = (int*) (pcs + p + sizeof(FuncRecord)); // must be zero-based

                            fo.WriteLine("		HelpContext:		{0:X8}", OptData[0]);
                            if (ub < 1) goto ExitIf;

                            fo.WriteLine("		HelpString:			{0:X8}", OptData[1]);
                            fo.WriteLine(" ==> ", tlString(cs, SegDir, OptData[1]));
                            if (ub < 2) goto ExitIf;

                            fo.WriteLine("		Entry:				{0:X8}", OptData[2]);

                            //#If %Def(%Study)
                            if (ub < 3) goto ExitIf;

                            fo.WriteLine("		Reserved09:	{0:X8}", OptData[3]);
                            if (ub < 4) goto ExitIf;

                            fo.WriteLine("		Reserved0A:	{0:X8}", OptData[4]);
                            //#EndIf
                            if (ub < 5) goto ExitIf;

                            fo.WriteLine("		HelpStringContext:	{0:X8}", OptData[5]);
                            if (ub < 6) goto ExitIf;

                            fo.WriteLine("		Custom Data:		{0:X8}", OptData[6]);
                        }
                        ExitIf:
                        fo.WriteLine();

                        for (j = 0; j < n; ++j) {
                            LSet(out ParmInfo, cs, oParamInfo);
                            fo.WriteLine("		----------------------------------------");
                            fo.WriteLine("		Parameter number:	{0}", j + 1);
                            fo.WriteLine("		DataType:			{0:X8} ", ParmInfo.DataType, ParmInfo.DataType >= 0 ? " = " + VarType(ParmInfo.DataType) : "");
                            fo.WriteLine("		Flags:				{0:X4} = {1}", ParmInfo.Flags, VarType(ParmInfo.Flags));
                            fo.WriteLine("		Name:				{0:X8} ==> {1}", ParmInfo.oName, tlName(cs, SegDir, ParmInfo.oName));
                            fo.Write("		ParamFlags:			{0:X8} = ", ParmInfo.fParam);
                            if (ParmInfo.fParam != 0) {
                                d = (int) ParmInfo.fParam;
                                for (n = 7; n >= 1; --n) { //  7 = ParseCount($ParamFlags)
                                    if ((d & PARAMFLAG_FHASCUSTDATA) != 0) {
                                        fo.Write((ParamFlags) n);
                                        if ((d & 0x03F) != 0) fo.WriteLine(", ");
                                    }
                                    d <<= 1;
                                }
                                fo.WriteLine();
                            } else {
                                fo.WriteLine("(none)");
                            }
                            // If (ParmInfo.fParam And %PARAMFLAG_FHASDEFAULT) Then
                            if (DefVal.Length >= 0 && (ParmInfo.fParam & PARAMFLAG_FHASDEFAULT) != 0) {
                                if (DefVal[j] < 0) { // the default value is in the lower three bytes
                                    DefVal[j] = DefVal[j] & 0x0FFFFFF;
                                } else { // it's an offset into the CustomData table
                                    DefVal[j] = Cvl(cs, SegDir.pCustData.Offs + DefVal[j] + 3);
                                }
                                fo.WriteLine("		Default Value:		{0:X8} == {0}", DefVal[j]);
                            }
                            oParamInfo = oParamInfo + sizeof(ParamInfo);
                        }
                        p = p + FuncRec.RecSize;
                    }
                }// nFunc


                // do the properties
                if (nProp != 0) {

                    fo.WriteLine();
                    fo.WriteLine("	----------------------------------------");
                    fo.WriteLine("		 Properties:");

                    for (i = 1; i <= nProp; ++i)
                    {

                        LSet(out PropRec, cs, p);

                        fo.WriteLine("	----------------------------------------");
                        fo.WriteLine("	ID:			{0:X8} == {0}", IdElem[iElem]);
                        fo.WriteLine("	Name:		{0:X8} ==> {1}", oName[iElem], tlName(cs, SegDir, oName[iElem]));
                        fo.WriteLine("	Reference:	{0:X8}", Refer[iElem]); // offset to the corresponding function record
                        ++iElem;
                        fo.WriteLine("	Record size (low-byte):	{0:X4}", PropRec.RecSize);
                        fo.WriteLine("	Property number?:		{0:X4}", PropRec.PropNum);
                        fo.WriteLine("	Flags:					{0:X4}{1}", PropRec.Flags, PropRec.Flags < 0 ? " = " + VarType(PropRec.Flags) : "");
                        fo.WriteLine("	DataType:				{0:X4} = {1}", PropRec.DataType, VarType(PropRec.DataType));

                        fo.Write("	Variable kind:			{0:X4} = ", PropRec.VarKind);
                        d = PropRec.VarKind;
                        for (n = 13; n >= 1; --n)
                        { //  13 = ParseCount($VarFlags)
                            if ((d & VARFLAG_FIMMEDIATEBIND) != 0)
                            {
                                fo.Write((VarFlags) n);
                                if ((d & 0x0FFF) != 0) fo.WriteLine(", ");
                            }
                            d <<= 1;
                        }
                        fo.WriteLine();

                        fo.WriteLine("	Variable desc size:		{0:X4}", PropRec.cVarDesc);
                        fo.WriteLine("	Value/Offset:			{0:X8}", PropRec.OffsValue);

                        //If %Def(%Study)
                        fo.WriteLine("	Unknown:				{0:X8}", PropRec.Unk, 8);
                        //EndIf

                        if (PropRec.RecSize > 20) {  // 20 = (5 * SizeOf(Long))
                            fo.WriteLine("	HelpContext:			{0:X8}", PropRec.HelpCntxt);

                            if (PropRec.RecSize > 24) { // 24 = (6 * SizeOf(Long))
                                fo.WriteLine("	HelpString:	{0:X8}", PropRec.oHelpStr);
                                fo.WriteLine(" ==> {0}", tlString(cs, SegDir, PropRec.oHelpStr));

                                if (PropRec.RecSize > 32) { // 32 = (8 * SizeOf(Long))

                                    if (PropRec.RecSize > 36) { // 36 = (9 * SizeOf(Long))
                                                                //If %Def(%Study)
                                        fo.WriteLine("	Reserved:			{0:X8}", PropRec.Res);
                                        //EndIf
                                        fo.WriteLine("	HelpStringContext:	{0:X8}", PropRec.HelpStrCnt);
                                    }

                                }

                            }
                        }
                        fo.WriteLine();
                        p = p + PropRec.RecSize;
                    }
                } // nProp

                //If %Def(%Study)
                // ----------------------------------------
                // 	Dump arrays of function and property: IDs, names and references
                // ----------------------------------------
                //  This is redundant, since the information is printed along with the
                // functions and properties they pertain to.
                n = nFunc + nProp;
                if (n > 0) {

                    iElem = 0;

                    if (nFunc != 0) {
                        fo.WriteLine();
                        fo.WriteLine("	----------------------------------------");
                        fo.WriteLine("		 Other Function Data:");
                    }

                    for (iElem = 0; iElem <= ub; ++iElem) {

                        if (n == nProp) { // the functions are done, so do the properties
                            fo.WriteLine();
                            fo.WriteLine("	----------------------------------------");
                            fo.WriteLine("		 Other Property Data:");
                        }
                        --n;

                        // ID number of the function or property
                        fo.WriteLine("	----------------------------------------");
                        fo.WriteLine("	ID:			{0:X8} == {0}", IdElem[iElem]);

                        // offset to names in the name table
                        fo.WriteLine("	Name:		{0:X8}", oName[iElem]);
                        fo.WriteLine(" ==> {0}", tlName(cs, SegDir, oName[iElem]));

                        // offset to the corresponding function record
                        fo.WriteLine("	Reference:	{0:X8}", Refer[iElem]);
                        fo.WriteLine();
                    }
                }
            }
        } // DisFunction

        // ****************************************
        // cs contains the TypeLib data
        // ls contains the name of the source file
        public unsafe void DisTypeLib(byte[] cs, TextWriter fo)
        {
            int i;
            int n;
            int p;

            LSet(out TlbHeader TlbHdr, cs, 0);
            int fName = (TlbHdr.fVar & HELPDLLFLAG) != 0 ? 4 : 0; // TypeLib file name flag
            p = sizeof(TlbHeader) + (4 * (TlbHdr.nTypeInfo)) + fName;
            LSet(out MSFT_SegDir SegDir, cs, p);

            PrintTypelibHeader(cs, fo, TlbHdr, SegDir, fName);

            PrintTypeinfoOffsets(cs, fo, p, fName, TlbHdr);

            PrintSegmentDirectory(fo, SegDir);

//If %Def(%Study)

            int fAlert = 0;
            SegDesc * pSegTmp = &SegDir.pTypInfo; //ReDim pSegTmp(14) As SegDesc At VarPtr(SegDir);
            for (i = 0; i <= 14; ++i)
            {
                if (pSegTmp[i].Res01 != -1)
                {
                    fo.WriteLine("!!!");
                    ++fAlert;
                }

                if (pSegTmp[i].Res02 != 0x0F)
                {
                    fo.WriteLine("!!!");
                    ++fAlert;
                }

                if (fAlert == 1)
                {
                    MsgBox("Interesting Reserved value found");
                    ++fAlert;
                }
            }

            // If fAlert Then
            fo.WriteLine();
            fo.WriteLine("@@@:	Reserved Fields");
            fo.WriteLine("Segment name:	Res01, Res02...");
            fo.WriteLine("*************************");
            fo.WriteLine("Type Info Table:		{0:X8}, {1:X8}", SegDir.pTypInfo.Res01, SegDir.pTypInfo.Res02);
            fo.WriteLine("Import Info:			{0:X8}, {1:X8}", SegDir.pImpInfo.Res01, SegDir.pImpInfo.Res02);
            fo.WriteLine("Imported Libraries:		{0:X8}, {1:X8}", SegDir.pImpFiles.Res01, SegDir.pImpFiles.Res02);
            fo.WriteLine("References Table:		{0:X8}, {1:X8}", SegDir.pRefer.Res01, SegDir.pRefer.Res02);
            fo.WriteLine("Lib Table:				{0:X8}, {1:X8}", SegDir.pLibs.Res01, SegDir.pLibs.Res02);
            fo.WriteLine("GUID Table:				{0:X8}, {1:X8}", SegDir.pGUID.Res01, SegDir.pGUID.Res02);
            fo.WriteLine("Unknown 01:				{0:X8}, {1:X8}", SegDir.Unk01.Res01, SegDir.Unk01.Res02);
            fo.WriteLine("Name Table:				{0:X8}, {1:X8}", SegDir.pNames.Res01, SegDir.pNames.Res02);
            fo.WriteLine("String Table:			{0:X8}, {1:X8}", SegDir.pStrings.Res01, SegDir.pStrings.Res02);
            fo.WriteLine("Type Descriptors:		{0:X8}, {1:X8}", SegDir.pTypDesc.Res01, SegDir.pTypDesc.Res02);
            fo.WriteLine("Array Descriptors:		{0:X8}, {1:X8}", SegDir.pArryDesc.Res01, SegDir.pArryDesc.Res02);
            fo.WriteLine("Custom Data:			{0:X8}, {1:X8}", SegDir.pCustData.Res01, SegDir.pCustData.Res02);
            fo.WriteLine("Custom Data/GUID's:		{0:X8}, {1:X8}", SegDir.pCDGuids.Res01, SegDir.pCDGuids.Res02);
            fo.WriteLine("Unknown 02:				{0:X8}, {1:X8}", SegDir.Unk02.Res01, SegDir.Unk02.Res02);
            fo.WriteLine("Unknown 03:				{0:X8}, {1:X8}", SegDir.Unk03.Res01, SegDir.Unk03.Res02);
            fo.WriteLine();
            // End If

            //EndIf


            // --------------------------------------
            // check two entries to be sure we found it
            if (SegDir.pTypInfo.Res02 != 0x0F || SegDir.pImpInfo.Res02 != 0x0F)
            {
                listener.Warn("Can't find the table directory");
                return;
            }

            PrintTypeInfos(cs, fo, SegDir);

            PrintImportInfo(cs, fo, SegDir);

            PrintImportedTypeLibs(cs, fo, SegDir);

            PrintReferenceTable(cs, fo, SegDir);

            fo.WriteLine();
            PrintLibrTable(cs, fo, SegDir);

            PrintGuidTable(cs, fo, SegDir);

            PrintUnknown01(cs, fo, SegDir);

            PrintNameTable(cs, fo, SegDir);

            PrintStringTable(cs, fo, SegDir);

            PrintTypeDescriptors(cs, fo, SegDir);

            PrintArrayDescriptors(cs, fo, SegDir);

            PrintCustomData(cs, fo, SegDir);

            PrintGuidOffsets(cs, fo, SegDir);

            // --------------------------------------
            // Print "Unknown 02"
            if (SegDir.Unk02.nLen != 0)
            {
                //If %Def(%Study)
                //Note "Unknown 02"
                //EndIf
                fo.WriteLine();
                fo.WriteLine("*************************");
                fo.WriteLine("Unknown 02");
                fo.WriteLine("*************************");
                p = SegDir.Unk02.Offs;
                n = p + SegDir.Unk02.nLen;
                while (p < n)
                {
                    fo.WriteLine("{0:X8}", CvDwd(cs, p));
                    p = p + 4;
                } //Loop While p <= Len(cs)
            }


            // --------------------------------------
            // Print "Unknown 03"
            if (SegDir.Unk03.nLen > 0)
            {
                //If %Def(%Study)
                //Note "Unknown 03"
                //EndIf
                fo.WriteLine();
                fo.WriteLine("*************************");
                fo.WriteLine("Unknown 03");
                fo.WriteLine("*************************");
                p = SegDir.Unk03.Offs;
                n = p + SegDir.Unk03.nLen;
                while (p < n)
                {
                    fo.WriteLine("{0:X8}", CvDwd(cs, p));
                    p = p + 4;
                } //Loop While p <= Len(cs)
            }
        }

        private static unsafe void PrintTypeinfoOffsets(byte[] cs, TextWriter fo, int p, int fName, TlbHeader TlbHdr)
        {
            int n;
            // --------------------------------------
            // print the (TypeInfo ?) offsets
            fo.WriteLine();
            fo.WriteLine("** Offsets to TypeInfo Data ***********************");
            p = p + fName;
            for (n = 1; n <= TlbHdr.nTypeInfo; ++n)
            {
                fo.WriteLine("{0:X8}", CvDwd(cs, p));
                p = p + 4;
                if (p >= cs.Length) break;
            }
        }

        private unsafe void PrintTypeInfos(byte[] cs, TextWriter fo, in MSFT_SegDir SegDir)
        {
            // --------------------------------------
            // print the TypeInfo structures
            fo.WriteLine();
            fo.WriteLine("** TypeInfo Data ***********************");
            int i = 0;
            int p = SegDir.pTypInfo.Offs;
            int n = Math.Min(cs.Length, p + SegDir.pTypInfo.nLen);
            while (p < n)
            {
                ++i;
                fo.WriteLine("-------------------------");
                fo.WriteLine("TypeInfo number:	{0}", i);
                LSet(out TypeInfo TypInfo, cs, p);

                fo.WriteLine("Type kind:	{0:X2} = {1}", TypInfo.TypeKind & 7, (TKind) (TypInfo.TypeKind & 7));
                int d = TypInfo.Align >> 3;
                fo.WriteLine("	Alignment: ({0:X2}/8) = ", TypInfo.Align, d);

                // (function records are printed near the end of the TypeInfo section)

                fo.WriteLine("Memory to allocate:		{0:X8}", TypInfo.Alloc);
                fo.WriteLine("Reconstituted Size:		{0:X8}", TypInfo.Reconst);

                //If %Def(%Study)
                fo.WriteLine("Reserved 01:	{0:X8}", TypInfo.Res01);
                fo.WriteLine("Reserved 02:	{0:X8}", TypInfo.Res02);
                //EndIf

                // counts of functions and properties
                fo.WriteLine();
                fo.WriteLine("Function count:	{0:X4} == {0}", TypInfo.nFuncs);
                fo.WriteLine("Property count:	{0:X4} == {0}", TypInfo.nProps);

                //If %Def(%Study)
                fo.WriteLine("Reserved 03:	{0:X8}", TypInfo.Res03);
                fo.WriteLine("Reserved 04:	{0:X8}", TypInfo.Res04);
                fo.WriteLine("Reserved 05:	{0:X8}", TypInfo.Res05);
                fo.WriteLine("Reserved 06:	{0:X8}", TypInfo.Res06);
                //EndIf

                fo.WriteLine("GUID:	{0:X8} ==> {1}", TypInfo.oGUID, tlGuid(cs, SegDir, TypInfo.oGUID));

                fo.WriteLine("Type Flags:	{0:X8} = ", TypInfo.fType);
                d = TypInfo.fType; // this is a bit field
                for (n = 14; n >= 1; --n)
                {
                    if ((d & TYPEFLAG_FREVERSEBIND) != 0)
                    {
                        fo.WriteLine((TypeFlags) n);
                        if ((d & TYPEFLAG_MASK) != 0)
                            fo.WriteLine(", ");
                    }
                    d <<= 1;
                }
                fo.WriteLine();


                fo.WriteLine("Name:	{0:X8} ==> {1}", TypInfo.oName, tlName(cs, SegDir, TypInfo.oName));
                fo.WriteLine("Version:	{0:X8}", TypInfo.Version);

                fo.WriteLine("Doc String:	{0:X8}", TypInfo.DocStr);
                fo.WriteLine(" ==> {0}", tlString(cs, SegDir, TypInfo.DocStr));

                fo.WriteLine("HelpStringContext:	{0:X8}", TypInfo.HelpStrCnt);
                fo.WriteLine("HelpContext:		{0:X8}", TypInfo.HelpCntxt);

                fo.WriteLine();
                fo.WriteLine("Custom data offset:		{0:X8}", TypInfo.oCustData);
                fo.WriteLine("Implemented interfaces:	{0:X4} == {0}", TypInfo.nImplTypes);
                fo.WriteLine("Virtual table size:		{0:X4}", TypInfo.cVft);
                fo.WriteLine("Unknown 03:				{0:X8}", TypInfo.Unk03);

                // position in type description table or in base interfaces
                // if coclass:	offset in reftable
                // if interface:	reference to inherited interface
                // if module:		offset to DLL name in name table

                //If %Def(%Study)
                switch (TypInfo.TypeKind & 0x0F)
                {
                // Case %TKIND_ENUM
                // Case %TKIND_RECORD
                // Case %TKIND_MODULE // offset to DLL name in string table
                // Case %TKIND_INTERFACE // reference to inherited interface?
                // Case %TKIND_DISPATCH
                // Case %TKIND_COCLASS // offset in reftable
                case TKIND_ALIAS:
                    //Note("TKIND_ALIAS");
                    break;
                case TKIND_UNION:
                    //Note("TKIND_UNION");
                    break;
                    // Case %TKIND_MAX
                    // Case Else
                }
                //EndIf

                // fo.WriteLine( "Type1:	", TypInfo.Type1, 8)
                switch (TypInfo.TypeKind)
                {

                // Case %TKIND_ENUM

                // Case %TKIND_RECORD

                case TKIND_MODULE: // offset to DLL name in string table
                    fo.WriteLine("DLL Name:	{0:X8} ==>", TypInfo.Type1);
                    if (TypInfo.Type1 >= 0) fo.WriteLine(tlName(cs, SegDir, TypInfo.Type1));
                    break;

                case TKIND_INTERFACE: // reference to inherited interface?
                    fo.WriteLine("Inherited interface?:	{0:X8}", TypInfo.Type1);
                    break;
                // Case %TKIND_DISPATCH

                case TKIND_COCLASS: // offset in reftable
                    fo.WriteLine("Reference table offset:	{0:X8}", TypInfo.Type1);
                    break;
                case TKIND_ALIAS: // the following is partly translated ReactOS code

                    if (TypInfo.Type1 < 0)
                    {
                        d = TypInfo.Type1 & VT_TypeMask;
                    }
                    else
                    { // get index into Lib Table?
                        d = TypInfo.Type1 / 8; // ?
                    }

                    if (TypInfo.Type1 == VT_UserDefined)
                    { // do RefType
                    }
                    break;

                // Case %TKIND_UNION

                // Case %TKIND_MAX

                default:
                    fo.WriteLine("DataType1:	{0:X8}", TypInfo.Type1);
                    break;
                }

                // if 0x8000, entry above is valid, else it is zero?
                fo.WriteLine("DataType2:	{0:X8}", TypInfo.Type2);

                //If %Def(%Study)
                fo.WriteLine("Reserved 7:	{0:X8}", TypInfo.Res07);
                fo.WriteLine("Reserved 8:	{0:X8}", TypInfo.Res08);
                //EndIf

                fo.WriteLine();
                fo.WriteLine("Records offset:	{0:X8}", TypInfo.oFunRec);
                if (TypInfo.oFunRec < cs.Length)
                { // do function/property records
                    DisFunction(cs, SegDir, TypInfo.oFunRec, TypInfo.nFuncs, TypInfo.nProps, fo);
                }
                p = p + sizeof(TypeInfo);
            }
        }

        private static unsafe void PrintGuidOffsets(byte[] cs, TextWriter fo, MSFT_SegDir SegDir)
        {
            // --------------------------------------
            // Print offsets of GUID// s and into the
            if (SegDir.pCDGuids.nLen != 0)
            {
                // custom data table
                fo.WriteLine();
                fo.WriteLine("** Offsets of GUID ***********************");
                int p = SegDir.pCDGuids.Offs;
                int n = p + SegDir.pCDGuids.nLen;
                while (p < n)
                {
                    fo.WriteLine("0:X8", CvDwd(cs, p));
                    p = p + 4;
                } //Loop While p <= Len(cs)
            }
        }

        private unsafe void PrintArrayDescriptors(byte[] cs, TextWriter fo, MSFT_SegDir SegDir)
        {
            // --------------------------------------
            // Print Array Descriptors
            if (SegDir.pArryDesc.nLen > 0)
            {

                //If %Def(%Study)
                //Note "Array(s) found"
                //EndIf

                //  What do the lower bits of td(1) mean (when td(1) < 0)?
                //  What is td(3) (it gets over-written when td(1) < 0) ?
                fo.WriteLine();
                fo.WriteLine("** Array Descriptors ***********************");
                int p = SegDir.pArryDesc.Offs;
                int n = p + SegDir.pArryDesc.nLen;
                while (p < n)
                {
                    LSet(out ARRAYDESC AryDsc, cs, p);
                    //ReDim td(3)		As Integer At VarPtr(AryDsc)

                    fo.WriteLine("--------------------------------------------");
                    fo.WriteLine("Raw:	{0}, {1}, {2}, {3}", CvDwd(cs, p), CvDwd(cs, p + 4), CvDwd(cs, p + 8), CvDwd(cs, p + 12));

                    if (AryDsc.u.hRefType >= 0)
                    { // a pointer to ANOTHER array descriptor?
                        AryDsc.u.lpadesc = (AryDsc.u.lpadesc & 0x07FFF) / 8;
                        fo.WriteLine("Offset to array descriptor:	", AryDsc.u.lpadesc);
                    }
                    else
                    { // the low word contains the variable-type code
                        fo.WriteLine("hRefType:	", AryDsc.u.hRefType, 8);
                        AryDsc.tVar = td(0) & VT_TypeMask;
                    }

                    int d = ((AryDsc.tVar & 255) == 0)
                        ? HiByt(AryDsc.tVar)
                        : AryDsc.tVar;
                    fo.WriteLine("Variable type:	{0:X4} = {1}", AryDsc.tVar, VarType(d));
                    fo.WriteLine("Number of dimensions:	{0:X4} == {0}", AryDsc.nDims);

                    p = p + sizeof(ARRAYDESC) - sizeof(SafeArrayBound);

                    for (int i = 1; i < AryDsc.nDims; ++i)
                    {
                        LSet(out SafeArrayBound SafBnd, cs, p);
                        fo.WriteLine("({0}) Elements: {1:X8} == {1} Lower bound: {2:X8} == {2}", i, SafBnd.nElements, SafBnd.lLBound);
                        p = p + sizeof(SafeArrayBound);
                        if (p >= cs.Length) break;
                    }

                } //Loop While p <= Len(cs)
            }

        }

        private static unsafe void PrintCustomData(byte[] cs, TextWriter fo, MSFT_SegDir SegDir)
        {
            // --------------------------------------
            // Print Custom Data
            // a guess as to the storage format...
            // Type CustomData
            // 	cWords				As Word
            // 	aData(cWords - 1)	As Word
            // 	sGUID				As Guid
            // End Type
            if (SegDir.pCustData.nLen != 0)
            {
                //If %Def(%Study)
                //if (SegDir.pCustData.nLen > 16) {
                //	Note "More than 16 bytes of custom data"
                //}
                //EndIf
                // custom data and default parameter values
                fo.WriteLine();
                fo.WriteLine("** Custom Data ***********************");
                int p = SegDir.pCustData.Offs;
                int n = p + SegDir.pCustData.nLen;
                while (p < n)
                {
                    fo.WriteLine("{0:X8}", CvDwd(cs, p));
                    p = p + 4;
                } // Loop While p <= Len(cs)
            }
        }

        private unsafe void PrintGuidTable(byte[] cs, TextWriter fo, MSFT_SegDir SegDir)
        {
            // --------------------------------------
            // Print GUID table
            fo.WriteLine();
            fo.WriteLine("** GUID Table ***********************");
            int p = SegDir.pGUID.Offs;
            int n = p + SegDir.pGUID.nLen;
            while (p < n)
            {

                LSet(out GuidEntry GuidEnt, cs, p);
                fo.WriteLine("--------------------------------------------");
                fo.WriteLine("GUID:	{0}", GuidEnt.oGUID);
                // 	The meaning of .hRefType:
                //  = -2 for a TypeLib GUID
                // TypeInfo offset for TypeInfo GUID,
                // Otherwise, the low two bits:
                // 	= 01 for an imported TypeInfo
                // 	= 10 for an imported TypeLib (used by imported TypeInfos)
                fo.WriteLine("Href Type:	{0:X8}", GuidEnt.hRefType);
                if (GuidEnt.hRefType == -2)
                {
                    fo.WriteLine(" = TypeLib GUID");
                }
                else if ((GuidEnt.hRefType & 3) == 1)
                {
                    fo.WriteLine(" = Imported TypeInfo");
                }
                else if ((GuidEnt.hRefType & 3) == 2)
                {
                    fo.WriteLine(" = Imported TypeLib");
                }
                else
                {
                    fo.WriteLine(" = Offset?");
                }

                fo.WriteLine("Next hash:	{0:X8}", GuidEnt.NextHash);
                p = p + sizeof(GuidEntry);
            } // While p <= Len(cs)
        }

        private unsafe void PrintImportedTypeLibs(byte[] cs, TextWriter fo, in MSFT_SegDir SegDir)
        {
            // --------------------------------------
            // Print Imported Type Libs
            fo.WriteLine();
            fo.WriteLine("** Imported Type Libs ***********************");
            int p = SegDir.pImpFiles.Offs;
            int n = p + SegDir.pImpFiles.nLen;
            while (p < n)
            {
                //Loop While p <= Len(cs)
                LSet(out TlbImpLib IFileInfo, cs, p);
                fo.WriteLine("--------------------------------------------");
                fo.WriteLine("GUID:			{0:X8} ==> {1}", IFileInfo.oGUID, tlGuid(cs, SegDir, IFileInfo.oGUID));
                fo.WriteLine("Locale ID?:		{0:X8} ==> {1}", IFileInfo.LCID, Locale(IFileInfo.LCID));
                fo.WriteLine("Major version:	{0:X4}", IFileInfo.MajVer);
                fo.WriteLine("Minor version:	{0:X4}", IFileInfo.MinVer);

                int d = IFileInfo.cSize >> 2;
                fo.WriteLine("Size = 		{0:X4} /4 = {1:X4}", IFileInfo.cSize, d);
                fo.WriteLine("File name:	{0}", Encoding.ASCII.GetString(cs, (int) p + sizeof(TlbImpLib), d));

                p = p + ((sizeof(TlbImpLib) + IFileInfo.cSize + 3) & ~3); // advance to next one
            }
        }

        private static unsafe void PrintSegmentDirectory(TextWriter fo, MSFT_SegDir SegDir)
        {
            // --------------------------------------
            // print the segment directory contents
            fo.WriteLine();
            fo.WriteLine("*************************");
            fo.WriteLine("Segment Directory (Segment name: Offset, Length)...");
            fo.WriteLine("*************************");
            fo.WriteLine("Type Info Table:		{0:X8}, {1:X8}", SegDir.pTypInfo.Offs, SegDir.pTypInfo.nLen);
            fo.WriteLine("Import Info:			{0:X8}, {1:X8}", SegDir.pImpInfo.Offs, SegDir.pImpInfo.nLen);
            fo.WriteLine("Imported Libraries:		{0:X8}, {1:X8}", SegDir.pImpFiles.Offs, SegDir.pImpFiles.nLen);
            fo.WriteLine("References Table:		{0:X8}, {1:X8}", SegDir.pRefer.Offs, SegDir.pRefer.nLen);
            fo.WriteLine("Lib Table:				{0:X8}, {1:X8}", SegDir.pLibs.Offs, SegDir.pLibs.nLen);
            fo.WriteLine("GUID Table:				{0:X8}, {1:X8}", SegDir.pGUID.Offs, SegDir.pGUID.nLen);
            fo.WriteLine("Unknown 01:				{0:X8}, {1:X8}", SegDir.Unk01.Offs, SegDir.Unk01.nLen);
            fo.WriteLine("Name Table:				{0:X8}, {1:X8}", SegDir.pNames.Offs, SegDir.pNames.nLen);
            fo.WriteLine("String Table:			{0:X8}, {1:X8}", SegDir.pStrings.Offs, SegDir.pStrings.nLen);
            fo.WriteLine("Type Descriptors:		{0:X8}, {1:X8}", SegDir.pTypDesc.Offs, SegDir.pTypDesc.nLen);
            fo.WriteLine("Array Descriptors:		{0:X8}, {1:X8}", SegDir.pArryDesc.Offs, SegDir.pArryDesc.nLen);
            fo.WriteLine("Custom Data:			{0:X8}, {1:X8}", SegDir.pCustData.Offs, SegDir.pCustData.nLen);
            fo.WriteLine("GUID Offsets:			{0:X8}, {1:X8}", SegDir.pCDGuids.Offs, SegDir.pCDGuids.nLen);
            fo.WriteLine("Unknown 02:				{0:X8}, {1:X8}", SegDir.Unk02.Offs, SegDir.Unk02.nLen);
            fo.WriteLine("Unknown 03:				{0:X8}, {1:X8}", SegDir.Unk03.Offs, SegDir.Unk03.nLen);
        }

        private unsafe void PrintTypelibHeader(byte[] cs, TextWriter fo, TlbHeader TlbHdr, MSFT_SegDir SegDir, int fName)
        {
            // --------------------------------------
            // print the TypeLib Header
            var enc = Encoding.UTF8;
            fo.WriteLine("** TypeLib Header ***********************");
            fo.WriteLine("Magic 1:		{0:X8} = {1}", TlbHdr.Magic1, enc.GetString(cs, 0, 4));  // 0x5446534D """MSFT"""
            fo.WriteLine("Magic 2:		{0:X8}", TlbHdr.Magic2); // 0x00010002 version number?

            // position of libid in guid table (should be, else -1)
            fo.WriteLine("GUID:			{0:X8} ==> {1}", TlbHdr.oGUID, tlGuid(cs, SegDir, TlbHdr.oGUID));

            fo.WriteLine("Locale ID:		{0:X8}", TlbHdr.LCID);
            fo.WriteLine("Locale ID 2:	{0:X8}", TlbHdr.LCID2);

            // fVar (largely unknown):
            // 	* the lower nibble is SysKind
            // 	* bit 5 is set if a helpfile is defined
            // 	* bit 8 is set if a help dll is defined
            fo.WriteLine("VarFlags:		{0:X8}", TlbHdr.fVar);
            fo.WriteLine("	System:		", (SysKind) ((TlbHdr.fVar & 7) + 1));
            fo.WriteLine("	Help file{0}specified", (TlbHdr.fVar & 16) != 0 ? " " : " not ");
            fo.WriteLine("	Help file is{0}in a DLL", (TlbHdr.fVar & 256) != 0 ? " " : " not ");


            fo.WriteLine("Version:	{0:X8}", TlbHdr.Version);
            fo.WriteLine("Flags:		{0:X8}", TlbHdr.Flags);

            fo.WriteLine("TypeInfo count:	{0:X8} == {0}", TlbHdr.nTypeInfo);   // number of TypeInfo's

            fo.WriteLine("HelpString:			{0:X8}", TlbHdr.HelpStr);    // position of HelpString in stringtable
            fo.WriteLine(" ==> {0}", tlString(cs, SegDir, TlbHdr.HelpStr));

            fo.WriteLine("HelpStringContext:	{0:X8}", TlbHdr.HelpStrCnt);

            fo.WriteLine("HelpContext:		{0:X8}", TlbHdr.HelpCntxt);

            fo.WriteLine("Name Table:");
            fo.WriteLine("	Names count:	{0:X8} == {0}", TlbHdr.nName);  // number of names in name table
            fo.WriteLine("	Characters:		{0:X8} == {0}", TlbHdr.nChars); // number of characters in name table

            fo.WriteLine();
            fo.WriteLine("TypeLib Name:	{0:X8} ==> {1}", TlbHdr.oName, tlName(cs, SegDir, TlbHdr.oName)); // offset of name in name table
            fo.WriteLine("Helpfile:		{0:X8} ==> {1}", TlbHdr.HelpFile, tlString(cs, SegDir, TlbHdr.HelpFile));// position of helpfile in stringtable

            // if -1 no custom data, else it is offset in custom data/guid offset table
            fo.WriteLine("Custom data offset:	{0:X8}", TlbHdr.CustDat);

            //If %Def(%Study)
            fo.WriteLine("Reserved1:	{0:X8}", TlbHdr.Res1);      // unknown always: 0x20 (guid hash size?)
            fo.WriteLine("Reserved2:	{0:X8}", TlbHdr.Res2);      // unknown always: 0x80 (name hash size?)
                                                                    //EndIf

            fo.WriteLine("IDispatch:			{0:X8}", TlbHdr.oDispatch); // hRefType to IDispatch, or -1 if no IDispatch
            fo.WriteLine("ImportInfo count:	{0:X8} == {0}", TlbHdr.nImpInfos); // number of ImpInfos

            int p = sizeof(TlbHeader);
            if (fName != 0)
            {
                int d = (int) CvDwd(cs, p);
                fo.WriteLine("TypeLib file name:			{0:X8} ==> {1}", d, tlString(cs, SegDir, d));
            }
        }

        private unsafe void PrintImportInfo(byte[] cs, TextWriter fo, in MSFT_SegDir SegDir)
        {
            // --------------------------------------
            // Print ImportInfo ?
            fo.WriteLine();
            fo.WriteLine("** ImportInfo ***********************");
            int i = 0;
            int p = SegDir.pImpInfo.Offs;
            int n = Math.Min(p + SegDir.pImpInfo.nLen, cs.Length);
            while (p < n)
            {
                ++i;
                LSet(out ImportInfo ImpInfo, cs, p);
                fo.WriteLine("--------------------------------------------");
                fo.WriteLine("Import Info number:	{0}", i);
                fo.WriteLine("Count:				{0}", ImpInfo.Count);
                fo.WriteLine("Offset in import file table:	{0:X8}", ImpInfo.oImpFile);

                if ((ImpInfo.Flags & 1) != 0)
                    fo.WriteLine("GUID:	{0:X8} ==> {1}", ImpInfo.oGuid, tlGuid(cs, SegDir, ImpInfo.oGuid));
                else
                    fo.WriteLine("TypeInfo index:	{0:X8} == {0}", ImpInfo.oGuid);

                fo.WriteLine("Type:	{0}", (TKind) ImpInfo.TypeKind);

                p = p + sizeof(ImportInfo);
            }
        }

        private unsafe int PrintReferenceTable(byte[] cs, TextWriter fo, MSFT_SegDir SegDir)
        {
            // --------------------------------------
            // Print References Table
            fo.WriteLine();
            fo.WriteLine("** References Table ***********************");
            int p = SegDir.pRefer.Offs;
            int n = p + SegDir.pRefer.nLen;
            while (p < n)
            {
                LSet(out RefRecord RefRec, cs, p);
                fo.WriteLine("--------------------------------------------");

                // if it// s a multiple of 4, it// s an offset in the TypeInfo table,
                // otherwise it// s an offset in the external reference table with
                // an offset of 1
                fo.WriteLine("Reference type:	{0:X8}", RefRec.RefType);
                if ((RefRec.RefType & 0x03) != 0)
                    fo.WriteLine("	==> (External Reference Table)");
                else
                    fo.WriteLine("	==> (TypeInfo)");

                fo.WriteLine("Flags:			{0:X8}", RefRec.Flags);
                if (RefRec.oCustData >= 0) fo.WriteLine("Custom data:	{0:X8}", RefRec.oCustData);
                if (RefRec.oNext >= 0) fo.WriteLine("Next offset:	{0:X8}", RefRec.oNext);
                p = p + sizeof(RefRecord);

            } // While p <= Len(cs)
            return p;
        }

        private static unsafe void PrintLibrTable(byte[] cs, TextWriter fo, MSFT_SegDir SegDir)
        {
            // --------------------------------------
            // Print "Lib Table"	(unknown format)
            // always exists, always the same size (0x80)
            // ...hash table with offsets to GUID?
            fo.WriteLine("** Lib Table (offsets into GUID table) ***********************");

            int p = SegDir.pLibs.Offs;
            int n = p + SegDir.pLibs.nLen;
            while (p < n)
            {
                int d = Cvl(cs, p);
                if (d >= 0) fo.WriteLine("{0:X8}", d);
                p = p + 4;

            } // While p <= Len(cs)
        }

        private unsafe void PrintTypeDescriptors(byte[] cs, TextWriter fo, MSFT_SegDir SegDir)
        {
            // --------------------------------------
            // Print Type Descriptors
            if (SegDir.pTypDesc.nLen != 0)
            {
                fo.WriteLine();
                fo.WriteLine("** Type Descriptors ***********************");
                int p = SegDir.pTypDesc.Offs;
                int n = p + SegDir.pTypDesc.nLen;
                while (p < n)
                {
                    fo.WriteLine("-------------------------");
                    fo.WriteLine("Raw:	{0:X8}{1:X8}", CvDwd(cs, p), CvDwd(cs, p + 4));
                    LSet(out TYPEDESC TypDsc, cs, p);

                    fo.WriteLine("Data type 1 = {0:X4}{1:X4}", TypDsc.v2, TypDsc.v1);
                    if ((TypDsc.v2 & 0x7FFE) == 0x7FFE)
                    {
                        fo.WriteLine(" = ");
                    }
                    else if ((TypDsc.v2 & VT_Vector) != 0)
                    {
                        fo.WriteLine(" = VT_Vector, ");
                    }
                    else if ((TypDsc.v2 & VT_Array) != 0)
                    {
                        fo.WriteLine(" = VT_Array, ");
                    }
                    else if ((TypDsc.v2 & VT_ByRef) != 0)
                    {
                        fo.WriteLine(" = VT_ByRef, ");
                        // ElseIf (TypDsc.v2 And %VT_Reserved) Then
                    }
                    else
                    {
                        fo.WriteLine(" = ");
                    }

                    fo.WriteLine(VarType(TypDsc.v1));

                    if ((TypDsc.v2 & 0x7FFE) == 0x7FFE)
                    {
                        fo.WriteLine();
                    }
                    else
                    {
                        fo.WriteLine(" == base type: {0}", VarType(TypDsc.v2 & VT_TypeMask));
                    }


                    if (LoByt(TypDsc.v1) == VT_Ptr || LoByt(TypDsc.v1) == VT_SafeArray)
                    {

                        if (TypDsc.v4 < 0)
                        {  // offset into type descriptor table
                            fo.WriteLine("Type descriptor table offset:	{0:X4}", TypDsc.v3 & 0x07FF8);
                        }
                        else
                        { // file offset to type descriptor
                          //  This doesn'tt sound sensible, but it looks like what the ReactOS code was doing.
                            int d = (TypDsc.v2 << 16) | TypDsc.v3;
                            fo.WriteLine("Type descriptor file offset:	{0:X8}", (d & (TypDsc.v3 / 8)));
                        }

                    }
                    else if (LoByt(TypDsc.v1) == VT_CArray)
                    {
                        fo.WriteLine("Array descriptor offset:	{0:X8}", MakLng(TypDsc.v3, TypDsc.v4));
                    }
                    else if (LoByt(TypDsc.v1) == VT_UserDefined)
                    {
                        fo.WriteLine("Type descriptor offset:		{0:X8}", MakLng(TypDsc.v3, TypDsc.v4) & 0x0FFFFFFF8);
                    }

                    p = p + sizeof(tagTYPEDESC);

                } //Loop While p <= Len(cs)

            }
        }

        private static unsafe void PrintStringTable(byte[] cs, TextWriter fo, in MSFT_SegDir SegDir)
        {
            // --------------------------------------
            // Print String Table
            // every entry, length-plus-text, is a minimum of eight bytes
            // (this keeps the offset in p zero-based)
            fo.WriteLine();
            fo.WriteLine("** String Table ***********************");
            int p = SegDir.pStrings.Offs;
            int n = p + SegDir.pStrings.nLen;
            int i = 1; // string number
            while (p < n)
            {
                int d = CvWrd(cs, p);
                fo.WriteLine("{0}    {1}	", i, Encoding.ASCII.GetString(cs, p + 2, d));
                // advance to next Dword-aligned offset beyond minimum length of eight bytes
                p = (p + Math.Max(d + 2, 8) + 3) & ~3;
                ++i;
            } // Loop While p <= Len(cs)
        }

        private static unsafe void PrintUnknown01(byte[] cs, TextWriter fo, in MSFT_SegDir SegDir)
        {
            // --------------------------------------
            // Print "Unknown 01" (length is "always" 0x200)
            fo.WriteLine();
            fo.WriteLine("*************************");
            fo.WriteLine("\"Unknown 01\" --- Table with Offsets into the Name Table");
            fo.WriteLine("*************************");
            int p = SegDir.Unk01.Offs;
            int n = p + SegDir.Unk01.nLen;
            while (p < n)
            {
                int d = Cvl(cs, p);
                if (d >= 0) fo.WriteLine("{0:X8}", d);
                p = p + 4;
            } // While p <= Len(cs)
        }

        private unsafe void PrintNameTable(byte[] cs, TextWriter fo, MSFT_SegDir SegDir)
        {
            // --------------------------------------
            // Print Name Table
            // (this keeps the offset in p zero-based)
            fo.WriteLine();
            fo.WriteLine("*************************");
            fo.WriteLine("Name Table");
            fo.WriteLine("*************************");
            int p = SegDir.pNames.Offs;
            int n = p + SegDir.pNames.nLen;
            while (p < n)
            {
                LSet(out NameIntro NameInt, cs, p);
                fo.WriteLine("--------------------------------------------");
                if (NameInt.hRefType != -1) fo.WriteLine("Offset:			{0:X8}", NameInt.hRefType);
                fo.WriteLine("Next hash:		{0:X8}", NameInt.NextHash);
                int d = LoByt(NameInt.cName);
                fo.WriteLine("Name length:	{0:X2} == {0}", d);
                fo.WriteLine("Flags?...		{0:X2}", HiByt(LoWrd(NameInt.cName)));
                fo.WriteLine("Hash code:		{0:X4}", HiWrd(NameInt.cName));

                p = p + sizeof(NameIntro);
                fo.WriteLine("\"{0}\"", Encoding.ASCII.GetString(cs, p, d));
                p = (p + d + 3) & ~3; // advance to next DWord-aligned offset
            }
            //Loop While p <= Len(cs)
        }

        private static uint CvDwd(byte[] cs, int offset)
        {
            return ByteMemoryArea.ReadLeUInt32(cs, offset);
        }

        private static int Cvl(byte[] cs, int offset)
        {
            return ByteMemoryArea.ReadLeInt32(cs, offset);

        }

        private static ushort CvWrd(byte[] cs, int offset)
        {
            return ByteMemoryArea.ReadLeUInt16(cs, offset);
        }

        private static byte LoByt(long n) => (byte) n;
        private static ushort LoWrd(long n) => (ushort) n;
        private static byte HiByt(long n) => (byte) (n >> 8);
        private static ushort HiWrd(long n) => (ushort) (n >> 16);

        private static uint MakLng(short lo, short hi) =>
            ((uint) lo) | (uint) ((uint)hi << 16);
#endif
    }
}
