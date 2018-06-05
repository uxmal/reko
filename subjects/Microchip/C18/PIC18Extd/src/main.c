//=============================================================================
// Filename: MAIN.C
//=============================================================================
//=============================================================================
// Compiled using MPLAB-C18 V3.47
// Target processor: PIC18F1230
//=============================================================================
//
// Example code to generate a TMR0 interrupt and toggle LEDs on pins RB0 and
// RB7. Toggles RB0 in the interrupt routine and sets RB7 to match RB0 in the
// main routine. This demonstrates that code is executing in both routines.
//
//=============================================================================

//----------------------------------------------------------------------------

#include <p18cxxx.h>

#pragma config OSC = HS
#pragma config XINST = ON

//----------------------------------------------------------------------------

void main (void);
void doloop (char flag);
void InterruptHandlerHigh (void);

union
{
  struct
  {
    unsigned Timeout:1;         // flag to indicate a TMR0 timeout
    unsigned None:7;
  } Bit;
  unsigned char Byte;
} Flags;

//----------------------------------------------------------------------------
// Main routine

void main ()
{
  Flags.Byte = 0;
  INTCON = 0x20;                //disable global and enable TMR0 interrupt
  INTCON2 = 0x84;               //TMR0 high priority
  RCONbits.IPEN = 1;            //enable priority levels
  TMR0H = 0;                    //clear timer
  TMR0L = 0;                    //clear timer
  T0CON = 0x82;                 //set up timer0 - prescaler 1:8
  INTCONbits.GIEH = 1;          //enable interrupts
  TRISB = 0;

  doloop(1);

}

void doloop(char loop)
{
while (loop)
    {
      if (Flags.Bit.Timeout == 1)
        {                                  //timeout?
          Flags.Bit.Timeout = 0;           //clear timeout indicor
          LATBbits.LATB7 = LATBbits.LATB0; //copy LED state from RB0 to RB7
        }
    }

}

//----------------------------------------------------------------------------
// High priority interrupt vector

#pragma code InterruptVectorHigh = 0x08
void InterruptVectorHigh (void)
{
  _asm
    goto InterruptHandlerHigh //jump to interrupt routine
  _endasm
}

//----------------------------------------------------------------------------
// High priority interrupt routine

#pragma code
#pragma interrupt InterruptHandlerHigh

void InterruptHandlerHigh ()
{
  if (INTCONbits.TMR0IF)
    {                                   //check for TMR0 overflow
      INTCONbits.TMR0IF = 0;            //clear interrupt flag
      Flags.Bit.Timeout = 1;            //indicate timeout
      LATBbits.LATB0 = !LATBbits.LATB0; //toggle LED on RB0
    }
}

//----------------------------------------------------------------------------
