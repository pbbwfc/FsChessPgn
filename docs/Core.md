
[Home](https://pbbwfc.github.io/FsChessPgn)  [Types](https://pbbwfc.github.io/FsChessPgn/Types)  [Core Functions](https://pbbwfc.github.io/FsChessPgn/Core)  [PGN Functions](https://pbbwfc.github.io/FsChessPgn/Pgn)  [WinForms](https://pbbwfc.github.io/FsChessPgn/winforms)

# Core Chess Functions

## Square

The following Square related functions are provided in the module Square:

| Function        | Type                            | Description                                                                         |
|:----------------|:--------------------------------|:------------------------------------------------------------------------------------|
| ToFile          | Square -> File                  | Gets the File for a Square                                                          |
| ToRank          | Square -> Rank                  | Gets the Rank for a Square                                                          |

## Piece

The following Piece related functions are provided in the module Piece:

| Function        | Type                            | Description                                                                         |
|:----------------|:--------------------------------|:------------------------------------------------------------------------------------|
| ToStr           | Piece -> string                 | Gets the string symbol for a Piece                                                  |
| ToPlayer        | Piece -> Player                 | Gets the player for a Piece                                                         |

## Board

The following Board related functions are provided in the module Board:

| Function        | Type                            | Description                                                                         |
|:----------------|:--------------------------------|:------------------------------------------------------------------------------------|
| FromStr         | string -> Brd                   | Create a new Board given a FEN string                                               |
| ToStr           | Brd -> string                   | Create a FEN string from this Board                                                 |
| Start           | Brd                             | The starting Board at the beginning of a game                                       |
| AllMoves        | Brd -> Move list                | Gets all legal moves for this Board                                                 |
| PossMoves       | Brd -> Square -> Move list      | Gets all possible moves for this Board from the specified Square                    |
| Push            | Move -> Brd -> Brd              | Make an encoded Move for this Board and return the new Board                        |
| PushSAN         | string -> Brd -> Brd            | Make a SAN Move such as Nf3 for this Board and return the new Board                 |
| IsCheck         | Brd -> bool                     | Is there a check on the Board                                                       |
| IsCheckMate     | Brd -> bool                     | Is the current position on the Board checkmate?                                     |
| IsStaleMate     | Brd -> bool                     | Is the current position on the Board stalemate?                                     |
| SquareAttacked  | Square -> Player -> Brd -> bool | Is the Square attacked by the specified Player for this Board                       |
| SquareAttackers | Square -> Player -> Brd -> bool | The Squares that attack the specified Square by the specified Player for this Board |
| ToPng           | string -> bool -> Brd -> unit   | Creates a PNG image ith specified name, flipped if specified for the given Board    |
| Print           | Brd -> unit                     | Prints an ASCII version of this Board                                               |

## Move

The following Move related functions are provided in the module Move:

| Function      | Type                  | Description                                                       |
|:--------------|:----------------------|:------------------------------------------------------------------|
| From          | Move -> Square        | Get the source Square for an encoded Move                         |
| To            | Move -> Square        | Get the target Square for an encoded Move                         |
| PromPcTp      | Move -> PieceType     | Get the promoted PieceType for an encoded Move                    |
| FromSan       | Brd -> string -> Move | Get an encoded move from a SAN string such as Nf3 for this Board  |
| FromUci       | Brd -> string -> Move | Get an encoded move from a UCI string such as g1f3 for this Board |
| ToUci         | Move -> string        | Get the UCI string such as g1f3 for a move                        |
| TopMove       | Brd -> Move -> pMove  | Get the pMove for a move for this board                           |
| ToSan         | Brd -> Move -> string | Get the SAN string such as Nf3 for a move for this board          |

## Game

The following Game related functions are provided in the module Game:

| Function        | Type                                         | Description                                                                         |
|:----------------|:---------------------------------------------|:------------------------------------------------------------------------------------|
| Start           | Game                                         | The starting Game with no moves                                                     |
| PushSAN         | string -> Game -> Game                       | Make a SAN Move such as Nf3 for this Game and return the new Game                   |
| Pop             | Game -> Game                                 | Pops a move of the end for this Game and return the new Game                        |
| MoveStr         | MoveTextEntry -> string                      | Gets a single move as a string given one of the list from Game.MoveText             |
| NAGStr          | NAG -> string                                | Gets a NAG as a string such as ?? given one from the list in Game.MoveText          |
| MovesStr        | MoveTextEntry list -> string                 | Gets the moves text as a string given the Game.MoveText                             |
| GetaMoves       | Game -> Game                                 | Gets the aMoves for the Game                                                        |
| AddMv           | Game -> int list -> pMove -> Game * int list | Adds a pMove to the Game given its address                                          |
| AddRav          | Game -> int list -> pMove -> Game * int list | Adds a RAV to the Game given the pMove is contains and its address                  |
