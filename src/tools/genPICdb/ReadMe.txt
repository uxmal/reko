genPICdb
--------

This .NET-based tool is used during development of Reko to generate the Microchip PIC16/PIC18 definitions database.
This database (a .ZIP file) is composed of XML files (one file per PIC model) that contains the characteristics of the PIC
micro-controllers (memory segments, registers, configuration words, ...).
The name of this database is 'picdb.zip' and is the result of the execution of 'genPICdb.exe'.

This tool assumes that, in the development environment, the Microchip MPLAB X IDE has been installed (refer to www.microchip.com).
This IDE contains the definitions of all the Microchip micro-controllers (PIC12, 16, 18, 24, 32 as well as AVR MCUs) from which
only the relevant (to Reko) PIC16 and PIC18 definitions are extracted. In case this MPLAB X IDE is not installed in the current
environment, a default database is created (more exactly we use a copy of the file 'defaultpicdb.zip').

Usage
-----

The 'genPICdb.exe' expects no command parameter. Simply launch it in a DOS or PowerShell window or from the Windows File Explorer.
If all goes well, an updated 'picdb.zip' will be created in the 'genPICdb' source directory.

It is recommended to run this utility each time an updated Microchip MPLAB X IDE is installed in the development environment.
