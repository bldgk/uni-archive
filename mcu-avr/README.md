# AVR ATmega Microcontroller Programs

Embedded C programs for AVR ATmega microcontrollers (2017), cleaned up and documented.

## Programs

| File | What | Peripherals |
|------|------|-------------|
| `adc_7segment.cpp` | Reads analog input, displays 4-digit value on multiplexed 7-segment display | ADC, Timer 2, GPIO |
| `stepper_motor.cpp` | Bipolar stepper motor control in full-step mode with forward/reverse | GPIO, delay |
| `pwm_motor.cpp` | PWM motor speed control with button interrupts for speed up/down/direction | Timer 3 PWM, INT0/5/7 |

## Target

- `adc_7segment.cpp` — ATmega8
- `stepper_motor.cpp` — ATmega8
- `pwm_motor.cpp` — ATmega128/2560

Originally developed for Proteus simulation. Can be compiled with `avr-gcc`:

```bash
avr-gcc -mmcu=atmega8 -Os -o adc_7segment.elf adc_7segment.cpp
avr-gcc -mmcu=atmega8 -Os -o stepper_motor.elf stepper_motor.cpp
avr-gcc -mmcu=atmega128 -Os -o pwm_motor.elf pwm_motor.cpp
```
