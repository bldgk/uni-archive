/*
 * ADC to 7-Segment Display
 *
 * Reads analog input via the ATmega8 ADC and displays the 4-digit
 * value on a multiplexed 7-segment display.
 *
 * Timer 2 overflow ISR handles display multiplexing — cycles through
 * 4 digits fast enough to appear simultaneous.
 * ADC interrupt fires when conversion is complete.
 *
 * Pinout:
 *   PORTD = 7-segment data (active low)
 *   PORTC = digit select (one-hot)
 *   ADC0  = analog input
 */

#include <avr/io.h>
#include <avr/interrupt.h>
#include <util/delay.h>

// 7-segment patterns (common anode, active low)
//                    0     1     2     3     4     5     6     7     8     9
const char SEG[] = {0x3F, 0x06, 0x5B, 0x4F, 0x66, 0x6D, 0x7D, 0x07, 0x7F, 0x6F};

volatile unsigned char current_digit = 0;
volatile unsigned int  adc_value = 0;

// ── Initialization ─────────────────────────────────

void init_ports(void) {
    DDRC  = 0xFF;           // digit select outputs
    PORTC = 0x00;
    DDRD  = 0xFF;           // segment data outputs
    PORTD = 0x00;
}

void init_timer2(void) {
    TIMSK |= (1 << TOIE2); // enable Timer 2 overflow interrupt
    TCCR2 |= (1 << CS21);  // prescaler /8
}

void init_adc(void) {
    ADCSRA |= (1 << ADEN)  // enable ADC
            | (1 << ADSC)   // start first conversion
            | (1 << ADPS2) | (1 << ADPS1)  // prescaler /64
            | (1 << ADIE); // enable ADC interrupt

    ADMUX &= ~((1 << REFS1) | (1 << REFS0));  // external AREF
}

// ── Interrupts ─────────────────────────────────────

ISR(ADC_vect) {
    adc_value = ADC;        // read 10-bit result (0-1023)
    ADCSRA |= (1 << ADSC); // start next conversion
}

ISR(TIMER2_OVF_vect) {
    PORTD = 0xFF;                        // blank all segments
    PORTC = (1 << current_digit);        // select current digit

    unsigned int val = adc_value;
    unsigned char digit;

    switch (current_digit) {
        case 0: digit = (val / 1000) % 10; break;  // thousands
        case 1: digit = (val / 100)  % 10; break;  // hundreds
        case 2: digit = (val / 10)   % 10; break;  // tens
        case 3: digit = val % 10;          break;   // ones
        default: digit = 0;
    }

    PORTD = ~SEG[digit];    // output pattern (active low)

    if (++current_digit > 3)
        current_digit = 0;
}

// ── Main ───────────────────────────────────────────

int main(void) {
    init_ports();
    init_timer2();
    init_adc();
    sei();                  // enable global interrupts

    while (1) {
        _delay_ms(50);
    }
}
