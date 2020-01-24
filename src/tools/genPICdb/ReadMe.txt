genPICdb
--------

This .NET-based tool is used during development of Reko to generate a Microchip PIC16/PIC18 definitions database.
This database (a .ZIP file) is composed of XML files (one file per PIC model) that contains the characteristics of the PIC
micro-controllers (memory segments, registers, configuration words, ...).
The name of this database is 'picdb.zip' and is the result of the execution of 'genPICdb.exe'.
The 'picdb.zip' file is mandatory for the 'Microchip.Utils.dll' library to work.

This generator tool assumes that, in its execution environment, the Microchip MPLAB X IDE has been installed (refer to www.microchip.com).
This IDE contains the definitions of all the Microchip micro-controllers (PIC12, 16, 18, 24, 32 as well as AVR MCUs) from which
only the relevant (to Reko) PIC16 and PIC18 definitions are extracted.

This utility tool was compiled and runs under:
- Microsoft Windows (7, 10 or higher) with Visual Studio 2019 and .NET Framework 4.7 or higher,
- Linux (Ubuntu 18.04) with Mono Develop 7.8 and Mono Framwork 6.6 or higher,
- MacOS (MacOS Catalina 10.15 in a VMware virtual machine) and Visual Studio for Mac 2019 version 8.4 and Mono Framework 6.6 or higher.


Usage
-----

The 'genPICdb.exe' expects no command parameter.
Simply launch it in a terminal session window (DOS or PowerShell, /bin/bash, Xterm, etc...) using the command line "./genPICdb.exe".
If all goes well, a new 'picdb.zip'is created in the 'genPICdb' execution directory.

It is recommended to run this utility any time an updated Microchip MPLAB X IDE is installed in the development environment to get
the most up-to-date PIC definitions.
