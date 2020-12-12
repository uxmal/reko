// **************************************
//    "GetTypeLibData.bas"

#if VISUALBASIC
namespace Reko.ImageLoaders.TypeLib
{

//This file is GPL 2008, by TheirCorp
// *************************************

using System.IO;
using Reko.Core;

public class GetTypeLibDatax
{

    // **************************************
    // this is a re-entrant procedure
    // ird must always arrive pointing to a resource directory

// For now, this assumes all offsets are relative to the start
// of the resource section (opinions seem to differ on this)
#if NO
    int GetResource( 
	string cs, 
	IMAGE_RESOURCE_DIRECTORY ird, 
	uint de,     // value to subtract from ".link" offsets// s
	uint ss ,   // section size
	uint offset   // offset to resource section
	)
{

int n;
uint p;
IMAGE_RESOURCE_DATA_ENTRY Ptr pirde;
IMAGE_RESOURCE_DIRECTORY_ENTRY Ptr irde;
IMAGE_RESOURCE_DIRECTORY_STRING ids;

Static fResult	As Long  ;
Static lvl		As Long	 ;// recursion level
Static rp		As Dword ;// resource pointer
Static rid		As Dword ;
Static typ		As Dword ;
Static ns		As String;

	if (lvl = 0) { rp = StrPtr(cs); } // = ird // save pointer to start of resource section
        ++lvl;

	irde = ird + SizeOf(IMAGE_RESOURCE_DIRECTORY);

	//  The named entries are case insensitive strings, sorted in ascending order.
	//  The entries with 16-bit IDs follow these and are also sorted in ascending order.
	For n = 1 To @ird.NumberOfNamedEntries + @ird.NumberOfIdEntries

		if ((n <= @ird.NumberOfNamedEntries) And (@irde.NameID And %IMAGE_RESOURCE_NAME_IS_STRING)) {
			// the name is a string
			p = rp + (@irde.NameID And &H7FFFFFFF)  // offset is relative to start of resource section

			rid = 0 // assign invalid value as a flag
			ns = Remove$(Peek$(p + 2, CvWrd(Peek$(p, 2)) * 2), $Nul) // unicode string
			if (UCase$(ns) = "TYPELIB") { fResult = 1 // found TypeLib data

		} else if  ((n > @ird.NumberOfNamedEntries) And (@irde.NameID And %IMAGE_RESOURCE_NAME_IS_STRING) = 0) {

			// it// s a 16-bit ID number
			p = LoWrd(@irde.NameID)
			if (lvl = 1) {
				typ = p
			} else if  (lvl = 2) {
				ns = ""
				rid = p
			} else if  (lvl = 3) {
			}

		}

		p = (@irde.Offset And &H7FFFFFFF)

		if ((@irde.Offset And %IMAGE_RESOURCE_DATA_IS_DIRECTORY)) { // it// s a subdirectory
			p = GetResource(cs, rp + p, de, ss, Offset)

		} else {    // it// s actual resource data
			// get offset in PE file to ImageResourceDirectoryEntry structure
			pirde = rp + p

			if (fResult = 1) {
				Incr fResult
				// @pirde.OffsetToData is the RVA to the resource data
				cs = Mid$(cs, @pirde.OffsetToData - de - Offset + 1, @pirde.Size)
			}

		}

		if (fResult == 2)
            break;
		irde = irde + SizeOf(IMAGE_RESOURCE_DIRECTORY_ENTRY);

	Next n

	-- lvl;
	if (lvl) {
		Function = ird;
	} else {
		Function = fResult;
		fResult = 0;
	}

        } // GetResource

#endif
// ***************************************
//  gets TypeLib data from a PE file// s resource section
int GetTypeLibData(string cs, string fs)
{
    //#Register All
    FileStream  ff;// file handle
    Long n;
    Long fTlb;
    DosHeader DosHdr;
    PEHeader  pPH;
    SectionInfo si;
    OptHeader pOH;
    SectionHeader pSH;
    DataDir  pDD;


	// --------------------------------------
	// get the Typelib data
	try
    {
        ff = new FileStream(fs, FileMode.Open, FileAccess.Read, FileShare.Read);
        
        byte[] cs = new byte[2048];
        ff.Read(cs, 0, cs.Length);

		// ----------------------------------
		// get PE signature if present
		DosHdr = cs;
            lfanew = ProgramImage.ReadUInt32(cs, 0x3C);
		if (cs[0] == 'M' && cs[1] == 'Z' && cs[lfanew] == 'P' && cs[lfanew+1] == 'E' && cs[lfanew+2] == 0 && cs[lfanew+3] == 0) {

			--fTlb // disable loading the file below
			pPH = StrPtr(cs) + DosHdr.lfanew + 4;
			pOH = pPH + SizeOf(PEHeader);

			// "pOH.NumberOfRvaAndSizes" is the number of entries, not the size of the array, as someone once wrote
			if (ResourceSection > @pOH.NumberOfRvaAndSizes) return;

			pDD = pOH + SizeOf(OptHeader) + ((ResourceSection - 1) * SizeOf(DataDir));
			si.dRVA = @pDD.RVA      ;
			si.dSize = @pDD.DirSize ;


			// find the section which matches si.dRVA in the section table
			pSH = pOH + SizeOf(OptHeader) + (@pOH.NumberOfRvaAndSizes * SizeOf(DataDir));
			For (int n = 1; n <= @pPH.NumberOfSections; ++n) {
				if ((si.dRVA => @pSH.RVA) && (si.dRVA < @pSH.RVA + @pSH.SizeOfRawData)) {
					si.SectName         = @pSH.SectName;
					si.VirtSize         = @pSH.VirtSize;             // size of unpadded section
					si.RVA              = @pSH.RVA;                  // @pSH.RVA is the offset to section when loaded
					si.RamAdd           = @pOH.ImageBase + @pSH.RVA; // section// s RAM address (for example: &H401000)
					si.SizeOfRawData    = @pSH.SizeOfRawData;        // size after padding to section alignment
					si.PtrToRawData     = @pSH.PtrToRawData;         // zero-based file offset to section
					si.StrPos           = @pSH.PtrToRawData + 1;     // one-based file offset to section
					si.EndPos           = si.StrPos + si.SizeOfRawData;
					si.Delta            = si.RVA - si.PtrToRawData;  // value to subtract from RVAs to get file offsets
					si.Characteristics  = @pSH.Characteristics;

					break;

				}

				pSH = pSH + SizeOf(SectionHeader); // advance pSH to next section header

			}

			// get TypeLib resource
			ff.Position = si.StrPos;
			cs = new byte[si.SizeOfRawData];
            ff.Read(cs, 0, cs.Length);
			if (GetResource(cs, StrPtr(cs), si.Delta, si.SizeOfRawData, si.PtrToRawData) = 0) {
				Reset cs
				UpdateLog( "No TypeLib data found in: " + fs);
			}

		}


		if (Left$(cs, 4) == "MSFT") { // it's a "tlb" (TypeLib) file
			++fTlb
			Function = 1
		} else if  (Left$(cs, 4) = "SLTG") {
			Incr fTlb
			Function = 2
		}

		if (fTlb > 0) {
			Seek# ff, 1
			Get$ #ff, Lof(ff), cs
		}

		ff.Close ();
    } catch
    {
		UpdateLog ("Error opening input file: " + fs);
		return;
	}
    
} // GetTypeLibData
#endif
// ***************************************
