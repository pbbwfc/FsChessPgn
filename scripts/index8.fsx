#load "setup.fsx"
open FsChess

let board = Board.Start
let fen = board|>Board.ToStr

let bd = "8/8/8/2k5/4K3/8/8/8 w - - 4 45"|>Board.FromStr
let pc = bd.[C5]
