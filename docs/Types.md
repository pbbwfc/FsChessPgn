
[Home](https://pbbwfc.github.io/FsChessPgn)  [Types](https://pbbwfc.github.io/FsChessPgn/Types)  [Core Functions](https://pbbwfc.github.io/FsChessPgn/Core)  [PGN Functions](https://pbbwfc.github.io/FsChessPgn/Pgn)  [WinForms](https://pbbwfc.github.io/FsChessPgn/winforms)

# Chess Types

## Piece types

These are provided as an enumeration **PieceType** with these values:

| Label  | Value |
|:-------|:------|
| EMPTY  | 0     | 
| Pawn   | 1     | 
| Knight | 2     | 
| Bishop | 3     | 
| Rook   | 4     | 
| Queen  | 5     | 
| King   | 6     | 

## Pieces

These are for each colour and are provided as an enumeration **Piece** with these values:

| Label   | Value |
|:--------|:------|
| EMPTY   | 0     | 
| WPawn   | 1     | 
| WKnight | 2     | 
| WBishop | 3     | 
| WRook   | 4     | 
| WQueen  | 5     | 
| WKing   | 6     | 
| BPawn   | 9     | 
| BKnight | 10    | 
| BBishop | 11    | 
| BRook   | 12    | 
| BQueen  | 13    | 
| BKing   | 14    | 

## Player

These are for each colour and are provided as an enumeration **Player** with these values:

| Label   | Value |
|:--------|:------|
| White   | 0     | 
| Black   | 1     | 

## Game Result

These are provided as an enumeration **GameResult** with these values:

| Label     | Value |
|:----------|:------|
| Draw      |  0    | 
| WhiteWins |  1    | 
| BlackWins | -1    | 
| Open      |  9    | 

## Files

These are _aliased_ to a _short_, with values 0s to 7s for **FileA** to **FileH**. FILE_EMPTY is given a value 8s.

As an example, **FileC** equals 2s.

Also provided are:

FILES = [FileA; FileB; FileC; FileD; FileE; FileF; FileG; FileH]

and

FILE_NAMES = ["a"; "b"; "c"; "d"; "e"; "f"; "g"; "h"]

## Ranks

These are _aliased_ to a _short_, with values 7s to 0s for **Rank1** to **Rank8**. RANK_EMPTY is given a value 8s.

As an example, **Rank3** equals 5s. 

Also provided are:

RANKS = [Rank8; Rank7; Rank6; Rank5; Rank4; Rank3; Rank2; Rank1]

and

RANK_NAMES = ["8"; "7"; "6"; "5"; "4"; "3"; "2"; "1"]

## Squares

These are _aliased_ to a _short_, with values 0s to 63s for **A8** to **H1**. OUTOFBOUNDS is given a value 64s.

As an example, **A2** equals 48s. 

Also provided are:

SQUARES = [A8; B8; ... G1; H1] 

and

SQUARE_NAMES = ["A8; "B8"; ... "G1"; "H1"]

A utility function **Sq** is provided, defined as:

```fsharp
let Sq(f:File,r:Rank) :Square = r * 8s + f
```

## Moves

These are three types of move supported. The main one is _aliased_ to an _int_ and encodes details of the move.



## Castle Flags

These are a means of storing the castling writes for a board position. They are provided as an enumeration **CstlFlgs** with these values:

| Label      | Value |
|:-----------|:------|
| EMPTY      | 0     | 
| WhiteShort | 1     | 
| WhiteLong  | 2     |
| BlackShort | 4     |
| BlackLong  | 8     |
| All        | 15    |

## Bitboards

This is used internally for efficent storage of squares and moves and calculations of possible moves. They are provided as an enumeration **Bitboard** of 64-bit unsigned integers.

Here are some sample values:

| Label    | Value                 |
|:---------|:----------------------|
| A8       | 1UL                   | 
| H1       | 9223372036854775808UL | 
| FileA    | 72340172838076673UL   |
| Rank4    | 1095216660480UL       |

## Move Types

These are a means of storing the broad type of each move for a SAN move. They are provided as a simple discriminated union **MoveType** with these choices:

 - Simple
 - Capture
 - CastleKingSide
 - CastleQueenSide

## Move Annotations

These are a means of storing the annotation such as _??_ for a SAN move. They are provided as a simple discriminated union **MoveAnnotation** with these choices:

 - Brilliant
 - Good
 - Interesting
 - Dubious
 - Mistake
 - Blunder

## SAN Moves

These are a means of storing a move in SAN format such as _Nxg6+!_. They are provided as a record type **pMove** with these fields:

| Field         | Type              | Description                                 |
|:--------------|:------------------|:--------------------------------------------|
| Mtype         | MoveType          | The broad type of the move                  |
| TargetSquare  | Square            | The square to which you move the piece      |
| Piece         | PieceType option  | The piece moved, but it could be None       |
| OriginFile    | File option       | The file moved from, but it could be None   |
| OriginRank    | Rank option       | The rank moved from, but it could be None   |
| PromotedPiece | PieceType option  | The piece promoted to, but it could be None |
| IsCheck       | bool              | Does the move check the king?               |
| IsDoubleCheck | bool              | Does the move double check the king?        |
| IsCheckMate   | bool              | Does the move mate the king?                |
| Annotation    | MoveAnnotation    | Anny annotation such as ?? for the move     |

## Boards

These are a means of storing a position on board. They are provided as a record type **Brd** with these fields:

| Field             | Type              | Description                                 |
|:------------------|:------------------|:--------------------------------------------|
| PieceAt           | Piece list        | The pieces on each square                   |
| WtKingPos         | Square            | The square of the white king                |
| BkKingPos         | Square            | The square of the black king                |
| PieceTypes        | Bitboard list     | Keeps track of the piece types on the board |
| WtPrBds           | Bitboard          | Keeps track of the white pieces             |
| BkPrBds           | Bitboard          | Keeps track of the black pieces             |
| PieceLocationsAll | Bitboard          | Keeps track of all pieces                   |
| Checkers          | Bitboard          | Keeps track of all pieces giving check      |
| WhosTurn          | Player            | Holds the player whose turn it is           |
| CastleRights      | CstlFlgs          | Holds the castling rights still available   |
| EnPassant         | Square            | Holds the possible square to take e.p.      |
| Fiftymove         | int               | Keeps track of moves for draw in 50         |
| Fullmove          | int               | Keeps track of overall double moves         |

This also provides and Indexer so that you can get a piece on a square using code like:


```fsharp
let pc = board.[C3]
```

## Board Related Moves

These are a means of storing a move with the associated board position. They are provided as a record type **aMove** with these fields:

| Field             | Type              | Description                                 |
|:------------------|:------------------|:--------------------------------------------|
| PreBrd            | Brd               | The position before the move                |
| Mv                | Move              | The encoded move                            |
| PostBrd           | Brd               | The position after the move                 |

## Move Entries

These are a means of storing the various type of move entry in a game. They are provided as a complex discriminated union **MoveTextEntry** with these choices:

| Choice        | Type                                     | Description                                                                                             |
|:--------------|:-----------------------------------------|:--------------------------------------------------------------------------------------------------------|
| HalfMoveEntry | int option * bool * pMove * aMove option | The main option: optional move counter, whether a continuation, the SAN move and the board related move |
| CommentEntry  | string                                   | Holds a comment in a game                                                                               |
| GameEndEntry  | GameResult                               | Holds the result at the end of a game                                                                   |
| NAGEntry      | int                                      | Holds a NAG entry such as += in a game                                                                  |
| RAVEntry      | MoveTextEntry list                       | Holds a RAV entry for variations in a game                                                              |

## Games

These are a means of storing a game. They are provided as a record type **Game** with these fields:

| Field             | Type               | Description                                       |
|:------------------|:-------------------|:--------------------------------------------------|
| Event             | string             | The game was in this event                        |
| Site              | string             | The game was at this site                         |
| Year              | int option         | The game was played in this year but may be None  |
| Month             | int option         | The game was played in this month but may be None |
| Day               | int option         | The game was played in this day but may be None   |
| Round             | string             | The game was in this round                        |
| WhitePlayer       | string             | The game had this player as white                 |
| BlackPlayer       | string             | The game had this player as black                 |
| Result            | GameResult         | The game had this result                          |
| BoardSetup        | Brd option         | The game optionally had this as a starting board  |
| AdditionalInfo    | Map<string,string> | The game had these as extra headers               |
| MoveText          | MoveTextEntry list | The game had these move entries                   |
