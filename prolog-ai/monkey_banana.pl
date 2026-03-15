% Monkey and Banana — classic AI planning problem.
%
% A monkey is at the door, a banana hangs from the ceiling in the center.
% The monkey can: walk, push a box, climb on the box, and grab the banana.
% Can the monkey get the banana?
%
% State: state(MonkeyPos, MonkeyOn, HasBanana, BoxPos)
%
% Run: swipl -g "main" -t halt monkey_banana.pl

% Actions: step(StateBefore, Action, StateAfter)
step(state(under_banana, on_box, try_banana, under_banana),
     grab,
     state(under_banana, on_box, has_banana, under_banana)).

step(state(Pos, on_floor, Want, Pos),
     climb,
     state(Pos, on_box, Want, Pos)).

step(state(Pos1, on_floor, Want, Pos1),
     push(Pos1, Pos2),
     state(Pos2, on_floor, Want, Pos2)).

step(state(Pos1, on_floor, Want, BoxPos),
     walk(Pos1, Pos2),
     state(Pos2, on_floor, Want, BoxPos)).

% Goal: monkey has the banana
obtain(state(_, _, has_banana, _)).
obtain(State1) :-
    step(State1, Action, State2),
    format("  ~w  -->  ~w~n", [Action, State2]),
    obtain(State2).

solve(MonkeyPos, BoxPos) :-
    format("~nStarting: monkey at ~w, box at ~w~n~n", [MonkeyPos, BoxPos]),
    obtain(state(MonkeyPos, on_floor, try_banana, BoxPos)),
    format("~nThe monkey got the banana!~n").

main :-
    solve(at_door, at_window).
