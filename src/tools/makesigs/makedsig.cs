/* Program for making the DCC signature file */

/* Note: there is an untested assumption that the *first* segment definition
	with class CODE will be the one containing all useful functions in the
	LEDATA records. Functions such as _exit() have more than one segment
	declared with class CODE (MSC8 libraries) */

using System;
using System.IO;
using word = System.UInt16;

partial class makedsig
{

/* Symbol table constnts */
const int	SYMLEN	=16;			/* Number of chars in the symbol name, incl null */
const int PATLEN	=23;			/* Number of bytes in the pattern part */
const double C		=2.2;			/* Sparseness of graph. See Czech,
								Havas and Majewski for details */
const int  SYMALLOC = 20;			/* Number of entries in key[] to realloc for at once */
const byte WILD	 = 0xF4	;	/* The value used for wildcards in patterns */

/* prototypes */

//int  readSyms(void);		/* Read the symbols into *keys[] */
//void readPattern(word off);	/* Read the pattern to pat[] */
//void saveFile(void);		/* Save the info */
//void fixWildCards(byte p[]);/* Insert wild cards into the pattern p[] */

byte [] buf=new byte [100], bufSave  =new byte[7];	/* Temp buffer for reading the file */
byte [] pat = new byte[PATLEN+3];			/* Buffer for the procedure pattern */
ushort cvSize, cvSizeSave;	/* Count of bytes remaining in the codeview area */
ushort byte6;					/* A temp */
uint filePos;		/* File position at start of codeview area */
uint imageOffset;	/* Offset of the executable image into the file */

 class HASHENTRY
{
	public char []name = new char[SYMLEN];		/* The symbol name */
	public byte []pat = new byte[PATLEN];		/* The pattern */
	public ushort offset;			/* Offset (needed temporarily) */
} ;

HASHENTRY [] keys;			/* Pointer to the array of keys */
int	 numKeys;				/* Number of useful codeview symbols */
Stream f, f2;				/* .lib and output files respectively */

byte [] leData;				/* Pointer to 64K of alloc'd data. Some .lib files
								have the symbols (PUBDEFs) *after* the data
								(LEDATA), so you need to keep the data here */
ushort maxLeData;				/* How much data we have in there */
	
void
main(int argc, string []argv)
{
	int s;

	if (((argv[1][0] == '-') || (argv[1][0] == '-')) &&
		((argv[1][1] == 'h') || (argv[1][1] == '?')))
	{
		Console.Out.WriteLine(
	"This program is to make 'signatures' of known c library calls for the dcc " +
	"program. It needs as the first arg the name of a library file, and as the " +
	"second arg, the name of the signature file to be generated."
			  );
		Environment.Exit(0);
	}
	if (argc <= 2)
	{
		Console.Out.WriteLine("Usage: makedsig <libname> <signame>\nor makedsig -h for help\n");
		Environment.Exit(1);
	}

	try 
    {
        f = new FileStream(argv[1], FileMode.Open);
	}
    catch
    {
		Console.WriteLine("Cannot read {0}", argv[1]);
		Environment.Exit(2);
	}

	try
    {
        f2 = new FileStream(argv[2], FileMode.Create);
	}
    catch
    {
		Console.WriteLine("Cannot write {0}", argv[2]);
		Environment.Exit(2);
	}

	Console.Error.Write("Seed: ");
	s = Convert.ToInt32(Console.In.ReadLine());
	var rnd = new Random(s);

	numKeys = readSyms();			/* Read the keys (symbols) */

Console.WriteLine("Num keys: {0}; vertices: {1}", numKeys, (int)(numKeys*C));

	hashParams(						/* Set the parameters for the hash table */
		numKeys,					/* The number of symbols */
		PATLEN,						/* The length of the pattern to be hashed */
		256,						/* The character set of the pattern (0-FF) */
		'\0',							/* Minimum pattern character value */
		(int)(numKeys*C));			/* C is the sparseness of the graph. See Czech,
										Havas and Majewski for details */

	/* The following two functions are in perfhlib.c */
	map();							/* Perform the mapping. This will call
										getKey() repeatedly */
	assign();						/* Generate the function g */

	saveFile();						/* Save the resultant information */

	f.Dispose();
	f2.Dispose();
	
}

private Stream StreamReader(string p1,string p2)
{
 	throw new NotImplementedException();
}

/* Called by map(). Return the i+1th key in *pKeys */
void getKey(int i, out byte [] pKeys)
{
    pKeys = keys[i].pat; // (byte *)&keys[i].pat;
}

/* Display key i */
void
dispKey(int i)
{
	Console.Write("{0}", keys[i].name);
}


/*	*	*	*	*	*	*	*	*	*	*	*  *\
*												*
*		R e a d   t h e   l i b   f i l e		*
*												*
\*	*	*	*	*	*	*	*	*	*	*	*  */

/* Read the .lib file, and put the keys into the array *keys[]. Return
	the count */

uint offset;
byte lnum = 0;				/* Count of LNAMES  so far */
byte segnum = 0;			/* Count of SEGDEFs so far */
byte codeLNAMES;			/* Index of the LNAMES for "CODE" class */
byte codeSEGDEF;			/* Index of the first SEGDEF that has class CODE */
const int NONE = 0xFF;			/* Improbable segment index */


//byte readByte(void);
//ushort readWord(void);
//void readString(void);
//void readNN(int n);
//void readNbuf(int n);
//char printable(char c);


/* read a byte from file f */
byte readByte()
{
	int b;

	if ((b = f.ReadByte()) == -1)
	{
		Console.WriteLine("Could not read byte offset {0:X}", offset);
		Environment.Exit(2);
	}
	offset++;
	return (byte)b;
}

ushort readWord()
{
	byte b1, b2;

	b1 = readByte();
	b2 = readByte();

	return b1 + (b2 << 8);
}

void readNbuf(int n)
{
	if (f.Read(buf, 0, n) != n)
	{
		Console.WriteLine("Could not read word offset {0:X}", offset);
		Environment.Exit(2);
	}
	offset += n;
}

void readNN(int n)
{
	try {
        f.Position += n;
	} catch {
		Console.WriteLine("Could not seek file");
		Environment.Exit(2);
	}
	offset += n;
}

/* read a length then string to buf[]; make it an asciiz string */
void readString()
{
	byte len;

	len = readByte();
	if (fread(buf, 1, len, f) != len)
	{
		printf("Could not read string len %d\n", len);
		exit(2);
	}
	buf[len] = '\0';
	offset += len;
}

static int	cAllocSym = 0;

void
allocSym(int count)
{
	/* Reallocate keys[] for count+1 entries, if needed */
	if (count >= cAllocSym)
	{
		cAllocSym += SYMALLOC;
		if ((keys = (HASHENTRY *)realloc(keys, cAllocSym * sizeof(HASHENTRY)))
			== 0)
		{
			Console.WriteLine("Could not realloc keys[] to %d bytes",
				cAllocSym * sizeof(HASHENTRY));
			Environment.Exit(10);
		}
	}
}


int
readSyms()
{
	int i;
	int count = 0;
	int	firstSym = 0;			/* First symbol this module */
	byte b, c, type;
	word w, len;

	codeLNAMES = NONE;			/* Invalidate indexes for code segment */
	codeSEGDEF = NONE;			/* Else won't be assigned */

    offset = 0;                 /* For diagnostics, really */

	if ((keys = (HASHENTRY *)malloc(SYMALLOC * sizeof(HASHENTRY))) == 0)
	{
		printf("Could not malloc the initial %d entries for keys[]\n");
		exit(10);
	}

	if ((leData = (byte *)malloc(0xFF80)) == 0)
	{
		printf("Could not malloc 64k bytes for LEDATA\n"); 
		exit(10);
	}
#if NO
	switch(_heapchk())
	{
		case _HEAPBADBEGIN:
			printf("Bad begin\n");
			break;
		case _HEAPBADNODE:
			printf("Bad node\n");
			break;
		case _HEAPEMPTY:
			printf("Bad empty\n");
			break;
		case _HEAPOK:
			printf("Heap OK\n");
			break;
	}
#endif

	while (true)
	{
		type = readByte();
		len = readWord();
/* Note: uncommenting the following generates a *lot* of output */
/*printf("Offset %05lX: type %02X len %d\n", offset-3, type, len);/**/
		switch (type)
		{
			case 0x96:				/* LNAMES */
				while (len > 1)
				{
				 	readString();
					++lnum;
					if (strcmp(buf, "CODE") == 0)
					{
						/* This is the class name we're looking for */
						codeLNAMES= lnum;
					}
					len -= strlen(buf)+1;
				}
				b = readByte();		/* Checksum */
				break;

			case 0x98:				/* Segment definition */
				b = readByte();		/* Segment attributes */
				if ((b & 0xE0) == 0)
				{
					/* Alignment field is zero. Frame and offset follow */
					readWord();
					readByte();
				}

				w = readWord();		/* Segment length */

				b = readByte();		/* Segment name index */
				++segnum;

				b = readByte();		/* Class name index */
				if ((b == codeLNAMES) && (codeSEGDEF == NONE))
				{
					/* This is the segment defining the code class */
					codeSEGDEF = segnum;
				}

				b = readByte();		/* Overlay index */
				b = readByte();		/* Checksum */
				break;

			case 0x90:				/* PUBDEF: public symbols */
				b = readByte();		/* Base group */
				c = readByte();		/* Base segment */
				len -= 2;
				if (c == 0)
				{
					w = readWord();
					len -= 2;
				}
				while (len > 1)
				{
					readString();
					w = readWord();		/* Offset */
					b = readByte();		/* Type index */
					if (c == codeSEGDEF)
					{
						byte *p;

						allocSym(count);
						p = buf;
						if (buf[0] == '_')	/* Leading underscore? */
						{
							p++; 			/* Yes, remove it*/
						}
						i = min(SYMLEN-1, strlen(p));
						memcpy(keys[count].name, p, i);
						keys[count].name[i] = '\0';
						keys[count].offset = w;
/*printf("%04X: %s is sym #%d\n", w, keys[count].name, count);/**/
						count++;
					}
					len -= strlen(buf) + 1 + 2 + 1;
				}
				b = readByte();		/* Checksum */
				break;


			case 0xA0:				/* LEDATA */
			{
				b = readByte();		/* Segment index */
				w = readWord();		/* Offset */
				len -= 3;
/*printf("LEDATA seg %d off %02X len %Xh, looking for %d\n", b, w, len-1, codeSEGDEF);/**/

				if (b != codeSEGDEF)
				{
					readNN(len);	/* Skip the data */
					break;			/* Next record */
				}


				if (fread(&leData[w], 1, len-1, f) != len-1)
				{
					printf("Could not read LEDATA length %d\n", len-1);
					exit(2);
				}
				offset += len-1;
				maxLeData = max(maxLeData, w+len-1);

			 	readByte();				/* Checksum */
				break;
			}

			default:
				readNN(len);			/* Just skip the lot */

				if (type == 0x8A)	/* Mod end */
				{
				/* Now find all the patterns for public code symbols that
					we have found */
					for (i=firstSym; i < count; i++)
					{
						word off = keys[i].offset;
						if (off == (word)-1)
						{
							continue;			/* Ignore if already done */
						}
						if (keys[i].offset > maxLeData)
						{
							Console.WriteLine(
							"Warning: no LEDATA for symbol #%d %s "+
							"(offset {0:X4}, max {1:X4})",
							i, keys[i].name, off, maxLeData);
							/* To make things consistant, we set the pattern for
								this symbol to nulls */
							memset(&keys[i].pat, 0, PATLEN);
							continue;
						}
						/* Copy to temp buffer so don't overrun later patterns.
							(e.g. when chopping a short pattern).
							Beware of short patterns! */
						if (off+PATLEN <= maxLeData)
						{
							/* Available pattern is >= PATLEN */
							memcpy(buf, &leData[off], PATLEN);
						}
						else
						{
							/* Short! Only copy what is available (and malloced!) */
							memcpy(buf, &leData[off], maxLeData-off);
							/* Set rest to zeroes */
							memset(&buf[maxLeData-off], 0, PATLEN-(maxLeData-off));
						}
						fixWildCards(buf);
						/* Save into the hash entry. */
						memcpy(keys[i].pat, buf, PATLEN);
						keys[i].offset = (word)-1;	/* Flag it as done */
/*printf("Saved pattern for %s\n", keys[i].name);/**/
					}


					while (readByte() == 0);
					readNN(-1);			/* Unget the last byte (= type) */
					lnum = 0;			/* Reset index into lnames */
					segnum = 0;			/* Reset index into snames */
					firstSym = count;	/* Remember index of first sym this mod */
					codeLNAMES = NONE;	/* Invalidate indexes for code segment */
					codeSEGDEF = NONE;
					memset(leData, 0, maxLeData);	/* Clear out old junk */
					maxLeData = 0;		/* No data read this module */
				}

				else if (type == 0xF1)
				{
					/* Library end record */
					return count;
				}

		}
	}


	free(leData);
	free(keys);

	return count;
}


/*	*	*	*	*	*	*	*	*	*	*	*  *\
*												*
*		S a v e   t h e   s i g   f i l e		*
*												*
\*	*	*	*	*	*	*	*	*	*	*	*  */


void
writeFile(byte [] buffer, int len)
{
    try
    {
        f2.Write(buffer, 0, len);
    } 
    catch
    {
        Console.WriteLine("Could not write to file");
        Environment.Exit(1);
    }
}

void
writeFileShort(word w)
{
	byte[] b = new byte[2];

	b[0] = (byte)(w & 0xFF);
    b[1] = (byte)(w >> 8);
    writeFile(b, 2);		/* Write a short little endian */
}

void
saveFile()
{
	int i, len;
	word [] pTable;

	writeFile("dccs", 4);					/* Signature */				
	writeFileShort(numKeys);				/* Number of keys */
	writeFileShort((short)(numKeys * C));	/* Number of vertices */
	writeFileShort(PATLEN);					/* Length of key part of entries */
	writeFileShort(SYMLEN);					/* Length of symbol part of entries */

	/* Write out the tables T1 and T2, with their sig and byte lengths in front */
	writeFile("T1", 2);						/* "Signature" */
	pTable = readT1();
	len = PATLEN * 256;
	writeFileShort(len * sizeof(word));
	for (i=0; i < len; i++)
	{
		writeFileShort(pTable[i]);
	}
	writeFile("T2", 2);
	pTable = readT2();
	writeFileShort(len * sizeof(word));
	for (i=0; i < len; i++)
	{
		writeFileShort(pTable[i]);
	}

	/* Write out g[] */
	writeFile("gg", 2);			  			/* "Signature" */
	pTable = readG();
	len = (short)(numKeys * C);
	writeFileShort(len * sizeof(word));
	for (i=0; i < len; i++)
	{
		writeFileShort(pTable[i]);
	}

	/* Now the hash table itself */
	writeFile("ht ", 2);			  			/* "Signature" */
	writeFileShort(numKeys * (SYMLEN + PATLEN + sizeof(word)));	/* byte len */
	for (i=0; i < numKeys; i++)
	{
		writeFile((char *)&keys[i], SYMLEN + PATLEN);
	}
}

}

