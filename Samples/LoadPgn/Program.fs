// This is s simple sample to load a pgn file

open FsChess.Pgn
open FsChess
open System.IO

[<EntryPoint>]
let main argv =
    
    if argv.Length<>1 then 
        printfn "You need to specify a pgn file, e.g. LoadPgn file.pgn"
        1
    else
        //end
        let pgnfile = argv.[0]
        let pgn = pgnfile|>Games.ReadListFromFile
        0 // return an integer exit code
