"""
Huffman Coding — encoder and decoder.

Builds a Huffman tree from character frequencies, generates
variable-length prefix codes, and encodes/decodes text.

Run: python3 huffman.py
"""
from collections import Counter
import heapq


# ── Tree Node ───────────────────────────────────────

class Node:
    def __init__(self, char=None, freq=0, left=None, right=None):
        self.char = char
        self.freq = freq
        self.left = left
        self.right = right

    def __lt__(self, other):
        return self.freq < other.freq

    def is_leaf(self):
        return self.left is None and self.right is None


# ── Huffman Coder ───────────────────────────────────

class HuffmanCoder:
    def __init__(self, text):
        self.text = text
        self.codes = {}
        self.tree = None
        self._build_tree()
        self._build_codes(self.tree, "")

    def _build_tree(self):
        freq = Counter(self.text)
        heap = [Node(char=ch, freq=f) for ch, f in freq.items()]
        heapq.heapify(heap)

        while len(heap) > 1:
            left = heapq.heappop(heap)
            right = heapq.heappop(heap)
            parent = Node(freq=left.freq + right.freq, left=left, right=right)
            heapq.heappush(heap, parent)

        self.tree = heap[0] if heap else None

    def _build_codes(self, node, prefix):
        if node is None:
            return
        if node.is_leaf():
            self.codes[node.char] = prefix or "0"
            return
        self._build_codes(node.left, prefix + "0")
        self._build_codes(node.right, prefix + "1")

    def encode(self, text=None):
        if text is None:
            text = self.text
        return "".join(self.codes[ch] for ch in text)

    def decode(self, bits):
        result = []
        node = self.tree
        for bit in bits:
            node = node.left if bit == "0" else node.right
            if node.is_leaf():
                result.append(node.char)
                node = self.tree
        return "".join(result)

    def print_codes(self):
        for ch, code in sorted(self.codes.items(), key=lambda x: len(x[1])):
            display = repr(ch) if ch in (' ', '\n', '\t') else f" {ch}"
            print(f"    {display}  {code:>12s}  (freq: {Counter(self.text)[ch]})")

    def compression_ratio(self):
        original_bits = len(self.text) * 8
        encoded_bits = len(self.encode())
        return original_bits / encoded_bits if encoded_bits else 0


# ── Tree visualization ──────────────────────────────

def print_tree(node, prefix="", is_left=True):
    if node is None:
        return
    connector = "├── " if is_left else "└── "
    if node.is_leaf():
        ch = repr(node.char) if node.char in (' ', '\n', '\t') else node.char
        print(f"{prefix}{connector}'{ch}' ({node.freq})")
    else:
        print(f"{prefix}{connector}[{node.freq}]")
    extension = "│   " if is_left else "    "
    print_tree(node.left, prefix + extension, True)
    print_tree(node.right, prefix + extension, False)


# ── Main ────────────────────────────────────────────

if __name__ == "__main__":
    text = "to be or not to be that is the question"

    print("╔══════════════════════════════════════════╗")
    print("║         Huffman Coding Demo              ║")
    print("╚══════════════════════════════════════════╝\n")

    print(f"  Input:  \"{text}\"")
    print(f"  Length: {len(text)} chars ({len(text)*8} bits)\n")

    huff = HuffmanCoder(text)

    print("  Code table:")
    huff.print_codes()

    encoded = huff.encode()
    decoded = huff.decode(encoded)

    print(f"\n  Encoded: {encoded[:60]}{'...' if len(encoded) > 60 else ''}")
    print(f"  Bits:    {len(encoded)}")
    print(f"  Ratio:   {huff.compression_ratio():.2f}x compression")
    print(f"\n  Decoded: \"{decoded}\"")
    print(f"  Match:   {'OK' if decoded == text else 'FAIL'}")

    print(f"\n  Huffman tree:")
    print_tree(huff.tree, "  ")
