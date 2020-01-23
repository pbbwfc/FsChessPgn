
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


You first create an index for the PGN file using _CreateIndex_. This creates a file with teh same name as the PGN file but with a ".bin" extension. This only needs to be done once until the PGN file is changed.

You then load this index using _GetIndex_ and then load the games but with their index for easier processing using _ReadIndexListFromFile_.

You can then carry out multiple quick searches againse a specified Board using _FastFindBoard_.







TODO
