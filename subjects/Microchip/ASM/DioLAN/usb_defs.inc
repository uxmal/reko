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
; UEPn Initialization Parameters
#define EP_CTRL		0x06	; Cfg Control pipe for this ep
#define EP_OUT		0x0C	; Cfg OUT only pipe for this ep
#define EP_IN		0x0A	; Cfg IN only pipe for this ep
#define EP_OUT_IN	0x0E	; Cfg both OUT & IN pipes for this ep
#define HSHK_EN		0x10	; Enable handshake packet

; Buffer Descriptor Status Register Initialization Parameters
#define _BSTALL		0x04	; Buffer Stall enable
#define _DTSEN		0x08	; Data Toggle Synch enable
#define _INCDIS		0x10	; Address increment disable
#define _KEN		0x20	; SIE keeps buff descriptors enable
#define _DAT0		0x00	; DATA0 packet expected next
#define _DAT1		0x40	; DATA1 packet expected next
#define _DTSMASK	0x40	; DTS Mask
#define _USIE		0x80	; SIE owns buffer
#define _UCPU		0x00	; CPU owns buffer

; USTAT checking constants
#define OUT		0
#define IN		1
#define EP00_OUT	((0x00 << 3) | (OUT << 2))
#define EP00_IN		((0x00 << 3) | (IN << 2))

; USB PID: Token Types - See chapter 8 in the USB specification
#define SETUP_TOKEN	0x34
#define OUT_TOKEN	0x04
#define IN_TOKEN	0x24

#define EP0_BUFF_SIZE	.64	

; HID
#define HID_INTF_ID	0x00
#define HID_UEP		UEP1
#define HID_BD_OUT	ep1Bo
#define HID_OUT_EP_SIZE	EP0_BUFF_SIZE
#define HID_BD_IN	ep1Bi
#define HID_IN_EP_SIZE	.64

; SETUP packet fields

#define RQ_TYPE_MASK	(0x03 << 5)
#define STANDARD	(0x00 << 5)
#define CLASS		(0x01 << 5)
#define VENDOR		(0x02 << 5)
; Recipient
#define RCPT_MASK	0x1F
#define RCPT_DEV	0
#define RCPT_INTF	1
#define RCPT_EP		2
#define RCPT_OTH	3
; Standard Requests
#define GET_STATUS	.0
#define CLR_FEATURE	.1
#define SET_FEATURE	.3
#define SET_ADR		.5
#define GET_DSC		.6
#define SET_DSC		.7
#define GET_CFG		.8
#define SET_CFG		.9
#define GET_INTF	.10
#define SET_INTF	.11
#define SYNCH_FRAME	.12

; HID Class-Specific Requests
#define GET_REPORT	0x01
#define GET_IDLE	0x02
#define GET_PROTOCOL	0x03
#define SET_REPORT	0x09
#define SET_IDLE	0x0A
#define SET_PROTOCOL	0x0B

; Host-to-device,Device-to-host
#define HOST_TO_DEV	0
#define DEV_TO_HOST	1
; DEVICE_REMOTE_WAKEUP,ENDPOINT_HALT
#define DEVICE_REMOTE_WAKEUP	0x01
#define ENDPOINT_HALT		0x00
;-----------------------------------------------------------------------------
; SETUP packet definitions
#define	bmRequestType	0
#define	bRequest	1
#define	wValue		2
#define	wIndex		4
#define	wLength		6

#define DataDir		7	; Bit number - Transaction Direction: 0-OUT, 1-IN

#define	bDscIndex	2	; For Configuration and String DSC Only
#define	bDscType	3	; Device,Configuration,String
#define	wLangID		4	; Language ID

#define	bDevADR		2	; Device Address 0-127
#define	bDevADRH	3	; Must equal zero

#define bCfgValue	2	; Configuration Value 0-255
#define bCfgRSD		3	; Must equal zero (Reserved)

#define Recipient	0	; Device,Interface,Endpoint,Other

#define EPNum		5	; Endpoint Number 0-15
#define EPDir		7	; Bit number - Endpoint Direction: 0-OUT, 1-IN

#define bAltID		2	; Alternate Setting Value 0-255
#define bAltID_H	3	; Must equal zero
#define bIntfID		4	; Interface Number Value 0-255
#define bIntfID_H	5	; Must equal zero

#define bFeature	2	; DEVICE_REMOTE_WAKEUP, ENDPOINT_HALT
;-----------------------------------------------------------------------------
