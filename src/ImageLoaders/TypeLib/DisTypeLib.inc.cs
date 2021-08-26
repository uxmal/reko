//**************************************
//	"DisTypeLib.inc"


//This file is GPL 2008, by TheirCorp
//**************************************

using System.Runtime.InteropServices;
namespace Reko.ImageLoaders.TypeLib 
{
    using static DisTypeLib;

public class DisTypeLib
{

public const string PeMZ			= "MZ";
public const string PePE32			= "PE\0\0";	 //Chr$(0x50, 0x45, 0x00, 0x00)

public const short SizeOfShortName				   = 8;
public const uint IMAGE_RESOURCE_NAME_IS_STRING	   = 0x080000000;
public const uint IMAGE_RESOURCE_DATA_IS_DIRECTORY     =0x080000000;
public const short ResourceSection				   = 3;
public const short NumberOfDirectoryEntries		   = 16;	// IMAGE_NUMBEROF_DIRECTORY_ENTRIES
public const short SectionHeaderSize				   = 40;	 // %IMAGE_SIZEOF_SECTION_HEADER
public const short MAXSTRING						   = 120;

public const short SUBDIR				   = 16;
public const short WINAPI				   = 1;
public const short WM_USER			   = 0x400;
public const short TRUE				   = 1;
public const short FALSE				   = 0;
public const short LF_FACESIZE		   = 32;
public const short MAX_PATH			   = 260; // max. length of full pathname
public const short MAX_EXT			   = 256;
public const short ANSI_CHARSET		   = 0;
public const short FF_DONTCARE		   = 0; // Don//t care or don't know.
public const short FW_DONTCARE		   = 0;
public const short FW_NORMAL			   = 400;
public const short LOGPIXELSY			   = 90; // Logical pixels/inch in Y

public const short WM_DESTROY			   = 0x2       ;
public const short WM_SETFONT			   = 0x30      ;
public const short WM_COMMAND			   = 0x111     ;
public const short WM_DROPFILES		   = 0x233         ;
public const short WM_NCACTIVATE		   = 0x86      ;
public const short WM_INITDIALOG		   = 0x110     ;
                                                       
public const uint WS_CHILD			   = 0x40000000    ;
public const uint WS_TABSTOP			   = 0x00010000;
public const uint WS_MINIMIZEBOX		   = 0x00020000;
public const uint WS_POPUP			   = 0x80000000    ;
public const uint WS_VISIBLE			   = 0x10000000;
public const uint WS_CLIPSIBLINGS	   = 0x04000000    ;
public const uint WS_CAPTION			   = 0x00C00000; // WS_BORDER OR WS_DLGFRAME
public const uint WS_BORDER			   = 0x00800000;    
public const uint WS_DLGFRAME		   = 0x00400000;    
public const uint WS_SYSMENU			   = 0x00080000;
public const uint WS_VSCROLL			   = 0x00200000;
public const uint WS_HSCROLL			   = 0x00100000;
                                                        
public const uint WS_EX_LEFT			   = 0x00000000;
public const uint WS_EX_LTRREADING	   = 0x00000000;
public const uint WS_EX_RIGHTSCROLLBAR  = 0x00000000;
public const uint WS_EX_CONTROLPARENT   = 0x00010000;
public const uint WS_EX_WINDOWEDGE	   = 0x00000100;
public const uint WS_EX_CLIENTEDGE	   = 0x00000200;
public const uint WS_EX_ACCEPTFILES	   = 0x00000010;
public const uint WS_EX_TOOLWINDOW	   = 0x00000080;

public const short HWND_DESKTOP			   = 0;

public const short BN_CLICKED				   = 0;
public const short BS_TEXT				   = 0x0;
public const short BS_PUSHBUTTON			   = 0x0;
public const short BS_GROUPBOX			   = 0x7;
public const short BS_CENTER				   = 0x300;
public const short BS_TOP					   = 0x400;
public const short BS_VCENTER				   = 0xC00;
public const short BS_DEFPUSHBUTTON		   = 0x1;
public const short BS_ICON				   = 0x40;
public const short BS_BITMAP				   = 0x80;

public const short DM_SETDEFID			   = WM_USER + 1;

public const short DS_3DLOOK				   = 0x0004;
public const short DS_MODALFRAME			   = 0x0080; // Can be combined with WS_CAPTION
public const short DS_NOFAILCREATE		   = 0x0010;
public const short DS_SETFONT				   = 0x0040; // User specified font for Dlg controls
public const short DS_SETFOREGROUND		   = 0x0200; // not in win3.1

public const short ES_LEFT		 = 0x0;
public const short ES_MULTILINE	 = 0x4;
public const short ES_AUTOVSCROLL  = 0x40;
public const short ES_AUTOHSCROLL  = 0x80;
public const short ES_WANTRETURN   = 0x1000;

public const short EM_SCROLL		 = 0xB5;
public const short SB_LINEDOWN	 = 1;
public const short EM_SCROLLCARET  = 0xB7;
public const short EM_SETSEL		 = 0xB1;

public const short SBS_HORZ					  = 0x0;
public const short SBS_VERT					  = 0x1;
public const short SBS_TOPALIGN				  = 0x2;
public const short SBS_LEFTALIGN				  = 0x2;
public const short SBS_BOTTOMALIGN			  = 0x4;
public const short SBS_RIGHTALIGN				  = 0x4;
public const short SBS_SIZEBOXTOPLEFTALIGN	  = 0x2;
public const short SBS_SIZEBOXBOTTOMRIGHTALIGN  = 0x4;
public const short SBS_SIZEBOX				  = 0x8;
public const short SBS_SIZEGRIP				  = 0x10;

public const short SS_CENTER					  = 0x00000001;
public const short SS_RIGHT					  = 0x00000002;

public const short MB_OK						  = 0x00000000;
public const short MB_OKCANCEL				  = 0x00000001;
public const short MB_ABORTRETRYIGNORE		  = 0x00000002;
public const short MB_YESNOCANCEL				  = 0x00000003;
public const short MB_YESNO					  = 0x00000004;
public const short MB_RETRYCANCEL				  = 0x00000005;
public const short MB_CANCELTRYCONTINUE		  = 0x00000006;

public const short IDOK						  = 1;
public const short IDCANCEL					  = 2;
public const short IDABORT					  = 3;
public const short IDRETRY					  = 4;
public const short IDIGNORE					  = 5;
public const short IDYES						  = 6;
public const short IDNO						  = 7;
public const short IDCLOSE					  = 8;
public const short IDHELP						  = 9;
public const short IDTRYAGAIN					  = 10;
public const short IDCONTINUE					  = 11;
public const short SW_SHOW					  = 5;

//-----------------------------------------------------------------
public const short LOAD_LIBRARY_AS_DATAFILE  = 0x00000002;
public const short RT_CURSOR				   = 1;
public const short RT_BITMAP				   = 2;
public const short RT_ICON				   = 3;
public const short RT_MENU				   = 4;
public const short RT_DIALOG				   = 5;
public const short RT_STRING				   = 6;
public const short RT_FONTDIR				   = 7;
public const short RT_FONT				   = 8;
public const short RT_ACCELERATOR			   = 9;
public const short RT_RCDATA				   = 10;
public const short RT_MESSAGETABLE		   = 11;
public const short RT_GROUP_CURSOR		   = 12;
public const short RT_GROUP_ICON			   = 14;
public const short RT_VERSION				   = 16;
public const short RT_DLGINCLUDE			   = 17;
public const short RT_PLUGPLAY			   = 19;
public const short RT_VXD					   = 20;
public const short RT_ANICURSOR			   = 21;
public const short RT_ANIICON				   = 22;
public const short RT_HTML				   = 23;
public const short RT_MANIFEST			   = 24;

public const uint READ_CONTROL				 = 0x00020000;
public const uint SYNCHRONIZE				 = 0x00100000;
public const uint STANDARD_RIGHTS_READ		 = READ_CONTROL;
public const uint KEY_QUERY_VALUE			 = 0x1;
public const uint KEY_ENUMERATE_SUB_KEYS		 = 0x8;
public const uint KEY_NOTIFY					 = 0x10;
public const uint KEY_READ					 = STANDARD_RIGHTS_READ | KEY_QUERY_VALUE | KEY_ENUMERATE_SUB_KEYS | KEY_NOTIFY & (~SYNCHRONIZE);
public const uint HKEY_CURRENT_USER			 = 0x80000001;
public const short ERROR_SUCCESS				 = 0;
}

[StructLayout(LayoutKind.Sequential, Pack =1)]
public unsafe struct LOGFONT
{
    public const int LF_FACESIZE = 32;

	public int lfHeight;
	public int lfWidth;
	public int lfEscapement;
	public int lfOrientation;
	public int lfWeight;
	public byte lfItalic;
	public byte lfUnderline;
	public byte lfStrikeOut;
	public byte lfCharSet;
	public byte lfOutPrecision;
	public byte lfClipPrecision;
	public byte lfQuality;
	public byte lfPitchAndFamily;
	public fixed byte lfFaceName[LF_FACESIZE];
}



//**************************************
//	PE format UDTs
//**************************************

// For resource directory entries that have actual string names, the Name
// field of the directory entry points to an object of the following type.
// All of these string objects are stored together after the last resource
// directory entry and before the first resource data object.  This minimizes
// the impact of these variable length objects on the alignment of the fixed
// size directory entry objects.

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public unsafe class IMAGE_RESOURCE_DIRECTORY_STRING
{
	public ushort Length;
	public byte NameString; //ASCII string
}

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct IMAGE_RESOURCE_DIRECTORY_ENTRY
{	public uint NameID;	//if bit 31 is set, the name is a string
	public uint Offset;	//if bit 31 is set, the data is a directory
}

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct IMAGE_RESOURCE_DIRECTORY
{
	public uint Characteristics;
	public uint TimeDateStamp;
	public ushort MajorVersion;
	public ushort MinorVersion;
	public ushort NumberOfNamedEntries;
	public ushort NumberOfIdEntries;
	//DirectoryEntries(0) As IMAGE_RESOURCE_DIRECTORY_ENTRY
}

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct IMAGE_RESOURCE_DATA_ENTRY
{
	public uint OffsetToData;
	public uint Size;
	public uint CodePage;
	public uint Reserved;
}


[StructLayout(LayoutKind.Sequential, Pack = 2)]
public unsafe struct SectionInfo
{
	public fixed byte SectName[SizeOfShortName];
	public uint VirtSize;
	public uint dRVA;	//RVA to specific data within section
	public uint dSize;	//size of specific data within section
	public uint RVA;
	public uint RamAdd;
	public uint SizeOfRawData;
	public uint PtrToRawData;
	public uint StrPos;
	public uint EndPos;
	public uint Delta;
	public uint Characteristics;
} //SectionHeader



[StructLayout(LayoutKind.Sequential, Pack=2)]
public unsafe struct DosHeader 			//DOS stub in EXE file
{
    public ushort Magic;		//Magic number
    public ushort cBlp;		//Bytes on last page of file
    public ushort cP;		//Pages in file
    public ushort cRlc;		//Relocations
    public ushort cParHdr;		//Size of header in paragraphs
    public ushort MinAlloc;		//Minimum extra paragraphs needed
    public ushort MaxAlloc;		//Maximum extra paragraphs needed
    public ushort ss;		//Initial (relative) SS value
    public ushort sp;		//Initial SP value
    public ushort csum;		//Checksum
    public ushort ip;		//Initial IP value
    public ushort cs;		//Initial (relative) CS value
    public ushort lfaRlc;		//File address of relocation table
    public ushort OvNo;		//Overlay number
    public fixed byte Res4[8]; //4 Reserved words
    public ushort OemId;        //OEM identifier (for oeminfo)
    public ushort OemInfo;      //OEM information oemid specific
    public fixed byte Res10[20];    //10 Reserved words
    public uint lfaNew;             //always at 0x3C and contains the offset of PE signature

} //DosHeader

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public unsafe struct PEHeader  //officially "IMAGEFILEHEADER"
{
    public ushort Machine;                    //machine type
    public ushort NumberOfSections;
    public uint TimeDateStamp;          //Number of seconds since December 31st, 1969, at 4:00 P.M
    public uint PointerToSymbolTable;	
	public uint NumberOfSymbols;
	public ushort SizeOfOptionalHeader;
	public ushort Characteristics;
} //PEHeader


[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct DataDir				//IMAGE_DATA_DIRECTORY
{	public uint RVA;
	public uint DirSize;
} //DataDir


//Optional header format ("IMAGE_OPTIONAL_HEADER32").
//this is not as "optional" as its name suggests
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct OptHeader

{	//Standard fields.
	public ushort Magic;  //"Magic number": 0x10B = PE32, 0x20B = PE32+, 0x107 = ROM image
	public byte MajLinkerVer;
	public byte MinLinkerVer;
	public uint SizeOfCode;
	public uint SizeOfInitData;
	public uint SizeOfUninitData;
	public uint AddrOfEntryPoint;
	public uint BaseOfCode;
	public uint BaseOfData;

	//NT additional fields.
	public uint ImageBase; //defaults: DLL = 0x10000000, EXE = 0x400000
	public uint SectionAlign; //must be => file alignment
	public uint FileAlign; //alignment of raw data of sections. value should be a power of 2, => 512 and <= 64K ( default = 512)
	public ushort MajOSVer;
	public ushort MinOSVer;
	public ushort MajImageVer;
	public ushort MinImageVer;
	public ushort MajSubsysVer;
	public ushort MinSubsysVer;
	public uint Wir32VerValue;
	public uint SizeOfImage; //size of image, including headers. must be a multiple of SectionAlign
	public uint SizeOfHeaders; //size of: stub + PE Header + section headers rounded up to multiple of FileAlign
	public uint CheckSum;
	public ushort Subsystem;
	public ushort DllCharacteristics;
	public uint SizeOfStackReserve;
	public uint SizeOfStackCommit;
	public uint SizeOfHeapReserve;
	public uint SizeOfHeapCommit;
	public uint LoaderFlags; //obsolete
	public uint NumberOfRvaAndSizes; //Number of data-directory entries in the remainder of Optional Header
    public fixed uint DataDirectory[NumberOfDirectoryEntries * 2];

} //OptionalHeader


//Section header format.
//Borland calls its code sections "CODE", rather than ".text".
public unsafe struct SectionHeader			 // _IMAGE_SECTION_HEADER

{
    public fixed byte SectName[SizeOfShortName];
	public uint VirtSize;
	//VirtualAddress		 As uint	 //changed from "RVA"
	public uint RVA;
	public uint SizeOfRawData;
	public uint PtrToRawData;
	public uint PtrToRelocations;
	public uint PtrToLineNums;
	public ushort NumberOfRelocations;
	public ushort NumberOfLineNums;
	public uint Characteristics;
} //SectionHeader


//**************************************
//	Declares
//**************************************
//Declare Function CreateFontIndirect Lib "GDI32.DLL" Alias "CreateFontIndirectA" (lpLogFont As LOGFONT) As Dword
//Declare Function DeleteObject Lib "GDI32.DLL" Alias "DeleteObject" (ByVal hObject As Dword) As Long
//Declare Function GetDeviceCaps Lib "GDI32.DLL" Alias "GetDeviceCaps" (ByVal hdc As Dword, ByVal nIndex As Long) As Long

//Declare Function GetLastError Lib "KERNEL32.DLL" Alias "GetLastError" () As Long
//Declare Function GetModuleFileName Lib "KERNEL32.DLL" Alias "GetModuleFileNameA" (ByVal hModule As Dword, lpFileName As Asciiz, ByVal nSize As Dword) As Dword

//Declare Sub DragAcceptFiles Lib "SHELL32.DLL" Alias "DragAcceptFiles" (ByVal hwnd As Dword, ByVal fAccept As Long)
//Declare Sub DragFinish Lib "SHELL32.DLL" Alias "DragFinish" (ByVal hDrop As Dword)
//Declare Function DragQueryFile Lib "SHELL32.DLL" Alias "DragQueryFileA" (ByVal hDrop As Dword, ByVal uiFile As Dword, lpStr As Asciiz, ByVal cch As Dword) As Dword
//Declare Function SetMenu Lib "USER32.DLL" Alias "SetMenu" (ByVal hWnd As Dword, ByVal hMenu As Dword) As Long

//Declare Function GetDC Lib "USER32.DLL" Alias "GetDC" (ByVal hWnd As Dword) As Dword
//Declare Function GetFocus Lib "USER32.DLL" Alias "GetFocus" () As Dword
//Declare Function ReleaseDC Lib "USER32.DLL" Alias "ReleaseDC" (ByVal hWnd As Dword, ByVal hDC As Dword) As Long
//Declare Function SetFocus Lib "USER32.DLL" Alias "SetFocus" (ByVal hWnd As Dword) As Long

}

