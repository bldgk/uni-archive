#ifndef STDIO_H
#define STDIO_H

void freeze();
void doprintf(const char *fmt, va_list args);
void printf(const char *msg, ...);
void putc(unsigned char c);
void puthexd(unsigned char digit);
void putdec(unsigned int byte);
void puthex(unsigned char byte);
void puthexi(unsigned int dword);
void puts(unsigned char *s);
void update_cursor();
void update_fixcursor();
void setcolor(unsigned color);
void start_textmode();
void gotoxy(int x, int y);
void clrscr();
unsigned char getc();

int cur_cursorX, cur_cursorY;
int fixcur_cursorX, fixcur_cursorY;
unsigned short *vgaadr;
unsigned textattrib, scrwidth, scrheight;

void freeze() {
    while (1);
}

void update_fixcursor() {
    fixcur_cursorX = cur_cursorX;
    fixcur_cursorY = cur_cursorY;
}

//unsigned char keyboard_wait_asci();
//unsigned char getc() {
    //return keyboard_wait_asci();
//}

void doprintf(const char *fmt, va_list args) {
    while (*fmt) {

        switch (*fmt) {
        case '%':
            fmt++;

            switch (*fmt) {
            case 's':
                puts(va_arg(args, char*));
                break;

            case 'c':
                putc(va_arg(args, unsigned int));
                break;

            case 'd':
                putdec(va_arg(args, unsigned int));
                break;

            case 'x':
                puthex(va_arg(args, unsigned int));
                break;

            case 'X':
                puthexi(va_arg(args, unsigned int));
                break;

            case 'z':
                setcolor(va_arg(args, unsigned int));
                break;

            }
            break;

        default:
            putc(*fmt);
            break;
        }
        fmt++;
    }
}

void putc(unsigned char c) {
    unsigned att;
    unsigned short *where;
    att = textattrib << 8;
    if (c == BACKSPACE) {
        if ((cur_cursorX != 0) && ((cur_cursorX > fixcur_cursorX) || (cur_cursorY > fixcur_cursorY)))
            cur_cursorX--;
        else if ((cur_cursorX == 0) && (cur_cursorY > fixcur_cursorY)) {
            cur_cursorY--;
            cur_cursorX = scrwidth - 1;
        } else return;
        where = vgaadr + (cur_cursorY * scrwidth + cur_cursorX);
        *where = ' ' | att;
    } else if (c == TAB) {
        cur_cursorX += 4;
    } else if (c == RETURN) {
        cur_cursorX = 0;
    } else if (c == ENTER) {
        cur_cursorX = 0;
        if (cur_cursorY == scrheight - 1) {
            memcpy(vgaadr, vgaadr + scrwidth, (scrheight - 1) * scrwidth * 2);
            int i;
            for (i = 0; i < scrwidth; i++)
                *(unsigned short *)(vgaadr + (scrheight - 1) * scrwidth + i) = ' ' | (0 << 8);
        } else
            cur_cursorY++;
    } else if (c >= ' ') {
		//if(c>=' ')
		//{
        where = vgaadr + (cur_cursorY * scrwidth + cur_cursorX);
        *where = c | att;
        cur_cursorX++;
    }
    if (cur_cursorX >= scrwidth) {
        cur_cursorX = 0;
        cur_cursorY++;
    }
    update_cursor();
}

void puts(unsigned char *s) {
    char c;
    while ((c = *s++) != '\0' ) {
        putc(c);
    }
}

void start_textmode() {
    vgaadr = (unsigned short *)0xb8000;
    textattrib = COLOR_LGRAY;
    scrwidth = 80;
    scrheight = 25;
    clrscr();
    gotoxy(0, 0);
}

void gotoxy(int x, int y) {
    if ((x < fixcur_cursorX) && (y < fixcur_cursorY))
        return;

    short position = (y * scrwidth + x);

    outb_p(0x3D4, 0x0F);
    outb_p(0x3D5, (unsigned char)(position & 0xFF));

    outb_p(0x3D4, 0x0E);
    outb_p(0x3D5, (unsigned char)((position >> 8) & 0xFF));

    cur_cursorX = x;
    cur_cursorY = y;
}

void update_cursor() {
    gotoxy(cur_cursorX, cur_cursorY);
}

void setcolor(unsigned color) {
    textattrib = color;
}

void puthexd(unsigned char digit) {
    char table[] = "0123456789ABCDEF";
    putc(table[digit]);
}

void putdec(unsigned int byte) {
    unsigned char b1;
    int b[30];
    signed int nb;
    int i = 0;

    while (1) {
        b1 = byte % 10;
        b[i] = b1;
        nb = byte / 10;
        if (nb <= 0) {
            break;
        }
        i++;
        byte = nb;
    }

    for (nb = i + 1; nb > 0; nb--) {
        puthexd(b[nb - 1]);
    }
}

void puthex(unsigned char byte) {
    unsigned char lb, rb;

    lb = byte >> 4;
    rb = byte & 0x0F;

    puthexd(lb);
    puthexd(rb);
}

void puthexi(unsigned int dword) {
    puthex((dword & 0xFF000000) >> 24);
    puthex((dword & 0x00FF0000) >> 16);
    puthex((dword & 0x0000FF00) >> 8);
    puthex((dword & 0x000000FF));
}

void printf(const char *msg, ...) {
    va_list printfargs;
    va_start(printfargs, msg);
    doprintf(msg, printfargs);
    va_end(printfargs);
}

void clrscr() {
    int i = 0;
    while (i < scrwidth * scrheight) {
        *(vgaadr + i) = ' ' | (0 << 8);
        i++;
    }
}

#endif
