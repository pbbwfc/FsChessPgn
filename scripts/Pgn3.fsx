#load "setup.fsx"
open FsChess.Pgn
open FsChess

let pgn = __SOURCE_DIRECTORY__ + "/data/pgn/french_Qd7.pgn"

//set seach board
let fen = "r1b1kbnr/1pq2ppp/p3p3/8/2BN4/8/PPP2PPP/R1BQ1RK1 w kq - 1 11"
let bd = fen|>Board.FromStr

let gmmvl = Games.FindBoard bd pgn

let len = gmmvl.Length

let stats = gmmvl|>Games.GetStats

