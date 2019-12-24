# FsChessPgn: a library for chess scripting using F#

## Introduction

FsChessPgn is a library for scripting in F# that provides chess facilities such as move generation, move validation and support for common chess formats such as FEN and PGN.

It is intended to be similar to the excellent [python-chess](https://github.com/niklasf/python-chess). This library is useful if you want to use F# rather than Python.

This is the Scholar's mate in FsChessPgn:

```fsharp
#load "setup.fsx"
open FsChess

let board = Board.Start

let mvs = board|>Board.AllMoves

let nbd = 
    board
    |>Board.PushSAN "e4"
    |>Board.PushSAN "e5"
    |>Board.PushSAN "Qh5"
    |>Board.PushSAN "Nc6"
    |>Board.PushSAN "Bc4"
    |>Board.PushSAN "Nf6"
    |>Board.PushSAN "Qxf7"

let ismate = nbd|>Board.IsCheckMate
```

This produces these results in F# Interactive:

```
val board : FsChessPgn.Types.Brd =
  rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1
val mvs : FsChessPgn.Types.Move list =
  [h2h4; g2g4; f2f4; e2e4; d2d4; c2c4; b2b4; a2a4; h2h3; g2g3; f2f3; e2e3;
   d2d3; c2c3; b2b3; a2a3; g1h3; g1f3; b1c3; b1a3]
val nbd : FsChessPgn.Types.Brd =
  r1bqkb1r/pppp1Qpp/2n2n2/4p3/2B1P3/8/PPPP1PPP/RNB1K1NR b KQkq - 0 4
val ismate : bool = true
```

## Detailed Documentation

* [Core](https://github.com/pbbwfc/FsChessPgn) - *TODO*
* [PGN parsing and writing](https://github.com/pbbwfc/FsChessPgn) - *TODO*

## Features

* Witten using .NET Core and designed to use the latest version of F# Interactive.

* Generates images of a Board in PNG format. We can use this script:

```fsharp
#load "setup.fsx"
open FsChess

let board = "r1bqkb1r/pppp1Qpp/2n2n2/4p3/2B1P3/8/PPPP1PPP/RNB1K1NR b KQkq - 0 4"|>Board.FromStr

do board|>Board.ToPng "c:/temp/fools.png" false
```
This produces this image:

![fools](fools.png)



* Can make moves given a Board. Can both make and unmake moves given a Game.

```fsharp
#load "setup.fsx"
open FsChess

let board = Board.Start

let new_board = board|>Board.PushSAN "e4"|>Board.PushSAN "e5"

//TODO:Game
```

* Show a simple ASCII board - *TODO*

* Detects checkmates, stalemates and draws by insufficient material.- *TODO*

* Detects checks and attacks.- *TODO*

* Parses and creates SAN representation of moves.- *TODO*

* Parses and creates FENs- *TODO*

* Reads and writes PGNs. Supports headers, comments, NAGs and a tree of variations. - *TODO*

## Installing

Please just download a copy of the code from GitHub and then start using any of the sample scripts. 