# x86 Bootloader with Hardware Task Switching

A 512-byte boot sector that demonstrates **hardware task switching** via TSS (Task State Segment) in x86 Protected Mode.

Written in 2014 as a university assignment (System Programming course), cleaned up in 2026.

## What it does

1. Boots from a floppy image, switches CPU from Real Mode to Protected Mode
2. Sets up a GDT with code/data segments and two TSS descriptors
3. Creates two tasks that ping-pong execution via `JMP TSS_SELECTOR:0`
4. Each task prints a message to VGA text memory (`0xB8000`)
5. After both tasks complete, prints "Tasks done!" and halts

```
Message from task one.
Message from task two.
Tasks done!
```

## Build & Run

```bash
nasm -f bin -o boot.bin bootloader-task-switch.asm
qemu-system-i386 -fda boot.bin
```

## Requires

- [NASM](https://nasm.us/) — assembler
- [QEMU](https://www.qemu.org/) — emulator (`brew install nasm qemu`)
