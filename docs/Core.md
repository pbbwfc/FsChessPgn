
[Home](https://pbbwfc.github.io/FsChessPgn)  [Types](https://pbbwfc.github.io/FsChessPgn/Types)  [Core Functions](https://pbbwfc.github.io/FsChessPgn/Core)  [PGN Functions](https://pbbwfc.github.io/FsChessPgn/Pgn)

# Core Chess Functions

## Board

The following Board related functions are provided in the module Board:

| Function        | Type                            | Description                                                                         |
|:----------------|:--------------------------------|:------------------------------------------------------------------------------------|
| FromStr         | string -> Brd                   | Create a new Board given a FEN string                                               |
| ToStr           | Brd -> string                   | Create a FEN string from this Board                                                 |
| Start           | Brd                             | The starting Board at the beginning of a game                                       |
| AllMoves        | Brd -> Move list                | Gets all legal moves for this Board                                                 |
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
| FromSan       | Brd -> string -> Move | Get an encoded move from a SAN string such as Nf3 for this Board  |
| FromUci       | Brd -> string -> Move | Get an encoded move from a UCI string such as g1f3 for this Board |
| ToUci         | Move -> string        | Get the UCI string such as g1f3 for a move                        |
| ToSan         | Brd -> Move -> string | Get the SAN string such as Nf3 for a move for this board          |

## Game

The following Game related functions are provided in the module Game:

| Function        | Type                            | Description                                                                         |
|:----------------|:--------------------------------|:------------------------------------------------------------------------------------|
| Start           | Game                            | The starting Game with no moves                                                     |
| PushSAN         | string -> Game -> Game          | Make a SAN Move such as Nf3 for this Game and return the new Game                   |
| Pop             | Game -> Game                    | Pops a move of the end for this Game and return the new Game                        |
| MovesStr        | MoveTextEntry list -> string    | Gets the moves text as a string given the Game.MoveText                             |
