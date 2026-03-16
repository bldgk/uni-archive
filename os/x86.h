#ifndef X86_H
#define X86_H

#define EXIT_REBOOT 1

static inline void outb_p(unsigned port, unsigned value) {
    asm("outb %b0, %w1":: "a"(value), "d"(port));
}

static inline unsigned inpb_p(unsigned port) {
    unsigned value;
    asm("inb %w1, %b0": "=a"(value): "d"(port));
    return value;
}

void exit(int flag) {
    int temp;

    switch (flag) {
    case EXIT_REBOOT:
        do {
            temp = inpb_p(0x64);
            if (temp & 1)
                inpb_p(0x60);
        } while (temp & 2);
        outb_p(0x64, 0xFE);   
        break;

    default:
        break;
    }
}

#endif
