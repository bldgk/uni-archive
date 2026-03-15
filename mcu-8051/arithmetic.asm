; Lab 2 — Integer Arithmetic
;
; Computes a series using division and accumulation.
; b1 = 243, b2 = 81, q = b1/b2, then iteratively divides
; and pushes results onto the stack.

org 0h
jmp Main

org 100h

Main:
    mov  P0, #243           ; b1
    push P0
    mov  P1, #81            ; b2
    push P1

    ; ── Division: q = b1 / b2 ──
    mov  A, P0
    mov  B, P1
    div  AB
    mov  P2, A              ; q = quotient

    ; ── Iterative division loop ──
    mov  P3, #6             ; n = 6 iterations
    dec  P3
    dec  P3                 ; adjust counter
    mov  A, P1

divide_loop:
    mov  B, P2
    div  AB
    push ACC
    djnz P3, divide_loop

    ; ── Sum results from stack ──
    mov  P3, #6
    clr  A

sum_loop:
    pop  B
    add  A, B
    djnz P3, sum_loop

    mov  P0, A              ; result in P0
    jmp  $
end
