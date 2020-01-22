// This is s simple sample to load a pgn file

open FsChess.Pgn
open FsChess
open System.IO

[<EntryPoint>]
let main argv =
    
    let fol = @"D:\pgns"
    //let f1 = System.IO.Path.Combine(fol,"recent.pgn")
    //let pgn1 = f1|>Games.ReadSeqFromFile
    //let pgn100000 = pgn1|>Seq.take 100000|>Seq.toList
    let f100000 = System.IO.Path.Combine(fol,"recent100000.pgn")
    //pgn100000|>Games.WriteFile f100000
    //f100000|>Games.CreateIndex


    let ipgn100000 = f100000|>Games.ReadIndexListFromFile
    let indx100000 = f100000|>Games.GetIndex



    let fen = "rnb1kbnr/pp1q1ppp/4p3/8/2Bp4/5N2/PPPN1PPP/R1BQK2R w KQkq - 2 7"
    let bd = fen|>Board.FromStr
    let start = System.DateTime.Now
    let mgms = ipgn100000|>Games.FastFindBoard bd indx100000
    let nd = System.DateTime.Now
    let elapsed = nd-start
    
    if argv.Length<>1 then 
        printfn "You need to specify a pgn file, e.g. LoadPgn file.pgn"
        1
    else
        //end
        let pgnfile = argv.[0]
        let pgn = pgnfile|>Games.ReadListFromFile
        0 // return an integer exit code
