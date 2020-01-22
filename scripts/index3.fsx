#load "setup.fsx"
open FsChess

//Board code
let board = Board.Start

let new_board = board|>Board.PushSAN "e4"|>Board.PushSAN "e5"

//Game code
let game = Game.Start

let new_game = game|>Game.PushSAN "e4"|>Game.PushSAN "e5"|>Game.PushSAN "Nf3"

let reduced_game = new_game|>Game.Pop
