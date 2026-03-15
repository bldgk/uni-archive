; x86 Protected Mode Bootloader with Hardware Task Switching
; Demonstrates TSS-based multitasking from a 512-byte boot sector
;
; Two tasks ping-pong via JMP to each other's TSS selectors,
; printing messages to VGA text memory, then halt.
;
; Build: nasm -f bin -o boot.bin bootloader.asm
; Run:   qemu-system-i386 -fda boot.bin

%define DATA_SELECTOR    (1 << 3)
%define CODE_SELECTOR    (2 << 3)
%define RM_DATA_SELECTOR (3 << 3)
%define RM_CODE_SELECTOR (4 << 3)
%define TSS_1_SELECTOR   (5 << 3)
%define TSS_2_SELECTOR   (6 << 3)
%define TSS_1_BASE       0x2000
%define TSS_2_BASE       0x2200
%define TASK_1_STACK     0x9000
%define TASK_2_STACK     0x9800

bits 16
org 0x7c00


start:
    cli
    lgdt    [cs:GDTR]

    mov     eax, cr0
    bts     eax, 0
    mov     cr0, eax

    jmp     CODE_SELECTOR:pm_start


bits 32

pm_start:
    ; PROTECTED MODE --------------------
    mov     ax, DATA_SELECTOR
    mov     ds, ax
    mov     es, ax
    mov     ss, ax
    mov     esp, 0x7c00
    cld

    call    main

    ; Leaving PM (not reached in this demo)
    jmp     RM_CODE_SELECTOR:pm_exiting

pm_exiting:
    mov     ax, RM_DATA_SELECTOR
    mov     ds, ax

    mov     eax, cr0
    btc     eax, 0
    mov     cr0, eax

    jmp     0:exit

bits 16

exit:
    ; REAL MODE --------------------
    mov     ax, 0
    mov     ds, ax
    sti
    jmp $

bits 32

main:
    ; Create TSS descriptors in GDT (entries 5 and 6)
    mov     edi, GDTEND
    mov     ebx, TSS_1_BASE
    call    make_tdesc

    mov     edi, GDTEND + 8
    mov     ebx, TSS_2_BASE
    call    make_tdesc

    ; Initialize TSS structures
    mov     eax, task_1
    mov     ebx, TASK_1_STACK
    mov     edi, TSS_1_BASE
    call    make_tss

    mov     eax, task_2
    mov     ebx, TASK_2_STACK
    mov     edi, TSS_2_BASE
    call    make_tss

    ; Switch to task 1 (no current task in TR, CPU skips state save)
    jmp     TSS_1_SELECTOR:0


; Create a TSS descriptor in the GDT
; edi = descriptor address, ebx = TSS base address
make_tdesc:
    mov     word [edi],     0x67    ; limit 0-15 (103 = minimum TSS size)
    mov     word [edi + 2], bx      ; base 0-15
    mov     byte [edi + 4], 0       ; base 16-23
    mov     byte [edi + 5], 0x89    ; P=1, DPL=0, Type=9 (available 32-bit TSS)
    mov     byte [edi + 6], 0x00    ; G=0, limit 16-19 = 0
    mov     byte [edi + 7], 0       ; base 24-31
    ret


; Initialize a TSS structure in memory
; edi = TSS base, eax = EIP, ebx = ESP
make_tss:
    mov     dword [edi + 0x04], ebx          ; ESP0 (ring 0 stack)
    mov     dword [edi + 0x08], DATA_SELECTOR ; SS0
    mov     dword [edi + 0x20], eax          ; EIP
    mov     dword [edi + 0x24], 0x2          ; EFLAGS (bit 1 = reserved, always 1)
    mov     dword [edi + 0x38], ebx          ; ESP
    mov     dword [edi + 0x48], DATA_SELECTOR ; ES
    mov     dword [edi + 0x4C], CODE_SELECTOR ; CS
    mov     dword [edi + 0x50], DATA_SELECTOR ; SS
    mov     dword [edi + 0x54], DATA_SELECTOR ; DS
    mov     dword [edi + 0x58], DATA_SELECTOR ; FS
    mov     dword [edi + 0x5C], DATA_SELECTOR ; GS
    mov     dword [edi + 0x64], 0x00680000   ; I/O map base beyond TSS limit (no bitmap)
    ret


; --- Task 1 ---
task_1:
    mov     esi, message1
    mov     edi, 0xb80a0            ; VGA row 1
    call    print

    mov     eax, [tasks_done]
    bts     eax, 0
    mov     [tasks_done], eax
    .done:
        jmp     TSS_2_SELECTOR:0    ; switch to task 2
        jmp     .done               ; loop back when task 2 returns


; Print null-terminated string to VGA text memory
; esi = string, edi = VGA offset (e.g. 0xb8000 + row*160)
print:
    .loop:
        lodsb
        test    al, al
        jz      .end
        stosb                       ; write character byte
        mov     byte [edi], 0x0F   ; attribute: white on black
        inc     edi                 ; skip attribute byte
        jmp     .loop
    .end:
    ret


; --- Task 2 ---
task_2:
    mov     esi, message2
    mov     edi, 0xb8140            ; VGA row 2
    call    print

    mov     eax, [tasks_done]
    bts     eax, 1
    mov     [tasks_done], eax

    ; Switch to task 1 and wait for it to finish
    .wait:
        jmp     TSS_1_SELECTOR:0    ; switch to task 1
        mov     eax, [tasks_done]
        test    eax, eax
        jz      .wait               ; (both bits set = non-zero, falls through)

    mov     esi, message3
    mov     edi, 0xb81e0            ; VGA row 3
    call    print

    .halt:
        hlt
        jmp     .halt


message1:   db "Message from task one.", 0
message2:   db "Message from task two.", 0
message3:   db "Tasks done!", 0

tasks_done: dd 0


GDTR:
    dw 7 * 8 - 1           ; limit: 7 entries (null + 4 static + 2 TSS)
    dd GDT                  ; base address

GDT:
    dq 0                                        ; 0: null descriptor
    db 0xFF, 0xFF, 0, 0, 0, 0x92, 0x8F, 0     ; 1: DATA - flat 4GB, RW
    db 0xFF, 0xFF, 0, 0, 0, 0x9A, 0xCF, 0     ; 2: CODE - flat 4GB, RX
    db 0xFF, 0xFF, 0, 0, 0, 0x92, 0x0F, 0     ; 3: RM DATA - 16-bit
    db 0xFF, 0xFF, 0, 0, 0, 0x9A, 0x0F, 0     ; 4: RM CODE - 16-bit
GDTEND:
    ; entries 5-6 (TSS descriptors) created at runtime

times 510-($-$$) db 0
    db 0x55
    db 0xAA
