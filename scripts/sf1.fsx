#load "setup.fsx"
open FsChess

let board = Board.Start

let bm = Stockfish.GetBestMove(board,15)

