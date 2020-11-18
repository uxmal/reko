;;; Segment normal (0100)

;; fn0100: 0100
fn0100 proc
	lim	gp0,#0x111
	sjs	gp15,0105
	urs	gp15

;; fn0105: 0105
;;   Called from:
;;     0102 (in fn0100)
fn0105 proc
	lr	gp1,gp0

l0106:
	l	gp0,gp1
	bez	010D

l0109:
	sjs	gp15,010E
	aisp	gp1,#1
	br	0106

l010D:
	urs	gp15

;; fn010E: 010E
;;   Called from:
;;     0109 (in fn0105)
fn010E proc
	co	gp0
	urs	gp15

;;; ...end of image
