;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;  BootLoader.                                                             ;;
;;  Copyright (C) 2007 Diolan ( http://www.diolan.com )                     ;;
;;                                                                          ;;
;;  This program is free software: you can redistribute it and/or modify    ;;
;;  it under the terms of the GNU General Public License as published by    ;;
;;  the Free Software Foundation, either version 3 of the License, or       ;;
;;  (at your option) any later version.                                     ;;
;;                                                                          ;;
;;  This program is distributed in the hope that it will be useful,         ;;
;;  but WITHOUT ANY WARRANTY; without even the implied warranty of          ;;
;;  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the           ;;
;;  GNU General Public License for more details.                            ;;
;;                                                                          ;;
;;  You should have received a copy of the GNU General Public License       ;;
;;  along with this program.  If not, see <http://www.gnu.org/licenses/>    ;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;-----------------------------------------------------------------------------
; State Machine Variables
;   State Machine State
#define USB_SM_DETACHED		0
#define USB_SM_ATTACHED		1
#define USB_SM_POWERED		2
#define USB_SM_DEFAULT		3
#define USB_SM_ADR_PENDING	4
#define USB_SM_ADDRESS		5
#define USB_SM_CONFIGURED	6
;   State Machine Control I/F state
#define USB_SM_CTRL_WAIT_SETUP	0
#define USB_SM_CTRL_TRF_TX	1
#define USB_SM_CTRL_TRF_RX	2
;-----------------------------------------------------------------------------
; Wrappers to manipulate BDTs
#define BDT_STAT(x)	(x + 0)
#define BDT_CNT(x)	(x + 1)
#define BDT_ADRL(x)	(x + 2)
#define BDT_ADRH(x)	(x + 3)
;-----------------------------------------------------------------------------
; BDT_STAT bits
#define	BC8	0
#define	BC9	1
#define	BSTALL	2	; Buffer Stall Enable
#define	DTSEN	3	; Data Toggle Synch Enable
#define	INCDIS	4	; Address Increment Disable
#define	KEN	5	; BD Keep Enable
#define	DTS	6	; Data Toggle Synch Value
#define	UOWN	7	; USB Ownership
#define	PID0	2	; Packet Identifier Bits
#define	PID1	3
#define	PID2	4
#define	PID3	5
;-----------------------------------------------------------------------------
; Data location flag
#define _RAM	0	; In ASM this interpretted as bit number
#define _ROM	1	
;-----------------------------------------------------------------------------
#define MUID_NULL	0
#define MUID_USB9	1
#define MUID_HID	2
#define MUID_CDC	3
;-----------------------------------------------------------------------------
