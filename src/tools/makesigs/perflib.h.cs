/* Perfect hashing function library. Contains functions to generate perfect
	hashing functions */

#if C
!define TRUE 1
!define FALSE 0
!define bool unsigned char
!define byte unsigned char
!define word unsigned short

/* Prototypes */
void hashParams(int NumEntry, int EntryLen, int SetSize, char SetMin,
					int NumVert);	/* Set the parameters for the hash table */
void hashCleanup(void);			/* Frees memory allocated by hashParams() */
void map(void);					/* Part 1 of creating the tables */
void assign(void);				/* Part 2 of creating the tables */
int  hash(byte *s);				/* Hash the string to an int 0 .. NUMENTRY-1 */

word *readT1(void);				/* Returns a pointer to the T1 table */
word *readT2(void);				/* Returns a pointer to the T2 table */
word *readG(void);				/* Returns a pointer to the g  table */


/* The application must provide these functions: */
void getKey(int i, byte **pKeys);/* Set *keys to point to the i+1th key */
void dispKey(int i);			/* Display the key */


/* Macro reads a LH word from the image regardless of host convention */
#if LH
!define LH(p)  ((int)((byte *)(p))[0] + ((int)((byte *)(p))[1] << 8))
#endif
#endif
