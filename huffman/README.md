# Huffman Coding

Huffman encoder and decoder with tree visualization. Originally a C#/WPF app (2015), ported to Python 3.

Builds an optimal prefix-free binary code from character frequencies using a priority queue (min-heap).

## Run

```bash
python3 huffman.py
```

```
  Input:  "to be or not to be that is the question"
  Length: 39 chars (312 bits)

  Code table:
    ' '            01  (freq: 9)
     o           101  (freq: 5)
     t           111  (freq: 7)
     e          1101  (freq: 4)
     ...

  Encoded: 111101010001110101101001110100101011...
  Bits:    130
  Ratio:   2.40x compression

  Decoded: "to be or not to be that is the question"
  Match:   OK

  Huffman tree:
  ├── [39]
  │   ├── [17]
  │   │   ├── [8]
  │   │   │   ├── [4]
  │   │   │   │   ├── 'i' (2)
  │   │   │   │   └── 'b' (2)
  │   │   ...
  │   └── [22]
  │       └── [13]
  │           └── 't' (7)
```

No dependencies beyond Python 3 standard library.
