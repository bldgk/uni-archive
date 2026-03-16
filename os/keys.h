#ifndef KEYS_H
#define KEYS_H

unsigned char kb_led = 0;
unsigned char kb_status = 0;
unsigned char kb_key = 0;
unsigned char kb_asci = 0;

void keyboard_ledstate(char flag, int enable) {
    if (enable) {
        kb_led |= flag;
    } else
        kb_led &= ~flag;
    while ((inpb_p(0x64) & 0x02));
    outb_p(KEYPORT, 0xED);
    while ((inpb_p(0x64) & 0x02));
    outb_p(KEYPORT, kb_led);
}

void keyboard_handler() {
    kb_key = inpb_p(KEYPORT);
    kb_asci = 0;

    if (kb_key & KEYPRESS) {
        kb_key &= ~KEYPRESS;

        switch (kb_key) {

        case KCAPS_LOCK:
            keyboard_ledstate(KCAPS_LED, kb_led & KCAPS_LED ? 0 : 1);
            break;

        case KNUM_LOCK:
            keyboard_ledstate(KNUM_LED, kb_led & KNUM_LED ? 0 : 1);
            break;

        case KSCROLL_LOCK:
            keyboard_ledstate(KSCROLL_LED, kb_led & KSCROLL_LED ? 0 : 1);
            break;

        case KALT:
            kb_status &= ~KMETA_ALT;
            break;

        case KCTRL:
            kb_status &= ~KMETA_CTRL;
            break;

        case KLEFT_SHIFT:
        case KRIGHT_SHIFT:
            kb_status &= ~KMETA_SHIFT;
            break;
        }

    } else {
        if (kb_key == KALT)
            kb_status |= KMETA_ALT;

        if (kb_key == KCTRL)
            kb_status |= KMETA_CTRL;

        if ((kb_key == KLEFT_SHIFT) || (kb_key == KRIGHT_SHIFT))
            kb_status |= KMETA_SHIFT;

        if ((kb_status & KMETA_ALT) && (kb_status & KMETA_CTRL) && (kb_key == KDEL))
            exit(EXIT_REBOOT);

        if (!(!(kb_led & KNUM_LED) && (kb_key >= 71) && (kb_key <= 83) && (kb_key != 74) && (kb_key!= 78))) { // check NUM PAD
            if (kb_status & KMETA_SHIFT) {
                if (kb_led & KCAPS_LED) {
                    kb_asci = asciNonShift[kb_key];
                } else
                    kb_asci = asciShift[kb_key];
            } else {
                if (kb_led & KCAPS_LED) {
                    kb_asci = asciShift[kb_key];
                } else
                    kb_asci= asciNonShift[kb_key];
            }
        }
    }

    irq_endhandler;
}

void keyboard_init() {
    idt_install(33, (unsigned int)&keyboard_handler, CS_SELECTOR, INT_GATE|BITS_32|PRESENT|RING0);
    pic_enable(IRQKEYBOARD);
}

unsigned char keyboard_wait_asci() {
    kb_asci = 0;
    while (!kb_asci);
    return kb_asci;
}

unsigned char keyboard_wait_key() {
    kb_key = 0;
    while (!kb_key);
    return kb_key;
}

#endif
