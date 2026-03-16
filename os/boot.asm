[BITS 16]	; 16-bit code
[ORG 0]		; Start at 0000:0000

jmp	entry	; Skip data section and jump to entry point

	cyls_read			dw 10	; Number of cylinders to read

	; User-facing messages
	msg_loading 		dw "Loading", 0
	msg_loading_proc 	db ".", 0

entry:
	cli				; Disable BIOS interrupts
	mov		ax, 0x07c0		; Address where BIOS loaded us
	mov		ds, ax
	mov 	ax, 0x9000		; Segment address
	mov 	es, ax
	xor 	si, si		; Copy from offset zero
	xor 	di, di
	sti				; Enable BIOS interrupts

	mov 	cx, 128		; Copy 128 double words (128 * 4 bytes) to 9000:0000
	rep 	movsd

	jmp 	0x9000:start	; Jump to the relocated copy

start:
	mov	si, msg_loading	; Print loading message
	call	print

	mov 	ax, cs		; Update segment registers
	mov 	ds, ax
	mov	ss, ax

	mov 	di, 1			; Start copying from floppy into 0290:0000, cylinder 1
	mov 	ax, 0x290
	xor 	bx, bx

.loop:
	mov		si, msg_loading_proc	; Print progress dot
	call	print

	mov 	cx, 0x50
	mov 	es, cx

	push 	di

	shr 	di, 1
	setc 	dh
	mov 	cx, di
	xchg 	cl, ch

	pop		di
	cmp 	di, cyls_read		; Have all cylinders been read?
	je 		.quit

	call 	read_cylinder		; Read next cylinder into 0050:0000 - 0050:2400

	pusha
	push 	ds

	mov 	cx, 0x50			; Copy this block further into 0290:0000
	mov 	ds, cx
	mov 	es, ax
	xor 	di, di
	xor 	si, si
	mov 	cx, 0x2400
	rep 	movsb

	pop 	ds
	popa

	inc 	di				; Increment cylinder index
	add 	ax, 0x240
	jmp 	short .loop			; Continue reading

.quit:
	mov 	ax, 0x50			; All cylinders copied, now read cylinder 0
	mov 	es, ax
	mov 	bx, 0
	mov 	ch, 0
	mov 	dh, 0
	call 	read_cylinder

	jmp 	0x0000:0x0700		; Jump to the second-stage bootloader
   call .quit
read_cylinder:				; Read a cylinder using BIOS interrupt 0x13
	pusha
	mov 	ah, 0x02
	mov 	al, 18
	mov 	cl, 1
	int 	0x13
	popa
	ret


print:					; Print null-terminated string at si
	pusha
.loop:
	lodsb
	or 		al, al
	jz 		.quit
	mov 	ah, 0x0e
	mov 	bx, 0x7
	int 	0x10
	jmp 	.loop
.quit:
	popa
	ret


TIMES 510 - ($-$$) db 0			; Pad with zeros up to byte 510
dw 0xaa55					; Boot sector signature

incbin   'setup.o'
