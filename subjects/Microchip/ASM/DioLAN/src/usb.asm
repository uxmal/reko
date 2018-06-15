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
; USB functions
;-----------------------------------------------------------------------------
	#include "P18F4455.INC"
	#include "boot.inc"
	#include "usb_defs.inc"
	#include "usb_desc.inc"
	#include "usb.inc"
;-----------------------------------------------------------------------------
; Global variables
BOOT_DATA	UDATA
	global	usb_sm_state
	global	usb_sm_ctrl_state
	global	usb_active_cfg
	global	usb_alt_intf
	global	Count
	global	pDst
	global	pSrc
	global	ctrl_trf_mem
	global	ctrl_trf_session_owner
	global	SetupPktCopy
tmpw			res	2
usb_sm_state		res	1
usb_sm_ctrl_state	res	1
usb_active_cfg		res	1
usb_alt_intf		res	1
Count			res	1	; Data counter
pDst			res	2	; Data destination pointer
pSrc			res	4	; Data source pointer
ctrl_trf_mem		res	1
ctrl_trf_session_owner	res	1
SetupPktCopy		res	8
;-----------------------------------------------------------------------------
; Local variables
byte_to_read		res	1
byte_to_send		res	1
;-----------------------------------------------------------------------------
; USB Fixed Location Variables
; BDT
USB_BDT		UDATA	0x400
	global	ep0Bo
	global	ep0Bi
	global	ep1Bo
	global	ep1Bi
	global	SetupPkt
	global	CtrlTrfData
ep0Bo	res	4	; Endpoint0 BD Out
ep0Bi	res	4	; Endpoint0 BD In
ep1Bo	res	4	; Endpoint1 BD Out
ep1Bi	res	4	; Endpoint1 BD In
; Buffers for SETUP and IN packets
SetupPkt	res	EP0_BUFF_SIZE	; SETUP packet buffer
CtrlTrfData	res	EP0_BUFF_SIZE	; IN packet buffer
;-----------------------------------------------------------------------------
BOOT_ASM_CODE	CODE
	extern	USB_DEV_DESC
	extern	USB_CFG_DESC
	extern	USB_IF_DESC
	extern	USB_HID_DESC
	extern	USB_EP_DESC
	extern	USB_LANG_DESC
	extern	USB_MFG_DESC
#if USE_PROD_STRING
	extern	USB_PROD_DESC
#endif
	extern	usb_sm_HID_init_EP
	extern	usb_sm_HID_request
;-----------------------------------------------------------------------------
; usb_init
; DESCR : Initialize USB controller
; INPUT : no
; OUTPUT: no
;-----------------------------------------------------------------------------
	global	usb_init
usb_init
	movlw	0x14		; Enable Internal Pull-Up
	movwf	UCFG		; Enable Internal Transceiver
				; Full Speed
				; Disable ping-pong
	clrf	UIE
	movlw	0x08
	movwf	UCON		;UCON = 0b00001000;	; Enable USBEN only
	; After enabling the USB module, it takes some time for the voltage
	; on the D+ or D- line to rise high enough to get out of the SE0 condition.
	; The USB Reset interrupt should not be unmasked until the SE0 condition is
	; cleared. This helps preventing the firmware from misinterpreting this
	; unique event as a USB bus reset from the USB host.
	; Wait USB module normal operation
	btfsc	UCON, SE0
	bra	$ - 2		; while(UCONbits.SE0);
	
	clrf	UIR		; Clear all USB interrupts
	movlw	0x11
	movwf	UIE		; UIE = 0b00010001	; Unmask RESET & IDLE interrupts only
	
	movlw	USB_SM_POWERED
	movwf	usb_sm_state
	
	return
;-----------------------------------------------------------------------------
; usb_sm_reset
; DESCR : Reset USB state machine
; INPUT : no
; OUTPUT: no
;-----------------------------------------------------------------------------
	global	usb_sm_reset
usb_sm_reset
	UD_TX	'R'
	clrf	UIR		; Clears all USB interrupts
	movlw	0x7B
	movwf	UIE		; Enable all interrupts except ACTVIE
	clrf	UADDR		; Reset to default address
	movlw	EP_CTRL | HSHK_EN
	movwf	UEP0		; Init EP0 as a Control EP
	; Flush any pending transactions
usb_sm_reset_trnif
	btfss	UIR, TRNIF
	bra	usb_sm_reset_trnif_end
	bcf	UIR, TRNIF
	bra	usb_sm_reset_trnif
usb_sm_reset_trnif_end
	bcf	UCON, PKTDIS	; Make sure packet processing is enabled

	call	usb_sm_prepare_next_setup_trf
	movlw	USB_SM_DEFAULT
	movwf	usb_sm_state
	return
;-----------------------------------------------------------------------------
; usb_sm_prepare_next_setup_trf
; DESCR : Prepare EP0 for next setup transfer
; INPUT : no
; OUTPUT: no
; Resources:
;	FSR2:	BDTs manipulation
;-----------------------------------------------------------------------------
	global	usb_sm_prepare_next_setup_trf
usb_sm_prepare_next_setup_trf
	movlw	USB_SM_CTRL_WAIT_SETUP
	movwf	usb_sm_ctrl_state
	; BDT configuration
	lfsr	FSR2, ep0Bo
	; Buffer OUT configuration
	; BDT CNT
	movlw	EP0_BUFF_SIZE
	movwf	PREINC2		; BDT_CNT(ep0Bo)
	; BDT ADDR
	movlw	LOW(SetupPkt)
	movwf	PREINC2		; BDT_ADRL(ep0Bo)
	movlw	HIGH(SetupPkt)
	movwf	PREINC2		; BDT_ADRH(ep0Bo)
	; BDT STAT
	; Buffer OUT: SIE ownership, DATA0 Expected, Toggle Synch Enabled
	subfsr	FSR2, 3
	movlw	(_USIE | _DAT0 | _DTSEN)
	movwf	INDF2		; BDT_STAT(ep0Bo)
	; Buffer IN configuration. Assume the ep0Bi in the same bank as ep0Bo
	; Buffer IN: CPU ownership
	addfsr	FSR2, 4		; movlb	HIGH(ep0Bi)
	movlw	_UCPU
	movwf	INDF2		; BDT_STAT(ep0Bi)
	return
;-----------------------------------------------------------------------------
; usb_sm_ctrl
; DESCR : Control Transfer over EP0
; INPUT : no
; OUTPUT: no
; Resources:
;	FSR1:	Packet processing
;	FSR2:	BDTs manipulation
;-----------------------------------------------------------------------------
	global	usb_sm_ctrl
usb_sm_ctrl
	movf	USTAT, W
	andlw	0x78
	bnz	usb_sm_ctrl_end	; Non EP0 transaction
	; Copy first 8 bytes of SETUP packet
	lfsr	FSR1, SetupPktCopy
	lfsr	FSR2, SetupPkt
	movlw	8
usb_sm_ctrl_copy_pkt
	movff	POSTINC2, POSTINC1
	decfsz	WREG
	bra	usb_sm_ctrl_copy_pkt
usb_sm_ctrl_process
	lfsr	FSR2, BDT_STAT(ep0Bo)
	movlw	EP00_OUT
	cpfseq	USTAT
	bra	usb_sm_ctrl_in_token
usb_sm_ctrl_out_setup
	movf	INDF2, W	; BDT_STAT(ep0Bo)
	andlw	0x3C
	sublw	SETUP_TOKEN
	bnz	usb_sm_ctrl_out_token
usb_sm_ctrl_setup_token
	rcall	usb_sm_ctrl_setup
	return
usb_sm_ctrl_out_token
	rcall	usb_sm_ctrl_out
	return
usb_sm_ctrl_in_token
	movlw	EP00_IN
	cpfseq	USTAT
	return
	rcall	usb_sm_ctrl_in
usb_sm_ctrl_end
	return
;-----------------------------------------------------------------------------
; usb_sm_ctrl_out
; DESCR : EP0 OUT packet handler
; INPUT : FSR2 point to BDnSTAT for EP0 OUT buffer
; OUTPUT: no
;-----------------------------------------------------------------------------
	global	usb_sm_ctrl_out
usb_sm_ctrl_out
	movlw	USB_SM_CTRL_TRF_RX
	cpfseq	usb_sm_ctrl_state
	bra	usb_sm_ctrl_out_trf_rx_end
usb_sm_ctrl_out_trf_rx
	rcall	usb_sm_ctrl_rx
	; Don't have to worry about overwriting _KEEP bit
	; because if _KEEP was set, TRNIF would not have been
	; generated in the first place.
	; FSR2 Pointed to BDT_STAT(ep0Bo)
	movlw	(_USIE | _DAT1 | _DTSEN)
	btfss	INDF2, DTS	; BDT_STAT(ep0Bo)
	movlw	(_USIE | _DAT0 | _DTSEN)
	movwf	INDF2		; BDT_STAT(ep0Bo)
	return
usb_sm_ctrl_out_trf_rx_end
	; CTRL_TRF_TX
	rcall	usb_sm_prepare_next_setup_trf
	return
;-----------------------------------------------------------------------------
; usb_sm_ctrl_in
; DESCR : EP0 IN packet handler
; INPUT : no
; OUTPUT: no
; Resources:
;	FSR2:	BDTs manipulation
;-----------------------------------------------------------------------------
	global usb_sm_ctrl_in
usb_sm_ctrl_in
	lfsr	FSR2, BDT_STAT(ep0Bi)
	btfsc	INDF2, UOWN	; BDT_STAT(ep0Bi)
	bra	$ - 2
	; Must check if in ADR_PENDING_STATE
	movlw	USB_SM_ADR_PENDING
	cpfseq	usb_sm_state
	bra	usb_sm_ctrl_in_addr_end
usb_sm_ctrl_in_addr
	; SetupPkt copyed to SetupPktCopy
	movf	(SetupPktCopy + bDevADR), W
	movwf	UADDR
	andlw	0xFF
	btfss	STATUS, Z
	movlw	USB_SM_ADDRESS
	btfsc	STATUS, Z
	movlw	USB_SM_DEFAULT
	movwf	usb_sm_state
usb_sm_ctrl_in_addr_end
	movlw	USB_SM_CTRL_TRF_TX
	cpfseq	usb_sm_ctrl_state
	bra	usb_sm_ctrl_in_tx_end
usb_sm_ctrl_in_tx
	; FSR2 must be pointed to BDT_STAT(ep0Bi)
	rcall	usb_sm_ctrl_tx
	; FSR2 must be pointed to BDT_STAT(ep0Bi)
	movlw	(_USIE | _DAT1 | _DTSEN)
	btfss	INDF2, DTS	; BDT_STAT(ep0Bi)
	movlw	(_USIE | _DAT0 | _DTSEN)
	movwf	BDT_STAT(ep0Bi)
	return
usb_sm_ctrl_in_tx_end
	rcall	usb_sm_prepare_next_setup_trf
	return
;-----------------------------------------------------------------------------
; usb_sm_ctrl_rx
; DESCR : Read data from EP
; INPUT : FSR2 point to BDnSTAT for EP0 OUT buffer
; OUTPUT: no
; Resources:
;	FSR0:	Packet processing
;	FSR1:	Packet processing
;	FSR2:	BDTs manipulation
;-----------------------------------------------------------------------------
	global	usb_sm_ctrl_rx
usb_sm_ctrl_rx
	; FSR2 Pointed to ep0Bo
	movf	PREINC2, W	; BDT_CNT(ep0Bo)
	movwf	byte_to_read
	; Accumulate total number of bytes read
	addwf	Count, F
	; pSrc.bRam = (byte*)&CtrlTrfData;
	; FSR1 = source address
	lfsr	FSR1, CtrlTrfData
	; FSR0 = destination address
	movff	(pDst + 1), FSR0H
	movff	pDst, FSR0L
usb_sm_ctrl_rx_read
	movff	POSTINC1, POSTINC0
	decfsz	byte_to_read
	bra	usb_sm_ctrl_rx_read
	movff	FSR0L, pDst
	movff	FSR0H, (pDst + 1)
usb_sm_ctrl_rx_read_end
	return
;-----------------------------------------------------------------------------
; usb_sm_ctrl_tx
; DESCR : Write data to EP
; INPUT : FSR2 point to BDnSTAT for EP0 IN buffer
; OUTPUT: no
; Resources:
;	FSR0:	Packet processing
;	FSR1:	Packet processing
;	FSR2:	BDTs manipulation _MUST_ be pointed to BDT_STAT(ep0Bi)
;-----------------------------------------------------------------------------
	global	usb_sm_ctrl_tx
usb_sm_ctrl_tx
	; FSR2 pointed to BDT_STAT(ep0Bi)
	; First, have to figure out how many byte of data to send
	movlw	EP0_BUFF_SIZE
	cpfslt	Count
	bra	usb_sm_ctrl_tx_size_end
	movf	Count, W
usb_sm_ctrl_tx_size_end
	movwf	byte_to_send
	; Load the number of bytes to send to BC9..0 in buffer descriptor
	bcf	INDF2, BC9	; BDT_STAT(ep0Bi)
	bcf	INDF2, BC8	; BDT_STAT(ep0Bi)
	movwf	PREINC2		; BDT_CNT(ep0Bi): WREG = byte_to_send
	; Subtract the number of bytes just about to be sent from the total
	subwf	Count, F

	; FSR0 = destination address
	lfsr	FSR0, CtrlTrfData
;    pDst.bRam = (byte*)&CtrlTrfData;
	; Determine type of memory source
	btfsc	ctrl_trf_mem, _RAM
	bra	usb_sm_ctrl_tx_rom
usb_sm_ctrl_tx_ram
	movf	byte_to_send, W	; Z flag affected
	bz	usb_sm_ctrl_tx_end
	; FSR1 = source address
	movff	(pSrc + 1), FSR1H
	movff	pSrc, FSR1L
usb_sm_ctrl_tx_ram_write
	movff	POSTINC1, POSTINC0
	decfsz	byte_to_send, F
	bra	usb_sm_ctrl_tx_ram_write
	movff	FSR1L, pSrc
	movff	FSR1H, (pSrc + 1)
	bra	usb_sm_ctrl_tx_end
usb_sm_ctrl_tx_rom
	clrf	TBLPTRU	; Descroprors, HID reports and others located below 256 bytes boundary
	clrf	TBLPTRH
	movf	pSrc, W
	movwf	TBLPTRL
usb_sm_ctrl_tx_rom_write
	tblrd*+
	movff	TABLAT, POSTINC0
	decfsz	byte_to_send, F
	bra	usb_sm_ctrl_tx_rom_write
	movff	TBLPTRL, pSrc
;	movff	TBLPTRH, (pSrc + 1)
usb_sm_ctrl_tx_end
	return
;-----------------------------------------------------------------------------
; usb_sm_ctrl_setup
; DESCR : Write data to EP
; INPUT : no
; OUTPUT: no
; Resources:
;	FSR0:	SetupPacket processing
;	FSR1:	CtrlTrfData processing
;	FSR2:	BDT manipulations
;-----------------------------------------------------------------------------
	global	usb_sm_ctrl_setup
usb_sm_ctrl_setup
	UD_TX	'S'
	; Stage 1
	movlw	USB_SM_CTRL_WAIT_SETUP
	movwf	usb_sm_ctrl_state
	clrf	ctrl_trf_session_owner
	clrf	Count
	
	; Determine Request type
usb_sm_ctrl_setup_rqtype
	movf	SetupPktCopy, W
	andlw	RQ_TYPE_MASK
	btfsc	STATUS,Z	; STANDARD = x00xxxxx
	bra	usb_sm_ctrl_setup_sdtrq
	sublw	CLASS
	btfsc	STATUS,Z	; CLASS = x01xxxxx
	bra	usb_sm_ctrl_setup_clsrq
	; No vendor requests
	UD_TX	'V'
	bra	usb_sm_ctrl_setup_stall_ep
usb_sm_ctrl_setup_rqtype_end
;--------	Process STANDARD request
usb_sm_ctrl_setup_sdtrq
	; Determine Request
	movf	(SetupPktCopy + bRequest), W	; Z flag affected
	bz	usb_sm_ctrl_setup_sdtrq_gets
	dcfsnz	WREG		; CLR_FEATURE = 1
	bra	usb_sm_ctrl_setup_sdtrq_clrf
	decf	WREG		; bRequest = 2 is reserved
	dcfsnz	WREG		;SET_FEATURE = 3
	bra	usb_sm_ctrl_setup_sdtrq_setf
	decf	WREG		; bRequest = 4 is reserved
	dcfsnz	WREG		;SET_ADR = 5
	bra	usb_sm_ctrl_setup_sdtrq_seta
	dcfsnz	WREG		;GET_DSC = 6
	bra	usb_sm_ctrl_setup_sdtrq_getd
	dcfsnz	WREG		;SET_DSC = 7
	bra	usb_sm_ctrl_setup_sdtrq_setd
	dcfsnz	WREG		;GET_CFG = 8
	bra	usb_sm_ctrl_setup_sdtrq_getc
	dcfsnz	WREG		;SET_CFG = 9
	bra	usb_sm_ctrl_setup_sdtrq_setc
	dcfsnz	WREG		;GET_INTF = 10
	bra	usb_sm_ctrl_setup_sdtrq_geti
	dcfsnz	WREG		;SET_INTF = 11
	bra	usb_sm_ctrl_setup_sdtrq_seti
	dcfsnz	WREG		;SYNCH_FRAME = 12
	bra	usb_sm_ctrl_setup_sdtrq_sf
	; Unknown request
	UD_TX('U')
	bra	usb_sm_ctrl_setup_stall_ep
;--------	GET_STATUS
usb_sm_ctrl_setup_sdtrq_gets
	lfsr	FSR0, CtrlTrfData
	clrf	POSTINC0
	clrf	POSTINC0
; Due to code size limits do not fully
; process GET_STATUS. 
; Simply return 0x0000 status - everything good
#define GET_STATUS_FULLY_SUPPORTED 0
#if GET_STATUS_FULLY_SUPPORTED
;!!!!!!!!!!!!!!!!!!!!!!!!!!
;!!!   NOT TESTED YET   !!!
;!!!!!!!!!!!!!!!!!!!!!!!!!!
	movf	SetupPktCopy, W
	andlw	RCPT_MASK	; RCPT_DEV = 0
	bz	usb_sm_ctrl_setup_sdtrq_gets_end
	dcfsnz	WREG	; RCPT_INTF = 1
	bra	usb_sm_ctrl_setup_sdtrq_gets_end
	dcfsnz	WREG	; RCPT_EP = 2
	bra	usb_sm_ctrl_setup_sdtrq_gets_re
	dcfsnz	WREG	; RCPT_OTH = 3
usb_sm_ctrl_setup_sdtrq_gets_ro
	bra	usb_sm_ctrl_setup_stall_ep
usb_sm_ctrl_setup_sdtrq_gets_re
	movff	(SetupPktCopy + EPNum), byte_to_read	; Save EPDir
	; pDst.bRam = (byte*) &ep0Bo + (SetupPkt.EPNum * 8) + (SetupPkt.EPDir * 4)
	movlw	0
	btfss	(SetupPktCopy + EPNum), EPDir
	movlw	4
	movwf	pDst	; SetupPkt.EPDir * 4
	clrf	(pDst + 1)
	bcf	(SetupPktCopy + EPNum), EPDir
	bcf	STATUS, C
	rlcf	(SetupPktCopy + EPNum), F
	rlcf	(SetupPktCopy + EPNum), F
	rlcf	(SetupPktCopy + EPNum), W
	iorwf	(SetupPktCopy + EPNum), F	; SetupPkt.EPNum * 8
	movlw	LOW(ep0Bo)
	addwf	pDst, F
	movlw	HIGH(ep0Bo)
	btfss	STATUS, C
	addlw	1
	movwf	(pDst + 1)
	;
	lfsr	FSR1, pDst
	lfsr	FSR0, CtrlTrfData
	movlw	1
	btfsc	INDF1, BSTALL	; Use BSTALL as a bit mask
	movwf	INDF0		; Set bit0 to indicate that EP Halted
	movff	byte_to_read, (SetupPktCopy + EPNum)	; Restore EPDir
#endif ; GET_STATUS_FULLY_SUPPORTED
	movlw	LOW(CtrlTrfData)
	movwf	pSrc
	movlw	HIGH(CtrlTrfData)
	movwf	(pSrc + 1)
	movlw	HIGH(Count)
	movlw	2
	movwf	Count
	bsf	ctrl_trf_session_owner, 0
usb_sm_ctrl_setup_sdtrq_gets_end
	bra	usb_sm_ctrl_setup_sdtrq_end	
;--------	CLR_FEATURE	SET_FEATURE
usb_sm_ctrl_setup_sdtrq_clrf
usb_sm_ctrl_setup_sdtrq_setf
; Due to code size limits CLR/SET FEATURE 
; not be supported
#define CLR_SET_FEATURE_SUPPORTED 0
#if CLR_SET_FEATURE_SUPPORTED
;!!!!!!!!!!!!!!!!!!!!!!!!!!
;!!!   NOT TESTED YET   !!!
;!!!!!!!!!!!!!!!!!!!!!!!!!!
	movlw	DEVICE_REMOTE_WAKEUP
	cpfseq	(SetupPktCopy + bFeature)
	bra	usb_sm_ctrl_setup_sdtrq_scf_wue
	movf	(SetupPktCopy + Recipient), F
	andlw	RCPT_MASK
	sublw	RCPT_DEV
	bnz	usb_sm_ctrl_setup_sdtrq_scf_wue
	; Bootloader does not support remote WakeUp
	bsf	ctrl_trf_session_owner, 0
usb_sm_ctrl_setup_sdtrq_scf_wue
	movlw	ENDPOINT_HALT
	cpfseq	(SetupPktCopy + bFeature)
	bra	usb_sm_ctrl_setup_sdtrq_scf_end
	movf	(SetupPktCopy + Recipient), F
	andlw	RCPT_MASK
	sublw	RCPT_EP
	bnz	usb_sm_ctrl_setup_sdtrq_scf_end
	movf	(SetupPktCopy + EPNum), W
	andlw	0x0F
	bz	usb_sm_ctrl_setup_sdtrq_scf_end
	movlb	HIGH(ctrl_trf_session_owner)
	bsf	ctrl_trf_session_owner, 0
	; Must do address calculation here
	movff	(SetupPktCopy + EPNum), byte_to_read	; Save EPDir
	; pDst.bRam = (byte*) &ep0Bo + (SetupPkt.EPNum * 8) + (SetupPkt.EPDir * 4)
	movlw	0
	btfss	(SetupPktCopy + EPNum), EPDir
	movlw	4
	movwf	pDst	; SetupPkt.EPDir * 4
	clrf	(pDst + 1)
	bcf	(SetupPktCopy + EPNum), EPDir
	bcf	STATUS, C
	rlcf	(SetupPktCopy + EPNum), F
	rlcf	(SetupPktCopy + EPNum), F
	rlcf	(SetupPktCopy + EPNum), W
	iorwf	(SetupPktCopy + EPNum), F	; SetupPkt.EPNum * 8
	movlw	LOW(ep0Bo)
	addwf	pDst, F
	movlw	HIGH(ep0Bo)
	btfss	STATUS, C
	addlw	1
	movwf	(pDst + 1)
	;
	lfsr	FSR1, pDst
	movlw	SET_FEATURE
	cpfseq	(SetupPktCopy + bRequest)
	bra	usb_sm_ctrl_setup_sdtrq_scf_c
	movlw	(_USIE | _BSTALL)
	movwf	INDF1
	bra	usb_sm_ctrl_setup_sdtrq_scf_e
usb_sm_ctrl_setup_sdtrq_scf_c
	; Saved EPDir
	movlw	_UCPU
	btfss	byte_to_read, EPDir
	movlw	(_USIE | _DAT0 | _DTSEN)
	movwf	INDF1
usb_sm_ctrl_setup_sdtrq_scf_e
	movff	byte_to_read, (SetupPktCopy + EPNum)	; Restore EPDir
usb_sm_ctrl_setup_sdtrq_scf_end
#endif
; CLR_SET_FEATURE_SUPPORTED
usb_sm_ctrl_setup_sdtrq_clrf_end
usb_sm_ctrl_setup_sdtrq_setf_end
	bra	usb_sm_ctrl_setup_sdtrq_end
;--------	SET_ADR
usb_sm_ctrl_setup_sdtrq_seta
	bsf	ctrl_trf_session_owner, 0
	movlw	USB_SM_ADR_PENDING
	movwf	usb_sm_state
	bra	usb_sm_ctrl_setup_sdtrq_end
usb_sm_ctrl_setup_sdtrq_seta_end
;--------	GET_DSC
usb_sm_ctrl_setup_sdtrq_getd
	bsf	ctrl_trf_session_owner, 0
	movf	(SetupPktCopy + bDscType), W	; Point to wValue high byte
	dcfsnz	WREG		; DSC_DEV = 0x01
	bra	usb_sm_ctrl_setup_dsc_dev
	dcfsnz	WREG		; DSC_CFG = 0x02
	bra	usb_sm_ctrl_setup_dsc_cfg
	dcfsnz	WREG		; DSC_STR = 0x03
	bra	usb_sm_ctrl_setup_dsc_str
	dcfsnz	WREG		; DSC_INTF = 0x04
	bra	usb_sm_ctrl_setup_dsc_if
	dcfsnz	WREG		; DSC_EP = 0x05
	bra	usb_sm_ctrl_setup_dsc_ep
usb_sm_ctrl_setup_dsc_unknown
	UD_TX('?')
	clrf	ctrl_trf_session_owner
;	bra	usb_sm_ctrl_setup_stall_ep
	bra	usb_sm_ctrl_setup_sdtrq_end
;--------	Get DSC_DEV descrptor address
usb_sm_ctrl_setup_dsc_dev
	movlw	LOW(USB_DEV_DESC)
	movwf	pSrc
	movlw	USB_DEV_DESC_SIZE
	bra	usb_sm_ctrl_setup_sdtrq_getd_end
usb_sm_ctrl_setup_dsc_dev_end
;--------	Get DSC_CFG descrptor address
usb_sm_ctrl_setup_dsc_cfg
	movlw	LOW(USB_CFG_DESC)
	movwf	pSrc
	movlw	USB_CFG_TOTAL_SIZE
	bra	usb_sm_ctrl_setup_sdtrq_getd_end
usb_sm_ctrl_setup_dsc_cfg_end
;--------	Get DSC_STR descrptor address
usb_sm_ctrl_setup_dsc_str
	; Get String Descriptor
	; Point to wValue low byte
	movf	(SetupPktCopy + bDscIndex), W	; String Descriptor Index, Z flag affected
	bnz	usb_sm_ctrl_setup_dsc_str_mfg
usb_sm_ctrl_setup_dsc_str_lng
	movlw	LOW(USB_LANG_DESC)
	movwf	pSrc
	movlw	USB_LANG_DESC_SIZE
	bra	usb_sm_ctrl_setup_dsc_str_end
usb_sm_ctrl_setup_dsc_str_mfg
	movlw	LOW(USB_MFG_DESC)
	movwf	pSrc
	movlw	USB_MFG_DESC_SIZE
usb_sm_ctrl_setup_dsc_str_end
	bra	usb_sm_ctrl_setup_sdtrq_getd_end
;--------	Get DSC_INTF descrptor address
usb_sm_ctrl_setup_dsc_if
usb_sm_ctrl_setup_dsc_if_end
	bra	usb_sm_ctrl_setup_dsc_unknown
;--------	Get DSC_EP descrptor address
usb_sm_ctrl_setup_dsc_ep
usb_sm_ctrl_setup_dsc_ep_end
	bra	usb_sm_ctrl_setup_dsc_unknown
;--------------------------------------------
usb_sm_ctrl_setup_sdtrq_getd_end
	movwf	Count
	bsf	ctrl_trf_mem, _RAM
	bra	usb_sm_ctrl_setup_sdtrq_end
;--------	SET_DSC
usb_sm_ctrl_setup_sdtrq_setd
usb_sm_ctrl_setup_sdtrq_setd_end
	bra	usb_sm_ctrl_setup_sdtrq_end
;--------	GET_CFG
usb_sm_ctrl_setup_sdtrq_getc
	bsf	ctrl_trf_session_owner, 0
	movlw	LOW(usb_active_cfg)
	movwf	pSrc
	movlw	HIGH(usb_active_cfg)
	movwf	(pSrc + 1)
	bcf	ctrl_trf_mem, _RAM
	movlw	1
	movwf	Count
	bra	usb_sm_ctrl_setup_sdtrq_end
usb_sm_ctrl_setup_sdtrq_getc_end
;--------	SET_CFG
usb_sm_ctrl_setup_sdtrq_setc
	bsf	ctrl_trf_session_owner, 0
	clrf	UEP1	; Disable EP1
	movf	(SetupPktCopy + bCfgValue), W
	movwf	usb_active_cfg
	andlw	0xFF
	bz	usb_sm_ctrl_setup_sdtrq_setc_0
usb_sm_ctrl_setup_sdtrq_setc_x
	movlw	USB_SM_CONFIGURED
	movwf	usb_sm_state
	; Modifiable Section
	rcall	usb_sm_HID_init_EP
	bra	usb_sm_ctrl_setup_sdtrq_setc_end
usb_sm_ctrl_setup_sdtrq_setc_0			
	movlw	USB_SM_ADDRESS
	movwf	usb_sm_state
usb_sm_ctrl_setup_sdtrq_setc_end
	bra	usb_sm_ctrl_setup_sdtrq_end
;--------	GET_INTF
usb_sm_ctrl_setup_sdtrq_geti
	bsf	ctrl_trf_session_owner, 0
	movlw	LOW(usb_alt_intf)
	movwf	pSrc
	movlw	HIGH(usb_alt_intf)
	movwf	(pSrc + 1)
	bcf	ctrl_trf_mem, _RAM
	movlw	1
	movwf	Count
usb_sm_ctrl_setup_sdtrq_geti_end
	bra	usb_sm_ctrl_setup_sdtrq_end
;--------	SET_INTF
usb_sm_ctrl_setup_sdtrq_seti
	bsf	ctrl_trf_session_owner, 0
	movff	(SetupPktCopy + bAltID), usb_alt_intf
usb_sm_ctrl_setup_sdtrq_seti_end
	bra	usb_sm_ctrl_setup_sdtrq_end
;--------	SYNCH_FRAME
usb_sm_ctrl_setup_sdtrq_sf
usb_sm_ctrl_setup_sdtrq_sf_end
	bra	usb_sm_ctrl_setup_sdtrq_end
;--------	End of STANDARD requests
usb_sm_ctrl_setup_sdtrq_end
	bra	usb_sm_ctrl_setup_end
;--------	Process CLASS request
usb_sm_ctrl_setup_clsrq
	rcall	usb_sm_HID_request
	bra	usb_sm_ctrl_setup_end_run_c
usb_sm_ctrl_setup_clsrq_end
	; Send STALL
usb_sm_ctrl_setup_stall_ep
	bra	usb_stall_ep0	; Return to caller from usb_stall_ep0
usb_sm_ctrl_setup_stall_ep_end
	; End of usb_sm_ctrl_setup
usb_sm_ctrl_setup_end
	btfsc	ctrl_trf_session_owner, 0
	bra	usb_sm_ctrl_setup_end_run_c
	bra	usb_sm_ctrl_setup_clsrq
usb_sm_ctrl_setup_end_run_c
	; PKTDIS bit is set when a Setup Transaction is received.
	; Clear to resume packet processing.
	bcf	UCON, PKTDIS
	btfsc	ctrl_trf_session_owner, 0
	bra	usb_sm_ctrl_setup_end_null
	; If no one knows how to service this request then stall.
	bra	usb_stall_ep0	; Return to caller from usb_stall_ep0
usb_sm_ctrl_setup_end_null
	btfss	SetupPktCopy, DataDir
	bra	usb_sm_ctrl_setup_end_out
usb_sm_ctrl_setup_end_in
	; DEV_TO_HOST
	movf	(SetupPktCopy + wLength), W
	cpfslt	Count	; wLegth < Count
	movwf	Count	; Yes Count = wLegth
	; FSR2 must be pointed to BDT_STAT(ep0Bi)
	lfsr	FSR2, BDT_STAT(ep0Bi)
	rcall	usb_sm_ctrl_tx
	; Prepare OUT EP to respond to early termination
	;
	; Since the SETUP transaction requires the DTS bit to be
	; DAT0 while the zero length OUT status requires the DTS
	; bit to be DAT1, the DTS bit check by the hardware should
	; be disabled. This way the SIE could accept either of
	; the two transactions.
	movlw	USB_SM_CTRL_TRF_TX
	movwf	usb_sm_ctrl_state
	lfsr	FSR2, BDT_ADRH(ep0Bo)
	movlw	HIGH(SetupPkt)
	movwf	POSTDEC2		; BDT_ADRH(ep0Bo)
	movlw	LOW(SetupPkt)
	movwf	POSTDEC2		; BDT_ADRL(ep0Bo)
	movlw	EP0_BUFF_SIZE
	movwf	POSTDEC2		; BDT_CNT(ep0Bo)
	movlw	_USIE			; Note: DTSEN is 0!
	movwf	POSTDEC2		; BDT_STAT(ep0Bo)
	; Prepare IN EP to transfer data, Cnt should have
	; been initialized by responsible request owner.
	lfsr	FSR2, BDT_ADRH(ep0Bi)
	movlw	HIGH(CtrlTrfData)
	movwf	POSTDEC2		; BDT_ADRH(ep0Bi)
	movlw	LOW(CtrlTrfData)
	movwf	POSTDEC2		; BDT_ADRL(ep0Bi)
	subfsr	FSR2, 1
	movlw	(_USIE | _DAT1 | _DTSEN)
	movwf	POSTDEC2		; BDT_STAT(ep0Bi)
	bra	usb_sm_ctrl_setup_end_in_out
usb_sm_ctrl_setup_end_out
	; HOST_TO_DEV
	movlw	USB_SM_CTRL_TRF_RX
	movwf	usb_sm_ctrl_state
	; Prepare IN EP to respond to early termination
	; This is the same as a Zero Length Packet Response
	; for control transfer without a data stage
	lfsr	FSR2, BDT_CNT(ep0Bi)
	clrf	POSTDEC2		; BDT_CNT(ep0Bi)
	movlw	(_USIE | _DAT1 | _DTSEN)
	movwf	POSTDEC2		; BDT_STAT(ep0Bi)
	; Now FSR2 point to ep0Bo ADRH
	; Prepare OUT EP to receive data.
	movlw	HIGH(CtrlTrfData)
	movwf	POSTDEC2		; BDT_ADRH(ep0Bo)
	movlw	LOW(CtrlTrfData)
	movwf	POSTDEC2		; BDT_ADRL(ep0Bo)
	movlw	EP0_BUFF_SIZE
	movwf	POSTDEC2		; BDT_CNT(ep0Bo)
	movlw	(_USIE | _DAT1 | _DTSEN)
	movwf	POSTDEC2		; BDT_STAT(ep0Bo)
usb_sm_ctrl_setup_end_in_out
	; End Stage 3
	return
;-----------------------------------------------------------------------------
; usb_stall_ep0
; DESCR : Set STALL for EP0
; INPUT : no
; OUTPUT: no
; Resources:
;	FSR2:	BDT manipulations
;-----------------------------------------------------------------------------
usb_stall_ep0
	UD_TX	't'
	;bsf	UEP0, EPSTALL
	; Must also prepare EP0 to receive the next SETUP transaction.
	lfsr	FSR2, BDT_ADRH(ep0Bo)
	movlw	HIGH(SetupPkt)
	movwf	POSTDEC2		; BDT_ADRH(ep0Bo)
	movlw	LOW(SetupPkt)
	movwf	POSTDEC2		; BDT_ADRL(ep0Bo)
	movlw	EP0_BUFF_SIZE
	movwf	POSTDEC2		; BDT_CNT(ep0Bo)
	movlw	(_USIE | _BSTALL)
	movwf	INDF2		; BDT_STAT(ep0Bo)
	addfsr	FSR2, 4
	movwf	INDF2			; BDT_STAT(ep0Bi)
	bcf	UCON, PKTDIS
	return
;-----------------------------------------------------------------------------
	END
