using Reko.Core;
using Reko.Core.Hll.C;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Environments.Dreamcast
{
    /// <summary>
    /// This class models the The Visual Memory Unit (VMU), also referred to 
    /// as the Visual Memory System in Japan and Europe, is the primary memory
    /// card produced by Sega for the Dreamcast home video game console. 
    /// </summary>
    public class VmuPlatform : Platform
    {
        public VmuPlatform(IServiceProvider services, IProcessorArchitecture arch, string platformId) 
            : base(services, arch, platformId)
        {
            this.StructureMemberAlignment = 1;
        }

        //$TODO: convert this to typelib file format.
        /*
           {  { 0x0000, "reset"},  // Reset
              { 0x0003, "EINT0"},  // 0x0003 external interrupt 0?
              { 0x000b, "iT0"},  // 0x000b timer/counter 0 interrupt
              { 0x0013, "iEINT1"},  // 0x0013 external interrupt 1?
              { 0x001b, "iT1"},  // 0x001b timer/counter 1 interrupt
              { 0x0023, "iP3"},  // 0x0023 divider circuit/port 1/port 3 interrupt?
              { 0x002b, "int2B"},  // 0x002b interrupt - unknown
              { 0x0033, "int33"},  // 0x0033 interrupt - unknown
              { 0x003b, "int3B"},  // 0x003b interrupt - unknown
              { 0x0043, "int43"},  // 0x0043 interrupt - unknown
              { 0x004b, "int4B"},  // 0x004b interrupt - unknown, but used by football program
              { 0x0100, "int100link"},   // link to ROM code
              { 0x0110, "int110link"},   // link to ROM code
              { 0x0120, "int120link"},   // link to ROM code
              { 0x0130, "intT1link"},   // link to ROM's T1 interrupt code
              { 0x01f0, "quit"},   // exit vector
           };


        codelist_type CODECMTS[] =
           {  { {0xB8, 0x0D,   -1}, ";execute code in other bank"},  // NOT1   EXT, 0
              { {0x23, 0x4c, 0xFF}, "     ;allow us to read buttons"},    // MOV    #$ff,P3  

              { {0x98, 0x4c,   -1}, ";branch if up button pressed"},    // BN     P3, 0, xxx
              { {0x99, 0x4c,   -1}, ";branch if down button pressed"},  // BN     P3, 1, xxx
              { {0x9a, 0x4c,   -1}, ";branch if left button pressed"},  // BN     P3, 2, xxx
              { {0x9b, 0x4c,   -1}, ";branch if right button pressed"}, // BN     P3, 3, xxx
              { {0x9c, 0x4c,   -1}, ";branch if A button pressed"},     // BN     P3, 4, xxx
              { {0x9d, 0x4c,   -1}, ";branch if B button pressed"},     // BN     P3, 5, xxx
              { {0x9e, 0x4c,   -1}, ";branch if Mode button pressed"},  // BN     P3, 6, xxx
              { {0x9f, 0x4c,   -1}, ";branch if Sleep button pressed"}, // BN     P3, 7. xxx

              { {0x78, 0x4c,   -1}, ";branch if up button isn't pressed"},    // BP     P3, 0, xxx
              { {0x79, 0x4c,   -1}, ";branch if down button isn't pressed"},  // BP     P3, 1, xxx
              { {0x7a, 0x4c,   -1}, ";branch if left button isn't pressed"},  // BP     P3, 2, xxx
              { {0x7b, 0x4c,   -1}, ";branch if right button isn't pressed"}, // BP     P3, 3, xxx
              { {0x7c, 0x4c,   -1}, ";branch if A button isn't pressed"},     // BP     P3, 4, xxx
              { {0x7d, 0x4c,   -1}, ";branch if B button isn't pressed"},     // BP     P3, 5, xxx
              { {0x7e, 0x4c,   -1}, ";branch if Mode button isn't pressed"},  // BP     P3, 6, xxx
              { {0x7f, 0x4c,   -1}, ";branch if Sleep button isn't pressed"}, // BP     P3, 7. xxx
              { {0x03, 0x4c,   -1}, "          ;read buttons"}, //  LD     P3

              { {0x78, 0x5c,   -1}, ";branch if Dreamcast connected"}, // BP     P7, 0, L05E4
              { {0x98, 0x5c,   -1}, ";branch if Dreamcast isn't connected"}, // BN     P7, 0, L05E4

              { {0x23, 0x27, 0x00}, "   ;turn off LCD"},                 // MOV    #$00,VCCR
              { {0x23, 0x27, 0x80}, "   ;turn on LCD"},                  // MOV    #$00,VCCR
              { {0xf8, 0x07,   -1}, "     ;halt- sleep until interrupt"},    // SET1   PCON, 0

              { {0x03, 0x66,   -1}, " ;read from work RAM"}, //  LD     VTRBF
              { {0x13, 0x66,   -1}, " ;write to work RAM"},  //  ST     VTRBF

              { {0x23, 0x0e, 0xa1}, "    ;set clock speed, normal (fast?)"},    //   MOV    #$a1,OCR
              { {0x23, 0x0e, 0x81}, "    ;set clock speed, LCD speed (slow?)"}, //   MOV    #$81,OCR

              { {0xfd, 0x0e,   -1}, ";set clock speed, normal (fast?)"},    //   SET1   OCR,5   
              { {0xdd, 0x0e,   -1}, ";set clock speed, LCD speed (slow?)"}, //   CLR1   OCR,5  

              { {0xdf, 0x08,   -1}, " ;disable all interrupts"},         // CLR1   IE, 7
              { {0xd8, 0x4e,   -1}, "    ;disable port 3 interrupts"},   //  CLR1   P3INT, 0
              { {0xf8, 0x4e,   -1}, "    ;enable port 3 interrupts"},    //  SET1   P3INT, 0
              { {0xd9, 0x4e,   -1}, "    ;clear port 3 interrupt flag"}, //  CLR1   P3INT, 1

              { {0x9f, 0x01,   -1}, ";branch if carry clear"}, // BN     PSW, 7, L0B6F
              { {0x7f, 0x01,   -1}, ";branch if carry set"},   // BP     PSW, 7, L0B8D
              { {0xdf, 0x01,   -1}, ";clear carry flag"},      // CLR1   PSW, 7
              { {0xff, 0x01,   -1}, ";set carry flag"},        // SET1   PSW, 7

              { {0xf9, 0x01,   -1}, "      ;unknown function!"},     // SET1   PSW, 1
              { {0xd9, 0x01,   -1}, "      ;unknown function!"},     // CLR1   PSW, 1

              { {0x30,   -1,   -1}, "               ; B:ACC:C <- ACC:B * C"}, //  MUL
              { {0x40,   -1,   -1}, "               ; ACC:C remainder B <- ACC:B / C"}, //  DIV

              { {-1, -1, -1},      "EOL"}        // ---End of list---
           };



        typedef struct {int entry; int exit;} firmwarecall_type;

        // List describing entry and exit points for built-in firmware:
        // The entry point is the code executed after the instruction that modifies
        // the EXT register (typically NOT1 EXT,0); the exit point is where execution
        // resumes.
        firmwarecall_type FIRMWARECALL[] =
           {  { 0x102, 0x105},  // 
              { 0x112, 0x115},  // 
              { 0x122, 0x125},  // 
              { 0x136, 0x139},  // 
              { 0x1f2,    -1},  // exit vector; doesn't return
              {   -1,     -1}   // end of list
           };
    */
        public override string DefaultCallingConvention => throw new NotImplementedException();

        public override SystemService? FindService(int vector, ProcessorState? state, IMemory? memory)
        {
            throw new NotImplementedException();
        }

        public override int GetBitSizeFromCBasicType(CBasicType cb)
        {
            throw new NotImplementedException();
        }

        public override ExternalProcedure? LookupProcedureByName(string? moduleName, string procName)
        {
            throw new NotImplementedException();
        }
    }
}