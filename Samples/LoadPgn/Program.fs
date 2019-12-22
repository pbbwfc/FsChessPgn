// This is s simple sample to load a pgn file

open FsChessPgn

[<EntryPoint>]
let main argv =
    if argv.Length<>1 then 
        printfn "You need to specify a pgn file, e.g. LoadPgn file.pgn"
        1
    else
        let pgnfile = argv.[0]
        let pgn = pgnfile|>Games.ReadFromFile
        let npgn = pgn|>Games.SetaMoves
        0 // return an integer exit code
