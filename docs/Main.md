
[Home](https://pbbwfc.github.io/FsChessPgn)  [Main Chess Features](https://pbbwfc.github.io/FsChessPgn/Main)  [PGN parsing and writing](https://pbbwfc.github.io/FsChessPgn/Pgn)

# Main Chess Features

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

SQUARES = 
       [

        A8; B8; C8; D8; E8; F8; G8; H8;

        A7; B7; C7; D7; E7; F7; G7; H7;

        A6; B6; C6; D6; E6; F6; G6; H6;

        A5; B5; C5; D5; E5; F5; G5; H5;

        A4; B4; C4; D4; E4; F4; G4; H4;

        A3; B3; C3; D3; E3; F3; G3; H3;

        A2; B2; C2; D2; E2; F2; G2; H2;

        A1; B1; C1; D1; E1; F1; G1; H1

        ] 

and

SQUARE_NAMES = ["A8; "B8"; ... "G1"; "H1"]

A utility function **Sq** is provided defined as:

```fsharp
let Sq(f:File,r:Rank) :Square = r * 8s + f
```