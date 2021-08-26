using System.IO;
using System.Text;
using Reko.Core;
using Reko.Core.Memory;
using Reko.Core.Services;

namespace Reko.ImageLoaders.TypeLib
{

    //This file is GPL 2008, by TheirCorp
    // *************************************

#if NOT_YET
    using static DisTypeLib;

    public class GetTypeLibDatax
{
        private DecompilerEventListener listener;

        public GetTypeLibDatax(DecompilerEventListener listener)
        {
            this.listener = listener;
        }

        uint fResult;
        int lvl;// recursion level
        int rp  ;// resource pointer
        int rid ;
        uint typ ;
        string ns = "";

        // **************************************
        // this is a re-entrant procedure
        // ird must always arrive pointing to a resource directory

        // For now, this assumes all offsets are relative to the start
        // of the resource section (opinions seem to differ on this)
unsafe int GetResource( 
	byte * pcs, 
	int ird, 
	uint de,     // value to subtract from ".link" offsets// s
	uint ss ,   // section size
	uint offset   // offset to resource section
	)
{

int n;
int p;
IMAGE_RESOURCE_DATA_ENTRY pirde;
IMAGE_RESOURCE_DIRECTORY_ENTRY rde;
IMAGE_RESOURCE_DIRECTORY_STRING ids;


	if (lvl == 0) { rp = ird; } // = ird' save pointer to start of resource section
        ++lvl;
    LSet(out IMAGE_RESOURCE_DIRECTORY rd, pcs, ird);
            int irde = ird + sizeof(IMAGE_RESOURCE_DIRECTORY);
    LSet(out rde, pcs, irde);
            //  The named entries are case insensitive strings, sorted in ascending order.
            //  The entries with 16-bit IDs follow these and are also sorted in ascending order.
    for (n = 1; n <= @rd.NumberOfNamedEntries + rd.NumberOfIdEntries; ++n) {

		if ((n <= @rd.NumberOfNamedEntries) && (rde.NameID & IMAGE_RESOURCE_NAME_IS_STRING) != 0) {
			// the name is a string
			p = rp + (int)(@rde.NameID & 0x7FFFFFFF);  // offset is relative to start of resource section

			rid = 0; // assign invalid value as a flag
			ns = Remove(Peek(p + 2, CvWrd(Peek(p, 2)) * 2), '\0'); // unicode string
			if (ns.ToUpper() == "TYPELIB") { fResult = 1; } // found TypeLib data

		} else if ((n > @ird.NumberOfNamedEntries) && (@irde.NameID & IMAGE_RESOURCE_NAME_IS_STRING) == 0) {

                    // it's a 16-bit ID number
                    p = LoWrd(@rde.NameID);
			if (lvl == 1) {
                        typ = (uint) p;
			} else if (lvl == 2) {
                        ns = "";
                        rid = p;
			} else if  (lvl == 3) {
			}
		}

		p = (int)(@rde.Offset & 0x7FFFFFFF);

		if ((@rde.Offset & IMAGE_RESOURCE_DATA_IS_DIRECTORY) != 0) { // it// s a subdirectory
                p = GetResource(pcs, rp + p, de, ss, Offset);

		} else {    // it's actual resource data
			// get offset in PE file to ImageResourceDirectoryEntry structure
			LSet(out pirde, pcs, rp + p);

			if (fResult == 1) {
				++fResult;
				// @pirde.OffsetToData is the RVA to the resource data
				cs = Mid(cs, @pirde.OffsetToData - de - Offset + 1, @pirde.Size);
			}

		}

		if (fResult == 2)
            break;
		irde = irde + sizeof(IMAGE_RESOURCE_DIRECTORY_ENTRY);
        LSet(out rde, pcs, irde);
	}

	-- lvl;
	if (lvl != 0) {
		return ird;
	} else {
		Function = fResult;
		fResult = 0;
	}

            } // GetResource


        private static void LSet<T>(out T d, byte[] cs, int offset)
            where T : struct
        {
            var rdr = new LeImageReader(cs, offset);
            d = rdr.ReadStruct<T>();
        }

        // ***************************************
        //  gets TypeLib data from a PE file's resource section
        unsafe int GetTypeLibData(string ccs, string fs)
{
    //#Register All
    FileStream  ff;// file handle
    int n;
    int fTlb = 0;
    DosHeader DosHdr;
    PEHeader  pPH;
    SectionInfo si = new SectionInfo();
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
        var rdr = new LeImageReader(cs, 0);
		DosHdr = rdr.ReadStruct<DosHeader>();
        uint lfanew = DosHdr.lfaNew;
		if (DosHdr.Magic == 0x5A4D &&
            cs[lfanew] == 'P' && cs[lfanew+1] == 'E' && cs[lfanew+2] == 0 && cs[lfanew+3] == 0) {

			--fTlb; // disable loading the file below
            rdr = new LeImageReader(cs, lfanew);
			pPH = rdr.ReadStruct<PEHeader>();
			pOH = rdr.ReadStruct<OptHeader>();

			// "pOH.NumberOfRvaAndSizes" is the number of entries, not the size of the array, as someone once wrote
			if (ResourceSection > @pOH.NumberOfRvaAndSizes) return 0;

			LSet(out pDD, cs, (int)rdr.Offset + ((ResourceSection - 1) * sizeof(DataDir)));
			si.dRVA = @pDD.RVA      ;
			si.dSize = @pDD.DirSize ;


			// find the section which matches si.dRVA in the section table
            int ish = (int) rdr.Offset + (int) @pOH.NumberOfRvaAndSizes * sizeof(DataDir);
			for (n = 1; n <= @pPH.NumberOfSections; ++n) {
                LSet(out pSH, cs, ish);
				if ((si.dRVA >= @pSH.RVA) && (si.dRVA < @pSH.RVA + @pSH.SizeOfRawData)) {
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
				ish = ish + sizeof(SectionHeader); // advance pSH to next section header
			}

			// get TypeLib resource
			ff.Position = (int)si.PtrToRawData;
			cs = new byte[si.SizeOfRawData];
            ff.Read(cs, 0, cs.Length);
            fixed(byte * pcs = cs) {
			if (GetResource(pcs, si.Delta, si.SizeOfRawData, si.PtrToRawData) == 0) {
				UpdateLog( "No TypeLib data found in: " + fs);
			}
            }

		}


		if (Encoding.ASCII.GetString(cs, 0, 4) == "MSFT") { // it's a "tlb" (TypeLib) file
			++fTlb;
			return 1;
		} else if (Encoding.ASCII.GetString(cs, 0, 4) == "SLTG") {
			++fTlb;
			return 2;
		}

        if (fTlb > 0) {
            ff.Position = 0;
            cs = new byte[ff.Length];
			ff.Read(cs, 0, cs.Length);
		}

		ff.Close();
    } catch
    {
		UpdateLog("Error opening input file: " + fs);
		return -1;
	}
}

#endif
}