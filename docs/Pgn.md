
[Home](https://pbbwfc.github.io/FsChessPgn)  [Types](https://pbbwfc.github.io/FsChessPgn/Types)  [Core Functions](https://pbbwfc.github.io/FsChessPgn/Core)  [PGN Functions](https://pbbwfc.github.io/FsChessPgn/Pgn)  [WinForms](https://pbbwfc.github.io/FsChessPgn/winforms)

# PGN reading, writing and statistics

The PGN related facilities are included in the namespace _FSChess.Pgn_ in the module _Games_. 

## Reading

You can either load a set of Games in its entirety into an F# List or can load them lazily using an F# Sequence. These are the relevant functions:

| Function         | Type                            | Description                                                                         |
|:-----------------|:--------------------------------|:------------------------------------------------------------------------------------|
| ReadListFromFile | string -> Game list             | Given a PGN file name, read this into a list of Games                               |
| ReadSeqFromFile  | string -> Seq<Game>             | Given a PGN file name, read this into a lazy sequence of Games                      |

Here is some sample code illustrating how to use these options:

```fsharp

#load "setup.fsx"
open FsChess.Pgn

let pgn = __SOURCE_DIRECTORY__ + "/data/pgn/kasparov-deep-blue-1997.pgn"

//loads all games
let gml = Games.ReadListFromFile(pgn)
let count = gml.Length

//does not load any game yet
let gms = Games.ReadSeqFromFile(pgn)

//Just loads first game
let first_game = gms|>Seq.head
let event = first_game.Event

```

This produces these results:

```
val gml : FsChess.Types.Game list =
  [moves: ...Rd5 44. f6 Rd1 45. g7 1-0; moves: ...Qb6+ 44. Kf1 Rb8 45. Ra6 1-0;
   moves: ...Kh7 47. Ra3 Kh8 48. Ra6 1/2-1/2;
   moves: ...Re4 55. a4 Kb3 56. Kc1 1/2-1/2;
   moves: ...48. g6 Kxb5 49. g7 Kb4 1/2-1/2;
   moves: ...exf5 18. Rxe7 Bxe7 19. c4 1-0]
val count : int = 6
val gms : seq<FsChess.Types.Game>
val first_game : FsChess.Types.Game = moves: ...Rd5 44. f6 Rd1 45. g7 1-0
val event : string = "IBM Man-Machine, New York USA"
```

## Writing

When you have a list of games you can write it to a file using this function:


| Function         | Type                            | Description                                                                         |
|:-----------------|:--------------------------------|:------------------------------------------------------------------------------------|
| WriteFile        | string -> Game list-> unit      | Given a PGN file name, write this list of Games to that file                        |

Here is some sample code illustrating how to use this function:

```fsharp

#load "setup.fsx"
open FsChess.Pgn

let pgn = __SOURCE_DIRECTORY__ + "/data/pgn/kasparov-deep-blue-1997.pgn"

//loads all games
let gml = Games.ReadListFromFile(pgn)

let gml2 = gml.[0..1]
let pgn2 = "c:/temp/test.pgn"

gml2|>Games.WriteFile pgn2

```

This produces these results:

```
val gml : FsChess.Types.Game list =
  [moves: ...Rd5 44. f6 Rd1 45. g7 1-0; moves: ...Qb6+ 44. Kf1 Rb8 45. Ra6 1-0;
   moves: ...Kh7 47. Ra3 Kh8 48. Ra6 1/2-1/2;
   moves: ...Re4 55. a4 Kb3 56. Kc1 1/2-1/2;
   moves: ...48. g6 Kxb5 49. g7 Kb4 1/2-1/2;
   moves: ...exf5 18. Rxe7 Bxe7 19. c4 1-0]
val gml2 : FsChess.Types.Game list =
  [moves: ...Rd5 44. f6 Rd1 45. g7 1-0; moves: ...Qb6+ 44. Kf1 Rb8 45. Ra6 1-0]
val pgn2 : string = "c:/temp/test.pgn"
val it : unit = ()
```

A file containing the first 2 games is created as _test.pgn_.

## Statistics

Working with individual games from a PGN file is done using the finctions in _FsChess_ in module _Game_. In addition functions relating to all the games in a PGN file are documented here.

For small PGN files we provide a simple way to search for a board using the function _FindBoard_ in the module _Games_. The results can then be summarised using _Get_ from the module _Stats_.

| Function         | Type                                        | Description                                                |
|:-----------------|:--------------------------------------------|:-----------------------------------------------------------|
| FindBoard        | Brd -> string -> (int * Game * string) list | Finds the Games that containing the specified Board        |
| Get              | (int * Game * string) list -> BrdStats      | Get Statistics for the Board                               |

This is illustrated by this code:


```fsharp

#load "setup.fsx"
open FsChess.Pgn
open FsChess

let pgn = __SOURCE_DIRECTORY__ + "/data/pgn/french_Qd7.pgn"

//set seach board
let fen = "r1b1kbnr/1pq2ppp/p3p3/8/2BN4/8/PPP2PPP/R1BQ1RK1 w kq - 1 11"
let bd = fen|>Board.FromStr

let gmmvl = Games.FindBoard bd pgn

let len = gmmvl.Length

let stats = gmmvl|>Stats.Get

```

This produces these results:

```
val pgn : string = "D:\GitHub\FsChessPgn\scripts/data/pgn/french_Qd7.pgn"
val fen : string =
  "r1b1kbnr/1pq2ppp/p3p3/8/2BN4/8/PPP2PPP/R1BQ1RK1 w kq - 1 11"
val bd : FsChess.Types.Brd =
  r1b1kbnr/1pq2ppp/p3p3/8/2BN4/8/PPP2PPP/R1BQ1RK1 w kq - 1 11
val gmmvl : (int * FsChess.Types.Game * string) list =
  [(2, moves: ...Kxh8 45. Qxg6 Rc4 46. Qe8+ 1/2-1/2, "Qe2");
   (8, moves: ...Rxb7 38. Nxb7 Kxb7 39. g4 1-0, "Bb3");
   (11, moves: ...61. Bf3 Kd7 62. Bd5 Kc7 1/2-1/2, "Bb3");
   (22, moves: ...18. Qg2 Ne5 19. Be2 Bc5 1/2-1/2, "Qe2");
   (28, moves: ...d4 51. Nxf3 d3 52. Rb4+ 1-0, "Bd3");
   (30, moves: ...Rfd8 21. Rg4 g6 22. Rxg6+ 1-0, "Bb3");
   (32, moves: ...Qxc4 19. Bxc4 Bf4 20. Bxf4 1/2-1/2, "Qe2");
   (33, moves: ...Qe7 39. f4 exf3 40. Qh4+ 1-0, "Bb3");
   (40, moves: ...34. Kg2 Rd2+ 35. Kh1 Rd1+ 1/2-1/2, "Bd3");
   (44, moves: ...25. Rxe6 Qxb3 26. Qxg4 Rxd4 0-1, "Bb3");
   (46, moves: ...26. Ba7 Qg3 27. Bf2 Qb8 1/2-1/2, "Qe2");
   (48, moves: ...Kd7 36. Rc1 Ra8 37. Rxa8 1/2-1/2, "Qe2");
   (54, moves: ...40. Rg2 Rf4 41. Re2 Rf1+ 0-1, "Bb3");
   (61, moves: ...60. c6 bxc6 61. Ra5 Kxf2 0-1, "Bb3");
   (64, moves: ...40. Kd4 Ra6 41. Kd3 Kxc5 0-1, "Bb3");
   (70, moves: ...Ke6 45. Nc7+ Kd6 46. Na6 1/2-1/2, "Qe2");
   (72, moves: ...b3 62. Bxb3 axb3 63. axb3 1-0, "Qe2");
   (74, moves: ...55. Rxc5 Kxc5 56. h4 e5 0-1, "Bb3")]
val len : int = 18
val stats : FsChess.Types.BrdStats =
  |  Move  | Count | Percent | WhiteWins |  Draws  | BlackWins |  Score  | DrawPc  |
  |--------|-------|---------|-----------|---------|-----------|---------|---------|
  | Bb3    |  9    |  50.0%  |  3        |  1      |  5        |  38.9%  |  11.1%  |
  | Qe2    |  7    |  38.9%  |  1        |  6      |  0        |  57.1%  |  85.7%  |
  | Bd3    |  2    |  11.1%  |  1        |  1      |  0        |  75.0%  |  50.0%  |
  |--------|-------|---------|-----------|---------|-----------|---------|---------|
  | TOTAL  |  18   |  100.0% |  5        |  8      |  5        |  50.0%  |  44.4%  |

```


For larger PGN files the above approach is slow. Instead a much quicker process uses these functions:

| Function              | Type                                                                                       | Description                                                    |
|:----------------------|:-------------------------------------------------------------------------------------------|:---------------------------------------------------------------|
| CreateIndex           | string -> unit                                                                             | Creates index on PGN for fast searches                         |
| GetIndex              | string -> Dictionary<Set<Square>,int list>                                                 | Get Statistics for the Board                                   |
| ReadIndexListFromFile | string -> (int * Game) list                                                                | Get a list of index * Game from a file                         |
| FastFindBoard         | Brd -> Dictionary<Set<Square>,int list> -> (int * Game) list -> (int * Game * string) list | Does a fast search using the Index and the index list of Games |


You first create an index for the PGN file using _CreateIndex_. This creates a file with the same name as the PGN file, but with a ".bin" extension. This only needs to be done once until the PGN file is changed.

You then load this index using _GetIndex_ and then load the games but with their index for easier processing using _ReadIndexListFromFile_.

You can then carry out multiple quick searches against different Boards using _FastFindBoard_.

The results can be summarised in the same way as with the slower approach.

This is illustrated by this code:


```fsharp

#load "setup.fsx"
open FsChess.Pgn
open FsChess

let pgn = __SOURCE_DIRECTORY__ + "/data/pgn/french_Qd7.pgn"
//only needs to be done once
pgn|>Games.CreateIndex

//set seach board
let fen = "r1b1kbnr/1pq2ppp/p3p3/8/2BN4/8/PPP2PPP/R1BQ1RK1 w kq - 1 11"
let bd = fen|>Board.FromStr

let indx = pgn|>Games.GetIndex
let igms = pgn|>Games.ReadIndexListFromFile

let gmmvl = Games.FastFindBoard bd indx igms

let len = gmmvl.Length

let stats = gmmvl|>Stats.Get

```

This produces these results:

```
val pgn : string = "D:\GitHub\FsChessPgn\scripts/data/pgn/french_Qd7.pgn"
val fen : string =
  "r1b1kbnr/1pq2ppp/p3p3/8/2BN4/8/PPP2PPP/R1BQ1RK1 w kq - 1 11"
val bd : FsChess.Types.Brd =
  r1b1kbnr/1pq2ppp/p3p3/8/2BN4/8/PPP2PPP/R1BQ1RK1 w kq - 1 11
val indx :
  System.Collections.Generic.Dictionary<Set<FsChess.Types.Square>,int list> =
  dict
    [(set [a2], []); (set [b2], []); (set [c2], []); (set [d2], [50; 15; 9]);
     (set [e2],
      [74; 73; 72; 71; 70; 69; 68; 67; 66; 65; 64; 63; 62; 61; 60; 59; 58; 57;
       56; 55; 54; 53; 52; 51; 49; 48; 47; 46; 45; 44; 43; 42; 41; 40; 39; 38;
       37; 36; 35; 34; 33; 32; 31; 30; 29; 28; 27; 26; 25; 24; 23; 22; 21; 20;
       19; 18; 17; 16; 14; 13; 12; 11; 10; 8; 7; 6; 5; 4; 3; 2; 1; 0]);
     (set [f2], []); (set [g2], []); (set [h2], []); (set [a7], []);
     (set [b7], []); (set [c7], []); (set [d7], []); (set [e7], []);
     (set [f7], []); (set [g7], []); (set [h7], []); (set [a2; b2], []);
     (set [a2; c2], []); (set [a2; d2], []); (set [a2; e2], []);
     (set [a2; f2], []); (set [a2; g2], []); (set [a2; h2], []);
     ...
val igms : (int * FsChess.Types.Game) list =
  [(0, moves: ...Rb1 78. Kc7 Rxb7+ 79. Kxb7 1/2-1/2);
   (1, moves: ...Kh6 99. Rf6+ Kxg7 100. f8=Q+ 1-0);
   (2, moves: ...Kxh8 45. Qxg6 Rc4 46. Qe8+ 1/2-1/2);
   (3, moves: ...36. h3 hxg4 37. hxg4 Rg7 0-1);
   (4, moves: ...35. Rc7 Bxg2+ 36. Ke1 Bf3 0-1);
   (5, moves: ...Qb1+ 47. Kh2 Qg6 48. Qh8# 1-0);
   (6, moves: ...29. Re2 Rd8 30. Re4 Rd2 1/2-1/2);
   ...
val gmmvl : (int * FsChess.Types.Game * string) list =
  [(2, moves: ...Kxh8 45. Qxg6 Rc4 46. Qe8+ 1/2-1/2, "Qe2");
   (8, moves: ...Rxb7 38. Nxb7 Kxb7 39. g4 1-0, "Bb3");
   (11, moves: ...61. Bf3 Kd7 62. Bd5 Kc7 1/2-1/2, "Bb3");
   (22, moves: ...18. Qg2 Ne5 19. Be2 Bc5 1/2-1/2, "Qe2");
   (28, moves: ...d4 51. Nxf3 d3 52. Rb4+ 1-0, "Bd3");
   (30, moves: ...Rfd8 21. Rg4 g6 22. Rxg6+ 1-0, "Bb3");
   (32, moves: ...Qxc4 19. Bxc4 Bf4 20. Bxf4 1/2-1/2, "Qe2");
   (33, moves: ...Qe7 39. f4 exf3 40. Qh4+ 1-0, "Bb3");
   (40, moves: ...34. Kg2 Rd2+ 35. Kh1 Rd1+ 1/2-1/2, "Bd3");
   (44, moves: ...25. Rxe6 Qxb3 26. Qxg4 Rxd4 0-1, "Bb3");
   (46, moves: ...26. Ba7 Qg3 27. Bf2 Qb8 1/2-1/2, "Qe2");
   (48, moves: ...Kd7 36. Rc1 Ra8 37. Rxa8 1/2-1/2, "Qe2");
   (54, moves: ...40. Rg2 Rf4 41. Re2 Rf1+ 0-1, "Bb3");
   (61, moves: ...60. c6 bxc6 61. Ra5 Kxf2 0-1, "Bb3");
   (64, moves: ...40. Kd4 Ra6 41. Kd3 Kxc5 0-1, "Bb3");
   (70, moves: ...Ke6 45. Nc7+ Kd6 46. Na6 1/2-1/2, "Qe2");
   (72, moves: ...b3 62. Bxb3 axb3 63. axb3 1-0, "Qe2");
   (74, moves: ...55. Rxc5 Kxc5 56. h4 e5 0-1, "Bb3")]
val len : int = 18
val stats : FsChess.Types.BrdStats =
  |  Move  | Count | Percent | WhiteWins |  Draws  | BlackWins |  Score  | DrawPc  |
  |--------|-------|---------|-----------|---------|-----------|---------|---------|
  | Bb3    |  9    |  50.0%  |  3        |  1      |  5        |  38.9%  |  11.1%  |
  | Qe2    |  7    |  38.9%  |  1        |  6      |  0        |  57.1%  |  85.7%  |
  | Bd3    |  2    |  11.1%  |  1        |  1      |  0        |  75.0%  |  50.0%  |
  |--------|-------|---------|-----------|---------|-----------|---------|---------|
  | TOTAL  |  18   |  100.0% |  5        |  8      |  5        |  50.0%  |  44.4%  |

```




TODO
