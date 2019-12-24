// This is s simple sample to load a pgn file

open FsChessPgn
//open FsChess

[<EntryPoint>]
let main argv =
    if argv.Length<>1 then 
        printfn "You need to specify a pgn file, e.g. LoadPgn file.pgn"
        1
    else
        //test code
        //let board = Board.Start
        //let new_board = board|>Board.PushSAN "e4"|>Board.PushSAN "e5"
        //let game = Game.Start
        //let new_game = game|>Game.PushSAN "e4"|>Game.PushSAN "e5"|>Game.PushSAN "Nf3"
        //end
        let pgnfile = argv.[0]
        let pgn = pgnfile|>Games.ReadFromFile
        let npgn = pgn|>Games.SetaMoves
        0 // return an integer exit code
