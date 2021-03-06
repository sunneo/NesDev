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
	.import		_pal_bg
	.import		_pal_spr
	.import		_ppu_wait_nmi
	.import		_ppu_on_all
	.import		_oam_spr
	.import		_pad_poll
	.import		_scroll
	.import		_rand8
	.import		_set_vram_update
	.import		_flush_vram_update
	.import		_delay
	.export		_metatiles
	.export		_metaattrs
	.export		_palBackground
	.export		_palSprites
	.export		_level_data
	.export		_prepare_row_update
	.export		_preload_screen
	.export		_get_metatile
	.export		_balls_init
	.export		_balls_update
	.export		_main

.segment	"DATA"

.segment	"ZEROPAGE"

.segment	"RODATA"

_metatiles:
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$80
	.byte	$81
	.byte	$90
	.byte	$91
	.byte	$82
	.byte	$83
	.byte	$92
	.byte	$93
	.byte	$84
	.byte	$85
	.byte	$94
	.byte	$95
	.byte	$86
	.byte	$87
	.byte	$96
	.byte	$97
_metaattrs:
	.byte	$00
	.byte	$00
	.byte	$55
	.byte	$AA
	.byte	$FF
_palBackground:
	.byte	$0F
	.byte	$16
	.byte	$26
	.byte	$36
	.byte	$0F
	.byte	$18
	.byte	$28
	.byte	$38
	.byte	$0F
	.byte	$19
	.byte	$29
	.byte	$39
	.byte	$0F
	.byte	$1C
	.byte	$2C
	.byte	$3C
_palSprites:
	.byte	$0F
	.byte	$17
	.byte	$27
	.byte	$37
	.byte	$0F
	.byte	$11
	.byte	$21
	.byte	$31
	.byte	$0F
	.byte	$15
	.byte	$25
	.byte	$35
	.byte	$0F
	.byte	$19
	.byte	$29
	.byte	$39
_level_data:
	.byte	$03
	.byte	$04
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$03
	.byte	$04
	.byte	$01
	.byte	$02
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$02
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$02
	.byte	$02
	.byte	$02
	.byte	$02
	.byte	$02
	.byte	$02
	.byte	$02
	.byte	$02
	.byte	$02
	.byte	$02
	.byte	$02
	.byte	$02
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$03
	.byte	$03
	.byte	$03
	.byte	$03
	.byte	$03
	.byte	$03
	.byte	$03
	.byte	$03
	.byte	$03
	.byte	$03
	.byte	$03
	.byte	$03
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$04
	.byte	$04
	.byte	$04
	.byte	$04
	.byte	$04
	.byte	$04
	.byte	$04
	.byte	$04
	.byte	$04
	.byte	$04
	.byte	$04
	.byte	$04
	.byte	$00
	.byte	$00
	.byte	$03
	.byte	$04
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$03
	.byte	$04
	.byte	$01
	.byte	$02
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$02
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$02
	.byte	$02
	.byte	$02
	.byte	$02
	.byte	$02
	.byte	$02
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$02
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$02
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$02
	.byte	$00
	.byte	$03
	.byte	$03
	.byte	$00
	.byte	$02
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$02
	.byte	$00
	.byte	$03
	.byte	$03
	.byte	$00
	.byte	$02
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$02
	.byte	$00
	.byte	$04
	.byte	$04
	.byte	$00
	.byte	$02
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$02
	.byte	$00
	.byte	$04
	.byte	$04
	.byte	$00
	.byte	$02
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$02
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$02
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$02
	.byte	$02
	.byte	$02
	.byte	$02
	.byte	$02
	.byte	$02
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$03
	.byte	$04
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$03
	.byte	$04
	.byte	$01
	.byte	$02
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$02
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$02
	.byte	$02
	.byte	$02
	.byte	$02
	.byte	$02
	.byte	$02
	.byte	$02
	.byte	$02
	.byte	$02
	.byte	$02
	.byte	$02
	.byte	$02
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$03
	.byte	$03
	.byte	$03
	.byte	$03
	.byte	$03
	.byte	$03
	.byte	$03
	.byte	$03
	.byte	$03
	.byte	$03
	.byte	$03
	.byte	$03
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$04
	.byte	$04
	.byte	$04
	.byte	$04
	.byte	$04
	.byte	$04
	.byte	$04
	.byte	$04
	.byte	$04
	.byte	$04
	.byte	$04
	.byte	$04
	.byte	$00
	.byte	$00
	.byte	$03
	.byte	$04
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$03
	.byte	$04
	.byte	$01
	.byte	$02
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$02
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$01
	.byte	$02
	.byte	$02
	.byte	$02
	.byte	$03
	.byte	$03
	.byte	$03
	.byte	$04
	.byte	$04
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$02
	.byte	$00
	.byte	$02
	.byte	$03
	.byte	$00
	.byte	$00
	.byte	$04
	.byte	$00
	.byte	$04
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$01
	.byte	$02
	.byte	$00
	.byte	$02
	.byte	$03
	.byte	$00
	.byte	$00
	.byte	$04
	.byte	$00
	.byte	$04
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$01
	.byte	$02
	.byte	$02
	.byte	$00
	.byte	$03
	.byte	$00
	.byte	$00
	.byte	$04
	.byte	$00
	.byte	$04
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$01
	.byte	$02
	.byte	$00
	.byte	$02
	.byte	$03
	.byte	$00
	.byte	$00
	.byte	$04
	.byte	$00
	.byte	$04
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$02
	.byte	$02
	.byte	$02
	.byte	$03
	.byte	$03
	.byte	$03
	.byte	$04
	.byte	$04
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$01
	.byte	$02
	.byte	$02
	.byte	$02
	.byte	$03
	.byte	$03
	.byte	$03
	.byte	$04
	.byte	$04
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$02
	.byte	$00
	.byte	$02
	.byte	$03
	.byte	$00
	.byte	$00
	.byte	$04
	.byte	$00
	.byte	$04
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$01
	.byte	$02
	.byte	$00
	.byte	$02
	.byte	$03
	.byte	$00
	.byte	$00
	.byte	$04
	.byte	$00
	.byte	$04
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$01
	.byte	$02
	.byte	$02
	.byte	$00
	.byte	$03
	.byte	$00
	.byte	$00
	.byte	$04
	.byte	$00
	.byte	$04
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$01
	.byte	$02
	.byte	$00
	.byte	$02
	.byte	$03
	.byte	$00
	.byte	$00
	.byte	$04
	.byte	$00
	.byte	$04
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$02
	.byte	$02
	.byte	$02
	.byte	$03
	.byte	$03
	.byte	$03
	.byte	$04
	.byte	$04
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$01
	.byte	$02
	.byte	$02
	.byte	$02
	.byte	$03
	.byte	$03
	.byte	$03
	.byte	$04
	.byte	$04
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$02
	.byte	$00
	.byte	$02
	.byte	$03
	.byte	$00
	.byte	$00
	.byte	$04
	.byte	$00
	.byte	$04
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$01
	.byte	$02
	.byte	$00
	.byte	$02
	.byte	$03
	.byte	$00
	.byte	$00
	.byte	$04
	.byte	$00
	.byte	$04
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$01
	.byte	$02
	.byte	$02
	.byte	$00
	.byte	$03
	.byte	$00
	.byte	$00
	.byte	$04
	.byte	$00
	.byte	$04
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$01
	.byte	$02
	.byte	$00
	.byte	$02
	.byte	$03
	.byte	$00
	.byte	$00
	.byte	$04
	.byte	$00
	.byte	$04
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$02
	.byte	$02
	.byte	$02
	.byte	$03
	.byte	$03
	.byte	$03
	.byte	$04
	.byte	$04
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$01
	.byte	$02
	.byte	$02
	.byte	$02
	.byte	$03
	.byte	$03
	.byte	$03
	.byte	$04
	.byte	$04
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$02
	.byte	$00
	.byte	$02
	.byte	$03
	.byte	$00
	.byte	$00
	.byte	$04
	.byte	$00
	.byte	$04
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$01
	.byte	$02
	.byte	$00
	.byte	$02
	.byte	$03
	.byte	$00
	.byte	$00
	.byte	$04
	.byte	$00
	.byte	$04
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$01
	.byte	$02
	.byte	$02
	.byte	$00
	.byte	$03
	.byte	$00
	.byte	$00
	.byte	$04
	.byte	$00
	.byte	$04
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$01
	.byte	$02
	.byte	$00
	.byte	$02
	.byte	$03
	.byte	$00
	.byte	$00
	.byte	$04
	.byte	$00
	.byte	$04
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$02
	.byte	$02
	.byte	$02
	.byte	$03
	.byte	$03
	.byte	$03
	.byte	$04
	.byte	$04
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$01
	.byte	$02
	.byte	$02
	.byte	$02
	.byte	$03
	.byte	$03
	.byte	$03
	.byte	$04
	.byte	$04
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$02
	.byte	$00
	.byte	$02
	.byte	$03
	.byte	$00
	.byte	$00
	.byte	$04
	.byte	$00
	.byte	$04
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$01
	.byte	$02
	.byte	$00
	.byte	$02
	.byte	$03
	.byte	$00
	.byte	$00
	.byte	$04
	.byte	$00
	.byte	$04
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$01
	.byte	$02
	.byte	$02
	.byte	$00
	.byte	$03
	.byte	$00
	.byte	$00
	.byte	$04
	.byte	$00
	.byte	$04
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$01
	.byte	$02
	.byte	$00
	.byte	$02
	.byte	$03
	.byte	$00
	.byte	$00
	.byte	$04
	.byte	$00
	.byte	$04
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$02
	.byte	$02
	.byte	$02
	.byte	$03
	.byte	$03
	.byte	$03
	.byte	$04
	.byte	$04
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$03
	.byte	$00
	.byte	$03
	.byte	$00
	.byte	$03
	.byte	$00
	.byte	$03
	.byte	$00
	.byte	$03
	.byte	$00
	.byte	$03
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$03
	.byte	$00
	.byte	$03
	.byte	$04
	.byte	$03
	.byte	$00
	.byte	$03
	.byte	$00
	.byte	$03
	.byte	$04
	.byte	$03
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$02
	.byte	$04
	.byte	$02
	.byte	$04
	.byte	$02
	.byte	$00
	.byte	$02
	.byte	$04
	.byte	$02
	.byte	$04
	.byte	$02
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$02
	.byte	$04
	.byte	$02
	.byte	$00
	.byte	$02
	.byte	$04
	.byte	$02
	.byte	$04
	.byte	$02
	.byte	$00
	.byte	$02
	.byte	$04
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$04
	.byte	$01
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$01
	.byte	$04
	.byte	$01
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$03
	.byte	$04
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$03
	.byte	$04
	.byte	$01
	.byte	$02
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$02
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$00
	.byte	$02
	.byte	$02
	.byte	$02
	.byte	$00
	.byte	$03
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$00
	.byte	$02
	.byte	$00
	.byte	$02
	.byte	$00
	.byte	$03
	.byte	$03
	.byte	$03
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$00
	.byte	$02
	.byte	$00
	.byte	$02
	.byte	$00
	.byte	$03
	.byte	$00
	.byte	$03
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$00
	.byte	$00
	.byte	$02
	.byte	$00
	.byte	$02
	.byte	$00
	.byte	$03
	.byte	$00
	.byte	$03
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$01
	.byte	$01
	.byte	$00
	.byte	$02
	.byte	$02
	.byte	$02
	.byte	$00
	.byte	$03
	.byte	$03
	.byte	$03
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$03
	.byte	$04
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$03
	.byte	$04
	.byte	$01
	.byte	$02
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$00
	.byte	$01
	.byte	$02

.segment	"BSS"

_update_list:
	.res	47,$00
_ball_x:
	.res	63,$00
_ball_y:
	.res	63,$00
_ball_dx:
	.res	63,$00
_ball_dy:
	.res	63,$00
.segment	"ZEROPAGE"
_col:
	.res	1,$00
_tile:
	.res	1,$00
_attr:
	.res	1,$00
_tile_off:
	.res	1,$00
_updn_off:
	.res	1,$00
_upda_off:
	.res	1,$00
_mask1:
	.res	1,$00
_mask2:
	.res	1,$00
_mask3:
	.res	1,$00
_mask4:
	.res	1,$00
_i:
	.res	1,$00
_j:
	.res	1,$00
_spr:
	.res	1,$00
_name_adr:
	.res	2,$00
_attr_adr:
	.res	2,$00
_src:
	.res	2,$00

; ---------------------------------------------------------------
; void __near__ prepare_row_update (unsigned char, unsigned int)
; ---------------------------------------------------------------

.segment	"CODE"

.proc	_prepare_row_update: near

.segment	"CODE"

	ldy     #$02
	ldx     #$00
	lda     (sp),y
	cmp     #$1E
	jsr     boolult
	jeq     L0543
	ldy     #$02
	ldx     #$00
	lda     (sp),y
	jsr     aslax4
	jsr     aslax1
	jsr     pushax
	ldx     #$20
	lda     #$00
	jsr     tosaddax
	sta     _name_adr
	stx     _name_adr+1
	ldy     #$02
	ldx     #$00
	lda     (sp),y
	jsr     asrax2
	jsr     aslax3
	jsr     pushax
	ldx     #$23
	lda     #$C0
	jsr     tosaddax
	sta     _attr_adr
	stx     _attr_adr+1
	jmp     L054C
L0543:	ldy     #$02
	ldx     #$00
	lda     (sp),y
	sec
	sbc     #$1E
	sta     (sp),y
	ldy     #$02
	ldx     #$00
	lda     (sp),y
	jsr     aslax4
	jsr     aslax1
	jsr     pushax
	ldx     #$28
	lda     #$00
	jsr     tosaddax
	sta     _name_adr
	stx     _name_adr+1
	ldy     #$02
	ldx     #$00
	lda     (sp),y
	jsr     asrax2
	jsr     aslax3
	jsr     pushax
	ldx     #$2B
	lda     #$C0
	jsr     tosaddax
	sta     _attr_adr
	stx     _attr_adr+1
L054C:	ldx     #$00
	lda     _name_adr+1
	ora     #$40
	sta     _update_list
	lda     _name_adr
	ldx     _name_adr+1
	ldx     #$00
	sta     _update_list+1
	ldx     #$00
	lda     _attr_adr+1
	ora     #$40
	sta     _update_list+35
	lda     _attr_adr
	ldx     _attr_adr+1
	ldx     #$00
	sta     _update_list+36
	ldx     #$00
	lda     #$03
	sta     _updn_off
	ldx     #$00
	lda     #$26
	sta     _upda_off
	ldy     #$01
	jsr     ldaxysp
	jsr     shlax4
	clc
	adc     #<(_level_data)
	tay
	txa
	adc     #>(_level_data)
	tax
	tya
	sta     _src
	stx     _src+1
	ldy     #$02
	ldx     #$00
	lda     (sp),y
	ldx     #$00
	and     #$01
	jsr     bnegax
	jeq     L0575
	ldx     #$00
	lda     #$00
	sta     _tile_off
	jmp     L057A
L0575:	ldx     #$00
	lda     #$02
	sta     _tile_off
L057A:	ldy     #$02
	ldx     #$00
	lda     (sp),y
	ldx     #$00
	and     #$02
	jsr     bnegax
	jeq     L057D
	ldx     #$00
	lda     #$FC
	sta     _mask1
	ldx     #$00
	lda     #$03
	sta     _mask2
	ldx     #$00
	lda     #$F3
	sta     _mask3
	ldx     #$00
	lda     #$0C
	sta     _mask4
	jmp     L0588
L057D:	ldx     #$00
	lda     #$CF
	sta     _mask1
	ldx     #$00
	lda     #$30
	sta     _mask2
	ldx     #$00
	lda     #$3F
	sta     _mask3
	ldx     #$00
	lda     #$C0
	sta     _mask4
L0588:	ldx     #$00
	lda     #$00
	sta     _col
L0591:	ldx     #$00
	lda     _col
	cmp     #$08
	jsr     boolult
	jne     L0594
	jmp     L0592
L0594:	lda     _src
	ldx     _src+1
	sta     regsave
	stx     regsave+1
	jsr     incax1
	sta     _src
	stx     _src+1
	lda     regsave
	ldx     regsave+1
	ldy     #$00
	jsr     ldauidx
	sta     _tile
	lda     #<(_update_list)
	ldx     #>(_update_list)
	clc
	adc     _upda_off
	bcc     L059F
	inx
L059F:	ldy     #$00
	jsr     ldauidx
	jsr     pushax
	ldx     #$00
	lda     _mask1
	jsr     tosandax
	jsr     pushax
	lda     #<(_metaattrs)
	ldx     #>(_metaattrs)
	clc
	adc     _tile
	bcc     L05A2
	inx
L05A2:	ldy     #$00
	jsr     ldauidx
	jsr     pushax
	ldx     #$00
	lda     _mask2
	jsr     tosandax
	jsr     tosorax
	sta     _attr
	ldx     #$00
	lda     _tile
	jsr     aslax2
	jsr     pushax
	ldx     #$00
	lda     _tile_off
	jsr     tosaddax
	sta     _tile
	ldx     #$00
	lda     _updn_off
	clc
	adc     #<(_update_list)
	tay
	txa
	adc     #>(_update_list)
	tax
	tya
	jsr     pushax
	ldx     #$00
	lda     _tile
	clc
	adc     #<(_metatiles)
	tay
	txa
	adc     #>(_metatiles)
	tax
	tya
	ldy     #$00
	jsr     ldauidx
	ldy     #$00
	jsr     staspidx
	ldx     #$00
	lda     _updn_off
	jsr     incax1
	clc
	adc     #<(_update_list)
	tay
	txa
	adc     #>(_update_list)
	tax
	tya
	jsr     pushax
	ldx     #$00
	lda     _tile
	jsr     incax1
	clc
	adc     #<(_metatiles)
	tay
	txa
	adc     #>(_metatiles)
	tax
	tya
	ldy     #$00
	jsr     ldauidx
	ldy     #$00
	jsr     staspidx
	lda     _src
	ldx     _src+1
	sta     regsave
	stx     regsave+1
	jsr     incax1
	sta     _src
	stx     _src+1
	lda     regsave
	ldx     regsave+1
	ldy     #$00
	jsr     ldauidx
	sta     _tile
	ldx     #$00
	lda     _upda_off
	pha
	clc
	adc     #$01
	sta     _upda_off
	pla
	clc
	adc     #<(_update_list)
	tay
	txa
	adc     #>(_update_list)
	tax
	tya
	jsr     pushax
	ldx     #$00
	lda     _attr
	jsr     pushax
	ldx     #$00
	lda     _mask3
	jsr     tosandax
	jsr     pushax
	lda     #<(_metaattrs)
	ldx     #>(_metaattrs)
	clc
	adc     _tile
	bcc     L05B6
	inx
L05B6:	ldy     #$00
	jsr     ldauidx
	jsr     pushax
	ldx     #$00
	lda     _mask4
	jsr     tosandax
	jsr     tosorax
	ldy     #$00
	jsr     staspidx
	ldx     #$00
	lda     _tile
	jsr     aslax2
	jsr     pushax
	ldx     #$00
	lda     _tile_off
	jsr     tosaddax
	sta     _tile
	ldx     #$00
	lda     _updn_off
	jsr     incax2
	clc
	adc     #<(_update_list)
	tay
	txa
	adc     #>(_update_list)
	tax
	tya
	jsr     pushax
	ldx     #$00
	lda     _tile
	clc
	adc     #<(_metatiles)
	tay
	txa
	adc     #>(_metatiles)
	tax
	tya
	ldy     #$00
	jsr     ldauidx
	ldy     #$00
	jsr     staspidx
	ldx     #$00
	lda     _updn_off
	jsr     incax3
	clc
	adc     #<(_update_list)
	tay
	txa
	adc     #>(_update_list)
	tax
	tya
	jsr     pushax
	ldx     #$00
	lda     _tile
	jsr     incax1
	clc
	adc     #<(_metatiles)
	tay
	txa
	adc     #>(_metatiles)
	tax
	tya
	ldy     #$00
	jsr     ldauidx
	ldy     #$00
	jsr     staspidx
	ldx     #$00
	lda     #$04
	clc
	adc     _updn_off
	sta     _updn_off
	ldx     #$00
	inc     _col
	lda     _col
	jmp     L0591
L0592:	jsr     incsp3
	rts

.endproc

; ---------------------------------------------------------------
; void __near__ preload_screen (unsigned int)
; ---------------------------------------------------------------

.segment	"CODE"

.proc	_preload_screen: near

.segment	"ZEROPAGE"

L05C5:
	.res	1,$00

.segment	"CODE"

	ldx     #$00
	lda     #$00
	sta     L05C5
L05C6:	ldx     #$00
	lda     L05C5
	cmp     #$1E
	jsr     boolult
	jne     L05C9
	jmp     L05C7
L05C9:	ldx     #$00
	lda     #$1D
	jsr     pushax
	ldx     #$00
	lda     L05C5
	jsr     tossubax
	jsr     pusha
	ldy     #$02
	jsr     ldaxysp
	jsr     shrax4
	jsr     pushax
	jsr     _prepare_row_update
	lda     #<(_update_list)
	ldx     #>(_update_list)
	jsr     _flush_vram_update
	ldy     #$00
	ldx     #$00
	lda     #$08
	jsr     addeqysp
	ldx     #$00
	inc     L05C5
	lda     L05C5
	jmp     L05C6
L05C7:	jsr     incsp2
	rts

.endproc

; ---------------------------------------------------------------
; unsigned char __near__ get_metatile (int, unsigned int, unsigned int)
; ---------------------------------------------------------------

.segment	"CODE"

.proc	_get_metatile: near

.segment	"CODE"

	ldy     #$05
	jsr     ldaxysp
	ldy     #$F0
	jsr     decaxy
	jsr     pushax
	ldx     #$00
	lda     #$F0
	jsr     pushax
	ldy     #$05
	jsr     ldaxysp
	jsr     tossubax
	jsr     tosaddax
	ldy     #$04
	jsr     staxysp
	ldy     #$05
	jsr     ldaxysp
	cpx     #$80
	lda     #$00
	ldx     #$00
	rol     a
	jeq     L05D9
	ldy     #$04
	ldx     #$05
	lda     #$00
	jsr     addeqysp
L05D9:	ldy     #$05
	jsr     ldaxysp
	jsr     asrax4
	jsr     aslax4
	jsr     pushax
	ldy     #$05
	jsr     ldaxysp
	jsr     shrax4
	jsr     tosorax
	clc
	adc     #<(_level_data)
	tay
	txa
	adc     #>(_level_data)
	tax
	tya
	ldy     #$00
	jsr     ldauidx
	jmp     L05D5
L05D5:	jsr     incsp6
	rts

.endproc

; ---------------------------------------------------------------
; void __near__ balls_init (void)
; ---------------------------------------------------------------

.segment	"CODE"

.proc	_balls_init: near

.segment	"CODE"

	ldx     #$00
	lda     #$00
	sta     _i
L05E5:	ldx     #$00
	lda     _i
	cmp     #$3F
	jsr     boolult
	jne     L05E8
	jmp     L05E6
L05E8:	lda     #<(_ball_x)
	ldx     #>(_ball_x)
	clc
	adc     _i
	bcc     L05EF
	inx
L05EF:	jsr     pushax
	jsr     _rand8
	ldy     #$00
	jsr     staspidx
	lda     #<(_ball_y)
	ldx     #>(_ball_y)
	clc
	adc     _i
	bcc     L05F3
	inx
L05F3:	jsr     pushax
	jsr     _rand8
	ldy     #$00
	jsr     staspidx
	jsr     _rand8
	sta     _j
	jsr     _rand8
	jsr     pushax
	ldx     #$00
	lda     #$03
	jsr     tosumodax
	jsr     incax1
	sta     _spr
	lda     #<(_ball_dx)
	ldx     #>(_ball_dx)
	clc
	adc     _i
	bcc     L05FC
	inx
L05FC:	jsr     pushax
	ldx     #$00
	lda     _j
	ldx     #$00
	and     #$01
	stx     tmp1
	ora     tmp1
	jeq     L05FE
	ldx     #$00
	lda     _spr
	jsr     negax
	ldx     #$00
	jmp     L0600
L05FE:	ldx     #$00
	lda     _spr
	ldx     #$00
L0600:	ldy     #$00
	jsr     staspidx
	jsr     _rand8
	jsr     pushax
	ldx     #$00
	lda     #$03
	jsr     tosumodax
	jsr     incax1
	sta     _spr
	lda     #<(_ball_dy)
	ldx     #>(_ball_dy)
	clc
	adc     _i
	bcc     L0607
	inx
L0607:	jsr     pushax
	ldx     #$00
	lda     _j
	ldx     #$00
	and     #$02
	stx     tmp1
	ora     tmp1
	jeq     L0609
	ldx     #$00
	lda     _spr
	jsr     negax
	ldx     #$00
	jmp     L060B
L0609:	ldx     #$00
	lda     _spr
	ldx     #$00
L060B:	ldy     #$00
	jsr     staspidx
	ldx     #$00
	inc     _i
	lda     _i
	jmp     L05E5
L05E6:	rts

.endproc

; ---------------------------------------------------------------
; void __near__ balls_update (void)
; ---------------------------------------------------------------

.segment	"CODE"

.proc	_balls_update: near

.segment	"CODE"

	ldx     #$00
	lda     #$00
	sta     _spr
	ldx     #$00
	lda     #$00
	sta     _i
L0610:	ldx     #$00
	lda     _i
	cmp     #$3F
	jsr     boolult
	jne     L0613
	jmp     L0611
L0613:	lda     #<(_ball_x)
	ldx     #>(_ball_x)
	clc
	adc     _i
	bcc     L061C
	inx
L061C:	ldy     #$00
	jsr     ldauidx
	jsr     pusha
	lda     #<(_ball_y)
	ldx     #>(_ball_y)
	clc
	adc     _i
	bcc     L061F
	inx
L061F:	ldy     #$00
	jsr     ldauidx
	jsr     pusha
	lda     #$40
	jsr     pusha
	ldx     #$00
	lda     _i
	ldx     #$00
	and     #$03
	jsr     pusha
	lda     _spr
	jsr     _oam_spr
	sta     _spr
	lda     #<(_ball_x)
	ldx     #>(_ball_x)
	clc
	adc     _i
	bcc     L0625
	inx
L0625:	jsr     pushax
	ldy     #$00
	jsr     ldauidx
	jsr     pushax
	lda     #<(_ball_dx)
	ldx     #>(_ball_dx)
	clc
	adc     _i
	bcc     L0628
	inx
L0628:	ldy     #$00
	jsr     ldauidx
	jsr     tosaddax
	ldy     #$00
	jsr     staspidx
	lda     #<(_ball_y)
	ldx     #>(_ball_y)
	clc
	adc     _i
	bcc     L062B
	inx
L062B:	jsr     pushax
	ldy     #$00
	jsr     ldauidx
	jsr     pushax
	lda     #<(_ball_dy)
	ldx     #>(_ball_dy)
	clc
	adc     _i
	bcc     L062E
	inx
L062E:	ldy     #$00
	jsr     ldauidx
	jsr     tosaddax
	ldy     #$00
	jsr     staspidx
	lda     #<(_ball_x)
	ldx     #>(_ball_x)
	clc
	adc     _i
	bcc     L0632
	inx
L0632:	ldy     #$00
	jsr     ldauidx
	cmp     #$F8
	lda     #$00
	ldx     #$00
	rol     a
	jeq     L062F
	lda     #<(_ball_dx)
	ldx     #>(_ball_dx)
	clc
	adc     _i
	bcc     L0636
	inx
L0636:	jsr     pushax
	lda     #<(_ball_dx)
	ldx     #>(_ball_dx)
	clc
	adc     _i
	bcc     L0639
	inx
L0639:	ldy     #$00
	jsr     ldauidx
	jsr     negax
	ldy     #$00
	jsr     staspidx
L062F:	lda     #<(_ball_y)
	ldx     #>(_ball_y)
	clc
	adc     _i
	bcc     L063D
	inx
L063D:	ldy     #$00
	jsr     ldauidx
	cmp     #$E8
	lda     #$00
	ldx     #$00
	rol     a
	jeq     L0612
	lda     #<(_ball_dy)
	ldx     #>(_ball_dy)
	clc
	adc     _i
	bcc     L0641
	inx
L0641:	jsr     pushax
	lda     #<(_ball_dy)
	ldx     #>(_ball_dy)
	clc
	adc     _i
	bcc     L0644
	inx
L0644:	ldy     #$00
	jsr     ldauidx
	jsr     negax
	ldy     #$00
	jsr     staspidx
L0612:	ldx     #$00
	inc     _i
	lda     _i
	jmp     L0610
L0611:	rts

.endproc

; ---------------------------------------------------------------
; void __near__ main (void)
; ---------------------------------------------------------------

.segment	"CODE"

.proc	_main: near

.segment	"ZEROPAGE"

L0646:
	.res	1,$00
L0647:
	.res	1,$00
L0648:
	.res	1,$00
L0649:
	.res	1,$00
L064A:
	.res	1,$00
L064B:
	.res	2,$00
L064C:
	.res	2,$00

.segment	"CODE"

	lda     #<(_palBackground)
	ldx     #>(_palBackground)
	jsr     _pal_bg
	lda     #<(_palSprites)
	ldx     #>(_palSprites)
	jsr     _pal_spr
	ldx     #$00
	lda     #$60
	sta     _update_list
	ldx     #$00
	lda     #$00
	sta     _update_list+1
	ldx     #$00
	lda     #$20
	sta     _update_list+2
	ldx     #$00
	lda     #$60
	sta     _update_list+35
	ldx     #$00
	lda     #$00
	sta     _update_list+36
	ldx     #$00
	lda     #$08
	sta     _update_list+37
	ldx     #$00
	lda     #$FF
	sta     _update_list+46
	ldx     #$00
	lda     #$00
	jsr     pushax
	jsr     _preload_screen
	ldx     #$00
	lda     #$F0
	sta     L064B
	stx     L064B+1
	ldx     #$00
	lda     #$00
	sta     L064C
	stx     L064C+1
	ldx     #$00
	lda     #$80
	sta     L0649
	ldx     #$00
	lda     #$78
	sta     L064A
	jsr     _balls_init
	jsr     _ppu_on_all
	lda     #$3C
	jsr     _delay
L0674:	jsr     _ppu_wait_nmi
	ldx     #$00
	lda     #$00
	sta     $401E
	ldx     #$00
	lda     #$00
	jsr     _set_vram_update
	lda     L064B
	ldx     L064B+1
	ldx     #$00
	and     #$07
	jsr     bnegax
	jeq     L067C
	lda     L064C
	ldx     L064C+1
	jsr     asrax3
	ldy     #$3B
	jsr     incaxy
	sta     L0648
	ldx     #$00
	lda     L0648
	cmp     #$3C
	lda     #$00
	ldx     #$00
	rol     a
	jeq     L0682
	ldx     #$00
	lda     L0648
	sec
	sbc     #$3C
	sta     L0648
L0682:	lda     L0648
	jsr     pusha
	lda     L064B
	ldx     L064B+1
	jsr     shrax4
	jsr     pushax
	jsr     _prepare_row_update
	lda     #<(_update_list)
	ldx     #>(_update_list)
	jsr     _set_vram_update
L067C:	ldx     #$00
	lda     #$00
	jsr     pushax
	lda     L064C
	ldx     L064C+1
	jsr     _scroll
	inc     L064B
	bne     L068F
	inc     L064B+1
L068F:	lda     L064B
	ldx     L064B+1
	lda     L064B
	ldx     L064B+1
	cmp     #$00
	txa
	sbc     #$05
	lda     #$00
	ldx     #$00
	rol     a
	jeq     L0690
	ldx     #$00
	lda     #$00
	sta     L064B
	stx     L064B+1
L0690:	lda     L064C
	sec
	sbc     #$01
	sta     L064C
	bcs     L0697
	dec     L064C+1
L0697:	ldx     L064C+1
	lda     L064C
	ldx     L064C+1
	cpx     #$80
	lda     #$00
	ldx     #$00
	rol     a
	jeq     L0698
	ldx     #$01
	lda     #$DF
	sta     L064C
	stx     L064C+1
L0698:	lda     L064B
	ldx     L064B+1
	jsr     pushax
	ldx     #$00
	lda     L0649
	ldx     #$00
	jsr     pushax
	ldx     #$00
	lda     L064A
	ldx     #$00
	jsr     pushax
	jsr     _get_metatile
	sta     L0647
	ldx     #$00
	lda     L0647
	jsr     bnega
	jeq     L06A1
	ldx     #$00
	lda     #$1F
	sta     L0647
	jmp     L06A5
L06A1:	ldx     #$00
	lda     #$20
	clc
	adc     L0647
	sta     L0647
L06A5:	ldx     #$00
	lda     L0649
	jsr     decax4
	jsr     pusha
	ldx     #$00
	lda     L064A
	jsr     decax4
	jsr     pusha
	lda     L0647
	jsr     pusha
	lda     #$00
	jsr     pusha
	lda     #$FC
	jsr     _oam_spr
	lda     #$00
	jsr     _pad_poll
	sta     L0646
	ldx     #$00
	lda     L0646
	ldx     #$00
	and     #$40
	stx     tmp1
	ora     tmp1
	jeq     L06B3
	ldx     #$00
	lda     L0649
	cmp     #$05
	lda     #$00
	ldx     #$00
	rol     a
	jeq     L06B3
	ldx     #$00
	lda     L0649
	sec
	sbc     #$02
	sta     L0649
L06B3:	ldx     #$00
	lda     L0646
	ldx     #$00
	and     #$80
	stx     tmp1
	ora     tmp1
	jeq     L06B9
	ldx     #$00
	lda     L0649
	cmp     #$FC
	jsr     boolult
	jeq     L06B9
	ldx     #$00
	lda     #$02
	clc
	adc     L0649
	sta     L0649
L06B9:	ldx     #$00
	lda     L0646
	ldx     #$00
	and     #$10
	stx     tmp1
	ora     tmp1
	jeq     L06BF
	ldx     #$00
	lda     L064A
	cmp     #$05
	lda     #$00
	ldx     #$00
	rol     a
	jeq     L06BF
	ldx     #$00
	lda     L064A
	sec
	sbc     #$02
	sta     L064A
L06BF:	ldx     #$00
	lda     L0646
	ldx     #$00
	and     #$20
	stx     tmp1
	ora     tmp1
	jeq     L06C5
	ldx     #$00
	lda     L064A
	cmp     #$EC
	jsr     boolult
	jeq     L06C5
	ldx     #$00
	lda     #$02
	clc
	adc     L064A
	sta     L064A
L06C5:	jsr     _balls_update
	ldx     #$00
	lda     #$00
	sta     $401F
	jmp     L0674
	rts

.endproc

