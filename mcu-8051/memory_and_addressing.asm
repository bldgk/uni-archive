; Lab 1 — Memory Operations and Addressing Modes
;
; Demonstrates external memory (XRAM) access, stack operations,
; and indirect addressing on the Intel 8051.

org 0h
jmp Main

org 100h

Main:
    ; ── Fill XRAM with descending values ──
    setb PSW.4              ; select register bank 2
    mov  A, #08h
    push ACC
    mov  DPTR, #004Ch

fill_xram:
    movx @DPTR, A           ; write A to external memory
    inc  DPTR
    dec  A
    jnz  fill_xram

    ; ── Read back from XRAM into registers ──
    mov  DPTR, #004Ch
    mov  R0, #10h
    pop  B                  ; B = 8 (loop counter)
    clr  A

read_xram:
    movx A, @DPTR           ; read from external memory
    push ACC
    inc  DPTR
    inc  R0
    dec  B
    jnz  read_xram

    jmp  $
end
