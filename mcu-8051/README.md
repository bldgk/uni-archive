# Intel 8051 Microcontroller Programs

Assembly and C programs for the Intel 8051 microcontroller (2014), cleaned up and organized by topic.

## Programs

| File | Topic | What it does |
|------|-------|-------------|
| `memory_and_addressing.asm` | Memory | XRAM read/write, stack operations, indirect addressing |
| `arithmetic.asm` | Math | Integer division, iterative computation, stack accumulation |
| `bit_manipulation.asm` | Bitwise | Nibble swap between registers using AND/OR/SWAP |
| `subroutines.asm` | Subroutines | CALL/RET, dispatch based on divisibility, 3 addressing modes |
| `timer.asm` | Timers | Timer 0 Mode 1 (16-bit), precise delays, overflow counting |
| `uart.asm` | Serial | UART 9600 baud setup, send/receive, string printing |
| `lcd_display.c` | LCD | HD44780 LCD controller via 8-bit parallel, number display |

## Target

Intel 8051 (MCS-51) architecture. Originally developed for Proteus simulation.

Can be assembled with [ASEM-51](http://plit.de/asem-51/) or simulated in [EdSim51](https://www.edsim51.com/).
