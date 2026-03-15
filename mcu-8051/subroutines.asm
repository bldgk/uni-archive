; Lab 5 — Subroutines and Addressing Modes
;
; Demonstrates CALL/RET, division subroutine, and three
; addressing modes: register-indirect (RPD), external (ZPD),
; and accumulator-direct (ACC).

org 0h
jmp Main

ADDRESS equ 0x20

org 100h

; ── Subroutines ─────────────────────────────────────

division:
    div  AB
    ret

store_rpd:                  ; Register-indirect: store to internal RAM
    mov  R1, #ADDRESS
    mov  A, R0
    mov  @R1, A
    ret

store_zpd:                  ; External memory: store to XRAM
    mov  DPTR, #ADDRESS
    mov  A, R0
    movx @DPTR, A
    ret

store_acc:                  ; Direct: store in accumulator
    mov  ACC, R0
    ret

; ── Dispatch: route based on divisibility ───────────

dispatch:
    ; if R0 % 2 == 0 → register-indirect
    mov  A, R0
    mov  B, #2
    call division
    mov  A, B               ; A = remainder
    jz   do_rpd

    ; if R0 % 3 == 0 → external memory
    mov  A, R0
    mov  B, #3
    call division
    mov  A, B
    jz   do_zpd

    ; otherwise → accumulator
    call store_acc
    ret

do_rpd:
    call store_rpd
    ret

do_zpd:
    call store_zpd
    ret

; ── Main ────────────────────────────────────────────

Main:
    mov  R0, #7             ; test value
    call dispatch
    jmp  $
end
