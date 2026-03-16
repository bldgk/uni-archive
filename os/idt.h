#ifndef IDT_H
#define IDT_H

#define IDT_BASE 0x6800
#define IDT_LOBASE 0x6800
#define IDT_HIBASE 0
#define IDT_LIMIT 36

#define CS_SELECTOR 0x08
#define INT_GATE 0x06
#define TRAP_GATE 0x07

#define BITS_16 0
#define BITS_32 0x08

#define ABSENT 0
#define PRESENT 0x80

#define RING0 0
#define RING1 0x20
#define RING2 0x40
#define RING3 0x60

typedef struct idt_tag {
    unsigned short int loffset;
    unsigned short int selector;
    unsigned char unused;
    unsigned char options;
    unsigned short int uoffset;
}idt_t;

typedef struct idtr_tag {
    unsigned short int limit;
    unsigned short int lobase;
    unsigned short int hibase;
}idtr_t;

void idt_install(int num, unsigned int offset, unsigned short int selector, unsigned char options) {
    idt_t *idt_ptr = (idt_t *)IDT_BASE + num;
    idt_ptr->loffset = offset & 0x0000FFFF;
    idt_ptr->selector = selector;
    idt_ptr->unused = 0;
    idt_ptr->options = options;
    idt_ptr->uoffset = (offset & 0xFFFF0000) >> 16;
}

void idt_load(idtr_t *idtr) {
    asm("lidt (%0)"::"p"(idtr));
}

void idt_init() {
    idtr_t idtr;
    idtr.limit = (unsigned short)(36 * 8);
    idtr.lobase = (unsigned short)IDT_LOBASE;
    idtr.hibase = (unsigned short)IDT_HIBASE;
    idt_load(&idtr);
}

#endif
