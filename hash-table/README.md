# Hash Table with Separate Chaining

A generic hash table implementation in C++ using linked-list buckets and a polynomial rolling hash function (base 53).

Supports insert, search, delete. Demonstrates collision handling via chaining.

## Build & Run

```bash
g++ -std=c++17 -o hash_table hash_table.cpp && ./hash_table
```

```
  bucket[7]:
    Taras Shevchenko
    Kirill Beldiaga
  bucket[9]:
    Alan Turing
  bucket[10]:
    Edsger Dijkstra
  bucket[12]:
    Bogdan Kalnybolochyk
  bucket[13]:
    Ada Lovelace

  Total: 6 entries, insert time: 5 us

  find(Alan Turing) -> FOUND
  find(Grace Hopper) -> NOT FOUND
```
