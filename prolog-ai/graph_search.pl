% Graph Search — Depth-First Search and Breadth-First Search in Prolog.
%
% Finds paths between nodes in an undirected graph using DFS and BFS.
%
% Run: swipl -g "main" -t halt graph_search.pl

% Graph edges (undirected)
edge(a, b). edge(b, c). edge(c, d). edge(d, a).
edge(a, o). edge(b, o). edge(c, o). edge(d, o).

connected(A, B) :- edge(A, B) ; edge(B, A).

% ── Depth-First Search ──────────────────────────────────

dfs(Goal, [Goal|Path], [Goal|Path]).
dfs(Goal, [Current|Path], Result) :-
    connected(Next, Current),
    \+ member(Next, Path),
    dfs(Goal, [Next, Current|Path], Result).

path_dfs(Start, End, Path) :-
    dfs(Start, [End], Path).

% ── Breadth-First Search ────────────────────────────────

bfs([[Goal|Path]|_], Goal, [Goal|Path]).
bfs([[Current|Path]|Paths], Goal, Solution) :-
    findall(
        [Next, Current|Path],
        (connected(Current, Next), \+ member(Next, [Current|Path])),
        NewPaths
    ),
    append(Paths, NewPaths, AllPaths),
    bfs(AllPaths, Goal, Solution).

path_bfs(Start, End, Path) :-
    bfs([[Start]], End, RevPath),
    reverse(RevPath, Path).

% ── Main ────────────────────────────────────────────────

main :-
    write('Graph: a-b, b-c, c-d, d-a, a-o, b-o, c-o, d-o'), nl, nl,

    write('DFS paths from a to d:'), nl,
    forall(path_dfs(a, d, P), format("  ~w~n", [P])),

    nl, write('BFS paths from a to d:'), nl,
    forall(path_bfs(a, d, P), format("  ~w~n", [P])),

    nl, write('BFS shortest path from a to o:'), nl,
    (   path_bfs(a, o, P2)
    ->  format("  ~w~n", [P2])
    ;   write("  no path found"), nl
    ).
