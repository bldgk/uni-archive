# Prolog AI

Classical AI problems implemented in Prolog (2014), cleaned up for SWI-Prolog.

## Programs

**Monkey and Banana** (`monkey_banana.pl`) — AI planning/search problem:

A monkey must figure out how to reach a banana hanging from the ceiling by walking, pushing a box, climbing it, and grabbing.

```
$ swipl -g "main" -t halt monkey_banana.pl

Starting: monkey at at_door, box at at_window

  walk(at_door,at_window)  -->  state(at_window,on_floor,try_banana,at_window)
  push(at_window,under_banana)  -->  state(under_banana,on_floor,try_banana,under_banana)
  climb  -->  state(under_banana,on_box,try_banana,under_banana)
  grab   -->  state(under_banana,on_box,has_banana,under_banana)

The monkey got the banana!
```

**Graph Search** (`graph_search.pl`) — DFS and BFS on undirected graphs:

```
         a ─── b
         │╲   ╱│
         │  o   │
         │╱   ╲│
         d ─── c
```

```
$ swipl -g "main" -t halt graph_search.pl

DFS paths from a to d:
  [a,b,c,d]
  [a,d]
  [a,o,d]
  ...

BFS paths from a to d:
  [a,d]          ← shortest first
  [a,o,d]
  [a,b,c,d]
  ...
```

**Animal Expert System** (`expert_animals.pl`) — interactive classification:

Asks yes/no questions about an animal's traits and identifies it.

```
$ swipl -g "main" -t halt expert_animals.pl

=== Animal Expert System ===
  size: large? (y/n) y
  body_length: more than 200 cm? (y/n) y
  has: trunk? (y/n) y
  color: gray? (y/n) y

I think it is: elephant
```

**Brainfuck Interpreter** (`brainfuck.pl`) — a Turing-complete esoteric language interpreter written in a logic language:

```
$ swipl -g "main" -t halt brainfuck.pl

Program: Hello World!
Output:  Hello World!

Program: Print ABCDE
Output:  ABCDE
```

## Requires

```bash
brew install swi-prolog
```
