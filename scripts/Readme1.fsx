#load "setup.fsx"
open FsChessPgn.Data

let board = Board.Start

let mvs = board|>MoveGenerate.AllMoves

let nbd = 
    board
    |>MoveUtil.ApplySAN "e4"
    |>MoveUtil.ApplySAN "e5"
    |>MoveUtil.ApplySAN "Qh5"
    |>MoveUtil.ApplySAN "Nc6"
    |>MoveUtil.ApplySAN "Bc4"
    |>MoveUtil.ApplySAN "Nf6"
    |>MoveUtil.ApplySAN "Qxf7"

let ismate = nbd|>MoveGenerate.IsMate
