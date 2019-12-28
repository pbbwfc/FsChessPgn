#load "setup.fsx"
open FsChess

let board = "r1bqkb1r/pppp1Qpp/2n2n2/4p3/2B1P3/8/PPPP1PPP/RNB1K1NR b KQkq - 0 4"|>Board.FromStr

do 
    board|>Board.ToPng "c:/temp/fools.png" false

