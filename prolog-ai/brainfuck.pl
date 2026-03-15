% Brainfuck Interpreter in Prolog
%
% A Turing-complete esoteric language interpreter written in a logic language.
% Supports all 8 Brainfuck commands: > < + - . , [ ]
%
% Run: swipl -g "main" -t halt brainfuck.pl

:- use_module(library(lists)).
:- discontiguous bf_run/1.

main :-
    write('=== Brainfuck Interpreter ==='), nl, nl,

    % "Hello World!" in Brainfuck
    Program = "++++++++[>++++[>++>+++>+++>+<<<<-]>+>+>->>+[<]<-]>>.>---.+++++++..+++.>>.<-.<.+++.------.--------.>>+.>++.",
    write('Program: Hello World!'), nl,
    write('Output:  '),
    bf_run(Program),
    nl, nl,

    % Simple counter: prints "ABCDE"
    Program2 = "++++++++[>++++++++<-]>+.+.+.+.+.",
    write('Program: Print ABCDE'), nl,
    write('Output:  '),
    bf_run(Program2),
    nl.

% ── Interpreter ─────────────────────────────────────────



:- dynamic cell/2.
:- dynamic pointer/1.

bf_run(Program) :-
    retractall(cell(_, _)),
    retractall(pointer(_)),
    assertz(pointer(0)),
    string_codes(Program, Codes),
    bf_exec(Codes, []).

bf_exec([], _).
bf_exec([Code|Rest], Stack) :-
    bf_step(Code, Rest, Stack, Rest2, Stack2),
    bf_exec(Rest2, Stack2).

% Get/set cell value
get_cell(Val) :-
    pointer(P),
    (cell(P, V) -> Val = V ; Val = 0).
set_cell(Val) :-
    pointer(P),
    retractall(cell(P, _)),
    assertz(cell(P, Val)).

% + increment
bf_step(0'+, Rest, Stack, Rest, Stack) :-
    get_cell(V), V1 is (V + 1) mod 256, set_cell(V1).

% - decrement
bf_step(0'-, Rest, Stack, Rest, Stack) :-
    get_cell(V), V1 is (V - 1 + 256) mod 256, set_cell(V1).

% > move right
bf_step(0'>, Rest, Stack, Rest, Stack) :-
    retract(pointer(P)), P1 is P + 1, assertz(pointer(P1)).

% < move left
bf_step(0'<, Rest, Stack, Rest, Stack) :-
    retract(pointer(P)), P1 is P - 1, assertz(pointer(P1)).

% . output
bf_step(0'., Rest, Stack, Rest, Stack) :-
    get_cell(V), char_code(C, V), write(C).

% , input
bf_step(0',, Rest, Stack, Rest, Stack) :-
    get_char(C), char_code(C, V), set_cell(V).

% [ loop start
bf_step(0'[, Rest, Stack, Rest2, Stack2) :-
    get_cell(V),
    (   V =:= 0
    ->  skip_forward(Rest, 1, Rest2), Stack2 = Stack
    ;   Rest2 = Rest, Stack2 = [Rest|Stack]
    ).

% ] loop end
bf_step(0'], Rest, Stack, Rest2, Stack2) :-
    get_cell(V),
    (   V =\= 0
    ->  Stack = [LoopStart|_], Rest2 = LoopStart, Stack2 = Stack
    ;   Stack = [_|Stack2], Rest2 = Rest
    ).

% Skip any other character
bf_step(_, Rest, Stack, Rest, Stack).

% Skip forward past matching ]
skip_forward([0']|Rest], 1, Rest) :- !.
skip_forward([0']|Rest], N, Out) :- N > 1, N1 is N - 1, skip_forward(Rest, N1, Out).
skip_forward([0'[|Rest], N, Out) :- N1 is N + 1, skip_forward(Rest, N1, Out).
skip_forward([_|Rest], N, Out) :- skip_forward(Rest, N, Out).
