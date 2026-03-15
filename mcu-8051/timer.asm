; Lab 6 — Timers and Delays
;
; Uses Timer 0 in Mode 1 (16-bit) to create precise delays.
; Counts overflow events in R1.

org 0h
jmp Main

org 100h

; ── Single timer delay (10ms @ 12MHz) ──────────────

timer_delay:
    mov  TMOD, #00000001b   ; Timer 0, Mode 1 (16-bit)
    mov  TH0, #0D8h         ; preload high byte
    mov  TL0, #0F8h         ; preload low byte  (65536 - 10000 = 0xD8F0)
    setb TR0                ; start timer

wait_overflow:
    jnb  TF0, wait_overflow ; wait for overflow flag
    clr  TF0                ; clear overflow flag
    clr  TR0                ; stop timer
    ret

; ── Long delay: N × timer_delay ────────────────────

long_delay:                 ; R0 = number of repetitions
    call timer_delay
    jb   TF0, handle_overflow
continue:
    djnz R0, long_delay
    ret

handle_overflow:
    inc  R1                 ; count overflows
    clr  TF0
    jmp  continue

; ── Main ────────────────────────────────────────────

Main:
    mov  R0, #200           ; 200 × 10ms = 2 seconds
    mov  R1, #0             ; overflow counter
    call long_delay
    jmp  $
end
