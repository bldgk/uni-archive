#include "colors.h"

#include "keydefs.h"

#include "memory.h"
#include "string.h"
#include "stdarg.h"
#include "x86.h"
#include "stdio.h"
#include "pic.h"

#include "idt.h"
#include "keys.h"
#include "erros.h"


int kernel_main() {
    pic_init();
    idt_init();
    start_textmode();

    printf("%zThis is Kernel!\n%z", COLOR_LBLUE, COLOR_GREEN);
	printf("%zOur operation system has been started succesfully!\n%z", COLOR_LGREEN, COLOR_GREEN);
	while(1);
	return 0;
}
