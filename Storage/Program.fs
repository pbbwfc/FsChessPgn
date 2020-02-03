open System
open FsChessDb

[<EntryPoint>]
let main argv =
    let pgn = "recent10000.pgn"
    let mutable CurPgn = new ChessPack()
    let db = "recent10000.fch"

    let s1 = DateTime.Now
    CurPgn <- Convert.FromPgn pgn
    let e1 = DateTime.Now
    let el1 = e1-s1
    Console.WriteLine("el1: " + el1.ToString())
    let s1 = DateTime.Now
    CurPgn|>Convert.ToPgn("Db_" + pgn)
    let e1 = DateTime.Now
    let el1 = e1-s1
    Console.WriteLine("el1: " + el1.ToString())
    let s1 = DateTime.Now
    CurPgn|>Convert.Save db
    let e1 = DateTime.Now
    let el1 = e1-s1
    Console.WriteLine("el1: " + el1.ToString())
    let s1 = DateTime.Now
    CurPgn <- Convert.Load db
    let e1 = DateTime.Now
    let el1 = e1-s1
    Console.WriteLine("el1: " + el1.ToString())

    let str = FsChessPgn.MoveUtil.Descs (CurPgn.Mvss.[0]|>List.ofArray) FsChess.Board.Start true
 
    0 // return an integer exit code
