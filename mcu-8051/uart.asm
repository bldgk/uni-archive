; Lab 7 — UART Serial Communication
;
; Configures the 8051 UART for 9600 baud using Timer 1
; in auto-reload mode (Mode 2).

org 0h
jmp Main

org 100h

; ── UART initialization ────────────────────────────

uart_init:
    clr  TR1                ; stop Timer 1
    clr  IE.3               ; disable Timer 1 interrupt
    clr  IE.4               ; disable UART interrupt

    mov  TMOD, #00100000b   ; Timer 1, Mode 2 (8-bit auto-reload)
    mov  TH1, #0FDh         ; reload value for 9600 baud @ 11.0592 MHz
    mov  TL1, #0FDh

    mov  SCON, #01010000b   ; UART Mode 1, REN enabled
    mov  PCON, #00000000b   ; SMOD = 0 (no baud rate doubling)

    setb TR1                ; start Timer 1
    ret

; ── Send byte in A via UART ────────────────────────

uart_send:
    mov  SBUF, A            ; load byte into serial buffer
wait_send:
    jnb  TI, wait_send      ; wait for transmit complete
    clr  TI                 ; clear transmit flag
    ret

; ── Receive byte into A via UART ───────────────────

uart_recv:
    jnb  RI, uart_recv      ; wait for receive complete
    clr  RI                 ; clear receive flag
    mov  A, SBUF            ; read received byte
    ret

; ── Send null-terminated string from DPTR ──────────

uart_print:
    clr  A
    movc A, @A+DPTR         ; read byte from code memory
    jz   print_done         ; null terminator
    call uart_send
    inc  DPTR
    jmp  uart_print
print_done:
    ret

; ── Main ────────────────────────────────────────────

Main:
    call uart_init
    mov  DPTR, #message
    call uart_print
    jmp  $

message:
    db 'Hello from 8051!', 0Dh, 0Ah, 0
end
