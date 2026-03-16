#ifndef ERRORS_H
#define ERRORS_H

void panic(char *msg, int flag) {

    printf("\n%zError: %z%s", COLOR_LRED, COLOR_LGRAY, msg);

    switch (flag) {
    case 1:
        printf("\nPress any key to continue...\n");
        keyboard_wait_key();
        break;

    case 2:
        printf("\nPress any key to reboot...\n");
        keyboard_wait_key();
        exit(EXIT_REBOOT);
        printf("System could not reboot\nPlease turn off the system\n");
        freeze();
        break;

    case 3:
        printf("\nPlease turn off the system\n");
        freeze();
        break;

    default:
        break;
    }

}

#endif
