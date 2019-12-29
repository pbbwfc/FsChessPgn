// This is s simple sample to load a pgn file

open FsChessPgn
open FsChess

[<EntryPoint>]
let main argv =
    if argv.Length<>1 then 
        printfn "You need to specify a pgn file, e.g. LoadPgn file.pgn"
        1
    else
        //test code
        let board = Board.Start

        let nbd = 
            board
            |>Board.PushSAN "e4"
            |>Board.PushSAN "e5"
            |>Board.PushSAN "Nc3"
            |>Board.PushSAN "Nc6"

        let mv3 = "g1e2"|>Move.FromUci nbd
        let uci3 = mv3|>Move.ToUci
        let san3 = mv3|>Move.ToSan nbd
        //end
        let pgnfile = argv.[0]
        let pgn = pgnfile|>Games.ReadFromFile
        let npgn = pgn|>Games.SetaMoves
        0 // return an integer exit code
