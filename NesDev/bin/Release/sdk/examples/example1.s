;
; File generated by cc65 v 2.13.3
;
	.fopt		compiler,"cc65 v 2.13.3"
	.setcpu		"6502"
	.smart		on
	.autoimport	on
	.case		on
	.debuginfo	off
	.importzp	sp, sreg, regsave, regbank, tmp1, ptr1, ptr2
	.macpack	longbranch
	.forceimport	__STARTUP__
	.import		_pal_col
	.import		_ppu_on_all
	.import		_ppu_system
	.import		_vram_adr
	.import		_vram_put
	.export		_put_str
	.export		_main

.segment	"RODATA"

L0001:
	.byte	$48,$45,$4C,$4C,$4F,$2C,$20,$57,$4F,$52,$4C,$44,$21,$00,$54,$48
	.byte	$49,$53,$20,$43,$4F,$44,$45,$20,$50,$52,$49,$4E,$54,$53,$20,$53
	.byte	$4F,$4D,$45,$20,$54,$45,$58,$54,$00,$55,$53,$49,$4E,$47,$20,$41
	.byte	$53,$43,$49,$49,$2D,$45,$4E,$43,$4F,$44,$45,$44,$20,$43,$48,$41
	.byte	$52,$53,$45,$54,$00,$28,$57,$49,$54,$48,$20,$43,$41,$50,$49,$54
	.byte	$41,$4C,$20,$4C,$45,$54,$54,$45,$52,$53,$20,$4F,$4E,$4C,$59,$29
	.byte	$00,$54,$4F,$20,$55,$53,$45,$20,$43,$48,$52,$20,$4D,$4F,$52,$45
	.byte	$20,$45,$46,$46,$49,$43,$49,$45,$4E,$54,$4C,$59,$00,$59,$4F,$55
	.byte	$27,$44,$20,$4E,$45,$45,$44,$20,$41,$20,$43,$55,$53,$54,$4F,$4D
	.byte	$20,$45,$4E,$43,$4F,$44,$49,$4E,$47,$00,$41,$4E,$44,$20,$41,$20
	.byte	$43,$4F,$4E,$56,$45,$52,$53,$49,$4F,$4E,$20,$54,$41,$42,$4C,$45
	.byte	$00,$43,$55,$52,$52,$45,$4E,$54,$20,$56,$49,$44,$45,$4F,$20,$4D
	.byte	$4F,$44,$45,$20,$49,$53,$00,$4E,$54,$53,$43,$00,$50,$41,$4C,$00

; ---------------------------------------------------------------
; void __near__ put_str (unsigned int, __near__ const unsigned char*)
; ---------------------------------------------------------------

.segment	"CODE"

.proc	_put_str: near

.segment	"CODE"

;
; vram_adr(adr);
;
	ldy     #$03
	lda     (sp),y
	tax
	dey
	lda     (sp),y
	jsr     _vram_adr
;
; if(!*str) break;
;
L0005:	ldy     #$01
	lda     (sp),y
	tax
	dey
	lda     (sp),y
	sta     ptr1
	stx     ptr1+1
	lda     (ptr1),y
	jeq     incsp4
;
; vram_put((*str++)-0x20);//-0x20 because ASCII code 0x20 is placed in tile 0 of the CHR
;
	iny
	lda     (sp),y
	tax
	dey
	lda     (sp),y
	sta     regsave
	stx     regsave+1
	clc
	adc     #$01
	bcc     L000D
	inx
L000D:	jsr     stax0sp
	ldy     #$00
	lda     (regsave),y
	sec
	sbc     #$20
	jsr     _vram_put
;
; }
;
	jmp     L0005

.endproc

; ---------------------------------------------------------------
; void __near__ main (void)
; ---------------------------------------------------------------

.segment	"CODE"

.proc	_main: near

.segment	"CODE"

;
; pal_col(1,0x30);//set while color
;
	lda     #$01
	jsr     pusha
	lda     #$30
	jsr     _pal_col
;
; put_str(NTADR_A(2,2),"HELLO, WORLD!");
;
	jsr     decsp4
	lda     #$42
	ldy     #$02
	sta     (sp),y
	iny
	lda     #$20
	sta     (sp),y
	lda     #<(L0001)
	ldy     #$00
	sta     (sp),y
	iny
	lda     #>(L0001)
	sta     (sp),y
	jsr     _put_str
;
; put_str(NTADR_A(2,6),"THIS CODE PRINTS SOME TEXT");
;
	jsr     decsp4
	lda     #$C2
	ldy     #$02
	sta     (sp),y
	iny
	lda     #$20
	sta     (sp),y
	lda     #<(L0001+14)
	ldy     #$00
	sta     (sp),y
	iny
	lda     #>(L0001+14)
	sta     (sp),y
	jsr     _put_str
;
; put_str(NTADR_A(2,7),"USING ASCII-ENCODED CHARSET");
;
	jsr     decsp4
	lda     #$E2
	ldy     #$02
	sta     (sp),y
	iny
	lda     #$20
	sta     (sp),y
	lda     #<(L0001+41)
	ldy     #$00
	sta     (sp),y
	iny
	lda     #>(L0001+41)
	sta     (sp),y
	jsr     _put_str
;
; put_str(NTADR_A(2,8),"(WITH CAPITAL LETTERS ONLY)");
;
	jsr     decsp4
	lda     #$02
	tay
	sta     (sp),y
	iny
	lda     #$21
	sta     (sp),y
	lda     #<(L0001+69)
	ldy     #$00
	sta     (sp),y
	iny
	lda     #>(L0001+69)
	sta     (sp),y
	jsr     _put_str
;
; put_str(NTADR_A(2,10),"TO USE CHR MORE EFFICIENTLY");
;
	jsr     decsp4
	lda     #$42
	ldy     #$02
	sta     (sp),y
	iny
	lda     #$21
	sta     (sp),y
	lda     #<(L0001+97)
	ldy     #$00
	sta     (sp),y
	iny
	lda     #>(L0001+97)
	sta     (sp),y
	jsr     _put_str
;
; put_str(NTADR_A(2,11),"YOU'D NEED A CUSTOM ENCODING");
;
	jsr     decsp4
	lda     #$62
	ldy     #$02
	sta     (sp),y
	iny
	lda     #$21
	sta     (sp),y
	lda     #<(L0001+125)
	ldy     #$00
	sta     (sp),y
	iny
	lda     #>(L0001+125)
	sta     (sp),y
	jsr     _put_str
;
; put_str(NTADR_A(2,12),"AND A CONVERSION TABLE");
;
	jsr     decsp4
	lda     #$82
	ldy     #$02
	sta     (sp),y
	iny
	lda     #$21
	sta     (sp),y
	lda     #<(L0001+154)
	ldy     #$00
	sta     (sp),y
	iny
	lda     #>(L0001+154)
	sta     (sp),y
	jsr     _put_str
;
; put_str(NTADR_A(2,16),"CURRENT VIDEO MODE IS");
;
	jsr     decsp4
	lda     #$02
	tay
	sta     (sp),y
	iny
	lda     #$22
	sta     (sp),y
	lda     #<(L0001+177)
	ldy     #$00
	sta     (sp),y
	iny
	lda     #>(L0001+177)
	sta     (sp),y
	jsr     _put_str
;
; if(ppu_system()) put_str(NTADR_A(24,16),"NTSC"); else put_str(NTADR_A(24,16),"PAL");
;
	jsr     _ppu_system
	tax
	beq     L0053
	jsr     decsp4
	lda     #$18
	ldy     #$02
	sta     (sp),y
	iny
	lda     #$22
	sta     (sp),y
	lda     #<(L0001+199)
	ldy     #$00
	sta     (sp),y
	iny
	lda     #>(L0001+199)
	jmp     L006B
L0053:	jsr     decsp4
	lda     #$18
	ldy     #$02
	sta     (sp),y
	iny
	lda     #$22
	sta     (sp),y
	lda     #<(L0001+204)
	ldy     #$00
	sta     (sp),y
	iny
	lda     #>(L0001+204)
L006B:	sta     (sp),y
	jsr     _put_str
;
; ppu_on_all();//enable rendering
;
	jsr     _ppu_on_all
;
; while(1);//do nothing, infinite loop
;
L0067:	jmp     L0067

.endproc

