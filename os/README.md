# MyOS

A minimal x86 operating system kernel that boots from a floppy disk image. Written in x86 assembly and C, it demonstrates the full boot process from BIOS power-on to a running 32-bit protected mode kernel.


## What it does

1. **Stage 1 bootloader** (`boot.asm`) — Loaded by BIOS from the first 512 bytes boot sector at 0x7C00 of a floppy disk. Reads the remaining sectors from disk into memory and hands off to the second stage (0x0700)

2. **Stage 2 setup** (`setup.asm`) — Initializes the Programmable Interrupt Controller (PIC), loads the Global Descriptor Table (GDT), enables A20 line (`A20.asm`), switches the CPU from 16-bit real mode to 32-bit protected mode, and copies the kernel to its target address at `0x200000`.

3. **Kernel** (`kernel.c`) — Initializes VGA text mode (80x25, 16 colors), prints a startup message, and enters the main loop. Includes:
   - `printf` with format specifiers (`%s`, `%d`, `%x`, `%z` for color)
   - PS/2 keyboard driver with interrupt handling (IRQ1), modifier keys, Caps/Num/Scroll Lock LEDs
   - Alt+Ctrl+Del reboot support
   - Panic/error handler with reboot options


## Memory Layout

- `0x0700` — Setup code
- `0x6800` — IDT (Interrupt Descriptor Table)
- `0x200000` — Kernel binary
- `0xB8000` — VGA text mode video memory


## Module Map (all header-only, no separate .c library files)

| Header | Purpose |
|--------|---------|
| `stdio.h` | printf (with `%z` color specifier), putc, cursor control, clrscr, text mode init |
| `x86.h` | Port I/O (`outb_p`, `inpb_p`), reboot |
| `idt.h` | IDT setup and loading |
| `pic.h` | 8259 PIC initialization, IRQ enable/disable |
| `keys.h` | Keyboard IRQ1 handler, modifier tracking, Alt+Ctrl+Del reboot |
| `keydefs.h` | PS/2 scan code tables and ASCII mappings |
| `colors.h` | VGA 16-color constants (`COLOR_LBLUE`, etc.) |
| `memory.h` | memcpy, memset, memcmp |
| `string.h` | strlen, strcat, strcpy, strcmp |
| `stdarg.h` | va_start, va_arg, va_end |
| `erros.h` | panic function (note: filename typo is intentional) |

## Prerequisites

```bash
brew install i686-elf-gcc nasm qemu
```

## Build and run

```bash
./buildnrun.sh
```

This compiles the C source, assembles the bootloader, links everything at address `0x200000`, assembles boot/setup with NASM into a floppy disk image (`MyOS.img`), and launches it in QEMU.
