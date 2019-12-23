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
