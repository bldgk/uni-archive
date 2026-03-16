#!/bin/bash
set -e

# Compile C files
i686-elf-gcc -ffreestanding -m32 -c entry.c -o entry.o
i686-elf-gcc -ffreestanding -m32 -c kernel.c -o kernel.o

# Link kernel at 0x200000
i686-elf-ld -Ttext 0x200000 -o kernel.bin entry.o kernel.o
i686-elf-objcopy kernel.bin -O binary

# Assemble bootloader and create disk image
nasm -fbin -o setup.o setup.asm
nasm -fbin -o MyOS.img boot.asm

# Run in QEMU
qemu-system-i386 -fda MyOS.img -boot a
