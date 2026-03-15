/*
 * PWM Motor Speed Control with External Interrupts
 *
 * Generates PWM on Timer 3 (ATmega128/2560) to control motor speed.
 * Three external interrupts:
 *   INT0 — toggle direction (swap PD1/PD2)
 *   INT5 — increase speed (+10 duty cycle)
 *   INT7 — decrease speed (-10 duty cycle)
 *
 * Pinout:
 *   PE3 (OC3A) = PWM output
 *   PD0 = direction toggle button (INT0)
 *   PE5 = speed up button (INT5)
 *   PE7 = speed down button (INT7)
 *   PD1, PD2 = direction indicator LEDs
 */

#include <avr/io.h>
#include <avr/interrupt.h>

#define PWM_TOP 512

void pwm_init(void) {
    DDRE |= (1 << 3);                          // PE3 (OC3A) as output

    ICR3 = PWM_TOP;                             // TOP value for PWM

    TCCR3A |= (1 << COM3A1) | (1 << WGM31);   // Fast PWM, clear on match
    TCCR3B |= (1 << WGM33) | (1 << WGM32)     // WGM mode 14 (Fast PWM, ICR3 top)
            | (1 << CS31);                      // prescaler /8
}

void pwm_set_duty(int duty) {
    OCR3A = duty;
}

void interrupts_init(void) {
    // Direction LEDs
    DDRD |= (1 << 1) | (1 << 2);
    PORTD |= (1 << PIND1);                     // initial direction

    // Buttons as inputs with pull-ups
    DDRD &= ~(1 << 0);
    DDRE &= ~((1 << 5) | (1 << 7));
    PORTD |= (1 << PIND0);
    PORTE |= (1 << PINE5) | (1 << PINE7);

    // Falling edge triggers
    EICRA = (1 << ISC01);                       // INT0: falling edge
    EICRB = (1 << ISC51) | (1 << ISC71);        // INT5, INT7: falling edge
    EIMSK |= (1 << INT0) | (1 << INT5) | (1 << INT7);
}

// Toggle direction
ISR(INT0_vect) {
    PORTD ^= (1 << PIND1);
    PORTD ^= (1 << PIND2);
}

// Speed up
ISR(INT5_vect) {
    if (OCR3A + 10 < ICR3)
        OCR3A += 10;
}

// Speed down
ISR(INT7_vect) {
    if (OCR3A > 10)
        OCR3A -= 10;
}

int main(void) {
    pwm_init();
    pwm_set_duty(128);      // ~25% initial duty cycle
    interrupts_init();
    sei();

    while (1) {
        // everything handled by interrupts
    }
}
