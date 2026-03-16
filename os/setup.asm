%define RM_DATA_SELECTOR (1 << 3)
%define RM_CODE_SELECTOR (2 << 3)

[BITS 16]
[ORG 0x0700]		; The first-stage bootloader jumps here

	cli			; Initialize segment registers
	mov 	ax, 0
	mov 	ds, ax
	mov 	es, ax
	mov 	ss, ax
	mov 	sp, 0x700
	sti

	; Set the base interrupt vector of the PIC to 0x20
	mov 	al, 00010001b
	out 	0x20, al
	mov 	al, 0x20
	out 	0x21, al
	mov 	al, 00000100b
	out 	0x21, al
	mov 	al, 00000001b
	out 	0x21, al

	cli

	; Load the GDTR register
	xor	ax, ax
	mov	ds, ax

	lgdt 	[gd_reg]

	; Set the PE bit in CR0
	mov 	eax, cr0
	or 	al, 1
	mov 	cr0, eax

	; Jump to protected mode (32-bit)
	jmp 	0x08:protected


[BITS 32]
protected:
	mov 	ax, 0x10		; Initialize segment registers
	mov 	ds, ax
	mov 	es, ax
	mov 	ss, ax
	mov		fs, ax
	mov		gs, ax

	call print
	; Copy the kernel to address 0x200000
	mov 	esi, kernel
	mov 	edi, 0x200000
	mov 	ecx, (kernel_size / 4) + 1 ; Kernel size in dwords
	rep 	movsd


	jmp	0x200000 ; Transfer control to the kernel

	; Leaving PM
  call pm_exiting

pm_exiting:

    mov     eax, cr0
    btc     eax, 0
    mov     cr0, eax


    jmp     0:exit
  ret
[BITS 16]
exit:
    ;; REAL MODE --------------------
    mov     ax, 0
    mov     ds, ax
    sti
    jmp $

[BITS 32]
print:


	mov     esi, message
    mov     edi, 0xb8140
	.lpmsg:
        lodsb
        stosb
        inc     edi
        test    al, al
        jnz .lpmsg
  ret



	message:
    dw "Protected Mode.", 0



gdt:
gdt_null:
	dd	0, 0		; Null descriptor
gdt_code:
	dw 	0xFFFF	; Code segment
	dw 	0
	db 	0
	db 	10011010b
	db 	11001111b
	db 	0
gdt_data:
	dw 	0xFFFF	; Data segment
	dw 	0
	db 	0,
	db 	10010010b
	db 	11001111b
	db 	0
gdt_end:
;gdt_null:
	dd	0, 0		; Null descriptor
;gdt_code:
	db 0xff, 0xff, 0, 0, 0, 0x92, 0x8f, 0 ; PM DATA
;gdt_data:
	db 0xff, 0xff, 0, 0, 0, 0x9a, 0xcf, 0 ; PM CODE
	db 0xff, 0xff, 0, 0, 0, 0x92, 0x0f, 0 ; RM DATA
    db 0xff, 0xff, 0, 0, 0, 0x9a, 0x0f, 0 ; RM CODE


; Value to load into GDTR
gd_reg:
	dw 	gdt_end - gdt - 1
	dd 	gdt

kernel:
	incbin 'kernel.bin'
kernel_size equ   $-kernel

; Uncomment to pad to a full floppy image
; Floppy size = 1474560
;TIMES 1474560 - 512 - ($-$$) db 0
