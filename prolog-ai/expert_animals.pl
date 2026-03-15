% Animal Expert System — identifies animals by asking yes/no questions.
%
% Classifies animals based on body length, fur pattern, and size category.
% Uses a dynamic fact database to remember user answers.
%
% Run: swipl -g "main" -t halt expert_animals.pl

:- dynamic known/3.

main :-
    nl, write('=== Animal Expert System ==='), nl,
    write('Answer questions with "y" or "n".'), nl, nl,
    (   identify(Animal)
    ->  format("~nI think it is: ~w~n", [Animal])
    ;   nl, write('Sorry, I could not identify the animal.'), nl
    ),
    clear_facts.

identify(elephant) :-
    it_is(large_animal),
    positive(body_length, 'more than 200 cm'),
    positive(has, 'trunk'),
    positive(color, 'gray').

identify(whale) :-
    it_is(large_animal),
    positive(body_length, 'more than 200 cm'),
    positive(lives_in, 'water'),
    positive(color, 'blue-gray').

identify(giraffe) :-
    it_is(large_animal),
    positive(body_length, 'more than 200 cm'),
    positive(has, 'long neck'),
    positive(has, 'spots').

identify(tiger) :-
    it_is(large_animal),
    positive(body_length, 'more than 55 cm'),
    positive(has, 'striped fur'),
    positive(color, 'orange').

identify(cheetah) :-
    it_is(small_animal),
    positive(body_length, 'more than 55 cm'),
    positive(has, 'striped fur'),
    positive(has, 'long tail'),
    positive(color, 'spotted').

identify(zebra) :-
    it_is(large_animal),
    positive(body_length, 'more than 75 cm'),
    positive(has, 'striped fur'),
    positive(has, 'hooves').

identify(penguin) :-
    it_is(small_animal),
    positive(body_length, 'less than 55 cm'),
    positive(has, 'black and white coloring'),
    positive(lives_in, 'cold climate').

identify(cat) :-
    it_is(small_animal),
    positive(has, 'soft fur'),
    positive(body_length, 'less than 55 cm'),
    positive(has, 'retractable claws').

identify(dog) :-
    it_is(small_animal),
    positive(has, 'tail'),
    positive(body_length, 'less than 75 cm'),
    positive(has, 'loyalty to humans').

it_is(large_animal) :- positive(size, large), !.
it_is(small_animal) :- positive(size, small), !.

% Ask the user, cache the answer
positive(Attr, Val) :-
    known(yes, Attr, Val), !.
positive(Attr, Val) :-
    \+ known(_, Attr, Val),
    ask(Attr, Val).

ask(Attr, Val) :-
    format("  ~w: ~w? (y/n) ", [Attr, Val]),
    read_string(user_input, 1, _, Reply),
    skip_line,
    (   Reply = "y"
    ->  assertz(known(yes, Attr, Val))
    ;   assertz(known(no, Attr, Val)), fail
    ).

clear_facts :-
    retractall(known(_, _, _)).
