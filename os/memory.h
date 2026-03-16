#ifndef MEMORY_H
#define MEMORY_H

void *memcpy(void *dest, const void *src, int n) {
    int i;
    char *d = (char *)dest, *s = (char *)src;

    for (i = 0; i < n; i++) d[i] = s[i];
    return dest;
}

void *memset(void *s, int c, unsigned n) {
    int i;
    char *ss = (char *)s;

    for (i = 0; i < n; i++) ss[i] = c;
    return s;
}

int memcmp(const void *a, int na, const void *b, int nb) {
    int i = 0;
    if (na == nb) {
        while (i < na) {
            if (*(char *)(a + i) != *(char *)(b + i)) {
                return 0;
            }
            i++;
        }
        return 1;
    }
    return 0;
}

#endif
