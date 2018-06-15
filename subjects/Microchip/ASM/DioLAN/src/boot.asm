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
; BootLoader Main code
;-----------------------------------------------------------------------------
	#include "P18F4455.INC"
	#include "boot.inc"
	#include "io_cfg.inc"
	#include "usb_defs.inc"
	#include "usb_desc.inc"
	#include "usb.inc"
	#include "boot_if.inc"
;-----------------------------------------------------------------------------
; Configuration Bits 
;-----------------------------------------------------------------------------
#if CPU_5V_HS
	CONFIG	PLLDIV = 4			; OSC/4 for 16MHz
	CONFIG  CPUDIV = OSC1_PLL2		; CPU_clk = PLL/2
	CONFIG 	USBDIV = 2			; USB_clk = PLL/2
	CONFIG 	FOSC = HSPLL_HS			; HS osc PLL
#else
	CONFIG	PLLDIV = 4			; OSC/4 for 16MHz
	CONFIG  CPUDIV = OSC1_PLL2		; CPU_clk = Fosc
	CONFIG 	USBDIV = 2			; USB_clk = PLL/2
	CONFIG 	FOSC = HS			; HS osc
#endif
	CONFIG  FCMEN = ON			; Fail Safe Clock Monitor
	CONFIG  IESO = OFF			; Int/Ext switchover mode
	CONFIG  PWRT = ON			; PowerUp Timer
	CONFIG  BOR = OFF			; Brown Out
	CONFIG  VREGEN = ON			; Int Voltage Regulator
	CONFIG  WDT = OFF			; WatchDog Timer
	CONFIG  MCLRE = ON			; MCLR
	CONFIG  LPT1OSC = OFF			; Low Power OSC
	CONFIG  PBADEN = ON			; PORTB<4:0> A/D
	CONFIG  CCP2MX = ON			; CCP2 Mux RC1
	CONFIG  STVREN = ON			; Stack Overflow Reset
	CONFIG  LVP = OFF			; Low Voltage Programming
	CONFIG  ICPRT = OFF			; ICP
	CONFIG  XINST = ON			; Ext CPU Instruction Set
	CONFIG	DEBUG = OFF			; Background Debugging
	CONFIG  CP0 = ON			; Code Protect
	CONFIG  CP1 = ON
	CONFIG  CP2 = ON
	CONFIG  CPB = ON   			; Boot Sect Code Protect
	CONFIG  CPD = OFF  			; EEPROM Data Protect
	CONFIG  WRT0 = OFF 			; Table Write Protect
	CONFIG  WRT1 = OFF
	CONFIG  WRT2 = OFF 
	CONFIG  WRTB = ON  			; Boot Table Write Protest
	CONFIG  WRTC = ON  			; CONFIG Write Protect
	CONFIG  WRTD = OFF 			; EEPROM Write Protect
	CONFIG  EBTR0 = OFF			; Ext Table Read Protect
	CONFIG  EBTR1 = OFF
	CONFIG  EBTR2 = OFF
	CONFIG  EBTRB = ON 			; Boot Table Read Protect
;--------------------------------------------------------------------------
; External declarations
	extern	usb_sm_state
	extern	usb_sm_ctrl_state
	extern	ep1Bo
	extern	ep1Bi
	extern	SetupPkt
	extern	SetupPktCopy
	extern	pSrc
	extern	pDst
	extern	Count
	extern	ctrl_trf_session_owner
	extern	ctrl_trf_mem
	extern	eep_mark_set
;--------------------------------------------------------------------------
; Variables
BOOT_DATA	UDATA
	global	boot_cmd; 
	global	boot_rep;
	global	active_protocol
	global	idle_rate
active_protocol	res	1
idle_rate	res	1
boot_cmd	res	BOOT_CMD_SIZE
boot_rep	res	BOOT_REP_SIZE
;--------------------------------------------------------------------------
; HID buffers
USB_HID		UDATA	0x500
	global	hid_report_out
	global	hid_report_in
hid_report_out	res	HID_OUT_EP_SIZE	; OUT packet buffet
hid_report_in	res	HID_IN_EP_SIZE	; IN packed buffer

;--------------------------------------------------------------------------
BOOT_ASM_CODE CODE
	extern	usb_init
	extern	usb_sm_ctrl
	extern	usb_sm_reset
	extern	usb_sm_prepare_next_setup_trf
	extern	copy_boot_rep
	extern	USB_HID_DESC
	extern	USB_HID_RPT
	extern	hid_process_cmd
;--------------------------------------------------------------------------
; main
; DESCR : Boot Loader main routine.
; WARNING WARNING WARNING WARNING WARNING WARNING WARNING WARNING
; WARNING                                                 WARNING
; WARNING     This code is not a routine!!!               WARNING
; WARNING     RESET command is used to "exit" from main   WARNING
; WARNING                                                 WARNING
; WARNING WARNING WARNING WARNING WARNING WARNING WARNING WARNING
; INPUT : no
; OUTPUT: no
;--------------------------------------------------------------------------
	global	main
main
	UD_INIT
	UD_TX	'X'

	; Decide what to run bootloader or application
#if USE_EEPROM_MARK
	; Check EEPROM mark
	movlw	EEPROM_MARK_ADDR
	movwf	EEADR
	movlw	0x01
	movwf	EECON1
	movlw	EEPROM_MARK
	subwf	EEDATA, W
	bz	bootloader
#endif
	; Check bootloader enable jumper
#ifdef USE_JP_BOOTLOADER_EN
	setf	JP_BOOTLOADER_TRIS
	btfsc	JP_BOOTLOADER_PORT, JP_BOOTLOADER_PIN
	goto	APP_RESET_VECTOR	; Run Application FW
#endif
	; Run bootloader
	bra	bootloader
	reset
;!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
;!!!    WARNING NEVER RETURN IN NORMAL WAY   !!!
;!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
;--------------------------------------------------------------------------
; bootloader
; DESCR : Run the Boot Loader.
; WARNING WARNING WARNING WARNING WARNING WARNING WARNING WARNING
; WARNING                                                 WARNING
; WARNING     This code is not a routine!!!               WARNING
; WARNING     Branch to bootloader occur if firmware      WARNING
; WARNING     updating mode is detected either throuch    WARNING
; WARNING     EEPROM MARK or FW Junper                    WARNING
; WARNING     RESET command is used to "exit"             WARNING
; WARNING     from bootloader                             WARNING
; WARNING                                                 WARNING
; WARNING WARNING WARNING WARNING WARNING WARNING WARNING WARNING
; INPUT : no
; OUTPUT: no
	global	bootloader
bootloader
#ifdef USE_LED
	bcf	LED_TRIS, LED_PIN
	bcf	LED, LED_PIN
#endif
	clrf	eep_mark_set	; EEP_MARK will be cleared
	rcall	usb_init
; Main Loop
bootloader_loop
	rcall	usb_state_machine
	rcall	hid_process_cmd
	rcall	hid_send_report
	bra	bootloader_loop
	reset
;!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
;!!!    WARNING NEVER RETURN IN NORMAL WAY   !!!
;!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
;--------------------------------------------------------------------------
; usb_state_machine
; DESCR : Handles USB state machine according to USB Spec.
;         Process low level action on USB controller.
; INPUT : no
; OUTPUT: no
;-----------------------------------------------------------------------------
	global	usb_state_machine
usb_state_machine
	; Bus Activity detected after IDLE state
usb_state_machine_actif
	btfss	UIR, ACTVIF
	bra	usb_state_machine_actif_end
	btfss	UIE, ACTVIE
	bra	usb_state_machine_actif_end
	UD_TX	'A'
	bcf	UCON, SUSPND
	bcf	UIE, ACTVIE
	bcf	UIR, ACTVIF
usb_state_machine_actif_end
	; Pointless to continue servicing if the device is in suspend mode.
	btfsc	UCON, SUSPND
	return
	; USB Bus Reset Interrupt.
	; When bus reset is received during suspend, ACTVIF will be set first,
	; once the UCONbits.SUSPND is clear, then the URSTIF bit will be asserted.
	; This is why URSTIF is checked after ACTVIF.
	;
	; The USB reset flag is masked when the USB state is in
	; DETACHED_STATE or ATTACHED_STATE, and therefore cannot
	; cause a USB reset event during these two states.
usb_state_machine_rstif
	btfss	UIR, URSTIF
	bra	usb_state_machine_rstif_end
	btfss	UIE, URSTIE
	bra	usb_state_machine_rstif_end
	rcall	usb_sm_reset
usb_state_machine_rstif_end
	; Idle condition detected
usb_state_machine_idleif
	btfss	UIR, IDLEIF
	bra	usb_state_machine_idleif_end
	btfss	UIE, IDLEIE
	bra	usb_state_machine_idleif_end
	UD_TX	'I'
	bsf	UIE, ACTVIE	; Enable bus activity interrupt
	bcf	UIR, IDLEIF
	bsf	UCON, SUSPND	; Put USB module in power conserve
				; mode, SIE clock inactive
        ; Now, go into power saving
        bcf	PIR2, USBIF	; Clear flag
        bsf	PIE2, USBIE	; Set wakeup source
	sleep
        bcf	PIE2, USBIE
usb_state_machine_idleif_end
	; SOF Flag
usb_state_machine_sof
	btfss	UIR, UERRIF
	bra	usb_state_machine_sof_end
	btfss	UIE, UERRIE
	bra	usb_state_machine_sof_end
	UD_TX	'F'
	bcf	UIR, SOFIF
usb_state_machine_sof_end

	; A STALL handshake was sent by the SIE
usb_state_machine_stallif
	btfss	UIR, STALLIF
	bra	usb_state_machine_stallif_end
	btfss	UIE, STALLIE
	bra	usb_state_machine_stallif_end
	UD_TX	'T'
	btfss	UEP0, EPSTALL
	bra	usb_state_machine_stallif_clr
	rcall	usb_sm_prepare_next_setup_trf	; Firmware Work-Around
	bcf	UEP0, EPSTALL
usb_state_machine_stallif_clr
	bcf	UIR, STALLIF
usb_state_machine_stallif_end
	; USB Error flag
usb_state_machine_err
	btfss	UIR, UERRIF
	bra	usb_state_machine_err_end
	btfss	UIE, UERRIE
	bra	usb_state_machine_err_end
	UD_TX	'E'
	bcf	UIR, UERRIF
usb_state_machine_err_end
	; Pointless to continue servicing if the host has not sent a bus reset.
	; Once bus reset is received, the device transitions into the DEFAULT
	; state and is ready for communication.
;    if( usb_sm_state < USB_SM_DEFAULT ) 
;		return;
	movlw	(USB_SM_DEFAULT - 1)	; Be carefull while changing USB_SM_* constants
	cpfsgt	usb_sm_state
	return
	; Detect Interrupt bit
usb_state_machine_trnif
	btfss	UIR, TRNIF
	bra	usb_state_machine_trnif_end
	btfss	UIE, TRNIE
	bra	usb_state_machine_trnif_end
	; Only services transactions over EP0.
	; Ignore all other EP transactions.
	rcall	usb_sm_ctrl
	bcf	UIR, TRNIF
usb_state_machine_trnif_end
	return
;-----------------------------------------------------------------------------
; HID
;-----------------------------------------------------------------------------
; usb_sm_HID_init_EP
; DESCR : Initialize Endpoints for HID
; INPUT : no
; OUTPUT: no
;-----------------------------------------------------------------------------
	global	usb_sm_HID_init_EP
usb_sm_HID_init_EP
#define USE_HID_EP_OUT 0
#if USE_HID_EP_OUT
	movlw	EP_OUT_IN | HSHK_EN
	movwf	HID_UEP		; Enable 2 data pipes
	movlb	HIGH(HID_BD_OUT)
	movlw	HID_OUT_EP_SIZE
	movwf	BDT_CNT(HID_BD_OUT)
	movlw	LOW(hid_report_out)
	movwf	BDT_ADRL(HID_BD_OUT)
	movlw	HIGH(hid_report_out)
	movwf	BDT_ADRH(HID_BD_OUT)
	movlw	(_USIE | _DAT0 | _DTSEN)
	movwf	BDT_STAT(HID_BD_OUT)
#else
	movlw	(EP_IN | HSHK_EN)
	movwf	HID_UEP		; Enable 1 data pipe
#endif
	movlb	HIGH(HID_BD_IN)
	movlw	LOW(hid_report_in)
	movwf	BDT_ADRL(HID_BD_IN)
	movlw	HIGH(hid_report_in)
	movwf	BDT_ADRH(HID_BD_IN)
	movlw	(_UCPU | _DAT1)
	movwf	BDT_STAT(HID_BD_IN)
	movlb	0
	clrf	(boot_rep + cmd)
	return
;--------------------------------------------------------------------------
; hid_send_report
; DESCR : Sends HID reports to host
; INPUT : no
; OUTPUT: no
; Resources:
;	FSR2:	BDTs manipulations
;-----------------------------------------------------------------------------
	global	hid_send_report
hid_send_report
	movf	(boot_rep + cmd), W	; Z flag affected
	bz	hid_send_report_end
	lfsr	FSR2, BDT_STAT(HID_BD_IN)
	btfsc	POSTINC2, UOWN		; BDT_STAT(HID_BD_IN)
	bra	hid_send_report_end
	; Copy boot_rep into hid_report_in
	; Clear bytes of boot_rep
	rcall	copy_boot_rep	; BSR not changed
	; BSR still valid for HID_BD_IN
	movlw	BOOT_REP_SIZE
	; FSR2 points to BDT_CNT(HID_BD_IN)
	movwf	POSTDEC2	; BDT_CNT(HID_BD_IN)
	; FSR2 points to BDT_STAT(HID_BD_IN)
	movlw	_DTSMASK
	andwf	INDF2, F	; BDT_STAT(HID_BD_IN), Save only DTS bit
	movlw	(1 << DTS)
	xorwf	INDF2, F	; BDT_STAT(HID_BD_IN), Toggle DTS bit
	movlw	_USIE|_DTSEN
	iorwf	INDF2, F	; BDT_STAT(HID_BD_IN), Turn ownership to SIE
hid_send_report_end
	return
;--------------------------------------------------------------------------
; usb_sm_HID_request
; DESCR : Process USB HID requests
; INPUT : no
; OUTPUT: no
;-----------------------------------------------------------------------------
	global	usb_sm_HID_request
usb_sm_HID_request
	UD_TX	'H'
	movf	(SetupPktCopy + Recipient), W
	andlw	RCPT_MASK
	sublw	RCPT_INTF
	btfss	STATUS, Z
	return
usb_sm_HID_rq_rcpt
	movf	(SetupPktCopy + bIntfID), W
	sublw	HID_INTF_ID
	btfss	STATUS, Z
	return
usb_sm_HID_rq_rcpt_id
	; There are two standard requests that we may support.
	; 1. GET_DSC(DSC_HID,DSC_RPT,DSC_PHY);
	; 2. SET_DSC(DSC_HID,DSC_RPT,DSC_PHY);
	movf	(SetupPktCopy + bRequest), W
	sublw	GET_DSC
	bnz	usb_sm_HID_rq_cls
	movf	(SetupPktCopy + bDscType), W
	; WREG = WREG - DSC_HID !!!
	addlw	(-DSC_HID)	; DSC_HID = 0x21
	bz	usb_sm_HID_rq_dsc_hid
	dcfsnz	WREG		; DSC_RPT = 0x22
	bra	usb_sm_HID_rq_dsc_rpt
	dcfsnz	WREG		; DSC_PHY = 0x23
	bra	usb_sm_HID_rq_dsc_phy
usb_sm_HID_rq_dsc_unknown
	UD_TX('!')
	bra	usb_sm_HID_rq_cls
;--------	Get DSC_HID descrptor address
usb_sm_HID_rq_dsc_hid
	movlw	LOW(USB_HID_DESC)
	movwf	pSrc
	movlw	HIGH(USB_HID_DESC)
	movwf	(pSrc + 1)
	movlw	USB_HID_DESC_SIZE
usb_sm_HID_rq_dsc_hid_end
	bra	usb_sm_HID_rq_dsc_end
;--------	Get DSC_RPT descrptor address
usb_sm_HID_rq_dsc_rpt
	movlw	LOW(USB_HID_RPT)
	movwf	pSrc
	movlw	HIGH(USB_HID_RPT)
	movwf	(pSrc + 1)
	movlw	USB_HID_RPT_SIZE
usb_sm_HID_rq_dsc_rpt_end
	bra	usb_sm_HID_rq_dsc_end
;--------	Get DSC_PHY descrptor address
usb_sm_HID_rq_dsc_phy
usb_sm_HID_rq_dsc_phy_end
	bra	usb_sm_HID_request_end
;--------
usb_sm_HID_rq_dsc_end
	movwf	Count
	bsf	ctrl_trf_session_owner, 0
	bsf	ctrl_trf_mem, _RAM
	bra	usb_sm_HID_request_end
;--------
; Class Request
usb_sm_HID_rq_cls
	movf	(SetupPktCopy + bmRequestType), W
	andlw	RQ_TYPE_MASK
	sublw	CLASS
	bz	usb_sm_HID_rq_cls_rq
	UD_TX('*')
	return
;--------
usb_sm_HID_rq_cls_rq
	movf	(SetupPktCopy + bRequest), W
	dcfsnz	WREG	; GET_REPORT = 0x01
	bra	usb_sm_HID_rq_cls_rq_grpt
	dcfsnz	WREG	; GET_IDLE = 0x02
	bra	usb_sm_HID_rq_cls_rq_gidle
	dcfsnz	WREG	; GET_PROTOCOL = 0x03
	bra	usb_sm_HID_rq_cls_rq_gprot
	; SET_REPORT = 0x09 -> 9 - 3 = 6
	; WREG = WREG - 6 !!!
	addlw	(-(SET_REPORT - GET_PROTOCOL))
	bz	usb_sm_HID_rq_cls_rq_srpt
	dcfsnz	WREG	; SET_IDLE = 0x0A
	bra	usb_sm_HID_rq_cls_rq_sidle
	dcfsnz	WREG	; SET_PROTOCOL = 0x0B
	bra	usb_sm_HID_rq_cls_rq_sprot
usb_sm_HID_rq_cls_rq_unknown
	UD_TX('#')
	bra	usb_sm_HID_request_end
;--------	GET_REPORT
usb_sm_HID_rq_cls_rq_grpt
	movlw	0		; No data to be transmitted
usb_sm_HID_rq_cls_rq_grpt_end
	bra	usb_sm_HID_rq_cls_rq_end
;--------	SET_REPORT
usb_sm_HID_rq_cls_rq_srpt
	movlw	LOW(boot_cmd)
	movwf	pDst
	movlw	HIGH(boot_cmd)
	movwf	(pDst + 1)
usb_sm_HID_rq_cls_rq_srpt_end
	bra	usb_sm_HID_rq_cls_rq_end_ses
#define GET_SET_IDLE 0
#if GET_SET_IDLE
;--------	GET_IDLE
usb_sm_HID_rq_cls_rq_gidle
	UD_TX('j')
	movlw	LOW(idle_rate)
	movwf	pSrc
	movlw	HIGH(idle_rate)
	movwf	(pSrc + 1)
        movlw	1	; For Count
usb_sm_HID_rq_cls_rq_gidle_end
	bra	usb_sm_HID_rq_cls_rq_end
;--------	SET_IDLE
usb_sm_HID_rq_cls_rq_sidle
	UD_TX('x')
	movff	(SetupPktCopy + (wValue + 1)), idle_rate
usb_sm_HID_rq_cls_rq_sidle_end
	bra	usb_sm_HID_rq_cls_rq_end_ses
#endif
#define GET_SET_PROTOCOL 0
#if GET_SET_PROTOCOL
;--------	GET_PROTOCOL
usb_sm_HID_rq_cls_rq_gprot
	UD_TX('y')
	movlw	LOW(active_protocol)
	movwf	pSrc
	movlw	HIGH(active_protocol)
	movwf	(pSrc + 1)
        movlw	1	; For Count
usb_sm_HID_rq_cls_rq_gprot_end
	bra	usb_sm_HID_rq_cls_rq_end
;--------	SET_PROTOCOL
usb_sm_HID_rq_cls_rq_sprot
	UD_TX('z')
	movf	(SetupPktCopy + wValue), W
	movwf	active_protocol
usb_sm_HID_rq_cls_rq_sprot_end
	bra	usb_sm_HID_rq_cls_rq_end_ses
#endif
usb_sm_HID_rq_cls_rq_end
	movwf	Count
	bcf	ctrl_trf_mem, _RAM
usb_sm_HID_rq_cls_rq_end_ses
	bsf	ctrl_trf_session_owner, 0
;--------
#if !GET_SET_IDLE
usb_sm_HID_rq_cls_rq_gidle
#if UART_DEBUG
	UD_TX('j')
	bra	usb_sm_HID_request_end
#endif
usb_sm_HID_rq_cls_rq_sidle
#if UART_DEBUG
	UD_TX('x')
	bra	usb_sm_HID_request_end
#endif
#endif
#if !GET_SET_PROTOCOL
usb_sm_HID_rq_cls_rq_gprot
#if UART_DEBUG
	UD_TX('y')
	bra	usb_sm_HID_request_end
#endif
usb_sm_HID_rq_cls_rq_sprot
#if UART_DEBUG
	UD_TX('z')
	bra	usb_sm_HID_request_end
#endif
#endif
usb_sm_HID_request_end
	return
;--------------------------------------------------------------------------
	END
