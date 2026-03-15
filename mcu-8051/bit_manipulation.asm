; Lab 3 — Bit Manipulation
;
; Swaps nibbles between two registers using AND, OR, and SWAP instructions.
; Extracts high/low nibbles and recombines them.

org 0h
jmp Main

MASK_LO equ 00001111b
MASK_HI equ 11110000b

org 100h

Main:
    mov  R0, #15h           ; R0 = 0x15
    mov  R1, #1Ah           ; R1 = 0x1A

    ; ── Extract low nibble of R0 (swapped) and low nibble of R1 ──
    mov  A, R0
    swap A                  ; swap nibbles: 0x15 -> 0x51
    anl  A, #MASK_LO        ; keep low nibble: 0x01
    mov  B, A

    mov  A, R1
    anl  A, #MASK_LO        ; low nibble of R1: 0x0A
    orl  A, B               ; combine: 0x0B... wait

    ; ── Combine with high nibble of R0 ──
    mov  B, A
    mov  A, R0
    anl  A, #MASK_HI        ; high nibble of R0: 0x10
    orl  A, B               ; final result

    mov  R0, A              ; store result
    jmp  $
end
