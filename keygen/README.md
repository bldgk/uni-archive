# License Key Generator

CLI tool that generates and validates license keys tied to the machine's MAC address.

The key is derived by XOR-scrambling the MAC bytes with a fixed seed, then encoding the result in base-36 formatted as `XXXXX-XXXXX-XXXXX`.

```
  ╔═══════════════════════════════════════╗
  ║         LICENSE KEY GENERATOR         ║
  ╚═══════════════════════════════════════╝

  [1] Generate key for this machine
  [2] Validate a key
  [3] Show MAC address
  [0] Exit

  > 1

    ┌─────────────────────┐
    │  02900-TL600-3M600  │
    └─────────────────────┘

  > 2
  Enter key: 02900-TL600-3M600

  ✓ Key is VALID for this machine.
```

## Build & Run

```bash
g++ -std=c++17 -o keygen keygen.cpp && ./keygen
```

Works on macOS and Linux — reads MAC address from network interfaces.
