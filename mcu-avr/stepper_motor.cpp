/*
 * Stepper Motor Controller
 *
 * Drives a bipolar stepper motor in full-step mode on ATmega8.
 * Outputs a 4-phase sequence on PORTD[0:3] with 200ms per step.
 *
 * Phase sequence (full-step):
 *   Step 1: +A +B  (0011)
 *   Step 2: -A +B  (0110)
 *   Step 3: -A -B  (1100)
 *   Step 4: +A -B  (1001)
 *
 * Pinout:
 *   PD0 = coil A+
 *   PD1 = coil B+
 *   PD2 = coil A-
 *   PD3 = coil B-
 */

#include <avr/io.h>
#include <util/delay.h>

#define STEP_DELAY_MS 200

// Full-step sequence for bipolar stepper
const unsigned char steps[] = {
    0b00000011,  // +A +B
    0b00000110,  // +B -A
    0b00001100,  // -A -B
    0b00001001,  // -B +A
};

void stepper_init(void) {
    DDRD  = 0x0F;           // PD0-PD3 as outputs
    PORTD = 0x00;
}

void stepper_step(unsigned char phase) {
    PORTD = steps[phase & 0x03];
    _delay_ms(STEP_DELAY_MS);
}

void stepper_rotate(int num_steps) {
    static unsigned char phase = 0;
    for (int i = 0; i < num_steps; i++) {
        stepper_step(phase);
        phase = (phase + 1) & 0x03;
    }
}

void stepper_rotate_reverse(int num_steps) {
    static unsigned char phase = 0;
    for (int i = 0; i < num_steps; i++) {
        stepper_step(phase);
        phase = (phase - 1) & 0x03;
    }
}

int main(void) {
    stepper_init();

    while (1) {
        stepper_rotate(200);         // one full revolution (~200 steps)
        stepper_rotate_reverse(200); // reverse
    }
}
