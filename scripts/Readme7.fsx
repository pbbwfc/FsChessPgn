#load "setup.fsx"
open FsChess

let board = Board.Start
let mv = "e4"|>Move.FromSan board
let uci = mv|>Move.ToUci
let san = mv|>Move.ToSan board
let mv2 = "Nf3"|>Move.FromSan board
let uci2 = mv2|>Move.ToUci
let san2 = mv2|>Move.ToSan board

let nbd = 
    board
    |>Board.PushSAN "e4"
    |>Board.PushSAN "e5"
    |>Board.PushSAN "Nc3"
    |>Board.PushSAN "Nc6"

let mv3 = "g1e2"|>Move.FromUci nbd
let uci3 = mv3|>Move.ToUci
let san3 = mv3|>Move.ToSan nbd
