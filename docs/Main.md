
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

These are _aliased_ to a _short_, with values 0s to 7s for **FileA** to **FileH**.

As an example, **FileC** equals 2s. 