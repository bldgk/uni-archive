# x87 FPU Arithmetic

x86 assembly program that computes `(a + b) * c + d` using both the **x87 FPU** (floating-point) and **integer ALU**, then prints the results.

The float-to-string conversion is done manually — extracting the integer part with `fist`/`fisub`, then isolating each fractional digit by repeatedly multiplying by 10.

```
Float:   (-7.005 + 3.005) * 2.005 + 3.005 = -5.015
Integer: (-7 + 3) * 2 + 3 = -5
```

## Build & Run

Requires a 32-bit Linux environment (uses `int 0x80` syscalls):

```bash
nasm -f elf32 -o fpu-calc.o fpu-calc.asm
ld -m elf_i386 -o fpu-calc fpu-calc.o
./fpu-calc
```

On macOS/64-bit Linux, use Docker:

```bash
docker run --rm -v $PWD:/w -w /w i386/alpine sh -c \
  "apk add nasm binutils && nasm -f elf32 -o fpu-calc.o fpu-calc.asm && \
   ld -m elf_i386 -o fpu-calc fpu-calc.o && ./fpu-calc"
```
