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

#if VISUALBASIC
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.TypeLib
{
    public class MSFT
    {
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

public const int HELPDLLFLAG	= 0x0100;


// ****************************************
// 	"Magic" Values
// ****************************************
public const int MSFT_SIGNATURE		= 0x05446534D; //   "MSFT"
public const int SLTG_SIGNATURE		= 0x047544C53; //   "SLTG"


// ****************************************
// 	Equates for Translating Flags
// 		and Codes to Text
// ****************************************
public enum SysKind		    { Win16,Win32,Macintosh }
public enum VarFlags		{ ReadOnly,Source,Bindable,RequestEdit,DisplayBind,DefaultBind,Hidden,Restricted,DefaultCollelem,UiDefault,NonBrowsable,Replaceable,ImmediateBind }
public enum TKind			{ Enum,Record,Module,Interface,Dispatch,Coclass,Alias,Union,Max }
public enum TypeFlags		{ AppObject,CanCreate,Licensed,PredeclId,Hidden,Control,Dual,Nonextensible,Oleautomation,Restricted,Aggregatable,Replaceable,Dispatchable,ReverseBind }
public enum ParamFlags		{ In,Out,LCID,RetVal,Opt,HasDefault,HasCustData }
public enum CallConv		{ FastCall,CDecl,Pascal,MacPascal,StdCall,FPFastCall,SysCall,MPWCDecl,MPWPascal,Max }
public enum InvoKind		{ Func,PropertyGet,PropertyPut,PropertyPutRef }
public enum FuncKind		{ Virtual,PureVirtual,NonVirtual,Static,Dispatch}


// ****************************************
// 	Variable-Type Codes, Masks and Flags
// ****************************************

public const int VT_Empty			= 0;
public const int VT_Null			= 1;
public const int VT_I2				= 2;
public const int VT_I4				= 3;
public const int VT_R4				= 4;
public const int VT_R8				= 5;
public const int VT_Cy				= 6;
public const int VT_Date			= 7;
public const int VT_BStr			= 8;
public const int VT_Dispatch		= 9;
public const int VT_Error			= 10;
public const int VT_Bool			= 11;
public const int VT_Variant			= 12;
public const int VT_Unknown			= 13;
public const int VT_Decimal			= 14;
public const int VT_I1				= 16;
public const int VT_UI1				= 17;
public const int VT_UI2				= 18;
public const int VT_UI4				= 19;
public const int VT_I8				= 20;
public const int VT_UI8				= 21;
public const int VT_Int				= 22;
public const int VT_UInt			= 23;
public const int VT_Void			= 24;
public const int VT_HResult			= 25;
public const int VT_Ptr				= 26;
public const int VT_SafeArray		= 27;
public const int VT_CArray			= 28;
public const int VT_UserDefined		= 29;
public const int VT_LPStr			= 30;
public const int VT_LPWStr			= 31;
public const int VT_Record			= 36;
public const int VT_FileTime		= 64;
public const int VT_Blob			= 65;
public const int VT_Stream			= 66;
public const int VT_Storage			= 67;
public const int VT_Streamed_Object	= 68;
public const int VT_Stored_Object	= 69;
public const int VT_Blob_Object		= 70;
public const int VT_CF				= 71;
public const int VT_ClsID			= 72;

// 	flags
public const int VT_Bstr_Blob		= 0x00FFF;
public const int VT_Vector			= 0x01000;
public const int VT_Array			= 0x02000;
public const int VT_ByRef			= 0x04000;
public const int VT_Reserved		= 0x08000;

// 	masks
public const int VT_Illegal			= 0x0FFFF;
public const int VT_IllegalMasked	= 0x00FFF;
public const int VT_TypeMask		= 0x00FFF;


// ****************************************
// 	Calling Conventions
// ****************************************
public const int CC_FASTCALL			= 0;
public const int CC_CDECL				= 1;
public const int CC_MSCPASCAL			= 2;
public const int CC_PASCAL				= 2;
public const int CC_MACPASCAL			= 3;
public const int CC_STDCALL				= 4;
public const int CC_FPFASTCALL			= 5;
public const int CC_SYSCALL				= 6;
public const int CC_MPWCDECL			= 7;
public const int CC_MPWPASCAL			= 8;
public const int CC_MAX					= 9;


// ****************************************
// 	Function Types
// ****************************************
public const int FUNC_VIRTUAL			= 0;
public const int FUNC_PUREVIRTUAL		= 1;
public const int FUNC_NONVIRTUAL		= 2;
public const int FUNC_STATIC			= 3;
public const int FUNC_DISPATCH			= 4;


// ****************************************
// 	Function Flags
// ****************************************
public const int FUNCFLAG_FRESTRICTED		= 0x00001;
public const int FUNCFLAG_FSOURCE			= 0x00002;
public const int FUNCFLAG_FBINDABLE			= 0x00004;
public const int FUNCFLAG_FREQUESTEDIT		= 0x00008;
public const int FUNCFLAG_FDISPLAYBIND		= 0x00010;
public const int FUNCFLAG_FDEFAULTBIND		= 0x00020;
public const int FUNCFLAG_FHIDDEN			= 0x00040;
public const int FUNCFLAG_FUSESGETLASTERROR	= 0x00080;
public const int FUNCFLAG_FDEFAULTCOLLELEM	= 0x00100;
public const int FUNCFLAG_FUIDEFAULT		= 0x00200;
public const int FUNCFLAG_FNONBROWSABLE		= 0x00400;
public const int FUNCFLAG_FREPLACEABLE		= 0x00800;
public const int FUNCFLAG_FIMMEDIATEBIND  	= 0x01000;
#if MAC
public const int FUNCFLAG_FORCELONG			= 2147483647;
#endif


// ****************************************
// 	Invocation Kinds
// **************************************** 
public const int INVOKE_FUNC			= 1 ;
public const int INVOKE_PROPERTYGET		= 2 ;
public const int INVOKE_PROPERTYPUT		= 4 ;
public const int INVOKE_PROPERTYPUTREF	= 8 ;


// ****************************************
// 	Parameter Flags
// ****************************************
public const int PARAMFLAG_NONE			= 0x000;
public const int PARAMFLAG_FIN			= 0x001;
public const int PARAMFLAG_FOUT			= 0x002;
public const int PARAMFLAG_FLCID		= 0x004;
public const int PARAMFLAG_FRETVAL		= 0x008;
public const int PARAMFLAG_FOPT			= 0x010;
public const int PARAMFLAG_FHASDEFAULT	= 0x020;
public const int PARAMFLAG_FHASCUSTDATA	= 0x040;


// ****************************************
// 	System Kind
// ****************************************
public const int SYS_WIN16		= 0;
public const int SYS_WIN32		= 1;
public const int SYS_MAC		= 2;

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
public const int TKIND_ENUM			= 0;
public const int TKIND_RECORD		= 1;
public const int TKIND_MODULE		= 2;
public const int TKIND_INTERFACE	= 3;
public const int TKIND_DISPATCH		= 4;
public const int TKIND_COCLASS		= 5;
public const int TKIND_ALIAS		= 6;
public const int TKIND_UNION		= 7;
public const int TKIND_MAX			= 8;


// ****************************************
// 	Type Flags
// ****************************************
public const int TYPEFLAG_FAPPOBJECT		= 0x00001;
public const int TYPEFLAG_FCANCREATE		= 0x00002;
public const int TYPEFLAG_FLICENSED			= 0x00004;
public const int TYPEFLAG_FPREDECLID		= 0x00008;
public const int TYPEFLAG_FHIDDEN			= 0x00010;
public const int TYPEFLAG_FCONTROL			= 0x00020;
public const int TYPEFLAG_FDUAL				= 0x00040;
public const int TYPEFLAG_FNONEXTENSIBLE	= 0x00080;
public const int TYPEFLAG_FOLEAUTOMATION	= 0x00100;
public const int TYPEFLAG_FRESTRICTED		= 0x00200;
public const int TYPEFLAG_FAGGREGATABLE		= 0x00400;
public const int TYPEFLAG_FREPLACEABLE		= 0x00800;
public const int TYPEFLAG_FDISPATCHABLE		= 0x01000;
public const int TYPEFLAG_FREVERSEBIND		= 0x02000;
public const int TYPEFLAG_MASK				= TYPEFLAG_FREVERSEBIND - 1;

// ****************************************
// 	Variable Kinds
// ****************************************
// not sure if these are ever used in MSFT format data
// %VAR_PERINSTANCE	= 0
// %VAR_STATIC		= %VAR_PERINSTANCE + 1
// %VAR_CONST			= %VAR_STATIC + 1
// %VAR_DISPATCH		= %VAR_CONST + 1
public const int VAR_PERINSTANCE	= 0;
public const int VAR_STATIC			= 1;
public const int VAR_CONST			= 2;
public const int VAR_DISPATCH		= 3;


// ****************************************
// 	Variable Flags
// ****************************************
public const int VARFLAG_FREADONLY			= 0x00001;
public const int VARFLAG_FSOURCE			= 0x00002;
public const int VARFLAG_FBINDABLE			= 0x00004;
public const int VARFLAG_FREQUESTEDIT		= 0x00008;
public const int VARFLAG_FDISPLAYBIND		= 0x00010;
public const int VARFLAG_FDEFAULTBIND		= 0x00020;
public const int VARFLAG_FHIDDEN			= 0x00040;
public const int VARFLAG_FRESTRICTED		= 0x00080;
public const int VARFLAG_FDEFAULTCOLLELEM	= 0x00100;
public const int VARFLAG_FUIDEFAULT			= 0x00200;
public const int VARFLAG_FNONBROWSABLE		= 0x00400;
public const int VARFLAG_FREPLACEABLE		= 0x00800;
public const int VARFLAG_FIMMEDIATEBIND		= 0x01000;




// ****************************************
// 	TypeLib UDTs
// ****************************************


struct TYPEDESCUNION
{
	public uint lptdesc; // TYPEDESC Ptr
	public uint lpadesc; // ARRAYDESC Ptr
	public int hRefType; // hRefType
} // TYPEDESCUNION


public class tagTYPEDESC
{
	public TYPEDESCUNION u;
	public int vt; // VARTYPE
} // TYPEDESC


public class tagPARAMDESCEX
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


struct ELEMDESCUNION
{
	public tagIDLDESC idldesc;	// info for remoting the element
	public tagPARAMDESC ParamDesc;	// info about the parameter
}


public class tagELEMDESC
{
	public tagTYPEDESC tdesc; // the type of the element
	public ELEMDESCUNION u;
} // tagELEMDESC



// 	MSFT typelibs
// These are TypeLibs created with ICreateTypeLib2 structure of the typelib type2 header
// it is at the beginning of a type lib file
public class  TlbHeader
{
	public uint Magic1	; // 0x5446534D "MSFT"								00
	public uint Magic2	; // 0x00010002 version number?
	public uint oGUID	; // position of libid in GUID table (should be,  else -1)
	public uint LCID	; // locale id
	public uint LCID2	; // 												10
	public uint fVar	; // (largely) unknown flags
				  				// * the lower nibble is SysKind
				  				// * bit 5 is set if a helpfile is defined
				  				// * bit 8 is set if a help dll is defined

	uint Version	;		 // set with SetVersion()
	uint Flags		;		 // set with SetFlags()
	uint nTypeInfo	;		 // number of TypeInfo// s							20
	uint HelpStr	;		 // offset to help string in string table
	uint HelpStrCnt	;		
	uint HelpCntxt	;		
	uint nName		;		 // number of names in name table					30
	uint nChars		;		 // characters in name table
	uint oName		;		 // offset of name in string table
	uint HelpFile	;		 // offset of helpfile in string table
	uint CustDat	;		 // if -1 no custom data, else it is offset		40
						 	// in custom data/GUID offset table
	uint Res1		;		 // unknown always: 0x20 (GUID hash size?)
	uint Res2		;		 // unknown always: 0x80 (name hash size?)
	uint oDispatch	;		 // hRefType to IDispatch, or -1 if no IDispatch
	uint nImpInfos	;		 // number of ImpInfos								50
	// oFileName			As Long // offset to typelib file name in string table
    } // TlbHeader


// segments in the type lib file have a structure like this:
public class SegDesc
{
	public uint Offs	; // absolute offset in file
	public uint nLen	; // length of segment
	public uint Res01	; // unknown always -1
	public uint Res02	; // unknown always 0x0F in the header
					 // 0x03 in the TypeInfo_data
}

// 	segment directory
public class MSFT_SegDir
{
		public SegDesc	pTypInfo	; // 1 - TypeInfo table
		public SegDesc	pImpInfo	; // 2 - table with info for imported types
		public SegDesc	pImpFiles	; // 3 - import libaries
		public SegDesc	pRefer		; // 4 - References table
		public SegDesc	pLibs		; // 5 - always exists, alway same size (0x80)
		 							  //   - hash table with offsets to GUID;;?
		public SegDesc	pGUID		; // 6 - all GUIDs are stored here together with
		 							  //   - offset in some table;;
		public SegDesc	Unk01		; // 7 - always created, always same size (0x0200)
									  //   - contains offsets into the name table
		public SegDesc	pNames		; // 8 - name table
		public SegDesc	pStrings	; // 9 - string table
		public SegDesc	pTypDesc	; // A - type description table
		public SegDesc	pArryDesc	; // B - array descriptions
		public SegDesc	pCustData	; // C - data table, used for custom data and default
  			    					  //   - parameter values
		public SegDesc	pCDGuids	; // D - table with offsets for the GUIDs and into
		    						  //   - the custom data table
		public SegDesc	Unk02		; // E - unknown
		public SegDesc	Unk03		; // F - unknown
} // MSFT_SegDir


// type info data
public class  TypeInfo {
	public Byte	TypeKind	;		// TKIND_xxx
	public Byte	Align		;		// alignment
	public short Unk		;		// unknown
	public int oFunRec		;		// 	- points past the file, if no elements
	public int Alloc		;		// 	Recommended (or required?) amount of memory to allocate for...?
	public int Reconst		;		// 	size of reconstituted TypeInfo data
	public int Res01		;		// 10 - always? 3
	public int Res02		;		// 	- always? zero
	public short nFuncs		;	     //  - count of functions
	public short nProps		;		//  - count of properties
	public int Res03		;		//    - always? zero
	public int Res04		;		// 20 - always? zero
	public int Res05		;		//    - always? zero
	public int Res06		;		//    - always? zero
	public int oGUID		;		//    - position in GUID table
	public int fType		;		// 30 - Typeflags
	public int oName		;		//    - offset in name table
	public int Version		;		//    - element version
	public int	DocStr		;		//    - offset of docstring in string tab
	public int HelpStrCnt	;		// 40
	public int HelpCntxt	;		
	public int	oCustData	;		//    - offset in custom data table
#if WORDS_BIGENDIAN		// 
	public short cVft; // virtual table size, not including inherits
	public short nImplTypes; // number of implemented interfaces
#else
	public short nImplTypes;        // number of implemented interfaces
	public short cVft;              // virtual table size, not including inherits
#endif
	public int Unk03		; // 50 - size in bytes, at least for structures

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
public class ImportInfo
{
	public short Count; // count
	public Byte Flags; // if <> 0 then oGUID is an offset to GUID, else it// s a TypeInfo index in the specified typelib
	public Byte TypeKind;	//  TKIND of reference
	public int oImpFile; // offset in the Import File table
	public int oGuid; // offset in GUID table or TypeInfo index (see bit 16 of Res0)
} // ImportInfo



// information on imported files
public class TlbImpLib
{
	public int oGUID;
	public int LCID;
	public ushort MajVer;
	public ushort MinVer;
	public ushort cSize; // divide by 4 to get the length of the file name
} // TlbImpLib



// Structure of the reference data
public class RefRecord
{
	// either offset in TypeInfo table, then it// s a multiple of 4...
	// or offset in the external reference table with an offset of 1
	public int RefType;
    public int Flags;				 // ?
	public int oCustData; // custom data
	public int oNext; // next offset, -1 if last
} // RefRecord



// this is how a GUID is stored
public class  GuidEntry
{
	public Guid oGUID;
	//  = -2 for a TypeLib GUID
	// TypeInfo offset for TypeInfo GUID,
	// Otherwise, the low two bits:
	// 	= 01 for an imported TypeInfo
	// 	= 10 for an imported TypeLib (used by imported TypeInfos)
	public int hRefType;
	public int NextHash;	// offset to next GUID in the hash bucket
}// GuidEntry



// some data preceding entries in the name table
public class NameIntro {
	// is -1 if name is for neither a TypeInfo,
	// a variable, or a function (that is, name
	// is for a typelib or a function parameter).
	// otherwise is the offset of the first
	// TypeInfo that this name refers to (either
	// to the TypeInfo itself or to a member of
	// the TypeInfo
	public int hRefType;

	public int NextHash;	// offset to next name in the hash bucket

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



public class TYPEDESC // a substitute for a tagTYPEDESC to simplify the code
{
	public short v1;
	public short v2;
	public short v3;
	public short v4;
} // TYPEDESC



// type for arrays
public class SafeArrayBound
{
	public uint nElements;
	public int lLBound;
}// SafeArrayBound

public class ARRAYDESC {
	public TYPEDESCUNION u;
	public ushort nDims;
	public ushort tVar; // VARTYPE
	public SafeArrayBound [] Bounds;
} // ARRAYDESC

public class CArrayDesc {
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
public class FuncRecord
{
	ushort RecSize;		// record size, including optional fields and ParamInfo// s
	ushort Unk1;			//  zero-based function number ?
	ushort DataType	; 		 // data type returned by the function

	// If the .Flags MSB = 1, then the low byte is valid. So far it seems
	// to always be valid, except for pointers. When MSB = 1, the low byte
	// is the code for a data type that's equivalent to or compatible with
	// that in .DataType.
	ushort Flags;
	public int Res1; // always(?) zero

#if WORDS_BIGENDIAN
	public short cFuncDesc; // size of reconstituted FUNCDESC and related structs
	public short oVtable; // offset in vtable
#else
	public short oVtable;				// offset in vtable
	public short cFuncDesc			;// size of reconstituted FUNCDESC and related structs
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
	uint FKCCIC;

	short nParams; // parameter count
	short Unk2;

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
public class  ParamInfo
{
	ushort DataType;
	ushort Flags	;
	uint oName		;
	uint fParam		;
} // ParamInfo



// 	Property description data
//  The size of the required fields of the PropRecord structure
// is 20 (= 0x14)
// These exist in arrays along with zero or more "FuncRecord"
// elements. Each array is preceded by a Dword stating the total
// size of the array.
public class PropRecord
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

public string Locale(int lcid ) {

    var locales = new [] {
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

int n;

    return 
        locales.Where(l >= l.Lcid == lcid)
            .Select(l >= l.Desc)
            .FirstOrDefault();

} // Locale

// ****************************************

string VarType(int pn) {
int n;
String ls;

//Data "Empty=0"
//Data "Null=1"
//Data "I2=2"
//Data "I4=3"
//Data "R4=4"
//Data "R8=5"
//Data "Cy=6"
//Data "Date=7"
//Data "BStr=8"
//Data "Dispatch=9"
//Data "Error=10"
//Data "Bool=11"
//Data "Variant=12"
//Data "Unknown=13"
//Data "Decimal=14"
//Data
//Data "I1=16"
//Data "UI1=17"
//Data "UI2=18"
//Data "UI4=19"
//Data "I8=20"
//Data "UI8=21"
//Data "Int=22"
//Data "UInt=23"
//Data "Void=24"
//Data "HResult=25"
//Data "Ptr=26"
//Data "SafeArray=27"
//Data "CArray=28"
//Data "UserDefined=29"
//Data "LPStr=30"
//Data "LPWStr=31"
//Data , , ,
//Data "Record=36"
// 	end of continuous sequence
//Data "FileTime=64"
//Data "Blob=65"
//Data "Stream=66"
//Data "Storage=67"
//Data "Streamed_Object=68"
//Data "Stored_Object=69"
//Data "Blob_Object=70"
//Data "CF=71"
//Data "ClsID=72"
// Data "Bstr_Blob=4095"

	pn = pn & VT_TypeMask;
	if (pn <= 36 )
		n = pn + 1;
	else {
		For n = 37 To DataCount
			If Val(Remain(Read(n), "=")) = pn break;
		Next n
	}

	ls = Extract$(Read$(n), "=");
	return IIf$(Len(ls), "VT_" & ls, "(Unknown)");

} // VarType

// ****************************************
// offs = zero-based offset into the GUID table
public String tlGuid(string cs, MSFT_SegDir SegDir, int offs ) {

	if (offs >= 0)
		return GuidTxt$(Mid$(cs, SegDir.pGUID.Offs + offs + 1, 16));
	else
        return null;

} // tlGuid

// ****************************************
// offs = zero-based offset into the name table
public string tlName(byte[]cs, MSFT_SegDir SegDir, int offs) {
NameIntro NameInt;

	if (offs >= 0 ) {
		offs = SegDir.pNames.Offs + offs + 1;
		LSet NameInt = Mid$(cs, offs);
		Function = $Dq & Mid$(cs, offs + SizeOf(NameIntro), NameInt.cName & 0x0FF) & $Dq;
	}

} // tlName

// ****************************************
// offs = zero-based offset into the string table
public String tlString(byte [] cs , MSFT_SegDir SegDir , int offs ) {

	if( offs >= 0 && offs < cs.Length){
		offs = SegDir.pStrings.Offs + offs + 1;
		Function = $Dq + Mid$(cs, offs + 2, CvWrd(cs, offs)) + $Dq;
	}

} // tlString


// ****************************************
// cs:		TypeLib data
// SegDir:	the segment directory
// pBase:		the zero-based offset to the FuncRec
// nFunc:		number of functions
// nProp:		number of properties
// fo:		file handle
#if DISPLAY
public Long DisFunction(cs As String, SegDir As MSFT_SegDir, ByVal pBase As Dword, ByVal nFunc As Long, ByVal nProp As Long) {
Long d;
Long i;
Long j;
Long n;
Long ub;
Dword p;
Dword pTmp;
Long iElem;
Long nAttr;
Long ArraySize;
Long oParamInfo;
Long oDefValue;

String ls;
FuncRecord FuncRec;
ParamInfo ParmInfo;
tagELEMDESC ElemDesc;
tagPARAMDESC ParamDesc;
NameIntro NameInt;
PropRecord PropRec;


	p = pBase
	ArraySize = CvDwd(cs, pBase + 1)
	fo.WriteLine(,
	fo.WriteLine(, "	Function record array size:	"; Hex$(ArraySize, 8)
	p = p + 5 // advance past the "ArraySize" value

	// ----------------------------------------
	// 	Other function and property data
	n = nFunc + nProp
	If n > 0 Then
		ub = n - 1
		pTmp = StrPtr(cs) + pBase + ArraySize + 4
		ReDim IdElem(ub)	As Long At pTmp

		pTmp = pTmp + (4 * n)
		ReDim oName(ub)		As Long At pTmp

		pTmp = pTmp + (4 * n)
		ReDim Refer(ub)		As Long At pTmp
	End If


	If nFunc Then

		fo.WriteLine(,
		fo.WriteLine(, "	----------------------------------------"
		fo.WriteLine( "				Functions:"

		For i = 1 To nFunc

			LSet FuncRec = Mid$(cs, p)

			fo.WriteLine( "	----------------------------------------"
			fo.WriteLine( "	ID:			"; Hex$(IdElem(iElem), 8); " =="; Str$(IdElem(iElem))
			fo.WriteLine( "	Name:		"; Hex$(oName(iElem), 8); " ==> "; tlName(cs, SegDir, oName(iElem))
			fo.WriteLine( "	Reference:	"; Hex$(Refer(iElem), 8) // offset to the corresponding function record
			Incr iElem
			fo.WriteLine( "	Record size:	"; Hex$(FuncRec.RecSize, 4)
			fo.WriteLine( "	Unknown 1:		"; Hex$(FuncRec.Unk1, 4)
			fo.WriteLine( "	Flags:			"; Hex$(FuncRec.Flags, 4); IIf$(FuncRec.Flags < 0, " = " & VarType(FuncRec.Flags), "")
			fo.WriteLine( "	DataType:		"; Hex$(FuncRec.DataType, 4); " = " & VarType(FuncRec.DataType)

#if Study
			fo.WriteLine(
			fo.WriteLine( "	Reserved 1:		"; Hex$(FuncRec.Res1, 8)
#endif

			fo.WriteLine( "	Vtable offset:	"; Hex$(FuncRec.oVtable, 4)
			fo.WriteLine( "	Func Desc Size:	"; Hex$(FuncRec.cFuncDesc, 4)


			// 	FKCCIC
			// The bits in FKCCIC have the following meanings:
			// 0 - 2 = function kind (eg virtual)
			// 3 - 6 = Invocation kind
			// 7 means custom data is present
			// 8 - 11 = calling convention
			// 12 means one or more parameters have a default value
			fo.WriteLine(
			fo.WriteLine( "	FKCCIC (raw):	"; Hex$(FuncRec.FKCCIC, 8)
			If (FuncRec.FKCCIC And 0x01000) Then fo.WriteLine( "		Default value(s) present"
			If (FuncRec.FKCCIC And 0x040000) Then fo.WriteLine( "		oEntry is numeric"
			If (FuncRec.FKCCIC And 0x080) Then fo.WriteLine( "		Custom data present"

			d = FuncRec.FKCCIC And 0x0F00
			Shift Right d, 8
			fo.WriteLine( "		Calling convention:	"; Hex$(d, 2); " = " & Parse$($CallConv, d + 1)

			d = FuncRec.FKCCIC And 0x078 // this is a bit field
			Shift Right d, 3
			fo.WriteLine( "		Invocation kind:	"; Hex$(d, 2); " = ";
			For n = 4 To 1 Step -1
				If (d And %INVOKE_PROPERTYPUTREF) Then
					fo.WriteLine( Parse$($InvoKind, n);
					If (d And 0x07) Then fo.WriteLine( ", ";
				End If
				Shift Left d, 1
			Next n
			fo.WriteLine(

			d = FuncRec.FKCCIC And 7
			fo.WriteLine( "		Function kind:		"; Hex$(d, 2); " = " & Parse$($FuncKind, d + 1)


			// 	Algorithm
			// 1) Dim the ParamInfo array at the end of the available space
			// 2) If (FKCCIC And 0x1000) then Dim an array of default values just before the ParamInfo array
			// 3) Assume anything preceding the above arrays is the function// s optional data
			fo.WriteLine(
			n = FuncRec.nParams
			fo.WriteLine( "	Number of parameters:	"; Hex$(n, 4); " =="; Str$(n)

//#If %Def(%Study)
			fo.WriteLine( "	Unknown 2:		"; Hex$(FuncRec.Unk2, 4)
//#EndIf

			oParamInfo = p + FuncRec.RecSize - (n * SizeOf(ParamInfo)) // must be one-based
			oDefValue = oParamInfo

			// If (FuncRec.FKCCIC And 0x01000) Then // there might be default values present
			If (FuncRec.FKCCIC And 0x01000) And (n > 0) Then // there might be default values present
				oDefValue = oDefValue - (n * 4)
				ReDim DefVal(n - 1)		As Long At StrPtr(cs) + oDefValue - 1 // must be zero-based
			End If


			// Dim array for the function// s optional data, if any
			ub = (((oDefValue - SizeOf(FuncRecord)) - p) \ 4) - 1
			If ub >= 0 Then

				fo.WriteLine( "		----------------------------------------"
				fo.WriteLine( "		Optional Data:"
				ReDim OptData(ub)	As Long At StrPtr(cs) + p + SizeOf(FuncRecord) - 1 // must be zero-based

				fo.WriteLine( "		HelpContext:		"; Hex$(OptData(0), 8)
				If ub < 1 Then Exit If

				fo.WriteLine( "		HelpString:			"; Hex$(OptData(1), 8);
				fo.WriteLine( " ==> " & tlString(cs, SegDir, OptData(1))
				If ub < 2 Then Exit If

				fo.WriteLine( "		Entry:				"; Hex$(OptData(2), 8)

//#If %Def(%Study)
				If ub < 3 Then Exit If

				fo.WriteLine( "		Reserved09:	"; Hex$(OptData(3), 8)
				If ub < 4 Then Exit If

				fo.WriteLine( "		Reserved0A:	"; Hex$(OptData(4), 8)
//#EndIf
				If ub < 5 Then Exit If

				fo.WriteLine( "		HelpStringContext:	"; Hex$(OptData(5), 8)
				If ub < 6 Then Exit If

				fo.WriteLine( "		Custom Data:		"; Hex$(OptData(6), 8)

			End If
			fo.WriteLine(


			For j = 0 To n - 1

				LSet ParmInfo = Mid$(cs, oParamInfo)
				fo.WriteLine( "		----------------------------------------"
				fo.WriteLine( "		Parameter number:	"; Str$(j + 1)
				fo.WriteLine( "		DataType:			"; Hex$(ParmInfo.DataType, 4); IIf$(ParmInfo.DataType >= 0, " = " & VarType(ParmInfo.DataType), "")
				fo.WriteLine( "		Flags:				"; Hex$(ParmInfo.Flags, 4); " = "; VarType(ParmInfo.Flags)
				fo.WriteLine( "		Name:				"; Hex$(ParmInfo.oName, 8); " ==> "; tlName(cs, SegDir, ParmInfo.oName)
				fo.WriteLine( "		ParamFlags:			"; Hex$(ParmInfo.fParam, 8); " =	";
				If ParmInfo.fParam Then
					d = ParmInfo.fParam
					For n = 7 To 1 Step -1 //  7 = ParseCount($ParamFlags)
						If (d And %PARAMFLAG_FHASCUSTDATA) Then
							fo.WriteLine( Parse$($ParamFlags, n);
							If (d And 0x03F) Then fo.WriteLine( ", ";
						End If
						Shift Left d, 1
					Next n
					fo.WriteLine(
				Else
					fo.WriteLine( "(none)"
				End If

				// If (ParmInfo.fParam And %PARAMFLAG_FHASDEFAULT) Then
				If (UBound(DefVal) >= 0) And (ParmInfo.fParam And %PARAMFLAG_FHASDEFAULT) Then

					If DefVal(j) < 0 Then // the default value is in the lower three bytes
						DefVal(j) = DefVal(j) And 0x0FFFFFF
					Else // it// s an offset into the CustomData table
						DefVal(j) = Cvl(cs, SegDir.pCustData.Offs + DefVal(j) + 3)
					End If
					fo.WriteLine( "		Default Value:		"; Hex$(DefVal(j), 8); " =="; Str$(DefVal(j))

				End If

				oParamInfo = oParamInfo + SizeOf(ParamInfo)

			Next j

			p = p + FuncRec.RecSize

		Next i

	End If // nFunc



	// do the properties
	If nProp Then

		fo.WriteLine(
		fo.WriteLine( "	----------------------------------------"
		fo.WriteLine( "		 Properties:"

		For i = 1 To nProp

			LSet PropRec = Mid$(cs, p)

			fo.WriteLine( "	----------------------------------------"
			fo.WriteLine( "	ID:			"; Hex$(IdElem(iElem), 8); " =="; Str$(IdElem(iElem))
			fo.WriteLine( "	Name:		"; Hex$(oName(iElem), 8); " ==> "; tlName(cs, SegDir, oName(iElem))
			fo.WriteLine( "	Reference:	"; Hex$(Refer(iElem), 8) // offset to the corresponding function record
			Incr iElem
			fo.WriteLine( "	Record size (low-byte):	"; Hex$(PropRec.RecSize, 4)
			fo.WriteLine( "	Property number?:		"; Hex$(PropRec.PropNum, 4)
			fo.WriteLine( "	Flags:					"; Hex$(PropRec.Flags, 4); IIf$(PropRec.Flags < 0, " = " & VarType(PropRec.Flags), "")
			fo.WriteLine( "	DataType:				"; Hex$(PropRec.DataType, 4); " = "; VarType(PropRec.DataType)

			fo.WriteLine( "	Variable kind:			"; Hex$(PropRec.VarKind, 4); " = ";
			d = PropRec.VarKind
			For n = 13 To 1 Step -1 //  13 = ParseCount($VarFlags)
				If (d And %VARFLAG_FIMMEDIATEBIND) Then
					fo.WriteLine( Parse$($VarFlags, n);
					If (d And 0x0FFF) Then fo.WriteLine( ", ";
				End If
				Shift Left d, 1
			Next n
			fo.WriteLine(

			fo.WriteLine( "	Variable desc size:		"; Hex$(PropRec.cVarDesc, 4)
			fo.WriteLine( "	Value/Offset:			"; Hex$(PropRec.OffsValue, 8)

//If %Def(%Study)
			fo.WriteLine( "	Unknown:				"; Hex$(PropRec.Unk, 8)/
//EndIf

			If PropRec.RecSize > 20 Then // 20 = (5 * SizeOf(Long))

				fo.WriteLine( "	HelpContext:			"; Hex$(PropRec.HelpCntxt, 8)

				If PropRec.RecSize > 24 Then // 24 = (6 * SizeOf(Long))
					fo.WriteLine( "	HelpString:	"; Hex$(PropRec.oHelpStr, 8);
					fo.WriteLine( " ==> "; tlString(cs, SegDir, PropRec.oHelpStr)

					If PropRec.RecSize > 32 Then // 32 = (8 * SizeOf(Long))

						If PropRec.RecSize > 36 Then // 36 = (9 * SizeOf(Long))
//If %Def(%Study)
							fo.WriteLine( "	Reserved:			"; Hex$(PropRec.Res, 8)
//EndIf
							fo.WriteLine( "	HelpStringContext:	"; Hex$(PropRec.HelpStrCnt, 8)
						End If

					End If

				End If

			End If

			fo.WriteLine(
			p = p + PropRec.RecSize

		Next i

	End If // nProp


//If %Def(%Study)
	// ----------------------------------------
	// 	Dump arrays of function and property: IDs, names and references
	// ----------------------------------------
	//  This is redundant, since the information is printed along with the
	// functions and properties they pertain to.
	n = nFunc + nProp
	If n > 0 Then

		iElem = 0

		If nFunc Then
			fo.WriteLine(
			fo.WriteLine( "	----------------------------------------"
			fo.WriteLine( "		 Other Function Data:"
		End If

		For iElem = 0 To ub Step 1

			If n = nProp Then // the functions are done, so do the properties
				fo.WriteLine(
				fo.WriteLine( "	----------------------------------------"
				fo.WriteLine( "		 Other Property Data:"
			End If
			Decr n

			// ID number of the function or property
			fo.WriteLine( "	----------------------------------------"
			fo.WriteLine( "	ID:			"; Hex$(IdElem(iElem), 8); " =="; Str$(IdElem(iElem))

			// offset to names in the name table
			fo.WriteLine( "	Name:		"; Hex$(oName(iElem), 8);
			fo.WriteLine( " ==> "; tlName(cs, SegDir, oName(iElem))

			// offset to the corresponding function record
			fo.WriteLine( "	Reference:	"; Hex$(Refer(iElem), 8)
			fo.WriteLine(

		Next iElem

	End If

//EndIf

	// Function =

} // DisFunction
#endif

// ****************************************
// cs contains the TypeLib data
// ls contains the name of the source file
uint DisTypeLib(byte [] cs, TextWriter fo)
{
int d			 ;
int i			 ;
int n			 ;
int p			 ;
int fName		 ; // TypeLib file name flag
TlbHeader TlbHdr;
MSFT_SegDir SegDir;
TypeInfo TypInfo;
ImportInfo ImpInfo;
TlbImpLib IFileInfo;
RefRecord RefRec;
GuidEntry GuidEnt;
NameIntro NameInt;
TYPEDESC TypDsc; // a simplified substitute for a tagTYPEDESC
ARRAYDESC AryDsc;
SafeArrayBound SafBnd;

String ls;


	// --------------------------------------
	// print the TypeLib Header
	LSet TlbHdr = cs;

	// get the segment directory in advance
	fName = IIf&((TlbHdr.fVar & HELPDLLFLAG), 4, 0);
	p = SizeOf(TlbHdr) + (4 * (TlbHdr.nTypeInfo)) + fName + 1;
	LSet SegDir = Mid$(cs, p);

    var enc = Encoding.UTF8;
	fo.WriteLine( "*************************");
	fo.WriteLine( "TypeLib Header");
	fo.WriteLine( "*************************");
	fo.WriteLine( "Magic 1:		{0:X8} = {1}", TlbHdr.Magic1, enc.GetString(cs, 0, 4));	// 0x5446534D """MSFT"""
	fo.WriteLine( "Magic 2:		{0:X8}", TlbHdr.Magic2);	// 0x00010002 version number?

	// position of libid in guid table (should be, else -1)
	fo.WriteLine( "GUID:			{0:X8} ==> {1}", TlbHdr.oGUID, tlGuid(cs, SegDir, TlbHdr.oGUID));

	fo.WriteLine( "Locale ID:		"; Hex$(TlbHdr.lcid, 8); " = "; Locale(TlbHdr.lcid) // locale id
	fo.WriteLine( "Locale ID 2:	{0:X8}", TlbHdr.lcid2);

	// fVar (largely unknown):
	// 	* the lower nibble is SysKind
	// 	* bit 5 is set if a helpfile is defined
	// 	* bit 8 is set if a help dll is defined
	fo.WriteLine( "VarFlags:		{0:X8}", TlbHdr.fVar)
	fo.WriteLine( "	System:		"; Parse$($SysKind, (TlbHdr.fVar And 7) + 1)
	fo.WriteLine( "	Help file"; IIf$((TlbHdr.fVar And 16), " ", " not "); "specified"
	fo.WriteLine( "	Help file is"; IIf$((TlbHdr.fVar And 256), " ", " not "); "in a DLL"

	fo.WriteLine( "Version:	"; Hex$(TlbHdr.version, 8)
	fo.WriteLine( "Flags:		"; Hex$(TlbHdr.Flags, 8)

	fo.WriteLine( "TypeInfo count:	"; Hex$(TlbHdr.nTypeInfo, 8); " =="; Str$(TlbHdr.nTypeInfo)	// number of TypeInfo// s

	fo.WriteLine( "HelpString:			"; Hex$(TlbHdr.HelpStr, 8);	// position of HelpString in stringtable
	fo.WriteLine( " ==> "; tlString(cs, SegDir, TlbHdr.HelpStr)

	fo.WriteLine( "HelpStringContext:	"; Hex$(TlbHdr.HelpStrCnt, 8)
	fo.WriteLine( "HelpContext:		"; Hex$(TlbHdr.HelpCntxt, 8)

	fo.WriteLine( "Name Table:"
	fo.WriteLine( "	Names count:	"; Hex$(TlbHdr.nName, 8); " =="; Str$(TlbHdr.nName)	// number of names in name table
	fo.WriteLine( "	Characters:		"; Hex$(TlbHdr.nChars, 8); " =="; Str$(TlbHdr.nChars)	// number of characters in name table

	fo.WriteLine(
	fo.WriteLine( "TypeLib Name:	"; Hex$(TlbHdr.oName, 8); " ==> "; tlName(cs, SegDir, TlbHdr.oName) // offset of name in name table
	fo.WriteLine( "Helpfile:		"; Hex$(TlbHdr.HelpFile, 8); " ==> "; tlString(cs, SegDir, TlbHdr.HelpFile) // position of helpfile in stringtable

	// if -1 no custom data, else it is offset in custom data/guid offset table
	fo.WriteLine( "Custom data offset:	"; Hex$(TlbHdr.CustDat, 8)

//If %Def(%Study)
	fo.WriteLine( "Reserved1:	"; Hex$(TlbHdr.Res1, 8)		// unknown always: 0x20 (guid hash size?)
	fo.WriteLine( "Reserved2:	"; Hex$(TlbHdr.Res2, 8)		// unknown always: 0x80 (name hash size?)
//EndIf

	fo.WriteLine( "IDispatch:			"; Hex$(TlbHdr.oDispatch, 8) // hRefType to IDispatch, or -1 if no IDispatch
	fo.WriteLine( "ImportInfo count:	"; Hex$(TlbHdr.nImpInfos, 8); " =="; Str$(TlbHdr.nImpInfos) // number of ImpInfos

	p = SizeOf(TlbHdr) + 1;
	if (fName ) {
		d = CvDwd(cs, p);
		fo.WriteLine( "TypeLib file name:			{0:X8} ==> {1}", d, tlString(cs, SegDir, d));
	}

	// --------------------------------------
	// print the (TypeInfo ?) offsets
	fo.WriteLine();
	fo.WriteLine( "*************************");
	fo.WriteLine( "Offsets to TypeInfo Data");
	fo.WriteLine( "*************************");
	p = p + fName;s
	for (n = 1; n <= TlbHdr.nTypeInfo; ++n) {
		fo.WriteLine( Hex$(CvDwd(cs, p), 8);
		p = p + 4;
		If p >= Len(cs) break;
	}


	// --------------------------------------
	// print the segment directory contents
	fo.WriteLine();
	fo.WriteLine( "*************************");
	fo.WriteLine( "Segment Directory (Segment name: Offset, Length)...");
	fo.WriteLine( "*************************");
	fo.WriteLine( "Type Info Table:		"; Hex$(SegDir.pTypInfo.Offs, 8); ", "; Hex$(SegDir.pTypInfo.nLen, 8)         );
	fo.WriteLine( "Import Info:			"; Hex$(SegDir.pImpInfo.Offs, 8); ", "; Hex$(SegDir.pImpInfo.nLen, 8)         );
	fo.WriteLine( "Imported Libraries:		"; Hex$(SegDir.pImpFiles.Offs, 8); ", "; Hex$(SegDir.pImpFiles.nLen, 8)   );
	fo.WriteLine( "References Table:		"; Hex$(SegDir.pRefer.Offs, 8); ", "; Hex$(SegDir.pRefer.nLen, 8)         );
	fo.WriteLine( "Lib Table:				"; Hex$(SegDir.pLibs.Offs, 8); ", "; Hex$(SegDir.pLibs.nLen, 8)           );
	fo.WriteLine( "GUID Table:				"; Hex$(SegDir.pGUID.Offs, 8); ", "; Hex$(SegDir.pGUID.nLen, 8)           );
	fo.WriteLine( "Unknown 01:				"; Hex$(SegDir.Unk01.Offs, 8); ", "; Hex$(SegDir.Unk01.nLen, 8)           );
	fo.WriteLine( "Name Table:				"; Hex$(SegDir.pNames.Offs, 8); ", "; Hex$(SegDir.pNames.nLen, 8)         );
	fo.WriteLine( "String Table:			"; Hex$(SegDir.pStrings.Offs, 8); ", "; Hex$(SegDir.pStrings.nLen, 8)     );
	fo.WriteLine( "Type Descriptors:		"; Hex$(SegDir.pTypDesc.Offs, 8); ", "; Hex$(SegDir.pTypDesc.nLen, 8)     );
	fo.WriteLine( "Array Descriptors:		"; Hex$(SegDir.pArryDesc.Offs, 8); ", "; Hex$(SegDir.pArryDesc.nLen, 8)   );
	fo.WriteLine( "Custom Data:			"; Hex$(SegDir.pCustData.Offs, 8); ", "; Hex$(SegDir.pCustData.nLen, 8)       );
	fo.WriteLine( "GUID Offsets:			"; Hex$(SegDir.pCDGuids.Offs, 8); ", "; Hex$(SegDir.pCDGuids.nLen, 8)     );
	fo.WriteLine( "Unknown 02:				"; Hex$(SegDir.Unk02.Offs, 8); ", "; Hex$(SegDir.Unk02.nLen, 8)           );
	fo.WriteLine( "Unknown 03:				"; Hex$(SegDir.Unk03.Offs, 8); ", "; Hex$(SegDir.Unk03.nLen, 8)           );


//If %Def(%Study)

	int fAlert;
	ReDim pSegTmp(14) As SegDesc At VarPtr(SegDir)
	For i = 0 To 14

		If pSegTmp(i).Res01 <> -1 Then
			fo.WriteLine( "!!!"
			Incr fAlert
		End If

		If pSegTmp(i).Res02 <> 0x0F Then
			fo.WriteLine( "!!!"
			Incr fAlert
		End If

		If fAlert = 1 Then
			MsgBox "Interesting Reserved value found"
			Incr fAlert
		End If

	Next i

	// If fAlert Then
	fo.WriteLine();
	fo.WriteLine( "@@@:	Reserved Fields");
	fo.WriteLine( "Segment name:	Res01, Res02...");
	fo.WriteLine( "*************************");
	fo.WriteLine( "Type Info Table:		{0:X8}, {1:X8}", SegDir.pTypInfo.Res01, SegDir.pTypInfo.Res02, 8);
	fo.WriteLine( "Import Info:			{0:X8}, {1:X8}", SegDir.pImpInfo.Res01, SegDir.pImpInfo.Res02, 8);
	fo.WriteLine( "Imported Libraries:		{0:X8}, {1:X8}", SegDir.pImpFiles.Res01, SegDir.pImpFiles.Res02, 8)
	fo.WriteLine( "References Table:		"; Hex$(SegDir.pRefer.Res01, 8); ", "; Hex$(SegDir.pRefer.Res02, 8)
	fo.WriteLine( "Lib Table:				"; Hex$(SegDir.pLibs.Res01, 8); ", "; Hex$(SegDir.pLibs.Res02, 8)
	fo.WriteLine( "GUID Table:				"; Hex$(SegDir.pGUID.Res01, 8); ", "; Hex$(SegDir.pGUID.Res02, 8)
	fo.WriteLine( "Unknown 01:				"; Hex$(SegDir.Unk01.Res01, 8); ", "; Hex$(SegDir.Unk01.Res02, 8)
	fo.WriteLine( "Name Table:				"; Hex$(SegDir.pNames.Res01, 8); ", "; Hex$(SegDir.pNames.Res02, 8)
	fo.WriteLine( "String Table:			"; Hex$(SegDir.pStrings.Res01, 8); ", "; Hex$(SegDir.pStrings.Res02, 8)
	fo.WriteLine( "Type Descriptors:		"; Hex$(SegDir.pTypDesc.Res01, 8); ", "; Hex$(SegDir.pTypDesc.Res02, 8)
	fo.WriteLine( "Array Descriptors:		"; Hex$(SegDir.pArryDesc.Res01, 8); ", "; Hex$(SegDir.pArryDesc.Res02, 8)
	fo.WriteLine( "Custom Data:			"; Hex$(SegDir.pCustData.Res01, 8); ", "; Hex$(SegDir.pCustData.Res02, 8)
	fo.WriteLine( "Custom Data/GUID// s:		"; Hex$(SegDir.pCDGuids.Res01, 8); ", "; Hex$(SegDir.pCDGuids.Res02, 8)
	fo.WriteLine( "Unknown 02:				"; Hex$(SegDir.Unk02.Res01, 8); ", "; Hex$(SegDir.Unk02.Res02, 8)
	fo.WriteLine( "Unknown 03:				"; Hex$(SegDir.Unk03.Res01, 8); ", "; Hex$(SegDir.Unk03.Res02, 8)
	fo.WriteLine(
	// End If

//EndIf


	// --------------------------------------
	// check two entries to be sure we found it
	If SegDir.pTypInfo.Res02 <> 0x0F Or SegDir.pImpInfo.Res02 <> 0x0F Then
		UpdateLog("Can// t find the table directory")
		Exit Function
	End If



	// **************************************
	// 	Print the data from the segments
	// **************************************

	// --------------------------------------
	// print the TypeInfo structures
	fo.WriteLine(
	fo.WriteLine( "*************************"
	fo.WriteLine( "TypeInfo Data..."
	fo.WriteLine( "*************************"
	i = 0
	p = SegDir.pTypInfo.Offs + 1
	n = p + SegDir.pTypInfo.nLen
	while (p < n)
    {
		Incr i
		fo.WriteLine( "-------------------------"
		fo.WriteLine( "TypeInfo number:	"; Format$(i)
		LSet TypInfo = Mid$(cs, p)

		fo.WriteLine( "Type kind:	"; Hex$((TypInfo.TypeKind And 7?), 2); " = "; Parse$($TKind, (TypInfo.TypeKind And 7?) + 1)
		d = TypInfo.Align : Shift Right d, 3
		fo.WriteLine( "	Alignment: ("; Hex$(TypInfo.Align, 2); "/8) = "; Hex$(d, 2)

		// (function records are printed near the end of the TypeInfo section)

		fo.WriteLine( "Memory to allocate:		"; Hex$(TypInfo.Alloc, 8)
		fo.WriteLine( "Reconstituted Size:		"; Hex$(TypInfo.Reconst, 8)

//If %Def(%Study)
		fo.WriteLine( "Reserved 01:	"; Hex$(TypInfo.Res01, 8)
		fo.WriteLine( "Reserved 02:	"; Hex$(TypInfo.Res02, 8)
//EndIf

		// counts of functions and properties
		fo.WriteLine(
		fo.WriteLine( "Function count:	"; Hex$(TypInfo.nFuncs, 4); " =="; Str$(TypInfo.nFuncs)
		fo.WriteLine( "Property count:	"; Hex$(TypInfo.nProps, 4); " =="; Str$(TypInfo.nProps)

//If %Def(%Study)
		fo.WriteLine( "Reserved 03:	"; Hex$(TypInfo.Res03, 8)
		fo.WriteLine( "Reserved 04:	"; Hex$(TypInfo.Res04, 8)
		fo.WriteLine( "Reserved 05:	"; Hex$(TypInfo.Res05, 8)
		fo.WriteLine( "Reserved 06:	"; Hex$(TypInfo.Res06, 8)
//EndIf

		fo.WriteLine( "GUID:	"; Hex$(TypInfo.oGUID, 8); " ==> "; tlGuid(cs, SegDir, TypInfo.oGUID)

		fo.WriteLine( "Type Flags:	"; Hex$(TypInfo.fType, 8); " = ";
		d = TypInfo.fType // this is a bit field
		For n = 14 To 1 Step -1
			If (d And %TYPEFLAG_FREVERSEBIND) Then
				fo.WriteLine( Parse$($TypeFlags, n);
				If (d And %TYPEFLAG_MASK) Then fo.WriteLine( ", ";
			End If
			Shift Left d, 1
		Next n
		fo.WriteLine(


		fo.WriteLine( "Name:	"; Hex$(TypInfo.oName, 8); " ==> "; tlName(cs, SegDir, TypInfo.oName)
		fo.WriteLine( "Version:	"; Hex$(TypInfo.version, 8)

		fo.WriteLine( "Doc String:	"; Hex$(TypInfo.DocStr, 8);
		fo.WriteLine( " ==> "; tlString(cs, SegDir, TypInfo.DocStr)

		fo.WriteLine( "HelpStringContext:	"; Hex$(TypInfo.HelpStrCnt, 8)
		fo.WriteLine( "HelpContext:		"; Hex$(TypInfo.HelpCntxt, 8)

		fo.WriteLine(
		fo.WriteLine( "Custom data offset:		"; Hex$(TypInfo.oCustData, 8)
		fo.WriteLine( "Implemented interfaces:	"; Hex$(TypInfo.nImplTypes, 4); " =="; Str$(TypInfo.nImplTypes)
		fo.WriteLine( "Virtual table size:		"; Hex$(TypInfo.cVft, 4)
		fo.WriteLine( "Unknown 03:				"; Hex$(TypInfo.Unk03, 8)

		// position in type description table or in base interfaces
		// if coclass:	offset in reftable
		// if interface:	reference to inherited interface
		// if module:		offset to DLL name in name table

//If %Def(%Study)
		Select Case (TypInfo.TypeKind And 0x0F)
			// Case %TKIND_ENUM
			// Case %TKIND_RECORD
			// Case %TKIND_MODULE // offset to DLL name in string table
			// Case %TKIND_INTERFACE // reference to inherited interface?
			// Case %TKIND_DISPATCH
			// Case %TKIND_COCLASS // offset in reftable
			Case %TKIND_ALIAS
				Note "TKIND_ALIAS"
			Case %TKIND_UNION
				Note "TKIND_UNION"
			// Case %TKIND_MAX
			// Case Else

		End Select
//EndIf

		// fo.WriteLine( "Type1:	"; Hex$(TypInfo.Type1, 8)
		Select Case TypInfo.TypeKind

			// Case %TKIND_ENUM

			// Case %TKIND_RECORD

			Case %TKIND_MODULE // offset to DLL name in string table
				fo.WriteLine( "DLL Name:	"; Hex$(TypInfo.Type1, 8); " ==> ";
				If TypInfo.Type1 >= 0 Then fo.WriteLine( tlName(cs, SegDir, TypInfo.Type1)

			Case %TKIND_INTERFACE // reference to inherited interface?
				fo.WriteLine( "Inherited interface?:	"; Hex$(TypInfo.Type1, 8)

			// Case %TKIND_DISPATCH

			Case %TKIND_COCLASS // offset in reftable
				fo.WriteLine( "Reference table offset:	"; Hex$(TypInfo.Type1, 8)

			Case %TKIND_ALIAS // the following is partly translated ReactOS code

					If TypInfo.Type1 < 0 Then
						d = TypInfo.Type1 And %VT_TYPEMASK
					Else // get index into Lib Table?
						d = TypInfo.Type1 \ 8 // ?
					End If

					If TypInfo.Type1 = %VT_UserDefined Then // do RefType
					End If

			// Case %TKIND_UNION

			// Case %TKIND_MAX

			Case Else
				fo.WriteLine( "DataType1:	"; Hex$(TypInfo.Type1, 8)

		End Select

		// if 0x8000, entry above is valid, else it is zero?
		fo.WriteLine( "DataType2:	"; Hex$(TypInfo.Type2, 8)

//If %Def(%Study)
		fo.WriteLine( "Reserved 7:	"; Hex$(TypInfo.Res07, 8)
		fo.WriteLine( "Reserved 8:	"; Hex$(TypInfo.Res08, 8)
//EndIf

		fo.WriteLine(
		fo.WriteLine( "Records offset:	"; Hex$(TypInfo.oFunRec, 8)
		If TypInfo.oFunRec < Len(cs) Then // do function/property records
			DisFunction(cs, SegDir, TypInfo.oFunRec, TypInfo.nFuncs, TypInfo.nProps)
		End If

		p = p + SizeOf(TypInfo)

	Loop While p <= Len(cs)


	// --------------------------------------
	// Print ImportInfo ?
	fo.WriteLine(
	fo.WriteLine( "*************************"
	fo.WriteLine( "ImportInfo"
	fo.WriteLine( "*************************"
	i = 0
	p = SegDir.pImpInfo.Offs + 1
	n = p + SegDir.pImpInfo.nLen
	Do Until p >= n

		Incr i
		LSet ImpInfo = Mid$(cs, p)
		fo.WriteLine( "--------------------------------------------"
		fo.WriteLine( "Import Info number:	"; Format$(i)
		fo.WriteLine( "Count:				"; Format$(ImpInfo.Count)
		fo.WriteLine( "Offset in import file table:	"; Hex$(ImpInfo.oImpFile, 8)

		If (ImpInfo.Flags And 1?) Then
			fo.WriteLine( "GUID:	"; Hex$(ImpInfo.oGuid, 8); " ==> "; tlGuid(cs, SegDir, ImpInfo.oGuid)
		Else
			fo.WriteLine( "TypeInfo index:	"; Hex$(ImpInfo.oGuid, 8); " =="; Str$(ImpInfo.oGuid)
		End If

		fo.WriteLine( "Type:	"; Parse$($TKind, ImpInfo.TypeKind)

		p = p + SizeOf(ImpInfo)

	Loop While p <= Len(cs)


	// --------------------------------------
	// Print Imported Type Libs
	fo.WriteLine(
	fo.WriteLine( "*************************"
	fo.WriteLine( "Imported Type Libs"
	fo.WriteLine( "*************************"
	p = SegDir.pImpFiles.Offs + 1
	n = p + SegDir.pImpFiles.nLen
	Do Until p >= n
		LSet IFileInfo = Mid$(cs, p)
		fo.WriteLine( "--------------------------------------------"
		fo.WriteLine( "GUID:			"; Hex$(IFileInfo.oGUID, 8); " ==> "; tlGuid(cs, SegDir, IFileInfo.oGuid)
		fo.WriteLine( "Locale ID?:		"; Hex$(IFileInfo.LCID, 8); " = "; Locale(IFileInfo.LCID)
		fo.WriteLine( "Major version:	"; Hex$(IFileInfo.MajVer, 4)
		fo.WriteLine( "Minor version:	"; Hex$(IFileInfo.MinVer, 4)

		d = IFileInfo.cSize
		Shift Right d, 2
		fo.WriteLine( "Size = 		"; Hex$(IFileInfo.cSize, 4); "/4 = "; Hex$(d, 4)
		fo.WriteLine( "File name:	"; Mid$(cs, p + SizeOf(TlbImpLib), d)

		p = p + ((SizeOf(TlbImpLib) + IFileInfo.cSize + 3) And Not 3) // advance to next one

	Loop While p <= Len(cs)


	// --------------------------------------
	// Print References Table
	fo.WriteLine(
	fo.WriteLine( "*************************"
	fo.WriteLine( "References Table"
	fo.WriteLine( "*************************"
	p = SegDir.pRefer.Offs + 1
	n = p + SegDir.pRefer.nLen
	Do Until p >= n
		LSet RefRec = Mid$(cs, p)
		fo.WriteLine( "--------------------------------------------"

		// if it// s a multiple of 4, it// s an offset in the TypeInfo table,
		// otherwise it// s an offset in the external reference table with
		// an offset of 1
		fo.WriteLine( "Reference type:	"; Hex$(RefRec.RefType, 8);
		If (RefRec.RefType And 0x03) Then
			fo.WriteLine( "	==> (External Reference Table)"
		Else
			fo.WriteLine( "	==> (TypeInfo)"
		End If

		fo.WriteLine( "Flags:			"; Hex$(RefRec.Flags, 8)
		If RefRec.oCustData >= 0 Then fo.WriteLine( "Custom data:	"; Hex$(RefRec.oCustData, 8)
		If RefRec.oNext >= 0 Then fo.WriteLine( "Next offset:	"; Hex$(RefRec.oNext, 8)
		p = p + SizeOf(RefRecord)

	Loop While p <= Len(cs)


	// --------------------------------------
	// Print "Lib Table"	(unknown format)
	// always exists, always the same size (0x80)
	// ...hash table with offsets to GUID?
	fo.WriteLine(
	fo.WriteLine( "*************************"
	fo.WriteLine( "Lib Table (offsets into GUID table)"
	fo.WriteLine( "*************************"
	p = SegDir.pLibs.Offs + 1
	n = p + SegDir.pLibs.nLen
	Do Until p >= n
		d = Cvl(cs, p)
		If d >= 0 Then fo.WriteLine( Hex$(d, 8)
		p = p + 4
	Loop While p <= Len(cs)


	// --------------------------------------
	// Print GUID table
	fo.WriteLine(
	fo.WriteLine( "*************************"
	fo.WriteLine( "GUID Table"
	fo.WriteLine( "*************************"
	p = SegDir.pGUID.Offs + 1
	n = p + SegDir.pGUID.nLen
	Do Until p >= n

		LSet GuidEnt = Mid$(cs, p)
		fo.WriteLine( "--------------------------------------------"
		fo.WriteLine( "GUID:	"; GuidTxt$(GuidEnt.oGUID)
		// 	The meaning of .hRefType:
		//  = -2 for a TypeLib GUID
		// TypeInfo offset for TypeInfo GUID,
		// Otherwise, the low two bits:
		// 	= 01 for an imported TypeInfo
		// 	= 10 for an imported TypeLib (used by imported TypeInfos)
		fo.WriteLine( "Href Type:	"; Hex$(GuidEnt.hRefType, 8);
		If GuidEnt.hRefType = -2 Then
			fo.WriteLine( " = TypeLib GUID"
		ElseIf (GuidEnt.hRefType And 3) = 1 Then
			fo.WriteLine( " = Imported TypeInfo"
		ElseIf (GuidEnt.hRefType And 3) = 2 Then
			fo.WriteLine( " = Imported TypeLib"
		Else
			fo.WriteLine( " = Offset?"
		End If

		fo.WriteLine( "Next hash:	"; Hex$(GuidEnt.NextHash, 8)
		p = p + SizeOf(GuidEntry)

	Loop While p <= Len(cs)


	// --------------------------------------
	// Print "Unknown 01" (length is "always" 0x200)
	fo.WriteLine(
	fo.WriteLine( "*************************"
	fo.WriteLine( """Unknown 01"" --- Table with Offsets into the Name Table"
	fo.WriteLine( "*************************"
	p = SegDir.Unk01.Offs + 1
	n = p + SegDir.Unk01.nLen
	Do Until p >= n
		d = Cvl(cs, p)
		If d >= 0 Then fo.WriteLine( Hex$(d, 8)
		p = p + 4
	Loop While p <= Len(cs)


	// --------------------------------------
	// Print Name Table
	// (this keeps the offset in p zero-based)
	fo.WriteLine(
	fo.WriteLine( "*************************"
	fo.WriteLine( "Name Table"
	fo.WriteLine( "*************************"
	p = SegDir.pNames.Offs
	n = p + SegDir.pNames.nLen
	Do Until p >= n

		LSet NameInt = Mid$(cs, p + 1)
		fo.WriteLine( "--------------------------------------------"
		If NameInt.hRefType <> -1 Then fo.WriteLine( "Offset:			"; Hex$(NameInt.hRefType, 8)
		fo.WriteLine( "Next hash:		"; Hex$(NameInt.NextHash, 8)
		d = LoByt(NameInt.cName)
		fo.WriteLine( "Name length:	"; Hex$(d, 2); " =="; Str$(d)
		fo.WriteLine( "Flags?...		"; Hex$(HiByt(LoWrd(NameInt.cName)), 2)
		fo.WriteLine( "Hash code:		"; Hex$(HiWrd(NameInt.cName), 4)

		p = p + SizeOf(NameInt)
		fo.WriteLine( $Dq; Mid$(cs, p + 1, d); $Dq
		p = (p + d + 3) And 0xFFFFFFFC // advance to next DWord-aligned offset

	Loop While p <= Len(cs)


	// --------------------------------------
	// Print String Table
	// every entry, length-plus-text, is a minimum of eight bytes
	// (this keeps the offset in p zero-based)
	fo.WriteLine(
	fo.WriteLine( "*************************"
	fo.WriteLine( "String Table"
	fo.WriteLine( "*************************"
	p = SegDir.pStrings.Offs
	n = p + SegDir.pStrings.nLen
	i = 1 // string number
	Do Until p >= n
		d = CvWrd(cs, p + 1)
		fo.WriteLine( Format$(i); ")	"; Mid$(cs, p + 3, d)
		// advance to next Dword-aligned offset beyond minimum length of eight bytes
		p = (p + Max&(d + 2, 8) + 3) And 0xFFFFFFFC
		Incr i
	Loop While p <= Len(cs)


	// --------------------------------------
	// Print Type Descriptors
	If SegDir.pTypDesc.nLen Then
		fo.WriteLine(
		fo.WriteLine( "*************************"
		fo.WriteLine( "Type Descriptors"
		fo.WriteLine( "*************************"
		p = SegDir.pTypDesc.Offs + 1
		n = p + SegDir.pTypDesc.nLen
		Do Until p >= n

			fo.WriteLine( "-------------------------"
			fo.WriteLine( "Raw:	"; Hex$(CvDwd(cs, p), 8), Hex$(CvDwd(cs, p + 4), 8)

			LSet TypDsc = Mid$(cs, p)

			fo.WriteLine( "Data type 1 = "; Hex$(TypDsc.v2, 4); Hex$(TypDsc.v1, 4);
			If (TypDsc.v2 And 0x7FFE) = 0x7FFE Then
				fo.WriteLine( " = ";
			ElseIf (TypDsc.v2 And %VT_Vector) Then
				fo.WriteLine( " = VT_Vector, ";
			ElseIf (TypDsc.v2 And %VT_Array) Then
				fo.WriteLine( " = VT_Array, ";
			ElseIf (TypDsc.v2 And %VT_ByRef) Then
				fo.WriteLine( " = VT_ByRef, ";
			// ElseIf (TypDsc.v2 And %VT_Reserved) Then
			Else
				fo.WriteLine( " = ";
			End If

			fo.WriteLine( VarType(TypDsc.v1);

			If (TypDsc.v2 And 0x7FFE) = 0x7FFE Then
				fo.WriteLine(
			Else
				fo.WriteLine( " == base type: "; VarType(TypDsc.v2 And %VT_TypeMask)
			End If


			If LoByt(TypDsc.v1) = %VT_Ptr Or LoByt(TypDsc.v1) = %VT_SafeArray Then

				If TypDsc.v4 < 0 Then  // offset into type descriptor table
					fo.WriteLine( "Type descriptor table offset:	"; Hex$(TypDsc.v3 And 0x07FF8, 4)
				Else // file offset to type descriptor
					//  This doesn// t sound sensible, but it looks like what the ReactOS code was doing.
					d = MakLng(TypDsc.v2, TypDsc.v3)
					fo.WriteLine( "Type descriptor file offset:	"; Hex$(d And (TypDsc.v3\8), 8)
				End If

			ElseIf LoByt(TypDsc.v1) = %VT_CArray Then
				fo.WriteLine( "Array descriptor offset:	"; Hex$(MakLng(TypDsc.v3, TypDsc.v4), 8)
			ElseIf LoByt(TypDsc.v1) = %VT_UserDefined Then
				fo.WriteLine( "Type descriptor offset:		"; Hex$(MakLng(TypDsc.v3, TypDsc.v4) And 0x0FFFFFFF8&, 8)
			End If

			p = p + SizeOf(tagTYPEDESC)

		Loop While p <= Len(cs)

	End If


	// --------------------------------------
	// Print Array Descriptors
	If SegDir.pArryDesc.nLen Then

//If %Def(%Study)
	Note "Array(s) found"
//EndIf

		//  What do the lower bits of td(1) mean (when td(1) < 0)?
		//  What is td(3) (it gets over-written when td(1) < 0) ?
		fo.WriteLine(
		fo.WriteLine( "*************************"
		fo.WriteLine( "Array Descriptors"
		fo.WriteLine( "*************************"
		p = SegDir.pArryDesc.Offs + 1
		n = p + SegDir.pArryDesc.nLen
		Do Until p >= n

			LSet AryDsc = Mid$(cs, p)
			ReDim td(3)		As Integer At VarPtr(AryDsc)

			fo.WriteLine( "--------------------------------------------"
			fo.WriteLine( "Raw:	"; Hex$(CvDwd(cs, p), 8); ", "; Hex$(CvDwd(cs, p + 4), 8); ", "; Hex$(CvDwd(cs, p + 8), 8); ", "; Hex$(CvDwd(cs, p + 12), 8)

			If AryDsc.u.hRefType >= 0 Then // a pointer to ANOTHER array descriptor?
				AryDsc.u.lpadesc = (AryDsc.u.lpadesc And 0x07FFF) \ 8
				fo.WriteLine( "Offset to array descriptor:	"; Hex$(AryDsc.u.lpadesc, 8)
			Else // the low word contains the variable-type code
				fo.WriteLine( "hRefType:	"; Hex$(AryDsc.u.hRefType, 8)
				AryDsc.tVar = td(0) And %VT_TypeMask
			End If

			If (AryDsc.tVar And 255) = 0 Then d = HiByt(AryDsc.tVar) Else d = AryDsc.tVar
			fo.WriteLine( "Variable type:	"; Hex$(AryDsc.tVar, 4); " = "; VarType(d)
			fo.WriteLine( "Number of dimensions:	"; Hex$(AryDsc.nDims, 4); " =="; Str$(AryDsc.nDims)

			p = p + SizeOf(ARRAYDESC) - SizeOf(SafeArrayBound)
			For i = 1 To AryDsc.nDims
				LSet SafBnd = Mid$(cs, p)
				fo.WriteLine( "("; Format$(i); ")	"; "Elements: "; Hex$(SafBnd.nElements, 8); " =="; Str$(SafBnd.nElements); "	Lower bound: "; Hex$(SafBnd.lLBound, 8); " =="; Str$(SafBnd.lLBound)
				p = p + SizeOf(SafeArrayBound)
				If p >= Len(cs) Then Exit Do
			Next i

		Loop While p <= Len(cs)

	End If

	// --------------------------------------
	// Print Custom Data
	// a guess as to the storage format...
	// Type CustomData
	// 	cWords				As Word
	// 	aData(cWords - 1)	As Word
	// 	sGUID				As Guid
	// End Type
	If SegDir.pCustData.nLen Then

//If %Def(%Study)
	If SegDir.pCustData.nLen > 16 Then
		Note "More than 16 bytes of custom data"
	End If
//EndIf
		// custom data and default parameter values
		fo.WriteLine(
		fo.WriteLine( "*************************"
		fo.WriteLine( "Custom Data"
		fo.WriteLine( "*************************"
		p = SegDir.pCustData.Offs + 1
		n = p + SegDir.pCustData.nLen
		Do Until p >= n
			fo.WriteLine( Hex$(CvDwd(cs, p), 8)
			p = p + 4
		Loop While p <= Len(cs)

	End If


	// --------------------------------------
	// Print offsets of GUID// s and into the
	If SegDir.pCDGuids.nLen Then
		// custom data table
		fo.WriteLine(
		fo.WriteLine( "*************************"
		fo.WriteLine( "Offsets of GUID// s"
		fo.WriteLine( "*************************"
		p = SegDir.pCDGuids.Offs + 1
		n = p + SegDir.pCDGuids.nLen
		Do Until p >= n
			fo.WriteLine( Hex$(CvDwd(cs, p), 8)
			p = p + 4
		Loop While p <= Len(cs)
	End If

	// --------------------------------------
	// Print "Unknown 02"
	If SegDir.Unk02.nLen Then
//If %Def(%Study)
	Note "Unknown 02"
//EndIf
		fo.WriteLine(
		fo.WriteLine( "*************************"
		fo.WriteLine( "Unknown 02"
		fo.WriteLine( "*************************"
		p = SegDir.Unk02.Offs + 1
		n = p + SegDir.Unk02.nLen
		Do Until p >= n
			fo.WriteLine( Hex$(CvDwd(cs, p), 8)
			p = p + 4
		Loop While p <= Len(cs)
	End If


	// --------------------------------------
	// Print "Unknown 03"
	If SegDir.Unk03.nLen Then
//If %Def(%Study)
	Note "Unknown 03"
//EndIf
		fo.WriteLine(
		fo.WriteLine( "*************************"
		fo.WriteLine( "Unknown 03"
		fo.WriteLine( "*************************"
		p = SegDir.Unk03.Offs + 1
		n = p + SegDir.Unk03.nLen
		Do Until p >= n
			fo.WriteLine( Hex$(CvDwd(cs, p), 8)
			p = p + 4
		Loop While p <= Len(cs)
	End If

}
}
}
}
#endif