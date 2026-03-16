#ifndef STRING_H
#define STRING_H

#define strcat(x, y) memcpy((void *)(x + strlen(x) + 1), (void *)y, (strlen(y) + 1))
#define strcpy(x, y) memcpy((void *)x, (void *)y, (strlen(x) + 1))
#define strcmp(x, y) memcmp((void *)x, strlen(x), (void *)y, strlen(y))

int strlen(char* charlen) {
    int valret;
    for (valret = 0; *charlen != '\0'; charlen++)
        valret++;
    return valret;
}

#endif
