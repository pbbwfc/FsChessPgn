
[Home](https://pbbwfc.github.io/FsChessPgn)  [Types](https://pbbwfc.github.io/FsChessPgn/Types)  [Core Functions](https://pbbwfc.github.io/FsChessPgn/Core)  [PGN Functions](https://pbbwfc.github.io/FsChessPgn/Pgn)

# PGN reading and writing

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