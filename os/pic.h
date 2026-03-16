#ifndef PIC_H
#define PIC_H

#define MASTER_PORT_A 0x20
#define MASTER_PORT_B 0x21
#define SLAVE_PORT_A 0xA0
#define SLAVE_PORT_B 0xA1

#define IRQTIMER 0x00
#define IRQKEYBOARD 0x01

#define irq_endhandler asm("movb $0x20, %al\n outb %al, $0x20\n mov %ebp, %esp\n pop %ebp\n iret\n");

unsigned char irq_status_master = 0;
unsigned char irq_status_slave = 0;

void pic_init() {
    outb_p(MASTER_PORT_A, 0x11);
    outb_p(SLAVE_PORT_A, 0x11);
    outb_p(MASTER_PORT_B, 0x20);
    outb_p(SLAVE_PORT_B, 0x28);
    outb_p(MASTER_PORT_B, 0x04);
    outb_p(SLAVE_PORT_B, 0x02);
    outb_p(MASTER_PORT_B, 0x01);
    outb_p(SLAVE_PORT_B, 0x01);
    outb_p(MASTER_PORT_B, 0xFF);
    outb_p(SLAVE_PORT_B, 0xFF);
}

void pic_enable(unsigned char irq_no) {
    unsigned char temp = 0x01;

    if (irq_no > 15)
        return;

    if (irq_no < 8) {
        temp <<= irq_no;
        irq_status_master |= temp;
        outb_p(MASTER_PORT_B, (unsigned char)~irq_status_master);
    } else {
        irq_no -= 8;
        temp <<= irq_no;
        irq_status_slave |= temp;
        outb_p(SLAVE_PORT_B,(unsigned char)~irq_status_slave);
        irq_status_master |= 0x04;
        outb_p(MASTER_PORT_B,(unsigned char)~irq_status_master);
    }
}

void pic_disable(unsigned char irq_no) {
    unsigned char temp = 0x01;

    if (irq_no > 15)
        return;

    if (irq_no < 8) {
        temp <<= irq_no;
        irq_status_master &= ~temp;

        outb_p(MASTER_PORT_B, (unsigned char)~irq_status_master);
    } else {
        irq_no -= 8;
        temp <<= irq_no;
        irq_status_slave &= ~temp;
        outb_p(SLAVE_PORT_B,(unsigned char)~irq_status_slave);
        irq_status_master &= ~0x04;
        outb_p(MASTER_PORT_B,(unsigned char)~irq_status_master);
    }
}

#endif
