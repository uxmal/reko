'**************************************
'	"DisTypeLib.inc"



'This file is GPL 2008, by TheirCorp
'**************************************


$PeMZ			= "MZ"
$PePE32			= "PE" & $Nul & $Nul	 'Chr$(&H50, &H45, &H00, &H00)

%SizeOfShortName				   = 8
%IMAGE_RESOURCE_NAME_IS_STRING	   = &H080000000???
%IMAGE_RESOURCE_DATA_IS_DIRECTORY  = &H080000000???
%ResourceSection				   = 3
%NumberOfDirectoryEntries		   = 16	'IMAGE_NUMBEROF_DIRECTORY_ENTRIES
%SizeOfShortName				   = 8
%SectionHeaderSize				   = 40	 '%IMAGE_SIZEOF_SECTION_HEADER
%MAXSTRING						   = 120

%SUBDIR				   = 16
%WINAPI				   = 1
%WM_USER			   = &H400
%TRUE				   = 1
%FALSE				   = 0
%LF_FACESIZE		   = 32
%MAX_PATH			   = 260 ' max. length of full pathname
%MAX_EXT			   = 256
%ANSI_CHARSET		   = 0
%FF_DONTCARE		   = 0 ' Don't care or don't know.
%FW_DONTCARE		   = 0
%FW_NORMAL			   = 400
%LOGPIXELSY			   = 90 ' Logical pixels/inch in Y

%WM_DESTROY			   = &H2
%WM_SETFONT			   = &H30
%WM_COMMAND			   = &H111
%WM_DROPFILES		   = &H233
%WM_NCACTIVATE		   = &H86
%WM_INITDIALOG		   = &H110

%WS_CHILD			   = &H40000000
%WS_TABSTOP			   = &H00010000
%WS_MINIMIZEBOX		   = &H00020000
%WS_POPUP			   = &H80000000
%WS_VISIBLE			   = &H10000000
%WS_CLIPSIBLINGS	   = &H04000000
%WS_CAPTION			   = &H00C00000 ' WS_BORDER OR WS_DLGFRAME
%WS_BORDER			   = &H00800000
%WS_DLGFRAME		   = &H00400000
%WS_SYSMENU			   = &H00080000
%WS_VSCROLL			   = &H00200000
%WS_HSCROLL			   = &H00100000

%WS_EX_LEFT			   = &H00000000
%WS_EX_LTRREADING	   = &H00000000
%WS_EX_RIGHTSCROLLBAR  = &H00000000
%WS_EX_CONTROLPARENT   = &H00010000
%WS_EX_WINDOWEDGE	   = &H00000100
%WS_EX_CLIENTEDGE	   = &H00000200
%WS_EX_ACCEPTFILES	   = &H00000010
%WS_EX_TOOLWINDOW	   = &H00000080

%HWND_DESKTOP			   = 0

%BN_CLICKED				   = 0
%BS_TEXT				   = &H0&
%BS_PUSHBUTTON			   = &H0&
%BS_GROUPBOX			   = &H7&
%BS_CENTER				   = &H300&
%BS_TOP					   = &H400&
%BS_VCENTER				   = &HC00&
%BS_DEFPUSHBUTTON		   = &H1&
%BS_ICON				   = &H40&
%BS_BITMAP				   = &H80&

%DM_SETDEFID			   = %WM_USER + 1

%DS_3DLOOK				   = &H0004&
%DS_MODALFRAME			   = &H0080& ' Can be combined with WS_CAPTION
%DS_NOFAILCREATE		   = &H0010&
%DS_SETFONT				   = &H0040& ' User specified font for Dlg controls
%DS_SETFOREGROUND		   = &H0200& ' not in win3.1

%ES_LEFT		 = &H0&
%ES_MULTILINE	 = &H4&
%ES_AUTOVSCROLL  = &H40&
%ES_AUTOHSCROLL  = &H80&
%ES_WANTRETURN   = &H1000&

%EM_SCROLL		 = &HB5
%SB_LINEDOWN	 = 1
%EM_SCROLLCARET  = &HB7
%EM_SETSEL		 = &HB1

%SBS_HORZ					  = &H0&
%SBS_VERT					  = &H1&
%SBS_TOPALIGN				  = &H2&
%SBS_LEFTALIGN				  = &H2&
%SBS_BOTTOMALIGN			  = &H4&
%SBS_RIGHTALIGN				  = &H4&
%SBS_SIZEBOXTOPLEFTALIGN	  = &H2&
%SBS_SIZEBOXBOTTOMRIGHTALIGN  = &H4&
%SBS_SIZEBOX				  = &H8&
%SBS_SIZEGRIP				  = &H10&

%SS_CENTER					  = &H00000001
%SS_RIGHT					  = &H00000002

%MB_OK						  = &H00000000&
%MB_OKCANCEL				  = &H00000001&
%MB_ABORTRETRYIGNORE		  = &H00000002&
%MB_YESNOCANCEL				  = &H00000003&
%MB_YESNO					  = &H00000004&
%MB_RETRYCANCEL				  = &H00000005&
%MB_CANCELTRYCONTINUE		  = &H00000006&

%IDOK						  = 1
%IDCANCEL					  = 2
%IDABORT					  = 3
%IDRETRY					  = 4
%IDIGNORE					  = 5
%IDYES						  = 6
%IDNO						  = 7
%IDCLOSE					  = 8
%IDHELP						  = 9
%IDTRYAGAIN					  = 10
%IDCONTINUE					  = 11
%SW_SHOW					  = 5

'-----------------------------------------------------------------
%LOAD_LIBRARY_AS_DATAFILE  = &H00000002
%RT_CURSOR				   = 1
%RT_BITMAP				   = 2
%RT_ICON				   = 3
%RT_MENU				   = 4
%RT_DIALOG				   = 5
%RT_STRING				   = 6
%RT_FONTDIR				   = 7
%RT_FONT				   = 8
%RT_ACCELERATOR			   = 9
%RT_RCDATA				   = 10
%RT_MESSAGETABLE		   = 11
%RT_GROUP_CURSOR		   = 12
%RT_GROUP_ICON			   = 14
%RT_VERSION				   = 16
%RT_DLGINCLUDE			   = 17
%RT_PLUGPLAY			   = 19
%RT_VXD					   = 20
%RT_ANICURSOR			   = 21
%RT_ANIICON				   = 22
%RT_HTML				   = 23
%RT_MANIFEST			   = 24

%READ_CONTROL				 = &H00020000
%SYNCHRONIZE				 = &H00100000
%STANDARD_RIGHTS_READ		 = %READ_CONTROL
%KEY_QUERY_VALUE			 = &H1
%KEY_ENUMERATE_SUB_KEYS		 = &H8
%KEY_NOTIFY					 = &H10
%KEY_READ					 = %STANDARD_RIGHTS_READ Or %KEY_QUERY_VALUE Or %KEY_ENUMERATE_SUB_KEYS Or %KEY_NOTIFY And (Not %SYNCHRONIZE)
%HKEY_CURRENT_USER			 = &H80000001
%ERROR_SUCCESS				 = 0&


'**************************************

Type LOGFONT
	lfHeight As Long
	lfWidth As Long
	lfEscapement As Long
	lfOrientation As Long
	lfWeight As Long
	lfItalic As Byte
	lfUnderline As Byte
	lfStrikeOut As Byte
	lfCharSet As Byte
	lfOutPrecision As Byte
	lfClipPrecision As Byte
	lfQuality As Byte
	lfPitchAndFamily As Byte
	lfFaceName As Asciiz * %LF_FACESIZE
End Type



'**************************************
'	PE format UDTs
'**************************************

' For resource directory entries that have actual string names, the Name
' field of the directory entry points to an object of the following type.
' All of these string objects are stored together after the last resource
' directory entry and before the first resource data object.  This minimizes
' the impact of these variable length objects on the alignment of the fixed
' size directory entry objects.

Type IMAGE_RESOURCE_DIRECTORY_STRING
	Length		As Word
	NameString  As Byte 'ASCII string
End Type

Type IMAGE_RESOURCE_DIRECTORY_ENTRY
	NameID  As Dword	'if bit 31 is set, the name is a string
	Offset  As Dword	'if bit 31 is set, the data is a directory
End Type

Type IMAGE_RESOURCE_DIRECTORY
	Characteristics		As Dword
	TimeDateStamp		As Dword
	MajorVersion		As Word
	MinorVersion		As Word
	NumberOfNamedEntries As Word
	NumberOfIdEntries   As Word
	'DirectoryEntries(0) As IMAGE_RESOURCE_DIRECTORY_ENTRY
End Type

Type IMAGE_RESOURCE_DATA_ENTRY
	OffsetToData	As Dword
	Size			As Dword
	CodePage		As Dword
	Reserved		As Dword
End Type


Type SectionInfo
	SectName			As String * %SizeOfShortName
	VirtSize			As Dword
	dRVA				As Dword	'RVA to specific data within section
	dSize				As Dword	'size of specific data within section
	RVA					As Dword
	RamAdd				As Dword
	SizeOfRawData		As Dword
	PtrToRawData		As Dword
	StrPos				As Dword
	EndPos				As Dword
	Delta				As Dword
	Characteristics		As Dword
End Type 'SectionHeader


Type DosHeader Byte			'DOS stub in EXE file
	Magic		As Word		'Magic number
	cBlp		As Word		'Bytes on last page of file
	cP			As Word		'Pages in file
	cRlc		As Word		'Relocations
	cParHdr		As Word		'Size of header in paragraphs
	MinAlloc	As Word		'Minimum extra paragraphs needed
	MaxAlloc	As Word		'Maximum extra paragraphs needed
	ss			As Word		'Initial (relative) SS value
	sp			As Word		'Initial SP value
	csum		As Word		'Checksum
	ip			As Word		'Initial IP value
	cs			As Word		'Initial (relative) CS value
	lfaRlc		As Word		'File address of relocation table
	OvNo		As Word		'Overlay number
	Res4		As Asciz * 8 '4 Reserved words
	OemId		As Word		'OEM identifier (for oeminfo)
	OemInfo		As Word		'OEM information oemid specific
	Res10		As Asciz * 20 '10 Reserved words
	lfaNew		As Long  'always at &H3C and contains the offset of PE signature

End Type 'DosHeader


Type PEHeader Byte  'officially "IMAGEFILEHEADER"
	Machine					As Word  'machine type
	NumberOfSections		As Word
	TimeDateStamp			As Dword 'Number of seconds since December 31st, 1969, at 4:00 P.M
	PointerToSymbolTable	As Dword
	NumberOfSymbols			As Dword
	SizeOfOptionalHeader	As Word
	Characteristics			As Word
End Type 'PEHeader


Type DataDir			Byte	'IMAGE_DATA_DIRECTORY
	RVA					As Dword
	DirSize				As Dword
End Type 'DataDir


'Optional header format ("IMAGE_OPTIONAL_HEADER32").
'this is not as "optional" as its name suggests
Type OptHeader			Byte
	'Standard fields.
	Magic				As Word  '"Magic number": &H10B = PE32, &H20B = PE32+, &H107 = ROM image
	MajLinkerVer		As Byte
	MinLinkerVer		As Byte
	SizeOfCode			As Dword
	SizeOfInitData		As Dword
	SizeOfUninitData	As Dword
	AddrOfEntryPoint	As Dword
	BaseOfCode			As Dword
	BaseOfData			As Dword

	'NT additional fields.
	ImageBase			As Dword 'defaults: DLL = &H10000000, EXE = &H400000
	SectionAlign		As Dword 'must be => file alignment
	FileAlign			As Dword 'alignment of raw data of sections. value should be a power of 2, => 512 and <= 64K ( default = 512)
	MajOSVer			As Word
	MinOSVer			As Word
	MajImageVer			As Word
	MinImageVer			As Word
	MajSubsysVer		As Word
	MinSubsysVer		As Word
	Win32VerValue		As Dword
	SizeOfImage			As Dword 'size of image, including headers. must be a multiple of SectionAlign
	SizeOfHeaders		As Dword 'size of: stub + PE Header + section headers rounded up to multiple of FileAlign
	CheckSum			As Dword
	Subsystem			As Word
	DllCharacteristics  As Word
	SizeOfStackReserve  As Dword
	SizeOfStackCommit   As Dword
	SizeOfHeapReserve   As Dword
	SizeOfHeapCommit	As Dword
	LoaderFlags			As Dword 'obsolete
	NumberOfRvaAndSizes As Dword 'Number of data-directory entries in the remainder of Optional Header
	'DataDirectory( %NumberOfDirectoryEntries ) As DataDir

End Type 'OptionalHeader


'Section header format.
'Borland calls its code sections "CODE", rather than ".text".
Type SectionHeader		Byte	 ' _IMAGE_SECTION_HEADER
	SectName			As String * %SizeOfShortName
	VirtSize			As Dword
	'VirtualAddress		 As Dword	 'changed from "RVA"
	RVA					As Dword
	SizeOfRawData		As Dword
	PtrToRawData		As Dword
	PtrToRelocations	As Dword
	PtrToLineNums		As Dword
	NumberOfRelocations As Word
	NumberOfLineNums	As Word
	Characteristics		As Dword
End Type 'SectionHeader


'**************************************
'	Declares
'**************************************
Declare Function CreateFontIndirect Lib "GDI32.DLL" Alias "CreateFontIndirectA" (lpLogFont As LOGFONT) As Dword
Declare Function DeleteObject Lib "GDI32.DLL" Alias "DeleteObject" (ByVal hObject As Dword) As Long
Declare Function GetDeviceCaps Lib "GDI32.DLL" Alias "GetDeviceCaps" (ByVal hdc As Dword, ByVal nIndex As Long) As Long

Declare Function GetLastError Lib "KERNEL32.DLL" Alias "GetLastError" () As Long
Declare Function GetModuleFileName Lib "KERNEL32.DLL" Alias "GetModuleFileNameA" (ByVal hModule As Dword, lpFileName As Asciiz, ByVal nSize As Dword) As Dword

Declare Sub DragAcceptFiles Lib "SHELL32.DLL" Alias "DragAcceptFiles" (ByVal hwnd As Dword, ByVal fAccept As Long)
Declare Sub DragFinish Lib "SHELL32.DLL" Alias "DragFinish" (ByVal hDrop As Dword)
Declare Function DragQueryFile Lib "SHELL32.DLL" Alias "DragQueryFileA" (ByVal hDrop As Dword, ByVal uiFile As Dword, lpStr As Asciiz, ByVal cch As Dword) As Dword
Declare Function SetMenu Lib "USER32.DLL" Alias "SetMenu" (ByVal hWnd As Dword, ByVal hMenu As Dword) As Long

Declare Function GetDC Lib "USER32.DLL" Alias "GetDC" (ByVal hWnd As Dword) As Dword
Declare Function GetFocus Lib "USER32.DLL" Alias "GetFocus" () As Dword
Declare Function ReleaseDC Lib "USER32.DLL" Alias "ReleaseDC" (ByVal hWnd As Dword, ByVal hDC As Dword) As Long
Declare Function SetFocus Lib "USER32.DLL" Alias "SetFocus" (ByVal hWnd As Dword) As Long

'**************************************

