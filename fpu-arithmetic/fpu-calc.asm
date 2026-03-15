; x87 FPU Arithmetic Demo
; Computes (a + b) * c + d using both FPU (float) and integer ALU,
; then prints the results with a hand-rolled float-to-string routine.
;
; Expression: (-7 + 3) * 2 + 3 = -5
; Float:      (-7.005 + 3.005) * 2.005 + 3.005 = -5.015
;
; Build (Linux 32-bit):
;   nasm -f elf32 -o fpu-calc.o fpu-calc.asm
;   ld -m elf_i386 -o fpu-calc fpu-calc.o
;
; Run:
;   ./fpu-calc                    (on 32-bit Linux)
;   qemu-i386 ./fpu-calc          (on 64-bit Linux)
;   docker run --rm -v $PWD:/w -w /w i386/alpine sh -c \
;     "apk add nasm binutils && nasm -f elf32 -o fpu-calc.o fpu-calc.asm && ld -m elf_i386 -o fpu-calc fpu-calc.o && ./fpu-calc"

section .data
    msg_float:  db "Float:   (", 0
    msg_int:    db 10, "Integer: (", 0
    msg_plus:   db " + ", 0
    msg_times:  db ") * ", 0
    msg_eq:     db " = ", 0
    msg_nl:     db 10, 0

    ; Integer operands
    num_a:  dd -7
    num_b:  dd  3
    num_c:  dd  2
    num_d:  dd  3

    ; Float operands
    fl_a:   dd -7.005
    fl_b:   dd  3.005
    fl_c:   dd  2.005
    fl_d:   dd  3.005

    ten:    dd 10
    minus:  db '-'

section .bss
    fres:           resd 1
    int_part:       resd 1
    frac_digits:    resb 16
    temp:           resd 1
    saved_cw:       resw 1
    trunc_cw:       resw 1
    int_buf:        resb 16

section .text
global _start

_start:
    ; ── Float calculation: (a + b) * c + d ──────────────

    call print_float_expr

    finit
    fld     dword [fl_a]        ; st0 = -7.005
    fadd    dword [fl_b]        ; st0 = -7.005 + 3.005 = -4.0
    fmul    dword [fl_c]        ; st0 = -4.0 * 2.005 = -8.02
    fadd    dword [fl_d]        ; st0 = -8.02 + 3.005 = -5.015

    ; Print the float result
    mov     esi, msg_eq
    call    print_str
    call    print_float         ; print st0, consumes it

    ; ── Integer calculation: (a + b) * c + d ────────────

    mov     esi, msg_int
    call    print_str

    ; Show expression
    mov     eax, [num_a]
    call    print_int
    mov     esi, msg_plus
    call    print_str
    mov     eax, [num_b]
    call    print_int
    mov     esi, msg_times
    call    print_str
    mov     eax, [num_c]
    call    print_int
    mov     esi, msg_plus
    call    print_str
    mov     eax, [num_d]
    call    print_int
    mov     esi, msg_eq
    call    print_str

    ; Compute
    mov     eax, [num_a]
    add     eax, [num_b]        ; eax = -7 + 3 = -4
    imul    dword [num_c]       ; eax = -4 * 2 = -8
    add     eax, [num_d]        ; eax = -8 + 3 = -5
    call    print_int

    mov     esi, msg_nl
    call    print_str

    ; Exit
    mov     eax, 1              ; sys_exit
    xor     ebx, ebx
    int     0x80


; ── print_float_expr: print "Float:   (a + b) * c + d" ──
print_float_expr:
    mov     esi, msg_float
    call    print_str
    ; Print float values inline (simplified: just show the expression symbolically)
    ; -7.005 + 3.005) * 2.005 + 3.005
    finit
    fld     dword [fl_a]
    call    print_float
    mov     esi, msg_plus
    call    print_str
    fld     dword [fl_b]
    call    print_float
    mov     esi, msg_times
    call    print_str
    fld     dword [fl_c]
    call    print_float
    mov     esi, msg_plus
    call    print_str
    fld     dword [fl_d]
    call    print_float
    ret


; ── print_float: print st0 as decimal, pops st0 ─────────
; Handles negative numbers. Prints up to 6 fractional digits.
print_float:
    ; Check sign
    ftst
    fstsw   ax
    sahf
    jnc     .positive
    ; Negative: print '-' and negate
    push    eax
    mov     eax, 4
    mov     ebx, 1
    mov     ecx, minus
    mov     edx, 1
    int     0x80
    pop     eax
    fchs                        ; st0 = |st0|
.positive:
    ; Set truncation rounding mode
    fstcw   [saved_cw]
    mov     ax, [saved_cw]
    or      ah, 0x0C            ; RC = 11 (truncate toward zero)
    mov     [trunc_cw], ax
    fldcw   [trunc_cw]

    ; Extract integer part
    fist    dword [int_part]
    fisub   dword [int_part]    ; st0 = fractional part only

    ; Print integer part
    mov     eax, [int_part]
    call    print_int

    ; Print decimal point
    push    eax
    mov     eax, 4
    mov     ebx, 1
    push    '.'
    mov     ecx, esp
    mov     edx, 1
    int     0x80
    add     esp, 4
    pop     eax

    ; Extract fractional digits (up to 6)
    fild    dword [ten]         ; st0=10, st1=frac
    fxch                        ; st0=frac, st1=10
    mov     edi, frac_digits
    mov     ecx, 6
.frac_loop:
    fmul    st0, st1            ; shift one digit left
    fist    dword [temp]        ; store digit
    fisub   dword [temp]        ; remove integer part
    mov     al, byte [temp]
    or      al, 0x30            ; to ASCII
    mov     byte [edi], al
    inc     edi
    dec     ecx
    jnz     .frac_loop

    mov     byte [edi], 0       ; null terminate
    ffree   st0
    ffree   st1
    fincstp
    fincstp
    fldcw   [saved_cw]          ; restore rounding mode

    ; Print fractional digits
    mov     esi, frac_digits
    call    print_str
    ret


; ── print_int: print signed 32-bit integer in EAX ───────
print_int:
    push    ebx
    push    ecx
    push    edx
    push    edi

    mov     edi, int_buf + 15
    mov     byte [edi], 0       ; null terminator
    dec     edi

    mov     ecx, eax
    test    eax, eax
    jns     .pi_positive
    neg     eax                 ; work with absolute value
.pi_positive:
    mov     ebx, 10
.pi_loop:
    xor     edx, edx
    div     ebx                 ; eax = quotient, edx = remainder
    add     dl, '0'
    mov     byte [edi], dl
    dec     edi
    test    eax, eax
    jnz     .pi_loop

    test    ecx, ecx
    jns     .pi_print
    mov     byte [edi], '-'     ; add minus sign
    dec     edi
.pi_print:
    inc     edi
    mov     esi, edi
    call    print_str

    pop     edi
    pop     edx
    pop     ecx
    pop     ebx
    ret


; ── print_str: print null-terminated string at ESI ──────
print_str:
    push    eax
    push    ebx
    push    ecx
    push    edx

    ; Find length
    mov     ecx, esi
    xor     edx, edx
.ps_len:
    cmp     byte [esi + edx], 0
    je      .ps_print
    inc     edx
    jmp     .ps_len
.ps_print:
    test    edx, edx
    jz      .ps_done
    mov     eax, 4              ; sys_write
    mov     ebx, 1              ; stdout
    int     0x80
.ps_done:
    pop     edx
    pop     ecx
    pop     ebx
    pop     eax
    ret
