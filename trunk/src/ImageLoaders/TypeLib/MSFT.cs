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
Local n		As Long
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
			If n => %Priority Then
				// MsgBox "Found something of interest" & $CrLf & ps
				Incr ct // disable any further alerts
			End If
		End If

		// print marker in output file to help find items of interest
		// (this assumes that the last file opened was the output file)
		Print# fo,
		Print# fo, $StudyMark; ps

		// maintain a log of findings
		Print# ff, ps

	End If

End Sub // Note

#endif




// ********************************************************************************
// 					TypeLib-Specific Equates and Data Types
// ********************************************************************************

// Note:	OAIDL.h says hRefType is a DWORD

%HELPDLLFLAG	= &H0100


// ****************************************
// 	"Magic" Values
// ****************************************
%MSFT_SIGNATURE		= &H05446534D //   "MSFT"
%SLTG_SIGNATURE		= &H047544C53 //   "SLTG"


// ****************************************
// 	Equates for Translating Flags
// 		and Codes to Text
// ****************************************
$SysKind		= "Win16,Win32,Macintosh"
$VarFlags		= "ReadOnly,Source,Bindable,RequestEdit,DisplayBind,DefaultBind,Hidden,Restricted,DefaultCollelem,UiDefault,NonBrowsable,Replaceable,ImmediateBind"
$TKind			= "Enum,Record,Module,Interface,Dispatch,Coclass,Alias,Union,Max"
$TypeFlags		= "AppObject,CanCreate,Licensed,PredeclId,Hidden,Control,Dual,Nonextensible,Oleautomation,Restricted,Aggregatable,Replaceable,Dispatchable,ReverseBind"
$ParamFlags		= "In,Out,LCID,RetVal,Opt,HasDefault,HasCustData"
$CallConv		= "FastCall,CDecl,Pascal,MacPascal,StdCall,FPFastCall,SysCall,MPWCDecl,MPWPascal,Max"
$InvoKind		= "Func,PropertyGet,PropertyPut,PropertyPutRef"
$FuncKind		= "Virtual,PureVirtual,NonVirtual,Static,Dispatch"


// ****************************************
// 	Variable-Type Codes, Masks and Flags
// ****************************************
%VT_Empty			= 0??
%VT_Null			= 1??
%VT_I2				= 2??
%VT_I4				= 3??
%VT_R4				= 4??
%VT_R8				= 5??
%VT_Cy				= 6??
%VT_Date			= 7??
%VT_BStr			= 8??
%VT_Dispatch		= 9??
%VT_Error			= 10??
%VT_Bool			= 11??
%VT_Variant			= 12??
%VT_Unknown			= 13??
%VT_Decimal			= 14??
%VT_I1				= 16??
%VT_UI1				= 17??
%VT_UI2				= 18??
%VT_UI4				= 19??
%VT_I8				= 20??
%VT_UI8				= 21??
%VT_Int				= 22??
%VT_UInt			= 23??
%VT_Void			= 24??
%VT_HResult			= 25??
%VT_Ptr				= 26??
%VT_SafeArray		= 27??
%VT_CArray			= 28??
%VT_UserDefined		= 29??
%VT_LPStr			= 30??
%VT_LPWStr			= 31??
%VT_Record			= 36??
%VT_FileTime		= 64??
%VT_Blob			= 65??
%VT_Stream			= 66??
%VT_Storage			= 67??
%VT_Streamed_Object	= 68??
%VT_Stored_Object	= 69??
%VT_Blob_Object		= 70??
%VT_CF				= 71??
%VT_ClsID			= 72??

// 	flags
%VT_Bstr_Blob		= &H00FFF??
%VT_Vector			= &H01000??
%VT_Array			= &H02000??
%VT_ByRef			= &H04000??
%VT_Reserved		= &H08000??

// 	masks
%VT_Illegal			= &H0FFFF??
%VT_IllegalMasked	= &H00FFF??
%VT_TypeMask		= &H00FFF??


// ****************************************
// 	Calling Conventions
// ****************************************
%CC_FASTCALL			= 0
%CC_CDECL				= 1
%CC_MSCPASCAL			= 2
%CC_PASCAL				= 2
%CC_MACPASCAL			= 3
%CC_STDCALL				= 4
%CC_FPFASTCALL			= 5
%CC_SYSCALL				= 6
%CC_MPWCDECL			= 7
%CC_MPWPASCAL			= 8
%CC_MAX					= 9


// ****************************************
// 	Function Types
// ****************************************
%FUNC_VIRTUAL			= 0
%FUNC_PUREVIRTUAL		= 1
%FUNC_NONVIRTUAL		= 2
%FUNC_STATIC			= 3
%FUNC_DISPATCH			= 4


// ****************************************
// 	Function Flags
// ****************************************
%FUNCFLAG_FRESTRICTED		= &H00001
%FUNCFLAG_FSOURCE			= &H00002
%FUNCFLAG_FBINDABLE			= &H00004
%FUNCFLAG_FREQUESTEDIT		= &H00008
%FUNCFLAG_FDISPLAYBIND		= &H00010
%FUNCFLAG_FDEFAULTBIND		= &H00020
%FUNCFLAG_FHIDDEN			= &H00040
%FUNCFLAG_FUSESGETLASTERROR	= &H00080
%FUNCFLAG_FDEFAULTCOLLELEM	= &H00100
%FUNCFLAG_FUIDEFAULT		= &H00200
%FUNCFLAG_FNONBROWSABLE		= &H00400
%FUNCFLAG_FREPLACEABLE		= &H00800
%FUNCFLAG_FIMMEDIATEBIND  	= &H01000
#If %Def(%MAC)
%FUNCFLAG_FORCELONG			= 2147483647
#Endif


// ****************************************
// 	Invocation Kinds
// ****************************************
%INVOKE_FUNC			= 1
%INVOKE_PROPERTYGET		= 2
%INVOKE_PROPERTYPUT		= 4
%INVOKE_PROPERTYPUTREF	= 8


// ****************************************
// 	Parameter Flags
// ****************************************
%PARAMFLAG_NONE			= &H000
%PARAMFLAG_FIN			= &H001
%PARAMFLAG_FOUT			= &H002
%PARAMFLAG_FLCID		= &H004
%PARAMFLAG_FRETVAL		= &H008
%PARAMFLAG_FOPT			= &H010
%PARAMFLAG_FHASDEFAULT	= &H020
%PARAMFLAG_FHASCUSTDATA	= &H040


// ****************************************
// 	System Kind
// ****************************************
%SYS_WIN16		= 0
%SYS_WIN32		= 1
%SYS_MAC		= 2

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
%TKIND_ENUM			= 0
%TKIND_RECORD		= 1
%TKIND_MODULE		= 2
%TKIND_INTERFACE	= 3
%TKIND_DISPATCH		= 4
%TKIND_COCLASS		= 5
%TKIND_ALIAS		= 6
%TKIND_UNION		= 7
%TKIND_MAX			= 8


// ****************************************
// 	Type Flags
// ****************************************
%TYPEFLAG_FAPPOBJECT		= &H00001
%TYPEFLAG_FCANCREATE		= &H00002
%TYPEFLAG_FLICENSED			= &H00004
%TYPEFLAG_FPREDECLID		= &H00008
%TYPEFLAG_FHIDDEN			= &H00010
%TYPEFLAG_FCONTROL			= &H00020
%TYPEFLAG_FDUAL				= &H00040
%TYPEFLAG_FNONEXTENSIBLE	= &H00080
%TYPEFLAG_FOLEAUTOMATION	= &H00100
%TYPEFLAG_FRESTRICTED		= &H00200
%TYPEFLAG_FAGGREGATABLE		= &H00400
%TYPEFLAG_FREPLACEABLE		= &H00800
%TYPEFLAG_FDISPATCHABLE		= &H01000
%TYPEFLAG_FREVERSEBIND		= &H02000
%TYPEFLAG_MASK				= %TYPEFLAG_FREVERSEBIND - 1

// ****************************************
// 	Variable Kinds
// ****************************************
// not sure if these are ever used in MSFT format data
// %VAR_PERINSTANCE	= 0
// %VAR_STATIC		= %VAR_PERINSTANCE + 1
// %VAR_CONST			= %VAR_STATIC + 1
// %VAR_DISPATCH		= %VAR_CONST + 1
%VAR_PERINSTANCE	= 0
%VAR_STATIC			= 1
%VAR_CONST			= 2
%VAR_DISPATCH		= 3


// ****************************************
// 	Variable Flags
// ****************************************
%VARFLAG_FREADONLY			= &H00001
%VARFLAG_FSOURCE			= &H00002
%VARFLAG_FBINDABLE			= &H00004
%VARFLAG_FREQUESTEDIT		= &H00008
%VARFLAG_FDISPLAYBIND		= &H00010
%VARFLAG_FDEFAULTBIND		= &H00020
%VARFLAG_FHIDDEN			= &H00040
%VARFLAG_FRESTRICTED		= &H00080
%VARFLAG_FDEFAULTCOLLELEM	= &H00100
%VARFLAG_FUIDEFAULT			= &H00200
%VARFLAG_FNONBROWSABLE		= &H00400
%VARFLAG_FREPLACEABLE		= &H00800
%VARFLAG_FIMMEDIATEBIND		= &H01000




// ****************************************
// 	TypeLib UDTs
// ****************************************


Union TYPEDESCUNION
	lptdesc		As Dword Ptr // TYPEDESC Ptr
	lpadesc		As Dword Ptr // ARRAYDESC Ptr
	hRefType	As Long // hRefType
End Union // TYPEDESCUNION


Type tagTYPEDESC
	u	As TYPEDESCUNION
	vt	As Long // VARTYPE
End Type // TYPEDESC


Type tagPARAMDESCEX
	cBytes				As Dword // ULONG
	// varDefaultValue		As Variant // VARIANTARG
End Type // tagPARAMDESCEX


Type tagPARAMDESC
	pParamDescEx	As tagPARAMDESCEX // or a Ptr ?
	fParam			As Word // (USHORT)
End Type // tagPARAMDESC


Type tagIDLDESC
	Res		As Dword // Reserved
	fIDL	As Word // USHORT
End Type tagIDLDESC


Union ELEMDESCUNION
	idldesc		As tagIDLDESC	// info for remoting the element
	ParamDesc	As tagPARAMDESC	// info about the parameter
End Union


Type tagELEMDESC
	tdesc	As tagTYPEDESC // the type of the element
	u		As ELEMDESCUNION
End Type // tagELEMDESC



// 	MSFT typelibs
// These are TypeLibs created with ICreateTypeLib2 structure of the typelib type2 header
// it is at the beginning of a type lib file
public class  TlbHeader
	uint Magic1	; // &H5446534D "MSFT"								00
	uint Magic2	; // &H00010002 version number?
	uint oGUID	; // position of libid in GUID table (should be,  else -1)
	uint LCID	; // locale id
	uint LCID2	; // 												10
	uint fVar	; // (largely) unknown flags
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
					; 	 	// in custom data/GUID offset table
	uint Res1		;		 // unknown always: &H20 (GUID hash size?)
	uint Res2		;		 // unknown always: &H80 (name hash size?)
	uint oDispatch	;		 // hRefType to IDispatch, or -1 if no IDispatch
	uint nImpInfos	;		 // number of ImpInfos								50
	// oFileName			As Long // offset to typelib file name in string table
    } // TlbHeader


// segments in the type lib file have a structure like this:
public class SegDesc
{
	uint Offs	; // absolute offset in file
	uint nLen	; // length of segment
	uint Res01	; // unknown always -1
	uint Res02	; // unknown always &H0F in the header
					 // &H03 in the TypeInfo_data
}

// 	segment directory
Type MSFT_SegDir
	pTypInfo			As SegDesc // 1 - TypeInfo table
	pImpInfo			As SegDesc // 2 - table with info for imported types
	pImpFiles			As SegDesc // 3 - import libaries
	pRefer				As SegDesc // 4 - References table
	pLibs				As SegDesc // 5 - always exists, alway same size (&H80)
						  			 //   - hash table with offsets to GUID?????
	pGUID				As SegDesc // 6 - all GUIDs are stored here together with
						  			 //   - offset in some table????
	Unk01				As SegDesc // 7 - always created, always same size (&H0200)
						  			 //   - contains offsets into the name table
	pNames				As SegDesc // 8 - name table
	pStrings			As SegDesc // 9 - string table
	pTypDesc			As SegDesc // A - type description table
	pArryDesc			As SegDesc // B - array descriptions
	pCustData			As SegDesc // C - data table, used for custom data and default
				  					 //   - parameter values
	pCDGuids			As SegDesc // D - table with offsets for the GUIDs and into
									 //   - the custom data table
	Unk02				As SegDesc // E - unknown
	Unk03				As SegDesc // F - unknown
} // MSFT_SegDir


// type info data
public class  TypeInfo
	TypeKind			As Byte	// TKIND_xxx
	Align				As Byte	// alignment
	Unk					As Integer	// unknown
	oFunRec				As Long // 	- points past the file, if no elements
	Alloc				As Long // 	Recommended (or required?) amount of memory to allocate for...?
	Reconst				As Long // 	size of reconstituted TypeInfo data
	Res01				As Long // 10 - always? 3
	Res02				As Long // 	- always? zero
	nFuncs				As Integer //  - count of functions
	nProps				As Integer //  - count of properties
	Res03				As Long //    - always? zero
	Res04				As Long // 20 - always? zero
	Res05				As Long //    - always? zero
	Res06				As Long //    - always? zero
	oGUID				As Long //    - position in GUID table
	fType				As Long // 30 - Typeflags
	oName				As Long //    - offset in name table
	Version				As Long //    - element version
	DocStr				As Long	//    - offset of docstring in string tab
	HelpStrCnt			As Long // 40
	HelpCntxt			As Long
	oCustData			As Long	//    - offset in custom data table
#If %Def(%WORDS_BIGENDIAN)		// 
	cVft		As Integer // virtual table size, not including inherits
	nImplTypes	As Integer // number of implemented interfaces
#Else
	nImplTypes	As Integer // number of implemented interfaces
	cVft	As Integer // virtual table size, not including inherits
#endif
	Unk03		As Long  // 50 - size in bytes, at least for structures

	Type1		As Long  // 	- position in type description table
						 // 	- or in base interfaces
						 // 	- if coclass: offset in reftable
						 // 	- if interface: reference to inherited if
						 // 	- if module: offset to dllname in name table
	Type2		As Long  // 	- if &H8000, entry above is valid, else it is zero?
	Res07		As Long  // 	- always? 0
	Res08		As Long  // 60 - always? -1
End Type // TypeInfo



// information on imported types
Type ImportInfo
	Count		As Integer // count
	Flags		As Byte // if <> 0 then oGUID is an offset to GUID, else it// s a TypeInfo index in the specified typelib
	TypeKind	As Byte	//  TKIND of reference
	oImpFile	As Long // offset in the Import File table
	oGuid		As Long // offset in GUID table or TypeInfo index (see bit 16 of Res0)
} // ImportInfo



// information on imported files
public class TlbImpLib
{
	oGUID	As Long
	LCID	As Long
	MajVer	As Word
	MinVer	As Word
	cSize	As Word // divide by 4 to get the length of the file name
} // TlbImpLib



// Structure of the reference data
Type RefRecord
	// either offset in TypeInfo table, then it// s a multiple of 4...
	// or offset in the external reference table with an offset of 1
	RefType				As Long
	Flags				As Long // ?
	oCustData			As Long // custom data
	oNext				As Long // next offset, -1 if last
End Type // RefRecord



// this is how a GUID is stored
Type GuidEntry
	oGUID		As String * 16
	//  = -2 for a TypeLib GUID
	// TypeInfo offset for TypeInfo GUID,
	// Otherwise, the low two bits:
	// 	= 01 for an imported TypeInfo
	// 	= 10 for an imported TypeLib (used by imported TypeInfos)
	hRefType	As Long
	NextHash	As Long	// offset to next GUID in the hash bucket
End Type // GuidEntry



// some data preceding entries in the name table
Type NameIntro
	// is -1 if name is for neither a TypeInfo,
	// a variable, or a function (that is, name
	// is for a typelib or a function parameter).
	// otherwise is the offset of the first
	// TypeInfo that this name refers to (either
	// to the TypeInfo itself or to a member of
	// the TypeInfo
	hRefType	As Long

	NextHash	As Long	// offset to next name in the hash bucket

	// only lower 8 bits are valid,
	// lower-middle 8 Bits are unknown (flags?),
	// upper 16 Bits are hash code
	cName		As Long
End Type // NameIntro


// this is only here to illustrate the storage format for strings
// Type TlbString
// 	nLen	As Word // length of string
// 	zStr	As String * nLen // text of string
// 	zPad	As String$(?, "W") // pad to Dword alignment
// End Type TlbString



Type TYPEDESC // a substitute for a tagTYPEDESC to simplify the code
	v1	As Integer
	v2	As Integer
	v3	As Integer
	v4	As Integer
End Type // TYPEDESC



// type for arrays
Type SafeArrayBound
	nElements	As Dword
	lLBound		As Long
End Type // SafeArrayBound

Type ARRAYDESC
	u			As TYPEDESCUNION
	nDims		As Word
	tVar		As Word // VARTYPE
	Bounds(0)	As SafeArrayBound
End Type // ARRAYDESC

Type CArrayDesc
	Desc	As ARRAYDESC
	Bnd		As SafeArrayBound
End Type



// Custom data table entries are Dword aligned by padding with (usually) "W"
// Type CustomData
// 	nLen				As Word // length of nLen plus bData in Words
// 	cData(nLen - 1)		As Word	// 
// End Type // CustomData



// the custom data/GUID table directory has entries like this
Type CDGuid
	oGUID		As Long
	oData		As Long
	oNext		As Long // next offset in the table, -1 if it// s the last
End Type // CDGuid



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
	ushort Flags				As Integer

	Res1				As Long // always(?) zero

#if WORDS_BIGENDIAN
	cFuncDesc			As Integer // size of reconstituted FUNCDESC and related structs
	oVtable				As Integer // offset in vtable
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

	short nParams				As Integer // parameter count
	short Unk2				As Integer

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
	ushort DataType	As Integer
	ushort Flags		As Integer
	uint oName		As Long
	uint fParam		As Long
} // ParamInfo



// 	Property description data
//  The size of the required fields of the PropRecord structure
// is 20 (= &H14)
// These exist in arrays along with zero or more "FuncRecord"
// elements. Each array is preceded by a Dword stating the total
// size of the array.
public class PropRecord
{
	RecSize				As Word // size of PropRecord
	PropNum				As Word // Property number?
	DataType	 		As Integer // data type of the variable
	Flags				As Integer // VarFlags
#If %Def(%WORDS_BIGENDIAN)
	cVarDesc			As Integer // size of reconstituted VARDESC and related structs
	VarKind	  			As Integer // VarKind
#Else
	VarKind				As Integer // VarKind --- %VAR and %VarFlags
	cVarDesc			As Integer // size of reconstituted VARDESC and related structs
#endif
	OffsValue			As Long // value of the variable or the offset in the data structure
	// ***** End of required fields *****

	// Optional attribute fields, the number of them is variable
	// and are determined from the record size (if there// s room
	// for it, then it// s there...)
	Unk					As Long
	HelpCntxt			As Long
	oHelpStr			As Long
	Res				 	As Long // unknown (-1)
	oCustData			As Long // custom data for variable
	HelpStrCnt			As Long
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

{ Lcid=0x0401, Code="ARA", Desc="Arabic Saudi Arabia" },{ Lcid=0x0801, Code="ARI", Desc="Arabic Iraq" },
{ Lcid=0x0C01, Code="ARE", Desc="Arabic Egypt" },
{ Lcid=0x1001, Code="ARL", Desc="Arabic Libya" },
{ Lcid=0x1401, Code="ARG", Desc="Arabic Algeria" },
{ Lcid=0x1801, Code="ARM", Desc="Arabic Morocco" },
{ Lcid=0x1C01, Code="ART", Desc="Arabic Tunisia" },
{ Lcid=0x2001, Code="ARO", Desc="Arabic Oman" },
{ Lcid=0x2401, Code="ARY", Desc="Arabic Yemen" },
{ Lcid=0x2801, Code="ARS", Desc="Arabic Syria" },
{ Lcid=0x2C01, Code="ARJ", Desc="Arabic Jordan" },
{ Lcid=0x3001, Code="ARB", Desc="Arabic Lebanon" },
{ Lcid=0x3401, Code="ARK", Desc="Arabic Kuwait" },
{ Lcid=0x3801, Code="ARU", Desc="Arabic U.A.E." },
{ Lcid=0x3C01, Code="ARH", Desc="Arabic Bahrain" },
{ Lcid=0x4001, Code="ARQ", Desc="Arabic Qatar" },
Data &H0402, "BGR", Bulgarian Bulgaria
{ Lcid=0x0403, Code="CAT", Desc="Catalan Spain" },
{ Lcid=0x0404, Code="CHT", Desc="Chinese Taiwan" },
{ Lcid=0x0804, Code="CHS", Desc="Chinese PRC" },
{ Lcid=0x0C04, Code="ZHH", Desc="Chinese Hong Kong" },
{ Lcid=0x1004, Code="ZHI", Desc="Chinese Singapore" },
{ Lcid=0x1404, Code="ZHM", Desc="Chinese Macau" },
{ Lcid=0x0405, Code="CSY", Desc="Czech Czech Republic" },
{ Lcid=0x0406, Code="DAN", Desc="Danish Denmark" },
{ Lcid=0x0407, Code="GERMANY", Desc="German Germany" },
{ Lcid=0x0807, Code="DES", Desc="German Switzerland" },
{ Lcid=0x0C07, Code="DEA", Desc="German Austria" },
{ Lcid=0x1007, Code="DEL", Desc="German Luxembourg" },
{ Lcid=0x1407, Code="DEC", Desc="German Liechtenstein" },
{ Lcid=0x0408, Code="ELL", Desc="Greek Greece" },
{ Lcid=0x0409, Code="USA", Desc="English United States" },
{ Lcid=0x0809, Code="ENG", Desc="English United Kingdom" },
{ Lcid=0x0C09, Code="ENA", Desc="English Australia" },
{ Lcid=0x1009, Code="ENC", Desc="English Canada" },
{ Lcid=0x1409, Code="ENZ", Desc="English New Zealand" },
{ Lcid=0x1809, Code="ENI", Desc="English Ireland" },
{ Lcid=0x1C09, Code="ENS", Desc="English South Africa" },
{ Lcid=0x2009, Code="ENJ", Desc="English Jamaica" },
{ Lcid=0x2409, Code="ENB", Desc="English Caribbean" },
{ Lcid=0x2809, Code="ENL", Desc="English Belize" },
{ Lcid=0x2C09, Code="ENT", Desc="English Trinidad" },
{ Lcid=0x3009, Code="ENW", Desc="English Zimbabwe" },
{ Lcid=0x3409, Code="ENP", Desc="English Philippines" },
{ Lcid=0x040A, Code="SPAIN", Desc="Spanish Spain" },
{ Lcid=0x080A, Code="ESM", Desc="Spanish Mexico" },
{ Lcid=0x0C0A, Code="ESN", Desc="Spanish Spain (International Sort)" },
{ Lcid=0x100A, Code="ESG", Desc="Spanish Guatemala" },
{ Lcid=0x140A, Code="ESC", Desc="Spanish Costa Rica" },
{ Lcid=0x180A, Code="ESA", Desc="Spanish Panama" },
{ Lcid=0x1C0A, Code="ESD", Desc="Spanish Dominican Republic" },
{ Lcid=0x200A, Code="ESV", Desc="Spanish Venezuela" },
{ Lcid=0x240A, Code="ESO", Desc="Spanish Colombia" },
{ Lcid=0x280A, Code="ESR", Desc="Spanish Peru" },
{ Lcid=0x2C0A, Code="ESS", Desc="Spanish Argentina" },
{ Lcid=0x300A, Code="ESF", Desc="Spanish Ecuador" },
{ Lcid=0x340A, Code="ESL", Desc="Spanish Chile" },
{ Lcid=0x380A, Code="ESY", Desc="Spanish Uruguay" },
{ Lcid=0x3C0A, Code="ESZ", Desc="Spanish Paraguay" },
{ Lcid=0x400A, Code="ESB", Desc="Spanish Bolivia" },
{ Lcid=0x440A, Code="ESE", Desc="Spanish El Salvador" },
{ Lcid=0x480A, Code="ESH", Desc="Spanish Honduras" },
{ Lcid=0x4C0A, Code="ESI", Desc="Spanish Nicaragua" },
{ Lcid=0x500A, Code="ESU", Desc="Spanish Puerto Rico" },
{ Lcid=0x040B, Code="FIN", Desc="Finnish Finland" },
{ Lcid=0x040C, Code="FRANCE", Desc="French France" },
{ Lcid=0x080C, Code="FRB", Desc="French Belgium" },
{ Lcid=0x0C0C, Code="FRC", Desc="French Canada" },
{ Lcid=0x100C, Code="FRS", Desc="French Switzerland" },
{ Lcid=0x140C, Code="FRL", Desc="French Luxembourg" },
{ Lcid=0x180C, Code="FRM", Desc="French Monaco" },
{ Lcid=0x040D, Code="HEB", Desc="Hebrew Israel" },
{ Lcid=0x040E, Code="HUN", Desc="Hungarian Hungary" },
{ Lcid=0x040F, Code="ISL", Desc="Icelandic Iceland" },
{ Lcid=0x0410, Code="ITALY", Desc="Italian Italy" },
{ Lcid=0x0810, Code="ITS", Desc="Italian Switzerland" },
{ Lcid=0x0411, Code="JAPAN", Desc="Japanese Japan" },
{ Lcid=0x0412, Code="KOREA", Desc="Korean Korea" },
{ Lcid=0x0413, Code="NLD", Desc="Dutch Netherlands" },
{ Lcid=0x0813, Code="NLB", Desc="Dutch Belgium" },
{ Lcid=0x0414, Code="NOR", Desc="Norwegian Norway (Bokmål)" },
{ Lcid=0x0814, Code="NON", Desc="Norwegian Norway (Nynorsk)" },
{ Lcid=0x0415, Code="PLK", Desc="Polish Poland" },
{ Lcid=0x0416, Code="BRAZIL", Desc="Portuguese Brazil" },
{ Lcid=0x0816, Code="PTG", Desc="Portuguese Portugal" },
{ Lcid=0x0418, Code="ROM", Desc="Romanian Romania" },
{ Lcid=0x0419, Code="RUS", Desc="Russian Russia" },
{ Lcid=0x041A, Code="HRV", Desc="Croatian Croatia" },
{ Lcid=0x081A, Code="SRL", Desc="Serbian Serbia (Latin)" },
{ Lcid=0x0C1A, Code="SRB", Desc="Serbian Serbia (Cyrillic)" },
{ Lcid=0x041B, Code="SKY", Desc="Slovak Slovakia" },
{ Lcid=0x041C, Code="SQI", Desc="Albanian Albania" },
{ Lcid=0x041D, Code="SVE", Desc="Swedish Sweden" },
{ Lcid=0x081D, Code="SVF", Desc="Swedish Finland" },
{ Lcid=0x041E, Code="THA", Desc="Thai Thailand" },
{ Lcid=0x041F, Code="TRK", Desc="Turkish Turkey" },
{ Lcid=0x0420, Code="URP", Desc="Urdu Pakistan" },
{ Lcid=0x0421, Code="IND", Desc="Indonesian Indonesia" },
{ Lcid=0x0422, Code="UKR", Desc="Ukrainian Ukraine" },
{ Lcid=0x0423, Code="BEL", Desc="Belarusian Belarus" },
{ Lcid=0x0424, Code="SLV", Desc="Slovene Slovenia" },
{ Lcid=0x0425, Code="ETI", Desc="Estonian Estonia" },
{ Lcid=0x0426, Code="LVI", Desc="Latvian Latvia" },
{ Lcid=0x0427, Code="LTH", Desc="Lithuanian Lithuania" },
{ Lcid=0x0827, Code="LTC", Desc="Classic Lithuanian Lithuania" },
{ Lcid=0x0429, Code="FAR", Desc="Farsi Iran" },
{ Lcid=0x042A, Code="VIT", Desc="Vietnamese Viet Nam" },
{ Lcid=0x042B, Code="HYE", Desc="Armenian Armenia" },
{ Lcid=0x042C, Code="AZE", Desc="Azeri Azerbaijan (Latin)" },
{ Lcid=0x082C, Code="AZE", Desc="Azeri Azerbaijan (Cyrillic)" },
{ Lcid=0x042D, Code="EUQ", Desc="Basque Spain" },
{ Lcid=0x042F, Code="MKI", Desc="Macedonian Macedonia" },
{ Lcid=0x0436, Code="AFK", Desc="Afrikaans South Africa" },
{ Lcid=0x0437, Code="KAT", Desc="Georgian Georgia" },
{ Lcid=0x0438, Code="FOS", Desc="Faeroese Faeroe Islands" },
{ Lcid=0x0439, Code="HIN", Desc="Hindi India" },
{ Lcid=0x043E, Code="MSL", Desc="Malay Malaysia" },
{ Lcid=0x083E, Code="MSB", Desc="Malay Brunei Darussalam" },
{ Lcid=0x043F, Code="KAZ", Desc="Kazak Kazakstan" },
{ Lcid=0x0441, Code="SWK", Desc="Swahili Kenya" },
{ Lcid=0x0443, Code="UZB", Desc="Uzbek Uzbekistan (Latin)" },
{ Lcid=0x0843, Code="UZB", Desc="Uzbek Uzbekistan (Cyrillic)" },
{ Lcid=0x0444, Code="TAT", Desc="Tatar Tatarstan" },
{ Lcid=0x0445, Code="BEN", Desc="Bengali India" },
{ Lcid=0x0446, Code="PAN", Desc="Punjabi India" },
{ Lcid=0x0447, Code="GUJ", Desc="Gujarati India" },
{ Lcid=0x0448, Code="ORI", Desc="Oriya India" },
{ Lcid=0x0449, Code="TAM", Desc="Tamil India" },
{ Lcid=0x044A, Code="TEL", Desc="Telugu India" },
{ Lcid=0x044B, Code="KAN", Desc="Kannada India" },
{ Lcid=0x044C, Code="MAL", Desc="Malayalam India" },
{ Lcid=0x044D, Code="ASM", Desc="Assamese India" },
{ Lcid=0x044E, Code="MAR", Desc="Marathi India" },
{ Lcid=0x044F, Code="SAN", Desc="Sanskrit India" },
{ Lcid=0x0457, Code="KOK", Desc="Konkani India" },
{ Lcid=0x0000, Code="Language-Neutral", Desc="Language-Neutral" },
{ Lcid=0x0400, Code="Process Default Language", Desc="Process Default Language" }
};

Local n		As Long

	For n = 1 To DataCount Step 3
		If lcid = Val(Read$(n)) Then
			Function = Read$(n + 2)
			Exit For
		End If
	Next n

End Function // Locale

// ****************************************

string VarType(int pn) {
Local n		As Long
Local ls	As String

Data "Empty=0"
Data "Null=1"
Data "I2=2"
Data "I4=3"
Data "R4=4"
Data "R8=5"
Data "Cy=6"
Data "Date=7"
Data "BStr=8"
Data "Dispatch=9"
Data "Error=10"
Data "Bool=11"
Data "Variant=12"
Data "Unknown=13"
Data "Decimal=14"
Data
Data "I1=16"
Data "UI1=17"
Data "UI2=18"
Data "UI4=19"
Data "I8=20"
Data "UI8=21"
Data "Int=22"
Data "UInt=23"
Data "Void=24"
Data "HResult=25"
Data "Ptr=26"
Data "SafeArray=27"
Data "CArray=28"
Data "UserDefined=29"
Data "LPStr=30"
Data "LPWStr=31"
Data , , ,
Data "Record=36"
// 	end of continuous sequence
Data "FileTime=64"
Data "Blob=65"
Data "Stream=66"
Data "Storage=67"
Data "Streamed_Object=68"
Data "Stored_Object=69"
Data "Blob_Object=70"
Data "CF=71"
Data "ClsID=72"
// Data "Bstr_Blob=4095"

	pn = pn And %VT_TypeMask
	If pn <= 36 Then
		n = pn + 1
	Else
		For n = 37 To DataCount
			If Val(Remain$(Read$(n), "=")) = pn Then Exit For
		Next n
	End If

	ls = Extract$(Read$(n), "=")
	Function = IIf$(Len(ls), "VT_" & ls, "(Unknown)")

End Function // VarType

// ****************************************
// offs = zero-based offset into the GUID table
Function tlGuid(cs As String, SegDir As MSFT_SegDir, ByVal offs As Long) As String

	If offs => 0 Then
		Function = GuidTxt$(Mid$(cs, SegDir.pGUID.Offs + offs + 1, 16))
	End If

End Function // tlGuid

// ****************************************
// offs = zero-based offset into the name table
Function tlName(cs As String, SegDir As MSFT_SegDir, ByVal offs As Long) As String
Local NameInt		As NameIntro

	If offs => 0 Then
		offs = SegDir.pNames.Offs + offs + 1
		LSet NameInt = Mid$(cs, offs)
		Function = $Dq & Mid$(cs, offs + SizeOf(NameIntro), NameInt.cName And &H0FF) & $Dq
	End If

End Function // tlName

// ****************************************
// offs = zero-based offset into the string table
Function tlString(cs As String, SegDir As MSFT_SegDir, ByVal offs As Long) As String

	If offs => 0 And offs < Len(cs) Then
		offs = SegDir.pStrings.Offs + offs + 1
		Function = $Dq & Mid$(cs, offs + 2, CvWrd(cs, offs)) & $Dq
	End If

End Function // tlString


// ****************************************
// cs:		TypeLib data
// SegDir:	the segment directory
// pBase:		the zero-based offset to the FuncRec
// nFunc:		number of functions
// nProp:		number of properties
// fo:		file handle
#if DISPLAY
Function DisFunction(cs As String, SegDir As MSFT_SegDir, ByVal pBase As Dword, ByVal nFunc As Long, ByVal nProp As Long) As Long
Local d				As Long
Local i				As Long
Local j				As Long
Local n				As Long
Local ub			As Long
Local p				As Dword
Local pTmp			As Dword
Local iElem			As Long
Local nAttr			As Long
Local ArraySize		As Long
Local oParamInfo	As Long
Local oDefValue		As Long

Local ls			As String
Local FuncRec		As FuncRecord
Local ParmInfo		As ParamInfo
Local ElemDesc		As tagELEMDESC
Local ParamDesc		As tagPARAMDESC
Local NameInt		As NameIntro
Local PropRec		As PropRecord


	p = pBase
	ArraySize = CvDwd(cs, pBase + 1)
	Print# fo,
	Print# fo, "	Function record array size:	"; Hex$(ArraySize, 8)
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

		Print# fo,
		Print# fo, "	----------------------------------------"
		Print# fo, "				Functions:"

		For i = 1 To nFunc

			LSet FuncRec = Mid$(cs, p)

			Print# fo, "	----------------------------------------"
			Print# fo, "	ID:			"; Hex$(IdElem(iElem), 8); " =="; Str$(IdElem(iElem))
			Print# fo, "	Name:		"; Hex$(oName(iElem), 8); " ==> "; tlName(cs, SegDir, oName(iElem))
			Print# fo, "	Reference:	"; Hex$(Refer(iElem), 8) // offset to the corresponding function record
			Incr iElem
			Print# fo, "	Record size:	"; Hex$(FuncRec.RecSize, 4)
			Print# fo, "	Unknown 1:		"; Hex$(FuncRec.Unk1, 4)
			Print# fo, "	Flags:			"; Hex$(FuncRec.Flags, 4); IIf$(FuncRec.Flags < 0, " = " & VarType(FuncRec.Flags), "")
			Print# fo, "	DataType:		"; Hex$(FuncRec.DataType, 4); " = " & VarType(FuncRec.DataType)

#If %Def(%Study)
			Print# fo,
			Print# fo, "	Reserved 1:		"; Hex$(FuncRec.Res1, 8)
#EndIf

			Print# fo, "	Vtable offset:	"; Hex$(FuncRec.oVtable, 4)
			Print# fo, "	Func Desc Size:	"; Hex$(FuncRec.cFuncDesc, 4)


			// 	FKCCIC
			// The bits in FKCCIC have the following meanings:
			// 0 - 2 = function kind (eg virtual)
			// 3 - 6 = Invocation kind
			// 7 means custom data is present
			// 8 - 11 = calling convention
			// 12 means one or more parameters have a default value
			Print# fo,
			Print# fo, "	FKCCIC (raw):	"; Hex$(FuncRec.FKCCIC, 8)
			If (FuncRec.FKCCIC And &H01000) Then Print# fo, "		Default value(s) present"
			If (FuncRec.FKCCIC And &H040000) Then Print# fo, "		oEntry is numeric"
			If (FuncRec.FKCCIC And &H080) Then Print# fo, "		Custom data present"

			d = FuncRec.FKCCIC And &H0F00
			Shift Right d, 8
			Print# fo, "		Calling convention:	"; Hex$(d, 2); " = " & Parse$($CallConv, d + 1)

			d = FuncRec.FKCCIC And &H078 // this is a bit field
			Shift Right d, 3
			Print# fo, "		Invocation kind:	"; Hex$(d, 2); " = ";
			For n = 4 To 1 Step -1
				If (d And %INVOKE_PROPERTYPUTREF) Then
					Print# fo, Parse$($InvoKind, n);
					If (d And &H07) Then Print# fo, ", ";
				End If
				Shift Left d, 1
			Next n
			Print# fo,

			d = FuncRec.FKCCIC And 7
			Print# fo, "		Function kind:		"; Hex$(d, 2); " = " & Parse$($FuncKind, d + 1)


			// 	Algorithm
			// 1) Dim the ParamInfo array at the end of the available space
			// 2) If (FKCCIC And &H1000) then Dim an array of default values just before the ParamInfo array
			// 3) Assume anything preceding the above arrays is the function// s optional data
			Print# fo,
			n = FuncRec.nParams
			Print# fo, "	Number of parameters:	"; Hex$(n, 4); " =="; Str$(n)

//#If %Def(%Study)
			Print# fo, "	Unknown 2:		"; Hex$(FuncRec.Unk2, 4)
//#EndIf

			oParamInfo = p + FuncRec.RecSize - (n * SizeOf(ParamInfo)) // must be one-based
			oDefValue = oParamInfo

			// If (FuncRec.FKCCIC And &H01000) Then // there might be default values present
			If (FuncRec.FKCCIC And &H01000) And (n > 0) Then // there might be default values present
				oDefValue = oDefValue - (n * 4)
				ReDim DefVal(n - 1)		As Long At StrPtr(cs) + oDefValue - 1 // must be zero-based
			End If


			// Dim array for the function// s optional data, if any
			ub = (((oDefValue - SizeOf(FuncRecord)) - p) \ 4) - 1
			If ub => 0 Then

				Print# fo, "		----------------------------------------"
				Print# fo, "		Optional Data:"
				ReDim OptData(ub)	As Long At StrPtr(cs) + p + SizeOf(FuncRecord) - 1 // must be zero-based

				Print# fo, "		HelpContext:		"; Hex$(OptData(0), 8)
				If ub < 1 Then Exit If

				Print# fo, "		HelpString:			"; Hex$(OptData(1), 8);
				Print# fo, " ==> " & tlString(cs, SegDir, OptData(1))
				If ub < 2 Then Exit If

				Print# fo, "		Entry:				"; Hex$(OptData(2), 8)

//#If %Def(%Study)
				If ub < 3 Then Exit If

				Print# fo, "		Reserved09:	"; Hex$(OptData(3), 8)
				If ub < 4 Then Exit If

				Print# fo, "		Reserved0A:	"; Hex$(OptData(4), 8)
//#EndIf
				If ub < 5 Then Exit If

				Print# fo, "		HelpStringContext:	"; Hex$(OptData(5), 8)
				If ub < 6 Then Exit If

				Print# fo, "		Custom Data:		"; Hex$(OptData(6), 8)

			End If
			Print# fo,


			For j = 0 To n - 1

				LSet ParmInfo = Mid$(cs, oParamInfo)
				Print# fo, "		----------------------------------------"
				Print# fo, "		Parameter number:	"; Str$(j + 1)
				Print# fo, "		DataType:			"; Hex$(ParmInfo.DataType, 4); IIf$(ParmInfo.DataType => 0, " = " & VarType(ParmInfo.DataType), "")
				Print# fo, "		Flags:				"; Hex$(ParmInfo.Flags, 4); " = "; VarType(ParmInfo.Flags)
				Print# fo, "		Name:				"; Hex$(ParmInfo.oName, 8); " ==> "; tlName(cs, SegDir, ParmInfo.oName)
				Print# fo, "		ParamFlags:			"; Hex$(ParmInfo.fParam, 8); " =	";
				If ParmInfo.fParam Then
					d = ParmInfo.fParam
					For n = 7 To 1 Step -1 //  7 = ParseCount($ParamFlags)
						If (d And %PARAMFLAG_FHASCUSTDATA) Then
							Print# fo, Parse$($ParamFlags, n);
							If (d And &H03F) Then Print# fo, ", ";
						End If
						Shift Left d, 1
					Next n
					Print# fo,
				Else
					Print# fo, "(none)"
				End If

				// If (ParmInfo.fParam And %PARAMFLAG_FHASDEFAULT) Then
				If (UBound(DefVal) => 0) And (ParmInfo.fParam And %PARAMFLAG_FHASDEFAULT) Then

					If DefVal(j) < 0 Then // the default value is in the lower three bytes
						DefVal(j) = DefVal(j) And &H0FFFFFF
					Else // it// s an offset into the CustomData table
						DefVal(j) = Cvl(cs, SegDir.pCustData.Offs + DefVal(j) + 3)
					End If
					Print# fo, "		Default Value:		"; Hex$(DefVal(j), 8); " =="; Str$(DefVal(j))

				End If

				oParamInfo = oParamInfo + SizeOf(ParamInfo)

			Next j

			p = p + FuncRec.RecSize

		Next i

	End If // nFunc



	// do the properties
	If nProp Then

		Print# fo,
		Print# fo, "	----------------------------------------"
		Print# fo, "		 Properties:"

		For i = 1 To nProp

			LSet PropRec = Mid$(cs, p)

			Print# fo, "	----------------------------------------"
			Print# fo, "	ID:			"; Hex$(IdElem(iElem), 8); " =="; Str$(IdElem(iElem))
			Print# fo, "	Name:		"; Hex$(oName(iElem), 8); " ==> "; tlName(cs, SegDir, oName(iElem))
			Print# fo, "	Reference:	"; Hex$(Refer(iElem), 8) // offset to the corresponding function record
			Incr iElem
			Print# fo, "	Record size (low-byte):	"; Hex$(PropRec.RecSize, 4)
			Print# fo, "	Property number?:		"; Hex$(PropRec.PropNum, 4)
			Print# fo, "	Flags:					"; Hex$(PropRec.Flags, 4); IIf$(PropRec.Flags < 0, " = " & VarType(PropRec.Flags), "")
			Print# fo, "	DataType:				"; Hex$(PropRec.DataType, 4); " = "; VarType(PropRec.DataType)

			Print# fo, "	Variable kind:			"; Hex$(PropRec.VarKind, 4); " = ";
			d = PropRec.VarKind
			For n = 13 To 1 Step -1 //  13 = ParseCount($VarFlags)
				If (d And %VARFLAG_FIMMEDIATEBIND) Then
					Print# fo, Parse$($VarFlags, n);
					If (d And &H0FFF) Then Print# fo, ", ";
				End If
				Shift Left d, 1
			Next n
			Print# fo,

			Print# fo, "	Variable desc size:		"; Hex$(PropRec.cVarDesc, 4)
			Print# fo, "	Value/Offset:			"; Hex$(PropRec.OffsValue, 8)

//If %Def(%Study)
			Print# fo, "	Unknown:				"; Hex$(PropRec.Unk, 8)/
//EndIf

			If PropRec.RecSize > 20 Then // 20 = (5 * SizeOf(Long))

				Print# fo, "	HelpContext:			"; Hex$(PropRec.HelpCntxt, 8)

				If PropRec.RecSize > 24 Then // 24 = (6 * SizeOf(Long))
					Print# fo, "	HelpString:	"; Hex$(PropRec.oHelpStr, 8);
					Print# fo, " ==> "; tlString(cs, SegDir, PropRec.oHelpStr)

					If PropRec.RecSize > 32 Then // 32 = (8 * SizeOf(Long))

						If PropRec.RecSize > 36 Then // 36 = (9 * SizeOf(Long))
//If %Def(%Study)
							Print# fo, "	Reserved:			"; Hex$(PropRec.Res, 8)
//EndIf
							Print# fo, "	HelpStringContext:	"; Hex$(PropRec.HelpStrCnt, 8)
						End If

					End If

				End If

			End If

			Print# fo,
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
			Print# fo,
			Print# fo, "	----------------------------------------"
			Print# fo, "		 Other Function Data:"
		End If

		For iElem = 0 To ub Step 1

			If n = nProp Then // the functions are done, so do the properties
				Print# fo,
				Print# fo, "	----------------------------------------"
				Print# fo, "		 Other Property Data:"
			End If
			Decr n

			// ID number of the function or property
			Print# fo, "	----------------------------------------"
			Print# fo, "	ID:			"; Hex$(IdElem(iElem), 8); " =="; Str$(IdElem(iElem))

			// offset to names in the name table
			Print# fo, "	Name:		"; Hex$(oName(iElem), 8);
			Print# fo, " ==> "; tlName(cs, SegDir, oName(iElem))

			// offset to the corresponding function record
			Print# fo, "	Reference:	"; Hex$(Refer(iElem), 8)
			Print# fo,

		Next iElem

	End If

//EndIf

	// Function =

End Function // DisFunction

// ****************************************
// cs contains the TypeLib data
// ls contains the name of the source file
Function DisTypeLib(cs As String) As Dword
Local d			 As Long
Local i			 As Long
Local n			 As Long
Local p			 As Long
Local fName		 As Long // TypeLib file name flag
Local TlbHdr	 As TlbHeader
Local SegDir	 As MSFT_SegDir
Local TypInfo	 As TypeInfo
Local ImpInfo	 As ImportInfo
Local IFileInfo	 As TlbImpLib
Local RefRec	 As RefRecord
Local GuidEnt	 As GuidEntry
Local NameInt	 As NameIntro
Local TypDsc	 As TYPEDESC // a simplified substitute for a tagTYPEDESC
Local AryDsc	 As ARRAYDESC
Local SafBnd	 As SafeArrayBound

Local ls		As String


	// --------------------------------------
	// print the TypeLib Header
	LSet TlbHdr = cs

	// get the segment directory in advance
	fName = IIf&((TlbHdr.fVar And %HELPDLLFLAG), 4, 0)
	p = SizeOf(TlbHdr) + (4 * (TlbHdr.nTypeInfo)) + fName + 1
	LSet SegDir = Mid$(cs, p)


	Print# fo, "*************************"
	Print# fo, "TypeLib Header"
	Print# fo, "*************************"
	Print# fo, "Magic 1:		"; Hex$(TlbHdr.Magic1, 8); " = "; Left$(cs, 4)	// &H5446534D """MSFT"""
	Print# fo, "Magic 2:		"; Hex$(TlbHdr.Magic2, 8)	// &H00010002 version number?

	// position of libid in guid table (should be, else -1)
	Print# fo, "GUID:			"; Hex$(TlbHdr.oGUID, 8); " ==> "; tlGuid(cs, SegDir, TlbHdr.oGUID)

	Print# fo, "Locale ID:		"; Hex$(TlbHdr.lcid, 8); " = "; Locale(TlbHdr.lcid) // locale id
	Print# fo, "Locale ID 2:	"; Hex$(TlbHdr.lcid2, 8)

	// fVar (largely unknown):
	// 	* the lower nibble is SysKind
	// 	* bit 5 is set if a helpfile is defined
	// 	* bit 8 is set if a help dll is defined
	Print# fo, "VarFlags:		"; Hex$(TlbHdr.fVar, 8)
	Print# fo, "	System:		"; Parse$($SysKind, (TlbHdr.fVar And 7) + 1)
	Print# fo, "	Help file"; IIf$((TlbHdr.fVar And 16), " ", " not "); "specified"
	Print# fo, "	Help file is"; IIf$((TlbHdr.fVar And 256), " ", " not "); "in a DLL"

	Print# fo, "Version:	"; Hex$(TlbHdr.version, 8)
	Print# fo, "Flags:		"; Hex$(TlbHdr.Flags, 8)

	Print# fo, "TypeInfo count:	"; Hex$(TlbHdr.nTypeInfo, 8); " =="; Str$(TlbHdr.nTypeInfo)	// number of TypeInfo// s

	Print# fo, "HelpString:			"; Hex$(TlbHdr.HelpStr, 8);	// position of HelpString in stringtable
	Print# fo, " ==> "; tlString(cs, SegDir, TlbHdr.HelpStr)

	Print# fo, "HelpStringContext:	"; Hex$(TlbHdr.HelpStrCnt, 8)
	Print# fo, "HelpContext:		"; Hex$(TlbHdr.HelpCntxt, 8)

	Print# fo, "Name Table:"
	Print# fo, "	Names count:	"; Hex$(TlbHdr.nName, 8); " =="; Str$(TlbHdr.nName)	// number of names in name table
	Print# fo, "	Characters:		"; Hex$(TlbHdr.nChars, 8); " =="; Str$(TlbHdr.nChars)	// number of characters in name table

	Print# fo,
	Print# fo, "TypeLib Name:	"; Hex$(TlbHdr.oName, 8); " ==> "; tlName(cs, SegDir, TlbHdr.oName) // offset of name in name table
	Print# fo, "Helpfile:		"; Hex$(TlbHdr.HelpFile, 8); " ==> "; tlString(cs, SegDir, TlbHdr.HelpFile) // position of helpfile in stringtable

	// if -1 no custom data, else it is offset in custom data/guid offset table
	Print# fo, "Custom data offset:	"; Hex$(TlbHdr.CustDat, 8)

//If %Def(%Study)
	Print# fo, "Reserved1:	"; Hex$(TlbHdr.Res1, 8)		// unknown always: &H20 (guid hash size?)
	Print# fo, "Reserved2:	"; Hex$(TlbHdr.Res2, 8)		// unknown always: &H80 (name hash size?)
//EndIf

	Print# fo, "IDispatch:			"; Hex$(TlbHdr.oDispatch, 8) // hRefType to IDispatch, or -1 if no IDispatch
	Print# fo, "ImportInfo count:	"; Hex$(TlbHdr.nImpInfos, 8); " =="; Str$(TlbHdr.nImpInfos) // number of ImpInfos

	p = SizeOf(TlbHdr) + 1
	If fName Then
		d = CvDwd(cs, p)
		Print# fo, "TypeLib file name:			"; Hex$(d, 8); " ==> "; tlString(cs, SegDir, d)
	End If

	// --------------------------------------
	// print the (TypeInfo ?) offsets
	Print# fo,
	Print# fo, "*************************"
	Print# fo, "Offsets to TypeInfo Data"
	Print# fo, "*************************"
	p = p + fName
	For n = 1 To TlbHdr.nTypeInfo
		Print# fo, Hex$(CvDwd(cs, p), 8)
		p = p + 4
		If p => Len(cs) Then Exit For
	Next n


	// --------------------------------------
	// print the segment directory contents
	Print# fo,
	Print# fo, "*************************"
	Print# fo, "Segment Directory (Segment name: Offset, Length)..."
	Print# fo, "*************************"
	Print# fo, "Type Info Table:		"; Hex$(SegDir.pTypInfo.Offs, 8); ", "; Hex$(SegDir.pTypInfo.nLen, 8)
	Print# fo, "Import Info:			"; Hex$(SegDir.pImpInfo.Offs, 8); ", "; Hex$(SegDir.pImpInfo.nLen, 8)
	Print# fo, "Imported Libraries:		"; Hex$(SegDir.pImpFiles.Offs, 8); ", "; Hex$(SegDir.pImpFiles.nLen, 8)
	Print# fo, "References Table:		"; Hex$(SegDir.pRefer.Offs, 8); ", "; Hex$(SegDir.pRefer.nLen, 8)
	Print# fo, "Lib Table:				"; Hex$(SegDir.pLibs.Offs, 8); ", "; Hex$(SegDir.pLibs.nLen, 8)
	Print# fo, "GUID Table:				"; Hex$(SegDir.pGUID.Offs, 8); ", "; Hex$(SegDir.pGUID.nLen, 8)
	Print# fo, "Unknown 01:				"; Hex$(SegDir.Unk01.Offs, 8); ", "; Hex$(SegDir.Unk01.nLen, 8)
	Print# fo, "Name Table:				"; Hex$(SegDir.pNames.Offs, 8); ", "; Hex$(SegDir.pNames.nLen, 8)
	Print# fo, "String Table:			"; Hex$(SegDir.pStrings.Offs, 8); ", "; Hex$(SegDir.pStrings.nLen, 8)
	Print# fo, "Type Descriptors:		"; Hex$(SegDir.pTypDesc.Offs, 8); ", "; Hex$(SegDir.pTypDesc.nLen, 8)
	Print# fo, "Array Descriptors:		"; Hex$(SegDir.pArryDesc.Offs, 8); ", "; Hex$(SegDir.pArryDesc.nLen, 8)
	Print# fo, "Custom Data:			"; Hex$(SegDir.pCustData.Offs, 8); ", "; Hex$(SegDir.pCustData.nLen, 8)
	Print# fo, "GUID Offsets:			"; Hex$(SegDir.pCDGuids.Offs, 8); ", "; Hex$(SegDir.pCDGuids.nLen, 8)
	Print# fo, "Unknown 02:				"; Hex$(SegDir.Unk02.Offs, 8); ", "; Hex$(SegDir.Unk02.nLen, 8)
	Print# fo, "Unknown 03:				"; Hex$(SegDir.Unk03.Offs, 8); ", "; Hex$(SegDir.Unk03.nLen, 8)


//If %Def(%Study)

	Local fAlert		As Long
	ReDim pSegTmp(14) As SegDesc At VarPtr(SegDir)
	For i = 0 To 14

		If pSegTmp(i).Res01 <> -1 Then
			Print# fo, "!!!"
			Incr fAlert
		End If

		If pSegTmp(i).Res02 <> &H0F Then
			Print# fo, "!!!"
			Incr fAlert
		End If

		If fAlert = 1 Then
			MsgBox "Interesting Reserved value found"
			Incr fAlert
		End If

	Next i

	// If fAlert Then
	Print# fo,
	Print# fo, "@@@:	Reserved Fields"
	Print# fo, "Segment name:	Res01, Res02..."
	Print# fo, "*************************"
	Print# fo, "Type Info Table:		"; Hex$(SegDir.pTypInfo.Res01, 8); ", "; Hex$(SegDir.pTypInfo.Res02, 8)
	Print# fo, "Import Info:			"; Hex$(SegDir.pImpInfo.Res01, 8); ", "; Hex$(SegDir.pImpInfo.Res02, 8)
	Print# fo, "Imported Libraries:		"; Hex$(SegDir.pImpFiles.Res01, 8); ", "; Hex$(SegDir.pImpFiles.Res02, 8)
	Print# fo, "References Table:		"; Hex$(SegDir.pRefer.Res01, 8); ", "; Hex$(SegDir.pRefer.Res02, 8)
	Print# fo, "Lib Table:				"; Hex$(SegDir.pLibs.Res01, 8); ", "; Hex$(SegDir.pLibs.Res02, 8)
	Print# fo, "GUID Table:				"; Hex$(SegDir.pGUID.Res01, 8); ", "; Hex$(SegDir.pGUID.Res02, 8)
	Print# fo, "Unknown 01:				"; Hex$(SegDir.Unk01.Res01, 8); ", "; Hex$(SegDir.Unk01.Res02, 8)
	Print# fo, "Name Table:				"; Hex$(SegDir.pNames.Res01, 8); ", "; Hex$(SegDir.pNames.Res02, 8)
	Print# fo, "String Table:			"; Hex$(SegDir.pStrings.Res01, 8); ", "; Hex$(SegDir.pStrings.Res02, 8)
	Print# fo, "Type Descriptors:		"; Hex$(SegDir.pTypDesc.Res01, 8); ", "; Hex$(SegDir.pTypDesc.Res02, 8)
	Print# fo, "Array Descriptors:		"; Hex$(SegDir.pArryDesc.Res01, 8); ", "; Hex$(SegDir.pArryDesc.Res02, 8)
	Print# fo, "Custom Data:			"; Hex$(SegDir.pCustData.Res01, 8); ", "; Hex$(SegDir.pCustData.Res02, 8)
	Print# fo, "Custom Data/GUID// s:		"; Hex$(SegDir.pCDGuids.Res01, 8); ", "; Hex$(SegDir.pCDGuids.Res02, 8)
	Print# fo, "Unknown 02:				"; Hex$(SegDir.Unk02.Res01, 8); ", "; Hex$(SegDir.Unk02.Res02, 8)
	Print# fo, "Unknown 03:				"; Hex$(SegDir.Unk03.Res01, 8); ", "; Hex$(SegDir.Unk03.Res02, 8)
	Print# fo,
	// End If

//EndIf


	// --------------------------------------
	// check two entries to be sure we found it
	If SegDir.pTypInfo.Res02 <> &H0F Or SegDir.pImpInfo.Res02 <> &H0F Then
		UpdateLog("Can// t find the table directory")
		Exit Function
	End If



	// **************************************
	// 	Print the data from the segments
	// **************************************

	// --------------------------------------
	// print the TypeInfo structures
	Print# fo,
	Print# fo, "*************************"
	Print# fo, "TypeInfo Data..."
	Print# fo, "*************************"
	i = 0
	p = SegDir.pTypInfo.Offs + 1
	n = p + SegDir.pTypInfo.nLen
	Do Until p => n

		Incr i
		Print# fo, "-------------------------"
		Print# fo, "TypeInfo number:	"; Format$(i)
		LSet TypInfo = Mid$(cs, p)

		Print# fo, "Type kind:	"; Hex$((TypInfo.TypeKind And 7?), 2); " = "; Parse$($TKind, (TypInfo.TypeKind And 7?) + 1)
		d = TypInfo.Align : Shift Right d, 3
		Print# fo, "	Alignment: ("; Hex$(TypInfo.Align, 2); "/8) = "; Hex$(d, 2)

		// (function records are printed near the end of the TypeInfo section)

		Print# fo, "Memory to allocate:		"; Hex$(TypInfo.Alloc, 8)
		Print# fo, "Reconstituted Size:		"; Hex$(TypInfo.Reconst, 8)

//If %Def(%Study)
		Print# fo, "Reserved 01:	"; Hex$(TypInfo.Res01, 8)
		Print# fo, "Reserved 02:	"; Hex$(TypInfo.Res02, 8)
//EndIf

		// counts of functions and properties
		Print# fo,
		Print# fo, "Function count:	"; Hex$(TypInfo.nFuncs, 4); " =="; Str$(TypInfo.nFuncs)
		Print# fo, "Property count:	"; Hex$(TypInfo.nProps, 4); " =="; Str$(TypInfo.nProps)

//If %Def(%Study)
		Print# fo, "Reserved 03:	"; Hex$(TypInfo.Res03, 8)
		Print# fo, "Reserved 04:	"; Hex$(TypInfo.Res04, 8)
		Print# fo, "Reserved 05:	"; Hex$(TypInfo.Res05, 8)
		Print# fo, "Reserved 06:	"; Hex$(TypInfo.Res06, 8)
//EndIf

		Print# fo, "GUID:	"; Hex$(TypInfo.oGUID, 8); " ==> "; tlGuid(cs, SegDir, TypInfo.oGUID)

		Print# fo, "Type Flags:	"; Hex$(TypInfo.fType, 8); " = ";
		d = TypInfo.fType // this is a bit field
		For n = 14 To 1 Step -1
			If (d And %TYPEFLAG_FREVERSEBIND) Then
				Print# fo, Parse$($TypeFlags, n);
				If (d And %TYPEFLAG_MASK) Then Print# fo, ", ";
			End If
			Shift Left d, 1
		Next n
		Print# fo,


		Print# fo, "Name:	"; Hex$(TypInfo.oName, 8); " ==> "; tlName(cs, SegDir, TypInfo.oName)
		Print# fo, "Version:	"; Hex$(TypInfo.version, 8)

		Print# fo, "Doc String:	"; Hex$(TypInfo.DocStr, 8);
		Print# fo, " ==> "; tlString(cs, SegDir, TypInfo.DocStr)

		Print# fo, "HelpStringContext:	"; Hex$(TypInfo.HelpStrCnt, 8)
		Print# fo, "HelpContext:		"; Hex$(TypInfo.HelpCntxt, 8)

		Print# fo,
		Print# fo, "Custom data offset:		"; Hex$(TypInfo.oCustData, 8)
		Print# fo, "Implemented interfaces:	"; Hex$(TypInfo.nImplTypes, 4); " =="; Str$(TypInfo.nImplTypes)
		Print# fo, "Virtual table size:		"; Hex$(TypInfo.cVft, 4)
		Print# fo, "Unknown 03:				"; Hex$(TypInfo.Unk03, 8)

		// position in type description table or in base interfaces
		// if coclass:	offset in reftable
		// if interface:	reference to inherited interface
		// if module:		offset to DLL name in name table

//If %Def(%Study)
		Select Case (TypInfo.TypeKind And &H0F)
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

		// Print# fo, "Type1:	"; Hex$(TypInfo.Type1, 8)
		Select Case TypInfo.TypeKind

			// Case %TKIND_ENUM

			// Case %TKIND_RECORD

			Case %TKIND_MODULE // offset to DLL name in string table
				Print# fo, "DLL Name:	"; Hex$(TypInfo.Type1, 8); " ==> ";
				If TypInfo.Type1 => 0 Then Print# fo, tlName(cs, SegDir, TypInfo.Type1)

			Case %TKIND_INTERFACE // reference to inherited interface?
				Print# fo, "Inherited interface?:	"; Hex$(TypInfo.Type1, 8)

			// Case %TKIND_DISPATCH

			Case %TKIND_COCLASS // offset in reftable
				Print# fo, "Reference table offset:	"; Hex$(TypInfo.Type1, 8)

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
				Print# fo, "DataType1:	"; Hex$(TypInfo.Type1, 8)

		End Select

		// if &H8000, entry above is valid, else it is zero?
		Print# fo, "DataType2:	"; Hex$(TypInfo.Type2, 8)

//If %Def(%Study)
		Print# fo, "Reserved 7:	"; Hex$(TypInfo.Res07, 8)
		Print# fo, "Reserved 8:	"; Hex$(TypInfo.Res08, 8)
//EndIf

		Print# fo,
		Print# fo, "Records offset:	"; Hex$(TypInfo.oFunRec, 8)
		If TypInfo.oFunRec < Len(cs) Then // do function/property records
			DisFunction(cs, SegDir, TypInfo.oFunRec, TypInfo.nFuncs, TypInfo.nProps)
		End If

		p = p + SizeOf(TypInfo)

	Loop While p <= Len(cs)


	// --------------------------------------
	// Print ImportInfo ?
	Print# fo,
	Print# fo, "*************************"
	Print# fo, "ImportInfo"
	Print# fo, "*************************"
	i = 0
	p = SegDir.pImpInfo.Offs + 1
	n = p + SegDir.pImpInfo.nLen
	Do Until p => n

		Incr i
		LSet ImpInfo = Mid$(cs, p)
		Print# fo, "--------------------------------------------"
		Print# fo, "Import Info number:	"; Format$(i)
		Print# fo, "Count:				"; Format$(ImpInfo.Count)
		Print# fo, "Offset in import file table:	"; Hex$(ImpInfo.oImpFile, 8)

		If (ImpInfo.Flags And 1?) Then
			Print# fo, "GUID:	"; Hex$(ImpInfo.oGuid, 8); " ==> "; tlGuid(cs, SegDir, ImpInfo.oGuid)
		Else
			Print# fo, "TypeInfo index:	"; Hex$(ImpInfo.oGuid, 8); " =="; Str$(ImpInfo.oGuid)
		End If

		Print# fo, "Type:	"; Parse$($TKind, ImpInfo.TypeKind)

		p = p + SizeOf(ImpInfo)

	Loop While p <= Len(cs)


	// --------------------------------------
	// Print Imported Type Libs
	Print# fo,
	Print# fo, "*************************"
	Print# fo, "Imported Type Libs"
	Print# fo, "*************************"
	p = SegDir.pImpFiles.Offs + 1
	n = p + SegDir.pImpFiles.nLen
	Do Until p => n
		LSet IFileInfo = Mid$(cs, p)
		Print# fo, "--------------------------------------------"
		Print# fo, "GUID:			"; Hex$(IFileInfo.oGUID, 8); " ==> "; tlGuid(cs, SegDir, IFileInfo.oGuid)
		Print# fo, "Locale ID?:		"; Hex$(IFileInfo.LCID, 8); " = "; Locale(IFileInfo.LCID)
		Print# fo, "Major version:	"; Hex$(IFileInfo.MajVer, 4)
		Print# fo, "Minor version:	"; Hex$(IFileInfo.MinVer, 4)

		d = IFileInfo.cSize
		Shift Right d, 2
		Print# fo, "Size = 		"; Hex$(IFileInfo.cSize, 4); "/4 = "; Hex$(d, 4)
		Print# fo, "File name:	"; Mid$(cs, p + SizeOf(TlbImpLib), d)

		p = p + ((SizeOf(TlbImpLib) + IFileInfo.cSize + 3) And Not 3) // advance to next one

	Loop While p <= Len(cs)


	// --------------------------------------
	// Print References Table
	Print# fo,
	Print# fo, "*************************"
	Print# fo, "References Table"
	Print# fo, "*************************"
	p = SegDir.pRefer.Offs + 1
	n = p + SegDir.pRefer.nLen
	Do Until p => n
		LSet RefRec = Mid$(cs, p)
		Print# fo, "--------------------------------------------"

		// if it// s a multiple of 4, it// s an offset in the TypeInfo table,
		// otherwise it// s an offset in the external reference table with
		// an offset of 1
		Print# fo, "Reference type:	"; Hex$(RefRec.RefType, 8);
		If (RefRec.RefType And &H03) Then
			Print# fo, "	==> (External Reference Table)"
		Else
			Print# fo, "	==> (TypeInfo)"
		End If

		Print# fo, "Flags:			"; Hex$(RefRec.Flags, 8)
		If RefRec.oCustData => 0 Then Print# fo, "Custom data:	"; Hex$(RefRec.oCustData, 8)
		If RefRec.oNext => 0 Then Print# fo, "Next offset:	"; Hex$(RefRec.oNext, 8)
		p = p + SizeOf(RefRecord)

	Loop While p <= Len(cs)


	// --------------------------------------
	// Print "Lib Table"	(unknown format)
	// always exists, always the same size (&H80)
	// ...hash table with offsets to GUID?
	Print# fo,
	Print# fo, "*************************"
	Print# fo, "Lib Table (offsets into GUID table)"
	Print# fo, "*************************"
	p = SegDir.pLibs.Offs + 1
	n = p + SegDir.pLibs.nLen
	Do Until p => n
		d = Cvl(cs, p)
		If d => 0 Then Print# fo, Hex$(d, 8)
		p = p + 4
	Loop While p <= Len(cs)


	// --------------------------------------
	// Print GUID table
	Print# fo,
	Print# fo, "*************************"
	Print# fo, "GUID Table"
	Print# fo, "*************************"
	p = SegDir.pGUID.Offs + 1
	n = p + SegDir.pGUID.nLen
	Do Until p => n

		LSet GuidEnt = Mid$(cs, p)
		Print# fo, "--------------------------------------------"
		Print# fo, "GUID:	"; GuidTxt$(GuidEnt.oGUID)
		// 	The meaning of .hRefType:
		//  = -2 for a TypeLib GUID
		// TypeInfo offset for TypeInfo GUID,
		// Otherwise, the low two bits:
		// 	= 01 for an imported TypeInfo
		// 	= 10 for an imported TypeLib (used by imported TypeInfos)
		Print# fo, "Href Type:	"; Hex$(GuidEnt.hRefType, 8);
		If GuidEnt.hRefType = -2 Then
			Print# fo, " = TypeLib GUID"
		ElseIf (GuidEnt.hRefType And 3) = 1 Then
			Print# fo, " = Imported TypeInfo"
		ElseIf (GuidEnt.hRefType And 3) = 2 Then
			Print# fo, " = Imported TypeLib"
		Else
			Print# fo, " = Offset?"
		End If

		Print# fo, "Next hash:	"; Hex$(GuidEnt.NextHash, 8)
		p = p + SizeOf(GuidEntry)

	Loop While p <= Len(cs)


	// --------------------------------------
	// Print "Unknown 01" (length is "always" &H200)
	Print# fo,
	Print# fo, "*************************"
	Print# fo, """Unknown 01"" --- Table with Offsets into the Name Table"
	Print# fo, "*************************"
	p = SegDir.Unk01.Offs + 1
	n = p + SegDir.Unk01.nLen
	Do Until p => n
		d = Cvl(cs, p)
		If d => 0 Then Print# fo, Hex$(d, 8)
		p = p + 4
	Loop While p <= Len(cs)


	// --------------------------------------
	// Print Name Table
	// (this keeps the offset in p zero-based)
	Print# fo,
	Print# fo, "*************************"
	Print# fo, "Name Table"
	Print# fo, "*************************"
	p = SegDir.pNames.Offs
	n = p + SegDir.pNames.nLen
	Do Until p => n

		LSet NameInt = Mid$(cs, p + 1)
		Print# fo, "--------------------------------------------"
		If NameInt.hRefType <> -1 Then Print# fo, "Offset:			"; Hex$(NameInt.hRefType, 8)
		Print# fo, "Next hash:		"; Hex$(NameInt.NextHash, 8)
		d = LoByt(NameInt.cName)
		Print# fo, "Name length:	"; Hex$(d, 2); " =="; Str$(d)
		Print# fo, "Flags?...		"; Hex$(HiByt(LoWrd(NameInt.cName)), 2)
		Print# fo, "Hash code:		"; Hex$(HiWrd(NameInt.cName), 4)

		p = p + SizeOf(NameInt)
		Print# fo, $Dq; Mid$(cs, p + 1, d); $Dq
		p = (p + d + 3) And &HFFFFFFFC // advance to next DWord-aligned offset

	Loop While p <= Len(cs)


	// --------------------------------------
	// Print String Table
	// every entry, length-plus-text, is a minimum of eight bytes
	// (this keeps the offset in p zero-based)
	Print# fo,
	Print# fo, "*************************"
	Print# fo, "String Table"
	Print# fo, "*************************"
	p = SegDir.pStrings.Offs
	n = p + SegDir.pStrings.nLen
	i = 1 // string number
	Do Until p => n
		d = CvWrd(cs, p + 1)
		Print# fo, Format$(i); ")	"; Mid$(cs, p + 3, d)
		// advance to next Dword-aligned offset beyond minimum length of eight bytes
		p = (p + Max&(d + 2, 8) + 3) And &HFFFFFFFC
		Incr i
	Loop While p <= Len(cs)


	// --------------------------------------
	// Print Type Descriptors
	If SegDir.pTypDesc.nLen Then
		Print# fo,
		Print# fo, "*************************"
		Print# fo, "Type Descriptors"
		Print# fo, "*************************"
		p = SegDir.pTypDesc.Offs + 1
		n = p + SegDir.pTypDesc.nLen
		Do Until p => n

			Print# fo, "-------------------------"
			Print# fo, "Raw:	"; Hex$(CvDwd(cs, p), 8), Hex$(CvDwd(cs, p + 4), 8)

			LSet TypDsc = Mid$(cs, p)

			Print# fo, "Data type 1 = "; Hex$(TypDsc.v2, 4); Hex$(TypDsc.v1, 4);
			If (TypDsc.v2 And &H7FFE) = &H7FFE Then
				Print# fo, " = ";
			ElseIf (TypDsc.v2 And %VT_Vector) Then
				Print# fo, " = VT_Vector, ";
			ElseIf (TypDsc.v2 And %VT_Array) Then
				Print# fo, " = VT_Array, ";
			ElseIf (TypDsc.v2 And %VT_ByRef) Then
				Print# fo, " = VT_ByRef, ";
			// ElseIf (TypDsc.v2 And %VT_Reserved) Then
			Else
				Print# fo, " = ";
			End If

			Print# fo, VarType(TypDsc.v1);

			If (TypDsc.v2 And &H7FFE) = &H7FFE Then
				Print# fo,
			Else
				Print# fo, " == base type: "; VarType(TypDsc.v2 And %VT_TypeMask)
			End If


			If LoByt(TypDsc.v1) = %VT_Ptr Or LoByt(TypDsc.v1) = %VT_SafeArray Then

				If TypDsc.v4 < 0 Then  // offset into type descriptor table
					Print# fo, "Type descriptor table offset:	"; Hex$(TypDsc.v3 And &H07FF8, 4)
				Else // file offset to type descriptor
					//  This doesn// t sound sensible, but it looks like what the ReactOS code was doing.
					d = MakLng(TypDsc.v2, TypDsc.v3)
					Print# fo, "Type descriptor file offset:	"; Hex$(d And (TypDsc.v3\8), 8)
				End If

			ElseIf LoByt(TypDsc.v1) = %VT_CArray Then
				Print# fo, "Array descriptor offset:	"; Hex$(MakLng(TypDsc.v3, TypDsc.v4), 8)
			ElseIf LoByt(TypDsc.v1) = %VT_UserDefined Then
				Print# fo, "Type descriptor offset:		"; Hex$(MakLng(TypDsc.v3, TypDsc.v4) And &H0FFFFFFF8&, 8)
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
		Print# fo,
		Print# fo, "*************************"
		Print# fo, "Array Descriptors"
		Print# fo, "*************************"
		p = SegDir.pArryDesc.Offs + 1
		n = p + SegDir.pArryDesc.nLen
		Do Until p => n

			LSet AryDsc = Mid$(cs, p)
			ReDim td(3)		As Integer At VarPtr(AryDsc)

			Print# fo, "--------------------------------------------"
			Print# fo, "Raw:	"; Hex$(CvDwd(cs, p), 8); ", "; Hex$(CvDwd(cs, p + 4), 8); ", "; Hex$(CvDwd(cs, p + 8), 8); ", "; Hex$(CvDwd(cs, p + 12), 8)

			If AryDsc.u.hRefType => 0 Then // a pointer to ANOTHER array descriptor?
				AryDsc.u.lpadesc = (AryDsc.u.lpadesc And &H07FFF) \ 8
				Print# fo, "Offset to array descriptor:	"; Hex$(AryDsc.u.lpadesc, 8)
			Else // the low word contains the variable-type code
				Print# fo, "hRefType:	"; Hex$(AryDsc.u.hRefType, 8)
				AryDsc.tVar = td(0) And %VT_TypeMask
			End If

			If (AryDsc.tVar And 255) = 0 Then d = HiByt(AryDsc.tVar) Else d = AryDsc.tVar
			Print# fo, "Variable type:	"; Hex$(AryDsc.tVar, 4); " = "; VarType(d)
			Print# fo, "Number of dimensions:	"; Hex$(AryDsc.nDims, 4); " =="; Str$(AryDsc.nDims)

			p = p + SizeOf(ARRAYDESC) - SizeOf(SafeArrayBound)
			For i = 1 To AryDsc.nDims
				LSet SafBnd = Mid$(cs, p)
				Print# fo, "("; Format$(i); ")	"; "Elements: "; Hex$(SafBnd.nElements, 8); " =="; Str$(SafBnd.nElements); "	Lower bound: "; Hex$(SafBnd.lLBound, 8); " =="; Str$(SafBnd.lLBound)
				p = p + SizeOf(SafeArrayBound)
				If p => Len(cs) Then Exit Do
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
		Print# fo,
		Print# fo, "*************************"
		Print# fo, "Custom Data"
		Print# fo, "*************************"
		p = SegDir.pCustData.Offs + 1
		n = p + SegDir.pCustData.nLen
		Do Until p => n
			Print# fo, Hex$(CvDwd(cs, p), 8)
			p = p + 4
		Loop While p <= Len(cs)

	End If


	// --------------------------------------
	// Print offsets of GUID// s and into the
	If SegDir.pCDGuids.nLen Then
		// custom data table
		Print# fo,
		Print# fo, "*************************"
		Print# fo, "Offsets of GUID// s"
		Print# fo, "*************************"
		p = SegDir.pCDGuids.Offs + 1
		n = p + SegDir.pCDGuids.nLen
		Do Until p => n
			Print# fo, Hex$(CvDwd(cs, p), 8)
			p = p + 4
		Loop While p <= Len(cs)
	End If

	// --------------------------------------
	// Print "Unknown 02"
	If SegDir.Unk02.nLen Then
//If %Def(%Study)
	Note "Unknown 02"
//EndIf
		Print# fo,
		Print# fo, "*************************"
		Print# fo, "Unknown 02"
		Print# fo, "*************************"
		p = SegDir.Unk02.Offs + 1
		n = p + SegDir.Unk02.nLen
		Do Until p => n
			Print# fo, Hex$(CvDwd(cs, p), 8)
			p = p + 4
		Loop While p <= Len(cs)
	End If


	// --------------------------------------
	// Print "Unknown 03"
	If SegDir.Unk03.nLen Then
//If %Def(%Study)
	Note "Unknown 03"
//EndIf
		Print# fo,
		Print# fo, "*************************"
		Print# fo, "Unknown 03"
		Print# fo, "*************************"
		p = SegDir.Unk03.Offs + 1
		n = p + SegDir.Unk03.nLen
		Do Until p => n
			Print# fo, Hex$(CvDwd(cs, p), 8)
			p = p + 4
		Loop While p <= Len(cs)
	End If

End Function
#endif
}
}
}