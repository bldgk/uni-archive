# SCADA Process Control Scripts

VBScript programs for SCADA systems (MasterSCADA) that simulate industrial processes via Modbus registers (2017).

Each script runs in a cyclic scan loop, reading sensor values from Modbus holding/coil registers, computing the process model, and writing actuator outputs back.

Includes a hand-rolled **IEEE 754 float ↔ Modbus register** conversion library — Modbus uses 16-bit words, so a 32-bit float must be manually packed/unpacked across 2 registers.

## Scripts

| File | Process | What it models |
|------|---------|---------------|
| `heater_control.vbs` | Heating chamber | 3 independent heaters (30/42/50°C targets) + natural cooling |
| `furnace_control.vbs` | Metallurgical line | Furnace (burner + pump) → rolling mill → cooling room |
| `elevator_control.vbs` | Grain elevator | 2 tanks with level sensors, valves, overflow detection |
| `modbus_float.vbs` | Shared library | IEEE 754 single-precision ↔ Modbus 16-bit word pairs |

## Modbus Float Conversion

```
                    32-bit IEEE 754 float
           ┌────────────────┬────────────────┐
           │   Hi Word (16) │   Lo Word (16) │
           │  Register N+1  │   Register N   │
           └────────────────┴────────────────┘
              Sign  Exponent     Mantissa
```

## Environment

Designed for MasterSCADA or similar SCADA systems with VBScript scripting support and Modbus RTU/TCP communication.
