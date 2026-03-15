/*
 * Lab 8 — LCD Display Controller
 *
 * Drives an HD44780-compatible LCD via 8-bit parallel interface
 * on the Intel 8051 microcontroller.
 *
 * Pins: P2 = data bus, P3.2 = RS, P3.3 = RW, P3.4 = EN
 * P1 = input port (reads 4-bit value and displays it)
 */

#include <reg51.h>

sfr   ldata     = 0xA0;    /* LCD data port (P2) */
sfr   inputData = 0x90;    /* Input port (P1) */

sbit  rs = P3^2;           /* Register Select */
sbit  rw = P3^3;           /* Read/Write */
sbit  en = P3^4;           /* Enable */

typedef unsigned char uchar;
typedef unsigned int  uint;

/* ── Delay ──────────────────────────────────────── */

void delay_ms(uint ms) {
    uint i, j;
    for (i = 0; i < ms; i++)
        for (j = 0; j < 1275; j++)
            ;
}

/* ── LCD Commands ───────────────────────────────── */

void lcd_command(uchar cmd) {
    rs = 0; rw = 0; en = 0;
    ldata = cmd;
    en = 1;
    delay_ms(1);
    en = 0;
    delay_ms(1);
}

void lcd_write(uchar ch) {
    en = 0; rs = 1; rw = 0;
    ldata = ch;
    en = 1;
    delay_ms(1);
    en = 0;
    delay_ms(1);
}

void lcd_set_row(uchar row) {
    lcd_command(row == 1 ? 0x80 : 0xC0);
}

void lcd_print(uchar row, uchar *str) {
    lcd_set_row(row);
    while (*str)
        lcd_write(*str++);
}

void lcd_init(void) {
    lcd_command(0x38);      /* 8-bit, 2 lines, 5x7 font */
    lcd_command(0x0E);      /* display on, cursor on */
    lcd_command(0x01);      /* clear display */
    delay_ms(2);
}

/* ── Number-to-string conversion ────────────────── */

void display_input(void) {
    uchar val = inputData & 0x0F;
    uchar buf[4];

    if (val < 10) {
        buf[0] = '0' + val;
        buf[1] = 0;
    } else {
        buf[0] = '1';
        buf[1] = '0' + (val - 10);
        buf[2] = 0;
    }
    lcd_print(1, buf);
}

/* ── Main ───────────────────────────────────────── */

void main(void) {
    while (1) {
        lcd_init();
        lcd_print(1, "8051 LCD Demo");
        lcd_print(2, "Input: ");
        lcd_set_row(2);
        lcd_write(' '); lcd_write(' '); lcd_write(' ');
        lcd_write(' '); lcd_write(' '); lcd_write(' ');
        lcd_write(' ');
        display_input();
        delay_ms(50);
    }
}
