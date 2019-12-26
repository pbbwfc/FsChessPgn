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

        let board = Board.FromStr("r1bqkb1r/pppp1Qpp/2n2n2/4p3/2B1P3/8/PPPP1PPP/RNB1K1NR b KQkq - 0 4")
        let chk1 = board|>Board.IsCheckMate
        //end
        let pgnfile = argv.[0]
        let pgn = pgnfile|>Games.ReadFromFile
        let npgn = pgn|>Games.SetaMoves
        0 // return an integer exit code
